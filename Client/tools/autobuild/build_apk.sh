#!/bin/sh
#UNITY path
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
#change to you project root
PROJECT_ROOT=/Users/hugula/Documents/hugula/client

svn cleanup $PROJECT_ROOT
svn cleanup $PROJECT_ROOT/Assets --remove-unversioned
svn revert $PROJECT_ROOT --depth=infinity
svn up $PROJECT_ROOT

rm -fr $PROJECT_ROOT/../../release/ios

#chmod 777
chmod 777 $PROJECT_ROOT/tools/luaTools/luajit2.04

#导出xcode

$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.ScriptingDefineSymbols defineSymbols:HUGULA_NO_LOG,HUGULA_SPLITE_ASSETBUNDLE,HUGULA_APPEND_CRC,HUGULA_RELEASE -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildSlua -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.ExportRes -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_PATH -executeMethod ProjectBuild.BuildForAndroid -logFile $stdout
