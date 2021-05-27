Shader "Arany/Separeted/Diffuse_UnLit_Blink" {
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
		_Color ( "Color Tint", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		_ColorMulti ( "Color Multiply", float ) =1
	[Space(5)]
	_MainTex ("Diffuse Texture gloss (a)", 2D) = "white" {}
		[Space(5)]
			_BlinkColor ( "Blink Color", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		_Speed ("Blink Speed", Float) = 255
		_BlinkAmp ("Blink Amplitude", Float) = 255
	//_EmmiA("Emmision", float) = 0
			//[Space(5)]

		//	_RimPower( "Rim Power", float) = 3.0
		//_RimOffset( "Rim Offset", float) = 3.0
		//_RimStr( "Rim Strenght", float) = 3.0
		//_RimStrA( "Rim Strenght Alpha", float) = 3.0

		//[Space(5)]
		//[Toggle(USE_Avoid)] _Avoid("Use Avoid", Int) = 0 
			
	}
	SubShader {
		Pass{
			Tags { "LightMode" = "ForwardBase" "Queue"="Geometry" "RenderType"="Overlay"}

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

			//#include "UnityCG.cginc"
			//#include "AutoLight.cginc"
			//#include "Lighting.cginc"
		
		//#pragma multi_compile __ GLOBAL_FOG
			//#pragma shader_feature __ USE_GroundFOGText	
			//#pragma shader_feature __ USE_Avoid
			//#pragma multi_compile_fwdadd_fullshadows


			#include "arany.cginc"
			
			

			uniform float4 _Color;
			uniform float _ColorMulti;

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;


			struct vertexInput {
				float4 vertex: POSITION;
				//float3 normal: NORMAL;
				half4 color : COLOR;
				float4 texcoord: TEXCOORD0;

			};
			struct vertexOutput {
				float4 pos: SV_POSITION;
				half4 color : color;
				float2 tex: TEXCOORD0;
				float adj: TEXCOORD1;
				//float4 posWorld: TEXCOORD1;
				//float3 normal: TEXCOORD5;
				//LIGHTING_COORDS(7,8)
			};

			vertexOutput vert( vertexInput v ) {
				vertexOutput o;
				//UNITY_INITIALIZE_OUTPUT(vertexOutput, o);
				 o.color = v.color;
				o.pos = UnityObjectToClipPos(v.vertex);

				//o.posWorld = mul( unity_ObjectToWorld, v.vertex );
			
				o.tex = v.texcoord.xy  * _MainTex_ST.xy + _MainTex_ST.zw;
				float tadj;
				BLINK_Vert  (o.adj)
				//o.normal = normalize(mul(unity_ObjectToWorld, half4(v.normal,0.0)).xyz);

				//TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}
			
			// Fragment Function
			fixed4 frag( vertexOutput i ): COLOR {

				fixed4 tex = _Color * tex2D( _MainTex, i.tex.xy)  * i.color * _ColorMulti;

				//UNITY_LIGHT_ATTENUATION(atten, i, i.posWorld.xyz);

				//LIGHT(tex.rgb, atten)
				//LIGHT_EM(tex.rgb, atten, _EmmiA )
				//LIGHT_NOSHADOW(tex.rgb)

			
				//COLADJ(tex.rgb)
				

				//RIM(tex.rgb, atten)
				//RIM(tex.rgb, 1)

				//#ifdef GLOBAL_FOG
				//	FOGG(tex.rgb)
				//#endif

				
				//AVOID(tex.rgb)
			

				//return fixed4(tex.rgb, tex.a) + fixed4(1,0,1,1);
				BLINK_Frag(tex, i.adj)
				return tex ;
				//return i.color;
				//return fixed4(1,0,1,1);
			}
			
			ENDCG
		}
			
	
	} 
}
