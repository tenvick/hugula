// **************************************************
// This file created by Brett Bibby (c) 2010-2013
// You may freely use and modify this file as you see fit
// You may not sell it
//**************************************************
// hidden object game exporter
//$.writeln("=== Starting Debugging Session ===");

// enable double clicking from the Macintosh Finder or the Windows Explorer
#target photoshop
// debug level: 0-2 (0:disable, 1:break on error, 2:break at beginning)
// $.level = 0;
// debugger; // launch debugger on next line

var jsxfile="Export PSD Xml New.jsx";

var strData = "";
function writeLine1(line) {
    strData += "\n"+line;
}

main();

function main() {
    
    destinationFolder = Folder.selectDialog("Choose the destination for export.");
    if (!destinationFolder) {
        return;
    }

    var scriptFile = File($.fileName);
    var scriptFolder = scriptFile.parent
    app.playbackDisplayDialogs=DialogModes.NO;
    var jsxPath = scriptFolder+"/"+jsxfile;
    var files = destinationFolder.getFiles("*.psd");
    var numFiles = files.length;
    writeLine1("数量:"+numFiles);
    for (var i = 0; i < numFiles; i++) {
        var file = files[i];
        app.open(file);
        writeLine1("psd:"+numFiles);
        var scriptFile = File(jsxPath);
        $.evalFile(scriptFile);
        app.activeDocument.close(SaveOptions.DONOTSAVECHANGES);
    }
    
    alert("XML全部导出完成 :"+strData);

    return
    
}
