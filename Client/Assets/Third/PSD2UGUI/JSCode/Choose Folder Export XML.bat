@echo off
set root=%CD%
echo %root%
set scriptPath="%root%\Choose Folder Export XML.jsx"
echo %scriptPath%
REM set scriptPath="F:\project\trunk_2_0\slg_client\Assets\ThirdEditor\PSD2UGUINew\JSCode\Export PSDUI New.jsx"
set photoshopPath="C:\Program Files\Adobe\Adobe Photoshop CC 2019\Photoshop.exe"
%photoshopPath%	-r %scriptPath% 
