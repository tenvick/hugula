@echo off

set scriptPath="D:\project\github\hugula\Client\Assets\Third\PSD2UGUI\JSCode\Export PSDUI New.jsx"
set photoshopPath="C:\Program Files\Adobe\Adobe Photoshop CC 2019\Photoshop.exe"

for /r %%f in (*.psd) do (
  echo %%f
  %photoshopPath% %%f -r %scriptPath% "%%f"
)

pause