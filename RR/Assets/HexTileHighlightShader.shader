Shader "Unlit/HexTileHighlightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
			
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag alpha

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 color : COLOR;
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float x = (uv.x - .5);
				float y = (uv.y - .5);
				float dist = sqrt(x * x + y * y);

				float4 col = i.color;
				col.a = (dist * sqrt(3));
				return col;
			}
			ENDCG
		}
    }
}
