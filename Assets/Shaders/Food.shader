// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Food"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (1,1,1,1)
        [HDR]_SurfaceColor("Liquid Surface Color", Color) = (1,1,1,1)
        _GlassColor("Glass Color", Color) = (1,1,1,1)
        _GlassTransparency("Glass Transparency", Float) = 0.1
        //_SubColor ("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BloodTex("Blood (RGBA)", 2D) = "white" {}
        _BloodIntensity("Blood Intensity", Range(0, 1)) = 0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _FillMax("Fill Maximum", Float) = 20
        _FillMin("Fill Minimum", Float) = -5
        _RimIntensity("Rim Intensity", Float) = 3
    }
        SubShader
        {
            Tags { "RenderType" = "AlphaCutout"}
            //LOD 200
            Zwrite On
            Cull Back // we want the front and back faces
            //AlphaToMask On
            //Blend One OneMinusSrcAlpha

            Pass
            {
                Name "LiquidBackFace"
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Front
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct v2f {
                        float4 pos          : POSITION;
                        float4 screenPos    : TEXCOORD0;
                        float4 worldPos     : TEXCOORD1;
                    };

                    sampler2D _CameraDepthTexture;
                    float _FillMax;
                    float _FillMin;
                    float3 _SurfaceColor;

                    v2f vert(appdata_full v)
                    {
                        v2f o;
                        o.pos = UnityObjectToClipPos(v.vertex);
                        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                        o.screenPos = ComputeScreenPos(o.pos);
                        return o;
                    }



                    half4 frag(v2f i) : COLOR
                    {
                        // Try get the pivot point of the model
                        float4 worldBase = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0));
                        // Get the width
                        float dist = (i.worldPos - worldBase).x;
                        // If the width in within the color showing range, show the color
                        if (dist < _FillMax && dist > _FillMin) {
                            return float4(_SurfaceColor,1.0);
                        }
                        clip(-1);
                        //float depth = 1 - saturate(_WaterDepth - (LinearEyeDepth(tex2D(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w).r) - i.screenPos.z));
                        return float4(1.0,1.0,1.0,0.0);
                    }
                    ENDCG
            }
            CGPROGRAM
            #pragma surface surf Standard alphatest:outAlpha addshadow fullforwardshadows
            #pragma target 3.0

            sampler2D _MainTex;
            sampler2D _BloodTex;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_BloodTex;
                float2 uv_BumpMap;
                float3 viewDir;
                float4 vertex : POSITION;
                float3 worldPos : POSITION;
                float3 normal : NORMAL;
                fixed facing : VFACE;
            };

            half _Glossiness;
            half _Metallic;
            fixed4 _Color;
            float _FillMax;
            float _FillMin;
            float3 _GlassColor;
            float _GlassTransparency;
            float _RimIntensity;
            float _BloodIntensity;
            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                fixed facing = IN.facing;
                float viewDotNormal = clamp(dot(normalize(IN.viewDir), o.Normal), 0, 1);
                half rim = 0.0;
                if (facing > 0) {
                    rim = 1.0 - viewDotNormal;
                }
                float3 objectPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0,0,0,1));


                if (objectPos.x < _FillMax && objectPos.x > _FillMin) {
                    o.Alpha = 1.0;
                    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                    fixed4 b = tex2D(_BloodTex, IN.uv_BloodTex);
                    o.Albedo = lerp(c, b, b.a * _BloodIntensity);
                }
                else {
                    o.Albedo = _GlassColor;
                    o.Alpha = -1;
                    o.Emission = 0;
                }
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                float outAlpha = o.Alpha;

            }
            ENDCG
        }
            FallBack "Diffuse"
}
