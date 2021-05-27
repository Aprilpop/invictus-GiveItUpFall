// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Scribe/ChromaticDistortion" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Radius ( "Radius", float ) = 0.005
		_Radius2 ( "Radius Main", float ) = 0.005
		_Str ( "strength", float ) =1
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
				fixed4 uv2	: TEXCOORD1;
				//fixed d	: TEXCOORD2;
	    	};
	    	
			uniform sampler2D	_MainTex;

			half4 				_MainTex_ST;

			float _Radius;
			float _Radius2;
			float _Str;

			//float2 Distore(float2 _in, float _radius, out float _d)
			//{
			//			float2 chuv = _in ;
			//	float2 cc = _in *2 -1;

			//	//_d = abs(  abs(cc.x) + abs(cc.y)  );
			//	//chuv += cc * _d * _radius;

			//	_d = max(  abs(cc.x) , abs(cc.y)  );
			//	chuv += cc * _d * _radius;
				
			//	return chuv;
			//}

			float2 Distore(float2 _in, float _radius)
			{
						float2 chuv = _in ;
				float2 cc = _in *2 -1;

				//_d = abs(  abs(cc.x) + abs(cc.y)  );
				//chuv += cc * _d * _radius;

				float _d = max(  abs(cc.x) , abs(cc.y)  );
				chuv += cc * _d * _radius;
				
				return chuv;
			}

	    	v2f vert(appdata v){
	    	
	    		v2f o;
	    		o.pos = UnityObjectToClipPos(v.vertex);
				o.uv =(v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				
	    		o.uv2.xy = Distore(o.uv, _Radius );
				o.uv2.zw = Distore(o.uv, -_Radius);
				//o.d = _d *_d;


	    		return o;
	    	}
	    	
			fixed4 frag(v2f i) : COLOR
			{
			float d = 0;
				fixed3 col = tex2D(_MainTex,i.uv.xy).rgb;

				fixed3 dist = col;
				//dist.r = tex2D(_MainTex,Distore(i.uv.xy, _Radius + _Radius2, d)).r;
				//dist.b = tex2D(_MainTex,Distore(i.uv.xy, -_Radius + _Radius2 , d)).b;
				//d *= d;
				//col  = lerp(col,dist, d  *_Str);

					float2 cc = i.uv *2 -1;


				fixed _d = max(  abs(cc.x) , abs(cc.y)  );

				dist.r = tex2D(_MainTex,i.uv2.xy).r;
				dist.b = tex2D(_MainTex,i.uv2.zw).b;
				col  = lerp(col,dist, _d * _d * _d  *_Str);

	  			return fixed4(col, 1);
			}
	    	ENDCG
		}

	} 
}
