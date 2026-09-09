Shader "Jimothy/WetAsphalt" {
Properties {_BaseMap("Asphalt",2D)="gray"{} _BaseColor("Road tint",Color)=(.64,.70,.73,1)}
SubShader {Tags{"RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent-20"}
Pass {Name "Damp aggregate" Tags{"LightMode"="UniversalForward"} Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off
HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);
CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;half4 _BaseColor;
CBUFFER_END
struct A{float4 p:POSITION;float2 uv:TEXCOORD0;};struct V{float4 p:SV_POSITION;float3 world:TEXCOORD0;float2 uv:TEXCOORD1;half fog:TEXCOORD2;};
V vert(A a){V o;VertexPositionInputs p=GetVertexPositionInputs(a.p.xyz);o.p=p.positionCS;o.world=p.positionWS;o.uv=a.uv;o.fog=ComputeFogFactor(p.positionCS.z);return o;}
half4 frag(V i):SV_Target {
 float radius=length(i.uv);float n=sin(i.world.x*8.1+i.world.z*5.2)*sin(i.world.x*4.3-i.world.z*9.7);float edge=saturate((1-radius)*4.2+n*.08);float opacity=smoothstep(0,1,edge)*.72;
 half3 aggregate=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.world.xz*.333333).rgb*_BaseColor.rgb;
 Light moon=GetMainLight();half3 illumination=max(SampleSH(half3(0,1,0)),half3(.065,.075,.095))+moon.color*saturate(moon.direction.y)*.45;
 half3 view=normalize(GetWorldSpaceViewDir(i.world));half3 halfway=normalize(view+moon.direction);half glint=pow(saturate(halfway.y),100)*.012;
 half3 damp=aggregate*illumination*.62+glint*half3(.55,.70,.85);
 return half4(MixFog(damp,i.fog),opacity);
}
ENDHLSL }
} }
