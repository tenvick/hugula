function foo()
{
local r
local a
r="$@"
while [[ "$r" != "$a" ]] ; do
a=${r%%/*}
echo "mkdir $a"
echo "cd $a"
r=${r#*/}
done
}
function upload_ftp()
{
echo "current folder "$1
echo "upload to "$FTP_ROOT$2
ftp -niv <<- EOF
open $FTP_IP
user $FTP_USER $FTP_PWD
lcd $1
$(foo "$FTP_ROOT$2")
cd $FTP_ROOT$2
bin
hash
pwd
prompt off
mput *.*
close
bye
EOF
}
upload_ftp D:\GitHub\hugula\FirstPackage\release\win/v4000 win/v4001
upload_ftp D:\GitHub\hugula\FirstPackage\release\win/v4000 win/v4000
upload_ftp D:\GitHub\hugula\FirstPackage\release\win/v4000\res win/v4001\res
upload_ftp D:\GitHub\hugula\FirstPackage\release\win/v4000\res/battle win/v4001\res/battle