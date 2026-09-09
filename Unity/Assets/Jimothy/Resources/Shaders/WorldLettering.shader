Shader "Jimothy/WorldLettering" {
Properties {_MainTex("Font",2D)="white"{} }
SubShader {Tags {"Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline"} Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Back
Pass {HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct A{float4 p:POSITION;float2 uv:TEXCOORD0;half4 c:COLOR;};struct V{float4 p:SV_POSITION;float2 uv:TEXCOORD0;half4 c:COLOR;};
TEXTURE2D(_MainTex);SAMPLER(sampler_MainTex);
V vert(A a){V o;o.p=TransformObjectToHClip(a.p.xyz);o.uv=a.uv;o.c=a.c;return o;}
half4 frag(V i):SV_Target {return half4(i.c.rgb*.62,i.c.a*SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv).a);}
ENDHLSL}
}}
