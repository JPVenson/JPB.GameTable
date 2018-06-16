using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JPB.GameTable.UI.ViewModel.SubSpaces;

namespace JPB.GameTable.UI.Services
{
	public class SubSpaceManager
	{
		public SubSpaceManager()
		{
			SubSpaces = new ConcurrentDictionary<SubSpaceAttribute, Type>();

			var subspaces = AppDomain.CurrentDomain.GetAssemblies().SelectMany(e => e.GetTypes())
									 .Where(e => !e.IsAbstract && e.IsClass && typeof(SubSpaceBase).IsAssignableFrom(e))
									 .ToDictionary(e => e.GetCustomAttribute<SubSpaceAttribute>());
			SubSpaces = subspaces;
		}

		public IDictionary<SubSpaceAttribute, Type> SubSpaces { get; set; }

		public void AddSubSpace(SubSpaceAttribute metaData, Type type)
		{
			SubSpaces.Add(metaData, type);
		}
	}
}
