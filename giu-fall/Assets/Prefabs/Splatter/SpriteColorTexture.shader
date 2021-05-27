// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dani/SpriteColorTexture"
{
	Properties
	{
		//_Brightness ("Additive Brightness", Range(0,1)) = 0
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color ("1st Color", Color) = (1,1,1,1)

		//targetPos ("2st Color", Float) = (1,1,1,1)

		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			//"IgnoreProjector"="True" 
			//"RenderType"="Transparent" 
			//"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				fixed2 uv  : TEXCOORD0;
			};
			
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv = TRANSFORM_TEX(IN.texcoord, _MainTex);
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif
				OUT.color = IN.color;
				return OUT;
			}

		


			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex,i.uv) * _Color * i.color;
				return c;
			}
		ENDCG
		}
	}
}
