Shader "Example/Slices" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_CutoutValue("CutoutValue", Float) = 0
	//_ToolPosition("Tool Position", vector) = (0,0,0,0) // The location of the tool - will be set by script
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
#pragma surface surf Lambert
		float4 _ToolPosition;
		float4 _hitPoint;
		float _CutoutValue;
		struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 worldPos;
	};
	sampler2D _MainTex;
	sampler2D _BumpMap;
	void surf(Input IN, inout SurfaceOutput o) {
		//slicing mode
		clip(_CutoutValue);
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
	}
		Fallback "Diffuse"
}