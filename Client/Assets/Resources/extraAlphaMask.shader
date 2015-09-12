Shader "Transparent/extraAlphaMask" 
{
	Properties {
    _Color ("TintColor", Color) = (0.75,0.75,0.75,1)
    _MainTex ("Diffuse", 2D) = "white" {}
    _AlphaMap ("Opacity", 2D) = "white" {}
    _Mask("Mask", 2D) = "white" {}
	}
 
//	SubShader {
//		Lighting Off
//		ZWrite Off
//		Blend SrcAlpha OneMinusSrcAlpha
//		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "Queue"="Transparent"}
//		LOD 100
// 
//		CGPROGRAM
//       	#pragma surface surf Unlit 
// 
//	   	half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten) 
//	   	{
//	      half4 c;
//	      c.rgb = s.Albedo;
//	      c.a = s.Alpha;
//	      return c;
//	    }
//	 
//		sampler2D _MainTex;
//		sampler2D _AlphaMap;
//		sampler2D _Mask;
//		float4 _Color;
//	 
//		struct Input {
//			float2 uv_MainTex;
//		};
//	 
//		void surf (Input IN, inout SurfaceOutput o) {
//			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb;
//			o.Alpha = c.a * tex2D(_AlphaMap, IN.uv_MainTex).r * tex2D(_Mask, IN.uv_MainTex).r;
//		}
//		ENDCG
//	}

SubShader {
 		Cull Off
		Lighting Off
		ZWrite Off
		AlphaTest Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		LOD 100
 
	    Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _AlphaMap;
			sampler2D _Mask;
			float4 _Color;
			float4 _MainTex_ST;
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    return o;
			}

			half4 frag (v2f i) : COLOR
			{
			    half4 texcol = tex2D (_MainTex, i.uv);
			    half4 texopa = tex2D (_AlphaMap, i.uv);
			    half4 texmask = tex2D (_Mask, i.uv);
			    
			    return half4(texcol[0]*_Color[0],texcol[1]*_Color[1],texcol[2]*_Color[2], texopa[0]*texmask[0]);
			}
			ENDCG
	   	}

	}
 
	FallBack "Diffuse"
}
