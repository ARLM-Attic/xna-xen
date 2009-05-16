using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace Xen.Graphics.ShaderSystem.CustomTool
{
	//storage for a static graphics device, used for compiling shaders
	public static class Graphics
	{
		private static GraphicsDevice device;
		private static DeviceType deviceType;
		private static Form hiddenWindow;
		private readonly static List<Texture> textureCache;
		private readonly static Dictionary<Type, SurfaceFormat> textureFormatDict;

		static Graphics()
		{
			textureCache = new List<Texture>();
			textureFormatDict = new Dictionary<Type, SurfaceFormat>();

			hiddenWindow = new Form();
			PresentationParameters present = new PresentationParameters();
			present.BackBufferCount = 0;
			
			try
			{
				device = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.NullReference, hiddenWindow.Handle, present);
				deviceType = DeviceType.NullReference;
			}
			catch
			{
				try
				{
					device = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Reference, hiddenWindow.Handle, present);
					deviceType = DeviceType.Reference;
				}
				catch
				{
					device = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, DeviceType.Hardware, hiddenWindow.Handle, present);
					deviceType = DeviceType.Hardware;
				}
			}
		}

		public static GraphicsDevice GraphicsDevice
		{
			get
			{
				return device;
			}
		}

		public static void EndGetTempTexture(Texture texture)
		{
			textureCache.Add(texture);
		}
		public static Texture BeginGetTempTexture(Type type)
		{
			if (type == typeof(Texture))
				type = typeof(Texture2D);

			for (int i = 0; i < textureCache.Count; i++)
			{
				if (textureCache[i].GetType() == type)
				{
					Texture t = textureCache[i];
					textureCache.RemoveAt(i);
					return t;
				}
			}

			//create.

			SurfaceFormat format = GetSupportedTextureFormat(type);

			Texture texture = null;

			if (type == typeof(Texture2D))
				texture = new Texture2D(GraphicsDevice, 2, 2, 1, TextureUsage.None, format);
			if (type == typeof(Texture3D))
				texture = new Texture3D(GraphicsDevice, 2, 2, 2, 1, TextureUsage.None, format);
			if (type == typeof(TextureCube))
				texture = new TextureCube(GraphicsDevice, 2, 1, TextureUsage.None, format);

			return texture;
		}


		static SurfaceFormat GetSupportedTextureFormat(Type type)
		{
			SurfaceFormat surface = SurfaceFormat.Unknown;

			if (textureFormatDict.TryGetValue(type, out surface))
				return surface;

			//find a supported format to use.

			GraphicsAdapter adapter = GraphicsAdapter.DefaultAdapter;

			SurfaceFormat[] formats = (SurfaceFormat[])Enum.GetValues(typeof(SurfaceFormat));
			ResourceType resType = ResourceType.Texture2D;

			if (type == typeof(Texture3D))
				resType = ResourceType.Texture3D;
			if (type == typeof(TextureCube))
				resType = ResourceType.TextureCube;

			surface = SurfaceFormat.Color;

			foreach (SurfaceFormat format in formats)
			{
				if (format != SurfaceFormat.Unknown)
				{
					//find a format that works for PS and VS
					if (adapter.CheckDeviceFormat(deviceType, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.None, resType, format) &&
						adapter.CheckDeviceFormat(deviceType, SurfaceFormat.Bgr32, TextureUsage.None, QueryUsages.VertexTexture, resType, format))
					{
						surface = format;
						break;
					}
				}
			}
			
			//ohh well. hope nothing breaks.
			textureFormatDict.Add(type, surface);

			return surface;
		}
	}
}
