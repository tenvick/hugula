@echo off
set root=%CD%
set scriptPath="%root%\Export PSDUI New Only xml.jsx"
REM set scriptPath="F:\project\trunk_2_0\slg_client\Assets\ThirdEditor\PSD2UGUINew\JSCode\Export PSDUI New.jsx"
set photoshopPath="C:\Program Files\Adobe\Adobe Photoshop CC 2019\Photoshop.exe"
%photoshopPath%	-r %scriptPath% 
