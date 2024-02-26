Shader "Unlit/TransitionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius("_Radius", float) = 0
        _Horizontal("_Horizontal", float) = 0
        _Vertical("_Vertical", float) = 0
        _RadiusSpeed("_RadiusSpeed",float) = 1
        _PosX("_PosX",float) = 0
        _PosY("_PosY",float) = 0
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
            float _Radius;
            float _Horizontal;
            float _Vertical;
            float _RadiusSpeed;
            float _PosX;
            float _PosY;

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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);

                /*
                i.uv returns the position of the pixel
                TopLeft corner = (0,0)
                BottomRight corner = (1,1)
                */
                float3 pos = float3((i.uv.x-_PosX) / _Vertical, 
                                    (i.uv.y-_PosY) / _Horizontal, 0);

                return length(pos) > _Radius/_RadiusSpeed ? fixed4(0,0,0,0) : col;
            }
            ENDCG
        }
    }
}
