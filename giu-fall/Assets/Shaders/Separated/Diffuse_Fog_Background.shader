Shader "Arany/Separeted/Diffuse__Fog_Background" {
	Properties {
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Int) = 5.0 // SrcAlpha
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Int) = 10.0 // OneMinusSrcAlpha
		[Space(5)]

		[Enum(Off,0,On,1)] _ZWrite ("ZWrite", Int) = 1.0 // On
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 2 // LEqual
		[Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Int) = 2 // Off
		_ZBias ("ZBias", Float) = 0.0
			[Space(5)]
		
		//[Toggle(USE_COLADJ)] _ColAdj("Use Color Adjust", Int) = 0 
		//[Space(5)]
		//_Color ( "Color Tint", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		_ColorMulti ( "Color Multiply", float ) =1
	[Space(5)]
	_MainTex ("Színezhetőség", 2D) = "white" {}
	_MainTex2("Gradient", 2D) = "white" {}
	[Space(5)]
	_EmmiA("Emmision", float) = 0
			//[Space(5)]
				[Enum(eColorPaletteColors)] _Tog ("_ColorType", Int) = 5.0 // SrcAlpha
				[Enum(eColorPaletteColors)] _Tog2("_ColorType2", Int) = 5.0 // SrcAlpha

		//[Space(5)]
		//[Toggle(USE_Avoid)] _Avoid("Use Avoid", Int) = 0 
			
	}
	SubShader {
		Pass{
			Tags { "LightMode" = "ForwardBase" "Queue"="Geometry" "RenderType"="Opaque"}

			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			ZTest [_ZTest]
			Cull [_Cull]
			Offset [_ZBias], [_ZBias]


			CGPROGRAM
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
			//#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag

		
			#pragma multi_compile __ GLOBAL_FOG
			#pragma multi_compile __ USE_GroundFOGText	
			//#pragma shader_feature __ USE_Avoid
			#pragma multi_compile_fwdadd_fullshadows


			#include "arany.cginc"
			
			float _Tog;
			float _Tog2;
			uniform float4 _PaletteColors[6];
			uniform float4 _Color;
			uniform float _ColorMulti;

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			uniform sampler2D _MainTex2;
			//uniform float4 _MainTex2_ST;

			struct vertexInput {
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				half4 color : COLOR;
				float4 texcoord: TEXCOORD0;

			};
			struct vertexOutput {
				float4 pos: SV_POSITION;
				half4 color : color;
				float2 tex: TEXCOORD0;
				float4 posWorld: TEXCOORD1;
				float3 normal: TEXCOORD5;
				LIGHTING_COORDS(7,8)
			};

			vertexOutput vert( vertexInput v ) {
				vertexOutput o;

				UNITY_INITIALIZE_OUTPUT(vertexOutput, o);
				 o.color = v.color;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.posWorld = mul( unity_ObjectToWorld, v.vertex );
			
				o.tex = v.texcoord.xy   * _MainTex_ST.xy + _MainTex_ST.zw;
			
				o.normal = normalize(mul(unity_ObjectToWorld, half4(v.normal,0.0)).xyz);

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}
			
			// Fragment Function
			fixed4 frag( vertexOutput i ): COLOR {
				fixed tex2 = tex2D(_MainTex2, i.tex.xy).a;
			fixed4 tex = lerp(_PaletteColors[_Tog2], _PaletteColors[_Tog], tex2);


#ifdef GLOBAL_FOG
				 FOGG(tex.rgb)
#endif
				return tex;
			}
			
			ENDCG
		}
			
		Pass{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On ZTest LEqual

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"

			struct appdata
			{
				half4 vertex : POSITION;
			};

			struct v2f
			{
				half4 pos			: SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
		
			fixed4 frag(v2f i) : COLOR
			{
				return fixed4(1,1,1,1);
			}
			ENDCG
		}
		
		
	} 
//	FallBack "Diffuse"
	//
}
