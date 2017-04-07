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
C:
cd C:\Program Files\Unity\Editor\
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.BuildSlua -logFile $stdout
echo "slua make sccuess"
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.DeleteStreamingOutPath -logFile $stdout
echo "Delete StreamingPath  sccuess"
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.ExportRes -logFile $stdout
echo "ExportRes  sccuess"
Unity.exe -projectPath %UNITY_ROOT% -quit -batchmode -executeMethod ProjectBuild.BuildForWindows -logFile $stdout
echo "windows build sccuess"

pause