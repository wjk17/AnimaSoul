// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DepthTest" 
{
	CGINCLUDE
	#include "UnityCG.cginc"  

	//仍然要声明一下_CameraDepthTexture这个变量，虽然Unity这个变量是unity内部赋值  
	sampler2D _CameraDepthTexture;
	sampler2D _MainTex;
	float4    _MainTex_TexelSize;

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv  : TEXCOORD0;
	};

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.x = 1 - v.texcoord.x;//x轴对称
		o.uv.y = v.texcoord.y;

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
	//直接根据UV坐标取该点的深度值  
	float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, 1 - i.uv);
	//将深度值变为线性01空间  
	//靠近摄像机的白，远离的黑
	depth = 1 - Linear01Depth(depth);
	return float4(depth, depth, depth, 1);
	}

		ENDCG

		SubShader
	{
		Pass
		{

			ZTest On
			Cull Off
			ZWrite On
			Fog{ Mode Off }

			CGPROGRAM
#pragma vertex vert  
#pragma fragment frag  
			ENDCG
		}

	}
}