set CURRENT_ROOT=%~dp0
set PAN=%~d0
%PAN%
cd %CURRENT_ROOT%
cd ..\..\
set PROJECT_ROOT=%cd%
echo %PROJECT_ROOT%
set UNITY_PATH="C:\Program Files\Unity\Editor\Unity.exe"

set slua_path=%PROJECT_ROOT%\Assets\Slua\LuaObject
echo %slua_path%
rd /s /q %slua_path%
echo "del slua sccuess "

set StreamingPath=%PROJECT_ROOT%\Assets\StreamingAssets
rd /s /q %slua_path%
echo "del StreamingPath sccuess "

%UNITY_PATH% -projectPath %PROJECT_ROOT% -quit -batchmode -executeMethod ProjectBuild.ScriptingDefineSymbols defineSymbols:HUGULA_NO_LOG  -logFile $stdout
echo "scriptingDefineSymbols  sccuess"
%UNITY_PATH% -projectPath %PROJECT_ROOT% -quit -batchmode -executeMethod ProjectBuild.BuildSlua -logFile $stdout
echo "slua make sccuess"
%UNITY_PATH% -projectPath %PROJECT_ROOT% -quit -batchmode -executeMethod ProjectBuild.ExportRes -logFile $stdout
echo "ExportRes  sccuess"
%UNITY_PATH% -projectPath %PROJECT_ROOT% -quit -batchmode -executeMethod ProjectBuild.BuildForAndroid -logFile $stdout
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
