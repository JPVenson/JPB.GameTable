using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPB.GameTable.Shared.Services;
using JPB.GameTable.UI.Database;
using JPB.GameTable.UI.Resources;
using JPB.GameTable.UI.Services;
using Unity;

namespace JPB.GameTable.UI.Unity
{
	public class UnityConfig
	{
		public UnityConfig()
		{
			
		}

		public static void Init()
		{
			IoC.Init(new UnityContainer());
			IoC.RegisterInstance(new DialogService());
			IoC.RegisterInstance(new CommandService());
			IoC.RegisterInstance(new GameAreaFocusService());
			IoC.RegisterInstance(new SubSpaceManager());
			IoC.RegisterInstance(new DbEntities());
			IoC.RegisterInstance(new ChatService());
		}
	}
}
