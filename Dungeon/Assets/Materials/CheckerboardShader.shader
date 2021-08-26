Shader "Custom/CheckerboardShader"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows
        #pragma target 3.0


        struct Input
        {
            float3 worldPos;
        };

        fixed4 _ColorA;
        fixed4 _ColorB;

        void surf (Input IN, inout  SurfaceOutput o)
        {
            float3 world = IN.worldPos;
            int mode = floor(world.x) + floor(world.z);

            int a = (uint)(abs(mode)) % 2; 
            a = abs(a); 
            int b = 1 - a;

            fixed4 c = _ColorA * a + _ColorB * b;
            o.Albedo = c.rgb;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
