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

echo "UNITY_PROJECT_ROOT=$UNITY_PROJECT_ROOT;IPA_OUTPATH=$IPA_OUTPATH;DEFINE=$DEFINE;bundle:$BUNDLEID version:$VERSION productName:$PRODUCTNAME;SETTING=$SETTING;DEV_TEAM=$DEV_TEAM;PRO_PROFILE_APPSTORE=$PRO_PROFILE_APPSTORE;PRO_PROFILE_ADHOC=$PRO_PROFILE_ADHOC;PRO_PROFILE_DEV=$PRO_PROFILE_DEV;NEED_UNITY_BUILD=$NEED_UNITY_BUILD;NEED_XCODE_BUILD=$NEED_XCODE_BUILD;"

PROJECT_ROOT=$UNITY_PROJECT_ROOT
PROJECT_ROOT=$('dirname' $PROJECT_ROOT)
PROJECT_ROOT=$('dirname' $PROJECT_ROOT)

XCODE_PROJECT_PATH=$PROJECT_ROOT/release/ios

cd $UNITY_PROJECT_ROOT

echo
echo -----------------------------------
echo Building IOS...
if [ "$UNITY_PATH" == "" ];then
    UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
fi

if [ "$NEED_UNITY_BUILD" == "" ];then
echo "switch ios ..."
# time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.SwitchBuildTarget buildTarget:ios -logFile $stdout
echo "build unity ..."
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.ScriptingDefineSymbols define:$DEFINE -logFile $stdout #--
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.CopyPlugins -logFile $stdout
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.BuildSlua -logFile $stdout
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.ExportRes bundle:$BUNDLEID version:$VERSION productName:$PRODUCTNAME -logFile $stdout
time $UNITY_PATH -quit -batchmode -projectPath $UNITY_PROJECT_ROOT -executeMethod ProjectBuild.BuildForIOS setting:$SETTING -logFile $stdout #setting:development,dev_scene,obb
fi

echo PROVISION_PROFILE"="$PRO_PROFILE

if [ -f $UNITY_PROJECT_ROOT/shell/environment_shell.sh ];then
	echo "source environment_shell "
	cd $UNITY_PROJECT_ROOT/shell
    source environment_shell.sh
fi

    IPAHEAD=`tr '[a-z]' '[A-Z]' <<<"game"`

    echo Building $DIR......

    cd $XCODE_PROJECT_PATH

    rm -fr huggula.xcarchive
    echo "rm  $PWD/huggula.xcarchive"
    #set path
    PARENT_DIR=$(dirname "$PWD")
    OUT_DIR=$PARENT_DIR/huggula
    if [ ! -d $OUT_DIR ];then
        mkdir $OUT_DIR
    fi

    # OUT_PLIST=$OUT_DIR/export.plist
    # if [ ! -f $OUT_PLIST ];then
    #     cp $UNITY_PROJECT_ROOT/Assets/Editor/DevExport.plist $OUT_PLIST
    # fi


function export_ipa()
{
    cd $XCODE_PROJECT_PATH
    local PRO_PROFILE=$1
    local EXPORT_OPTIONS_PLIST=$2 #$UNITY_PROJECT_ROOT/Assets/Editor/DevExport.plist
    local FILE_HEAD=$3 
    echo "export_ipa PRO_PROFILE=$PRO_PROFILE;EXPORT_OPTIONS_PLIST=$EXPORT_OPTIONS_PLIST;FILE_HEAD=$FILE_HEAD"
    xcodebuild clean -project Unity-iPhone.xcodeproj -configuration Release -alltargets || exit
    echo "hugula.xcarchive"
    xcodebuild archive -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -archivePath hugula.xcarchive PROVISIONING_PROFILE=$PRO_PROFILE CONFIGURATION_BUILD_DIR=$OUT_DIR || exit
    echo "exportArchive hugula.xcarchive"
    xcodebuild -exportArchive -archivePath hugula.xcarchive -exportPath $OUT_DIR  -exportOptionsPlist $EXPORT_OPTIONS_PLIST  || exit
    
    TMP_FOLDER=$(date +%Y%m-W%W)
    TMP_FILE=$(date +%y%m%d_%H%M)

    IPANAME=Unity-iPhone
    echo $IPANAME
    IPA=$OUT_DIR/$IPANAME.ipa
    DSYM=$OUT_DIR/ios.app.dSYM
    ZIP_DSYM=$DSYM.zip
    OUT_FILE_NAME=$FILE_HEAD$APP_FILENAME"_"$TMP_FILE
    echo $IPA

    if [ -f $IPA ];then
        
        if [ ! -d $IPA_OUTPATH/$TMP_FOLDER ];then
            mkdir $IPA_OUTPATH/$TMP_FOLDER
        fi
        
        mv $IPA $IPA_OUTPATH/$TMP_FOLDER/$OUT_FILE_NAME.ipa
        echo "mv $IPA to $IPA_OUTPATH/$TMP_FOLDER/$OUT_FILE_NAME.ipa"

        echo "zip $DSYM"
        zip -r $ZIP_DSYM $DSYM

        mv $ZIP_DSYM $IPA_OUTPATH/$TMP_FOLDER/$OUT_FILE_NAME.dSYM.zip
        echo "mv $ZIP_DSYM to $IPA_OUTPATH/$TMP_FOLDER/$OUT_FILE_NAME.dSYM.zip"
        rm -fr $DSYM
        echo "rm $DSYM"
        # echo "rm hugula.xcarchive"
    else
        echo "file not exist $IPA"
    fi
}


if [ "$NEED_XCODE_BUILD" == "" ];then
    if [ "$PRO_PROFILE_DEV" != "" ];then
        echo "build xcode DEV "
        export_ipa $PRO_PROFILE_DEV $UNITY_PROJECT_ROOT/Assets/Editor/ExportPlist/DevExport.plist   HUGULA_
    fi

    if [ "$PRO_PROFILE_ADHOC" != "" ];then
        echo "build xcode ADHOC "
        export_ipa $PRO_PROFILE_ADHOC $UNITY_PROJECT_ROOT/Assets/Editor/ExportPlist/ADHocExport.plist   HUGULA_ADHOC_
    fi
    
    if [ "$PRO_PROFILE_APPSTORE" != "" ];then
        echo "build xcode APPSTORE "
        export_ipa $PRO_PROFILE_APPSTORE $UNITY_PROJECT_ROOT/Assets/Editor/ExportPlist/AppStoreExport.plist   HUGULA_AppStore_
    fi
fi

echo
echo -----------------------------------
echo ...IOS done...
echo -----------------------------------
echo

