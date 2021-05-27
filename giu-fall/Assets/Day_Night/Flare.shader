// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with '_Object2World'

Shader "Scribe/Flare" {
	Properties {
	 
	    [HideInInspector] _MainTex ("Control (RGBA)", 2D) = "white" {}
	    [HideInInspector] _FlareTex ("Control (RGBA)", 2D) = "white" {}
	}
	     
	SubShader {
		Fog { Mode off }
	    Pass
	    {
	    	Cull Off
	    	ZWrite Off
	    	Blend Off
			ColorMask A

	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex : POSITION;
				fixed4 color  : COLOR0;
	    	};
	    	
	    	struct v2f{
	    		half4 pos : SV_POSITION;
	    		fixed4 color : TEXCOORD0;
	    	};
	    	
	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = mul(unity_ObjectToWorld,v.vertex);
				#if !UNITY_UV_STARTS_AT_TOP
					o.pos.y = -o.pos.y;
				#endif
				o.color = v.color;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				return i.color;
			}
	    	
	    	ENDCG
	    }
	 
	    Pass
	    {
	    	Cull Off
	    	ZWrite Off
	    	ZTest Always
	    	Blend SrcAlpha One
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed4 uv	: TEXCOORD0;
	    	};
	    	
			uniform half4 _UVOffset;
			uniform fixed2 _Center;
			uniform fixed4 _Color;
			
			uniform sampler2D _MainTex;
			uniform sampler2D _FlareTex;
	    	
	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = mul(unity_ObjectToWorld,v.vertex);
	    		o.uv.xy = (v.texcoord.xy*_UVOffset.xy)+_UVOffset.zw;
	    		o.uv.zw = _Center;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				bool notVisible = bool(tex2D(_FlareTex,i.uv.zw).a);
				fixed3 col = tex2D(_MainTex,i.uv.xy).rgb * _Color.rgb;
				return fixed4(col * (_Color.a * fixed(!notVisible)),1);
			}
	    	ENDCG
	    }

	    Pass
	    {
	    	Cull Off
	    	ZWrite Off
	    	ZTest Always
	    	Blend SrcAlpha One
	    	CGPROGRAM
	    	
	    	#pragma vertex vert
	    	#pragma fragment frag
	    	
	    	struct appdata{
	    		half4 vertex 	: POSITION;
				fixed4 color	: COLOR0;
	    		half4 texcoord : TEXCOORD0; 
	    	};
	    	
	    	struct v2f{
	    		half4 pos	: SV_POSITION;
	    		fixed4 uv	: TEXCOORD0;
				fixed4 col  : TEXCOORD1;
	    	};
	    	
			uniform fixed2 _Center;
			
			uniform sampler2D _MainTex;
			uniform sampler2D _FlareTex;
	    	
	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = mul(unity_ObjectToWorld,v.vertex);
				o.col = v.color;
	    		o.uv.xy = v.texcoord.xy;
	    		o.uv.zw = _Center;
	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
				bool notVisible = bool(tex2D(_FlareTex,i.uv.zw).a);
				fixed3 col = tex2D(_MainTex,i.uv.xy).rgb * i.col.rgb;
				return fixed4(col * (i.col.a * fixed(!notVisible)),1);
			}
	    	ENDCG
	    }

	 	 	 
	} // End SubShader
	// Fallback to Diffuse
	Fallback "Diffuse"
} // Ehd Shader