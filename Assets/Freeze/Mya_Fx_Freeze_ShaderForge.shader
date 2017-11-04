// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:5579,x:34342,y:33070,varname:node_5579,prsc:2|spec-3813-OUT,gloss-5893-OUT,normal-215-OUT,emission-923-OUT,voffset-2054-OUT;n:type:ShaderForge.SFN_Tex2d,id:5033,x:33447,y:33198,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_BenTi,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5123108e157d9084984f8f4fa33bf6f1,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:3725,x:33447,y:33031,ptovrint:False,ptlb:Color,ptin:_Color,varname:_BenTiColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1198,x:33621,y:33151,varname:node_1198,prsc:2|A-3725-RGB,B-5033-RGB;n:type:ShaderForge.SFN_Tex2d,id:5487,x:32818,y:33437,ptovrint:False,ptlb:NoiseTex,ptin:_NoiseTex,varname:_xiaosan_texture,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e78258f9777c2ef42b32c5eb3283ccde,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:3542,x:32229,y:33576,ptovrint:False,ptlb:Blend,ptin:_Blend,varname:_xiosan,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5704669,max:1;n:type:ShaderForge.SFN_Color,id:274,x:33681,y:33336,ptovrint:False,ptlb:FrozenColor,ptin:_FrozenColor,varname:_bian_color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.06374352,c2:0.5910318,c3:0.9632353,c4:1;n:type:ShaderForge.SFN_Add,id:1664,x:33681,y:33496,varname:node_1664,prsc:2|A-7290-OUT,B-4664-OUT;n:type:ShaderForge.SFN_Fresnel,id:4664,x:33501,y:33517,varname:node_4664,prsc:2|EXP-6826-OUT;n:type:ShaderForge.SFN_Slider,id:6826,x:33165,y:33447,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:_waifaguang_bian,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4,max:10;n:type:ShaderForge.SFN_Tex2d,id:2093,x:33618,y:32827,ptovrint:False,ptlb:FrozenNorm,ptin:_FrozenNorm,varname:_Normal_bingwenli,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:91e7a0309f39bd14cbdd22ecf1e3a79c,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:215,x:34065,y:32952,varname:node_215,prsc:2|A-5321-OUT,B-2093-RGB,T-7081-OUT;n:type:ShaderForge.SFN_Vector3,id:5321,x:33618,y:32715,varname:node_5321,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:3813,x:33882,y:32665,ptovrint:False,ptlb:FrozenSpecular,ptin:_FrozenSpecular,varname:_specular,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.579953,max:2;n:type:ShaderForge.SFN_Slider,id:5893,x:33882,y:32764,ptovrint:False,ptlb:FrozenGloss,ptin:_FrozenGloss,varname:_gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:2054,x:33936,y:33851,varname:node_2054,prsc:2|A-570-OUT,B-6282-OUT,C-7149-OUT;n:type:ShaderForge.SFN_Slider,id:6282,x:33539,y:34005,ptovrint:False,ptlb:FrozenSqueeze,ptin:_FrozenSqueeze,varname:_tuqi,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:1;n:type:ShaderForge.SFN_NormalVector,id:570,x:33601,y:33830,prsc:2,pt:False;n:type:ShaderForge.SFN_Power,id:7290,x:33501,y:33393,varname:node_7290,prsc:2|VAL-5012-OUT,EXP-6826-OUT;n:type:ShaderForge.SFN_Step,id:2996,x:33670,y:33674,varname:node_2996,prsc:2|A-3369-OUT,B-9105-OUT;n:type:ShaderForge.SFN_Vector1,id:3369,x:33501,y:33635,varname:node_3369,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:9559,x:33906,y:33470,varname:node_9559,prsc:2|A-274-RGB,B-1664-OUT,C-2996-OUT;n:type:ShaderForge.SFN_Add,id:923,x:34153,y:33193,varname:node_923,prsc:2|A-1198-OUT,B-4667-OUT;n:type:ShaderForge.SFN_Multiply,id:7081,x:33901,y:33003,varname:node_7081,prsc:2|A-2503-OUT,B-2996-OUT;n:type:ShaderForge.SFN_Slider,id:2503,x:33574,y:33014,ptovrint:False,ptlb:FrozenNorPow,ptin:_FrozenNorPow,varname:node_2503,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Subtract,id:9105,x:32986,y:33560,varname:node_9105,prsc:2|A-5487-R,B-3297-OUT;n:type:ShaderForge.SFN_OneMinus,id:5012,x:33232,y:33262,varname:node_5012,prsc:2|IN-9105-OUT;n:type:ShaderForge.SFN_Clamp01,id:7149,x:33185,y:33738,varname:node_7149,prsc:2|IN-9105-OUT;n:type:ShaderForge.SFN_RemapRange,id:3297,x:32823,y:33606,varname:node_3297,prsc:2,frmn:0,frmx:1,tomn:0,tomx:1.1|IN-5994-OUT;n:type:ShaderForge.SFN_OneMinus,id:5994,x:32588,y:33590,varname:node_5994,prsc:2|IN-3542-OUT;n:type:ShaderForge.SFN_Multiply,id:4667,x:34087,y:33449,varname:node_4667,prsc:2|A-9559-OUT,B-9559-OUT,C-2307-OUT;n:type:ShaderForge.SFN_Vector1,id:2307,x:33906,y:33613,varname:node_2307,prsc:2,v1:2;proporder:3725-5033-5487-2093-2503-6282-274-3813-5893-6826-3542;pass:END;sub:END;*/

Shader "Shader Forge/dongjie_vogeo" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _FrozenNorm ("FrozenNorm", 2D) = "bump" {}
        _FrozenNorPow ("FrozenNorPow", Range(0, 2)) = 1
        _FrozenSqueeze ("FrozenSqueeze", Range(0, 1)) = 0.3
        _FrozenColor ("FrozenColor", Color) = (0.06374352,0.5910318,0.9632353,1)
        _FrozenSpecular ("FrozenSpecular", Range(0, 2)) = 1.579953
        _FrozenGloss ("FrozenGloss", Range(0, 1)) = 1
        _Fresnel ("Fresnel", Range(0, 10)) = 4
        _Blend ("Blend", Range(0, 1)) = 0.5704669
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform float4 _LightColor0;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            uniform sampler2D _NoiseTex; uniform float4 _NoiseTex_ST;
            uniform float _Blend;
            uniform float4 _FrozenColor;
            uniform float _Fresnel;
            uniform sampler2D _FrozenNorm; uniform float4 _FrozenNorm_ST;
            uniform float _FrozenSpecular;
            uniform float _FrozenGloss;
            uniform float _FrozenSqueeze;
            uniform float _FrozenNorPow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
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
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 _NoiseTex_var = tex2Dlod(_NoiseTex,float4(TRANSFORM_TEX(o.uv0, _NoiseTex),0.0,0));
                float node_9105 = (_NoiseTex_var.r-((1.0 - _Blend)*1.1+0.0));
                v.vertex.xyz += (v.normal*_FrozenSqueeze*saturate(node_9105));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _FrozenNorm_var = UnpackNormal(tex2D(_FrozenNorm,TRANSFORM_TEX(i.uv0, _FrozenNorm)));
                float4 _NoiseTex_var = tex2D(_NoiseTex,TRANSFORM_TEX(i.uv0, _NoiseTex));
                float node_9105 = (_NoiseTex_var.r-((1.0 - _Blend)*1.1+0.0));
                float node_2996 = step(0.0,node_9105);
                float3 normalLocal = lerp(float3(0,0,1),_FrozenNorm_var.rgb,(_FrozenNorPow*node_2996));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _FrozenGloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_FrozenSpecular,_FrozenSpecular,_FrozenSpecular);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_9559 = (_FrozenColor.rgb*(pow((1.0 - node_9105),_Fresnel)+pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel))*node_2996);
                float3 emissive = ((_Color.rgb*_MainTex_var.rgb)+(node_9559*node_9559*2.0));
/// Final Color:
                float3 finalColor = specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
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
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _NoiseTex; uniform float4 _NoiseTex_ST;
            uniform float _Blend;
            uniform float _FrozenSqueeze;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 _NoiseTex_var = tex2Dlod(_NoiseTex,float4(TRANSFORM_TEX(o.uv0, _NoiseTex),0.0,0));
                float node_9105 = (_NoiseTex_var.r-((1.0 - _Blend)*1.1+0.0));
                v.vertex.xyz += (v.normal*_FrozenSqueeze*saturate(node_9105));
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
