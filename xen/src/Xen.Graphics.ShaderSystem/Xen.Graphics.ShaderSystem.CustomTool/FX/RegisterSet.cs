﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xen.Graphics.ShaderSystem.CustomTool.FX
{
	public enum RegisterRank
	{
		Unknown = 0,
		FloatNx1 = 1,
		FloatNx2 = 2,
		FloatNx3 = 3,
		FloatNx4 = 4,
		IntNx1 = 5,
		IntNx2 = 6,
		IntNx3 = 7,
		IntNx4 = 8,
		Bool = 9
	}

	public enum RegisterCategory
	{
		Float4,
		Temp,
		Texture,
		Sampler,
		Boolean,
		Integer4
	}

	public struct Register
	{
		public string Name;
		public RegisterCategory Category;
		public int Index;
		public int Size;
		public string Type;
		public int ArraySize; // -1 if not an array.
		public string Semantic;
		public RegisterRank Rank;
	}

	//this code is a bit hacky
	public sealed class RegisterSet
	{
		private readonly Register[] registers;
		private int calculatedFloatRegisters = -1, minFloatRegisters = -1;

		public int RegisterCount { get { return registers.Length; } }
		public Register GetRegister(int index) { return registers[index]; }
		public bool TryGetRegister(string name, out Register register)
		{
			register = new Register();
			for (int i = 0; i < registers.Length; i++)
			{
				if (registers[i].Name == name)
				{
					register = registers[i];
					return true;
				}
			}
			return false;
		}

		public int FloatRegisterCount
		{
			get
			{
				if (calculatedFloatRegisters == -1)
				{
					int maxConstant = 0;
					for (int i = 0; i < RegisterCount; i++)
					{
						Register reg = GetRegister(i);
						if (reg.Category == RegisterCategory.Float4)
							maxConstant = Math.Max(maxConstant, reg.Index + reg.Size);
					}
					calculatedFloatRegisters = maxConstant;
				}
				return Math.Max(calculatedFloatRegisters, minFloatRegisters);
			}
		}

		public void SetMinFloatRegisterCount(int count)
		{
			minFloatRegisters = count;
		}

		public IEnumerator<Register> GetEnumerator()
		{
			return ((IEnumerable<Register>)registers).GetEnumerator();
		}

		public RegisterSet(Register[] set)
		{
			this.registers = set;
		}

		public RegisterSet(string header)
		{
			//extract the registers used...
			/*
			 * Example header: 
			 * 
			//
            // Generated by Microsoft (R) D3DX9 Shader Compiler 9.15.779.0000
            //
            // Parameters:
            //
            //   float2 shadowCameraNearFar;
            //
            //
            // Registers:
            //
            //   Name                Reg   Size
            //   ------------------- ----- ----
            //   shadowCameraNearFar c0       1
            //
			 * 
			 */

			Dictionary<string, Register> registers = new Dictionary<string, Register>();

			Tokenizer tokenizer = new Tokenizer(header, false, true, true);
			string[] lines = header.Split('\n');
			int state = 0;

			while (tokenizer.NextToken())
			{
				switch (tokenizer.Token)
				{
					case ":":
						break;
					case "Parameters":
						state = 1;
						break;

					case "//":

						//determine if the line has content...
						if (lines[tokenizer.Line].Trim().Length > 2)
						{

							if (state == 1)
							{
								//try and extract something

								//   float2 shadowCameraNearFar;

								tokenizer.NextToken();

								string type = tokenizer.Token;
								tokenizer.NextToken();

								if (type == "Registers")
								{
									state = 2; //done, go to registers
									break;
								}

								string name = tokenizer.Token;

								//possible array, or ;
								tokenizer.NextToken();
								string token = tokenizer.Token;
								int array = -1;

								if (token == "[")
								{
									tokenizer.NextToken();
									array = int.Parse(tokenizer.Token);
									tokenizer.NextToken(); //eat the ]
									tokenizer.NextToken();
								}

								//should be a ;
								if (tokenizer.Token != ";")
									throw new CompileException("Expected ';' in shader header");

								Register reg = new Register();
								reg.ArraySize = array;
								reg.Name = name;
								reg.Type = type;

								registers.Add(name, reg);
							}

							if (state == 2 || state == 3 || state == 4)
							{
								//   Name                Reg   Size
								//   ------------------- ----- ----
								//   shadowCameraNearFar c0       1

								string name, register, size;

								tokenizer.NextToken();
								name = tokenizer.Token;

								tokenizer.NextToken();
								register = tokenizer.Token;

								tokenizer.NextToken();
								size = tokenizer.Token;

								bool skip = false;

								if (name == "Name" && register == "Reg" && size == "Size")
									skip = true;
								if (name.Replace("-","").Length == 0 &&
									register.Replace("-","").Length == 0 &&
									size.Replace("-","").Length == 0)
									skip = true;

								if (!skip)
								{
									Register reg;
									if (registers.TryGetValue(name, out reg))
									{
										reg.Size = int.Parse(size);
										switch (register[0])
										{
											case 'c':
												reg.Category = RegisterCategory.Float4;
												break;
											case 'i':
												reg.Category = RegisterCategory.Integer4;
												break;
											case 'b':
												reg.Category = RegisterCategory.Boolean;
												break;
											case 't':
												reg.Category = RegisterCategory.Texture;
												break;
											case 's':
												reg.Category = RegisterCategory.Sampler;
												break;
											case 'r':
												reg.Category = RegisterCategory.Temp;
												break;
											default:
												throw new CompileException(string.Format("Unexpected constant type '{0}'", register[0]));
										}
										reg.Index = int.Parse(register.Substring(1));
										reg.Rank = ExtractRank(reg.Type, reg.Category, reg.ArraySize, reg.Size);
										registers[name] = reg;
									}
								}
							}
						}
						
						break;
				}
			}

			List<Register> registerList = new List<Register>();
			foreach (Register register in registers.Values)
				registerList.Add(register);

			this.registers = registerList.ToArray();
		}

		//figure out what the register is, in terms of how it is stored on the video card...
		private RegisterRank ExtractRank(string type, RegisterCategory category, int array, int size)
		{
			if (category != RegisterCategory.Float4 || type.Length < 3)
				return RegisterRank.Unknown;

			RegisterRank rank = RegisterRank.Unknown;
			int start = 0;

			switch (type.Substring(0, 3))
			{
				case "flo"://float
					rank = RegisterRank.FloatNx1;
					start = 5;
					break;
				case "int"://int
					rank = RegisterRank.IntNx1;
					start = 3;
					break;
				case "boo"://bool
					return RegisterRank.Bool;
				default:
					return RegisterRank.Unknown;
			}

			if (type.Length != start + 3) //not a matrix...
				return rank;

			if (type[start + 1] != 'x' ||
				!char.IsNumber(type[start + 2]))
				return RegisterRank.Unknown;

			int dim = int.Parse(type[start + 2].ToString());

			//a value may be defined as float4x4, however only float4x3 may be allocated (for example)
			//the rank must be the minimum of the two.
			if (array == -1)
				dim = Math.Min(dim, size);

			rank += (dim - 1);
			return rank;
		}


		public void MergeSemantics(RegisterSet fxRegisters)
		{
			for (int i = 0; i < this.registers.Length; i++)
			{
				//find the matching reg in the FX registers
				foreach (Register fx in fxRegisters)
				{
					if (fx.Name == this.registers[i].Name)
						this.registers[i].Semantic = fx.Semantic;
				}
			}
		}

		//generates a hashing array for these shader registers
		//used for merging two shaders, to do an approximate check to make sure they are compatible.
		public int[] GetHashSet()
		{
			List<int> hash = new List<int>(this.registers.Length*3+1);

			for (int i = 0; i < this.registers.Length; i++)
			{
				if (this.registers[i].Category == RegisterCategory.Float4)
				{
					hash.Add(this.registers[i].Index);
					hash.Add(this.registers[i].Size | ((int)this.registers[i].Rank << 16));
					hash.Add((this.registers[i].Semantic ?? this.registers[i].Name).GetHashCode());
				}
			}
			hash.Add(FloatRegisterCount);

			return hash.ToArray();
		}
	}
}
