Shader "Transparent/extraAlphaTint" 
{
	Properties {
    _Color ("TintColor", Color) = (0.75,0.75,0.75,1)
    _MainTex ("Diffuse", 2D) = "white" {}
    _AlphaMap ("Opacity", 2D) = "white" {}
	}

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
			    
			    return half4(texcol[0]*_Color[0],texcol[1]*_Color[1],texcol[2]*_Color[2], texopa[0]);
			}
			ENDCG
	   	}

	}
 
	FallBack "Diffuse"
}
