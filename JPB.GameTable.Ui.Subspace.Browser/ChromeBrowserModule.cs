using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JPB.GameTable.Ui.Contracts.SubSpaces;
using JPB.GameTable.Ui.Subspace.Browser.Resources;
using JPB.GameTable.Ui.Subspace.Browser.ViewModel;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;
using JPB.GameTable.UI.ViewModel.SubSpaces;

namespace JPB.GameTable.Ui.Subspace.Browser
{
	[SubSpaceExport]
	public class ChromeBrowserModule : ISubSpaceExport
	{
		/// <inheritdoc />
		public void OnStart()
		{
			Application.Current.MainWindow.Resources.MergedDictionaries.Add(new Resource());
			IoC.Resolve<SubSpaceManager>().AddSubSpace(new SubSpaceAttribute("Browser"), typeof(BrowserSubSpaceViewModel));
		}
	}
}
