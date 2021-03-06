using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Xen.Threading
{
	/// <summary>
	/// Interface to a task action that can be performed on a thread
	/// </summary>
	public interface IAction
	{
		/// <summary>
		/// Performs a task action (usually on a thread)
		/// </summary>
		/// <param name="data"></param>
		void PerformAction(object data);
	}

	[System.Diagnostics.DebuggerStepThrough]
	class TaskSource
	{
		public TaskSource(ThreadPool pool)
		{
			this.pool = pool;
		}
		public readonly ThreadPool pool;
		public IAction action;
		public object data;
		public int id;
	}

	/// <summary>
	/// Provides a callback to wait for a task to complete
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public struct WaitCallback
	{
		readonly TaskSource task;
		readonly int id;

		internal WaitCallback(TaskSource wait, int id)
		{
			this.task = wait;
			this.id = id;
		}

		/// <summary>
		/// Wait for the task to complete, where possible another task will be performed while waiting
		/// </summary>
		public void WaitForCompletion()
		{
			if (task == null)
				return;

			//is task still active?
			if (task.id == this.id)
			{
				//try and run the task now..
				if (task.pool.RunQueueTask(task))
					return;

				while (task.id == this.id)
				{
					//if so, do something else if possible
					if (!task.pool.RunQueueTask())
						Thread.Sleep(0); // otherwise sleep.
				}
			}

			//nullify
			this = new WaitCallback();
		}

		/// <summary>
		/// True if the task has completed
		/// </summary>
		public bool TaskComplete
		{
			get 
			{
				return task == null || task.id != this.id;
			}
		}
	}

	/// <summary>
	/// A thread pool for running tasks on threads at varying priority (such as the <see cref="Application.ThreadPool"/> instance)
	/// </summary>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class ThreadPool : IDisposable
	{
		private readonly object sync = new object();
		private readonly WaitHandle[] waitHandles;
		private readonly WorkUnit[] threads;
		private readonly WaitHandle[] backgroundWaitHandles;
		private readonly WorkUnit[] backgroundUnits;
		private readonly List<WeakReference> temporaryTasks;
		private bool disposed;
		private int temporaryThreadIndex;
		private Stack<TaskSource> taskCache = new Stack<TaskSource>();
		private Queue<TaskSource> taskQueue = new Queue<TaskSource>();
		private int taskIndex;

		/// <summary>
		/// Constructs a thread pool with <see cref="Environment.ProcessorCount"/>-1 threads
		/// </summary>
		public ThreadPool() : 
#if XBOX360
			this(3)
#else
			this(Environment.ProcessorCount-1)
#endif
		{
		}
		/// <summary>
		/// Constructs a thread pool with a specific number of threads
		/// </summary>
		/// <param name="threadCount"></param>
		public ThreadPool(int threadCount)
		{
			this.threads = new WorkUnit[threadCount];
			this.waitHandles = new WaitHandle[threadCount];
			this.backgroundUnits = new WorkUnit[threadCount];
			this.backgroundWaitHandles = new WaitHandle[threadCount];
			this.temporaryTasks = new List<WeakReference>();

			for (int i = 0; i < threadCount; i++)
			{
				threads[i] = new WorkUnit(ThreadPriority.Normal,false,i,this);
				waitHandles[i] = threads[i].GetCompleteHandle();

				backgroundUnits[i] = new WorkUnit(ThreadPriority.BelowNormal, true, i, this);
				backgroundWaitHandles[i] = backgroundUnits[i].GetCompleteHandle();
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		~ThreadPool()
		{
			Dispose();
		}

		/// <summary>
		/// Wait for all non-background tasks to complete
		/// </summary>
		public void WaitForAllTasksComplete()
		{
			if (disposed)
				throw new ObjectDisposedException("this");

			while (RunQueueTask())
			{
			}

			for (int i = 0; i < threads.Length; i++)
			{
				if (!threads[i].IsCurrentThread)
					threads[i].GetCompleteHandle().WaitOne();
			}

			while (RunQueueTask())
			{
			}
		}
		/// <summary>
		/// Wait for all background tasks to complete
		/// </summary>
		public void WaitForAllBackgroundTasksComplete()
		{
			if (disposed)
				throw new ObjectDisposedException("this");

			for (int i = 0; i < backgroundUnits.Length; i++)
			{
				if (!backgroundUnits[i].IsCurrentThread)
					backgroundUnits[i].GetCompleteHandle().WaitOne();
			}

			foreach (WeakReference wr in temporaryTasks)
			{
				WorkUnit unit = wr.Target as WorkUnit;
				if (unit == null)
					continue;
				if (!unit.IsCurrentThread)
					unit.GetCompleteHandle().WaitOne();
			}
		}

		private TaskSource GetSource(IAction action, object data)
		{
			TaskSource task = null;
			if (taskCache.Count > 0)
				task = taskCache.Pop();
			else
				task = new TaskSource(this);
			task.action = action;
			task.data = data;
			task.id = ++taskIndex;
			return task;
		}
		private void PushSource(TaskSource source)
		{
			source.id++;
			source.action = null;
			source.data = null;
			taskCache.Push(source);
		}

		/// <summary>
		/// Run a task on a thread. If no threads are free then the task will be buffered and run at a later time. On single threaded systems the task will run immediately
		/// </summary>
		/// <param name="task">Task to run</param>
		/// <param name="taskData">Optional data to pass to the task</param>
		/// <returns>Returns a callback that can be used to wait for task completion</returns>
		public WaitCallback QueueTask(IAction task, object taskData)
		{
			if (disposed)
				throw new ObjectDisposedException("this");

			if (task == null)
				throw new ArgumentNullException();

			if (this.threads.Length == 0)
			{
				//run it now
				task.PerformAction(taskData);
				return new WaitCallback();
			}

			Monitor.Enter(sync);

			//find a free thread
			int completeIndex = WaitAny(this.waitHandles);
			TaskSource source;
			int id;

			if (completeIndex == -1)
			{
				//no free threads, queue the task for later execution

				//get a free task source
				source = GetSource(task, taskData);
				id = source.id;
				taskQueue.Enqueue(source);
				Monitor.Exit(sync);
				return new WaitCallback(source,id);//return a WaitCallback wrapping the task
			}

			//give the task to the free thread
			WorkUnit unit = threads[completeIndex];
			source = GetSource(task, taskData);
			id = source.id;
			unit.Run(source); // run it now

			Monitor.Exit(sync);
			return new WaitCallback(source,id);
		}

		//find a waiting task and run it
		internal bool RunQueueTask()
		{
			TaskSource source = null;
			Monitor.Enter(sync);
			if (taskQueue.Count > 0)
				source = taskQueue.Dequeue();
			Monitor.Exit(sync);

			if (source != null)
				return RunQueueTask(source);

			return false;
		}

		internal bool RunQueueTask(TaskSource source)
		{
			IAction acton = Interlocked.Exchange(ref source.action, null);

			if (acton != null) // action gets nulled when the task is being run...
			{
				acton.PerformAction(source.data);
				ClearTask(source);
				return true;
			}
			return false;
		}

		internal void ClearTask(TaskSource source)
		{
			Monitor.Enter(sync);
			this.PushSource(source);
			Monitor.Exit(sync);
		}

		//WaitHandle.WaitAny isn't supported on xbox, and it allocates!
		internal int WaitAny(WaitHandle[] handles)
		{	
			for (int i = 0; i <handles.Length ; i++)
				if (handles[i].WaitOne(0, false))
					return i;
			return -1;
		}

		/// <summary>
		/// Run a task on a low-priority background thread
		/// </summary>
		/// <param name="task">Task to run</param>
		/// <param name="taskData">Optional data to pass to the task</param>
		/// <returns>Returns a callback to wait for task completion</returns>
		public WaitCallback RunBackgroundTask(IAction task, object taskData)
		{
			if (disposed)
				throw new ObjectDisposedException("this");

			if (task == null)
				throw new ArgumentNullException();

			Monitor.Enter(sync);

			int completeIndex = WaitAny(backgroundWaitHandles);

			TaskSource source;
			int id;
			WorkUnit unit;
			if (completeIndex == -1)
			{
				//no free threads, create a new thread
				source = GetSource(task, taskData);
				id = source.id;
				Monitor.Exit(sync);

				unit = new WorkUnit(ThreadPriority.BelowNormal,true, temporaryThreadIndex++,this);
				unit.SetTemporary();
				unit.Run(source);

				lock (temporaryTasks)
				{
					for (int i = 0; i < temporaryTasks.Count; i++)
					{
						if (temporaryTasks[i].Target == null)
						{
							temporaryTasks[i].Target = unit;
							return new WaitCallback(source,id);
						}
					}
					temporaryTasks.Add(new WeakReference(unit));
					return new WaitCallback(source,id);
				}
			}

			unit = backgroundUnits[completeIndex];
			source = GetSource(task, taskData);
			id = source.id;
			unit.Run(source);

			Monitor.Exit(sync);
			return new WaitCallback(source,id);
		}

		/// <summary>
		/// Gets the number of worker threads created
		/// </summary>
		public int ThreadCount
		{
			get { return this.threads.Length; }
		}

	
		internal class WorkUnit : IDisposable
		{
			readonly private AutoResetEvent start, running;
			readonly private ManualResetEvent complete;
			private TaskSource task;
			private readonly Thread thread;
			private bool temporary;
			private ThreadPool parent;
#if XBOX360
			private int threadAffinity;
			private static int[] threadIds = new int[] { 3, 4, 5 };
#else
			private System.Globalization.CultureInfo culture; 
#endif

			public WorkUnit(ThreadPriority priority, bool background, int index, ThreadPool parent)
			{
				this.parent = parent;
				complete = new ManualResetEvent(false);
				start = new AutoResetEvent(false);
				running = new AutoResetEvent(false);

				thread = new Thread(Process);
				thread.Priority = priority;
				thread.IsBackground = background;

#if XBOX360
				threadAffinity = threadIds[index % threadIds.Length];
#else
				culture = Thread.CurrentThread.CurrentCulture;
#endif
				thread.Start();
				complete.WaitOne();
			}

			public ThreadPool ThreadPool
			{
				get { return parent; }
			}

			public void SetTemporary()
			{
				temporary = true;
				thread.IsBackground = true;
			}

			public bool IsCurrentThread
			{
				get { return Thread.CurrentThread == thread; }
			}

			public void Dispose()
			{	
				complete.WaitOne();
				task = null;
				start.Set();
				thread.Join();
			}

			public WaitHandle GetCompleteHandle()
			{
				return complete;
			}

			public void WaitForCompletion()
			{
				complete.WaitOne();
			}

			public bool PollCompleted()
			{
				return complete.WaitOne(0,false);
			}

			public void Run(TaskSource task)
			{
				complete.WaitOne();

				this.task = task;

				start.Set();
				running.WaitOne();
			}
			
			void Process()
			{
#if XBOX360
				thread.SetProcessorAffinity(new int[] { threadAffinity });
#else
				//match culture with the creation thread
				Thread.CurrentThread.CurrentCulture = culture;
#endif

				while (!temporary)
				{
					complete.Set();
					start.WaitOne();

					TaskSource task = this.task;

					if (task == null)
						break;

					this.task = null;

					complete.Reset();
					running.Set();

					IAction action = Interlocked.Exchange(ref task.action, null);

					if (action != null) // set action to null when a task in in flight
					{
						action.PerformAction(task.data);
						parent.ClearTask(task);
					}

					if (!temporary)
					{
						while (parent.RunQueueTask())
							continue;
					}
				}
				complete.Set();
			}
		}

		/// <summary>
		/// Dispose the thread pool
		/// </summary>
		public void Dispose()
		{
			if (disposed)
				return;

			disposed = true;

			for (int i = 0; i < threads.Length; i++)
				threads[i].Dispose();
			for (int i = 0; i < backgroundUnits.Length; i++)
				backgroundUnits[i].Dispose();

			foreach (WeakReference wr in temporaryTasks)
			{
				WorkUnit unit = wr.Target as WorkUnit;
				if (unit != null)
					unit.Dispose();
			}

			temporaryTasks.Clear();
			GC.SuppressFinalize(this);
		}
	}
}
