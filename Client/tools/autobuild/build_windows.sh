#!/bin/sh
#UNITY path
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity

#
PROJECT_ROOT=/Users/puwenbin/Documents/tapEnjoy/slgioswindows/client
RELEASE_ROOT=/Users/puwenbin/Documents/tapEnjoy/slgioswindows/pc


rm -fr $PROJECT_ROOT/Assets/Lua
rm -fr $PROJECT_ROOT/Assets/Slua/LuaObject

svn cleanup $PROJECT_ROOT/Assets
svn cleanup $PROJECT_ROOT/Assets --remove-unversioned
svn revert $PROJECT_ROOT --depth=infinity
svn up $PROJECT_ROOT

chmod 777 $PROJECT_ROOT/tools/luaTools/luajit2.04

#导出

$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildSlua
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.DeleteStreamingOutPath
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.ExportRes
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildForWindows
