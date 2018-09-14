// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UnityChan/UCTS_Color0" {
    Properties {

        _BaseCustomDir("BaseCustomDir", Vector) = (0,0,0,0)
		_CustomDirWeight("CustomDirWeight", Vector) = (0,0,0,0)

        [Enum(OFF,0,FRONT,1,BACK,2)] _CullMode("Cull Mode", int) = 2  //OFF/FRONT/BACK
        _Color ("Color", Color) = (1,1,1,1)
        [MaterialToggle] _Is_LightColor_Base ("Is_LightColor_Base", Float ) = 1
        _1st_ShadeColor ("1st_ShadeColor", Color) = (1,1,1,1)
        [MaterialToggle] _Is_LightColor_1st_Shade ("Is_LightColor_1st_Shade", Float ) = 1
        _2nd_ShadeColor ("2nd_ShadeColor", Color) = (1,1,1,1)
        [MaterialToggle] _Is_LightColor_2nd_Shade ("Is_LightColor_2nd_Shade", Float ) = 1
        _NormalMap ("NormalMap", 2D) = "bump" {}
        [MaterialToggle] _Is_NormalMapToBase ("Is_NormalMapToBase", Float ) = 0
        [MaterialToggle] _Set_SystemShadowsToBase ("Set_SystemShadowsToBase", Float ) = 1
        _Tweak_SystemShadowsLevel ("Tweak_SystemShadowsLevel", Range(-0.5, 0.5)) = 0
        _BaseColor_Step ("BaseColor_Step", Range(0, 1)) = 0.6
        feather1 ("Base/Shade_Feather", Range(0.0001, 1)) = 0.0001
        _ShadeColor_Step ("ShadeColor_Step", Range(0, 1)) = 0.4
        feather2 ("1st/2nd_Shades_Feather", Range(0.0001, 1)) = 0.0001
        _HighColor ("HighColor", Color) = (1,1,1,1)
        [MaterialToggle] _Is_LightColor_HighColor ("Is_LightColor_HighColor", Float ) = 1
        [MaterialToggle] _Is_NormalMapToHighColor ("Is_NormalMapToHighColor", Float ) = 0
        _HighColor_Power ("HighColor_Power", Range(0, 1)) = 0
        [MaterialToggle] _Is_SpecularToHighColor ("Is_SpecularToHighColor", Float ) = 0
        [MaterialToggle] _Is_BlendAddToHiColor ("Is_BlendAddToHiColor", Float ) = 0
        [MaterialToggle] _Is_UseTweakHighColorOnShadow ("Is_UseTweakHighColorOnShadow", Float ) = 0
        _TweakHighColorOnShadow ("TweakHighColorOnShadow", Range(0, 1)) = 0
//高光和边缘光
        _highColorMask ("highColorMask", 2D) = "white" {}
        _Tweak_HighColorMaskLevel ("Tweak_HighColorMaskLevel", Range(-1, 1)) = 0
        [MaterialToggle] _RimLight ("RimLight", Float ) = 0
        _RimLightColor ("RimLightColor", Color) = (1,1,1,1)
        [MaterialToggle] _Is_LightColor_RimLight ("Is_LightColor_RimLight", Float ) = 1
        [MaterialToggle] _Is_NormalMapToRimLight ("Is_NormalMapToRimLight", Float ) = 0
        _RimLight_Power ("RimLight_Power", Range(0, 1)) = 0.1
        _RimLight_InsideMask ("RimLight_InsideMask", Range(0.0001, 1)) = 0.0001
        [MaterialToggle] _RimLight_FeatherOff ("RimLight_FeatherOff", Float ) = 0
//反向边缘光
        [MaterialToggle] _LightDirection_MaskOn ("LightDirection_MaskOn", Float ) = 0
        _Tweak_LightDirection_MaskLevel ("Tweak_LightDirection_MaskLevel", Range(0, 0.5)) = 0
        [MaterialToggle] _Add_Antipodean_RimLight ("Add_Antipodean_RimLight", Float ) = 0
        _Ap_RimLightColor ("Ap_RimLightColor", Color) = (1,1,1,1)
        [MaterialToggle] _Is_LightColor_Ap_RimLight ("Is_LightColor_Ap_RimLight", Float ) = 1
        _Ap_RimLight_Power ("Ap_RimLight_Power", Range(0, 1)) = 0.1
        [MaterialToggle] _Ap_RimLight_FeatherOff ("Ap_RimLight_FeatherOff", Float ) = 0
//反向边缘光
        _RimLightMask ("RimLightMask", 2D) = "white" {}
        _Tweak_RimLightMaskLevel ("Tweak_RimLightMaskLevel", Range(-1, 1)) = 0

        _Outline_Width ("Outline_Width", Float ) = 1
        _Farthest_Distance ("Farthest_Distance", Float ) = 10
        _Nearest_Distance ("Nearest_Distance", Float ) = 0.5
        _Outline_Sampler ("Outline_Sampler", 2D) = "white" {}
        _Outline_Color ("Outline_Color", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _Is_BlendBaseColor ("Is_BlendBaseColor", Float ) = 0
        //Offset parameter
        _Offset_Z ("Offset_Camera_Z", Float) = 0
        _GI_Intensity ("GI_Intensity", Range(0, 1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            //#pragma fragmentoption ARB_precision_hint_fastest
            //#pragma multi_compile_shadowcaster
            //#pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 switch
            #pragma target 3.0
            //�A�E�g���C�������͈ȉ���cginc��.
            #include "UCTS_Outline0.cginc"
            ENDCG
        }
//ToonCoreStart
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull[_CullMode]            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 switch
            #pragma target 3.0
            uniform float _ShadeColor_Step;
            uniform float4 _1st_ShadeColor;
            uniform float4 _2nd_ShadeColor;
            uniform float _BaseColor_Step;
            uniform float4 _Color;
            uniform fixed _Set_SystemShadowsToBase;
            uniform float4 _HighColor;
            uniform float _HighColor_Power;
            uniform fixed _Is_BlendAddToHiColor;
            uniform fixed _Is_UseTweakHighColorOnShadow;
            uniform fixed _Is_SpecularToHighColor;
            uniform float feather2;
            uniform float feather1;
            uniform fixed _RimLight;
            uniform float _RimLight_Power;
            uniform float4 _RimLightColor;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform fixed _Is_NormalMapToBase;
            uniform fixed _Is_NormalMapToHighColor;
            uniform fixed _Is_NormalMapToRimLight;

            uniform float _TweakHighColorOnShadow;
            fixed3 DecodeLightProbe( fixed3 N ){
            return ShadeSH9(float4(N,1));
            }
            
            uniform float _GI_Intensity;
            uniform fixed _Is_LightColor_Base;
            uniform fixed _Is_LightColor_1st_Shade;
            uniform fixed _Is_LightColor_2nd_Shade;
            uniform fixed _Is_LightColor_HighColor;
            uniform fixed _Is_LightColor_RimLight;
            uniform float _Tweak_SystemShadowsLevel;
            uniform float _RimLight_InsideMask;
            uniform fixed _RimLight_FeatherOff;
            uniform fixed _LightDirection_MaskOn;
            uniform fixed _Add_Antipodean_RimLight;
            uniform float _Ap_RimLight_Power;
            uniform fixed _Ap_RimLight_FeatherOff;
            uniform float4 _Ap_RimLightColor;
            uniform fixed _Is_LightColor_Ap_RimLight;
            uniform float _Tweak_LightDirection_MaskLevel;
            uniform sampler2D _highColorMask; uniform float4 _highColorMask_ST;
            uniform sampler2D _RimLightMask; uniform float4 _RimLightMask_ST;
            uniform float _Tweak_HighColorMaskLevel;
            uniform float _Tweak_RimLightMaskLevel;
			uniform float _Offset_Z;

            uniform float4 _BaseCustomDir;
			uniform float4 _CustomDirWeight;

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
				o.normalDir = UnityObjectToWorldNormal(v.normal);				
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz ); // 得到切线的在世界空间的方向
                // 通过叉乘得出一条 垂直于 法线和切线构成的平面 的副法线
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w); // 副法线方向
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); // 顶点的世界坐标
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
				// wjk
				float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - o.pos.xyz);
				float4 viewDirectionVP = mul(UNITY_MATRIX_VP, float4(viewDirection.xyz, 1));
				_Offset_Z = _Offset_Z * -0.1;
				o.pos.z = o.pos.z + _Offset_Z*viewDirectionVP.z;
				//
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir); //用切线 副法线 法线 构建矩阵
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);

                float3 normalMap = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normalLocal = normalMap.rgb;
                float3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
                
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				lightDirection.x = lerp(lightDirection.x, _BaseCustomDir.x, _CustomDirWeight.x);
				lightDirection.y = lerp(lightDirection.y, _BaseCustomDir.y, _CustomDirWeight.y);
				lightDirection.z = lerp(lightDirection.z, _BaseCustomDir.z, _CustomDirWeight.z);
				lightDirection = normalize(lightDirection);                


                float3 lightColor = _LightColor0.rgb;
                float3 halfVL = normalize(viewDirection+lightDirection);
////// Lighting:
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos); 

				//return float4(lightColor,1);//

                float3 colorB = _Color.rgb;
                float3 colorB0 = lerp(colorB, (colorB*lightColor), _Is_LightColor_Base );

                float3 color1 = _1st_ShadeColor.rgb;
                float3 colorS1 = lerp(color1, (color1*lightColor), _Is_LightColor_1st_Shade);

                float3 color2 = _2nd_ShadeColor.rgb;
                float3 colorS2 = lerp(color2, (color2*lightColor), _Is_LightColor_2nd_Shade);


                float soild2 = _ShadeColor_Step - feather2;
                float soild1 = _BaseColor_Step - feather1;

                float proj = 0.5 + 0.5 * dot(i.normalDir, lightDirection);
                float systemProj = proj * saturate(((atten * 0.5) + 0.5 + _Tweak_SystemShadowsLevel));
                proj = lerp(proj, systemProj, _Set_SystemShadowsToBase);


                float t2 = saturate(1 - (proj - soild2) / feather2);
                float3 colorS1n2 = lerp(colorS1, colorS2, t2);

                float t1 = saturate(1 - (proj - soild1) / feather1);
                float3 colorBnS1 = lerp(colorB0, colorS1n2, t1); // Final Color                

                float4 _HighColorMask_var = tex2D(_highColorMask,TRANSFORM_TEX(i.uv0, _highColorMask)); // HighColorMask
                float spec = 0.5*dot(halfVL, lerp( i.normalDir, normalDirection, _Is_NormalMapToHighColor ))+0.5; //  Specular
                float highMask = saturate((_HighColorMask_var.g + _Tweak_HighColorMaskLevel))*lerp( (1.0 - step(spec,(1.0 - _HighColor_Power))), pow(spec,exp2(lerp(11,1,_HighColor_Power))), _Is_SpecularToHighColor );
                float3 highLight = lerp( _HighColor.rgb, (_HighColor.rgb*lightColor), _Is_LightColor_HighColor ) * highMask;

                float3 highColor = (lerp( saturate((colorBnS1-highMask)), colorBnS1, _Is_BlendAddToHiColor )+lerp( highLight, (highLight*((1.0 - t1)+(t1*_TweakHighColorOnShadow))), _Is_UseTweakHighColorOnShadow ));
                
                float4 rimLightMask = tex2D(_RimLightMask, TRANSFORM_TEX(i.uv0, _RimLightMask)); // RimLightMask
                float3 _Is_LightColor_RimLight_var = lerp( _RimLightColor.rgb, (_RimLightColor.rgb*lightColor), _Is_LightColor_RimLight );
                float rimProj = (1.0 - dot(lerp( i.normalDir, normalDirection, _Is_NormalMapToRimLight ),viewDirection));
                float rimPower = pow(rimProj, exp2(lerp(3,0,_RimLight_Power)));

                float node_8305 = saturate(lerp( (0 + ( (rimPower - _RimLight_InsideMask) * (1 - 0) ) / (1 - _RimLight_InsideMask)), step(_RimLight_InsideMask,rimPower), _RimLight_FeatherOff ));
                float halfDot = 0.5*dot(i.normalDir,lightDirection)+0.5;
                float3 _LightDirection_MaskOn_var = lerp( (_Is_LightColor_RimLight_var*node_8305), (_Is_LightColor_RimLight_var*saturate((node_8305-((1.0 - halfDot)+_Tweak_LightDirection_MaskLevel)))), _LightDirection_MaskOn );
                float node_8113 = pow(rimProj,exp2(lerp(3,0,_Ap_RimLight_Power)));
                float3 rimLight = (saturate((rimLightMask.g+_Tweak_RimLightMaskLevel))*lerp( _LightDirection_MaskOn_var, (_LightDirection_MaskOn_var+(lerp( _Ap_RimLightColor.rgb, (_Ap_RimLightColor.rgb*lightColor), _Is_LightColor_Ap_RimLight )*saturate((lerp( (0 + ( (node_8113 - _RimLight_InsideMask) * (1 - 0) ) / (1 - _RimLight_InsideMask)), step(_RimLight_InsideMask,node_8113), _Ap_RimLight_FeatherOff )-(saturate(halfDot)+_Tweak_LightDirection_MaskLevel))))), _Add_Antipodean_RimLight ));
                float3 rim = lerp( highColor, (highColor+rimLight), _RimLight );           


                //float3 finalColor = saturate((1.0-(1.0-saturate( rim))*(1.0-(DecodeLightProbe( normalDirection )*_GI_Intensity))));

                fixed4 finalRGBA = fixed4(rim,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				//finalRGBA = float4(max(finalRGBA.rgb, colorS2),1);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 switch
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
//ToonCoreEnd
    }
    FallBack "Legacy Shaders/VertexLit"
}
