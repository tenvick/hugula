using UnityEngine;

namespace Hugula
{
	/// <summary>
	/// 代码版本号Res verion.
	/// </summary>
	[SLua.CustomLuaClass]
	public class CodeVersion {

		/// <summary>
		/// 代码版本号码
		/// 如果代码有更新请手动改写，热更新的时候此版本号不一致会导致强更。
		/// </summary>
		#if UNITY_IOS || UNITY_IPHONE
			public const uint CODE_VERSION = 0x00001;
		#elif UNITY_ANDROID
			public const uint CODE_VERSION = 0x00001;
		#else
			public const uint CODE_VERSION = 0x00001;
		#endif
	}

}