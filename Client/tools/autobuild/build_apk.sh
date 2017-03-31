#!/bin/sh
#UNITY path
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
#
PROJECT_ROOT=/Users/puwenbin/Documents/tapEnjoy/slgios/client

svn cleanup $PROJECT_ROOT
svn cleanup $PROJECT_ROOT/Assets --remove-unversioned
svn revert $PROJECT_ROOT --depth=infinity
svn up $PROJECT_ROOT

rm -fr $PROJECT_ROOT/../../release/ios

#chmod 777
chmod 777 $PROJECT_ROOT/tools/luaTools/luajit2.04

#导出xcode

$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildSlua
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.DeleteStreamingOutPath
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.ExportRes
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_PATH -executeMethod ProjectBuild.BuildForAndroid
