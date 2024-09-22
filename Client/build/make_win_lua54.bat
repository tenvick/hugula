:: Build for x64 architecture with symbols
mkdir build64_54 & pushd build64_54
cmake -DLUA_VERSION=5.4.6 -DLUAC_COMPATIBLE_FORMAT=ON -G "Visual Studio 16 2019" -A x64  ..
popd
cmake --build build64_54 --config RelWithDebInfo
md plugin_lua54\Plugins\x86_64
copy /Y build64_54\RelWithDebInfo\xlua.pdb plugin_lua54\Plugins\x86_64\xlua.pdb
copy /Y build64_54\RelWithDebInfo\xlua.dll plugin_lua54\Plugins\x86_64\xlua.dll
copy /Y build64_54\RelWithDebInfo\xlua.dll ..\Assets\Plugins\x86_64\xlua.dll

mkdir build32_54 & pushd build32_54
cmake -DLUA_VERSION=5.4.6 -DLUAC_COMPATIBLE_FORMAT=ON -G "Visual Studio 16 2019" -A Win32 ..
popd
cmake --build build32_54 --config RelWithDebInfo
md plugin_lua54\Plugins\x86
copy /Y build32_54\RelWithDebInfo\xlua.pdb plugin_lua54\Plugins\x86\xlua.pdb
copy /Y build32_54\RelWithDebInfo\xlua.dll plugin_lua54\Plugins\x86\xlua.dll
copy /Y build32_54\RelWithDebInfo\xlua.dll ..\Assets\Plugins\x86\xlua.dll

pause