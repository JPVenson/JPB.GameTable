using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JPB.GameTable.UI.Helper;
using Unity;

namespace JPB.GameTable.UI.Unity
{
	public static class IoC
	{
		static IoC()
		{
			_creationAwaiter = new ConcurrentDictionary<Type, ManualResetEventSlim>();
		}

		public static UnityContainer Container { get; private set; }
		private static readonly ConcurrentDictionary<Type, ManualResetEventSlim> _creationAwaiter;

		public static void Init(UnityContainer container)
		{
			Container = container;
		}

		public static void RegisterInstance(object instance, Type impl = null)
		{
			if (impl == null)
			{
				impl = instance.GetType();
			}
			Container.RegisterInstance(impl, instance);

			ManualResetEventSlim waiter;
			if (_creationAwaiter.TryRemove(impl, out waiter))
			{
				waiter.Set();
			}
		}

		public static void Register(Type forType, Type impl)
		{
			Container.RegisterType(forType, impl);
			ManualResetEventSlim waiter;
			if (_creationAwaiter.TryRemove(forType, out waiter))
			{
				waiter.Set();
			}
		}

		public static void Register(Type impl)
		{
			Register(impl, impl);
		}

		public static T Resolve<T>() where T : class
		{
			return Container.Resolve(typeof(T)) as T;
		}

		public static async Task<T> ResolveLater<T>() where T : class
		{
			if (Container.IsRegistered(typeof(T)))
			{
				return Container.Resolve(typeof(T)) as T;
			}
			var manualResetEventSlim = _creationAwaiter.GetOrAdd(typeof(T), f => new ManualResetEventSlim());
			await manualResetEventSlim.WaitHandle.WaitOneAsync();
			return Resolve<T>();
		}

		public static IEnumerable<T> ResolveMany<T>() where T : class
		{
			return Container.ResolveAll<T>();
		}
	}
}
