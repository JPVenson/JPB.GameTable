using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JPB.GameTable.UI.Helper
{
	public static class WaitHandleExtentions
	{
		public static Task WaitOneAsync(this WaitHandle waitHandle)
		{
			if (waitHandle == null)
				throw new ArgumentNullException("waitHandle");

			var tcs = new TaskCompletionSource<bool>();
			var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
			delegate { tcs.TrySetResult(true); }, null, -1, true);
			var t = tcs.Task;
			t.ContinueWith((antecedent) => rwh.Unregister(null));
			return t;
		}
	}

	public class AsyncLock
	{
		private readonly Task<IDisposable> _releaserTask;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly IDisposable _releaser;
		private static readonly ConcurrentDictionary<object, AsyncLock> _locks = new ConcurrentDictionary<object, AsyncLock>();

		public static AsyncLock FromObject(object key)
		{
			return _locks.GetOrAdd(key, (handle) => new AsyncLock(handle));
		}

		public AsyncLock()
		{
			var handle = FromObject(this);
			_releaser = handle._releaser;
			_releaserTask = handle._releaserTask;
		}

		private AsyncLock(object handle)
		{
			_releaser = new Releaser(_semaphore, handle);
			_releaserTask = Task.FromResult(_releaser);
		}
		public IDisposable Lock()
		{
			_semaphore.Wait();
			return _releaser;
		}
		public Task<IDisposable> LockAsync()
		{
			var waitTask = _semaphore.WaitAsync();
			return waitTask.IsCompleted
				? _releaserTask
				: waitTask.ContinueWith(
				(_, releaser) => (IDisposable)releaser,
				_releaser,
				CancellationToken.None,
				TaskContinuationOptions.ExecuteSynchronously,
				TaskScheduler.Default);
		}
		private class Releaser : IDisposable
		{
			private readonly SemaphoreSlim _semaphore;
			private readonly object _handle;

			public Releaser(SemaphoreSlim semaphore, object handle)
			{
				_semaphore = semaphore;
				_handle = handle;
			}
			public void Dispose()
			{
				_semaphore.Release();
				AsyncLock handler;
				_locks.TryRemove(_handle, out handler);
			}
		}
	}
}
