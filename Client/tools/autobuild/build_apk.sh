#!/bin/sh
#UNITY path
export PATH=/usr/local/bin:$PATH
export ANDROID_SDK_ROOT=/Users/mac/Development/android-sdk-macosx
export ANDROID_HOME=$ANDROID_SDK_ROOT

UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
#change to you project root
CURRENT_ROOT=$(cd `dirname $0`; pwd)
echo "root"$CURRENT_ROOT
PROJECT_ROOT=$CURRENT_ROOT/../../../
cd $PROJECT_ROOT
PROJECT_ROOT=$(pwd)
echo $PROJECT_ROOT

APK_OUTPATH=$PROJECT_ROOT/FirstPackage/release/android
FIRESTPACKAGE_ROOT=$PROJECT_ROOT/FirstPackage
UNITY_PROJECT=$PROJECT_ROOT/client


#HUGULA_RELEASE,HUGULA_SPLITE_ASSETBUNDLE,HUGULA_NO_LOG 预编译命令
DEFINE=
#development,obb,
SETTING=
#BUNDLEID=
VERSION=0.4.3
#是否使用il2cpp方式构建版本 默认为空要走
USE_IL2CPP=
NEED_UNITY_BUILD=


if [ "$NEED_UNITY_BUILD" == "" ];then
	rm -fr $UNITY_PROJECT/Assets/Slua/LuaObject
	rm -fr $UNITY_PROJECT/Assets/LuaBytes
	rm -fr $UNITY_PROJECT/Assets/StreamingAssets
	rm -fr $UNITY_PROJECT/Temp

	# svn cleanup $FIRESTPACKAGE_ROOT
    # svn cleanup $FIRESTPACKAGE_ROOT --remove-unversioned
	# svn revert $FIRESTPACKAGE_ROOT --depth=infinity
	# svn up $FIRESTPACKAGE_ROOT
    
	# svn cleanup $UNITY_PROJECT
	# svn cleanup $UNITY_PROJECT/Assets --remove-unversioned
	# svn revert $UNITY_PROJECT --depth=infinity
fi
	# svn up $UNITY_PROJECT

chmod 777 $UNITY_PROJECT/tools/luaTools/luajit2.04
chmod 777 $UNITY_PROJECT/tools/autobuild/build_android.sh


$UNITY_PROJECT/tools/autobuild/build_android.sh UNITY_PROJECT_ROOT=$UNITY_PROJECT APK_OUTPATH=$APK_OUTPATH DEFINE=$DEFINE SETTING=$SETTING VERSION=$VERSION USE_IL2CPP=$USE_IL2CPP UNITY_PATH=$UNITY_PATH


if [ -f $UNITY_PROJECT/shell/environment_shell.sh ];then
	echo "source environment_shell "
	cd $UNITY_PROJECT/shell
    source environment_shell.sh
fi

echo $CODE_VERSION
echo $APP_NUMBER

-- to do 同步资源到svn 
