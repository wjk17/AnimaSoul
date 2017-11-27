﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PostEffect/ImageGlass0"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_NoiseTex("Noise Texture",2D) = "white"{}
	_MaskTex("Mask Texture",2D) = "white"{}
	_HeatTime("Heat Time",range(0,1.5)) = 1
		_HeatForce("Heat Force",range(0,1.0)) = 0.1
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha
		AlphaTest Greater 0.1
		Cull Off
		ZWrite Off
		Lighting Off

		GrabPass{ "_RefractionTex" }

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _NoiseTex;
	float4 _NoiseTex_ST;
	float  _HeatTime;
	float  _HeatForce;
	sampler2D _RefractionTex;
	sampler2D _MaskTex;


	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 uvgrab : TEXCOORD1;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv,_MainTex);
		o.uvgrab = ComputeGrabScreenPos(o.vertex);
		return o;
	}


	fixed4 frag(v2f i) : SV_Target
	{

		/*===============================================================
		将uv坐标偏移
		================================================================*/
		half4 offestcol1 = tex2D(_NoiseTex,i.uv + _Time.xz * _HeatTime);
		half4 offestcol2 = tex2D(_NoiseTex,i.uv - _Time.yx * _HeatTime);
		i.uvgrab.x += ((offestcol1.r + offestcol2.r) - 1) * _HeatForce;
		i.uvgrab.y += ((offestcol1.g + offestcol2.g) - 1) * _HeatForce;

		/*===========================================================*/
		half4 col = tex2Dproj(_RefractionTex,UNITY_PROJ_COORD(i.uvgrab));

		half4 maskcol = tex2D(_MaskTex, i.uv);

		col.a = maskcol.a;

		half4 tint = tex2D(_MainTex,i.uv);

		return col * tint;
	}
		ENDCG
	}
	}
}