//
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
// Upgrade NOTE: excluded shader from DX11 because it uses wrong array syntax (type[size] name)
#pragma exclude_renderers d3d11
#pragma multi_compile __ USE_GroundFOGText	
#pragma multi_compile __ GLOBAL_FOG

#if defined(UNITY_LIGHT_PROBE_PROXY_VOLUME)
#undef UNITY_LIGHT_PROBE_PROXY_VOLUME
#endif
#define UNITY_LIGHT_PROBE_PROXY_VOLUME 0

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"

	uniform float4 _ColorG[3];		

#define LIGHT(_col, _atten) \
fixed3 lightDirection =normalize( _WorldSpaceLightPos0.xyz); \
fixed diffuseReflection = max(dot(normalize(  i.normal), lightDirection), 0.0)  ;\
_col =lerp(_col * UNITY_LIGHTMODEL_AMBIENT.rgb, _col , diffuseReflection ) ;

//	#define LIGHT(_col, _atten) \
//fixed3 lightDirection =normalize( _WorldSpaceLightPos0.xyz); \
//fixed diffuseReflection = max(dot(normalize(  i.normal), lightDirection), 0.0)  ;\
//_col =diffuseReflection ;

fixed _EmmiA;

#define LIGHT_EM(_col, _atten, _emmi ) \
fixed3 lightDirection =normalize( _WorldSpaceLightPos0.xyz); \
fixed diffuseReflection = max(dot(normalize(  i.normal), lightDirection), 0.0) * _atten ;\
 _col =lerp(  lerp(_col * UNITY_LIGHTMODEL_AMBIENT.rgb, _col * _LightColor0.rgb, diffuseReflection ) ,_col ,_emmi);

///

#define LIGHT_NOSHADOW(_col)  fixed3 lightDirection =normalize( _WorldSpaceLightPos0.xyz); fixed diffuseReflection = max(dot(normalize(  i.normal), lightDirection), 0.0)  ; _col =lerp(_col * UNITY_LIGHTMODEL_AMBIENT.rgb, _col * _LightColor0.rgb, diffuseReflection ) ;


fixed3				_RGBMOD;
fixed3				_BSCMOD;
fixed2				_tintScale = fixed2(1.0,-1.0);
fixed3				_tintColor;

#define COLADJ(_col) \
	fixed lum = dot(_col, fixed3(0.2125, 0.7154, 0.0721)); \
	fixed3 lumColor = fixed3(lum, lum, lum); \
	fixed3 satColor =clamp( lerp(lumColor, _col, _BSCMOD.y), 0,1); \
	fixed3 rgbColor = clamp((lerp(fixed3(0.5, 0.5, 0.5), satColor, _BSCMOD.z) * _RGBMOD) *_BSCMOD.x, 0.0, 1.0); \
	_col = lerp(rgbColor,lumColor +_tintColor,1-clamp(lumColor * lumColor * _tintScale.x - _tintScale.y,0.0,1.0));




	
#ifdef USE_GroundFOGText
	sampler2D _FogTex;
	float4 _FogLayer1;
	float4 _FogLayer2;
#endif
	float _FogEnd;
	float3 _FogCenter;
	uniform fixed4 _GroundFogColor;
	fixed4 _HeightFogColor;
	fixed4 _FogColor;
	float4 _FogSE;
	float4 _FadeDist;
	float4 _FadeTarget;

	
#ifdef USE_GroundFOGText
	#define FOGG(_col) \
		fixed _dist =distance( _FogCenter.xyz , i.posWorld.xyz );\
				fixed adjF = clamp( (_dist - _FogSE.x) / (_FogSE.y - _FogSE.x),0,1);\
		adjF = clamp(adjF *( 2 - adjF),0,1);\
		fixed DistanceTop =  clamp(   i.posWorld.y -_FadeTarget.x,0 , _FadeTarget.y - _FadeTarget.x ); \
		fixed changeDistanceTop = (DistanceTop ) / (_FadeTarget.y - _FadeTarget.x);\
		fixed3 col2 = lerp(_col, _HeightFogColor.rgb,changeDistanceTop * _HeightFogColor.a);\
		fixed DistanceY =  clamp(   i.posWorld.y -_FogSE.z,0 , _FogSE.w-_FogSE.z );\
		fixed changeFactorY =1-  (DistanceY ) / (_FogSE.w - _FogSE.z);\
		fixed _fog = tex2D( _FogTex,( float2( i.posWorld.x /_FogLayer1.x + _Time.y * _FogLayer1.z, i.posWorld.z / _FogLayer1.y + i.posWorld.y/ _FogLayer1.y + _Time.y * _FogLayer1.w))    );\
		fixed _fog2 = tex2D( _FogTex, (float2( i.posWorld.x /_FogLayer2.x + _Time.y * _FogLayer2.z, i.posWorld.z /_FogLayer2.y + i.posWorld.y /_FogLayer2.y + _Time.y * _FogLayer2.w) )   );\
		fixed finalfog =  clamp( max(_fog ,  _fog2)   , 0,1) ;\
		col2 = lerp(col2, _GroundFogColor.rgb *changeFactorY ,changeFactorY   * finalfog *_GroundFogColor.a);\
		_col.rgb = lerp(col2, _FogColor.rgb ,  adjF *_FogColor.a);
#else
	#define FOGG(_col) \
		fixed _dist =distance( _FogCenter.xyz , i.posWorld.xyz );\
		fixed adjE = clamp( (_dist - _FogEnd) / ( 50),0,1);\
		fixed adjF = clamp( (_dist - _FogSE.x) / (_FogSE.y - _FogSE.x),0,1);\
		adjF = clamp(adjF *( 2 - adjF),0,1);\
		fixed DistanceTop =  clamp(   i.posWorld.y -_FadeTarget.x,0 , _FadeTarget.y - _FadeTarget.x ); \
		fixed changeDistanceTop = (DistanceTop ) / (_FadeTarget.y - _FadeTarget.x);\
		fixed3 col2 = lerp(_col, _HeightFogColor.rgb,changeDistanceTop * _HeightFogColor.a);\
		fixed DistanceY =  clamp(   i.posWorld.y -_FogSE.z,0 , _FogSE.w-_FogSE.z );\
		fixed changeFactorY =1-  (DistanceY ) / (_FogSE.w - _FogSE.z);\
		col2 = lerp(col2, _GroundFogColor.rgb *changeFactorY ,changeFactorY   *_GroundFogColor.a);\
		_col.rgb = lerp(col2, _FogColor.rgb , adjF   * max(_FogColor.a, adjE));
#endif

//_col.rgb = lerp(col2, _FogColor.rgb , adjF   * max(_FogColor.a, adjE));
#ifdef USE_GroundFOGText
	#define GROUNDFOGG(_col) \
		fixed _dist =distance( _FogCenter.xyz , i.posWorld.xyz );\
				fixed adjF = clamp( (_dist - _FogSE.x) / (_FogSE.y - _FogSE.x),0,1);\
		adjF = clamp(adjF *( 2 - adjF),0,1);\
		fixed DistanceY =  clamp(   i.posWorld.y -_FogSE.z,0 , _FogSE.w-_FogSE.z );\
		fixed changeFactorY =1-  (DistanceY ) / (_FogSE.w - _FogSE.z);\
		fixed _fog = tex2D( _FogTex,( float2( i.posWorld.x + _Time.y, i.posWorld.z + i.posWorld.y))  /20  );\
		fixed _fog2 = tex2D( _FogTex, (float2( i.posWorld.x + _Time.y /5, i.posWorld.z + i.posWorld.y + _Time.y /5) ) /50  );\
		fixed finalfog =  clamp( max(_fog ,  _fog2)  , 0,1) ;\
		fixed3 col2 = lerp(_col.rgb, _GroundFogColor.rgb   ,changeFactorY   * finalfog *_GroundFogColor.a);\
		_col.rgb = lerp(col2, _FogColor.rgb ,  adjF *_FogColor.a);
#else
	#define GROUNDFOGG(_col) \
		fixed _dist =distance( _FogCenter.xyz , i.posWorld.xyz );\
				fixed adjF = clamp( (_dist - _FogSE.x) / (_FogSE.y - _FogSE.x),0,1);\
		adjF = clamp(adjF *( 2 - adjF),0,1);\
		fixed DistanceY =  clamp(   i.posWorld.y -_FogSE.z,0 , _FogSE.w-_FogSE.z );\
		fixed changeFactorY =1-  (DistanceY ) / (_FogSE.w - _FogSE.z);\
		fixed finalfog =  changeFactorY;\
		fixed3 col2 = lerp(_col.rgb, _GroundFogColor.rgb *changeFactorY  ,changeFactorY   * finalfog *_GroundFogColor.a);\
		_col.rgb = lerp(col2, _FogColor.rgb , adjF *_FogColor.a);
#endif


/*	
#ifdef USE_GroundFOGText
	#define GROUNDFOGG(_col) \
		fixed DistanceY =  clamp(   i.posWorld.y -_FogSE.z,0 , _FogSE.w-_FogSE.z );\
		fixed changeFactorY =1-  (DistanceY ) / (_FogSE.w - _FogSE.z);\
		fixed _fog = tex2D( _FogTex,( float2( i.posWorld.x + _Time.y, i.posWorld.z))  /20  );\
		fixed _fog2 = tex2D( _FogTex, (float2( i.posWorld.x + _Time.y /5, i.posWorld.z + _Time.y /5) ) /50  );\
		fixed finalfog =  clamp( max(_fog ,  _fog2) + changeFactorY * 0.2   , 0,1) ;\
		_col.rgb = lerp(_col.rgb, _GroundFogColor.rgb *changeFactorY ,changeFactorY   * finalfog *_GroundFogColor.a);
#else
	#define GROUNDFOGG(_col) \
		fixed DistanceY =  clamp(   i.posWorld.y -_FogSE.z,0 , _FogSE.w-_FogSE.z );\
		fixed changeFactorY =1-  (DistanceY ) / (_FogSE.w - _FogSE.z);\
		_col.rgb = lerp(_col.rgb, _GroundFogColor.rgb *changeFactorY ,changeFactorY   * changeFactorY *_GroundFogColor.a);
#endif
*/

	fixed _AvoidMulti;
	fixed4 _AvoidColor;
	fixed4 _AvoidVector;
	fixed4 _AvoidVector2;

#define AVOID(_col) \
	fixed adj = (sin(_Time.y * 15) + 1) / 2 * 0.2f ;\
	fixed d =clamp (  abs (i.posWorld.x - _AvoidVector2.x) /  _AvoidVector.x ,0,1) ;\
	d= 1- d*d*d*d;\
	fixed dy =clamp (  abs (i.posWorld.y - _AvoidVector2.y) /  _AvoidVector.y ,0,1) ;\
	dy =clamp( 1 - (dy *dy*dy),0,1);\
	fixed dz = abs(i.posWorld.z ) /  _AvoidVector.z;\
	dz =clamp( 1 - (dz *dz*dz),0,1);\
	_col = lerp(_col ,  _AvoidColor.rgb * _AvoidMulti, clamp(  d *dz * dy ,0,1)* adj) ;\


	

	////afafaf

					uniform float _RimPower;
			uniform float _RimOffset;
			uniform float _RimStr;
			uniform float _RimStrA;
		
		#define RIM(_col, _atten) \
					fixed3 _normal = normalize(i.normal);\
					fixed rimDot = 1 - saturate(dot( normalize(i.viewDir), _normal));\
					_col  += _LightColor0.rgb * pow(rimDot + _RimOffset, _RimPower) * _RimStr * _atten;

							int _toggle;
					
					uniform float4 _WarnColor;
					uniform float4 _PickColor;
		#define RIMWarn(_col, _adj) \
					fixed3 _normal = normalize(i.normal);\
					fixed rimDot = 1 - saturate(dot( normalize(i.viewDir), _normal));\
					_col.rgb  = lerp(_col.rgb,_ColorG[_toggle].rgb, min(_ColorG[_toggle].a, ( pow(rimDot, _RimPower) * _RimStr  + _RimOffset) *_adj));


					//_col  += _col * _LightColor0.rgb * (atten*rimDot);\

/*
			#define RIM(_col, _atten) \
					fixed3 _normal = normalize(i.normal);\
					fixed rimDot = 1 - saturate(dot( normalize(i.viewDir), _normal));\
					fixed NdotL = dot(_WorldSpaceLightPos0, _normal);\
					fixed rimIntensity = rimDot * pow(NdotL, _RimStr);\
					_col  += _col * _LightColor0.rgb * (atten*rimIntensity);\			
				
	/*		
#define RIM(_col, _atten) \
		fixed viewDirection = normalize( _WorldSpaceCameraPos.xyz - i.posWorld.xyz );\
		fixed rimPow =pow( 1.0 - saturate( dot( viewDirection, i.normal ) ), _RimPower );\
		_col +=rimPow;
*/
			fixed4 _BlinkColor;
			float _Speed;
			float _BlinkAmp;	

		#define BLINK_Vert(_adj) _adj = (sin(_Time.y * _Speed) + 1) / 2 * _BlinkAmp ;

		#define BLINK_Frag(_col, _adj) _col = lerp(_col,_BlinkColor,_adj);

		#define BLINK_Frag2(_col, _adj) _col.rgb = lerp(_col.rgb,_ColorG[_toggle].rgb, min(_ColorG[_toggle].a, _adj));	

		#define BLINK_Frag3(_col, _adj) \
		_col.rgb *= _ColorG[_toggle];\
		_col.a *=  min(_ColorG[_toggle].a, _adj);	

			uniform float _Shininess;
			uniform float _SpecSTR;

#define SPEC(_col, _atten) \
			fixed3 h = normalize(normalize( _WorldSpaceLightPos0.xyz) + normalize( _WorldSpaceCameraPos - i.posWorld ));\
			fixed intSpec = max(dot(h, normalize(  i.normal)), 0.0);\
			fixed shine = _Shininess * _Shininess ;\
			_col += _LightColor0.rgb * pow(intSpec,shine) * _SpecSTR *_atten ;