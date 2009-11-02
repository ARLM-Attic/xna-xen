using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xen.Graphics;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Xen
{
	/// <summary>
	/// Interface to an object that wishes to load and unload content through an XNA <see cref="ContentManager"/>, or handle implementation specific content load/unload logic, for use with the <see cref="ContentRegister"/> class (such as the <see cref="Application.Content"/> instance)
	/// </summary>
	/// <remarks>
	/// <para>This interface is intended as a lightweight replacement for the content loading methods found in <see cref="Microsoft.Xna.Framework.DrawableGameComponent"/></para>
	/// <para>All XNA content should be loaded within these methods, with the exception of Textures returned by <see cref="DrawTargetTexture2D.GetTexture()"/> and <see cref="DrawTargetTextureCube.GetTexture()"/>.
	/// <br/>These textures become invalid after a call to <see cref="UnloadContent"/>, they are guarenteed to be valid again during the subsequent <see cref="LoadContent"/> call.</para>
	/// <para>When registered with a <see cref="ContentRegister"/> object, a weak reference of the instance will be stored. This will prevent the object from being kept alive in an unexpected way</para>
	/// </remarks>
	public interface IContentOwner
	{
		/// <summary>
		/// Load all XNA <see cref="ContentManager"/> content, or get textures from <see cref="DrawTargetTexture2D"/> or <see cref="DrawTargetTextureCube"/> objects.
		/// </summary>
		/// <param name="content"></param>
		/// <param name="state">Current state of the application (restricted access)</param>
		/// <param name="manager">XNA content manage</param>
		void LoadContent(ContentRegister content, DrawState state, Microsoft.Xna.Framework.Content.ContentManager manager);
		/// <summary>
		/// Unload all XNA <see cref="ContentManager"/> content, or null textures from <see cref="DrawTargetTexture2D"/> or <see cref="DrawTargetTextureCube"/> objects.
		/// </summary>
		/// <param name="content"></param>
		/// <param name="state">Current state of the application (restricted access)</param>
		void UnloadContent(ContentRegister content, DrawState state);
	}

	/// <summary>
	/// Interface to a <see cref="ContentRegister"/>
	/// </summary>
	public interface IContentRegister : IDisposable
	{
		/// <summary>
		/// Register an <see cref="IContentOwner"/> instance with this content manager
		/// </summary>
		/// <param name="owner"></param>
		void Add(IContentOwner owner);
		/// <summary>
		/// Unregister an <see cref="IContentOwner"/> instance with this content manager. NOTE: Instances are stored by weak reference and do not need to be manually removed (see remarks)
		/// </summary>
		/// <remarks><para>Instances are stored by weak reference, so this method should only be called when removing the object early is desired.</para>
		/// <para>Instances will not be kept alive when added, and do not need to be removed to make sure they are garbage collected</para></remarks>
		/// <param name="owner"></param>
		void Remove(IContentOwner owner);
	}

	/// <summary>
	/// Wrapper on an XNA <see cref="ContentManager"/>. Keeps track of <see cref="IContentOwner"/> instatnces by <see cref="WeakReference">weak reference</see>, calling Load/Unload content.
	/// </summary>
	/// <remarks>The <see cref="Application"/> class has its own instance of <see cref="Application.Content"/></remarks>
#if !DEBUG_API
	[System.Diagnostics.DebuggerStepThrough]
#endif
	public sealed class ContentRegister : IContentRegister
	{

		/// <summary>
		/// Construct a object content manager, creating an XNA content manager
		/// </summary>
		/// <param name="application">Application instance</param>
		public ContentRegister(Application application) : this(application,((Game)application).Content.RootDirectory)
		{
		}

		/// <summary>
		/// Construct a object content manager, creating an XNA content manager
		/// </summary>
		/// <param name="application">Application instance</param>
		/// <param name="rootDirectory">Root content directory</param>
		public ContentRegister(Application application, string rootDirectory)
			: this(application, new ContentManager(((Game)application).Services, rootDirectory))
		{
		}

		/// <summary>
		/// Construct a object content manager
		/// </summary>
		/// <param name="application">Application instance</param>
		/// <param name="manager">XNA ContentManager instatnce</param>
		internal ContentRegister(Application application, ContentManager manager)
		{
			if (application == null || manager == null)
				throw new ArgumentNullException();

			this.application = application;
			service = (IGraphicsDeviceService)manager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));

			if (service == null)
				throw new ArgumentException("manager.Services.IGraphicsDeviceService not found");

			created = service.GraphicsDevice != null;

			service.DeviceDisposing += new EventHandler(DeviceResetting);
			service.DeviceResetting += new EventHandler(DeviceResetting);
			service.DeviceCreated += new EventHandler(DeviceCreated);
			service.DeviceReset += new EventHandler(DeviceReset);

			this.manager = manager;
		}

		void DeviceResetting(object sender, EventArgs e)
		{
			CallUnload();
		}

		void DeviceReset(object sender, EventArgs e)
		{
			CallLoad();
		}

		void DeviceCreated(object sender, EventArgs e)
		{
			created = true;
			CallLoad();
		}

		private List<WeakReference> items = new List<WeakReference>();
		private List<WeakReference> highPriorityItems = new List<WeakReference>();
		private Stack<WeakReference> nullReferences = new Stack<WeakReference>();
		private List<WeakReference> buffer;
		private ContentManager manager;
		private IGraphicsDeviceService service;
		private Application application;
		private bool created;
		private object sync = new object();
		private List<IContentOwner> delayedRemoveList = new List<IContentOwner>();
		private List<IContentOwner> delayedAddList = new List<IContentOwner>();

		/// <summary>
		/// Register an <see cref="IContentOwner"/> instance with this content manager
		/// </summary>
		/// <param name="owner"></param>
		public void Add(IContentOwner owner)
		{
			if (Monitor.TryEnter(sync))
			{
				try
				{
					if (manager == null)
						throw new ObjectDisposedException("this");
					if (owner == null)
						throw new ArgumentNullException();
					if (nullReferences.Count > 0)
					{
						WeakReference wr = nullReferences.Pop();
						wr.Target = owner;
						items.Add(wr);
					}
					else
						items.Add(new WeakReference(owner));

					if (created)
						owner.LoadContent(this,application.GetProtectedDrawState(service.GraphicsDevice), manager);
				}
				finally
				{
					Monitor.Exit(sync);
				}
			}
			else
			{
				lock (delayedAddList)
					delayedAddList.Add(owner);
			}
		}
		void ProcessDelayed()
		{
			lock (delayedAddList)
			{
				foreach (IContentOwner owner in delayedAddList)
					Add(owner);
				delayedAddList.Clear();
			}
			lock (delayedRemoveList)
			{
				foreach (IContentOwner owner in delayedRemoveList)
					Remove(owner);
				delayedRemoveList.Clear();
			}
		}
		internal void AddHighPriority(IContentOwner owner)
		{
			lock (sync)
			{
				if (manager == null)
					throw new ObjectDisposedException("this");
				if (owner == null)
					throw new ArgumentNullException();
				if (nullReferences.Count > 0)
				{
					WeakReference wr = nullReferences.Pop();
					wr.Target = owner;
					highPriorityItems.Add(wr);
				}
				else
					highPriorityItems.Add(new WeakReference(owner));

				if (created)
					owner.LoadContent(this, application.GetProtectedDrawState(service.GraphicsDevice), manager);

				ProcessDelayed();
			}
		}

		/// <summary>
		/// Unregister an <see cref="IContentOwner"/> instance with this content manager. NOTE: Instances are stored by weak reference and do not need to be manually removed (see remarks)
		/// </summary>
		/// <remarks><para>Instances are stored by weak reference, so this method should only be called when removing the object early is desired.</para>
		/// <para>Instances will not be kept alive when added, and do not need to be removed to make sure they are garbage collected</para></remarks>
		/// <param name="owner"></param>
		public void Remove(IContentOwner owner)
		{
			if (Monitor.TryEnter(sync))
			{
				try
				{
					foreach (WeakReference wr in this.items)
					{
						if (wr.Target == owner)
						{
							if (this.items.Count > 1)
							{
								wr.Target = this.items[this.items.Count - 1].Target;
								this.items[this.items.Count - 1].Target = null;
							}
							else
								wr.Target = null;
							break;
						}
					}
					nullReferences.Push(this.items[this.items.Count - 1]);
					this.items.RemoveAt(this.items.Count - 1);
				}
				finally
				{
					Monitor.Exit(sync);
				}
			}
			else
			{
				lock (delayedRemoveList)
					delayedRemoveList.Add(owner);
			}
		}

		void CallLoad()
		{
			lock (sync)
			{
				if (buffer == null)
					buffer = new List<WeakReference>();
				
				DrawState state = application.GetProtectedDrawState(service.GraphicsDevice);
				for (int i = 0; i < highPriorityItems.Count; i++)
				{
					IContentOwner loader = highPriorityItems[i].Target as IContentOwner;
					if (loader != null)
						loader.LoadContent(this, state, manager);
					else
					{
						nullReferences.Push(highPriorityItems[i]);
						highPriorityItems.RemoveAt(i--);
					}
				}

				foreach (WeakReference wr in items)
				{
					IContentOwner loader = wr.Target as IContentOwner;
					if (loader != null)
						buffer.Add(wr);
					else
						nullReferences.Push(wr);
				}
				foreach (WeakReference wr in buffer)
				{
					IContentOwner loader = wr.Target as IContentOwner;
					if (loader != null)
					{
						loader.LoadContent(this, state, manager);
					}
				}

				List<WeakReference> list = items;
				items = buffer;
				buffer = list;

				buffer.Clear();

				ProcessDelayed();
			}
		}
		void CallUnload()
		{
			lock (sync)
			{
				if (buffer == null)
					buffer = new List<WeakReference>();

				DrawState state = application.GetProtectedDrawState(service.GraphicsDevice);
				for (int i = 0; i < highPriorityItems.Count; i++)
				{
					IContentOwner loader = highPriorityItems[i].Target as IContentOwner;
					if (loader != null)
						loader.UnloadContent(this, state);
					else
					{
						nullReferences.Push(highPriorityItems[i]);
						highPriorityItems.RemoveAt(i--);
					}
				}

				foreach (WeakReference wr in items)
				{
					IContentOwner loader = wr.Target as IContentOwner;
					if (loader != null)
						buffer.Add(wr);
					else
						nullReferences.Push(wr);
				}
				foreach (WeakReference wr in buffer)
				{
					IContentOwner loader = wr.Target as IContentOwner;
					if (loader != null)
						loader.UnloadContent(this, state);
				}

				List<WeakReference> list = items;
				items = buffer;
				buffer = list;

				buffer.Clear();

				ProcessDelayed();
			}
		}

		/// <summary>
		/// Dispose the Content manager and unload all instances
		/// </summary>
		public void Dispose()
		{
			if (items != null)
			{
				CallUnload();
				items.Clear();
			}
			if (service != null)
			{
				service.DeviceDisposing -= new EventHandler(DeviceResetting);
				service.DeviceResetting -= new EventHandler(DeviceResetting);
				service.DeviceCreated -= new EventHandler(DeviceCreated);
				service.DeviceReset -= new EventHandler(DeviceReset);
				service = null;
			}
			if (manager != null)
			{
				manager.Dispose();
				manager = null;
			}
			buffer = null;
			items = null;
		}

		/// <summary>
		/// Gets or sets the ContentManager root directory.
		/// </summary>
		public string RootDirectory
		{
			get { return this.manager.RootDirectory; }
			set { this.manager.RootDirectory = value; }
		}
	}
}
