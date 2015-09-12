Shader "Transparent/extraAlpha UI" 
{
	Properties {
    _MainTex ("Diffuse", 2D) = "white" {}
    _AlphaMap ("Opacity", 2D) = "white" {}
	}

 	SubShader {
 		Cull Off
		Lighting Off
		ZWrite Off
		AlphaTest Off
		Fog { Mode Off }
		Offset -1, -1
		ColorMask RGB
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
			float4 _MainTex_ST;
			
			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    half4 color : COLOR;
			    float2  uv : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.color = v.color;
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    return o;
			}

			half4 frag (v2f i) : COLOR
			{
			    half4 texcol = tex2D (_MainTex, i.uv)* i.color;
			    half4 texopa = tex2D (_AlphaMap, i.uv);
			    
			    return half4(texcol[0],texcol[1],texcol[2], texopa[0]);
			}
			ENDCG
	   	}

	}
	FallBack "Diffuse"
}
