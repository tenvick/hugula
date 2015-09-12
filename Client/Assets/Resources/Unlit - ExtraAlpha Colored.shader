Shader "Unlit/ExtraAlpha Colored"
{
	Properties
	{
		_MainTex ("Diffuse", 2D) = "white" {}
		_AlphaMap ("Opacity", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
	
			struct v2f
			{
				float4  pos : SV_POSITION;
			    half4 color : COLOR;
			    float2  uv : TEXCOORD0;
				fixed gray : TEXCOORD1;
			};
	
			sampler2D _MainTex;
			sampler2D _AlphaMap;
			float4 _MainTex_ST;
				
			v2f vert (appdata_t v)
			{
				v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.color = v.color;
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.gray = dot(v.color, fixed4(1,1,1,0));
			    return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col;  
				half4 texcol = tex2D (_MainTex, i.uv);
			    half4 texopa = tex2D (_AlphaMap, i.uv);
				if (i.gray == 0)  
				{  
					texcol.a = texopa[0];
					col = texcol* i.color;
					col.rgb = dot(texcol.rgb, fixed3(.222,.707,.071));
				}
				else
				{
					texcol.a = texopa[0];
					col = texcol* i.color;
				}
				return col;
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}