Shader "Arany/Separeted/Overlay" {
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
		_Color2 ( "Color 2", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		_ColorMulti ( "Color Multiply", float ) =1
	[Space(5)]
	_MainTex ("Diffuse RGB", 2D) = "white" {}
	_DamageTex ("Damage A", 2D) = "black" {}
		[Space(5)]
		//_DamageDir ( "DamageDir", Float ) = 1
			_Fade ( "_Fade", Float ) = 0
			_Fade2 ( "_Fade", Float ) = 1
		_ColorGas ( "Color 2", Color ) = ( 1.0, 1.0, 1.0, 1.0 )

			_FadeGas ( "_FadeGas", Float ) = 0
			_Fade2Gas ( "_Fade2Gas", Float ) = 1

		//	_BlinkColor ( "Blink Color", Color ) = ( 1.0, 1.0, 1.0, 1.0 )
		//_Speed ("Blink Speed", Float) = 255
		//_BlinkAmp ("Blink Amplitude", Float) = 255
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

			#include "UnityCG.cginc"
			//#include "AutoLight.cginc"
			//#include "Lighting.cginc"
		
		//#pragma multi_compile __ GLOBAL_FOG
			//#pragma shader_feature __ USE_GroundFOGText	
			//#pragma shader_feature __ USE_Avoid
			//#pragma multi_compile_fwdadd_fullshadows


			//#include "arany.cginc"
			
			 //uniform float4 _ScreenParams;

			uniform float4 _Color;
			uniform float4 _Color2;
			uniform float _Fade;
			uniform float _Fade2;

			uniform float _FadeGas;
			uniform float _Fade2Gas;
			uniform float4 _ColorGas;
			uniform float _ColorMulti;

			uniform sampler2D _MainTex;
			uniform sampler2D _DamageTex;
			uniform float4 _MainTex_ST;
			uniform float2 _DamageDir;

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
				float4 screenpos : TEXCOORD2;
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
				o.screenpos = ComputeScreenPos(o.pos);
				//float tadj;
				//BLINK_Vert  (o.adj)
				//o.normal = normalize(mul(unity_ObjectToWorld, half4(v.normal,0.0)).xyz);

				//TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}
			
			// Fragment Function
			fixed4 frag( vertexOutput i ): COLOR {

				fixed3 tex = tex2D( _MainTex, i.tex.xy) * _Color.rgb * _ColorMulti * i.color.rgb;
				fixed damTex = tex2D( _DamageTex, i.tex.xy).r;
				float2 _uv = i.screenpos.xy / i.screenpos.w;
				fixed2 _sC = fixed2( _uv.x  *2 -1 , _uv.y *2 -1 );
				fixed _s = clamp(1-(abs(_uv.x - _DamageDir.x) ),0,1) * clamp(1-(abs(_uv.y - _DamageDir.y) ),0,1);

				//fixed _s = max( dot( _DamageDir.xy  , i.screenpos ),0);
                
                //fixed3 col = damTex;
                fixed3 colGas = lerp(  tex, lerp( tex, _ColorGas.rgb, clamp( pow( damTex, max(_Fade2Gas,1)), 0, 1)), _FadeGas);
				fixed3 col = lerp(  colGas, lerp( colGas, _Color2.rgb, clamp( pow( damTex,max( _Fade2,1))* (_s ), 0, 1)),_Fade);
				


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
				//BLINK_Frag(tex, i.adj)
				//return col ;

				return fixed4(col,1);
				//return fixed4(_s,0,0,1);
				//return fixed4(_sc.x, _sc.y, 0, 1);
			}
			
			ENDCG
		}
			
	
	} 
}
