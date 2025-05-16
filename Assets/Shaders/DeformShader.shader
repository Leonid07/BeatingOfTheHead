Shader "Custom/DeformShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BruiseTex ("Bruise Texture", 2D) = "white" {} // Текстура синяков
        _DeformPositions ("Deform Positions", Vector) = (0,0,0,0)
        _DeformPowers ("Deform Powers", Float) = 0.0
        _DeformRadius ("Deform Radius", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BruiseTex; // Добавляем текстуру синяков
            float4 _DeformPositions[10];
            float _DeformPowers[10];
            float _DeformRadius;
            int _DeformCount;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float3 totalDeform = 0;

                for(int i = 0; i < _DeformCount; i++)
                {
                    float3 offset = v.vertex.xyz - _DeformPositions[i].xyz;
                    float distance = length(offset);
                    if(distance < _DeformRadius)
                    {
                        float deform = (1 - distance / _DeformRadius) * _DeformPowers[i];
                        totalDeform -= normalize(offset) * deform;
                    }
                }

                v.vertex.xyz += totalDeform;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.uv);
                fixed4 bruiseColor = tex2D(_BruiseTex, i.uv);
                return lerp(mainColor, bruiseColor, bruiseColor.a); // Смешиваем по альфа-каналу
            }
            ENDCG
        }
    }
}