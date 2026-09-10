Shader "Jimothy/FindPortrait" {
 SubShader { Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" } Pass {
 Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 struct A {float4 positionOS:POSITION;float3 normalOS:NORMAL;half4 color:COLOR;};
 struct V {float4 positionCS:SV_POSITION;float3 normalWS:TEXCOORD0;float3 positionWS:TEXCOORD1;half4 color:COLOR;};
 V vert(A a){V o;o.positionCS=TransformObjectToHClip(a.positionOS.xyz);o.positionWS=TransformObjectToWorld(a.positionOS.xyz);o.normalWS=TransformObjectToWorldNormal(a.normalOS);o.color=a.color;return o;}
 half4 frag(V i,FRONT_FACE_TYPE front:FRONT_FACE_SEMANTIC):SV_Target {
 half3 n=normalize(i.normalWS)*IS_FRONT_VFACE(front,1,-1);half3 v=GetWorldSpaceNormalizeViewDir(i.positionWS);
 half key=saturate(dot(n,normalize(float3(-.5,.85,-.6))));half rim=pow(1-saturate(dot(n,v)),3);
 half3 light=half3(.40,.46,.43)+key*half3(.83,.70,.51)+rim*half3(.20,.24,.22);
 return half4(i.color.rgb*light,1);
 }
 ENDHLSL
 } }
}
