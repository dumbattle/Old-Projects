Shader "Unlit/HexTerraceShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_LightDirection("Light Direction", Vector) = (1,1,1,0)
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"


			struct appdata
			{
				float4 normal : NORMAL;
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 normal : NORMAL;
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.normal = v.normal;
				return o;
			}

			float4 _Color;
			float4 _LightDirection;

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col;

				if (i.normal.x == 0 && i.normal.y == 1 && i.normal.z == 0) {
					if (i.worldPos.y > 7) {						
						col =  fixed4(1, 1, 1, 0);
					}
					else if(i.worldPos.y > 4) {
						col = fixed4(159.0 / 255.0, 103.0 / 255.0, 55.0 / 255.0, 1) * ((i.worldPos.y + 3 )/ 10);
					}
					else if(i.worldPos.y >= 0) {
						col = fixed4(0, (i.worldPos.y +8)/ 12,0,1);
					}
				}

				_LightDirection = normalize(_LightDirection);

				float dot = i.normal.x * _LightDirection.x;
				dot += i.normal.y * _LightDirection.y;
				dot += i.normal.z * _LightDirection.z;
				dot = clamp(dot, .5, 1);

				return col * dot ;
			}
			ENDCG
		}
    }
}
