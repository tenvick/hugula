mkdir build64 & pushd build64
cmake -DLUA_VERSION=5.4.6 -DLUAC_COMPATIBLE_FORMAT=ON -G "Visual Studio 16 2019"  -A x64 ..
popd
cmake --build build64 --config Debug
md plugin_lua54\Plugins\x86_64
copy /Y build64\Debug\xlua.dll plugin_lua54\Plugins\x86_64\xlua.dll
copy /Y build64\Debug\xlua.dll ..\Assets\Plugins\x86_64\xlua.dll
pause