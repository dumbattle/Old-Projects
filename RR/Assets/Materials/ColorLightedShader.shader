// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/ColorLightedShader"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_LightDirection("LD", Vector) = (1,1,1,1)

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

            #include "UnityCG.cginc"

            struct appdata
            {
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Color;
			float3 _LightDirection;

            v2f vert (appdata v)
            {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
                fixed4 col = tex2D(_MainTex, i.uv);

				float dp = 0;

				dp += col.x * _LightDirection.x;
				dp += col.y * _LightDirection.y;
				dp += col.z * _LightDirection.z;
				dp = clamp(dp, .1, 1);

				float cameraDist = length(i.posWorld.xyz - _WorldSpaceCameraPos.xyz);
				cameraDist = clamp(cameraDist, 1, 500);
				cameraDist /= 500;
				float closeMod = .4;
				cameraDist = sqrt(cameraDist) * closeMod + 1 - closeMod;
				//return col;
				return _Color * dp  ;
            }
            ENDCG
        }
    }
}
