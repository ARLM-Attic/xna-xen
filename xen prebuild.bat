@echo off
@echo.
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo - 
@echo - This process will prebuild xen (Debug) for XNA 3.0.
@echo - 
@echo - A VisualStudio CustomTool plugin will be installed,
@echo - which may prompt for admin rights in Windows Vista.
@echo - 
@echo - Please report any bugs you find - and of course
@echo - I greatly appreciate all feedback, good or bad!
@echo - 
@echo - Thank you :-)
@echo -
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo.
pause
@echo.
@echo Prebuilding xen for XNA GS 3.0 (This may take a minute or so...)
@echo.
@echo Register Visual Studio Custom Tool
"xen\bin\Xen.Graphics.ShaderSystem.CustomTool\registry\XenFxRegistryTest.exe" "xen\bin\Xen.Graphics.ShaderSystem.CustomTool\registry\XenFxRegistrySetup.exe" "xen\bin\Xen.Graphics.ShaderSystem.CustomTool\Xen.Graphics.ShaderSystem.CustomTool.dll" "XenFX" "{43ACA195-467F-4EEC-A949-5873BBD5413A}"
@echo Building Xen.Ex Shaders
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
@echo Building Tutorials.Tutorial_09 FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Tutorials\Tutorial 09\shader.fx" "Tutorials.Tutorial_09" "xen\src\Tutorials\Tutorial 09\shader.fx.cs"
@echo Building Tutorials.Tutorial_16 FX
xen\bin\Xen.Graphics.ShaderSystem.CustomTool\cmdxenfx.exe "xen\src\Tutorials\Tutorial 16\shader.fx" "Tutorials.Tutorial_16" "xen\src\Tutorials\Tutorial 16\shader.fx.cs"
@echo Building Xen and Xen.Ex DEBUG
%SYSTEMROOT%\Microsoft.NET\Framework\v3.5\MSBuild.exe /t:Rebuild .\xen\prebuild\sln\prebuild.sln
@echo.
@echo -----------------------------------
@echo -----------------------------------
@echo - If part of the prebuild failed:
@echo -
@echo - Check the following are installed:
@echo -
@echo - .NET Framework 3.5
@echo - XNA Game Studio 3.0
@echo - DirectX SDK
@echo -----------------------------------
@echo -----------------------------------
@echo.
@echo -----------------------------------
@echo - To get started, open:
@echo - ./xen/Tutorials.sln
@echo -----------------------------------
@echo.
pause