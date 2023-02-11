@echo off
set root=%CD%
set scriptPath="%root%\Export PSDUI New.jsx"
set photoshopPath="C:\Program Files\Adobe\Adobe Photoshop CC 2019\Photoshop.exe"
%photoshopPath%	-r %scriptPath% 
