Shader "Unlit/GrayColorMove"
{
	Properties{
		_ColorLevel("Color Level", Range(0, 1)) = 0
		_Color("Color", Color) = (0,0,0,0)
		_Scale("Scale", Range(0, 10)) = 0
		_Lean("Lean", Range(-2, 2)) = 0
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		uniform float _ColorLevel;
	float4 _Color;
	float _Scale;
	float _Lean;

	struct Input {
		float2 uv_Texture;
	};

	void vert(inout appdata_full v) {
		// Do whatever you want with the "vertex" property of v here

		float _Speed = 30;
		//float _Amount = 5;
		//float _Distance = 0.1;

		if (_Scale > 0)
		{
			if (_Lean > 0 && _Lean < 2) {
				v.vertex.x += v.vertex.y * (_Scale / 3) * sin(_Time.y*_Speed);
			}
			else if (_Lean < 0 && _Lean > -2) {
				v.vertex.x -= v.vertex.y * (_Scale / 3) * sin(_Time.y*_Speed);
			}
			else if (_Lean > 1) {
				v.vertex.z += v.vertex.y * (_Scale / 3) * sin(_Time.y*_Speed);
			}
			else if (_Lean < -1) {
				v.vertex.z -= v.vertex.y * (_Scale / 3) * sin(_Time.y*_Speed);
			}
			
			v.vertex.y *= (1 + (_Scale / 3));
		}

		//v.vertex.x += sin(_Time.y * _Speed + v.vertex.y * _Amount) * _Distance;
		//v.vertex.x += sin(_Scale * _Speed + v.vertex.y * _Amount) * _Distance;
		//v.vertex.x += v.vertex.y * sin(_Time.w);
		//v.vertex.y *= _Scale;
	}

	//void vert(inout appdata_full v) {
	//	// Do whatever you want with the "vertex" property of v here

	//	//v.vertex.xyz *= _Scale;
	//	v.vertex.y *= _Scale;
	//}

	//void vert(inout appdata_full v) {
	//	// Do whatever you want with the "vertex" property of v here

	//	//v.vertex.xyz *= _Scale;
	//	float phase = _Time * 20.0;
	//	float offset = (v.vertex.x + (v.vertex.z * 0.2)) * 0.5;
	//	v.vertex.y *= (sin(phase + offset) * 0.2) * _Scale;
	//}

	//void vert(inout appdata_full v) {
	//	// Do whatever you want with the "vertex" property of v here

	//	v.vertex.xyz += v.normal.xyz * _scale;

	//	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	//}

	//v2f vert(appdata_base v)
	//{
	//	v2f OUT;
	//	float3 norm = normalize(v.normal); //Unity 5 fix

	//	v.vertex.xyz += norm * _scale;
	//	OUT.pos = mul(UNITY_MATRIX_MVP, v.vertex);

	//	OUT.normals = v.normal;
	//	return OUT;
	//}

	void surf(Input IN, inout SurfaceOutput o) {
		//o.Albedo = lerp(_Color.rgb, dot(_Color.rgb, float3(0.3, 0.59, 0.11)), _ColorLevel);
		o.Albedo = lerp(dot(_Color.rgb, float3(0.3, 0.59, 0.11)), _Color.rgb, _ColorLevel);
	}

	ENDCG
	}

		FallBack "Standard"
}