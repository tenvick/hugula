#!/bin/sh
#UNITY path
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity

#
#change to you project root
PROJECT_ROOT=/Users/hugula/Documents/hugula/client
RELEASE_ROOT=/Users/hugula/Documents/hugula/windows/pc


rm -fr $PROJECT_ROOT/Assets/Lua
rm -fr $PROJECT_ROOT/Assets/Slua/LuaObject

svn cleanup $PROJECT_ROOT/Assets
svn cleanup $PROJECT_ROOT/Assets --remove-unversioned
svn revert $PROJECT_ROOT --depth=infinity
svn up $PROJECT_ROOT

chmod 777 $PROJECT_ROOT/tools/luaTools/luajit2.04

#导出

$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildSlua -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.DeleteStreamingOutPath -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.ExportRes -logFile $stdout
$UNITY_PATH -quit -batchmode -projectPath $PROJECT_ROOT -executeMethod ProjectBuild.BuildForWindows -logFile $stdout
