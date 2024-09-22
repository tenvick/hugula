@echo off

call "D:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat"

echo Swtich to x64 build env
cd %~dp0\luajit-2.1-agentzh\src
call msvcbuild_mt.bat static
@REM call msvcbuild.bat static
cd ..\..

mkdir build_lj64 & pushd build_lj64
cmake -DUSING_LUAJIT=ON -G "Visual Studio 17 2022" -A x64 ..
IF %ERRORLEVEL% NEQ 0 cmake -DUSING_LUAJIT=ON -G "Visual Studio 17 2022" -A x64 ..
popd
cmake --build build_lj64 --config RelWithDebInfo
md plugin_luajit\Plugins\x86_64
md plugin_luajit\luac
copy /Y build_lj64\RelWithDebInfo\xlua.dll plugin_luajit\Plugins\x86_64\xlua.dll
copy /Y build_lj64\RelWithDebInfo\xlua.pdb plugin_luajit\Plugins\x86_64\xlua.pdb
copy /Y build_lj64\RelWithDebInfo\xlua.dll ..\Assets\Plugins\x86_64\xlua.dll
cd %~dp0\luajit-2.1-agentzh\src 
mkdir "..\..\plugin_luajit\luac" 2>nul
copy /Y luajit.exe ..\..\plugin_luajit\luac\luajit21_x64.exe
mkdir "..\..\..\tools\luaTools\win\21-agentzh" 2>nul
copy /Y luajit.exe ..\..\..\tools\luaTools\win\21-agentzh\luajit21_x64.exe

call "D:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars32.bat"

echo Swtich to x86 build env
cd %~dp0\luajit-2.1-agentzh\src
call msvcbuild_mt.bat static
@REM call msvcbuild.bat static
cd ..\..

mkdir build_lj32 & pushd build_lj32
cmake -DUSING_LUAJIT=ON -G "Visual Studio 17 2022" -A Win32 ..
IF %ERRORLEVEL% NEQ 0 cmake -DUSING_LUAJIT=ON -G "Visual Studio 17 2022" -A Win32 ..
popd
cmake --build build_lj32 --config RelWithDebInfo
md plugin_luajit\Plugins\x86
copy /Y build_lj32\RelWithDebInfo\xlua.dll plugin_luajit\Plugins\x86\xlua.dll
copy /Y build_lj32\RelWithDebInfo\xlua.pdb plugin_luajit\Plugins\x86\xlua.pdb
copy /Y build_lj32\RelWithDebInfo\xlua.dll ..\Assets\Plugins\x86\xlua.dll
cd %~dp0\luajit-2.1-agentzh\src 
copy /Y luajit.exe ..\..\plugin_luajit\luac\luajit21_x32.exe
copy /Y luajit.exe ..\..\..\tools\luaTools\win\21-agentzh\luajit21_x32.exe


pause