// shader，放在需要显示阴影的对象上
Shader "Custom/PlanarShadow" 
{
	Properties
	{
		// Specular vs Metallic workflow
		[HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

		[MainTexture] _BaseMap("Albedo", 2D) = "white" {}
		[MainColor] _BaseColor("Color", Color) = (1,1,1,1)

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
		_SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		_Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicGlossMap("Metallic", 2D) = "white" {}

		_SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
		_SpecGlossMap("Specular", 2D) = "white" {}

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax("Scale", Range(0.005, 0.08)) = 0.005
		_ParallaxMap("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		[HDR] _EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DetailMask("Detail Mask", 2D) = "white" {}
		_DetailAlbedoMapScale("Scale", Range(0.0, 2.0)) = 1.0
		_DetailAlbedoMap("Detail Albedo x2", 2D) = "linearGrey" {}
		_DetailNormalMapScale("Scale", Range(0.0, 2.0)) = 1.0
		[Normal] _DetailNormalMap("Normal Map", 2D) = "bump" {}

		// SRP batching compatibility for Clear Coat (Not used in Lit)
		[HideInInspector] _ClearCoatMask("_ClearCoatMask", Float) = 0.0
		[HideInInspector] _ClearCoatSmoothness("_ClearCoatSmoothness", Float) = 0.0

			// Blending state
			[HideInInspector] _Surface("__surface", Float) = 0.0
			[HideInInspector] _Blend("__blend", Float) = 0.0
			[HideInInspector] _AlphaClip("__clip", Float) = 0.0
			[HideInInspector] _SrcBlend("__src", Float) = 1.0
			[HideInInspector] _DstBlend("__dst", Float) = 0.0
			[HideInInspector] _ZWrite("__zw", Float) = 1.0
			[HideInInspector] _Cull("__cull", Float) = 2.0

			_ReceiveShadows("Receive Shadows", Float) = 1.0
			// Editmode props
			[HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

			// ObsoleteProperties
			[HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
			[HideInInspector] _Color("Base Color", Color) = (1, 1, 1, 1)
			[HideInInspector] _GlossMapScale("Smoothness", Float) = 0.0
			[HideInInspector] _Glossiness("Smoothness", Float) = 0.0
			[HideInInspector] _GlossyReflections("EnvironmentReflections", Float) = 0.0

			[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
			[HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
			[HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

		SubShader
	{
		// Universal Pipeline tag is required. If Universal render pipeline is not set in the graphics settings
		// this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
		// material work with both Universal Render Pipeline and Builtin Unity Pipeline
		Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel" = "4.5"}
		LOD 300

		// ------------------------------------------------------------------
		//  Forward pass. Shades all light in a single pass. GI + emission + Fog
		Pass
		{
		// Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
		// no LightMode tag are also rendered by Universal Render Pipeline
		Name "ForwardLit"
		Tags{"LightMode" = "UniversalForward"}

		Blend[_SrcBlend][_DstBlend]
		ZWrite[_ZWrite]
		Cull[_Cull]

		HLSLPROGRAM
		#pragma exclude_renderers gles gles3 glcore
		#pragma target 4.5

		// -------------------------------------
		// Material Keywords
		#pragma shader_feature_local _NORMALMAP
		#pragma shader_feature_local_fragment _ALPHATEST_ON
		#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
		#pragma shader_feature_local_fragment _EMISSION
		#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
		#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		#pragma shader_feature_local_fragment _OCCLUSIONMAP
		#pragma shader_feature_local _PARALLAXMAP
		#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
		#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
		#pragma shader_feature_local_fragment _SPECULAR_SETUP
		#pragma shader_feature_local _RECEIVE_SHADOWS_OFF

		// -------------------------------------
		// Universal Pipeline keywords
		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
		#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
		#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
		#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
		#pragma multi_compile_fragment _ _SHADOWS_SOFT
		#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
		#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
		#pragma multi_compile _ SHADOWS_SHADOWMASK

		// -------------------------------------
		// Unity defined keywords
		#pragma multi_compile _ DIRLIGHTMAP_COMBINED
		#pragma multi_compile _ LIGHTMAP_ON
		#pragma multi_compile_fog

		//--------------------------------------
		// GPU Instancing
		#pragma multi_compile_instancing
		#pragma multi_compile _ DOTS_INSTANCING_ON

		#pragma vertex LitPassVertex
		#pragma fragment LitPassFragment

		#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"
		ENDHLSL
	}

		//计算阴影
		Pass
		{
			Tags{"LightMode" = "SRPDefaultUnlit"}
			Stencil          //加个模板
			{
				Ref 0
				Comp equal
				Pass incrWrap
				Fail keep
				ZFail keep
			}
			ZWrite off

		//	Blend DstColor SrcColor
		Blend Srcalpha OneminusSrcAlpha
		Offset -1, -1		//使阴影在平面之上  
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		float4x4 _World2Ground;  //阴影接收平面（世界空间到模型空间的转换矩阵）
		float4x4 _Ground2World;	 //阴影接收平面（模型空间到世界空间的转换矩阵）
		float _Instensity;

		struct v2f {
			float4 pos:SV_POSITION;
			float atten : TEXCOORD0;
		};

		v2f vert(float4 vertex:POSITION)
		{
			float3 litDir;
			litDir = WorldSpaceLightDir(vertex);//世界空间主光照相对于当前物体的方向
			litDir = mul(_World2Ground,float4(litDir,0)).xyz;//光源方向转换到接受阴影的平面空间
			litDir = normalize(litDir);// 归一
			float4 vt;

			vt = mul(unity_ObjectToWorld,vertex); //将当前物体转换到世界空间
			float scale = 1.25 - (vt.y) / 20; // 根据高度控制阴影大小
			float4 v = vertex;
			v.xyz = vertex.xyz * scale;
			vt = mul(unity_ObjectToWorld, v);
			vt = mul(_World2Ground,vt); // 将物体在世界空间的矩阵转换到地面空间
			vt.xz = vt.xz - (vt.y / litDir.y)*litDir.xz;// 用三角形相似计算沿光源方向投射后的XZ
			vt.y = 0;// 使阴影保持在接受平面上
			vt = mul(_Ground2World, vt); // 阴影顶点矩阵返回到世界空间
			vt = mul(unity_WorldToObject, vt); // 返回到物体的坐标
			v2f o;
			o.pos = UnityObjectToClipPos(vt);//输出到裁剪空间
			//o.atten = distance(vertex, vt) / _Instensity;// 根据物体顶点到阴影的距离计算衰减
			return o;
		}

		float4 frag(v2f i) :COLOR
		{
			return float4(0.1, 0.1, 0.1, 0.4);//一个灰色的阴影出来了
			//return smoothstep(0,1,i.atten / 2);
		}

		ENDCG
	}
	Pass
	{
			// Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
			// no LightMode tag are also rendered by Universal Render Pipeline
			Name "GBuffer"
			Tags{"LightMode" = "UniversalGBuffer"}

			ZWrite[_ZWrite]
			ZTest LEqual
			Cull[_Cull]

			HLSLPROGRAM
			#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.5

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			//#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
			#pragma shader_feature_local_fragment _EMISSION
			#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature_local_fragment _OCCLUSIONMAP
			#pragma shader_feature_local _PARALLAXMAP
			#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#pragma shader_feature_local_fragment _SPECULAR_SETUP
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			//#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			//#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex LitGBufferPassVertex
			#pragma fragment LitGBufferPassFragment

			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitGBufferPass.hlsl"
			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags{"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask 0
			Cull[_Cull]

			HLSLPROGRAM
			#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.5

			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
			ENDHLSL
		}

			// This pass is used when drawing to a _CameraNormalsTexture texture
			Pass
			{
				Name "DepthNormals"
				Tags{"LightMode" = "DepthNormals"}

				ZWrite On
				Cull[_Cull]

				HLSLPROGRAM
				#pragma exclude_renderers gles gles3 glcore
				#pragma target 4.5

				#pragma vertex DepthNormalsVertex
				#pragma fragment DepthNormalsFragment

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
			ENDHLSL
		}

			// This pass it not used during regular rendering, only for lightmap baking.
			Pass
			{
				Name "Meta"
				Tags{"LightMode" = "Meta"}

				Cull Off

				HLSLPROGRAM
				#pragma exclude_renderers gles gles3 glcore
				#pragma target 4.5

				#pragma vertex UniversalVertexMeta
				#pragma fragment UniversalFragmentMeta

				#pragma shader_feature_local_fragment _SPECULAR_SETUP
				#pragma shader_feature_local_fragment _EMISSION
				#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
				#pragma shader_feature_local_fragment _ALPHATEST_ON
				#pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

				#pragma shader_feature_local_fragment _SPECGLOSSMAP

				#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

				ENDHLSL
			}
			Pass
			{
				Name "Universal2D"
				Tags{ "LightMode" = "Universal2D" }

				Blend[_SrcBlend][_DstBlend]
				ZWrite[_ZWrite]
				Cull[_Cull]

				HLSLPROGRAM
				#pragma exclude_renderers gles gles3 glcore
				#pragma target 4.5

				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature_local_fragment _ALPHATEST_ON
				#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

				#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
				ENDHLSL
			}
	}

		SubShader
		{
			// Universal Pipeline tag is required. If Universal render pipeline is not set in the graphics settings
			// this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
			// material work with both Universal Render Pipeline and Builtin Unity Pipeline
			Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True" "ShaderModel" = "2.0"}
			LOD 300

			// ------------------------------------------------------------------
			//  Forward pass. Shades all light in a single pass. GI + emission + Fog
			Pass
			{
			// Lightmode matches the ShaderPassName set in UniversalRenderPipeline.cs. SRPDefaultUnlit and passes with
			// no LightMode tag are also rendered by Universal Render Pipeline
			Name "ForwardLit"
			Tags{"LightMode" = "UniversalForward"}

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]
			Cull[_Cull]

			HLSLPROGRAM
			#pragma only_renderers gles gles3 glcore d3d11
			#pragma target 2.0

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON
			#pragma shader_feature_local_fragment _EMISSION
			#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature_local_fragment _OCCLUSIONMAP
			#pragma shader_feature_local _PARALLAXMAP
			#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#pragma shader_feature_local_fragment _SPECULAR_SETUP
			#pragma shader_feature_local _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment

			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"
			ENDHLSL
		}


		Pass
		{
			Name "DepthOnly"
			Tags{"LightMode" = "DepthOnly"}

			ZWrite On
			ColorMask 0
			Cull[_Cull]

			HLSLPROGRAM
			#pragma only_renderers gles gles3 glcore d3d11
			#pragma target 2.0

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
			ENDHLSL
		}

			// This pass is used when drawing to a _CameraNormalsTexture texture
			Pass
			{
				Name "DepthNormals"
				Tags{"LightMode" = "DepthNormals"}

				ZWrite On
				Cull[_Cull]

				HLSLPROGRAM
				#pragma only_renderers gles gles3 glcore d3d11
				#pragma target 2.0

				#pragma vertex DepthNormalsVertex
				#pragma fragment DepthNormalsFragment

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
			ENDHLSL
		}

			// This pass it not used during regular rendering, only for lightmap baking.
			Pass
			{
				Name "Meta"
				Tags{"LightMode" = "Meta"}

				Cull Off

				HLSLPROGRAM
				#pragma only_renderers gles gles3 glcore d3d11
				#pragma target 2.0

				#pragma vertex UniversalVertexMeta
				#pragma fragment UniversalFragmentMeta

				#pragma shader_feature_local_fragment _SPECULAR_SETUP
				#pragma shader_feature_local_fragment _EMISSION
				#pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
				#pragma shader_feature_local_fragment _ALPHATEST_ON
				#pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
				#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

				#pragma shader_feature_local_fragment _SPECGLOSSMAP

				#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Shaders/LitMetaPass.hlsl"

				ENDHLSL
			}
			Pass
			{
				Name "Universal2D"
				Tags{ "LightMode" = "Universal2D" }

				Blend[_SrcBlend][_DstBlend]
				ZWrite[_ZWrite]
				Cull[_Cull]

				HLSLPROGRAM
				#pragma only_renderers gles gles3 glcore d3d11
				#pragma target 2.0

				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature_local_fragment _ALPHATEST_ON
				#pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

				#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
				ENDHLSL
			}
		}

			FallBack "Hidden/Universal Render Pipeline/FallbackError"
			CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.LitShader"
}



