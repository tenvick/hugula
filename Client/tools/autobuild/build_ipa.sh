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

IPA_OUTPATH=$PROJECT_ROOT/FirstPackage/release/ios
FIRESTPACKAGE_ROOT=$PROJECT_ROOT/FirstPackage
UNITY_PROJECT=$PROJECT_ROOT/client
XCODE_PROJ_PATH=$PROJECT_ROOT/release


#HUGULA_RELEASE,HUGULA_SPLITE_ASSETBUNDLE,HUGULA_NO_LOG
DEFINE=HUGULA_RELEASE,HUGULA_NO_LOG
#development
SETTING=
BUNDLEID=com.hugula.demo
VERSION=0.13.3
#team id
DEV_TEAM=
#填写后构建dev版本
PRO_PROFILE_DEV=
#填写后构建adhoc版本
PRO_PROFILE_ADHOC=
#填写后构建appstore版本
PRO_PROFILE_APPSTORE=
#是否需要走unity构建流程 默认为空要走
NEED_UNITY_BUILD=
#是否需要走xcode构建流程 默认为空要走
NEED_XCODE_BUILD=


if [ "$NEED_UNITY_BUILD" == "" ];then
	rm -fr $UNITY_PROJECT/Assets/Slua/LuaObject
	rm -fr $UNITY_PROJECT/Assets/LuaBytes
	rm -fr $UNITY_PROJECT/Assets/StreamingAssets
	rm -fr $XCODE_PROJ_PATH/ios
	rm -fr $XCODE_PROJ_PATH/warx

	# svn cleanup $FIRESTPACKAGE_ROOT
	# svn revert $FIRESTPACKAGE_ROOT --depth=infinity
	# svn up $FIRESTPACKAGE_ROOT

	# svn cleanup $UNITY_PROJECT
	# svn cleanup $UNITY_PROJECT/Assets --remove-unversioned
	# svn revert $UNITY_PROJECT --depth=infinity
    
fi
    # svn up $UNITY_PROJECT
    
#chmod 777 
chmod 777 $UNITY_PROJECT/tools/luaTools/luajit2.1
chmod 777 $UNITY_PROJECT/tools/luaTools/luajit64
chmod 777 $UNITY_PROJECT/tools/autobuild/build_ios.sh

$UNITY_PROJECT/tools/autobuild/build_ios.sh UNITY_PROJECT_ROOT=$UNITY_PROJECT BUNDLEID=$BUNDLEID IPA_OUTPATH=$IPA_OUTPATH DEFINE=$DEFINE SETTING=$SETTING  VERSION=$VERSION DEV_TEAM=$DEV_TEAM PRO_PROFILE_ADHOC=$PRO_PROFILE_ADHOC PRO_PROFILE_APPSTORE=$PRO_PROFILE_APPSTORE PRO_PROFILE_DEV=$PRO_PROFILE_DEV NEED_UNITY_BUILD=$NEED_UNITY_BUILD NEED_XCODE_BUILD=$NEED_XCODE_BUILD UNITY_PATH=$UNITY_PATH


if [ -f $PROJECT_ROOT/project/client/shell/environment_shell.sh ];then
	echo " environment_shell "
	cd $PROJECT_ROOT/project/client/shell
    source environment_shell.sh
fi

echo $CODE_VERSION
echo $APP_NUMBER
