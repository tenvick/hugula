#!/bin/bash

cd "$( dirname "${BASH_SOURCE[0]}" )"
echo $( dirname "${BASH_SOURCE[0]}" )

current=$(pwd)
sourcedir=$current/../../Assets/Lua
configdir=$current/../../Assets/Config
outdir=$current/../../Assets/Tmp/PW

jit204dir=$outdir
iosv7dir=$outdir
ios64dir=$outdir
macdir=$outdir

echo $current
echo $jit204dir

rm -fr $jit204dir
mkdir $jit204dir

#rm -fr $iosv7dir
#mkdir $iosv7dir
#
#rm -fr $macdir
#mkdir $macdir
#
#rm -fr $ios64dir
#mkdir $ios64dir


allFiles() {
    for file in $1/* 
    do
        if [ -d $file ]; then
            allFiles $file $2
        elif [ ${file##*.} = "lua" ]; then
            name=${file%".lua"}
            name=${name#$2$"/"}
            name=${name////_}
            exname=".u3d"
            bytename=${name}${exname} 

            $current/luac -o $macdir/$bytename $file
            echo  $file" => "$bytename
            bytename=${name}"_32"${exname}
            $current/luajit2.04 -b -g $file $jit204dir/$bytename
            $current/luajit2.1 -b -g $file $iosv7dir/$bytename
            bytename=${name}"_64"${exname}
            $current/luajit64 -b -g $file $ios64dir/$bytename
            echo  $file" => "$bytename
        fi
    done
}


echo $sourcedir" => "$outdir
allFiles $sourcedir $sourcedir
#
#echo $configdir" => "$outdir
#allFiles $configdir $configdir

