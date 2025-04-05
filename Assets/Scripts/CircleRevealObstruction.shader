Shader "Custom/CircleRevealObstruction"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeAlpha ("Fade Alpha", Range(0,1)) = 1
        _RevealRadius ("Reveal Radius", Float) = 2
        _RevealPosition ("Reveal Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        sampler2D _MainTex;
        float _FadeAlpha;
        float _RevealRadius;
        float4 _RevealPosition;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float dist = distance(IN.worldPos, _RevealPosition.xyz);

            float alpha = _FadeAlpha;
            if (dist < _RevealRadius)
            {
                // Smoothstep for soft circle edge
                alpha = lerp(1, _FadeAlpha, smoothstep(_RevealRadius, _RevealRadius - 0.5, dist));
            }

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = alpha;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
