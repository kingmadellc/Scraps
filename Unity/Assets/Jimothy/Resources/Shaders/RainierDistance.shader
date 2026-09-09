Shader "Jimothy/RainierDistance" {
Properties {_BaseColor("Snow and rock tint",Color)=(1,1,1,1)}
SubShader {Tags{"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque"}
Pass {Tags{"LightMode"="UniversalForward"} Cull Back
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
CBUFFER_START(UnityPerMaterial)
float4 _BaseColor;
CBUFFER_END
struct A{float4 p:POSITION;float3 n:NORMAL;half4 c:COLOR;float2 uv:TEXCOORD0;};
struct V{float4 p:SV_POSITION;half3 n:TEXCOORD0;half4 c:COLOR;float3 world:TEXCOORD1;float2 uv:TEXCOORD2;};
V vert(A a){V o;o.world=TransformObjectToWorld(a.p.xyz);o.p=TransformWorldToHClip(o.world);o.n=TransformObjectToWorldNormal(a.n);o.c=a.c;o.uv=a.uv;return o;}
float hash21(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
float noise21(float2 p){float2 a=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash21(a),hash21(a+float2(1,0)),f.x),lerp(hash21(a+float2(0,1)),hash21(a+1),f.x),f.y);}
half4 frag(V i):SV_Target {
 half3 n=normalize(i.n);Light moon=GetMainLight();float2 p=i.uv;float h=i.world.y+8;
 float broad=noise21(p*.095),detail=noise21(p*.6)*.65+noise21(p*1.5)*.35;
 float angle=atan2(p.x,p.y+12),radius=length(p*float2(1,.95));
 float channel=pow(.5+.5*sin(angle*14+radius*.038),4);
 // Snow tongues descend between dark volcanic cleavers; summit snow stays continuous.
 float snowline=19-channel*12+(broad-.5)*7;
 float snow=smoothstep(snowline-2,snowline+3,h);
 float crags=smoothstep(.38,.64,n.y+(broad-.5)*.25);
 snow*=lerp(.08,1,crags);snow*=lerp(.14,1,smoothstep(.18,.65,channel+broad*.38));snow=max(snow,smoothstep(43,49,h)*.97);
 float crevasse=smoothstep(.61,.78,noise21(p*float2(.75,2.8)+broad*3))*smoothstep(.45,.72,noise21(p*.37));
 half3 rock=half3(.063,.080,.110)*( .70+detail*.6);
 half3 ice=lerp(half3(.24,.36,.49),half3(.66,.75,.85),smoothstep(15,46,h));
 half3 base=lerp(rock,ice,snow)*(1-crevasse*snow*.17);
 float lighting=.25+.55*saturate(dot(n,moon.direction));
 half3 color=base*lighting;
 float haze=lerp(.08,.20,saturate(distance(_WorldSpaceCameraPos,i.world)/380));
 return half4(lerp(color,half3(.035,.061,.098),haze),1);
}
ENDHLSL
}}
}
