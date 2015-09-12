using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_Msg : LuaObject {
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int constructor(IntPtr l) {
		try {
			int argc = LuaDLL.lua_gettop(l);
			Msg o;
			if(argc==1){
				o=new Msg();
				pushValue(l,o);
				return 1;
			}
			else if(argc==2){
				System.Byte[] a1;
				checkType(l,2,out a1);
				o=new Msg(a1);
				pushValue(l,o);
				return 1;
			}
			LuaDLL.luaL_error(l,"New object failed.");
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ToArray(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ToArray();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Debug(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.Debug();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ToCArray(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ToCArray();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Write(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Byte[] a1;
			checkType(l,2,out a1);
			self.Write(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteBoolean(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Boolean a1;
			checkType(l,2,out a1);
			self.WriteBoolean(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteByte(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Byte a1;
			checkType(l,2,out a1);
			self.WriteByte(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteChar(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Char a1;
			checkType(l,2,out a1);
			self.WriteChar(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteUShort(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.UInt16 a1;
			checkType(l,2,out a1);
			self.WriteUShort(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteUInt(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.UInt32 a1;
			checkType(l,2,out a1);
			self.WriteUInt(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteULong(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.UInt64 a1;
			checkType(l,2,out a1);
			self.WriteULong(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteShort(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			self.WriteShort(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteFloat(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Single a1;
			checkType(l,2,out a1);
			self.WriteFloat(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteInt(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			self.WriteInt(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteString(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			self.WriteString(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int WriteUTFBytes(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.String a1;
			checkType(l,2,out a1);
			self.WriteUTFBytes(a1);
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadBoolean(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadBoolean();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadByte(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadByte();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadChar(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadChar();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadUShort(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadUShort();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadUInt(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadUInt();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadULong(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadULong();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadShort(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadShort();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadInt(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadInt();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadFloat(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadFloat();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadString(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			var ret=self.ReadString();
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int ReadUTF(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Int32 a1;
			checkType(l,2,out a1);
			var ret=self.ReadUTF(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int Debug_s(IntPtr l) {
		try {
			System.Byte[] a1;
			checkType(l,1,out a1);
			var ret=Msg.Debug(a1);
			pushValue(l,ret);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Length(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			pushValue(l,self.Length);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Position(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			pushValue(l,self.Position);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_Position(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			System.Int64 v;
			checkType(l,2,out v);
			self.Position=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int get_Type(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			pushValue(l,self.Type);
			return 1;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static public int set_Type(IntPtr l) {
		try {
			Msg self=(Msg)checkSelf(l);
			int v;
			checkType(l,2,out v);
			self.Type=v;
			return 0;
		}
		catch(Exception e) {
			LuaDLL.luaL_error(l, e.ToString());
			return 0;
		}
	}
	static public void reg(IntPtr l) {
		getTypeTable(l,"Msg");
		addMember(l,ToArray);
		addMember(l,Debug);
		addMember(l,ToCArray);
		addMember(l,Write);
		addMember(l,WriteBoolean);
		addMember(l,WriteByte);
		addMember(l,WriteChar);
		addMember(l,WriteUShort);
		addMember(l,WriteUInt);
		addMember(l,WriteULong);
		addMember(l,WriteShort);
		addMember(l,WriteFloat);
		addMember(l,WriteInt);
		addMember(l,WriteString);
		addMember(l,WriteUTFBytes);
		addMember(l,ReadBoolean);
		addMember(l,ReadByte);
		addMember(l,ReadChar);
		addMember(l,ReadUShort);
		addMember(l,ReadUInt);
		addMember(l,ReadULong);
		addMember(l,ReadShort);
		addMember(l,ReadInt);
		addMember(l,ReadFloat);
		addMember(l,ReadString);
		addMember(l,ReadUTF);
		addMember(l,Debug_s);
		addMember(l,"Length",get_Length,null,true);
		addMember(l,"Position",get_Position,set_Position,true);
		addMember(l,"Type",get_Type,set_Type,true);
		createTypeMetatable(l,constructor, typeof(Msg));
	}
}
