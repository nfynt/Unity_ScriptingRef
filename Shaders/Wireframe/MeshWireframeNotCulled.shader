Shader "Nfynt/MeshWireframeNotCulled"
{
	Properties
	{
	    _LineColor ("Line color", Color) = (0, 0, 0, 1)
	    _LineSize ("Line size", float) = 0.3
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+100" }
		LOD 100

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#include "MeshWireframe.cginc"
			ENDCG
		}
		
		Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
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
