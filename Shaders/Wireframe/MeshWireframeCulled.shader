﻿Shader "Nfynt/MeshWireframeCulled"
{
	Properties
	{
	    _LineColor ("Line color", Color) = (0, 0, 0, 1)
	    _LineSize ("Line size", float) = 0.3
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			
			#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#include "MeshWireframe.cginc"
			ENDCG
		}
	}
}