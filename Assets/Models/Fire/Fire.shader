Shader "Unlit/Fire"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion Map", 2D) = "grey" {}
        _DistortionIntensity ("Distortion Intensity", Float) = 0.1
        _DistortionTimeScale ("Distortion Time Scale", Float) = 4
        _TextureSize ("Texture Size", Float) = 256
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off
        AlphaToMask On
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Distortion;
            float4 _Distortion_ST;
            float _DistortionIntensity;
            float _DistortionTimeScale;
            float _TextureSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the distortion map
                float2 distortionUV = i.uv;
                distortionUV.y -= floor(((_Time) * _DistortionTimeScale) * _TextureSize) / _TextureSize;
                //distortionUV
                float distorty = tex2D(_Distortion, distortionUV).r;


                //Sample main tex based on distortion, but only perform distortion on the y axis
                float2 mainTexUV = i.uv;
                mainTexUV.y += (distorty-0.5) * 2 * _DistortionIntensity;
                mainTexUV.x = round(mainTexUV.x * _TextureSize) / _TextureSize;
                mainTexUV.y = round(mainTexUV.y * _TextureSize) / _TextureSize;
                fixed4 col = tex2D(_MainTex, mainTexUV);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
