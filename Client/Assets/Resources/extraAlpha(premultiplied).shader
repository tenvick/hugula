Shader "Unlit/extraAlpha(premultiplied)" 
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
		Blend One OneMinusSrcAlpha
		
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
			
			struct appdata {
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			v2f vert (appdata v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
//			    o.uv = v.texcoord;
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);//enable texture tiling and offset in the material(need  _MainTex_ST)
			    return o;
			}

			half4 frag (v2f i) : COLOR
			{
			    half3 texcol = tex2D (_MainTex, i.uv).rgb;
			    half opa = tex2D (_AlphaMap, i.uv).r;
			    
			    return half4(texcol, opa);
			}
			ENDCG
	   	}

	}
	FallBack "Diffuse"
}
