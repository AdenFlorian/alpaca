// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:2,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:4,dpts:0,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33181,y:32678,varname:node_3138,prsc:2|diff-5007-OUT,alpha-751-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32460,y:32647,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Depth,id:5343,x:32216,y:32851,varname:node_5343,prsc:2;n:type:ShaderForge.SFN_ProjectionParameters,id:9869,x:32425,y:33064,varname:node_9869,prsc:2;n:type:ShaderForge.SFN_Divide,id:2332,x:32641,y:32958,varname:node_2332,prsc:2|A-9654-OUT,B-9869-FAR;n:type:ShaderForge.SFN_Slider,id:783,x:32081,y:33012,ptovrint:False,ptlb:depth-mod,ptin:_depthmod,varname:node_783,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:1000;n:type:ShaderForge.SFN_Multiply,id:9654,x:32420,y:32925,varname:node_9654,prsc:2|A-5343-OUT,B-783-OUT;n:type:ShaderForge.SFN_Clamp,id:751,x:32993,y:32978,varname:node_751,prsc:2|IN-2332-OUT,MIN-2397-OUT,MAX-2409-OUT;n:type:ShaderForge.SFN_Vector1,id:2397,x:32781,y:33002,varname:node_2397,prsc:2,v1:0;n:type:ShaderForge.SFN_Fresnel,id:1545,x:32488,y:32162,varname:node_1545,prsc:2|EXP-6772-OUT;n:type:ShaderForge.SFN_Multiply,id:5007,x:32817,y:32606,varname:node_5007,prsc:2|A-2673-OUT,B-7241-RGB;n:type:ShaderForge.SFN_Clamp,id:2673,x:32669,y:32301,varname:node_2673,prsc:2|IN-3231-OUT,MIN-5109-OUT,MAX-7699-OUT;n:type:ShaderForge.SFN_Slider,id:5109,x:32231,y:32388,ptovrint:False,ptlb:min-clamp,ptin:_minclamp,varname:node_5109,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:7699,x:32326,y:32477,ptovrint:False,ptlb:max-clamp,ptin:_maxclamp,varname:node_7699,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:6772,x:32152,y:32247,ptovrint:False,ptlb:exp,ptin:_exp,varname:node_6772,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Multiply,id:3231,x:33029,y:32310,varname:node_3231,prsc:2|A-1545-OUT,B-6946-OUT;n:type:ShaderForge.SFN_Slider,id:6946,x:32729,y:32447,ptovrint:False,ptlb:multi,ptin:_multi,varname:node_6946,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:10;n:type:ShaderForge.SFN_Slider,id:2409,x:32624,y:33148,ptovrint:False,ptlb:depth-clamp,ptin:_depthclamp,varname:node_2409,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.9,max:1;proporder:7241-783-5109-7699-6772-6946-2409;pass:END;sub:END;*/

Shader "Shader Forge/inner-atmos" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _depthmod ("depth-mod", Range(1, 1000)) = 1
        _minclamp ("min-clamp", Range(0, 10)) = 0
        _maxclamp ("max-clamp", Range(0, 10)) = 1
        _exp ("exp", Range(0, 10)) = 0
        _multi ("multi", Range(0, 10)) = 2
        _depthclamp ("depth-clamp", Range(0, 1)) = 0.9
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha DstColor
            ZTest Less
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform float _depthmod;
            uniform float _minclamp;
            uniform float _maxclamp;
            uniform float _exp;
            uniform float _multi;
            uniform float _depthclamp;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 projPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuseColor = (clamp((pow(1.0-max(0,dot(normalDirection, viewDirection)),_exp)*_multi),_minclamp,_maxclamp)*_Color.rgb);
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor,clamp(((partZ*_depthmod)/_ProjectionParams.b),0.0,_depthclamp));
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZTest Less
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform float _depthmod;
            uniform float _minclamp;
            uniform float _maxclamp;
            uniform float _exp;
            uniform float _multi;
            uniform float _depthclamp;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 projPos : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuseColor = (clamp((pow(1.0-max(0,dot(normalDirection, viewDirection)),_exp)*_multi),_minclamp,_maxclamp)*_Color.rgb);
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * clamp(((partZ*_depthmod)/_ProjectionParams.b),0.0,_depthclamp),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
