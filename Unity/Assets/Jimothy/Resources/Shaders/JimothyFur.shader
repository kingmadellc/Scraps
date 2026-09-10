Shader "Jimothy/RootedFur" {
Properties {_BaseColor("Tint",Color)=(1,1,1,1) _FiberSheen("Fiber sheen",Range(0,1))=.22}
SubShader {Tags{"RenderPipeline"="UniversalPipeline" "Queue"="Geometry" "RenderType"="Opaque"}
Pass {Name "ForwardLit" Tags{"LightMode"="UniversalForward"} Cull Off ZWrite On
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
CBUFFER_START(UnityPerMaterial)
float4 _BaseColor;float _FiberSheen;float4 _FurMotion;
CBUFFER_END
struct A{float4 p:POSITION;float3 n:NORMAL;float4 tangent:TANGENT;float2 uv:TEXCOORD0;float4 color:COLOR;};
struct V{float4 p:SV_POSITION;float3 world:TEXCOORD0;half3 normal:TEXCOORD1;half3 strand:TEXCOORD2;half4 color:COLOR;half fog:TEXCOORD3;half tip:TEXCOORD4;};
V vert(A a){
 V o;float3 w=TransformObjectToWorld(a.p.xyz);float tip=a.uv.y*a.uv.y*saturate(a.color.a);
 // Bound root-to-tip movement. Individual lengths control flexibility, roots remain skinned.
 w+=(_FurMotion.xyz+float3(sin(_Time.y*2.1+w.x*3+w.z*2)*.002,0,cos(_Time.y*1.7+w.z*3)*.001))*tip;
 o.p=TransformWorldToHClip(w);o.world=w;o.normal=TransformObjectToWorldNormal(a.n);
 half3 across=TransformObjectToWorldDir(a.tangent.xyz);
 half3 fiber=cross(o.normal,across);o.strand=dot(fiber,fiber)>.001?normalize(fiber):normalize(cross(o.normal,half3(.01,.99,.01)));
 o.color=a.color;o.tip=a.uv.y;o.fog=ComputeFogFactor(o.p.z);return o;
}
half3 FiberLight(Light light,half3 n,half3 strand,half3 view,half tip,half3 albedo){
 half wrap=saturate((dot(n,light.direction)+.25)/1.25);
 half3 h=SafeNormalize(light.direction+view);
 half th=dot(strand,h);half sinTH=sqrt(saturate(1-th*th));
 // Broad longitudinal lobe: low-energy strand sheen, not a plastic body highlight.
 half sheen=pow(sinTH,12)*saturate(dot(n,light.direction))*(.25+.75*tip)*_FiberSheen;
 half transmission=pow(saturate(dot(-light.direction,view)),4)*(.015+.035*tip);
 return light.color*light.distanceAttenuation*light.shadowAttenuation*(albedo*(wrap+transmission)+half3(.68,.72,.76)*sheen);
}
half4 frag(V i):SV_Target{
 half3 n=normalize(i.normal);half3 v=GetWorldSpaceNormalizeViewDir(i.world);half3 strand=normalize(i.strand);
 half3 albedo=i.color.rgb*_BaseColor.rgb;
 half3 col=albedo*max(SampleSH(n),half3(.035,.038,.045))*(.78+.22*i.tip);
 col+=FiberLight(GetMainLight(TransformWorldToShadowCoord(i.world)),n,strand,v,i.tip,albedo);
 #ifdef _ADDITIONAL_LIGHTS
 uint count=GetAdditionalLightsCount();for(uint k=0;k<count;k++)col+=FiberLight(GetAdditionalLight(k,i.world),n,strand,v,i.tip,albedo);
 #endif
 return half4(MixFog(col,i.fog),1);
}
ENDHLSL
}
}}
