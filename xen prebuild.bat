@echo off
@echo Prebuilding xen (XNA GS 2.0)
@echo Register Visual Studio Custom Tool
"xen\bin\Xen.Graphics.ShaderSystem.CustomTool\registry\XenFxRegistryTest.exe" "xen\bin\Xen.Graphics.ShaderSystem.CustomTool\registry\XenFxRegistrySetup.exe" "xen\bin\Xen.Graphics.ShaderSystem.CustomTool\Xen.Graphics.ShaderSystem.CustomTool.dll" "XenFX" "{43ACA195-467F-4EEC-A949-5873BBD5413A}"
@echo Copying Xen Shader System files
copy "xen\prebuild\bin\XNAGS_2_0\x86\Xen.Graphics.ShaderSystem.dll" "xen\bin\x86\" /Y /V
copy "xen\prebuild\bin\XNAGS_2_0\x86\Xen.Graphics.ShaderSystem.xml" "xen\bin\x86\" /Y /V
copy "xen\prebuild\bin\XNAGS_2_0\Xbox 360\Xen.Graphics.ShaderSystem.dll" "xen\bin\Xbox 360\" /Y /V
copy "xen\prebuild\bin\XNAGS_2_0\Xbox 360\Xen.Graphics.ShaderSystem.xml" "xen\bin\Xbox 360\" /Y /V
@echo Copying Xen Content Project files
copy "xen\src\Xen\ContentProject\XNAGS_2_0\Content.contentproj" "xen\src\Xen\Content\" /Y /V
copy "xen\src\Xen.Ex\ContentProject\XNAGS_2_0\Content.contentproj" "xen\src\Xen.Ex\Content\" /Y /V
copy "xen\src\Tutorials\ContentProject\XNAGS_2_0\Content.contentproj" "xen\src\Tutorials\Content\" /Y /V
@echo Building Xen.Ex Shaders (This may take a minute or so...)
@echo Building Xen.Ex.Filters FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Xen.Ex\Filters\Shader.fx" "Xen.Ex.Filters" "xen\src\Xen.Ex\Filters\Shader.fx.cs"
@echo Building Xen.Ex.Graphics2D FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Xen.Ex\Graphics2D\FillTex.fx" "Xen.Ex.Graphics2D" "xen\src\Xen.Ex\Graphics2D\FillTex.fx.cs"
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Xen.Ex\Graphics2D\Shader.fx" "Xen.Ex.Graphics2D" "xen\src\Xen.Ex\Graphics2D\Shader.fx.cs"
@echo Building Xen.Ex.Shaders FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Xen.Ex\Shaders\Simple.fx" "Xen.Ex.Shaders" "xen\src\Xen.Ex\Shaders\Simple.fx.cs"
@echo Building Xen.Ex.Material FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Xen.Ex\Material\Material.fx" "Xen.Ex.Material" "xen\src\Xen.Ex\Material\Material.fx.cs"
@echo Building Xen.Ex.Material FX (Blending)
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Xen.Ex\Material\BlendMaterial.fx" "Xen.Ex.Material" "xen\src\Xen.Ex\Material\BlendMaterial.fx.cs"
@echo Building Xen.Ex.Material FX (Instancing)
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Xen.Ex\Material\InstanceMaterial.fx" "Xen.Ex.Material" "xen\src\Xen.Ex\Material\InstanceMaterial.fx.cs"
@echo Building Tutorials.Tutorial_03 FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Tutorials\Tutorial 03\shader.fx" "Tutorials.Tutorial_03" "xen\src\Tutorials\Tutorial 03\shader.fx.cs"
@echo Building Tutorials.Tutorial_16 FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Tutorials\Tutorial 16\shader.fx" "Tutorials.Tutorial_16" "xen\src\Tutorials\Tutorial 16\shader.fx.cs"
@echo Building Xen and Xen.Ex DEBUG
%SYSTEMROOT%\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe /t:Rebuild .\xen\prebuild\sln\prebuild.sln
@echo -----------------------------------
@echo -----------------------------------
@echo - If part of the prebuild failed:
@echo -
@echo - Check the following are installed:
@echo -
@echo - .NET Framework 2.0
@echo - XNA Game Studio 2.0
@echo - DirectX SDK
@echo -----------------------------------
@echo -----------------------------------
pause