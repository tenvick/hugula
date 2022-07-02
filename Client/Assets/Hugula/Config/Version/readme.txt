版本模板文件配置说明
静态变量用于配置 Hugula/Config/Version/
在生成version.json时候替换
{app.platform}   CUtils.platform
{app.version}   CodeVersion.APP_VERSION
{app.number}    CodeVersion.APP_NUMBER
{app.manifest_name}     Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME
{app.manifest_crc}  Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME crc code
{app.manifest_full_name}   like this {streaming_all}_{crc}.u3d
{app.res_ver_folder}     Common.RES_VER_FOLDER

动态变量用于运行时 HotUpdate.cs
HugulaSetting配置
{0} CUtils.platform
{1} Common.RES_VER_FOLDER
{2} ver_name 带版本号的 Common.CRC32_VER_FILENAME
{3} CodeVersion.APP_VERSION

{udid} UnityEngine.SystemInfo.deviceUniqueIdentifier
{timeline} = CUtils.ConvertDateTimeInt(System.DateTime.Now)

ver_name: CUtils.InsertAssetBundleName(Common.CRC32_VER_FILENAME, $"_{CodeVersion.APP_VERSION}") eg:ver_0.5.1.json

本地version.txt Hugula/Config/Resources/ver.json
此配置用于获取版本失败的回退

Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME文件下载路径为:Path.Combine(remoteVer.cdn_host[0], remoteVer.manifest_name);
远端资源下载为  :CUtils.PathCombine(remoteVer.cdn_hosts[i], HotResConfig.RESOURCE_FOLDER_NAME)