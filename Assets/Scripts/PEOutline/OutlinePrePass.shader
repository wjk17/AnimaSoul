// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//描边Shader  
//by：puppet_master  
//2017.6.7  
  
Shader "ApcShader/OutlinePrePass"  
{  
    Properties{  
        _DepthTex("DepthTex", 2D) = "white" {}  
		_MainTex("MainTex", 2D) = "white" {}  
    }

	CGINCLUDE
    #include "UnityCG.cginc"

	struct v2f  
    {  
        float4 pos : SV_POSITION;  
		float2 uv : TEXCOORD0;
    };  
	struct v2fd
    {  
        float4 pos : SV_POSITION;  
		float2 uv : TEXCOORD0;
		float depth : SV_Depth;
    };  
              
    v2f vert(appdata_full v)  
    {  
        v2f o;  
        o.pos = UnityObjectToClipPos(v.vertex);  
		o.uv = v.texcoord.xy;
        return o;  
    }  
	sampler2D _DepthTex;
	half4 _DepthTex_ST;
	sampler2D _MainTex;
	half4 _MainTex_ST;
	ENDCG
    //子着色器    
    SubShader
    {  
		Pass //0 这个Pass直接输出描边颜色
		{
            ZTest On
            Cull Off
            ZWrite On  
            Fog{ Mode Off }  
		    CGPROGRAM 
            fixed4 frag(v2f i) : SV_Target  
            {  
				return float4(0, 1, 0, 0);
            }  
            #pragma vertex vert  
            #pragma fragment frag  
            ENDCG  
		}
        Pass //1 这个Pass输出颜色和深度到g和b通道
        {     
		    ZTest On
            Cull Off  
            ZWrite On  
            Fog{ Mode Off }  
            CGPROGRAM  
            fixed4 frag(v2f i) : SV_Target  
            {                  
				//float b = tex2D(_DepthTex, UnityStereoScreenSpaceUVAdjust(i.uv, _DepthTex_ST)).r;
				float b = 0;
                float g = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST)).g;
				return float4(0, g, b, 0);
            }
            #pragma vertex vert  
            #pragma fragment frag  
            ENDCG  
        }  
        Pass //2 这个Pass直接输出背景颜色
		{
            ZTest On
            Cull Off
            ZWrite On  
            Fog{ Mode Off }  
		    CGPROGRAM 
            fixed4 frag(v2f i) : SV_Target  
            {  
                float l = 0.0390625;
				return float4(l, l, l, 0);
            }  
            #pragma vertex vert  
            #pragma fragment frag  
            ENDCG  
		}
		// Pass //2 这个Pass直接输出深度
		// {
        //     ZTest On
        //     Cull Off  
        //     ZWrite On  
        //     Fog{ Mode Off }  
		//     CGPROGRAM 
        //     fixed4 frag(v2fd i) : SV_Target  
        //     {  
		// 		return float4(0, 0, i.depth, 1);
        //     }  
        //     #pragma vertex vert  
        //     #pragma fragment frag  
        //     ENDCG  
		// }
        // // 3, handle scene depth
        // Pass
        // {
        //     CGPROGRAM  
        //     fixed4 frag(v2fd i) : SV_TARGET
        //     {
        //         fixed4 black = float4(0,0,0,0);
        //         fixed4 white = float4(1,1,1,0);
        //         float t = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST)).r;
        //         fixed4 final = lerp(black, white, t);
        //         return final;
        //     }
        //     #pragma vertex vert
        //     #pragma fragment frag  
        //     ENDCG  
        // }
    }  
}  