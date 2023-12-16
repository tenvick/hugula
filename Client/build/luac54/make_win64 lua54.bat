mkdir build64 & pushd build64
cmake -DLUA_VERSION=5.4.6 -DLUAC_COMPATIBLE_FORMAT=ON -G "Visual Studio 16 2019" ..
IF %ERRORLEVEL% NEQ 0 cmake -DLUAC_COMPATIBLE_FORMAT=ON -G "Visual Studio 16 2019" ..
popd
cmake --build build64 --config Release
pause