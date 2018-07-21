Shader "Custom/DitheredOpacity" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        #include "UnityCG.cginc"
        #include "Includes/Dither.cginc"

        struct Input {
            float2 uv_MainTex;
            UNITY_VPOS_TYPE screenPos : VPOS;
        };

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o) {
            clipBayerDither (IN.screenPos.xy / IN.screenPos.w, _Color.a);
            // clipFSDither (IN.screenPos.xy / IN.screenPos.w, _Color.a);

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
