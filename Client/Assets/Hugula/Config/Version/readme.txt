版本模板文件配置说明
静态变量在生成version.json时候替换
{app.platform}   CUtils.platform
{app.version}   CodeVersion.APP_VERSION
{app.number}    CodeVersion.APP_NUMBER
{app.manifest_name}     Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME
{app.manifest_crc}  Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME crc code
{app.manifest_full_name}   like this {streaming_all}_{crc}.u3d

动态变量

HugulaSetting配置
{0} CUtils.platform
{1}  CodeVersion.APP_VERSION
{2} UnityEngine.SystemInfo.deviceUniqueIdentifier
{3} timeline
    verUrl = string.Format(host, CUtils.platform, CodeVersion.APP_VERSION, udid, CUtils.ConvertDateTimeInt(System.DateTime.Now));

本地version.txt Hugula/Config/Resources/version.txt
此配置仅仅用于fast包下载会根据Version配置自动生成

远端streaming_all.u3d文件下载路径为:Path.Combine(remoteVer.cdn_host[0], remoteVer.manifest_name);
远端资源下载为  :CUtils.PathCombine(remoteVer.cdn_hosts[i], HotResConfig.RESOURCE_FOLDER_NAME)