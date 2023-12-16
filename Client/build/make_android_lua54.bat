for /f %%a in ('dir /a:d /b %ANDROID_SDK%\cmake\') do set cmake_version=%%a

set cmake_bin=%ANDROID_SDK%\cmake\%cmake_version%\bin\cmake.exe
set ninja_bin=%ANDROID_SDK%\cmake\%cmake_version%\bin\ninja.exe

mkdir build_v7a
%cmake_bin% -H.\ -B.\build_v7a "-GAndroid Gradle - Ninja" -DLUA_VERSION=5.4.6 -DANDROID_ABI=armeabi-v7a -DLUAC_COMPATIBLE_FORMAT=ON -DANDROID_NDK=%ANDROID_NDK% -DCMAKE_BUILD_TYPE=RelWithDebInfo -DCMAKE_MAKE_PROGRAM=%ninja_bin% -DCMAKE_TOOLCHAIN_FILE=.\cmake\android.windows.toolchain.cmake "-DCMAKE_CXX_FLAGS=-std=c++11 -fexceptions"
%ninja_bin% -C .\build_v7a
mkdir .\plugin_lua54\Plugins\Android\Libs\armeabi-v7a
@REM move .\build_v7a\libxlua.so .\plugin_lua54\Plugins\Android\Libs\armeabi-v7a\libxlua.so


:: 设置库文件和符号文件的路径
set lib_path=.\build_v7a\libxlua.so
set symbol_file=.\build_v7a\libxlua.sym

:: 生成符号文件
%ANDROID_NDK%\toolchains\aarch64-linux-android-4.9\prebuilt\windows-x86_64\bin\aarch64-linux-android-objcopy.exe --only-keep-debug %lib_path% %symbol_file%

:: 剥离库文件中的符号
%ANDROID_NDK%\toolchains\aarch64-linux-android-4.9\prebuilt\windows-x86_64\bin\aarch64-linux-android-strip.exe --strip-unneeded %lib_path%

:: 将符号文件与剥离后的库关联起来
%ANDROID_NDK%\toolchains\aarch64-linux-android-4.9\prebuilt\windows-x86_64\bin\aarch64-linux-android-objcopy.exe --add-gnu-debuglink=%symbol_file% %lib_path%

:: 移动库文件到插件目录
move %lib_path% .\plugin_lua54\Plugins\Android\Libs\build_v7a\libxlua.so
:: 也可以选择移动符号文件到指定目录，以便以后使用
move %symbol_file% .\plugin_lua54\Plugins\Android\Libs\build_v7a\libxlua.sym

@REM mkdir build_android_x86
@REM %cmake_bin% -H.\ -B.\build_android_x86 "-GAndroid Gradle - Ninja" -DLUA_VERSION=5.4.6 -DANDROID_ABI=x86 -DANDROID_NDK=%ANDROID_NDK% -DLUAC_COMPATIBLE_FORMAT=ON -DCMAKE_BUILD_TYPE=RelWithDebInfo -DCMAKE_MAKE_PROGRAM=%ninja_bin% -DCMAKE_TOOLCHAIN_FILE=.\cmake\android.windows.toolchain.cmake "-DCMAKE_CXX_FLAGS=-std=c++11 -fexceptions"
@REM %ninja_bin% -C .\build_android_x86
@REM mkdir .\plugin_lua54\Plugins\Android\Libs\x86
@REM move .\build_android_x86\libxlua.so .\plugin_lua54\Plugins\Android\Libs\x86\libxlua.so

mkdir build_android_arm64_v8a
%cmake_bin% -H.\ -B.\build_android_arm64_v8a "-GAndroid Gradle - Ninja" -DLUA_VERSION=5.4.6 -DANDROID_ABI=arm64-v8a -DLUAC_COMPATIBLE_FORMAT=ON -DANDROID_NDK=%ANDROID_NDK% -DCMAKE_BUILD_TYPE=RelWithDebInfo -DCMAKE_MAKE_PROGRAM=%ninja_bin% -DCMAKE_TOOLCHAIN_FILE=.\cmake\android.windows.toolchain.cmake "-DCMAKE_CXX_FLAGS=-std=c++11 -fexceptions"
%ninja_bin% -C .\build_android_arm64_v8a

mkdir .\plugin_lua54\Plugins\Android\Libs\arm64-v8a
@REM move .\build_android_arm64_v8a\libxlua.so .\plugin_lua54\Plugins\Android\Libs\arm64-v8a\libxlua.so

:: 设置库文件和符号文件的路径
set lib_path=.\build_android_arm64_v8a\libxlua.so
set symbol_file=.\build_android_arm64_v8a\libxlua.sym

:: 生成符号文件
%ANDROID_NDK%\toolchains\aarch64-linux-android-4.9\prebuilt\windows-x86_64\bin\aarch64-linux-android-objcopy.exe --only-keep-debug %lib_path% %symbol_file%

:: 剥离库文件中的符号
%ANDROID_NDK%\toolchains\aarch64-linux-android-4.9\prebuilt\windows-x86_64\bin\aarch64-linux-android-strip.exe --strip-unneeded %lib_path%

:: 将符号文件与剥离后的库关联起来
%ANDROID_NDK%\toolchains\aarch64-linux-android-4.9\prebuilt\windows-x86_64\bin\aarch64-linux-android-objcopy.exe --add-gnu-debuglink=%symbol_file% %lib_path%

:: 移动库文件到插件目录
move %lib_path% .\plugin_lua54\Plugins\Android\Libs\arm64-v8a\libxlua.so
:: 也可以选择移动符号文件到指定目录，以便以后使用
move %symbol_file% .\plugin_lua54\Plugins\Android\Libs\arm64-v8a\libxlua.sym
echo "compile success"
pause