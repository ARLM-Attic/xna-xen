
float4x4 worldViewProj : WORLDVIEWPROJECTION;



float4 colour : GLOBAL;

float scale = 1;


bool testBool : register(b0);

//--------------------------------------------------------------//
// vertex shader, scales the mesh by the scale attribute
//--------------------------------------------------------------//
void Tutorial03VS(	
					float4 position			: POSITION, 
				out float4 positionOut		: POSITION)
{
	position.xyz *= scale;
	
	positionOut = mul(position,worldViewProj);
}


//--------------------------------------------------------------//
// pixel shader, returns the global colour
//--------------------------------------------------------------//
float4 Tutorial03PS() : COLOR 
{
if (testBool)
return float4(0,1,0,1);
else
	return colour;
}



//--------------------------------------------------------------//
// Technique that uses the shaders (a class will be generated)
//--------------------------------------------------------------//
technique Tutorial03Technique
{
   pass
   {
		VertexShader = compile vs_3_0 Tutorial03VS();
		PixelShader = compile ps_3_0 Tutorial03PS();
   }
}




//---------------------------------------------------------------------------------------------------
//Advanced use:
//---------------------------------------------------------------------------------------------------

//The following covers advanced uses of the shader system

//Sometimes an attribute must be set through an IShader interface - where the class type is unknown.
//For performance reasons, you cannot set an attribute by string name. The overhead would be very high.
//All attribute names have a unique integer ID.
//The DrawState object can get a unique ID using 'GetShaderAttributeNameUniqueID("name")'.
//This ID will never change for the entire life of the application.
//The value returned will always be a positive integer.
//A good time to call GetShaderAttributeNameUniqueID() would be in a ContentLoad, keeping a local copy
//of the index.
//Every shader instance will implement 'SetAttribute(...)' methods, which will return true if the
//attribute was set correctly.


//As demonstrated in the Tutorial code, the DrawState object has a generic method 'GetShader<T>'
//This method will get an application-wide global instance of a shader (by generic type)
//Calling this method is very fast.
//Shaders are quite CPU efficient, so reusing them is encouraged.
//Creating an instance per use will add memory overhead - the CPU overhead difference will be fairly small.


//The shader generated by this file will use around 128 bytes per instance.
//Most of this is the 64bytes for the WorldViewProjection matrix.
//It will use around 48 static bytes and around 140 bytes for the embedded static shader code.
//More complex shaders use more memory.

//There are a few restrictions on what can be used in a shader compiled with the plugin,
//The largest restriction is that Techniques may not make changes to render state.
//The choice was intentional not to support render state changes. While at first it may seem
//useful, in larger projects having shaders changing render state can cause a *lot* of very
//subtle bugs that can be very difficult to fix.

//A shader is intended to only change shader state, not render state.
//By extension of this restriction, multi-pass shaders are not supported by the plugin.

//Although, there is one exception. Texture sampler states may be set by the shader.
//This is because texture sample state changes the shader output, whereas render state does not.
//(The default texture sampler mode is always bilinear filtering)

//Custom structures cannot be used for shader attributes.
//(This doesn't work properly with effects either)



//Finally, compiler hints can be provided to the custom tool.
//This can be done by setting the very first line of a shader .fx file to a special comment.
//For example, set the very first line of this file to:

//CompilerOptions = InternalClass

//And the generated shader class will be an internal class, not accessible outside the project.
//The following options are supported (in the following format):
//CompilerOptions = NoPreShader, InternalClass, ParentNamespace, AvoidFlowControl, PreferFlowControl, PartialPrecision, DefinePlatform

//If your shader requires XBOX specific logic, then use the 'DefinePlatform' compiler option.
//When used, the shader will be defined for each platform.
//(the shader will be generated once for windows and once for xbox).
//
//The macro 'XBOX360' will be defined, allowing:
//
//#ifdef XBOX360
//...
//#else
//...
//#endif
//
//

//If you wish to use the Xbox special assembly instruction 'vfetch', see Tutorial 16.
//(Vfetch must be used through a macro in xen shaders)

//---------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------
