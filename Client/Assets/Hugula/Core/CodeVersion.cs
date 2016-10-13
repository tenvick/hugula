using UnityEngine;
using System.Collections;

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
		public const uint CODE_VERSION = 0x0000c;


	}

}