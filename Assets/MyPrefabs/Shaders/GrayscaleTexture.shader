// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GrayscaleTexture"
{
	Properties{
		_Texture("Base (RGB)", 2D) = "white" {}
		_ColorLevel("Color Level", Range(0, 1)) = 0
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		//Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};

				sampler2D _Texture;
				float4 _Texture_ST;
				uniform float _ColorLevel;

				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _Texture);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_Texture, i.texcoord);
					c.rgb = lerp(c.rgb, dot(c.rgb, float3(0.3, 0.59, 0.11)), _ColorLevel);
					return c;
				}
			ENDCG
		}
	}
Fallback "Standard"
}
