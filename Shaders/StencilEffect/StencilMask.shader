Shader "Nfynt/Mask" {
	SubShader {
		Tags { "RenderType"="Transparent" }
		Stencil {
			Ref 1
			Comp always
			Pass replace
		}

		CGPROGRAM
		#pragma surface surf Lambert alpha

		struct Input {
			fixed3 Albedo;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = fixed3(1, 1, 1);
			o.Alpha = 0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}