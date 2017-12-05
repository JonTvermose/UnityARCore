// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GrayscaleTexture"
{
	Properties{
		_Texture("Base (RGB)", 2D) = "white" {}
		_ColorLevel("Color Level", Range(0, 1)) = 0
		[Toggle] _IsSnow("Enable snow", float) = 0
		_SnowLevel("Snow Level", Range(0, 1)) = 0
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

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
					half3 worldNormal : NORMAL;
				};

				sampler2D _Texture;
				float4 _Texture_ST;
				uniform float _ColorLevel;
				float _IsSnow;
				float _SnowLevel;

				v2f vert(appdata_full v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _Texture);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
				
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_Texture, i.texcoord);
					c.rgb = lerp(dot(c.rgb, float3(0.3, 0.59, 0.11)), c.rgb, _ColorLevel);

					if (_IsSnow && i.worldNormal.y > 0.0) {
						c.rgb = lerp(c.rgb, dot(float3(1,1,1), float3(0.3, 0.59, 0.11)), _SnowLevel);		
					}

					return c;
				}
			ENDCG
		}
	}
Fallback "Standard"
}
