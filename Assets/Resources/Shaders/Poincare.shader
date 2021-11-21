Shader "Custom/Poincare"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _P0 ("Origin", Vector) = (0.0, 0.0, 0.0)
        _Height ("Height of Hyperboloid", Float) = 1.0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            float3 _P0;
            float _Height;

            v2f vert (appdata v)
            {
                v2f o;
                float4x4 customModel;
                customModel = unity_ObjectToWorld;
                float4 Pe = mul(customModel, v.vertex);
                float3 P0 = float3(_P0.x, _P0.y - _Height, _P0.z);
                float interHeight = sqrt(pow(Pe.x, 2) + pow(Pe.z, 2) + 1);
                float3 Ps = float3(Pe.x, interHeight, Pe.z);  
                float3 V = Pe - P0;
                float t = P0.y/V.y;
                float pX = P0.x + (t * V.x);
                float pZ = P0.z + (t * V.z);
                float3 projectedP = float3(pX, 0.0, pZ);
                float4 resultVertex = mul(unity_WorldToObject, projectedP);
                o.vertex = UnityObjectToClipPos(resultVertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
