Shader "Jimothy/RootedFur" {
Properties {_BaseMap("Strands",2D)="white"{} _BaseColor("Tint",Color)=(1,1,1,1) _Cutoff("Cutout",Range(0,1))=.12}
SubShader {Tags{"RenderPipeline"="UniversalPipeline" "Queue"="Geometry" "RenderType"="Opaque"}
Pass {Name "ForwardLit" Tags{"LightMode"="UniversalForward"} Cull Off ZWrite On AlphaToMask Off
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
CBUFFER_START(UnityPerMaterial)
float4 _BaseColor;float _Cutoff;float4 _FurMotion;
CBUFFER_END
struct A{float4 p:POSITION;float3 n:NORMAL;float2 uv:TEXCOORD0;float4 color:COLOR;};
struct V{float4 p:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;float2 uv:TEXCOORD2;half4 color:COLOR;half fog:TEXCOORD3;};
V vert(A a){V o;float3 w=TransformObjectToWorld(a.p.xyz);float tip=a.uv.y*a.uv.y*saturate(a.color.a);w+=(_FurMotion.xyz+float3(sin(_Time.y*2.1+w.x*3+w.z*2)*.0025,0,cos(_Time.y*1.7+w.z*3)*.0015))*tip;o.p=TransformWorldToHClip(w);o.world=w;o.normal=TransformObjectToWorldNormal(a.n);o.uv=a.uv;o.color=a.color;o.fog=ComputeFogFactor(o.p.z);return o;}
half4 frag(V i, FRONT_FACE_TYPE front:FRONT_FACE_SEMANTIC):SV_Target{
 // V8 linear-color fibers taper in actual geometry; no alpha map holes or rectangular-card cutoff.
 half alpha=1;
 half3 n=normalize(i.normal);half3 v=GetWorldSpaceNormalizeViewDir(i.world);
 Light light=GetMainLight(TransformWorldToShadowCoord(i.world));half3 illum=max(SampleSH(n),half3(.025,.028,.033))+light.color*(.08+.92*saturate(dot(n,light.direction)))*light.shadowAttenuation;
 #ifdef _ADDITIONAL_LIGHTS
 uint count=GetAdditionalLightsCount();for(uint k=0;k<count;k++){Light l=GetAdditionalLight(k,i.world);illum+=l.color*l.distanceAttenuation*(.2+.8*saturate(dot(n,l.direction)));}
 #endif
 half3 col=i.color.rgb*_BaseColor.rgb*min(illum,half3(1.3,1.3,1.3));return half4(MixFog(col,i.fog),alpha);
}
ENDHLSL
}
}}
