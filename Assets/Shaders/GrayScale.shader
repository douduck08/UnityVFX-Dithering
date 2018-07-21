Shader "Hidden/GrayScale" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA

            #include "UnityCG.cginc"
            #include "Includes/Common.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            uniform float4 _MainTex_TexelSize;
            uniform float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = UnityStereoScreenSpaceUVAdjust(v.uv, _MainTex_ST);
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                UNITY_SETUP_INSTANCE_ID(i);
                float4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv);
                float3 luma = getLuma(col.rgb);
                return float4(luma, 1);
            }
            ENDCG
        }
    }
}
