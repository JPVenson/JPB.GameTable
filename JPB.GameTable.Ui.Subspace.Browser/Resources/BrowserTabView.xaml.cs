using System;
using System.Windows.Controls;
using CefSharp;
using JPB.GameTable.Ui.Subspace.Browser.ViewModel;

namespace JPB.GameTable.Ui.Subspace.Browser.Resources
{
	public class DownloadHandler : IDownloadHandler
	{
		public event EventHandler<DownloadItem> OnBeforeDownloadFired;

		public event EventHandler<DownloadItem> OnDownloadUpdatedFired;

		public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
		{
			var handler = OnBeforeDownloadFired;
			if (handler != null)
			{
				handler(this, downloadItem);
			}

			if (!callback.IsDisposed)
			{
				using (callback)
				{
					callback.Continue(downloadItem.SuggestedFileName, showDialog: true);
				}
			}
		}

		public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
		{
			var handler = OnDownloadUpdatedFired;
			if (handler != null)
			{
				handler(this, downloadItem);
			}
		}
	}

	public partial class BrowserTabView : UserControl
	{
		static BrowserTabView()
		{
			CefSettings settings = new CefSettings();
			settings.UserAgent = "Mozilla/5.0 (Windows; Android 6.0.1; vivo 1603 Build/MMB29M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.83 Mobile Safari/537.36";
			Cef.Initialize(settings);
		}

		public BrowserTabView()
		{
			InitializeComponent();

			var downloadHandler = new DownloadHandler();
			downloadHandler.OnBeforeDownloadFired += OnBeforeDownloadFired;
			downloadHandler.OnDownloadUpdatedFired += OnDownloadUpdatedFired;
			browser.DownloadHandler = downloadHandler;
			browser.Loaded += Browser_Loaded;
			browser.LoadingStateChanged += Browser_LoadingStateChanged;

			browser.LoadError += (sender, args) =>
			{
				// Don't display an error for downloaded files.
				if (args.ErrorCode == CefErrorCode.Aborted)
				{
					return;
				}

				// Don't display an error for external protocols that we allow the OS to
				// handle. See OnProtocolExecution().
				//if (args.ErrorCode == CefErrorCode.UnknownUrlScheme)
				//{
				//	var url = args.Frame.Url;
				//	if (url.StartsWith("spotify:"))
				//	{
				//		return;
				//	}
				//}

				// Display a load error message.
				var errorBody = string.Format("<html><body bgcolor=\"white\"><h2>Failed to load URL {0} with error {1} ({2}).</h2></body></html>",
											  args.FailedUrl, args.ErrorText, args.ErrorCode);

				args.Frame.LoadStringForUrl(errorBody, args.FailedUrl);
			};
		}

		private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
		{
			if (!e.Browser.IsLoading)
			{
				browser.Dispatcher.BeginInvoke(new Action(() =>
				{
					browser.ZoomLevel = -3.0;
					browser.DpiScaleFactor = 1;
					browser.GetBrowserHost().NotifyScreenInfoChanged();
				}));
			}
		}

		private void Browser_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
		}

		private void OnBeforeDownloadFired(object sender, DownloadItem e)
		{
			this.UpdateDownloadAction("OnBeforeDownload", e);
		}

		private void OnDownloadUpdatedFired(object sender, DownloadItem e)
		{
			this.UpdateDownloadAction("OnDownloadUpdated", e);
		}

		private void UpdateDownloadAction(string downloadAction, DownloadItem downloadItem)
		{
			this.Dispatcher.InvokeAsync(() =>
			{
				var viewModel = (BrowserTabViewModel)this.DataContext;
				viewModel.LastDownloadAction = downloadAction;
				viewModel.DownloadItem = downloadItem;
			});
		}
	}
}
