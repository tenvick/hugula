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

//==================================log相关=====================
var errorData = "";
var sceneData = "";
var psdWHalf;
var psdHHalf;

var offsetX = 0; //智能图层文件夹绝对坐标x偏移
var offsetY = 0;//智能图层文件夹绝对坐标y偏移

var Imagekeywords =["size","hide","dotexport","_9s","_lhalf","_bhalf","_quarter"];

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

function writeErrorLine(line) {
    errorData += "\n" + line;
}

//保存文件
function saveFile(destFile, str) {
    var sFile = new File(destFile);
    sFile.encoding = "utf-8";   //写文件时指定编码，不然中文会出现乱码
    sFile.open('w');
    sFile.writeln(str);
    sFile.close();
}

//打开一个文件
function openFile(destFile) {
    var f = new File(destFile);
    f.open();
    f.execute();
}

function openFolder(destFolder) {
    var df = new Folder(destFolder);
    df.execute();
}

//====================================命名检测=======
function syntaxSugar(name) {
    return name.replace("@mb:", "@#");
}

function checkLayerName(fileName) {
    var idx1 = fileName.indexOf("@mb")
    var idx2 = fileName.indexOf("@mb:")
    var idx3 = fileName.indexOf("mb:")
    var id1 = fileName.indexOf("@")

    if (idx1 >= 0 && idx2 < 0) {
        writeErrorLine("Error 图层:       [" + fileName + "]          模板命名错误,缺少':'应该是 @mb:模板名");
    }

    if (idx3 >= 0 && idx2 < 0) {
        writeErrorLine("Error 图层:       [" + fileName + "]          模板命名错误,缺少':'应该是 @mb:模板名");
    }

    if (id1 > 0) {
        var tmpName = fileName.substring(0, fileName.indexOf("@"));  //截取@之前的字符串作为图片的名称。
        if (fileName.indexOf(" ") >= 0) {
            writeErrorLine("warning 图层:       [" + fileName + "]          命名错误,包含空格,已经替换为[" + tmpName.replace(" ", "_") + "]  ");
        }
    }
}

//老的模板需要判断mb_root
function checkOldMobanRoot(_layer) {
    if (_layer.name.indexOf("@mb:") >= 0) {
        var checked = false;
        for (var i = 0; i < _layer.layers.length; i++) {
            var layer = _layer.layers[i];
            if (layer.name == "mb_root") {
                checked = true;
            }
        }

        if (!checked) {
            writeErrorLine("Error 模板文件夹:[" + _layer.name + "] 没有 mb_root 图层，请添加。")
        }
    }
}


function makeValideLayerName(name) {
    var validName = name.replace(/^\s+|\s+$/gm, ''); // trim spaces
    validName = validName.replace(/[\\\*\/\?:"\|<>]/g, '');
    validName=validName.replace(/[ ]/g, '_');
    return validName;
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
        else {
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
    return fileName.search("#") >= 0;
}

function getTemplateType(fileName) {
    var idx = fileName.indexOf("@#"); //可以缩放的模板!#
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


//检测智能图层是否应该作为普通图片
function checkSmartIsImage(layerName) {
    var idx =-1;
    if (isTemplate(layerName))
        return false;
    else if (layerName.indexOf("@grp")>=0) //group
    {
        return false;
    }else if((idx =layerName.indexOf("@"))>=0) //特殊判断
    {
        var extendName = layerName.substring(idx,layerName.length).toLowerCase();

        for(var i=0;i<Imagekeywords.length;i++)
        {
            if(extendName.indexOf(Imagekeywords[i])>=0) //图片
            {
                return true;
            }
        }

        return false;
    }
    return true;
}

function getLayerHashKey(layer) {
    return layer.id + "-" + layer.itemIndex + "-" + makeValideLayerName(layer.name);
}

//==================================文本相关===================

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
        return obj.textItem.leading.value * scale.y;
    } catch (error) {
        return "X";
    }
}


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

//**************************布局与bounds相关信息***************************** */
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
        return getSmartObjectBounds(node);
    else
        return getImageInfo(node);
}

function getImageInfo(node) {
    return {
        l: node.bounds[0].value, t: node.bounds[1].value, r: node.bounds[2].value, b: node.bounds[3].value,
        toString: function () {
            return "L:" + this.l + " T:" + this.t + " R:" + this.r + " B:" + this.b;
        }
    };
}

function getSmartObjectBounds(node) {
    try {
        app.activeDocument.activeLayer = node
        var r = new ActionReference();
        r.putEnumerated(charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
        var d;
        try { d = executeActionGet(r); } catch (e) { alert(e); return; }
        try { d = d.getObjectValue(stringIDToTypeID("smartObjectMore")); } catch (e) { alert(e); return; }
        try { d = d.getList(stringIDToTypeID("transform")); } catch (e) { alert(e); return; }
        var w = d.getDouble(0) - d.getDouble(2);
        var h = d.getDouble(1) - d.getDouble(7);
        var angle = Math.atan(h / w) * 180.0 / Math.PI;
        return {
            l: d.getDouble(0), t: d.getDouble(1), r: d.getDouble(2), b: d.getDouble(7), angle: angle,
            toString: function () {
                return "L:" + this.l + " T:" + this.t + " R:" + this.r + " B:" + this.b + " angle:" + angle + "\nw:" + w + "h:" + h +
                    "\n" + [[d.getDouble(0), d.getDouble(1)],
                    [d.getDouble(2), d.getDouble(3)],
                    [d.getDouble(4), d.getDouble(5)],
                    [d.getDouble(6), d.getDouble(7)]];
            }
        };
    }
    catch (e) { alert(e); }
}

function boundsToBoxString(bounds) {
    return "x:" + bounds.l + "y:" + bounds.t + "width:" + (bounds.r - bounds.l) + "height:" + (bounds.b - bounds.t);
}

function posToBoxString(pos) {
    return "x:" + pos.x + "y:" + pos.y + "width:" + pos.w + "height:" + pos.h;
}

function get_smart_object_corners(node) {
    try {
        app.activeDocument.activeLayer = node
        var r = new ActionReference();
        r.putEnumerated(charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
        var d;
        try { d = executeActionGet(r); } catch (e) { alert(e); return; }
        try { d = d.getObjectValue(stringIDToTypeID("smartObjectMore")); } catch (e) { alert(e); return; }
        try { d = d.getList(stringIDToTypeID("transform")); } catch (e) { alert(e); return; }

        var ret = [[d.getDouble(0), d.getDouble(1)],
        [d.getDouble(2), d.getDouble(3)],
        [d.getDouble(4), d.getDouble(5)],
        [d.getDouble(6), d.getDouble(7)]];
        return ret;
    }
    catch (e) { alert(e); }
}

function get_smart_object_angle(node) {
    try {
        app.activeDocument.activeLayer = node
        var r = new ActionReference();
        r.putEnumerated(charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
        var d;
        try { d = executeActionGet(r); } catch (e) { alert(e); return; }
        try { d = d.getObjectValue(stringIDToTypeID("smartObjectMore")); } catch (e) { alert(e); return; }
        try { d = d.getList(stringIDToTypeID("transform")); } catch (e) { alert(e); return; }
        var w = d.getDouble(0) - d.getDouble(2);
        var h = d.getDouble(1) - d.getDouble(3);
        var angle = Math.atan(h / w) * 180.0 / Math.PI;
        return angle;
    }
    catch (e) { alert(e); }
}


function getTextInfo(node) {
    disableEffect();
    app.activeDocument.activeLayer = node;
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
    app.activeDocument.activeLayer = node;
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
    var x = l + w / 2 - psdWHalf + offsetX;
    var y = -(t + h / 2 - psdHHalf + offsetY);
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

function getScale(bounds1, bounds2) {
    var scale = { scaleX: 100, scaleY: 100 };
    try {
        var w = (bounds1.r - bounds1.l) / (bounds2.r - bounds2.l) * 100;
        var h = (bounds1.b - bounds1.t) / (bounds2.b - bounds2.t) * 100;
        scale.scaleX = w;
        scale.scaleY = h;
    } catch (e) {
        alert(e);
        scale.scaleX = 100;
        scale.scaleY = 100;
    }
    return scale
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

//***************************图层显示隐藏*** */
//支持BackgroundLayer
function SetLayerVisible(layer, bl) {
    if (layer.isBackgroundLayer) //背景层不能隐藏
    {
        layer.isBackgroundLayer = false;
        layer.visible = bl;
        layer.isBackgroundLayer = true;
    }
    else {
        layer.visible = bl;
    }
}

//****************************智能图层相关******************* */
//smart layer must open
function getOpenSmartSourceBounds(obj) {
    var doc = app.activeDocument;

    return {
        l: 0, t: 0, r: doc.width.value, b: doc.height.value,
        toString: function () {
            return "L:" + this.l + " T:" + this.t + " R:" + this.r + " B:" + this.b;
        }
    };
}

function openSmartObject(smartObjectLayer) {
    if (smartObjectLayer.kind == LayerKind.SMARTOBJECT) {
        app.activeDocument.activeLayer = smartObjectLayer;
        // 如果当前图层是智能对象，就打开该智能对象图层
        var idplacedLayerEditContents = stringIDToTypeID("placedLayerEditContents");
        var desc2 = new ActionDescriptor();
        var idnull = charIDToTypeID("null");
        var ref1 = new ActionReference();
        var idcontentLayer = stringIDToTypeID("contentLayer");
        ref1.putClass(idcontentLayer);
        desc2.putReference(idnull, ref1);
        executeAction(idplacedLayerEditContents, desc2, DialogModes.NO);
    }
}

function closeSmartObject(smartObjectLayer) {
    if (smartObjectLayer.kind == LayerKind.SMARTOBJECT) {
        // app.activeDocument.activeLayer = smartObjectLayer;
        var idCls = charIDToTypeID("Cls ");
        var desc3 = new ActionDescriptor();
        var idSvng = charIDToTypeID("Svng");
        var idYsN = charIDToTypeID("YsN ");
        var idN = charIDToTypeID("N   ");
        desc3.putEnumerated(idSvng, idYsN, idN);
        executeAction(idCls, desc3, DialogModes.NO);
    }
}

//必须打开smart图层
function getLayersInSmartObject(smartObjectLayer) {
    var layers = app.activeDocument.layers;
    smartObjectLayer.layers = layers;
    return layers;
}



//****************反射读取属性******************** */
function reflectProperties(obj) {
    var props = obj.reflect.properties;
    var str = obj.toString();
    for (var i = 0; i < props.length; i++) {
        try {
            str += " \n" + props[i].name + "=" + obj[props[i].name];
        } catch (e) {
        }
    }

    return str;
}

function reflectMethods(obj) {
    var meths = obj.reflect.methods;
    var str = obj.toString();
    for (var i = 0; i < meths.length; i++) {
        try {
            str += " \n" + meths[i].name + "()";
        } catch (e) {
        }
    }

    return str;
}