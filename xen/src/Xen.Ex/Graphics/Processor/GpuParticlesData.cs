using System;
using System.Collections.Generic;
using System.Text;
using Xen.Graphics.State;
using Xen.Graphics;
using Xen.Camera;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Xen.Ex.Graphics.Display;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics2D;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Xen.Ex.Graphics.Content;

namespace Xen.Ex.Graphics.Processor
{
	//byte data for gpu vertex shaders (which are common for all particle systems)
	static class GpuVertexShaderData
	{
#if XBOX360
		public static byte[] VS_FRAME		= new byte[] { 16,42,17,1,0,0,1,24,0,0,0,156,0,0,0,0,0,0,0,36,0,0,0,0,0,0,0,224,0,0,0,0,0,0,0,0,0,0,0,184,0,0,0,28,0,0,0,170,255,254,3,0,0,0,0,3,0,0,0,28,0,0,16,0,0,0,0,163,0,0,0,88,0,2,0,0,0,4,0,0,0,0,0,108,0,0,0,0,0,0,0,124,0,2,0,6,0,1,0,0,0,0,0,136,0,0,0,0,0,0,0,152,0,2,0,4,0,1,0,0,0,0,0,136,0,0,0,0,87,111,114,108,100,86,105,101,119,80,114,111,106,101,99,116,105,111,110,0,0,3,0,3,0,4,0,4,0,1,0,0,0,0,0,0,114,97,110,100,83,105,122,101,0,171,171,171,0,1,0,3,0,1,0,4,0,1,0,0,0,0,0,0,116,97,114,103,101,116,83,105,122,101,0,118,115,95,51,95,48,0,50,46,48,46,55,54,56,48,46,48,0,171,171,171,0,0,0,0,0,0,0,156,0,1,0,1,0,0,0,0,0,0,0,0,0,0,16,33,0,0,0,1,0,0,0,2,0,0,0,1,0,0,2,144,0,16,0,3,0,48,80,4,0,0,240,80,0,0,16,11,48,5,32,3,0,0,18,0,194,0,0,0,0,0,64,5,0,0,18,0,196,0,0,0,0,0,48,9,0,0,34,0,0,0,0,0,5,248,16,0,0,0,6,136,0,0,0,0,5,248,0,0,0,0,15,200,0,0,0,0,200,1,128,62,0,167,167,0,175,1,0,0,200,2,128,62,0,167,167,0,175,1,1,0,200,4,128,62,0,167,167,0,175,1,2,0,200,8,128,62,0,167,167,0,175,1,3,0,200,12,0,0,0,172,177,0,33,4,6,0,200,12,0,0,0,6,172,0,225,0,0,0,200,15,128,0,0,0,0,0,226,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,};
		public static byte[] VS_ONCE		= new byte[] { 16,42,17,1,0,0,1,88,0,0,1,84,0,0,0,0,0,0,0,36,0,0,0,212,0,0,0,252,0,0,0,0,0,0,0,0,0,0,0,172,0,0,0,28,0,0,0,158,255,254,3,0,0,0,0,3,0,0,0,28,0,0,16,0,0,0,0,151,0,0,0,88,0,2,0,7,0,240,0,0,0,0,0,96,0,0,0,0,0,0,0,112,0,2,0,6,0,1,0,0,0,0,0,124,0,0,0,0,0,0,0,140,0,2,0,4,0,1,0,0,0,0,0,124,0,0,0,0,105,110,100,105,99,101,115,0,0,1,0,3,0,1,0,4,0,240,0,0,0,0,0,0,114,97,110,100,83,105,122,101,0,171,171,171,0,1,0,3,0,1,0,4,0,1,0,0,0,0,0,0,116,97,114,103,101,116,83,105,122,101,0,118,115,95,51,95,48,0,50,46,48,46,55,54,56,48,46,48,0,171,171,171,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,20,0,252,0,16,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,64,0,0,1,20,0,81,0,1,0,0,0,0,0,0,0,0,0,0,96,198,0,0,0,1,0,0,0,1,0,0,0,6,0,0,2,144,0,0,0,4,0,0,240,80,0,1,241,81,0,2,242,82,0,3,243,83,0,4,244,84,0,5,245,85,0,0,16,17,0,0,16,15,0,0,16,18,0,0,16,19,0,0,16,20,0,0,16,21,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,63,128,0,0,64,0,0,0,191,128,0,0,0,0,0,0,16,1,16,4,0,0,18,0,194,0,0,0,0,0,96,5,48,11,18,0,18,0,0,0,0,0,0,0,96,14,196,0,18,0,0,0,0,0,32,20,0,0,34,0,0,0,0,0,5,248,0,0,0,0,15,199,0,0,0,0,48,65,0,0,0,177,177,177,161,0,6,0,92,0,0,0,0,0,0,198,226,0,0,0,200,2,0,0,160,108,198,0,33,7,4,0,176,128,0,0,0,0,0,65,194,0,0,255,44,24,1,0,0,27,0,177,234,0,0,128,200,2,0,0,1,177,108,108,237,0,1,1,200,137,192,62,0,177,177,198,139,0,255,255,48,40,0,0,160,27,27,177,192,0,0,7,200,2,128,62,0,27,27,198,139,0,4,255,92,0,0,0,0,0,0,198,226,0,0,0,200,3,192,1,224,26,26,0,34,7,7,0,92,4,0,0,0,108,177,177,161,0,6,0,200,12,192,0,0,236,236,0,226,0,0,0,200,15,128,2,224,0,0,0,34,7,7,0,200,15,128,3,224,0,0,0,34,8,8,0,200,15,128,4,224,0,0,0,34,9,9,0,200,15,128,5,224,0,0,0,34,10,10,0,0,0,0,0,0,0,0,0,0,0,0,0,};
		public static byte[] VS_ONCE_CLONE	= new byte[] { 16,42,17,1,0,0,1,84,0,0,1,84,0,0,0,0,0,0,0,36,0,0,0,240,0,0,1,24,0,0,0,0,0,0,0,0,0,0,0,200,0,0,0,28,0,0,0,189,255,254,3,0,0,0,0,4,0,0,0,28,0,0,16,0,0,0,0,182,0,0,0,108,0,2,0,7,0,240,0,0,0,0,0,116,0,0,0,0,0,0,0,132,0,2,0,6,0,1,0,0,0,0,0,144,0,0,0,0,0,0,0,160,0,2,0,5,0,1,0,0,0,0,0,144,0,0,0,0,0,0,0,171,0,2,0,4,0,1,0,0,0,0,0,144,0,0,0,0,105,110,100,105,99,101,115,0,0,1,0,3,0,1,0,4,0,240,0,0,0,0,0,0,114,97,110,100,83,105,122,101,0,171,171,171,0,1,0,3,0,1,0,4,0,1,0,0,0,0,0,0,115,111,117,114,99,101,83,105,122,101,0,116,97,114,103,101,116,83,105,122,101,0,118,115,95,51,95,48,0,50,46,48,46,55,54,56,48,46,48,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,20,0,252,0,16,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,64,0,0,1,20,0,17,0,2,0,0,0,0,0,0,0,0,0,0,32,66,0,0,0,1,0,0,0,1,0,0,0,2,0,0,2,144,0,0,0,3,0,0,240,80,0,1,241,81,0,0,16,21,0,0,16,20,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,63,0,0,0,191,0,0,0,63,128,0,0,191,128,0,0,16,1,16,3,0,0,18,0,194,0,0,0,0,0,96,4,96,10,18,0,18,0,0,0,0,0,0,0,96,16,196,0,34,0,0,0,5,248,0,0,0,0,15,248,0,0,0,0,48,136,2,0,0,108,177,108,161,0,6,0,92,6,0,2,0,22,188,27,33,5,255,2,200,1,0,0,160,108,198,0,33,7,4,0,176,64,0,0,0,0,0,192,194,0,0,255,52,66,0,0,160,177,198,198,33,7,5,0,0,64,0,0,0,0,0,198,226,0,0,0,168,64,0,0,0,0,0,2,194,0,0,4,200,3,0,1,0,176,0,0,232,128,0,0,200,6,0,1,1,188,188,188,237,0,1,1,176,17,1,2,2,177,198,193,128,1,255,255,176,44,1,1,0,172,241,2,192,2,1,255,200,139,192,62,0,178,178,0,226,1,1,0,92,0,0,0,0,0,0,27,226,0,0,2,168,65,0,0,0,108,0,131,202,1,0,6,200,1,0,0,0,108,27,198,171,0,5,2,184,18,0,0,0,27,27,192,194,1,1,255,200,3,192,1,224,26,26,0,34,7,7,0,200,15,128,0,0,221,221,0,226,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,};
#else
		public static byte[] VS_FRAME = new byte[] { 0, 3, 254, 255, 254, 255, 56, 0, 67, 84, 65, 66, 28, 0, 0, 0, 170, 0, 0, 0, 0, 3, 254, 255, 3, 0, 0, 0, 28, 0, 0, 0, 0, 1, 0, 0, 163, 0, 0, 0, 88, 0, 0, 0, 2, 0, 0, 0, 4, 0, 2, 0, 108, 0, 0, 0, 0, 0, 0, 0, 124, 0, 0, 0, 2, 0, 6, 0, 1, 0, 2, 0, 136, 0, 0, 0, 0, 0, 0, 0, 152, 0, 0, 0, 2, 0, 4, 0, 1, 0, 2, 0, 136, 0, 0, 0, 0, 0, 0, 0, 87, 111, 114, 108, 100, 86, 105, 101, 119, 80, 114, 111, 106, 101, 99, 116, 105, 111, 110, 0, 3, 0, 3, 0, 4, 0, 4, 0, 1, 0, 0, 0, 0, 0, 0, 0, 114, 97, 110, 100, 83, 105, 122, 101, 0, 171, 171, 171, 1, 0, 3, 0, 1, 0, 4, 0, 1, 0, 0, 0, 0, 0, 0, 0, 116, 97, 114, 103, 101, 116, 83, 105, 122, 101, 0, 118, 115, 95, 51, 95, 48, 0, 77, 105, 99, 114, 111, 115, 111, 102, 116, 32, 40, 82, 41, 32, 68, 51, 68, 88, 57, 32, 83, 104, 97, 100, 101, 114, 32, 67, 111, 109, 112, 105, 108, 101, 114, 32, 57, 46, 49, 53, 46, 55, 55, 57, 46, 48, 48, 48, 48, 0, 31, 0, 0, 2, 0, 0, 0, 128, 0, 0, 15, 144, 31, 0, 0, 2, 5, 0, 0, 128, 1, 0, 15, 144, 31, 0, 0, 2, 0, 0, 0, 128, 0, 0, 15, 224, 31, 0, 0, 2, 5, 0, 0, 128, 1, 0, 15, 224, 9, 0, 0, 3, 0, 0, 1, 224, 0, 0, 228, 144, 0, 0, 228, 160, 9, 0, 0, 3, 0, 0, 2, 224, 0, 0, 228, 144, 1, 0, 228, 160, 9, 0, 0, 3, 0, 0, 4, 224, 0, 0, 228, 144, 2, 0, 228, 160, 9, 0, 0, 3, 0, 0, 8, 224, 0, 0, 228, 144, 3, 0, 228, 160, 1, 0, 0, 2, 0, 0, 3, 128, 4, 0, 228, 160, 5, 0, 0, 3, 0, 0, 12, 128, 0, 0, 68, 128, 6, 0, 85, 160, 5, 0, 0, 3, 1, 0, 12, 224, 0, 0, 228, 128, 1, 0, 68, 144, 1, 0, 0, 2, 1, 0, 3, 224, 1, 0, 228, 144, 255, 255, 0, 0, };
		public static byte[] VS_ONCE = new byte[] { 0,3,254,255,254,255,53,0,67,84,65,66,28,0,0,0,158,0,0,0,0,3,254,255,3,0,0,0,28,0,0,0,0,1,0,0,151,0,0,0,88,0,0,0,2,0,7,0,240,0,2,0,96,0,0,0,0,0,0,0,112,0,0,0,2,0,6,0,1,0,2,0,124,0,0,0,0,0,0,0,140,0,0,0,2,0,4,0,1,0,2,0,124,0,0,0,0,0,0,0,105,110,100,105,99,101,115,0,1,0,3,0,1,0,4,0,240,0,0,0,0,0,0,0,114,97,110,100,83,105,122,101,0,171,171,171,1,0,3,0,1,0,4,0,1,0,0,0,0,0,0,0,116,97,114,103,101,116,83,105,122,101,0,118,115,95,51,95,48,0,77,105,99,114,111,115,111,102,116,32,40,82,41,32,68,51,68,88,57,32,83,104,97,100,101,114,32,67,111,109,112,105,108,101,114,32,57,46,49,53,46,55,55,57,46,48,48,48,48,0,81,0,0,5,0,0,15,160,0,0,128,63,0,0,0,64,0,0,128,191,0,0,0,0,31,0,0,2,0,0,0,128,0,0,15,144,31,0,0,2,0,0,0,128,0,0,15,224,31,0,0,2,5,0,0,128,1,0,15,224,31,0,0,2,5,0,1,128,2,0,15,224,31,0,0,2,5,0,2,128,3,0,15,224,31,0,0,2,5,0,3,128,4,0,15,224,31,0,0,2,5,0,4,128,5,0,15,224,31,0,0,2,5,0,5,128,6,0,15,224,12,0,0,3,0,0,8,128,0,0,0,144,0,0,0,145,19,0,0,2,0,0,4,128,0,0,0,144,2,0,0,3,0,0,2,128,0,0,170,129,0,0,0,144,12,0,0,3,0,0,4,128,0,0,170,129,0,0,170,128,4,0,0,4,0,0,8,128,0,0,255,128,0,0,170,128,0,0,85,128,46,0,0,2,0,0,8,176,0,0,255,128,1,0,0,3,0,0,13,128,7,32,228,160,0,0,255,176,5,0,0,3,1,0,8,128,0,0,0,128,4,0,170,160,1,0,0,2,0,0,2,128,0,0,0,160,4,0,0,4,0,0,2,128,0,0,0,128,4,0,170,160,0,0,85,128,19,0,0,2,0,0,1,128,1,0,255,128,2,0,0,3,0,0,2,128,0,0,85,128,0,0,0,129,5,0,0,3,0,0,2,128,0,0,85,128,4,0,255,160,19,0,0,2,1,0,4,128,1,0,255,139,13,0,0,3,1,0,8,128,1,0,255,128,1,0,255,129,18,0,0,4,0,0,1,128,1,0,255,128,1,0,170,128,1,0,170,129,5,0,0,3,1,0,2,128,6,0,85,160,0,0,0,144,4,0,0,4,0,0,3,224,0,0,85,160,0,0,228,128,0,0,170,160,5,0,0,3,1,0,8,224,1,0,85,128,6,0,85,160,1,0,0,2,1,0,4,224,1,0,85,128,19,0,0,3,0,0,2,128,7,32,85,160,0,0,255,176,1,0,0,2,0,0,12,224,0,0,52,160,2,0,0,4,1,0,8,128,0,0,85,129,7,32,85,160,0,0,255,176,12,0,0,3,0,0,1,128,0,0,85,129,0,0,85,128,12,0,0,5,0,0,2,128,7,32,85,160,0,0,255,176,7,32,85,161,0,0,255,176,1,0,0,2,1,0,3,224,0,0,255,160,4,0,0,4,0,0,2,128,0,0,85,128,0,0,0,128,1,0,255,128,5,0,0,3,2,0,15,224,0,0,174,128,0,0,240,160,46,0,0,2,0,0,8,176,0,0,85,128,1,0,0,3,3,0,15,224,7,32,228,160,0,0,255,176,1,0,0,3,4,0,15,224,8,32,228,160,0,0,255,176,1,0,0,3,5,0,15,224,9,32,228,160,0,0,255,176,1,0,0,3,6,0,15,224,10,32,228,160,0,0,255,176,255,255,0,0,};
		public static byte[] VS_ONCE_CLONE = new byte[] { 0,3,254,255,254,255,61,0,67,84,65,66,28,0,0,0,189,0,0,0,0,3,254,255,4,0,0,0,28,0,0,0,0,1,0,0,182,0,0,0,108,0,0,0,2,0,7,0,240,0,2,0,116,0,0,0,0,0,0,0,132,0,0,0,2,0,6,0,1,0,2,0,144,0,0,0,0,0,0,0,160,0,0,0,2,0,5,0,1,0,2,0,144,0,0,0,0,0,0,0,171,0,0,0,2,0,4,0,1,0,2,0,144,0,0,0,0,0,0,0,105,110,100,105,99,101,115,0,1,0,3,0,1,0,4,0,240,0,0,0,0,0,0,0,114,97,110,100,83,105,122,101,0,171,171,171,1,0,3,0,1,0,4,0,1,0,0,0,0,0,0,0,115,111,117,114,99,101,83,105,122,101,0,116,97,114,103,101,116,83,105,122,101,0,118,115,95,51,95,48,0,77,105,99,114,111,115,111,102,116,32,40,82,41,32,68,51,68,88,57,32,83,104,97,100,101,114,32,67,111,109,112,105,108,101,114,32,57,46,49,53,46,55,55,57,46,48,48,48,48,0,171,81,0,0,5,0,0,15,160,0,0,128,63,0,0,0,64,0,0,128,191,0,0,0,0,81,0,0,5,1,0,15,160,0,0,0,63,0,0,0,191,0,0,0,0,0,0,0,0,31,0,0,2,0,0,0,128,0,0,15,144,31,0,0,2,0,0,0,128,0,0,15,224,31,0,0,2,5,0,0,128,1,0,15,224,31,0,0,2,5,0,1,128,2,0,15,224,19,0,0,2,0,0,8,128,0,0,0,144,2,0,0,3,0,0,8,128,0,0,255,129,0,0,0,144,46,0,0,2,0,0,8,176,0,0,255,128,1,0,0,3,0,0,15,128,7,32,228,160,0,0,255,176,5,0,0,3,1,0,2,128,0,0,0,128,4,0,170,160,1,0,0,2,1,0,8,128,0,0,0,160,4,0,0,4,0,0,1,128,0,0,0,128,4,0,170,160,1,0,255,128,19,0,0,2,1,0,4,128,1,0,85,128,2,0,0,3,1,0,4,128,0,0,0,128,1,0,170,129,19,0,0,2,2,0,8,128,1,0,85,139,13,0,0,3,0,0,1,128,1,0,85,128,1,0,85,129,5,0,0,3,1,0,2,128,1,0,170,128,4,0,255,160,18,0,0,4,1,0,1,128,0,0,0,128,2,0,255,128,2,0,255,129,4,0,0,4,0,0,3,224,0,0,85,160,1,0,228,128,0,0,170,160,5,0,0,3,1,0,2,128,6,0,85,160,0,0,0,144,5,0,0,3,1,0,8,224,1,0,85,128,6,0,85,160,5,0,0,3,0,0,1,128,0,0,85,128,5,0,170,160,4,0,0,4,0,0,2,128,0,0,85,128,5,0,170,160,1,0,255,128,19,0,0,2,1,0,8,128,0,0,0,128,1,0,0,2,1,0,4,224,1,0,85,128,2,0,0,3,0,0,2,128,0,0,85,128,1,0,255,129,19,0,0,2,1,0,4,128,0,0,0,139,13,0,0,3,1,0,8,128,0,0,0,128,0,0,0,129,5,0,0,3,0,0,2,128,0,0,85,128,5,0,255,160,18,0,0,4,0,0,1,128,1,0,255,128,1,0,170,128,1,0,170,129,1,0,0,2,0,0,12,224,0,0,52,160,1,0,0,2,1,0,12,128,5,0,228,160,4,0,0,4,0,0,3,128,1,0,238,128,1,0,228,160,0,0,228,128,5,0,0,3,2,0,15,224,0,0,174,128,0,0,240,160,4,0,0,4,1,0,3,224,0,0,228,128,0,0,232,160,0,0,227,160,255,255,0,0,};
#endif

		//the source used to generate the shaders above...
		#region Compute Vertex Shaders


#if BUILD_VS_CODE


		private static string vsCode = @"	
float4x4 WorldViewProjection : register(c0);
float4 targetSize : register(c4);
float4 sourceSize : register(c5);
float4 randSize : register(c6);
float4 indices[240] : register(c7);

float InvRandSize()
{
	return randSize.y;
}

void VS_FRAME(
		float4 position		: POSITION,
	out	float4 positionOut	: POSITION,
		float4 texCoord		: TEXCOORD0,
	out	float4 texRandOut	: TEXCOORD0)
{
	positionOut = mul(position, WorldViewProjection);
	texRandOut.xy = texCoord.xy;

	texRandOut.zw = texCoord.xy * (targetSize.xy * InvRandSize());
}

float2 GetPosition(float2 invSize, float index)
{
	index *= invSize.x;
	return float2(fmod(index,1),(floor(index+1))*invSize.y);
}

void GenerateCommonData(float4 data, float index, out float4 positionOut, out float2 randCoordOut, out float2 lifeIndexOut)
{
	float positionIndex = data.x;

	lifeIndexOut = data.zw;
	
	positionOut    = float4(0,0,0,1);
	float2 positionValue = GetPosition(targetSize.zw,positionIndex);
	positionOut.xy = positionValue * 2 - 1;

	float invRand = InvRandSize();
	randCoordOut = float2(index * invRand, index * invRand * invRand);
}


//read from textures
void VS_ONCE_CLONE(
		float4 position     : POSITION, 
	out	float4 positionOut  : POSITION,
	out	float4 texRandOut   : TEXCOORD0,
	out	float4 lifeIndexOut : TEXCOORD1)
{
	float4 data;
	data = indices[position.x];

	float2 randCoordOut = 0;

	float2 lifeIndex = 0;
	GenerateCommonData(data, position.x, positionOut, randCoordOut, lifeIndex);
	lifeIndexOut = float4(lifeIndex,0,0);

	float textureIndex  = data.y;

	float2 texCoordOut = 0;
	texCoordOut    = GetPosition(sourceSize.zw,textureIndex);
	texCoordOut   += float2(0.5,-0.5) * sourceSize.zw;
	texCoordOut.y  = 1 - texCoordOut.y;

	texRandOut = float4(texCoordOut, randCoordOut);
}

//read from constants
void VS_ONCE(
		float4 position        : POSITION, 
	out	float4 positionOut     : POSITION,
	out	float4 texRandCoordOut : TEXCOORD0,
	out	float4 lifeIndexOut    : TEXCOORD1,
	out	float4 defaultPosition : TEXCOORD2,
	out	float4 defaultVelocity : TEXCOORD3,
	out	float4 defaultColour   : TEXCOORD4,
	out	float4 defaultUserData : TEXCOORD5)
{
	float4 data;
	int index = position.x;
	data = indices[index];

	int dataIndex = data.y;

	defaultPosition = indices[dataIndex + 0];
	defaultVelocity = indices[dataIndex + 1];
	defaultColour   = indices[dataIndex + 2];
	defaultUserData = indices[dataIndex + 3];

	data.y = 0;

	float2 randCoordOut = 0;

	float2 lifeIndex = 0;
	GenerateCommonData(data, position.x, positionOut, randCoordOut, lifeIndex);
	lifeIndexOut = float4(lifeIndex,0,0);

	texRandCoordOut = float4(0,0,randCoordOut);
}";

			static GpuVertexShaderData()
			{
				StringBuilder vs = new StringBuilder();

				vs.AppendLine("#if XBOX360");

				CompiledShader vscs_0 = ShaderCompiler.CompileFromSource(vsCode, null, null, CompilerOptions.None, "VS_FRAME", ShaderProfile.VS_3_0, TargetPlatform.Xbox360);
				CompiledShader vscs_1 = ShaderCompiler.CompileFromSource(vsCode, null, null, CompilerOptions.None, "VS_ONCE", ShaderProfile.VS_3_0, TargetPlatform.Xbox360);
				CompiledShader vscs_2 = ShaderCompiler.CompileFromSource(vsCode, null, null, CompilerOptions.None, "VS_ONCE_CLONE", ShaderProfile.VS_3_0, TargetPlatform.Xbox360);

				byte[] bytes = vscs_0.GetShaderCode();


				vs.Append("\t\tpublic static byte[] VS_FRAME		= new byte[] { ");
				for (int i = 0; i < bytes.Length; i++)
				{
					vs.Append((int)bytes[i]);
					vs.Append(",");
				}
				vs.AppendLine("};");

				bytes = vscs_1.GetShaderCode();

				vs.Append("\t\tpublic static byte[] VS_ONCE		= new byte[] { ");
				for (int i = 0; i < bytes.Length; i++)
				{
					vs.Append((int)bytes[i]);
					vs.Append(",");
				}
				vs.AppendLine("};");

				bytes = vscs_2.GetShaderCode();

				vs.Append("\t\tpublic static byte[] VS_ONCE_CLONE	= new byte[] { ");
				for (int i = 0; i < bytes.Length; i++)
				{
					vs.Append((int)bytes[i]);
					vs.Append(",");
				}
				vs.AppendLine("};");


				vs.AppendLine("#else");


				//do it all again for PC

				vscs_0 = ShaderCompiler.CompileFromSource(vsCode, null, null, CompilerOptions.None, "VS_FRAME", ShaderProfile.VS_3_0, TargetPlatform.Windows);
				vscs_1 = ShaderCompiler.CompileFromSource(vsCode, null, null, CompilerOptions.None, "VS_ONCE", ShaderProfile.VS_3_0, TargetPlatform.Windows);
				vscs_2 = ShaderCompiler.CompileFromSource(vsCode, null, null, CompilerOptions.None, "VS_ONCE_CLONE", ShaderProfile.VS_3_0, TargetPlatform.Windows);

				bytes = vscs_0.GetShaderCode();


				vs.Append("\t\tpublic static byte[] VS_FRAME		= new byte[] { ");
				for (int i = 0; i < bytes.Length; i++)
				{
					vs.Append((int)bytes[i]);
					vs.Append(",");
				}
				vs.AppendLine("};");

				bytes = vscs_1.GetShaderCode();

				vs.Append("\t\tpublic static byte[] VS_ONCE		= new byte[] { ");
				for (int i = 0; i < bytes.Length; i++)
				{
					vs.Append((int)bytes[i]);
					vs.Append(",");
				}
				vs.AppendLine("};");

				bytes = vscs_2.GetShaderCode();

				vs.Append("\t\tpublic static byte[] VS_ONCE_CLONE	= new byte[] { ");
				for (int i = 0; i < bytes.Length; i++)
				{
					vs.Append((int)bytes[i]);
					vs.Append(",");
				}
				vs.AppendLine("};");



				vs.AppendLine("#endif");

				string vss = vs.ToString();

				System.Diagnostics.Debugger.Break();
			}

#endif

		#endregion

	}

	/// <summary>
	/// stores the raw data for a particle system shader
	/// </summary>
	public struct ParticleSystemCompiledShaderData
	{
		/// <summary></summary>
		public readonly byte[] PixelShaderCode;
		/// <summary></summary>
		public readonly int ColourSamplerIndex, UserSamplerIndex, LifeSamplerIndex;

		/// <summary></summary><param name="reader"></param>
		public ParticleSystemCompiledShaderData(ContentReader reader)
		{
			this.PixelShaderCode = reader.ReadBytes(reader.ReadInt32());
			this.ColourSamplerIndex = reader.ReadInt32();
			this.UserSamplerIndex = reader.ReadInt32();
			this.LifeSamplerIndex = reader.ReadInt32();
		}

		internal ParticleSystemCompiledShaderData(byte[] shaderCode, int colourIndex, int userIndex, int lifeIndex)
		{
			if (shaderCode == null)
				throw new ArgumentNullException();
			this.PixelShaderCode = shaderCode;
			this.ColourSamplerIndex = colourIndex;
			this.UserSamplerIndex = userIndex;
			this.LifeSamplerIndex = lifeIndex;
		}
		
#if DEBUG && !XBOX360

		internal void Write(BinaryWriter writer)
		{
			writer.Write(this.PixelShaderCode.Length);
			writer.Write(this.PixelShaderCode);
			writer.Write(this.ColourSamplerIndex);
			writer.Write(this.UserSamplerIndex);
			writer.Write(this.LifeSamplerIndex);
		}

#endif
	}

	/// <summary>
	/// Data storage for the GPU particle system shader code
	/// </summary>
	public sealed class GpuParticleProcessorData
	{
		/// <summary></summary>
		public readonly ParticleSystemCompiledShaderData OnceShaderData, OnceCloneShaderData, FrameShaderData, FrameMoveShaderData;
		private readonly GpuParticleShader onceShader, onceCloneShader, frameShader, frameMoveShader;

		/// <summary></summary>
		/// <param name="onceShaderData"></param>
		/// <param name="onceCloneShaderData"></param>
		/// <param name="frameShaderData"></param>
		/// <param name="frameMoveShaderData"></param>
		public GpuParticleProcessorData(ParticleSystemCompiledShaderData onceShaderData, ParticleSystemCompiledShaderData onceCloneShaderData, ParticleSystemCompiledShaderData frameShaderData, ParticleSystemCompiledShaderData frameMoveShaderData)
		{
			this.OnceShaderData = onceShaderData;
			this.OnceCloneShaderData = onceCloneShaderData;
			this.FrameShaderData = frameShaderData;
			this.FrameMoveShaderData = frameMoveShaderData;

			this.onceShader = new GpuParticleShader(GpuVertexShaderData.VS_ONCE, this.OnceShaderData);
			this.onceCloneShader = new GpuParticleShader(GpuVertexShaderData.VS_ONCE_CLONE, this.OnceCloneShaderData);
			this.frameShader = new GpuParticleShader(GpuVertexShaderData.VS_FRAME, this.FrameShaderData);
			this.frameMoveShader = new GpuParticleShader(GpuVertexShaderData.VS_ONCE_CLONE, this.FrameMoveShaderData);
		}

		internal GpuParticleProcessorData(ContentReader reader) :
			this(new ParticleSystemCompiledShaderData(reader), new ParticleSystemCompiledShaderData(reader), new ParticleSystemCompiledShaderData(reader), new ParticleSystemCompiledShaderData(reader))
		{
		}

		/// <summary></summary>
		public IShader OnceShader { get { return onceShader; } }
		/// <summary></summary>
		public IShader OnceCloneShader { get { return onceCloneShader; } }
		/// <summary></summary>
		public IShader FrameShader { get { return frameShader; } }
		/// <summary></summary>
		public IShader FrameMoveShader { get { return frameMoveShader; } }

#if DEBUG && !XBOX360

		internal void Write(BinaryWriter writer)
		{
			OnceShaderData.Write(writer);
			OnceCloneShaderData.Write(writer);
			FrameShaderData.Write(writer);
			FrameMoveShaderData.Write(writer);
		}

		internal GpuParticleProcessorData(ParticleSystemTypeData typeData, bool useColourValues, bool usesUserValues, bool storeLifeData, TargetPlatform targetPlatform) : this(
			GpuParticleShaderBuilder.BuildGpuLogicPixelShader(typeData.ParticleLogicData.Once, GpuParticleShaderBuilder.LogicType.Once, usesUserValues, useColourValues, storeLifeData, targetPlatform, typeData.GpuBufferPosition),
			GpuParticleShaderBuilder.BuildGpuLogicPixelShader(typeData.ParticleLogicData.Once, GpuParticleShaderBuilder.LogicType.OnceClone, usesUserValues, useColourValues, storeLifeData, targetPlatform, typeData.GpuBufferPosition),
			GpuParticleShaderBuilder.BuildGpuLogicPixelShader(typeData.ParticleLogicData.Frame, GpuParticleShaderBuilder.LogicType.Frame, usesUserValues, useColourValues, storeLifeData, targetPlatform, typeData.GpuBufferPosition),
			GpuParticleShaderBuilder.BuildGpuLogicPixelShader(typeData.ParticleLogicData.Frame, GpuParticleShaderBuilder.LogicType.FrameMove, usesUserValues, useColourValues, storeLifeData, targetPlatform, typeData.GpuBufferPosition))
		{
		}

#endif
	}

	sealed class ConstantCache
	{
		public Vector4[] 
			buffer240 = new Vector4[240 + GpuParticleShader.ConstantCacheOffset],
			buffer128 = new Vector4[128 + GpuParticleShader.ConstantCacheOffset], 
			buffer64 = new Vector4[64 + GpuParticleShader.ConstantCacheOffset],
			buffer32 = new Vector4[32 + GpuParticleShader.ConstantCacheOffset],
			buffer16 = new Vector4[16 + GpuParticleShader.ConstantCacheOffset];
	}

	//manual implementation of the IShader interface
	sealed class GpuParticleShader : IShader, IDisposable
	{
		internal const int ConstantCacheOffset = 7;
		//byte data for each shader type
		private readonly byte[] vsb;
		private readonly byte[] psb;
		//the actual shaders
		private VertexShader vs;
		private PixelShader ps;

		//texture sampler indices for the optional PS textures
		private readonly int colourSamplerIndex, userSamplerIndex, lifeSamplerIndex;

		//source textures being used
		private Texture2D positionSize, velocityRotation, colourValues, userValues, randTexture, lifeTexture;
		//shader constant name indices
		private int world, viewsize, device;
		//vertex shader constants
		private Xen.Graphics.ShaderSystem.Constants.ConstantArray vreg = new Xen.Graphics.ShaderSystem.Constants.ConstantArray(ConstantCacheOffset);
		private Vector4[] psConstants;

		//shader has three modes, per-frame, 'copy' (move) and add.
		private bool enabledMoveVS, enabledAddVS;
		private Vector4[] vsMoveConstants;
		private ConstantCache constantCache;
		static string ConstantCacheName = typeof(GpuParticleShader).FullName + ".ConstantCache";
		private readonly Random random = new Random();

		private readonly Dictionary<ParticleSpawnValues, float> spawnIndices;

		public GpuParticleShader(byte[] vs, ParticleSystemCompiledShaderData pixelShaderData)
		{
			this.vsb = vs;
			this.psb = pixelShaderData.PixelShaderCode;

			this.colourSamplerIndex = pixelShaderData.ColourSamplerIndex;
			this.userSamplerIndex = pixelShaderData.UserSamplerIndex;
			this.lifeSamplerIndex = pixelShaderData.LifeSamplerIndex;

			this.spawnIndices = new Dictionary<ParticleSpawnValues, float>(new ParticleSpawnValues.ParticleSpawnValuesComparer());
			this.psConstants = new Vector4[6];
		}

		public void SetTextures(DrawState state, Texture2D positionSize, Texture2D velocityRot, Texture2D colour, Texture2D userValues, Texture2D randTexture, Texture2D lifeTexture)
		{
			this.positionSize		= positionSize;
			this.velocityRotation	= velocityRot;
			this.colourValues		= colour;
			this.userValues			= userValues;
			this.randTexture		= randTexture;
			this.lifeTexture		= lifeTexture;
		}

		public void SetMoveShaderDisabled(DrawState state)
		{
			SetMoveShaderEnabled(state, null, null, null, null, 0, 0);
		}

		//setup the shaders that copy or add particles
		public int SetMoveShaderEnabled(DrawState state, GpuParticleProcessor target, GpuParticleProcessor moveSource, Vector4[] constants, ParticleSpawnValues[] initialData, int count, int startIndex)
		{
			if (constants != null)
			{
				if (constantCache == null)
				{
					constantCache = state.UserValues[ConstantCacheName] as ConstantCache;
					if (constantCache == null)
					{
						constantCache = new ConstantCache();
						state.UserValues[ConstantCacheName] = constantCache;
					}
				}

				Vector4[] buffer = null;
				int copyCount = 0;

				int regCount = count;

				if (initialData != null)
					regCount = Math.Min(240, count * 5); //adding requires potentially lots of space

				if (count > 128)
				{
					copyCount = Math.Min(240, regCount);
					buffer = constantCache.buffer240;
				}
				else if (regCount > 64)
				{
					copyCount = Math.Min(128, regCount);
					buffer = constantCache.buffer128;
				}
				else if (regCount > 32)
				{
					copyCount = Math.Min(64, regCount);
					buffer = constantCache.buffer64;
				}
				else if (regCount > 16)
				{
					copyCount = Math.Min(32, regCount);
					buffer = constantCache.buffer32;
				}
				else
				{
					copyCount = Math.Min(16, regCount);
					buffer = constantCache.buffer16;
				}


				if (initialData != null)
					this.enabledAddVS = true;
				else
					this.enabledMoveVS = true;

				this.vsMoveConstants = buffer;

				//write from the 7th index on
				int index = ConstantCacheOffset;

				if (initialData != null)
				{
					//initial data gets rather complex.
					//the initial particle data is huge (64bytes), but often there is loads of duplicates.
					//
					//the constants array also doesn't use the 'y' value.
					//So, the starting info will be put in a dictionary, to remove duplicates.
					//Then, the y value will be used to write indices to read from.

					//at this point, the copyCount value is useless.

					spawnIndices.Clear();
					int space = buffer.Length - index;
					int written = 0;
					float indexF = 0;

					int endIndex = count + startIndex;

					//copy as many entries as possible
					copyCount = 0;
					float offset = 0;

					for (int i = startIndex; i < endIndex; i++)
					{
						float dataIndex;

						if (written == space)
							break;

						if (!spawnIndices.TryGetValue(initialData[i], out dataIndex))
						{
							//have to write data
							if (written + 5 > space)
								break;

							Vector4 value = constants[i];
							value.Y = indexF;

							buffer[index++] = value;
							spawnIndices.Add(initialData[i], indexF);

							written += 5;
							indexF += 4;//value takes 4 registers
						}
						else
						{
							//this is a duplicate
							Vector4 value = constants[i];
							value.Y = dataIndex;

							buffer[index++] = value;

							written ++;
						}
						copyCount++;
						offset++;
					}

					//thats as much as can be written
					//fill in the details...

					//offset the indices with the starting point to read from
					for (int i = 0; i < copyCount; i++)
						buffer[ConstantCacheOffset + i].Y += offset;

					indexF = 0;
					foreach (ParticleSpawnValues value in spawnIndices.Keys)
					{
						//this should be in logical order

						if (indexF != spawnIndices[value])
							throw new InvalidOperationException();
						indexF += 4;

						buffer[index++] = value.PositionSize;
						buffer[index++] = value.VelocityRotation;
						buffer[index++] = value.Colour;
						buffer[index++] = value.UserValues;
					}
				}
				else
				{
					for (int i = 0; i < copyCount; i++)
						buffer[index++] = constants[startIndex++];
				}

				//write target size into index 4 XY
				//write destination size into index 5 XY

				moveSource = moveSource ?? target;

				buffer[4] = new Vector4(target.ResolutionX, target.ResolutionY, 1.0f / target.ResolutionX, 1.0f / target.ResolutionY);
				buffer[5] = new Vector4(moveSource.ResolutionX, moveSource.ResolutionY, 1.0f / moveSource.ResolutionX, 1.0f / moveSource.ResolutionY);
			
				return copyCount;
			}
			else
			{
				this.vsMoveConstants = null;
				this.enabledMoveVS = false;
				this.enabledAddVS = false;
				return 0;
			}
		}

		//stepSize (delta time), current step, noise base XY
		public void SetConstants(float[] globals, float deltaTime, float time, float maxTimeStep)
		{
			this.psConstants[0].X = globals[0];
			this.psConstants[0].Y = globals[1];
			this.psConstants[0].Z = globals[2];
			this.psConstants[0].W = globals[3];

			this.psConstants[1].X = globals[4];
			this.psConstants[1].Y = globals[5];
			this.psConstants[1].Z = globals[6];
			this.psConstants[1].W = globals[7];

			this.psConstants[2].X = globals[8];
			this.psConstants[2].Y = globals[9];
			this.psConstants[2].Z = globals[10];
			this.psConstants[2].W = globals[11];

			this.psConstants[3].X = globals[12];
			this.psConstants[3].Y = globals[13];
			this.psConstants[3].Z = globals[14];
			this.psConstants[3].W = globals[15];

			this.psConstants[4].X = maxTimeStep;
			this.psConstants[4].Y = time;

			this.psConstants[5].Z = deltaTime;
		}

		public void Bind(Xen.Graphics.ShaderSystem.IShaderSystem state)
		{
			bool tc,ic;
			int deviceID = state.Begin(this, 4, 0, out tc, out ic);
			if (device != deviceID)
			{
				if (vs != null)
					vs.Dispose();
				if (ps != null)
					ps.Dispose();

				device = deviceID;

				//internally, the code is smart enough to not duplicate creating the pixel shader
				state.CreateShaders(out vs, out ps, vsb, psb, 0, 0, 0, 0);
			}

			float randX = (float)(random.NextDouble());
			float randY = (float)(random.NextDouble());

			this.psConstants[4].Z = randX;
			this.psConstants[4].W = randY;

			//random texture is limited in size, so randomly offset by a tiny amount
			randX = (float)((random.NextDouble() * 2 - 1) / 256.0);
			randY = (float)((random.NextDouble() * 2 - 1) / 256.0);

			this.psConstants[5].X = randX;
			this.psConstants[5].Y = randY;

			state.SetShaders(this.vs, this.ps);

			TextureSamplerState point = TextureSamplerState.PointFiltering;

			state.SetPixelShaderSampler(0, randTexture, point);

			if (positionSize != null)
				state.SetPixelShaderSampler(1, positionSize,	point);
			if (velocityRotation != null)
				state.SetPixelShaderSampler(2, velocityRotation, point);
			
			if (this.colourSamplerIndex != -1)
				state.SetPixelShaderSampler(this.colourSamplerIndex, colourValues, point);
			
			if (this.userSamplerIndex != -1)
				state.SetPixelShaderSampler(this.userSamplerIndex, userValues, point);

			if (this.lifeSamplerIndex != -1)
				state.SetPixelShaderSampler(this.lifeSamplerIndex, lifeTexture, point);


			state.SetWorldViewProjectionMatrix(this.vreg.Matrix4Transpose(0), ref this.world);

			const float randTextureSize = (float)RandomValueTexture.Resolution;
			const float invRandTextureSize = 1.0f / randTextureSize; 

			if (enabledMoveVS || enabledAddVS)
			{
				for (int i = 0; i < 4; i++) //copy the WVP matrix
					this.vsMoveConstants[i] = this.vreg.array[i];

				vsMoveConstants[6] = new Vector4(randTextureSize, invRandTextureSize, 0, 0);

				state.SetShaderConstants(this.vsMoveConstants, psConstants);
			}
			else
			{
				state.SetWindowSizeVector2(this.vreg.Vector2(4), ref this.viewsize);
				this.vreg.array[6] = new Vector4(randTextureSize, invRandTextureSize, 0, 0);

				state.SetShaderConstants(this.vreg.array, psConstants);
			}
		}

		//vertex shader input requirements... 
		void IShader.GetVertexInput(int index, out VertexElementUsage elementUsage, out int elementIndex)
		{
			elementIndex = 0;
			elementUsage = index == 0 ? VertexElementUsage.Position : VertexElementUsage.TextureCoordinate;
		}

		int IShader.GetVertexInputCount()
		{
			if (enabledMoveVS || enabledAddVS)
				return 1;
			return 2;
		}

		bool IShader.HasChanged
		{
			get { return true; }
		}

		//ignore
		#region attributes

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, bool value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, float[] value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Vector2[] value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Vector3[] value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Vector4[] value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Matrix[] value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, float value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Vector2 value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Vector3 value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Vector4 value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetAttribute(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, ref Matrix value)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetSamplerState(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, TextureSamplerState sampler)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, TextureCube texture)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Texture3D texture)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Texture2D texture)
		{
			throw new NotImplementedException();
		}

		bool IShader.SetTexture(Xen.Graphics.ShaderSystem.IShaderSystem state, int name_uid, Texture texture)
		{
			throw new NotImplementedException();
		}

		#endregion


		public void Dispose()
		{
			DisposeMember(ref vs);
			DisposeMember(ref ps);

			positionSize = null;
			velocityRotation = null;
			colourValues = null;
			userValues = null;
			randTexture = null;
			lifeTexture = null;

			vreg = null;
			psConstants = null;

			vsMoveConstants = null;
			constantCache = null;
		}

		private void DisposeMember<T>(ref T obj) where T : class, IDisposable
		{
			if (obj != null)
				obj.Dispose();
			obj = null;
		}
	}

}
