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

var errorData = "";
var sceneData;
var sourcePsd;
var duppedPsd;
var destinationFolder;
var uuid;
var sourcePsdName;
var slicePaddingArr = new Array(0, 0, 0, 0)
var sliceOriArr = new Array(0, 0, 0, 0)
var textScaleArr = new Array(0, 0)

var depth = 0

var psdWHalf;
var psdHHalf;
var psdW;
var psdH;

main();

function main() {
    // got a valid document?
    if (app.documents.length <= 0) {
        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert("You must have a document open to export!");
        }
        // quit, returning 'cancel' makes the actions palette not record our script
        return 'cancel';
    }

    // cache useful variables
    uuid = 1;
    sourcePsdName = app.activeDocument.name;
    destinationFolder = app.activeDocument.path.toString() + "/out";
    var folder = new Folder(destinationFolder);
    if (!folder.exists) {
        folder.create();
    }
    var layerCount = app.documents[sourcePsdName].layers.length;
    var layerSetsCount = app.documents[sourcePsdName].layerSets.length;

    if ((layerCount <= 1) && (layerSetsCount <= 0)) {
        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert("You need a document with multiple layers to export!");
            // quit, returning 'cancel' makes the actions palette not record our script
            return 'cancel';
        }
    }

    // setup the units in case it isn't pixels
    var savedRulerUnits = app.preferences.rulerUnits;
    var savedTypeUnits = app.preferences.typeUnits;
    app.preferences.rulerUnits = Units.PIXELS;
    app.preferences.typeUnits = TypeUnits.PIXELS;

    // duplicate document so we can extract everythng we need
    duppedPsd = app.activeDocument;//.duplicate();
    duppedPsd.activeLayer = duppedPsd.layers[duppedPsd.layers.length - 1];
    psdWHalf = duppedPsd.width.value / 2;
    psdHHalf = duppedPsd.height.value / 2;
    psdW = duppedPsd.width.value;
    psdH = duppedPsd.height.value;

    //hideAllLayers(duppedPsd);

    // export layers
    sceneData = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
    writeNewLine("<PSDUI>");
    writeNewLine("\r\n");
    writeNewLine("<psdSize>");
    writeNewLine("<width>" + duppedPsd.width.value + "</width>", 1);
    writeNewLine("<height>" + duppedPsd.height.value + "</height>", 1);
    writeNewLine("</psdSize>");
    writeNewLine("\r\n");
    writeNewLine("<layers>");
    writeNewLine("\r\n");
    //导出对比图
    writeNewLine("<Layer name='"+sourcePsdName.replace("\.psd", "").replace(" ", "-")+"' visible='true' layerName='"+sourcePsdName.name+"' >", 1);
    writeNewLine("<type>RawImage</type>", 2);
    writeNewLine("<miniType>RawImage</miniType>", 2);
    writeNewLine("<position><x>0</x><y>0</y></position>", 2);
    writeNewLine("<size><width>" + psdW + "</width><height>" + psdH + "</height></size>", 2);
    writeNewLine("</Layer>", 1);
    exportAllLayers(duppedPsd, 0);
    writeNewLine("</layers>");

    writeNewLine("</PSDUI>");
    $.writeln(sceneData);


    // create export
    var destFile = destinationFolder + "/" + sourcePsdName.replace("\.psd", "_info.xml");
    var sceneFile = new File(destFile);
    sceneFile.encoding = "utf-8";   //写文件时指定编码，不然中文会出现乱码
    sceneFile.open('w');
    sceneFile.writeln(sceneData);
    sceneFile.close();

    //create error tips
    if(errorData)
    {
        var destErrFile = destinationFolder + "/" + sourcePsdName.replace("\.psd", "_Error.txt");
        var sceneErrFile = new File(destErrFile);
        sceneErrFile.encoding = "utf-8";   //写文件时指定编码，不然中文会出现乱码
        sceneErrFile.open('w');
        sceneErrFile.writeln(errorData);
        sceneErrFile.close();
        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert("出错误信息 \n"+errorData);
            alert(" 导出错误信息！ 路径：" + destErrFile);
        }
    }

    app.preferences.rulerUnits = savedRulerUnits;
    app.preferences.typeUnits = savedTypeUnits;
    if (app.playbackDisplayDialogs != DialogModes.NO) {
        alert(" 导出成功！ 路径：" + destFile);
    }
}

function writeNewLine(line, depth) {
    if (depth > 0) {
        var tmp = ""
        var i = 0;
        while (i < depth) {
            tmp += "    ";
            i++;
        }
        sceneData += "\n" + tmp + line;

    } else {
        sceneData += "\n" + line;
    }
}


function writeLine(line) {
    sceneData += line;
}

function writeErrorLine(line)
{
    errorData += "\n" + line;
}

function exportAllLayers(obj, depth) {
    if (typeof (obj) == "undefined") {
        return;
    }

    if (typeof (obj.layers) != "undefined" && obj.layers.length > 0) {
        for (var i = obj.layers.length - 1; 0 <= i; i--) {
            writeNewLine("\r\n");
            exportLayer(obj.layers[i], 1);
        }

    }
    else {
        writeNewLine("\r\n");
        exportLayer(obj.layer, 0);
    };
}

function exportLayer(obj, depth) {
    if (typeof (obj) == "undefined") {
        return;
    }
    exportComponents(obj, depth);

}

//导出组件
function exportComponents(_layer, depth) {

    var _layerName=syntaxSugar(_layer.name)

    if (_layer.typename == "LayerSet" && checkShouldExport(_layerName)) {
        checkLayerName(_layer.name);
        var validFileName = makeValideTextureName(_layerName);  // _layer.name.substring(0, _layer.name.search("@"));

        writeNewLine("<Layer name='"+validFileName+"' visible='"+_layer.visible+"' layerName='"+_layer.name+"' >", depth + 1);
        if (isTemplate(_layerName)) {
            writeNewLine("<type>" + getTemplateType(_layerName) + "</type>", depth + 2);
            writeNewLine("<templateName>" +getTemplateName(_layerName)+ "</templateName> ", depth + 2);
            writeNewLine("<miniType>" + getTypeName(_layerName) + "</miniType>", depth + 2);
        }
        else {
            writeNewLine("<type>" + getTypeName(_layerName) + "</type>", depth + 2);
        }
        writeNewLine("<tag>" + getTagList(_layerName) + "</tag>", depth + 2);
        writeNewLine("<layers>", depth + 2);
        for (var i = 0; i < _layer.layers.length; i++) {
            exportComponents(_layer.layers[i], depth + 3);
        }
        writeNewLine("</layers>", depth + 2);
        writeNewLine("</Layer>", depth + 1);

    } else if (_layer.typename = "ArtLayer" && checkShouldExport(_layerName)) {
        checkLayerName(_layer.name);
        var validFileName = makeValideTextureName(_layerName);
        writeNewLine("<Layer name='"+validFileName+"' visible='"+_layer.visible+"' layerName='"+_layer.name+"' >", depth + 1);
        writeNewLine("<tag>" + getTagList(_layerName) + "</tag>", depth + 2);
        if (LayerKind.TEXT == _layer.kind) {
            exportLabel(_layer, validFileName, depth + 2);
        }
        else {
            app.activeDocument.activeLayer = _layer;
            exportImage(_layer, validFileName, depth + 2);
        }
        writeNewLine("</Layer>", depth + 1);

    }

}


function setLayerSizeAndPos(layer, depth) {
    layer.visible = true;

    var recSize = getLayerRec(duppedPsd.duplicate());

    writeNewLine("<position>", depth);
    writeLine("<x>" + recSize.x + "</x>", depth);
    writeLine("<y>" + recSize.y + "</y>", depth);
    writeLine("</position>", depth);
    writeNewLine("<size>", depth);
    writeLine("<width>" + recSize.width + "</width>", depth);
    writeLine("<height>" + recSize.height + "</height>", depth);
    writeLine("</size>", depth);

    layer.visible = false;

    return recSize;
}


function exportLabel(obj, validFileName, depth) {
    //有些文本如标题，按钮，美术用的是其他字体，可能还加了各种样式，需要当做图片切出来使用
    
    var lowerName = obj.name.toLowerCase()
    if (lowerName.search("_sprite") >= 0) {
        exportImage(obj, validFileName, depth);
        return;
    }
    var _layerName=syntaxSugar(obj.name)

    if (isTemplate(_layerName)) {
        writeNewLine("<type>Customer</type>", depth);
        writeNewLine("<miniType>Text</miniType>", depth);
        writeNewLine("<templateName>" + getTemplateName(_layerName) + "</templateName> ", depth);
    } else {
        writeNewLine("<type>Text</type>", depth);
    }


    //处理静态文本，会对应unity的静态字体
    var StaticText = false;
    if (lowerName.search("_static") >= 0) {
        StaticText = true;
    }

    setArtNodeXmlInfo(obj, depth);
    app.activeDocument.activeLayer = obj;
    var layerDesc = getActiveLayerDescriptor();
    var outline = getOutline(layerDesc);
    if ("" != outline) {
        writeNewLine("<outline>" + outline + "</outline>", depth);
    }
    var gradient = getGradientFill(layerDesc);
    if ("" != gradient) {
        writeNewLine("<gradient>" + gradient + "</gradient>", depth);
    }

    var shadow = getShadow(layerDesc);
    if ("" != shadow) {
        writeNewLine("<shadow>" + shadow + "</shadow>", depth);
    }

    writeNewLine("<arguments>", depth);
    writeLine("<string>" + gen_text_color(obj) + "</string>", depth);

    if (StaticText == true) {
        writeLine("<string>" + obj.textItem.font + "_Static" + "</string>", depth);
    }
    else {
        writeLine("<string>" + gen_font(obj) + "</string>", depth);
    }
    //writeNewLine( "<string>" + obj.textItem.font + "</string>",depth);
    var scale = getTextScale(obj, textScaleArr);
    var fontSize = Math.round(gen_text_size(obj) * Math.min(scale.x, scale.y));

    writeLine("<string>" + fontSize + "</string>", depth);
    writeLine("<string><![CDATA[" + obj.textItem.contents.replace(/&/gi, "") + "]]></string>", depth);

    //段落文本带文本框，可以取得对齐方式
    if (obj.textItem.kind == TextType.PARAGRAPHTEXT) {
        writeLine("<string>" + gen_text_justify(obj) + "</string>", depth);     //加对齐方式
    }
    writeLine("</arguments>", depth);

    // 透明度
    writeNewLine("<opacity>" + obj.opacity + "</opacity>", depth);

    // 新增渐变
    if (obj.name.search("_JB") >= 0) {
        var _text = obj.name.substring(obj.name.search("_JB"), obj.name.length);

        var params = _text.split("|");
        params = params[0].split(":");

        if (params.length > 1) {
            writeLine("<gradient>");

            for (var i = 0; i < params.length; ++i) {
                if (params[i].search("_") >= 0) {
                    continue;
                }

                writeLine(params[i]);

                if (i < params.length - 1) {
                    writeLine("|");
                }
            }

            writeLine("</gradient>");
        }
    }

    // 新增描边
    if (obj.name.search("_OL") >= 0) {
        var _text = obj.name.substring(obj.name.search("_OL"), obj.name.length);

        var params = _text.split("|");
        params = params[0].split(":");

        if (params.length > 1) {
            writeLine("<outline>");

            for (var i = 0; i < params.length; ++i) {
                if (params[i].search("_") >= 0) {
                    continue;
                }

                writeLine(params[i]);

                if (i < params.length - 1) {
                    writeLine("|");
                }
            }

            writeLine("</outline>");
        }
    }

    // obj.visible = false;
}

function gen_font(obj) {
    try {
        return obj.textItem.font.toString();
    } catch (error) {
        return "ArialMT";
    }
}

function gen_text_color(obj) {
    try {
        return obj.textItem.color.rgb.hexValue.toString();
    } catch (error) {
        return "FFFFFF";
    }
}

function gen_text_size(obj) {
    try {
        return obj.textItem.size.value;
    } catch (error) {
        return 26;
    }
}

function gen_text_justify(obj) {
    try {
        returnobj.textItem.justification;
    } catch (error) {
        return Justification.LEFT;
    }
}

function gen_text_leading(obj, scale) {
    try {
        return obj.textItem.leading.value * scale.y ;
    } catch (error) {
        return "X";
    }
}

function exportImage(obj, validFileName, depth) {

    //var saveToDisk = checkShouldSavePng(obj.name);
    var _layerName=syntaxSugar(obj.name)
    var oriName = obj.name.substring(obj.name.indexOf("@", obj.name.length));
    var lowerName = obj.name.toLowerCase()
    
    if (isTemplate(_layerName)) {
        writeNewLine("<type>" + getTemplateType(_layerName) + "</type>", depth);
        writeNewLine("<miniType>Image</miniType>", depth);
        writeNewLine("<templateName>" + getTemplateName(_layerName) + "</templateName> ", depth);
        setArtNodeXmlInfo(obj, depth);
        return
    } else
    {
        writeNewLine("<type>" + "Image" + "</type>", depth);
    }

    writeNewLine("<opacity>" + obj.opacity + "</opacity>", depth);

    if (oriName.toLowerCase().search("_9s") >= 0) {
        writeNewLine("<miniType>SliceImage</miniType>", depth);
        setArtNodeXmlInfo(obj, depth);
        var posInfo = getLayerPosInfo(obj);
        _9sliceCutImg(obj.name, posInfo.w, posInfo.h, depth);

        return;
    }
    else if (oriName.toLowerCase().search("_lefthalf") > 0 || oriName.toLowerCase().search("_lhalf") > 0)       //左右对称的图片切左边一半
    {
        writeNewLine("<miniType>" + "LeftHalfImage" + "</miniType>", depth);

        setArtNodeXmlInfo(obj, depth);
        return;
    }
    else if (oriName.toLowerCase().search("_bottomhalf") > 0 || oriName.toLowerCase().search("_bhalf") > 0)     //上下对称的图片切底部一半
    {
        writeNewLine("<miniType>" + "BottomHalfImage" + "</miniType>", depth);
        setArtNodeXmlInfo(obj, depth);
        return;
    }
    else if (oriName.toLowerCase().search("_quarter") > 0)     //上下左右均对称的图片切左下四分之一
    {
        writeNewLine("<miniType>" + "QuarterImage" + "</miniType>", depth);
        setArtNodeXmlInfo(obj, depth);
        return;
    }
    else //if(saveToDisk)
    {
        writeNewLine("<miniType>" + "Image" + "</miniType>", depth);
        setArtNodeXmlInfo(obj, depth);
    }


}

function hideAllLayers(obj) {
    // hideLayerSets(obj);
}

function hideLayerSets(obj) {
    for (var i = obj.layers.length - 1; 0 <= i; i--) {
        if (obj.layers[i].typename == "LayerSet") {
            hideLayerSets(obj.layers[i]);
        }
        else {
            obj.layers[i].visible = false;
        }
    }
}

//显示图层组及组下所有图层
function showAllLayers(obj) {
    showLayerSets(obj);
}

function showLayerSets(obj) {
    for (var i = obj.layers.length - 1; 0 <= i; i--) {
        if (obj.layers[i].typename == "LayerSet") {
            showLayerSets(obj.layers[i]);
        }
        else {
            obj.layers[i].visible = true;
        }
    }
}



function getLayerRec(psd, notMerge) {
    // we should now have a single art layer if all went well
    if (!notMerge && checkCanMerge(psd)) {
        psd.mergeVisibleLayers();
    }

    // figure out where the top-left corner is so it can be exported into the scene file for placement in game
    // capture current size
    var height = psd.height.value;
    var width = psd.width.value;
    var top = psd.height.value;
    var left = psd.width.value;
    // trim off the top and left
    psd.trim(TrimType.TRANSPARENT, true, true, false, false);
    // the difference between original and trimmed is the amount of offset
    top -= psd.height.value;
    left -= psd.width.value;
    // trim the right and bottom
    psd.trim(TrimType.TRANSPARENT);
    // find center
    top += (psd.height.value / 2)
    left += (psd.width.value / 2)
    // unity needs center of image, not top left
    top = -(top - (height / 2));
    left -= (width / 2);

    height = psd.height.value;
    width = psd.width.value;

    psd.close(SaveOptions.DONOTSAVECHANGES);

    return {
        y: top,
        x: left,
        width: width,
        height: height
    };
}


function checkCanMerge(obj) {
    var layers = obj.layers;
    var canMerge = false;
    var j = 0;
    for (var i = 0; i < layers.length; i++) {
        var layer = layers[i];
        if (layer.visible && layer.typename == "ArtLayer") {
            j++;
        }
    }

    if (j > 1) canMerge = true;
    return canMerge;
}

function saveScenePng(psd, fileName, writeToDisk, depth, notMerge) {
    var layerCount = typeof (psd.layers) != "undefined" && psd.layers.length > 1;
    //alert(" saveScenePng.name = "+psd.name+" width= "+psd.width.value+" height"+psd.height.value )
    // we should now have a single art layer if all went well
    var canMerge = checkCanMerge(psd);
    if (!notMerge && canMerge) {
        psd.mergeVisibleLayers();
        writeErrorLine("mergeVisibleLayers saveScenePng.name = "+psd.name+" width= "+psd.width.value+" height"+psd.height.value);
        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert(" mergeVisibleLayers saveScenePng.name = "+psd.name+" width= "+psd.width.value+" height"+psd.height.value );
        }
    }

    // figure out where the top-left corner is so it can be exported into the scene file for placement in game
    // capture current size
    var height = psd.height.value;
    var width = psd.width.value;
    var top = psd.height.value;
    var left = psd.width.value;
    // trim off the top and left
    psd.trim(TrimType.TRANSPARENT, true, true, false, false);
    // the difference between original and trimmed is the amount of offset
    top -= psd.height.value;
    left -= psd.width.value;
    // trim the right and bottom
    psd.trim(TrimType.TRANSPARENT);
    // find center
    top += (psd.height.value / 2)
    left += (psd.width.value / 2)
    // unity needs center of image, not top left
    top = -(top - (height / 2));
    left -= (width / 2);

    height = psd.height.value;
    width = psd.width.value;

    var rec = {
        y: top,
        x: left,
        width: width,
        height: height
    };

    /** 
    // save the scene data
    if (!notMerge) {
        writeNewLine("<position>", depth);
        writeLine("<x>" + rec.x + "</x>", depth);
        writeLine("<y>" + rec.y + "</y>", depth);
        writeLine("</position>", depth);

        writeNewLine("<size>", depth);
        writeLine("<width>" + rec.width + "</width>", depth);
        writeLine("<height>" + rec.height + "</height>", depth);
        writeLine("</size>", depth);
        // alert("saveScenePng notMerge = false x="+ rec.x+" y="+rec.y+" saveScenePng.name = "+fileName);
    }
    */
    if (writeToDisk) {
        //alert(" writeToDisk saveScenePng.name = "+fileName+"psd.name="+psd.name+" width="+psd.width.value+" height="+psd.height.value )
        // save the image
        var pngFile = new File(destinationFolder + "/" + fileName + ".png");
        //var pngSaveOptions = new PNGSaveOptions();
        //psd.saveAs(pngFile, pngSaveOptions, true, Extension.LOWERCASE);

        var pngSaveOptions = new ExportOptionsSaveForWeb();
        pngSaveOptions.format = SaveDocumentType.PNG;
        pngSaveOptions.PNG8 = false;
        psd.exportDocument(pngFile, ExportType.SAVEFORWEB, pngSaveOptions);

    }
    psd.close(SaveOptions.DONOTSAVECHANGES);

}


function syntaxSugar(name)
{
    return name.replace("@mb:","@#");
}

function checkLayerName(fileName)
{
    var idx1 = fileName.indexOf("@mb")
    var idx2 = fileName.indexOf("@mb:")
    var id1 = fileName.indexOf("@")

    if(idx1>=0 && idx2<0)
    {
        writeErrorLine("warning 图层:["+fileName+"] 模板命名错误,缺少':'应该是 @mb:模板名");
    }

    if(id1>0)
    {
        var tmpName = fileName.substring(0, fileName.indexOf("@"));  //截取@之前的字符串作为图片的名称。
        if(fileName.indexOf(" ")>=0)
        {
            writeErrorLine("warning 图层:["+fileName+"]命名错误,包含空格,请去掉空格["+tmpName+"]");
        }
    }
}

function makeValideTextureName(fileName) {
    var validName = fileName.replace(/^\s+|\s+$/gm, ''); // trim spaces

    validName = validName.replace(/[\\\*\/\?:"\|<>]/g, ''); // remove characters not allowed in a file name
    //validName = validName.replace(/[ ]/g, '_'); // replace spaces with underscores, since some programs still may have troubles with them
    var tempIndex = validName.indexOf("#") //第一个#后表示模板名
    var nameIndex = validName.indexOf("@") //第一个@前面表示真名    name@被截取#moban
    if (nameIndex >= 0) {
        if (tempIndex >= 0) {
            var tmpName = validName.substring(0, nameIndex);  //截取@之前的字符串作为图片的名称。
            validName = validName.replace(/[ ]/g, '_');
            validName = tmpName + validName.substring(tempIndex); //截取#之后的字符
        }
        else
        {
            validName = validName.substring(0, nameIndex);  //截取@之前的字符串作为图片的名称。
            validName = validName.replace(/[ ]/g, '_');
        }
    }

    //截取第一个空格前的名字作为真名 处理类似问题：iconName 拷贝 1
    var arr = validName.split(" ");
    validName = arr[0];
    if (validName == "") {
        validName = getTemplateName(fileName)
    }
    return validName
}

function getTagList(fileName) {
    var array = fileName.split("@");
    if (array.length > 1) {
        fileName = array[1];
        array = fileName.split("_");
        if (array.length > 0) {
            var str = "";
            var item;
            for (var i = 0; i < array.length; i++) {
                item = array[i];
                str += "<string>" + item + "</string>";
                if (item.toLowerCase().lastIndexOf("9s") >= 0)
                    str += "<string>Slice</string>";

            }
            return str;
        }
    }

    return "";
}

function isTemplate(fileName) {
   return fileName.search("#") >= 0 ;
}

function getTemplateType(fileName) {
    var idx = fileName.indexOf("@#"); //可以缩放的模板@#
    if (idx >= 0) {
        return "Moban";
    } else {
        return "Customer";
    }
}

function getTemplateName(fileName) {
    var idx = fileName.indexOf("#"); //第一个#号后面为模板名字
    if (idx >= 0) {
        var array = fileName.split("#");
        if (array.length > 1) {
            array = array[1].split(" ");//不能带空格
            if (array[0])
                return array[0];
        }
    }
    // alert("layer:"+layer.name+" Template Name is Empty .  \r\n you should name like is 'name#templateName' ");
    return "";
}

function getTypeName(fileName) {
    var array = fileName.split("@");
    if (array.length > 1) {
        var array1 = array[1].split("#");
        if (array1.length > 1)
            return array1[0];

        array = array[1].split(" ");//不能带空格
        if (!array[0]) return "Default";

        return array[0];
    } else {
        return "Default";
    }
}

//检查是否需要导出
function checkShouldExport(fileName) {
    var idx = fileName.lastIndexOf("@");
    var hIdx = fileName.toLowerCase().lastIndexOf("dotexport");
    //    var hIdx1 = fileName.lastIndexOf("Size");
    //    hIdx = hIdx>hIdx1?hIdx:hIdx1;
    return hIdx < idx || idx == -1;
}

//检查是否需要保存
function checkShouldSavePng(fileName) {
    var idx = fileName.lastIndexOf("@");
    var hIdx = fileName.toLowerCase().lastIndexOf("hide");
    var hIdx1 = fileName.toLowerCase().lastIndexOf("ref");
    hIdx = hIdx > hIdx1 ? hIdx : hIdx1;
    return hIdx < idx || idx == -1;
}

//************************************
function char2Type(charId) {
    return app.charIDToTypeID(charId);
}

function getActiveLayerDescriptor() {
    var ref = new ActionReference();
    ref.putEnumerated(char2Type("Lyr "), char2Type("Ordn"), char2Type("Trgt"));
    return executeActionGet(ref);
}


function disableEffect() {
    var idsetd = charIDToTypeID("setd");
    var desc79 = new ActionDescriptor();
    var idnull = charIDToTypeID("null");
    var ref87 = new ActionReference();
    var idPrpr = charIDToTypeID("Prpr");
    var idlfxv = charIDToTypeID("lfxv");
    ref87.putProperty(idPrpr, idlfxv);
    var idDcmn = charIDToTypeID("Dcmn");
    var idOrdn = charIDToTypeID("Ordn");
    var idTrgt = charIDToTypeID("Trgt");
    ref87.putEnumerated(idDcmn, idOrdn, idTrgt);
    desc79.putReference(idnull, ref87);
    var idT = charIDToTypeID("T   ");
    var desc80 = new ActionDescriptor();
    var idlfxv = charIDToTypeID("lfxv");
    desc80.putBoolean(idlfxv, false);
    var idlfxv = charIDToTypeID("lfxv");
    desc79.putObject(idT, idlfxv, desc80);
    executeAction(idsetd, desc79, DialogModes.NO);
}

function enableEffect() {
    var idsetd = charIDToTypeID("setd");
    var desc77 = new ActionDescriptor();
    var idnull = charIDToTypeID("null");
    var ref86 = new ActionReference();
    var idPrpr = charIDToTypeID("Prpr");
    var idlfxv = charIDToTypeID("lfxv");
    ref86.putProperty(idPrpr, idlfxv);
    var idDcmn = charIDToTypeID("Dcmn");
    var idOrdn = charIDToTypeID("Ordn");
    var idTrgt = charIDToTypeID("Trgt");
    ref86.putEnumerated(idDcmn, idOrdn, idTrgt);
    desc77.putReference(idnull, ref86);
    var idT = charIDToTypeID("T   ");
    var desc78 = new ActionDescriptor();
    var idlfxv = charIDToTypeID("lfxv");
    desc78.putBoolean(idlfxv, true);
    var idlfxv = charIDToTypeID("lfxv");
    desc77.putObject(idT, idlfxv, desc78);
    executeAction(idsetd, desc77, DialogModes.NO);
}

function getArtNodeBounds(node) {
    if (LayerKind.TEXT == node.kind)
        return getTextInfo(node);
    else if (LayerKind.SMARTOBJECT == node.kind)
        return getSmartInfo(node);
    else
        return getImageInfo(node);
}

function getImageInfo(node) {
    return { l: node.boundsNoEffects[0].value, t: node.boundsNoEffects[1].value, r: node.boundsNoEffects[2].value, b: node.boundsNoEffects[3].value };
}

function getSmartInfo(node) {
    duppedPsd.activeLayer = node
    app.activeDocument.activeLayer.rasterize(RasterizeType.ENTIRELAYER);

    return getImageInfo(node);
    // var r = new ActionReference();
    // r.putEnumerated(charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
    // var d = executeActionGet(r).getObjectValue(stringIDToTypeID("smartObjectMore")).getList(stringIDToTypeID("transform"))

    // var xs = [d.getUnitDoubleValue(0), d.getUnitDoubleValue(2), d.getUnitDoubleValue(4), d.getUnitDoubleValue(6)];
    // var ys = [d.getUnitDoubleValue(1), d.getUnitDoubleValue(3), d.getUnitDoubleValue(5), d.getUnitDoubleValue(7)];

    // var l = Math.min(xs[0], Math.min(xs[1], Math.min(xs[2], xs[3])));
    // var r = Math.max(xs[0], Math.max(xs[1], Math.max(xs[2], xs[3])));
    // var t = Math.min(ys[0], Math.min(ys[1], Math.min(ys[2], ys[3])));
    // var b = Math.max(ys[0], Math.max(ys[1], Math.max(ys[2], ys[3])));
    // // alert("l:" + l + " t:" + t + " r:" + r + " b:" + b);
    // return { l: l, t: t, r: r, b: b }
}

function getTextInfo(node) {
    disableEffect();
    duppedPsd.activeLayer = node;
    var ref = new ActionReference()
    ref.putEnumerated(charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
    var bounds = app.executeActionGet(ref).getObjectValue(stringIDToTypeID('bounds'));
    enableEffect();

    var left = bounds.getUnitDoubleValue(stringIDToTypeID('left'));
    var top = bounds.getUnitDoubleValue(stringIDToTypeID('top'));
    var right = bounds.getUnitDoubleValue(stringIDToTypeID('right'));
    var bottom = bounds.getUnitDoubleValue(stringIDToTypeID('bottom'));
    return { l: left, t: top, r: right, b: bottom };
}



function getTextScale(node) {
    duppedPsd.activeLayer = node;
    var ref = new ActionReference()
    ref.putEnumerated(charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
    var scale = { x: 1, y: 1 };
    var desc = app.executeActionGet(ref).getObjectValue(stringIDToTypeID('textKey'))
    if (desc.hasKey(stringIDToTypeID('transform'))) {
        var transform = desc.getObjectValue(stringIDToTypeID('transform'))
        scale.x = transform.getUnitDoubleValue(stringIDToTypeID('xx'))
        scale.y = transform.getUnitDoubleValue(stringIDToTypeID('yy'))
    }
    return scale;
}

function convertBounds2Pos(l, t, r, b) {
    var w = r - l;
    var h = b - t;
    var x = l + w / 2 - psdWHalf;
    var y = -(t + h / 2 - psdHHalf);
    return { x: x, y: y, w: w, h: h };
}

function getLayerBounds(obj) {
    if (obj.typename == "ArtLayer")
        return getArtNodeBounds(obj);
    var bounds = { l: 3000, t: 3000, r: -3000, b: -3000 };
    for (var i = obj.layers.length - 1; 0 <= i; i--) {
        var layer = obj.layers[i];
        var childBounds = getLayerBounds(layer);
        bounds.l = Math.min(bounds.l, childBounds.l);
        bounds.t = Math.min(bounds.t, childBounds.t);
        bounds.r = Math.max(bounds.r, childBounds.r);
        bounds.b = Math.max(bounds.b, childBounds.b);
    }
    return bounds;
}

function getLayerPosInfo(obj) {
    var bounds = getLayerBounds(obj);
    return convertBounds2Pos(bounds.l, bounds.t, bounds.r, bounds.b);
}

function getPosBoundsXmlInfo(x, y, w, h, depth) {
    writeNewLine("<position><x>" + x + "</x><y>" + y + "</y></position>", depth);
    writeNewLine("<size><width>" + w + "</width><height>" + h + "</height></size>", depth);
}

function setArtNodeXmlInfo(node, depth) {
    var posInfo = getLayerPosInfo(node);
    getPosBoundsXmlInfo(posInfo.x, posInfo.y, posInfo.w, posInfo.h, depth)
}

function move(artnode, x, y) {
    var Position = artnode.bounds;
    var deltaX = x - Position[0];
    var deltaY = y - Position[1];

    artnode.translate(deltaX, deltaY);
}

function inplacePast() {
    var idpast = charIDToTypeID("past");
    var desc13 = new ActionDescriptor();
    var idinPlace = stringIDToTypeID("inPlace");
    desc13.putBoolean(idinPlace, true);
    var idAntA = charIDToTypeID("AntA");
    var idAnnt = charIDToTypeID("Annt");
    var idAnno = charIDToTypeID("Anno");
    desc13.putEnumerated(idAntA, idAnnt, idAnno);
    var idAs = charIDToTypeID("As  ");
    var idPxel = charIDToTypeID("Pxel");
    desc13.putClass(idAs, idPxel);
    executeAction(idpast, desc13, DialogModes.NO);
}

/******************************文本*********************************************************************************************************************************************************************************/

function changeToHex(rgbTxt) {
    var value = "";
    for (var i = 0, len = rgbTxt.length; i < len; i++) {
        var string = rgbTxt[i].toString(16);
        if (string.length < 2) {
            string = "0" + string;
        }
        value += string;
    }
    return value;
}


function descToColorList(colorDesc, colorPath) {
    var i, rgb = ["'Rd  '", "'Grn '", "'Bl  '"];
    var rgbTxt = [];

    colorDesc = colorDesc.getObjectValue(colorPath);
    if (!colorDesc) {
        return null;
    }

    for (var j = 0; j < 3; j++) {
        var c = colorDesc.getInteger(colorDesc.getKey(j));//getEnumerationValue(stringIDToTypeID(rgb[i]));
        rgbTxt.push(c);
    }

    return rgbTxt;
}

function getOutline(layerDesc)//layerDesc参数即上文通过layer获得的ActionDescriptor类型对象
{
    var isEffectVisible = layerDesc.getBoolean(stringIDToTypeID("layerFXVisible"));//判断图层是否有可见效果
    if (!isEffectVisible) {
        return "";
    }
    var lfxDesc = layerDesc.getObjectValue(stringIDToTypeID("layerEffects"));//获得图层效果属性
    if (!lfxDesc || !lfxDesc.hasKey(stringIDToTypeID("frameFX"))) {
        return "";
    }

    var dsDesc = lfxDesc ? lfxDesc.getObjectValue(stringIDToTypeID("frameFX")) : null;//获得图层描边属性
    //var dsDesc = lfxDesc ? lfxDesc.getString(stringIDToTypeID("dropShadow")) : null;//获得图层投影属性

    if (dsDesc == null) {
        return "";
    }
    var enable = dsDesc.getBoolean(stringIDToTypeID("enabled"));//判断描边/投影是否启用
    if (!enable) {
        return "";
    }
    var result = dsDesc.getUnitDoubleValue(stringIDToTypeID('size'));
    var rgbTxt = descToColorList(dsDesc, stringIDToTypeID("color"));//获得图层描边/投影颜色
    return result + "_" + changeToHex(rgbTxt);//转换成16进制
}


function getShadow(layerDesc)//layerDesc参数即上文通过layer获得的ActionDescriptor类型对象
{
    var isEffectVisible = layerDesc.getBoolean(stringIDToTypeID("layerFXVisible"));//判断图层是否有可见效果
    if (!isEffectVisible) {
        return "";
    }
    var lfxDesc = layerDesc.getObjectValue(stringIDToTypeID("layerEffects"));//获得图层效果属性
    if (!lfxDesc || !lfxDesc.hasKey(stringIDToTypeID("dropShadow"))) {
        return "";
    }

    var dsDesc = lfxDesc ? lfxDesc.getObjectValue(stringIDToTypeID("dropShadow")) : null;//获得图层阴影属性

    if (dsDesc == null) {
        return "";
    }
    var enable = dsDesc.getBoolean(stringIDToTypeID("enabled"));//判断描边/投影是否启用
    if (!enable) {
        return "";
    }
    var result = dsDesc.getUnitDoubleValue(stringIDToTypeID('distance'));
    var rgbTxt = descToColorList(dsDesc, stringIDToTypeID("color"));//获得图层描边/投影颜色
    return result + "_" + changeToHex(rgbTxt) + "_" + dsDesc.getUnitDoubleValue(stringIDToTypeID('opacity')) + "_" + dsDesc.getUnitDoubleValue(stringIDToTypeID('localLightingAngle')) + "_" + dsDesc.getUnitDoubleValue(stringIDToTypeID('blur'));//转换成16进制
}

function getGradientFill(layerDesc) {
    var isEffectVisible = layerDesc.getBoolean(stringIDToTypeID("layerFXVisible"));
    if (!isEffectVisible) {
        return "";
    }
    var lfxDesc = layerDesc.getObjectValue(stringIDToTypeID("layerEffects"));
    if (!lfxDesc || !lfxDesc.hasKey(stringIDToTypeID("gradientFill"))) {
        return "";
    }

    var dsDesc = lfxDesc ? lfxDesc.getObjectValue(stringIDToTypeID("gradientFill")) : null;
    if (dsDesc == null) {
        return "";
    }
    var enable = dsDesc.getBoolean(stringIDToTypeID("enabled"));
    if (!enable) {
        return "";
    }

    var graDesc = dsDesc.getObjectValue(stringIDToTypeID("gradient"));

    var colorList = graDesc.getList(stringIDToTypeID("colors"));//ps中可以有多个渐变颜色，unity中一般两个
    var result = "";

    for (var j = 0; j < colorList.count; j++) {
        var c = colorList.getObjectValue(j);//getEnumerationValue(stringIDToTypeID(rgb[i]));
        var rgbTxt = descToColorList(c, stringIDToTypeID("color"));
        result += changeToHex(rgbTxt) + ",";
    }

    result = result.substr(0, result.length - 1);
    return result;
}



/***************************************************************************************************************************************************************************************************************/
//对称的图片处理，切一半
//2017.01.10
//by zs

// 裁切 基于透明像素
function trim(doc) {
    doc.trim(TrimType.TRANSPARENT, true, true, true, true);
}

// 裁剪左半部分
function cutLeftHalf(doc, layerName, saveToDisk, depth) {
    doc.mergeVisibleLayers();

    trim(doc);
    var _obj = doc.activeLayer

    var width = doc.width;
    var height = doc.height;
    var side = width / 2;

    var region = Array(Array(0, height), Array(0, 0), Array(side, 0), Array(side, height));

    var selectRect = doc.selection.select(region);
    doc.selection.copy();
    var newStem = doc.paste();
    newStem.name = layerName;

    var deltaX = 0;
    var deltaY = 0;
    if (region[0][0] != 0) {
        deltaX = -(width - side * 2);
    }
    newStem.translate(deltaX, deltaY);

    _obj.visible = false;
    trim(doc);
    saveScenePng(doc, layerName, saveToDisk, depth, true);
    // exportHalfImage(doc,"LeftHalf");
}

// 裁剪下半部分
function cutBottomHalf(doc, layerName, saveToDisk, depth) {
    doc.mergeVisibleLayers();

    trim(doc);
    var _obj = doc.activeLayer
    var width = doc.width;
    var height = doc.height;
    var side = height / 2;

    //var region = Array(Array(0,side),Array(0,0),Array(width,0),Array(width,side));
    var region = Array(Array(0, height), Array(0, side), Array(width, side), Array(width, height));

    var selectRect = doc.selection.select(region);
    doc.selection.copy();
    var newStem = doc.paste();
    newStem.name = layerName;

    var deltaX = 0;
    var deltaY = 0;
    if (region[0][1] != side) {
        deltaY = -(height - side * 2);
    }
    newStem.translate(deltaX, deltaY);

    _obj.visible = false;

    trim(doc);
    saveScenePng(doc, layerName, saveToDisk, depth, true);
    //exportHalfImage(doc,"UpHalf");
}

// 裁剪左下四分之一
function cutQuarter(doc, layerName, saveToDisk, depth) {
    doc.mergeVisibleLayers();

    trim(doc);
    var _obj = doc.activeLayer
    var width = doc.width;
    var height = doc.height;
    var side = height / 2;

    var region = Array(Array(0, height), Array(0, height / 2), Array(width / 2, height / 2), Array(width / 2, height));

    var selectRect = doc.selection.select(region);
    doc.selection.copy();
    var newStem = doc.paste();
    newStem.name = layerName;

    var deltaX = 0;
    var deltaY = 0;
    if (region[0][1] != side) {
        deltaY = -(height - side * 2);
    }
    newStem.translate(deltaX, deltaY);

    _obj.visible = false;

    trim(doc);
    saveScenePng(doc, layerName, saveToDisk, depth, true);
}

function exportHalfImage(psd, halfType, saveToDisk, depth) {
    hideAllLayers(psd);

    var layerName = "";
    for (var i = psd.layers.length - 1; 0 <= i; i--) {
        layerName = psd.layers[i].name;
        if (layerName.match(halfType)) {
            psd.layers[i].visible = true;
            saveScenePng(psd, layerName, saveToDisk, depth, true);
        }
    }
}


/***************************************************************************************************************************************************************************************************************/
//九宫格切图
//2017.01.13
//by HuangLang

function _9sliceCutImg(layerName, w, h, depth) {
    // 创建图层组
    var width = w;
    var height = h;
    var re = /\s*_9S(\:\d+)+/g;
    var getStr = ""
    var result = layerName.match(re)
    if (result) {
        getStr = result[0]
    } else {
        writeErrorLine("Error 图层：" + layerName + "的九宫格格式不对！应为_9S:XX或:XX:XX:XX:XX");
        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert("Error 图层：" + layerName + "的九宫格格式不对！应为_9S:XX或:XX:XX:XX:XX");
        }
        return;
    }

    var nums = getStr.split(":")

    if (nums.length == 2) {
        for (var j = 0; j < slicePaddingArr.length; j++) {
            sliceOriArr[j] = parseInt(nums[1])
            slicePaddingArr[j] = parseInt(nums[1])
        }
    }
    else if (nums.length == 5) {
        for (var j = 0; j < slicePaddingArr.length; j++) {
            var num = parseInt(nums[j + 1])
            sliceOriArr[j] = num
            if (num == 0) {
                if ((j + 1) % 2 == 0) {
                    num = parseInt(height / 2)

                } else {

                    num = parseInt(width / 2)
                }
            }
            slicePaddingArr[j] = num
        }
    } else {
        writeErrorLine("Error 图层：" + layerName + "的九宫格格式不对！应为_9S:XX或:XX:XX:XX:XX");
        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert("Error 图层：" + layerName + "的九宫格格式不对！应为_9S:XX或:XX:XX:XX:XX");
        }
        return;
    }


    writeNewLine("<arguments>", depth);
    writeLine("<string>" + sliceOriArr[0] + "</string>");
    writeLine("<string>" + sliceOriArr[1] + "</string>");
    writeLine("<string>" + sliceOriArr[2] + "</string>");
    writeLine("<string>" + sliceOriArr[3] + "</string>");
    writeLine("</arguments>");
}