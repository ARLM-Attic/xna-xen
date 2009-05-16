@echo off
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo -
@echo - This tool recurses through the current directory.
@echo - It will update any out of date shaders it finds.
@echo - 
@echo - Due to a bug, Visual Studio Custom Tools do not 
@echo - re-process a file when it has changed externally.
@echo - 
@echo - This tool is useful for updating project shaders
@echo - when using source control, such as SVN.
@echo -
@echo -----------------------------------------------------
@echo -----------------------------------------------------
@echo.
pause
@echo on
..\bin\Xen.Graphics.ShaderSystem.CustomTool\XenShaderUpdate.exe