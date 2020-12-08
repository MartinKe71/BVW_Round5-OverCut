Shader "Custom/TwoSided"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BloodTex("Blood (RGBA)", 2D) = "white" {}
        _BloodIntensity("Blood Intensity", Range(0, 1)) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _FillMax("Fill Maximum", Float) = 10
        _FillMin("Fill Minimum", Float) = -5
    }
    SubShader
    {
        Tags {"RenderType" = "AlphaCutout" }
        LOD 200
        Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alphatest:alphaOut addshadow fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BloodTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BloodTex;
            fixed facing : VFACE;
            float3 normal : NORMAL;
            float3 worldPos : POSITION;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _BloodIntensity;
        float _FillMax;
        float _FillMin;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed facing = IN.facing;
            if (facing < 0) {
                o.Normal = -o.Normal;
            }

            float3 objectPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1));

            fixed4 c = fixed4(0, 0, 0, 0);
            
            if (objectPos.x < _FillMax && objectPos.x > _FillMin)
            {
                c = tex2D(_MainTex, IN.uv_MainTex)* _Color;
            }
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            float alphaOut = c.a;
            clip(c.a - 0.5);  
        }
        ENDCG
    }
    FallBack "Diffuse"
}
