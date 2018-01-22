// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//后处理描边Shader  
//by：puppet_master  
//2017.6.7  
  
Shader "Custom/OutLinePostEffect" {  
  
    Properties{  
        _MainTex("Base (RGB)", 2D) = "white" {}  
        _BlurTex("Blur", 2D) = "white"{}  
		_SceneDepthTex("SceneDepthTex", 2D) = "white"{}  
		_OutlineCol("OutlineCol", Color) = (0,0,0,1)
    }  
  
    CGINCLUDE  
    #include "UnityCG.cginc"  
      
    //用于剔除中心留下轮廓  
    struct v2f_cull  
    {  
        float4 pos : SV_POSITION;  
        float2 uv : TEXCOORD0;  
    };  
  
    //用于Sobel描边
    struct v2f_sobel  
    {  
        float4 pos : SV_POSITION;  
        float2 uv : TEXCOORD0;
        float2 uvTR : TEXCOORD1;
        float2 uvTL : TEXCOORD2;
        float2 uvBR : TEXCOORD3;
        float2 uvBL : TEXCOORD4;
        float2 uvT : TEXCOORD5;
        float2 uvL : TEXCOORD6;
        float2 uvR : TEXCOORD7;
        float2 uvB : TEXCOORD8;
    };  

    //用于模糊  
    struct v2f_blur  
    {  
        float4 pos : SV_POSITION;  
        float2 uv  : TEXCOORD0;  
        float4 uv01 : TEXCOORD1;  
        float4 uv23 : TEXCOORD2;  
        float4 uv45 : TEXCOORD3;  
    };  
  
    //用于最后叠加  
    struct v2f_add  
    {  
        float4 pos : SV_POSITION;  
        float2 uv : TEXCOORD0;  
        float2 uv1 : TEXCOORD1;  
    };  
  
	sampler2D _SceneDepthTex;  
    float4 _SceneDepthTex_TexelSize;  
    sampler2D _MainTex;  
    float4 _MainTex_TexelSize;  
    half4 _MainTex_ST;  
    sampler2D _BlurTex;  
    float4 _BlurTex_TexelSize;  
    float4 _offsets;  
	float4 _OutlineCol;
    float _OutlineStrength;  
	float _DepthOffset;
	float _DepthExponent;
    float _SampleDistance;

    //Blur图和原图进行相减获得轮廓  
    v2f_sobel vert_sobel(appdata_img v)  
    {  
        v2f_sobel o;  
        o.pos = UnityObjectToClipPos(v.vertex);  
        o.uv = v.texcoord.xy;  
        //dx中纹理从左上角为初始坐标，需要反向  
#if UNITY_UV_STARTS_AT_TOP  
        if (_MainTex_TexelSize.y < 0)  
            o.uv.y = 1 - o.uv.y;  
#endif    
        float2 uvDist = _SampleDistance * _MainTex_TexelSize.xy;
        o.uv = UnityStereoScreenSpaceUVAdjust(o.uv, _MainTex_ST);
        o.uvTR = UnityStereoScreenSpaceUVAdjust(o.uv+uvDist, _MainTex_ST);
        o.uvTL = UnityStereoScreenSpaceUVAdjust(o.uv+uvDist*float2(-1,1), _MainTex_ST);
        o.uvBR = UnityStereoScreenSpaceUVAdjust(o.uv-uvDist*float2(-1,1), _MainTex_ST);
        o.uvBL = UnityStereoScreenSpaceUVAdjust(o.uv-uvDist, _MainTex_ST);
        o.uvT = UnityStereoScreenSpaceUVAdjust(o.uv+uvDist*float2(0,1), _MainTex_ST);
        o.uvL = UnityStereoScreenSpaceUVAdjust(o.uv-uvDist*float2(1,0), _MainTex_ST);
        o.uvR = UnityStereoScreenSpaceUVAdjust(o.uv+uvDist*float2(1,0), _MainTex_ST);
        o.uvB = UnityStereoScreenSpaceUVAdjust(o.uv-uvDist*float2(0,1), _MainTex_ST);
        return o;   
    }  
  
    fixed4 frag_sobel(v2f_sobel i) : SV_Target  
    {  
        float4 depthD;
		float4 depthA;

		depthD.x = tex2D(_MainTex, i.uvTR).b; // TR
		depthD.y = tex2D(_MainTex, i.uvTL).b; // TL
		depthD.z = tex2D(_MainTex, i.uvBR).b; // BR
		depthD.w = tex2D(_MainTex, i.uvBL).b; // BL
		depthA.x = tex2D(_MainTex, i.uvT).b; // T
		depthA.y = tex2D(_MainTex, i.uvL).b; // L
		depthA.z = tex2D(_MainTex, i.uvR).b; // R
		depthA.w = tex2D(_MainTex, i.uvB).b; // B

		float depth = max(depthD.x, depthD.y);
		depth = max(depth, depthD.z);
		depth = max(depth, depthD.w);
		depth = max(depth, depthA.x);
		depth = max(depth, depthA.y);
		depth = max(depth, depthA.z);
		depth = max(depth, depthA.w);

        float centerDepth = tex2D(_MainTex, i.uv).g;
		float4 depthsDiag;
		float4 depthsAxis;
		depthsDiag.x = tex2D(_MainTex, i.uvTR).g; // TR
		depthsDiag.y = tex2D(_MainTex, i.uvTL).g; // TL
		depthsDiag.z = tex2D(_MainTex, i.uvBR).g; // BR
		depthsDiag.w = tex2D(_MainTex, i.uvBL).g; // BL

		depthsAxis.x = tex2D(_MainTex, i.uvT).g; // T
		depthsAxis.y = tex2D(_MainTex, i.uvL).g; // L
		depthsAxis.z = tex2D(_MainTex, i.uvR).g; // R
		depthsAxis.w = tex2D(_MainTex, i.uvB).g; // B

		depthsDiag -= centerDepth;
		depthsAxis /= centerDepth;

		const float4 HorizDiagCoeff = float4(1,1,-1,-1);
		const float4 VertDiagCoeff = float4(-1,1,-1,1);
		const float4 HorizAxisCoeff = float4(1,0,0,-1);
		const float4 VertAxisCoeff = float4(0,1,-1,0);

		float4 SobelH = depthsDiag * HorizDiagCoeff + depthsAxis * HorizAxisCoeff;
		float4 SobelV = depthsDiag * VertDiagCoeff + depthsAxis * VertAxisCoeff;

		float SobelX = dot(SobelH, float4(1,1,1,1));
		float SobelY = dot(SobelV, float4(1,1,1,1));
		float Sobel = sqrt(SobelX * SobelX + SobelY * SobelY);

		float near = step(0.5, Sobel); // 像素是否邻近轮廓

        Sobel = saturate(Sobel);
        float4 color = float4(0, near, depth, 0);

		// float4 red = float4(depth + _DepthOffset, 0, 0, 0);
		// float4 black = float4(0, 0, 0, 0);

		// float outside = 1;//step(0.1, centerDepth); // 像素是否处于轮廓外				

		// float t = near * outside;
		// float4 color = lerp(black, red, t); //满足这两个条件的像素就是“外轮廓”，输出到 r 值
		return color;
    }  

    //Blur图和原图进行相减获得轮廓  
    v2f_cull vert_cull(appdata_img v)  
    {  
        v2f_cull o;  
        o.pos = UnityObjectToClipPos(v.vertex);  
        o.uv = v.texcoord.xy;  
        //dx中纹理从左上角为初始坐标，需要反向  
#if UNITY_UV_STARTS_AT_TOP  
        if (_MainTex_TexelSize.y < 0)  
            o.uv.y = 1 - o.uv.y;  
#endif    
        return o;  
    }  
  
    fixed4 frag_cull(v2f_cull i) : SV_Target  
    {  
        fixed4 colorMain = tex2D(_MainTex, i.uv);  
        fixed4 colorBlur = tex2D(_BlurTex, i.uv);  
        //最后的颜色是_BlurTex - _MainTex，周围0-0=0，黑色；边框部分为描边颜色-0=描边颜色；中间部分为描边颜色-描边颜色=0。最终输出只有边框  
        //return fixed4((colorBlur - colorMain).rgb, 1);  
        return colorBlur - colorMain;  
    }  
    //高斯模糊 vert shader（之前的文章有详细注释，此处也可以用BoxBlur，更省一点）  
    v2f_blur vert_blur(appdata_img v)  
    {  
        v2f_blur o;  
        _offsets *= _MainTex_TexelSize.xyxy;  
        o.pos = UnityObjectToClipPos(v.vertex);  
        o.uv = v.texcoord.xy;  
  
        o.uv01 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1);  
        o.uv23 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 2.0;  
        o.uv45 = v.texcoord.xyxy + _offsets.xyxy * float4(1, 1, -1, -1) * 3.0;  
  
        return o;  
    }  
      //高斯模糊 pixel shader  
    fixed4 frag_blurB(v2f_blur i) : SV_Target  
    {  
        fixed4 color = tex2D(_MainTex, i.uv);
        color.b = max(color.b, tex2D(_MainTex, i.uv).b);  
        color.b = max(color.b, tex2D(_MainTex, i.uv01.xy).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv01.zw).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv23.xy).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv23.zw).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv45.xy).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv45.zw).b);
        return color;  
    }  
    //高斯模糊 pixel shader  
    fixed4 frag_blur(v2f_blur i) : SV_Target  
    {  
        fixed4 color = fixed4(0,0,0,0);  
        color.g += 0.40 * tex2D(_MainTex, i.uv).g;
        color.g += 0.15 * tex2D(_MainTex, i.uv01.xy).g;
        color.g += 0.15 * tex2D(_MainTex, i.uv01.zw).g;
        color.g += 0.10 * tex2D(_MainTex, i.uv23.xy).g;
        color.g += 0.10 * tex2D(_MainTex, i.uv23.zw).g;
        color.g += 0.05 * tex2D(_MainTex, i.uv45.xy).g;
        color.g += 0.05 * tex2D(_MainTex, i.uv45.zw).g;
        color.b = max(color.b, tex2D(_MainTex, i.uv).b);  
        color.b = max(color.b, tex2D(_MainTex, i.uv01.xy).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv01.zw).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv23.xy).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv23.zw).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv45.xy).b);
        color.b = max(color.b, tex2D(_MainTex, i.uv45.zw).b);
        return color;  
    }  
	
	fixed4 frag_blur0(v2f_blur i) : SV_Target
	{
		fixed4 color = fixed4(0,0,0,0);
        color += 0.40 * tex2D(_MainTex, i.uv);
        color += 0.15 * tex2D(_MainTex, i.uv01.xy);
        color += 0.15 * tex2D(_MainTex, i.uv01.zw);
        color += 0.10 * tex2D(_MainTex, i.uv23.xy);
        color += 0.10 * tex2D(_MainTex, i.uv23.zw);
        color += 0.05 * tex2D(_MainTex, i.uv45.xy);
        color += 0.05 * tex2D(_MainTex, i.uv45.zw);
        return color;
	}

    //最终叠加 vertex shader  
    v2f_add vert_add(appdata_img v)  
    {  
        v2f_add o;  
        //mvp矩阵变换  
        o.pos = UnityObjectToClipPos(v.vertex);  
        //uv坐标传递  
        o.uv.xy = v.texcoord.xy;  
        o.uv1.xy = o.uv.xy;  
        #if UNITY_UV_STARTS_AT_TOP  
        if (_MainTex_TexelSize.y < 0)  
            o.uv.y = 1 - o.uv.y;  
        #endif    
        return o;        
    }  
  
    fixed4 frag_add(v2f_add i) : SV_Target  
    {  
        //取原始场景图片进行采样  
        fixed4 ori = tex2D(_MainTex, i.uv1);  
        //取得到的轮廓图片进行采样  
        fixed4 blur = tex2D(_BlurTex, i.uv);  
		fixed outline = blur.g;
		fixed outlineDepth = blur.b;
		fixed scnDepth = tex2D(_SceneDepthTex, i.uv).r;//值越大越接近镜头，0是背景
		outlineDepth = pow(outlineDepth, _DepthExponent);
		outlineDepth += _DepthOffset;
		scnDepth = pow(scnDepth, _DepthExponent);

		fixed zpass = 1 - step(outlineDepth, scnDepth);

        fixed t = step(0.5, blur.g);

		//fixed t = zpass * outline * _OutlineStrength;
        //t = t * outline;
		//fixed4 color = lerp(float4(1,1,1,1), _OutlineCol, t);        
        //fixed4 final = ori * color;
        fixed4 color = lerp(ori, _OutlineCol, t);        
        fixed4 final = color;
        return final;
    }  

    ENDCG  
  
    SubShader  
    {  
        //pass 0: 高斯模糊  
        Pass  
        {  
            ZTest Off  
            Cull Off  
            ZWrite Off  
            Fog{ Mode Off }  
  
            CGPROGRAM  
            #pragma vertex vert_blur  
            #pragma fragment frag_blur
            ENDCG  
        }            
        //pass 1: 剔除中心部分   
        Pass  
        {  
            ZTest Off  
            Cull Off  
            ZWrite Off  
            Fog{ Mode Off }  
  
            CGPROGRAM  
            #pragma vertex vert_cull  
            #pragma fragment frag_cull  
            ENDCG  
        }  
        //pass 2: 最终叠加  
        Pass  
        {  
  
            ZTest On  
            Cull Off  
            ZWrite On
            Fog{ Mode Off }  
  
            CGPROGRAM  
            #pragma vertex vert_add  
            #pragma fragment frag_add  
            ENDCG  
        }  
		//pass 3: Sobel边缘检测
		Pass
		{

			ZTest On
			Cull Off
			ZWrite On
			Fog{ Mode Off }

			CGPROGRAM
			#pragma vertex vert_sobel
			#pragma fragment frag_sobel
			ENDCG
		}
        //pass 4: 高斯模糊 B通道
        Pass  
        {  
            ZTest Off  
            Cull Off  
            ZWrite Off  
            Fog{ Mode Off }  
  
            CGPROGRAM  
            #pragma vertex vert_blur  
            #pragma fragment frag_blurB 
            ENDCG  
        }      
    }  
}  