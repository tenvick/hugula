echo off
echo buildText

set TEXTOUTDIR=%CD%\..\..\Assets\Config\Lan\Resources\
set STRINGTOOLS=%CD%\..\..\tools\site-packages\stringLanguage.py
set STRINGSOURCE="Language.xls"

python %STRINGTOOLS% %STRINGSOURCE% %TEXTOUTDIR%
echo export text finish