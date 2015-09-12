echo off
echo buildText

set TEXTOUTDIR=%CD%\..\..\Assets\Config\
set STRINGTOOLS=%CD%\..\..\tools\site-packages\stringDirXlsConvertCSV.py
set STRINGSOURCE="Configs"

python %STRINGTOOLS% %STRINGSOURCE% %TEXTOUTDIR%
echo export text finish