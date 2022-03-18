版本模板文件配置说明
静态变量在生成version.json时候替换
{app.platform}   CUtils.platform
{app.version}   CodeVersion.APP_VERSION
{app.number}    CodeVersion.APP_NUMBER
{app.manifest_name}     Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME
{app.manifest_crc}  Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME crc code
{app.manifest_full_name}   like this {streaming_all}_{crc}.u3d
动态变量
{0} CUtils.platform
{1} CodeVersion.APP_VERSION
cdn_host链接在运行时候会执行下面代码
remoteVer.cdn_host[i] = string.Format(cdn_hosts[i], CUtils.platform, CodeVersion.APP_VERSION);
eg 配置"cdn_host":["http://10.23.27.216/win/{1}"] 运行结果 "cdn_host":["http://10.23.27.216/win/0.5.3"]


HugulaSetting配置
{0} CUtils.platform
{1}  CodeVersion.APP_VERSION
{2} UnityEngine.SystemInfo.deviceUniqueIdentifier
{3} timeline
    verUrl = string.Format(host, CUtils.platform, CodeVersion.APP_VERSION, udid, CUtils.ConvertDateTimeInt(System.DateTime.Now));

本地version.txt Hugula/Config/Resources/version.txt
此配置仅仅用于fast包下载会根据Version配置自动生成