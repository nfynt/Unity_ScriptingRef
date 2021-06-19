Shader "Nfynt/MeshOutline"
{
    Properties
    {
        _OutlineWidth("Outline Width", Range(1.0,5.0)) = 1.1
        _OutlineColor("Outline Color", Color) = (1,1,1,0.5)
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent"  }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass{
            cull off
            ZWrite off
            ZTest Always
        HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

        #include "UnityCG.cginc"

            struct VertexInput
            {
                float4 vertex: POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(VertexInput v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex * _OutlineWidth);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }

        ENDHLSL

        }

    }
    FallBack "Diffuse"
}
