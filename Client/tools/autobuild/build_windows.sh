#!/bin/sh
#UNITY path
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
#
#change to you project root
CURRENT_ROOT=$(cd `dirname $0`; pwd)
echo "root"$CURRENT_ROOT
PROJECT_ROOT=$CURRENT_ROOT/../../
cd $PROJECT_ROOT
PROJECT_ROOT=$(pwd)
echo $PROJECT_ROOT

rm -fr $PROJECT_ROOT/Assets/Slua/LuaObject
rm -fr $PROJECT_ROOT/Assets/LuaBytes
rm -fr $PROJECT_ROOT/Assets/StreamingAssets
rm -fr $PROJECT_ROOT/release/*

chmod 777 $PROJECT_ROOT/tools/luaTools/luajit2.04

#导出

$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.ScriptingDefineSymbols defineSymbols:HUGULA_NO_LOG,HUGULA_SPLITE_ASSETBUNDLE,HUGULA_APPEND_CRC,HUGULA_RELEASE -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildSlua -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.ExportRes -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildForWindows -logFile $stdout
