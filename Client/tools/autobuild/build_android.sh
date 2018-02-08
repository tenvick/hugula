#!/bin/bash
# if [ $# -lt 3 ];then
#     echo "Params error!"
#     echo "Need version project define bundle id app display mod team sign"
#     exit
#elif [ ! -d ( ];then
#    echo "The first param is not a dictionary."
#    exit
# fi
echo "Input Params Count [$#]"

for arg in "$@"
do
    eval $arg
done

echo "UNITY_PROJECT_ROOT=$UNITY_PROJECT_ROOT;APK_OUTPATH=$APK_OUTPATH;DEFINE=$DEFINE;bundle:$BUNDLEID version:$VERSION productName:$PRODUCTNAME;SETTING=$SETTING;USE_IL2CPP=$USE_IL2CPP;NEED_UNITY_BUILD=$NEED_UNITY_BUILD;NEED_XCODE_BUILD"

cd $UNITY_PROJECT_ROOT

echo
echo -----------------------------------
echo Building Android...

chmod 777 $UNITY_PROJECT_ROOT/tools/luaTools/luajit2.04
if [ "$UNITY_PATH" == "" ];then
    UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
fi
echo "switch android ..."
# time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.SwitchBuildTarget buildTarget:android -logFile $stdout
echo "build unity ..."
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.ScriptingDefineSymbols define:$DEFINE -cleanedLogFile ScriptOnly #-logFile $stdout #define:HUGULA_RELEASE
echo "------------ copy plugins -----------------------"
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.CopyPlugins -logFile $stdout
echo "------------ build slua -----------------------"
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.BuildSlua -logFile $stdout
echo "------------ Export Resources -----------------------"
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.ExportRes bundle:$BUNDLEID version:$VERSION productName:$PRODUCTNAME -logFile $stdout
#time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.BuildForAndroidProject -logFile $stdout
echo "------------ Export Build Android apk -----------------------"
if [ "$USE_IL2CPP" == "" ];then
    time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.BuildForAndroidIL2CPP setting:$SETTING -logFile $stdout
else
    time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.BuildForAndroid setting:$SETTING -logFile $stdout
fi
# fi

echo
echo -----------------------------------
echo ...APK Done...
echo -----------------------------------
echo

if [ -f $UNITY_PROJECT_ROOT/shell/environment_shell.sh ];then
	echo "source environment_shell "
	cd $UNITY_PROJECT_ROOT/shell
    source environment_shell.sh
fi

APK=$UNITY_PROJECT_ROOT/hugula.apk
DST_DIR=$APK_OUTPATH 
STAMP_FOLDER=$(date +%Y%m-W%W)
STAMP=$(date +%y%m%d_%H%M)

if [ -f $APK ];then
	if [ ! -d $DST_DIR/$STAMP_FOLDER ];then
		mkdir $DST_DIR/$STAMP_FOLDER
    fi
fi

APK_NAME=hugula_$STAMP"_"$APP_FILENAME.apk
echo $APK_NAME
DEBUG_FILE_NAME=hugula_$STAMP"_"$APP_FILENAME.so.debug
OBB=$UNITY_PROJECT_ROOT/hugula.main.obb

if [ -f $OBB ];then
	if [ ! -d $DST_DIR/$STAMP_FOLDER/$STAMP ];then
		mkdir $DST_DIR/$STAMP_FOLDER/$STAMP
    fi
    echo $OBB_NAME
	mv $APK $DST_DIR/$STAMP_FOLDER/$STAMP/$APK_NAME
	cp $OBB $DST_DIR/$STAMP_FOLDER/$STAMP/$OBB_NAME
else
	mv $APK $DST_DIR/$STAMP_FOLDER/$APK_NAME
fi

DEBUG_FILE=$UNITY_PROJECT_ROOT/IL2CPPSymbols/armeabi-v7a/libil2cpp.so.debug
if [ -f $DEBUG_FILE ];then
	mv $DEBUG_FILE $DST_DIR/$STAMP_FOLDER/$DEBUG_FILE_NAME
fi

echo
echo -----------------------------------
echo ...ANDROID done...
echo -----------------------------------
echo

