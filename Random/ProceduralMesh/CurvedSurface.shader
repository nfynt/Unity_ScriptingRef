Shader "Nfynt/CylindericalTextureMap"{
    Properties
    {
        [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            Cull Off
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                struct appdata
                {
                    float4 vertex : POSITION;   // vertex position
                    float2 uv : TEXCOORD0;      // texture coordinate
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;      // texture coordinate
                    float4 vertex : SV_POSITION;    // clip space position
                };

                sampler2D _MainTex;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    if (i.uv[0] > 1 || i.uv[0]<0) discard;
                    fixed4 col = tex2D(_MainTex, i.uv);
                    return col;
                }
                ENDCG
            }
        }
}