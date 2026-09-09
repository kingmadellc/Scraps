Shader "Jimothy/FindSurface" {
 SubShader { Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" } Pass {Name "Lit find" Tags{"LightMode"="UniversalForward"}
 Cull Off
 HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
 #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
 #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
 #pragma multi_compile_fog
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 struct A {float4 positionOS:POSITION;float3 normalOS:NORMAL;float4 color:COLOR;};
 struct V {float4 positionCS:SV_POSITION;float3 normalWS:TEXCOORD0;float3 positionWS:TEXCOORD1;half fog:TEXCOORD2;float4 color:COLOR;half3 vertexLight:TEXCOORD3;};
 V vert(A a){V o;VertexPositionInputs p=GetVertexPositionInputs(a.positionOS.xyz);o.positionCS=p.positionCS;o.positionWS=p.positionWS;o.normalWS=TransformObjectToWorldNormal(a.normalOS);o.color=a.color;o.fog=ComputeFogFactor(p.positionCS.z);o.vertexLight=VertexLighting(p.positionWS,o.normalWS);return o;}
 half4 frag(V i,FRONT_FACE_TYPE front:FRONT_FACE_SEMANTIC):SV_Target {
  InputData input=(InputData)0;input.positionWS=i.positionWS;input.normalWS=normalize(i.normalWS)*IS_FRONT_VFACE(front,1,-1);input.viewDirectionWS=GetWorldSpaceNormalizeViewDir(i.positionWS);input.shadowCoord=TransformWorldToShadowCoord(i.positionWS);input.fogCoord=i.fog;input.vertexLighting=i.vertexLight;input.bakedGI=SampleSH(input.normalWS);input.normalizedScreenSpaceUV=GetNormalizedScreenSpaceUV(i.positionCS);input.shadowMask=half4(1,1,1,1);
  SurfaceData surface=(SurfaceData)0;surface.albedo=i.color.rgb;surface.smoothness=.24;surface.occlusion=1;surface.alpha=1;surface.normalTS=half3(0,0,1);half4 c=UniversalFragmentPBR(input,surface);c.rgb=MixFog(c.rgb,i.fog);return c;
 }
 ENDHLSL
 } }
}
