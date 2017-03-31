set CURRENT_ROOT=%~dp0
set PAN=%~d0
%PAN%
cd %CURRENT_ROOT%
cd ..\..\
set UNITY_ROOT=%cd%
echo %UNITY_ROOT%

set slua_path=%UNITY_ROOT%\Assets\Slua\LuaObject
echo %slua_path%
rd /s /q %slua_path%
echo "del slua sccuess "

echo "cd to Unity Editor path"
D:
cd D:\Program Files\Unity\Editor\
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.BuildSlua
echo "slua make sccuess"
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.DeleteStreamingOutPath
echo "Delete StreamingPath  sccuess"
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.ExportRes
echo "ExportRes  sccuess"
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.BuildForAndroid
echo "android apk build sccuess"
echo "begin move apk to release"
echo CURRENT_ROOT
%PAN%
cd %CURRENT_ROOT%
cd ..\..\Assets
set m=%date:~5,2%
set d=%date:~8,2%
move warx.apk ..\..\..\release\warx%m%%d%.apk
echo "build warx"%m%%d%".apk sccuess"

pause
