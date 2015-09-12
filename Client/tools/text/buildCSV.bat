echo off
echo buildText

set TEXTOUTDIR=%CD%\..\..\Assets\Config\
set STRINGTOOLS=%CD%\..\..\tools\site-packages\stringXlsConvertCSV.py
set STRINGSOURCE="Config.xls"

python %STRINGTOOLS% %STRINGSOURCE% %TEXTOUTDIR%
echo export text finish