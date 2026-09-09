Shader "Jimothy/ClosingTimeSky" {
Properties { _Tint("Tint", Color)=(.035,.07,.14,1) _MoonDirection("Moon direction",Vector)=(.28,.848,-.45,0) }
SubShader { Tags {"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"} Cull Off ZWrite Off
Pass { HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
float4 _MoonDirection;
struct A{float4 p:POSITION;}; struct V{float4 p:SV_POSITION;float3 ray:TEXCOORD0;};
V vert(A a){V o;o.p=TransformObjectToHClip(a.p.xyz);o.ray=a.p.xyz;return o;}
float hash(float3 p){return frac(sin(dot(p,float3(127.1,311.7,74.7)))*43758.5453);}
half4 frag(V i):SV_Target{
 float3 d=normalize(i.ray);float height=saturate(d.y);float3 c=lerp(float3(.022,.034,.057),float3(.003,.009,.024),pow(height,.5));c+=float3(.016,.008,.004)*pow(1-height,9);
 float3 md=normalize(_MoonDirection.xyz),right=normalize(cross(md,float3(0,1,0))),up=cross(right,md);float alignment=dot(d,md);
 float2 disc=float2(dot(d,right),dot(d,up))/.016;float r=length(disc);float aa=max(fwidth(r),.002);float moon=1-smoothstep(1-aa,1+aa,r);moon*=step(0,alignment);
 float maria=sin(disc.x*5+sin(disc.y*8))*sin(disc.y*6-disc.x*3);float mottling=.78+.20*maria;float limb=sqrt(saturate(1-r*r));
 c+=moon*lerp(float3(.48,.55,.68),float3(1.1,1.18,1.3),limb)*mottling;c+=pow(saturate(alignment),180)*float3(.018,.028,.047);
 float star=hash(floor(d*800));c+=step(.99965,star)*smoothstep(.12,.45,height)*.35*(1-moon);return half4(c,1);
}
ENDHLSL }
} Fallback Off }
