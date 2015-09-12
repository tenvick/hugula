#!/bin/bash
echo 开始导出文本资源
#cd /Users/enveesoft/baiyao/Client/tools/text
pwd
#AppType=`../../tools/utils.sh inputAppType $1`
#PlatformType=`../../tools/utils.sh inputPlatformType $2`
#AppDir=`../../tools/utils.sh getAppDir $PlatformType $AppType`
#STRINGTOOLS="../../tools/stringconv.py"
STRINGTOOLS="../site-packages/stringXlsConvertCSV.py" #"site-packages/stringconv.py
TEXTOUTDIR="../../Assets/Config/"
STRINGSOURCE="Config.xls"

python $STRINGTOOLS $STRINGSOURCE $TEXTOUTDIR
echo 导出文本资源成功
