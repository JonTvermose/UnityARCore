Shader "Unlit/GrayscaleColor"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Bump("Bump", 2D) = "bump" {}
		_ColorLevel("Color Level", Range(0, 1)) = 0
		_Color("Color", Color) = (0,0,0,0)
		[Toggle] _IsSnow("Enable snow", float) = 0
		_SnowLevel("Snow Level", Range(0, 1)) = 0
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			#pragma surface surf Lambert vertex:vert
			uniform float _ColorLevel;
			float4 _Color;
			float _SnowLevel;
			float _IsSnow;
			sampler2D _MainTex;
			sampler2D _Bump;

			struct Input {
				float2 uv_Texture;
				float2 uv_Bump;
				float3 worldNormal;
				INTERNAL_DATA
			};

			// Code stolen from https://forum.unity.com/threads/snow-up-vector-shader.177606/ and modified
			void vert(inout appdata_full v) {
				if (_IsSnow) {
					//Convert the normal to world coortinates
					float3 snormal = float3(0, -1, 0);
					float3 sn = mul((float3x3)unity_WorldToObject, snormal).xyz;

					if (dot(v.normal, sn) >= lerp(1, -1, (_SnowLevel * 2) / 3))
					{
						v.vertex.xyz += normalize(sn + v.normal) * 0.1 * _SnowLevel;
					}
				}
			}

			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = lerp(dot(_Color.rgb, float3(0.3, 0.59, 0.11)), _Color.rgb, _ColorLevel);

				if (_IsSnow) {
					half4 c = tex2D(_MainTex, IN.uv_Texture);
					o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
					if (dot(WorldNormalVector(IN, o.Normal), float3(0, 1, 0)) >= 0.0f)
					{
						o.Albedo += _SnowLevel;
					}
					o.Alpha = 1;
				}
			}

		ENDCG
	}

FallBack "Standard"
}
