Shader "Jimothy/FoliageVisibility" {
Properties {_BaseColor("Leaf color",Color)=(.07,.18,.105,1) _BaseMap("Base",2D)="white"{} _Cutoff("Cutoff",Range(0,1))=.5 _Cull("Cull",Float)=2}
SubShader {Tags{"RenderPipeline"="UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest"}
Pass {Name "ForwardLit" Tags{"LightMode"="UniversalForward"} Cull Off
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile_fragment _ _SHADOWS_SOFT
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
CBUFFER_START(UnityPerMaterial)
half4 _BaseColor;
CBUFFER_END
float4 _JimothySightTarget;
float4 _JimothySightCamera;
struct A{float4 p:POSITION;float3 n:NORMAL;float2 uv:TEXCOORD0;};struct V{float4 p:SV_POSITION;float3 world:TEXCOORD0;half3 n:TEXCOORD1;half fog:TEXCOORD2;float2 uv:TEXCOORD3;};
V vert(A a){V o;VertexPositionInputs p=GetVertexPositionInputs(a.p.xyz);o.uv=a.uv;o.p=p.positionCS;o.world=p.positionWS;o.n=TransformObjectToWorldNormal(a.n);o.fog=ComputeFogFactor(p.positionCS.z);return o;}
half4 frag(V i):SV_Target {
 float3 sight=_JimothySightTarget.xyz-_JimothySightCamera.xyz;float len2=dot(sight,sight);float t=dot(i.world-_JimothySightCamera.xyz,sight)/max(.001,len2);
 if(_JimothySightTarget.w>0&&t>0&&t<1.03){float d=length(i.world-(_JimothySightCamera.xyz+sight*t));float opacity=smoothstep(.35,.85,d);float noise=frac(52.9829189*frac(dot(i.p.xy,float2(.06711056,.00583715))));clip(opacity-noise-.001);}
 half3 n=normalize(i.n);Light main=GetMainLight(TransformWorldToShadowCoord(i.world));half3 illumination=max(SampleSH(n),half3(.25,.30,.24))+main.color*(saturate(dot(n,main.direction)*.65+.35)*.8+.12)*lerp(.55,1,main.shadowAttenuation);
 #if defined(_ADDITIONAL_LIGHTS)
 uint count=min(GetAdditionalLightsCount(),4u);for(uint lightIndex=0;lightIndex<count;lightIndex++){Light local=GetAdditionalLight(lightIndex,i.world);illumination+=local.color*local.distanceAttenuation*(saturate(dot(n,local.direction)*.5+.5))*.65;}
 #endif
 float midrib=1-smoothstep(.009,.034,abs(i.uv.x-.5));
 float branch=abs(frac(i.uv.y*7-abs(i.uv.x-.5)*2.8)-.5);
 float veins=(1-smoothstep(.015,.07,branch))*.065+midrib*.10;
 float mottling=.92+.08*sin(i.world.x*29+i.world.z*17)*sin(i.world.y*31+i.uv.y*15);
 half3 color=_BaseColor.rgb*illumination*(mottling+veins);
 color+=main.color*pow(saturate(dot(n,normalize(main.direction+GetWorldSpaceNormalizeViewDir(i.world)))),22)*.025;return half4(MixFog(color,i.fog),1);
}
ENDHLSL }
UsePass "Universal Render Pipeline/Lit/ShadowCaster"
UsePass "Universal Render Pipeline/Lit/DepthOnly"
} }
