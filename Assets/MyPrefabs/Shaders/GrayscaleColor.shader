Shader "Unlit/GrayscaleColor"
{
	Properties{
		_ColorLevel("Color Level", Range(0, 1)) = 0
		_Color("Color", Color) = (0,0,0,0)
	}

	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			#pragma surface surf Lambert
			uniform float _ColorLevel;
			float4 _Color;

			struct Input {
				float2 uv_Texture;
			};

			void surf(Input IN, inout SurfaceOutput o) {
			//o.Albedo = lerp(_Color.rgb, dot(_Color.rgb, float3(0.3, 0.59, 0.11)), _ColorLevel);
				o.Albedo = lerp(dot(_Color.rgb, float3(0.3, 0.59, 0.11)), _Color.rgb, _ColorLevel);
			}

		ENDCG
	}

FallBack "Standard"
}
