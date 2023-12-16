if [ -n "$ANDROID_NDK" ]; then
    export NDK=${ANDROID_NDK}
elif [ -n "$ANDROID_NDK_HOME" ]; then
    export NDK=${ANDROID_NDK_HOME}
elif [ -n "$ANDROID_NDK_HOME" ]; then
    export NDK=${ANDROID_NDK_HOME}
else
    export NDK=~/android-ndk-r15c
fi

NDK=/Users/cdl2/unity2019.4.35f1NDK
echo $NDK

if [ ! -d "$NDK" ]; then
    echo "Please set ANDROID_NDK environment to the root of NDK."
    exit 1
fi

function build() {
    API=$1
    ABI=$2
    TOOLCHAIN_ANME=$3
    BUILD_PATH=build54.Android.${ABI}
    SYMBOL_PATH=plugin_lua54/Plugins/Android/libs/${ABI}/

    cmake -H. -B${BUILD_PATH} -DLUA_VERSION=5.4.6 -DLUAC_COMPATIBLE_FORMAT=ON -DANDROID_ABI=${ABI} -DCMAKE_BUILD_TYPE=RelWithDebInfo -DCMAKE_TOOLCHAIN_FILE=${NDK}/build/cmake/android.toolchain.cmake -DANDROID_NATIVE_API_LEVEL=${API} -DANDROID_TOOLCHAIN=clang -DANDROID_TOOLCHAIN_NAME=${TOOLCHAIN_ANME}
    cmake --build ${BUILD_PATH} --config Release
    mkdir -p plugin_lua54/Plugins/Android/libs/${ABI}/
   // cp ${BUILD_PATH}/libxlua.so plugin_lua54/Plugins/Android/libs/${ABI}/libxlua.so
    # Create directories for the symbols and the library
    mkdir -p ${SYMBOL_PATH}
   
    # Extract the symbols
    ${NDK}/toolchains/llvm/prebuilt/darwin-x86_64/bin/llvm-objcopy --only-keep-debug ${BUILD_PATH}/libxlua.so ${BUILD_PATH}/libxlua.sym

    # Strip the symbols from the library
    ${NDK}/toolchains/llvm/prebuilt/darwin-x86_64/bin/llvm-strip --strip-unneeded ${BUILD_PATH}/libxlua.so

    # Add a link to the symbol file in the stripped library
    ${NDK}/toolchains/llvm/prebuilt/darwin-x86_64/bin/llvm-objcopy --add-gnu-debuglink=${BUILD_PATH}/libxlua.sym ${BUILD_PATH}/libxlua.so

    # Copy the library to the plugin directory
    cp ${BUILD_PATH}/libxlua.sym plugin_lua54/Plugins/Android/libs/${ABI}/libxlua.sym
    cp ${BUILD_PATH}/libxlua.so plugin_lua54/Plugins/Android/libs/${ABI}/libxlua.so
    cp ${BUILD_PATH}/libxlua.so ../Assets/Plugins/Android/libs/${ABI}/libxlua.so

}

build android-18 armeabi-v7a arm-linux-androideabi-4.9
build android-18 arm64-v8a  arm-linux-androideabi-clang
build android-18 x86 x86-4.9
