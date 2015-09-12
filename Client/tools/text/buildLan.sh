#!/bin/bash
echo 开始导出文本资源
pwd

STRINGTOOLS="../site-packages/stringLanguage.py" #"site-packages/stringconv.py
TEXTOUTDIR="../../Assets/Lan"
STRINGSOURCE="Language.xls"

python $STRINGTOOLS $STRINGSOURCE $TEXTOUTDIR
echo 导出文本资源成功
