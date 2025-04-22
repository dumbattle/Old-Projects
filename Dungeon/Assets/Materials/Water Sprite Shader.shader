Shader "Unlit/Water Sprite Shader"
{
    Properties
    {

        _WaterCells("Water Cells", 2D) = "white" {}



        _ColorW("Water Color", Color) = (1,1,1,1)
        _ColorS("Shore Color", Color) = (1,1,1,1)

        _t("T", Float) = 0
        _tide("Tide", Float) = 0
        _shore("Shore size", Float) = 0
    }
        SubShader
        {

            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull back
            LOD 100

            Pass
            {
                CGPROGRAM
               #pragma vertex vert alpha
                #pragma fragment frag alpha


                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _WaterCells;
                float4 _WaterCells_TexelSize;
                fixed4 _ColorW;
                fixed4 _ColorS;
                float _t;
                float _tide;
                float _shore;


                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                    return o;
                }
                fixed4 frag(v2f i) : SV_Target
                {

                    fixed4 waterColCode;


                // grid size
                int w = _WaterCells_TexelSize.z;
                int h = _WaterCells_TexelSize.w;

                // which grid
                int gx = (int)(i.uv.x * (w));
                int gy = (int)(i.uv.y * (h));


                // is water
                int isWater = 0;
                waterColCode = tex2D(_WaterCells, float2((gx + .1) / (float)w, (gy + .1) / (float)h));
                isWater = waterColCode.a > 0 ? 1 : 0;

                if (isWater == 0) {
                    return fixed4(0, 0, 0, 0);
                }
                // get mode

                int x2;
                int y2;
                int top = 0;
                int bottom = 0;
                int right = 0;
                int left = 0;
                int top_right = 0;
                int top_left = 0;
                int bottom_right = 0;
                int bottom_left = 0;
                int inRange = 0;

                // check top
                x2 = gx;
                y2 = gy + 1;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                top = waterColCode.a > 0 ? 1 : 0;

                // check bottom
                x2 = gx;
                y2 = gy - 1;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                bottom = waterColCode.a > 0 ? 1 : 0;

                // check right
                x2 = gx + 1;
                y2 = gy;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                right = waterColCode.a > 0 ? 1 : 0;

                // check left
                x2 = gx - 1;
                y2 = gy;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                left = waterColCode.a > 0 ? 1 : 0;

                // check corners
                x2 = gx + 1;
                y2 = gy + 1;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                top_right = waterColCode.a > 0 ? 1 : 0;

                x2 = gx - 1;
                y2 = gy + 1;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                top_left = waterColCode.a > 0 ? 1 : 0;

                x2 = gx + 1;
                y2 = gy - 1;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                bottom_right = waterColCode.a > 0 ? 1 : 0;

                x2 = gx - 1;
                y2 = gy - 1;
                waterColCode = tex2D(_WaterCells, float2((x2 + .1) / w, (y2 + .1) / h));
                bottom_left = waterColCode.a > 0 ? 1 : 0;

                // center 2 square
                float cx = gx + .5 + (right - left) * .5;
                float cy = gy + .5 + (top - bottom) * .5;

                // size of squares
                float xx = .5 + right + left;
                float yy = .5 + top + bottom;

                // scaled uv
                float x = i.uv.x * w;
                float y = i.uv.y * h;

                //dist horizontal calculation
                float dhx = max(abs(x - cx) - xx / 2.0, 0);
                float dhy = max(abs(y - (gy + .5)) - .5 / 2.0, 0);

                float distH = sqrt(dhx * dhx + dhy * dhy);

                //dist vertical calculation
                float dvx = max(abs(x - (gx + .5)) - .5 / 2.0, 0);
                float dvy = max(abs(y - cy) - yy / 2.0, 0);

                float distV = sqrt(dvx * dvx + dvy * dvy);


                float distDromCenter = min(distV, distH);



                // get dist to each corner

                float tr = 0;
                float tl = 0;
                float br = 0;
                float bl = 0;

                x = x % 1;
                y = y % 1;

                tr = (x - 1) * (x - 1) + (y - 1) * (y - 1);
                tl = (x) * (x)+(y - 1) * (y - 1);
                br = (x - 1) * (x - 1) + (y) * (y);
                bl = (x) * (x)+(y) * (y);

                // closest corner
                int closest =
                    tr < tl
                        ? tr < br
                            ? tr < bl ? 1 : 4
                            : br < bl ? 3 : 4
                        : tl < br
                            ? tl < bl ? 2 : 4
                            : br < bl ? 3 : 4
                    ;

                // coords for corner
                float cornerX = (tr < bl&& tr < tl) || (br < bl&& tr < tl) ? 1 : 0;
                float cornerY = (tr < bl&& tr < br) || (tl < bl&& tl < br) ? 1 : 0;
                
                // dist to that corner
                float cDist = (cornerX - x) * (cornerX - x) + (cornerY - y) * (cornerY - y);
                float minCornerDist = .25 - _t;
                
                minCornerDist =
                    x > 1 - minCornerDist && (y > 1 - minCornerDist) ? closest == 1 && (top * right)    ? top_right    ? 0 : .25 - _t : 99 :
                    x < minCornerDist     && (y > 1 - minCornerDist) ? closest == 2 && (top * left)     ? top_left     ? 0 : .25 - _t : 99 :
                    x > 1 - minCornerDist && (y < minCornerDist)     ? closest == 3 && (bottom * right) ? bottom_right ? 0 : .25 - _t : 99 :
                    x < minCornerDist     && (y < minCornerDist)     ? closest == 4 && (bottom * left)  ? bottom_left  ? 0 : .25 - _t : 99 :
                    99;


                float t = (_SinTime[3] + 1) /2 * _tide;
                float cDist2 = cDist + t;
                float distDromCenter2 = distDromCenter - t;

                cDist = (cDist  + _shore > minCornerDist * minCornerDist  ? 1 : 0);
                distDromCenter = distDromCenter - _shore <= _t? 1 : 0;

                cDist2 = (cDist2 > minCornerDist * minCornerDist? 1 : 0);
                distDromCenter2 = distDromCenter2 <= _t ? 1 : 0;


                int ww = max(distDromCenter2, cDist2);
                int s = max(distDromCenter, cDist);
                fixed4 result = _ColorW * (ww) + _ColorS * (s && ! ww);
                result.a *= isWater;

                return result;
            }
            ENDCG
        }
    }
}
