using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JPB.GameTable.Ui.Contracts.SubSpaces;
using JPB.GameTable.Ui.Subspace.Browser;
using JPB.GameTable.UI.Services;
using JPB.GameTable.UI.Unity;

namespace JPB.GameTable.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		/// <inheritdoc />
		protected override void OnStartup(StartupEventArgs e)
		{
			//KeyHandler.RegisterKeyHandler();
			UnityConfig.Init();
			base.OnStartup(e);
			var interceptMouse = new InputInterceptor();
			interceptMouse.Start();
			IoC.RegisterInstance(interceptMouse);
			ShutdownMode = ShutdownMode.OnMainWindowClose;

			//var enumerable = AppDomain.CurrentDomain.GetAssemblies().SelectMany(f => f.GetTypes())
			//                          .Where(f => f.GetCustomAttribute<SubSpaceExportAttribute>() != null);

			//foreach (var type in enumerable)
			//{
			//	var subSpaceExport = Activator.CreateInstance(type) as ISubSpaceExport;
			//	if (subSpaceExport != null)
			//	{
			//		subSpaceExport.OnStart();
			//	}
			//}
		}

		/// <inheritdoc />
		protected override void OnExit(ExitEventArgs e)
		{
			//KeyHandler.DetachKeyboardHook();
			IoC.Resolve<InputInterceptor>().Stop();
			base.OnExit(e);
		}
	}
}
