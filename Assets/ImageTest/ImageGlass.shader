Shader "PostEffect/ImageGlass"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_NoiseTex("Noise Texture (RG)", 2D) = "white" {}
	_MaskTex("Mask Texture", 2D) = "white" {}
	_HeatTime("Heat Time", range(0,1.5)) = 1
		_HeatForce("Heat Force", range(0,0.1)) = 0.1
	}

		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert_img  
#pragma fragment frag  
#pragma fragmentoption ARB_precision_hint_fastest  
#include "UnityCG.cginc"  

		float _HeatForce;
	float _HeatTime;

	uniform sampler2D _MainTex;
	uniform sampler2D _NoiseTex; uniform float4 _NoiseTex_ST;
	uniform sampler2D _MaskTex;

	fixed4 frag(v2f_img i) : COLOR
	{
		fixed mask = tex2D(_MaskTex,i.uv).a;

	// 扭曲效果  

	half2 uv1 = i.uv + _Time.xz*_HeatTime;
	half2 uv2 = i.uv - _Time.yx*_HeatTime;

	uv1 = TRANSFORM_TEX(uv1, _NoiseTex);

	half4 offsetColor1 = tex2D(_NoiseTex, uv1);
	half4 offsetColor2 = tex2D(_NoiseTex, uv2);
	i.uv.x += ((offsetColor1.r + offsetColor2.r) - 1) * _HeatForce * mask;
	i.uv.y += ((offsetColor1.g + offsetColor2.g) - 1) * _HeatForce * mask;

	fixed4 renderTex = tex2D(_MainTex, i.uv);

	return renderTex;
	}

		ENDCG
	}
	}
		FallBack off
}