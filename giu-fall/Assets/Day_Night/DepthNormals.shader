// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DepthNormals" {
Properties {
   _MainTex ("", 2D) = "white" {}
   _HighlightDirection ("Highlight Direction", Vector) = (1, 0,0)
}

SubShader {
Tags { "RenderType"="Opaque" }

	//0 RawDepth
	Pass{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

	sampler2D _CameraDepthTexture;

	struct v2f {
	   float4 pos : SV_POSITION;
	   float4 scrPos: TEXCOORD1;
	};

	//Our Vertex Shader
	v2f vert (appdata_base v){
	   v2f o;
	   o.pos = UnityObjectToClipPos (v.vertex);
	   o.scrPos=ComputeScreenPos(o.pos);
	//   o.scrPos.y = 1 - o.scrPos.y;
	   return o;
	}

	sampler2D _MainTex;	
	float4 _HighlightDirection;

	//Our Fragment Shader
	half frag (v2f i) : COLOR{

	 return Linear01Depth((tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos))));

	}
	ENDCG

	}

	//1 BlurPass
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

	//2 FinalDOF v1
	Pass{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"


		struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};

	struct v2f {
	   float4 pos : SV_POSITION;
	   float4 scrPos: TEXCOORD1;
		fixed2 uv	: TEXCOORD0;

	};
		sampler2D _Blur;

	sampler2D _MainTex;
	half4 		_MainTex_ST;

	float4 _Adj;

	sampler2D _CameraDepthTexture;
	//Our Vertex Shader
	v2f vert (appdata v){
	   v2f o;
	   o.pos = UnityObjectToClipPos (v.vertex);
	   o.uv = (v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
	   o.scrPos=ComputeScreenPos(o.pos);
	   return o;
	}

	half4 frag (v2f i) : COLOR{

	    fixed4 col = tex2D(_MainTex, i.uv);
		 fixed4 blur = tex2D(_Blur, i.uv);
    fixed depth = Linear01Depth((tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos))));
    fixed backdepth =clamp((depth - _Adj.x)* _Adj.y ,-1,1);

	fixed scr =clamp((1-i.scrPos.y -0.5)*2, 0,1);
	fixed frontdepth =clamp( (-backdepth +_Adj.z ) * _Adj.w * scr , 0,1);
    return lerp(col,blur,max(abs( backdepth), frontdepth));
	}
	ENDCG

	}

	//3 ADJDepth
	Pass{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

	sampler2D _CameraDepthTexture;
	float4 _Adj;

	struct v2f {
	   float4 pos : SV_POSITION;
	   float4 scrPos: TEXCOORD1;
	};

	//Our Vertex Shader
	v2f vert (appdata_base v){
	   v2f o;
	   o.pos = UnityObjectToClipPos (v.vertex);
	   o.scrPos=ComputeScreenPos(o.pos);
	//   o.scrPos.y = 1 - o.scrPos.y;
	   return o;
	}



	//Our Fragment Shader
	half frag (v2f i) : COLOR{

		half depth = Linear01Depth((tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos))));
		fixed backdepth =clamp((depth - _Adj.x)* _Adj.y ,-1,1);

		fixed scr =clamp((1-i.scrPos.y -0.5)*2, 0,1);
		fixed frontdepth =clamp( (-backdepth +_Adj.z ) * _Adj.w , 0,1);
		return max(abs( backdepth), frontdepth);
	}
	ENDCG

	}

	//4 BlurPass v2
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
	    	
			uniform sampler2D	_Depth;
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
				fixed depth = tex2D(_Depth, i.uv).r;
			    half3 color = tex2D(_MainTex, i.uv).rgb * 0.32754;
			    color += tex2D(_MainTex, i.uv1.xy * depth ).rgb * 0.16377;
			    color += tex2D(_MainTex, i.uv1.zw * depth ).rgb * 0.16377;
			    color += tex2D(_MainTex, i.uv2.xy * depth ).rgb * 0.08188;
			    color += tex2D(_MainTex, i.uv2.zw * depth ).rgb * 0.08188;
			    color += tex2D(_MainTex, i.uv3.xy * depth ).rgb * 0.04094;
			    color += tex2D(_MainTex, i.uv3.zw * depth ).rgb * 0.04094;
			    color += tex2D(_MainTex, i.uv4.xy * depth ).rgb * 0.02047;
			    color += tex2D(_MainTex, i.uv4.zw * depth ).rgb * 0.02047;
			    color += tex2D(_MainTex, i.uv5.xy * depth ).rgb * 0.01023;
			    color += tex2D(_MainTex, i.uv5.zw * depth ).rgb * 0.01023;
			    color += tex2D(_MainTex, i.uv6.xy * depth ).rgb * 0.00511;
			    color += tex2D(_MainTex, i.uv6.zw * depth ).rgb * 0.00511;
				//return depth;
			    return fixed4(color,1);
				    
			}
	    	ENDCG
		}

	}
FallBack "Diffuse"
}