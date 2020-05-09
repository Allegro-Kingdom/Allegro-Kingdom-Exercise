#ifndef MK_GLOW_INC
	#define MK_GLOW_INC

	void Blur(inout fixed4 c, sampler2D tex, float2 uv, fixed2 offUv)
	{
		#if MK_GLOW_BLUR_HQ
		for (int p = 0; p < 5; p++)
		{
			#if UNITY_SINGLE_PASS_STEREO
			c += tex2D(_MKGlowTex, UnityStereoScreenSpaceUVAdjust(uv.xy + offUv * p, _MainTex_ST));
			c += tex2D(_MKGlowTex, UnityStereoScreenSpaceUVAdjust(uv.xy - offUv * p, _MainTex_ST));
		#else
			c += tex2D(tex, uv.xy + offUv * p);
			c += tex2D(tex, uv.xy - offUv * p);
		#endif
		}
		c /= 10;
		#else
		#if UNITY_SINGLE_PASS_STEREO
			c += tex2D(_MKGlowTex, UnityStereoScreenSpaceUVAdjust(uv.xy + offUv, _MainTex_ST));
			c += tex2D(_MKGlowTex, UnityStereoScreenSpaceUVAdjust(uv.xy - offUv, _MainTex_ST));
		#else
			c += tex2D(tex, uv.xy + offUv);
			c += tex2D(tex, uv.xy - offUv);
		#endif
		c /= 2;
		#endif
	}
#endif