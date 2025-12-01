Shader "Custom/HeartSlice"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Fill("Fill", Range(0,1)) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Fill;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float angle01(float2 uv)
            {
                float2 c = uv - float2(0.5, 0.5);
                float a = atan2(c.y, c.x);
                a = (a + 3.14159265) / (2 * 3.14159265);
                return a;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float a = angle01(i.uv);

                if (a > _Fill) discard;

                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
