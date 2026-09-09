Shader "Jimothy/MoonlitPavers" {
 Properties { _BaseMap("Fired clay",2D)="white"{} _BumpMap("Worn bevel",2D)="bump"{} _SurfaceMap("Roughness / mortar",2D)="white"{} }
 SubShader { Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry"}
 Pass {Name "Moonlit pavers" Tags {"LightMode"="UniversalForward"}
 HLSLPROGRAM
 #pragma target 3.0
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
 #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
 #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
 #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
 #pragma multi_compile_fog
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 TEXTURE2D(_BaseMap);SAMPLER(sampler_BaseMap);TEXTURE2D(_BumpMap);SAMPLER(sampler_BumpMap);TEXTURE2D(_SurfaceMap);SAMPLER(sampler_SurfaceMap);
 struct A {float4 positionOS:POSITION;float3 normalOS:NORMAL;float4 tangentOS:TANGENT;float2 uv:TEXCOORD0;};
 struct V {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;half3 normalWS:TEXCOORD1;half4 tangentWS:TEXCOORD2;float2 uv:TEXCOORD3;half fog:TEXCOORD4;half3 vertexLight:TEXCOORD5;};
 V vert(A a){V o;VertexPositionInputs p=GetVertexPositionInputs(a.positionOS.xyz);VertexNormalInputs n=GetVertexNormalInputs(a.normalOS,a.tangentOS);o.positionCS=p.positionCS;o.positionWS=p.positionWS;o.normalWS=n.normalWS;o.tangentWS=half4(n.tangentWS,a.tangentOS.w*GetOddNegativeScale());o.uv=a.uv/2.56;o.fog=ComputeFogFactor(p.positionCS.z);o.vertexLight=VertexLighting(p.positionWS,n.normalWS);return o;}
 float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
 float noise(float2 p){float2 i=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash(i),hash(i+float2(1,0)),f.x),lerp(hash(i+float2(0,1)),hash(i+1),f.x),f.y);}
 half4 frag(V i):SV_Target {
  half4 tex=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv),surface=SAMPLE_TEXTURE2D(_SurfaceMap,sampler_SurfaceMap,i.uv);
  // RGB runtime normal maps use alpha=1, avoiding platform-dependent double-encoded X normals.
  half3 normalTS=SAMPLE_TEXTURE2D(_BumpMap,sampler_BumpMap,i.uv).xyz*2-1;
  float wet=smoothstep(.44,.76,noise(i.positionWS.xz*.43)+noise(i.positionWS.xz*1.35)*.12);
  float gutter=smoothstep(3.9,5.4,abs(i.positionWS.x));wet=saturate(wet+gutter*.22);
  half3 tangent=normalize(i.tangentWS.xyz),normal=normalize(i.normalWS),bitangent=cross(normal,tangent)*i.tangentWS.w;
  InputData input=(InputData)0;input.positionWS=i.positionWS;input.normalWS=normalize(TransformTangentToWorld(normalTS,half3x3(tangent,bitangent,normal)));input.viewDirectionWS=GetWorldSpaceNormalizeViewDir(i.positionWS);input.shadowCoord=TransformWorldToShadowCoord(i.positionWS);input.fogCoord=i.fog;input.vertexLighting=i.vertexLight;input.bakedGI=SampleSH(input.normalWS);input.normalizedScreenSpaceUV=GetNormalizedScreenSpaceUV(i.positionCS);input.shadowMask=half4(1,1,1,1);
  SurfaceData data=(SurfaceData)0;data.albedo=tex.rgb*lerp(1,.70,wet);data.metallic=0;data.specular=half3(.04,.04,.04);data.smoothness=lerp(surface.a,.79,wet);data.normalTS=normalTS;data.occlusion=surface.g;data.alpha=1;
  // Filter the specular lobe as small bevels recede, avoiding shimmering wet highlights.
  half3 nx=ddx(input.normalWS),ny=ddy(input.normalWS);half variance=max(dot(nx,nx),dot(ny,ny));data.smoothness=1-sqrt(saturate((1-data.smoothness)*(1-data.smoothness)+min(.18,variance*.35)));
  half4 color=UniversalFragmentPBR(input,data);color.rgb=MixFog(color.rgb,i.fog);return color;
 }
 ENDHLSL
 }
 UsePass "Universal Render Pipeline/Lit/ShadowCaster"
 UsePass "Universal Render Pipeline/Lit/DepthOnly"
 UsePass "Universal Render Pipeline/Lit/DepthNormals"
 }
}
