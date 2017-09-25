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
rd /s /q %StreamingPath%
echo "del StreamingPath sccuess "

%UNITY_PATH% -projectPath %PROJECT_ROOT% -quit -batchmode -executeMethod ProjectBuild.ScriptingDefineSymbols defineSymbols:HUGULA_SPLITE_ASSETBUNDLE,HUGULA_APPEND_CRC,HUGULA_RELEASE -logFile $stdout
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
cd ..\..\
set m=%date:~5,2%
set d=%date:~8,2%
move hugula.apk ..\FirstPackage\hugula%m%%d%.apk
echo "build HUGULA"%m%%d%".apk sccuess"

pause
