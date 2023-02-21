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
#include "PSDUI Libs.jsx";

var sourcePsd;
var duppedPsd;
var destinationFolder;
var uuid;
var sourcePsdName;
var slicePaddingArr = new Array(0, 0, 0, 0)
var sliceOriArr = new Array(0, 0, 0, 0)
var textScaleArr = new Array(0, 0)

var depth = 0

var psdW;
var psdH;

var alreadyExportPngs = {}//本次已经导出的png名字


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
    duppedPsd = app.activeDocument.duplicate();
    psdWHalf = duppedPsd.width.value / 2;
    psdHHalf = duppedPsd.height.value / 2;
    psdW = duppedPsd.width.value;
    psdH = duppedPsd.height.value;


    // export layers
    sceneData = "";
    //导出对比图
    var pngSaveOptions = new ExportOptionsSaveForWeb();
    pngSaveOptions.format = SaveDocumentType.PNG;
    pngSaveOptions.PNG8 = false;
    var pngFile = File(destinationFolder + "/" + sourcePsdName.replace(" ", "-").replace("\.psd", "\.png"));
    duppedPsd.exportDocument(pngFile, ExportType.SAVEFORWEB, pngSaveOptions);

    //
    duppedPsd.activeLayer = duppedPsd.layers[duppedPsd.layers.length - 1];
    hideAllLayers(duppedPsd);
    exportAllLayers(duppedPsd, 0);

    duppedPsd.close(SaveOptions.DONOTSAVECHANGES);
    //create error tips
    if (errorData) {
        var destErrFile = destinationFolder + "/" + sourcePsdName.replace("\.psd", "_Error.txt");
        saveFile(destErrFile, errorData)

        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert("出错误信息 \n" + errorData);
            alert(" 导出错误信息！ 路径： \n" + destErrFile + "\n 点击确定打开文件");
            openFile(destErrFile);
        }
    }

    app.preferences.rulerUnits = savedRulerUnits;
    app.preferences.typeUnits = savedTypeUnits;

    if (app.playbackDisplayDialogs != DialogModes.NO) {
        alert(" 导出成功！  \n路径：" + destinationFolder + "\n 点击确定打开文件夹");
        openFolder(destinationFolder);
    }
}

function exportAllLayers(obj, depth) {
    if (typeof (obj) == "undefined") {
        return;
    }

    if (typeof (obj.layers) != "undefined" && obj.layers.length > 0) {
        for (var i = 0; i < obj.layers.length; i++) {
            exportLayer(obj.layers[i], 1);
        }
    }
    else {
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

    if (typeof (_layer) == "undefined") {
        return;
    }

    var _layerName = syntaxSugar(_layer.name)
    var shouldExport = checkShouldExport(_layerName);
    if (!shouldExport) return; //不需要导出
    var isSMARTOBJECT = LayerKind.SMARTOBJECT == _layer.kind

    var validFileName = makeValideTextureName(_layerName);  // _layer.name.substring(0, _layer.name.search("@"));
    if (_layer.typename == "LayerSet") {
        checkLayerName(_layer.name);
        if (isTemplate(_layerName)) {
            //老的模板需要判断对齐规则
            checkOldMobanRoot(_layer);
        }
        for (var i = 0; i < _layer.layers.length; i++) {
            exportComponents(_layer.layers[i], depth + 3);
        }
    } else if (isSMARTOBJECT) {
        if (checkSmartIsImage(_layerName)) {
            if(!containsChinese(validFileName))
            {
                app.activeDocument.activeLayer = _layer;
                exportImage(_layer, validFileName, depth + 2);
            }
        } else {
            app.activeDocument.activeLayer = _layer;
            openSmartObject(_layer);
            getLayersInSmartObject(_layer);//获取智能图层信息

            hideAllLayers(_layer);
            var sRootLayer;
            for (var i = 0; i < _layer.layers.length; i++) {
                sRootLayer = _layer.layers[i];
                exportComponents(sRootLayer, depth + 3);
            }
            closeSmartObject(_layer);
            hideAllLayers(_layer.parent);
        }
    }
    else if (_layer.typename = "ArtLayer" && !_layer.isBackgroundLayer) {

        checkLayerName(_layer.name);

        if (LayerKind.TEXT == _layer.kind) {
            exportLabel(_layer, validFileName, depth + 2);
        }
        else if (!isSMARTOBJECT && !isTemplate(_layerName) && !containsChinese(validFileName)) //非智能图层非模板
        {
            app.activeDocument.activeLayer = _layer;
            exportImage(_layer, validFileName, depth + 2);
        }
    }
}


function exportLabel(obj, validFileName, depth) {
    //有些文本如标题，按钮，美术用的是其他字体，可能还加了各种样式，需要当做图片切出来使用

    var lowerName = obj.name.toLowerCase()
    if (lowerName.search("_sprite") >= 0) {
        exportImage(obj, validFileName, depth);
        return;
    }
}


function exportImage(obj, validFileName, depth) {

    var saveToDisk = checkShouldSavePng(obj.name);
    if (!saveToDisk) {
        SetLayerVisible(obj, false);
        return;
    }
    var item = alreadyExportPngs[validFileName];
    if (item) {
        //本次已经导出。
        return;
    }

    alreadyExportPngs[validFileName] = true;//记录导出

    var oriName = obj.name.substring(obj.name.indexOf("@", obj.name.length));
    if (oriName.toLowerCase().search("_9s") >= 0) {
        if (saveToDisk) {
            SetLayerVisible(obj, true);
            _9sliceCutImg(duppedPsd.duplicate(), obj.name, validFileName, saveToDisk, depth);
            SetLayerVisible(obj, false);
        }
        return;
    }
    else if (oriName.toLowerCase().search("_lhalf") > 0)       //左右对称的图片切左边一半
    {
        if (saveToDisk) {
            SetLayerVisible(obj, true);
            cutLeftHalf(duppedPsd.duplicate(), obj, validFileName);
            SetLayerVisible(obj, false);
        }
        return;
    }
    else if (oriName.toLowerCase().search("_bhalf") > 0)     //上下对称的图片切底部一半
    {
        if (saveToDisk) {
            SetLayerVisible(obj, true);
            cutBottomHalf(duppedPsd.duplicate(), obj, validFileName);
            SetLayerVisible(obj, false);
        }
        return;
    }
    else if (oriName.toLowerCase().search("_quarter") > 0)     //上下左右均对称的图片切左下四分之一
    {
        if (saveToDisk) {
            SetLayerVisible(obj, true);
            cutQuarter(duppedPsd.duplicate(), obj, validFileName);
            SetLayerVisible(obj, false);
        }
        return;
    }
    else {
        if (saveToDisk) {
            SetLayerVisible(obj, true);
            saveScenePng(duppedPsd.duplicate(), obj, validFileName);
            SetLayerVisible(obj, false);
        }
    }

}

function hideAllLayers(obj) {
    hideLayerSets(obj);
}


function hideLayerSets(obj) {
    for (var i = obj.layers.length - 1; 0 <= i; i--) {
        if (obj.layers[i].typename == "LayerSet") {
            hideLayerSets(obj.layers[i]);
        }
        else {
            SetLayerVisible(obj.layers[i], false);
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
            SetLayerVisible(obj.layers[i], true);
        }
    }
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

function saveScenePng(psd, layer, fileName, notMerge) {
    var layerCount = typeof (psd.layers) != "undefined" && psd.layers.length > 1;
    // alert(" saveScenePng.name = " + fileName + " width= " + psd.width.value + " height" + psd.height.value + " layer" + layer.bounds)
    // we should now have a single art layer if all went well
    var canMerge = checkCanMerge(psd);
    if (!notMerge && canMerge) {
        psd.mergeVisibleLayers();
        writeErrorLine("mergeVisibleLayers saveScenePng.name = " + psd.name + " width= " + psd.width.value + " height" + psd.height.value);
        if (app.playbackDisplayDialogs != DialogModes.NO) {
            alert(" mergeVisibleLayers saveScenePng.name = " + psd.name + " width= " + psd.width.value + " height" + psd.height.value);
        }
    }
    psd.crop(layer.bounds);
    //psd.trim(TrimType.TRANSPARENT, true, true, true, true);
    //psd.trim(TrimType.TRANSPARENT);
    var pngFile = new File(destinationFolder + "/" + fileName + ".png");
    var pngSaveOptions = new ExportOptionsSaveForWeb();
    pngSaveOptions.format = SaveDocumentType.PNG;
    pngSaveOptions.PNG8 = false;
    psd.exportDocument(pngFile, ExportType.SAVEFORWEB, pngSaveOptions);
    if (app.playbackDisplayDialogs != DialogModes.NO) {
        // alert("导出图片成功 saveScenePng.name = " + fileName + " width= " + psd.width.value + " height" + psd.height.value);
    }
    psd.close(SaveOptions.DONOTSAVECHANGES);
}


//检查是否需要保存
function checkShouldSavePng(fileName) {
    var idx = fileName.lastIndexOf("@");
    var hIdx = fileName.toLowerCase().lastIndexOf("hide");
    var hIdx1 = fileName.toLowerCase().lastIndexOf("ref");
    hIdx = hIdx > hIdx1 ? hIdx : hIdx1;
    return hIdx < idx || idx == -1;
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

function move(artnode, x, y) {
    var Position = artnode.bounds;
    var deltaX = x - Position[0];
    var deltaY = y - Position[1];

    artnode.translate(deltaX, deltaY);
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
function cutLeftHalf(doc, layer, layerName) {
    var canMerge = checkCanMerge(doc);
    if (canMerge) {
        doc.mergeVisibleLayers();
    }

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

    SetLayerVisible(_obj, false);
    trim(doc);
    saveScenePng(doc, _obj, layerName, true);
}

// 裁剪下半部分
function cutBottomHalf(doc, layer, layerName) {
    var canMerge = checkCanMerge(doc);
    if (canMerge) {
        doc.mergeVisibleLayers();
    }

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

    SetLayerVisible(_obj, false);

    trim(doc);
    saveScenePng(doc, _obj, layerName);
}

// 裁剪左下四分之一
function cutQuarter(doc, layer, layerName) {
    var canMerge = checkCanMerge(doc);
    if (canMerge) {
        doc.mergeVisibleLayers();
    }

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

    SetLayerVisible(_obj, false);

    trim(doc);
    saveScenePng(doc, _obj, layerName, true);
}

function exportHalfImage(psd, halfType) {
    hideAllLayers(psd);

    var layerName = "";
    for (var i = psd.layers.length - 1; 0 <= i; i--) {
        layerName = psd.layers[i].name;
        if (layerName.match(halfType)) {
            SetLayerVisible(psd.layers[i], true);
            saveScenePng(psd, psd.layers[i], layerName, true);
        }
    }
}


/***************************************************************************************************************************************************************************************************************/
//九宫格切图
//2017.01.13
//by HuangLang

function _9sliceCutImg(doc, layerName, vaildName, saveToDisk, depth) {
    // 创建图层组
    var _obj = doc.activeLayer
    var stemGroup = doc.layerSets.add();
    stemGroup.name = layerName;
    //alert("_9sliceCutImgr 图层：" + layerName + " _obj"+reflectProperties(_obj));
    doc.mergeVisibleLayers();
    trim(doc);
    var width = doc.width;
    var height = doc.height;
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

    var _obj = doc.activeLayer
    //左下左上，右上右下
    //左下左上，右上右下
    var selRegion = Array(
        Array(Array(0, slicePaddingArr[1]), Array(0, 0), Array(slicePaddingArr[0], 0), Array(slicePaddingArr[0], slicePaddingArr[1])),
        Array(Array(width - slicePaddingArr[2], slicePaddingArr[1]), Array(width - slicePaddingArr[2], 0), Array(width, 0), Array(width, slicePaddingArr[1])),
        Array(Array(0, height), Array(0, height - slicePaddingArr[3]), Array(slicePaddingArr[0], height - slicePaddingArr[3]), Array(slicePaddingArr[0], height)),
        Array(Array(width - slicePaddingArr[2], height), Array(width - slicePaddingArr[2], height - slicePaddingArr[3]), Array(width, height - slicePaddingArr[3]), Array(width, height)),
    );
    for (var i = 0; i < selRegion.length; i++) {
        doc.activeLayer = _obj;
        // alert("select region "+selRegion[i])
        doc.selection.select(selRegion[i]);
        doc.selection.copy();
        inplacePast();
        var newStem = doc.activeLayer;
        newStem.name = vaildName;
        var logicOriX = 0;
        var logicOriY = 0;

        var logicX = 0;
        var logicY = 0;
        if (selRegion[i][0][0] != 0) {
            logicX = slicePaddingArr[0];
            logicOriX = width - slicePaddingArr[2];
        }
        if (selRegion[i][1][1] != 0) {
            logicY = slicePaddingArr[1];
            logicOriY = height - slicePaddingArr[3];
        }

        var setPosX = logicOriX - newStem.bounds[0] + logicX;
        var setPosY = logicOriY - newStem.bounds[1] + logicY;
        move(newStem, setPosX, setPosY);
    }

    SetLayerVisible(_obj, false);
    doc.mergeVisibleLayers();

    trim(doc);
    saveScenePng(doc, _obj, vaildName, true, depth, true);
}