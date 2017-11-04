Shader "Custom/RT_Multiply" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
	}


		CGINCLUDE

#include "UnityCG.cginc"

		struct v2f {
		float4 pos : SV_POSITION;
		float2 uv[5] : TEXCOORD0;
	};

	struct v2fd {
		float4 pos : SV_POSITION;
		float2 uv[2] : TEXCOORD0;
	};

	sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;

	sampler2D _CameraDepthNormalsTexture;
	sampler2D_float _CameraDepthTexture;


	v2fd vertD(appdata_img v)
	{
		v2fd o;
		o.pos = UnityObjectToClipPos(v.vertex);

		float2 uv = v.texcoord.xy;
		o.uv[0] = uv;

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			uv.y = 1 - uv.y;
#endif

		o.uv[1] = uv;

		return o;
	}

	float4 fragD(v2fd i) : SV_Target
	{
		float4 Col = tex2D(_MainTex, i.uv[0].xy);
		if (Col.r + Col.g + Col.b > 0.5) discard;
	return Col;// * lerp(tex2D(_MainTex, i.uv[0].xy), _TestColor, _BgFade);

			   //return Sobel * lerp(tex2D(_MainTex, i.uv[0].xy), _BgColor, _BgFade);
	}

		ENDCG

		Subshader {
		Pass{
		ZTest Always Cull Off ZWrite Off

		CGPROGRAM
#pragma target 3.0   
#pragma vertex vertD
#pragma fragment fragD
			ENDCG
		}
	}

	Fallback off

} // shader
