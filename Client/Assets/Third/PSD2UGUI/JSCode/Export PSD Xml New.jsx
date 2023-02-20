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

// var smartTargetLayer;//当前打开的智能图层对象
// var smartLayerBounds;//当前智能图层的bounds
var smartChildrenLayerRoot = {}; //
// var smartScale = { x: 1, y: 1 }; //当前缩放
// var smartAngle = 0; //当前旋转

var smartLayerSetArray = [];//智能图层嵌套堆栈

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

    exportAllLayers(duppedPsd, 0);
    //导出对比图
    writeNewLine("<Layer name='" + sourcePsdName.replace("\.psd", "").replace(" ", "-") + "' visible='true' layerName=\"" + sourcePsdName + "\" >", 1);
    writeNewLine("<type>RawImage</type>", 2);
    writeNewLine("<miniType>RawImage</miniType>", 2);
    writeNewLine("<position><x>0</x><y>0</y></position>", 2);
    writeNewLine("<size><width>" + psdW + "</width><height>" + psdH + "</height></size>", 2);
    writeNewLine("</Layer>", 1);
    writeNewLine("</layers>");
    writeNewLine("</PSDUI>");
    $.writeln(sceneData);


    // create export
    var destFile = destinationFolder + "/" + sourcePsdName.replace("\.psd", "_info.xml");
    saveFile(destFile, sceneData);

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
        // alert(" 导出成功！ 路径：" + destFile);
    }
}


function exportAllLayers(obj, depth) {
    if (typeof (obj) == "undefined") {
        return;
    }

    if (typeof (obj.layers) != "undefined" && obj.layers.length > 0) {
        for (var i = 0; i < obj.layers.length; i++) {
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

    var _layerName = syntaxSugar(_layer.name)
    var shouldExport = checkShouldExport(_layerName);
    if (!shouldExport) return; //不需要导出
    var isSMARTOBJECT = LayerKind.SMARTOBJECT == _layer.kind
    var validFileName = makeValideTextureName(_layerName);  // _layer.name.substring(0, _layer.name.search("@"));

    if (_layer.typename == "LayerSet") {
        checkLayerName(_layer.name);
        writeNewLine("<Layer id='" + _layer.id + "' name='" + validFileName + "' visible='" + _layer.visible + "' layerName=\"" + makeValideLayerName(_layer.name) + "\" index='" + _layer.itemIndex + "' >", depth + 1);
        if (isTemplate(_layerName)) {

            writeNewLine("<type>" + getTemplateType(_layerName) + "</type>", depth + 2);
            writeNewLine("<templateName>" + getTemplateName(_layerName) + "</templateName> ", depth + 2);
            writeNewLine("<miniType>" + getTypeName(_layerName) + "</miniType>", depth + 2);
            //老的模板需要判断对齐规则
            checkOldMobanRoot(_layer);
        }
        else {
            writeNewLine("<type>" + getTypeName(_layerName) + "</type>", depth + 2);
        }
        writeNewLine("<tag>" + getTagList(_layerName) + "</tag>", depth + 2);
        var smartInfo = smartChildrenLayerRoot[getLayerHashKey(_layer)];
        if (smartInfo) //有缩放
        {
            var smartScale = smartInfo.scale;
            smartChildrenLayerRoot[getLayerHashKey(_layer)] = null;
            writeNewLine("<scale><x>" + smartScale.x + "</x><y>" + smartScale.y + "</y></scale>", depth + 2);
            var smartLayerBounds = smartInfo.bounds;
            var posInfo = convertBounds2Pos(smartLayerBounds.l, smartLayerBounds.t, smartLayerBounds.r, smartLayerBounds.b);
            getPosBoundsXmlInfo(posInfo.x - offsetX, posInfo.y - offsetY, posInfo.w, posInfo.h, depth + 2)
        }
        writeNewLine("<layers>", depth + 2);

        for (var i = 0; i < _layer.layers.length; i++) {
            exportComponents(_layer.layers[i], depth + 3);
        }

        writeNewLine("</layers>", depth + 2);
        writeNewLine("</Layer>", depth + 1);

    } else if (isSMARTOBJECT) {

        if (checkSmartIsImage(_layerName)) { //如果是图片方式渲染
            writeNewLine("<Layer id='" + _layer.id + "' name='" + validFileName + "' visible='" + _layer.visible + "' layerName=\"" + makeValideLayerName(_layer.name) + "\"  index='" + _layer.itemIndex + "'  >", depth + 1);
            writeNewLine("<tag>" + getTagList(_layerName) + "</tag>", depth + 2);
            setArtNodeXmlInfo(_layer, depth + 2);
            app.activeDocument.activeLayer = _layer;
            exportImage(_layer, validFileName, depth + 2);
            writeNewLine("</Layer>", depth + 1);
        }
        else {

            var smartInfo = {};
            app.activeDocument.activeLayer = _layer;
            var smartLayerBounds = getSmartObjectBounds(_layer);//当前智能图层的尺寸
            var smartAngle = smartLayerBounds.angle;

            smartInfo["bounds"] = smartLayerBounds;
            smartInfo["angle"] = smartLayerBounds.angle;

            //设置偏移绝对坐标x偏移
            offsetX += smartLayerBounds.l;
            offsetY += smartLayerBounds.t;
            smartInfo["offset"] = { x: offsetX, y: offsetY };
            openSmartObject(_layer);
            smartInfo["layer"] = _layer;
            getLayersInSmartObject(_layer);//获取智能图层信息

            var smartTargetLayerSourceBounds = getOpenSmartSourceBounds(_layer)
            smartInfo["sourceBounds"] = smartTargetLayerSourceBounds;

            var scale = getScale(smartLayerBounds, smartTargetLayerSourceBounds);
            var smartScale = {
                toString: function () {
                    return "scale x:" + this.x + " y:" + this.y;
                }
            };
            smartScale.x = scale.scaleX.toFixed(2);
            smartScale.y = scale.scaleY.toFixed(2);
            smartInfo["scale"] = smartScale;

            var sRootLayer;
            for (var i = 0; i < _layer.layers.length; i++) {
                sRootLayer = _layer.layers[i];
                smartChildrenLayerRoot[getLayerHashKey(sRootLayer)] = smartInfo; //记录缩放根节点
                exportComponents(sRootLayer, depth + 3);
            }
            closeSmartObject(_layer);
            offsetX -= smartLayerBounds.l;
            offsetY -= smartLayerBounds.t;
        }
    }
    else if (_layer.typename = "ArtLayer" && !_layer.isBackgroundLayer) {

        checkLayerName(_layer.name);

        if (LayerKind.TEXT == _layer.kind) {
            writeNewLine("<Layer id='" + _layer.id + "'  name='" + validFileName + "' visible='" + _layer.visible + "' layerName=\"" + makeValideLayerName(_layer.name) + "\"  index='" + _layer.itemIndex + "'  >", depth + 1);
            writeNewLine("<tag>" + getTagList(_layerName) + "</tag>", depth + 2);
            setArtNodeXmlInfo(_layer, depth + 2);
            if (smartChildrenLayerRoot[_layer]) //有缩放
            {
                writeNewLine("<scale><x>" + smartScale.x + "</x><y>" + smartScale.y + "</y></scale>", depth + 2);
            }
            exportLabel(_layer, validFileName, depth + 2);
            writeNewLine("</Layer>", depth + 1);
        }
        else if (!isSMARTOBJECT && !isTemplate(_layerName)) //非智能图层非模板
        {
            writeNewLine("<Layer id='" + _layer.id + "' name='" + validFileName + "' visible='" + _layer.visible + "' layerName=\"" + makeValideLayerName(_layer.name) + "\"  index='" + _layer.itemIndex + "'  >", depth + 1);
            writeNewLine("<tag>" + getTagList(_layerName) + "</tag>", depth + 2);
            var smartInfo = smartChildrenLayerRoot[getLayerHashKey(_layer)];
            if (smartInfo) //图片缩放直接设置大小
            {
                smartChildrenLayerRoot[getLayerHashKey(_layer)] = null;
                var smartScale = smartInfo.scale;
                var posInfo = getLayerPosInfo(_layer);
                posInfo.x -= posInfo.w * 0.005 * (100 - smartScale.x);
                posInfo.y += posInfo.h * 0.005 * (100 - smartScale.y);
                posInfo.w = posInfo.w * smartScale.x * 0.01;
                posInfo.h = posInfo.h * smartScale.y * 0.01;
                getPosBoundsXmlInfo(posInfo.x, posInfo.y, posInfo.w, posInfo.h, depth + 2);
            } else {
                setArtNodeXmlInfo(_layer, depth + 2);
            }
            app.activeDocument.activeLayer = _layer;
            exportImage(_layer, validFileName, depth + 2);
            writeNewLine("</Layer>", depth + 1);
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
    var _layerName = syntaxSugar(obj.name)

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

}

function exportImage(obj, validFileName, depth) {

    var _layerName = syntaxSugar(obj.name)
    var oriName = obj.name.substring(obj.name.indexOf("@", obj.name.length));
    var lowerName = obj.name.toLowerCase()

    writeNewLine("<type>" + "Image" + "</type>", depth);
    writeNewLine("<opacity>" + obj.opacity + "</opacity>", depth);

    if (oriName.toLowerCase().search("_9s") >= 0) {
        var posInfo = getLayerPosInfo(obj);
        writeNewLine("<miniType>SliceImage</miniType>", depth);
        _9sliceCutImg(obj.name, posInfo.w, posInfo.h, depth);
        return;
    }
    else if (oriName.toLowerCase().search("_lefthalf") > 0 || oriName.toLowerCase().search("_lhalf") > 0)       //左右对称的图片切左边一半
    {
        writeNewLine("<miniType>" + "LeftHalfImage" + "</miniType>", depth);
        return;
    }
    else if (oriName.toLowerCase().search("_bottomhalf") > 0 || oriName.toLowerCase().search("_bhalf") > 0)     //上下对称的图片切底部一半
    {
        writeNewLine("<miniType>" + "BottomHalfImage" + "</miniType>", depth);
        return;
    }
    else if (oriName.toLowerCase().search("_quarter") > 0)     //上下左右均对称的图片切左下四分之一
    {
        writeNewLine("<miniType>" + "QuarterImage" + "</miniType>", depth);
        return;
    }
    else {
        writeNewLine("<miniType>" + "Image" + "</miniType>", depth);
    }

}

//******* 智能对象图层信息写入******* */


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