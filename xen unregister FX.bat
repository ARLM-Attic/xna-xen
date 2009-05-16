@echo off
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo -
@echo - This tool unregisters the shader plugin
@echo -
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo.
pause

xen\bin\Xen.Graphics.ShaderSystem.CustomTool\registry\XenFxRegistrySetup.exe "xen\bin\Xen.Graphics.ShaderSystem.CustomTool\Xen.Graphics.ShaderSystem.CustomTool.dll" "XenFX" "{43ACA195-467F-4EEC-A949-5873BBD5413A}" remove