mkdir -p build_ios_54 && cd build_ios_54
cmake -DLUA_VERSION=5.4.6 -DLUAC_COMPATIBLE_FORMAT=ON -DCMAKE_TOOLCHAIN_FILE=../cmake/ios.toolchain.cmake -DPLATFORM=OS64 -GXcode ../
cd ..
cmake --build build_ios_54 --config RelWithDebInfo
mkdir -p plugin_lua54/Plugins/iOS/
# Copy the static library with debug symbols to the plugin directory
cp build_ios_54/RelWithDebInfo-iphoneos/libxlua.a plugin_lua54/Plugins/iOS/libxlua.a 
# Strip debug symbols from the static library for the production release
xcrun strip -S build_ios_54/RelWithDebInfo-iphoneos/libxlua.a
cp build_ios_54/RelWithDebInfo-iphoneos/libxlua.a ../Assets/Plugins/iOS/libxlua.a