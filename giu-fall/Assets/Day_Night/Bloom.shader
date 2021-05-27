// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Scribe/Bloom" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Blend Off
		Fog { Mode off }

		Pass{
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed2 uv	: TEXCOORD0;
	    	};
	    	
			
			uniform sampler2D 	_MainTex;
			half4 				_MainTex_ST;
			fixed				_Splitter;			

	    	
	    	v2f vert(appdata v){
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
	    		o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				fixed3 color = tex2D(_MainTex, i.uv).rgb;
				return fixed4(color * fixed(bool(clamp(dot(color,fixed3(0.2125, 0.7154, 0.0721)) -_Splitter,0.0,1.0))),1.0);
			}
	    	ENDCG
		}

		Pass{
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed2 uv	: TEXCOORD0;
	    	};
	    	
			
			uniform sampler2D 	_MainTex;
			half4 				_MainTex_ST;
			fixed4				_Color;		
			fixed				_Splitter;			

	    	
	    	v2f vert(appdata v){
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
	    		o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				fixed3 color = tex2D(_MainTex, i.uv).rgb;
				return fixed4(_Color.rgb * fixed(bool(clamp(dot(color,fixed3(0.2125, 0.7154, 0.0721)) -_Splitter,0.0,1.0))),1.0);
				    
			}
	    	ENDCG
		}
		
		Pass{
		
	    	CGPROGRAM
	    	#pragma target 3.0
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed2 uv	: TEXCOORD0;
				fixed4 uv1	: TEXCOORD1;
				fixed4 uv2	: TEXCOORD2;
				fixed4 uv3	: TEXCOORD3;
				fixed4 uv4	: TEXCOORD4;
				fixed4 uv5	: TEXCOORD5;
				fixed4 uv6	: TEXCOORD6;
	    	};
	    	
			
			uniform sampler2D	_MainTex;
			half4 _MainTex_ST;
	        fixed2 _UvStep;

	    	
	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
	    		o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	            o.uv1 = fixed4(o.uv - _UvStep * 2.0, o.uv + _UvStep * 2.0);
	            o.uv2 = fixed4(o.uv - _UvStep * 3.0, o.uv + _UvStep * 3.0);
	            o.uv3 = fixed4(o.uv - _UvStep * 4.0, o.uv + _UvStep * 4.0);
	            o.uv4 = fixed4(o.uv - _UvStep * 5.0, o.uv + _UvStep * 5.0);
	            o.uv5 = fixed4(o.uv - _UvStep * 6.0, o.uv + _UvStep * 6.0);
	            o.uv6 = fixed4(o.uv - _UvStep * 7.0, o.uv + _UvStep * 7.0);
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
			    half3 color = tex2D(_MainTex, i.uv).rgb * 0.32754;
			    color += tex2D(_MainTex, i.uv1.xy).rgb * 0.16377;
			    color += tex2D(_MainTex, i.uv1.zw).rgb * 0.16377;
			    color += tex2D(_MainTex, i.uv2.xy).rgb * 0.08188;
			    color += tex2D(_MainTex, i.uv2.zw).rgb * 0.08188;
			    color += tex2D(_MainTex, i.uv3.xy).rgb * 0.04094;
			    color += tex2D(_MainTex, i.uv3.zw).rgb * 0.04094;
			    color += tex2D(_MainTex, i.uv4.xy).rgb * 0.02047;
			    color += tex2D(_MainTex, i.uv4.zw).rgb * 0.02047;
			    color += tex2D(_MainTex, i.uv5.xy).rgb * 0.01023;
			    color += tex2D(_MainTex, i.uv5.zw).rgb * 0.01023;
			    color += tex2D(_MainTex, i.uv6.xy).rgb * 0.00511;
			    color += tex2D(_MainTex, i.uv6.zw).rgb * 0.00511;
			    return fixed4(color,1);
				    
			}
	    	ENDCG
		}

		Pass{
		
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed2 uv	: TEXCOORD0;
	    	};
	    	
			
			uniform sampler2D	_BlurTex;
			uniform sampler2D	_MainTex;

			half4 				_MainTex_ST;
			fixed		 		_BlurInt;
			fixed3 				_ModChannel;
			fixed3				_ModColor;

	    	
	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
	    		o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				half3 color = tex2D(_MainTex, i.uv).rgb;
				
				//fixed3 blur =clamp( tex2D(_BlurTex,i.uv).rgb * _BlurInt, 0,0.3);
				fixed3 blur =tex2D(_BlurTex,i.uv).rgb * _BlurInt;

				//color = clamp((color+blur) - color*blur,0.0,1.0);
				color = clamp((color+blur) - color*blur,0.0,1.0);
			    color += _ModChannel;
			    half3 brtColor = color * _ModColor.x;
				half lum = dot(brtColor, half3(0.2125, 0.7154, 0.0721));
	  			half3 satColor = lerp(half3(lum,lum,lum), brtColor, _ModColor.y);
	  			half3 conColor = clamp(lerp(half3(0.5, 0.5, 0.5), satColor, _ModColor.z),0.0,1.0);
	  			return fixed4(conColor,1);
			}
	    	ENDCG
		}

		Pass{
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed2 uv	: TEXCOORD0;
	    	};
	    	
			
			uniform sampler2D	_BlurTex;
			uniform sampler2D	_MainTex;

			half4 				_MainTex_ST;
			fixed		 		_BlurInt;

	    	
	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
	    		o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				fixed3 blur = tex2D(_BlurTex,i.uv).rgb * _BlurInt;
	  			return fixed4(blur,1);
			}
	    	ENDCG
		}

		Pass{

				CGPROGRAM

#pragma vertex vert
#pragma fragment frag

			struct appdata {
				half4 vertex 	: POSITION;
				half4 texcoord : TEXCOORD0;
			};

			struct v2f {
				half4 pos	: SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};

			uniform sampler2D	_MainTex;
			half4 				_MainTex_ST;
			fixed3 				_ModChannel;
			fixed3				_ModColor;


			v2f vert(appdata v) {

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				half3 color = tex2D(_MainTex, i.uv).rgb;
				color += _ModChannel;
				half3 brtColor = color * _ModColor.x;
				half lum = dot(brtColor, half3(0.2125, 0.7154, 0.0721));
				half3 satColor = lerp(half3(lum,lum,lum), brtColor, _ModColor.y);
				half3 conColor = clamp(lerp(half3(0.5, 0.5, 0.5), satColor, _ModColor.z),0.0,1.1);
				return fixed4(conColor,1);
			}
				ENDCG
			}


		Pass{
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed2 uv	: TEXCOORD0;
	    	};
	    	
			
			uniform sampler2D 	_MainTex;
			half4 				_MainTex_ST;
			fixed				_Splitter;		

	    	
	    	v2f vert(appdata v){
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
	    		o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				fixed3 color = tex2D(_MainTex, i.uv).rgb;
				//return fixed4(color * fixed(bool(clamp(dot(color,fixed3(0.2125, 0.7154, 0.0721)) -_Splitter,0.0,1.0))),1.0);

				fixed high = fixed(bool(clamp(dot(color,fixed3(0.2125, 0.7154, 0.0721)) -_Splitter,0.0,1.0)));
				fixed low =	fixed(bool(clamp(1-(dot(color,fixed3(0.2125, 0.7154, 0.0721)) *_Splitter *4),0.0,1.0)));
				fixed final = clamp(high + low,0,1);
				return fixed4(lerp(fixed3(0.5, 0.5,0.5), color, final),1);
			}
	    	ENDCG
		}

		Pass{
	//	Blend DstColor SrcColor
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed2 uv	: TEXCOORD0;
	    	};
	    	
			
			uniform sampler2D	_BlurTex;
			uniform sampler2D	_MainTex;

			half4 				_MainTex_ST;
			fixed		 		_BlurInt;
			fixed3 				_ModChannel;
			fixed3				_ModColor;

	    	
	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
	    		o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				half3 color = tex2D(_MainTex, i.uv).rgb;
				
				fixed3 blur =pow( tex2D(_BlurTex,i.uv).rgb , _BlurInt);
				color = clamp(color+blur * 2,0.0,1.0);

			    color += _ModChannel;
			    half3 brtColor = color * _ModColor.x;
				half lum = dot(brtColor, half3(0.2125, 0.7154, 0.0721));
	  			half3 satColor = lerp(half3(lum,lum,lum), brtColor, _ModColor.y);
	  			half3 conColor = clamp(lerp(half3(0.5, 0.5, 0.5), satColor, _ModColor.z),0.0,1.1);
	  			return fixed4(conColor,1);
			}
	    	ENDCG
		}



	} 
}
