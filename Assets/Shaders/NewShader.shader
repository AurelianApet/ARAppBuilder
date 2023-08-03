Shader "Custom/NewShader"
{
	Properties 
	{  
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range(0,0.9)) = 0.5
	}
		
	SubShader 
	{ 
		Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
		CGPROGRAM
		#pragma surface surf Lambert alpha sampler2D _MainTex
			
			struct Input { float2 uv_MainTex; };
			
			sampler2D _MainTex;
			float4 _Color;
			
			float Epsilon = 1e-10;
 
			float3 RGBtoHCV(in float3 RGB)
			{
				// Based on work by Sam Hocevar and Emil Persson
				float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
				float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
				float C = Q.x - min(Q.w, Q.y);
				float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
				return float3(H, C, Q.x);
			}
        
        	float3 RGBtoHSL(in float3 RGB)
			{
				float3 HCV = RGBtoHCV(RGB);
				float L = HCV.z - HCV.y * 0.5;
				float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
				return float3(HCV.x, S, L);
			}
			
			float GetA(float a)
			{
				return a / 256.0f;
			}
			
			void surf (Input IN, inout SurfaceOutput o)
			{
				half4 c = tex2D (_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb;
				
				float3 hsl = RGBtoHSL(o.Albedo.xyz);
				
				if (hsl.x > GetA(180) && hsl.x < GetA(230))
				{					
					if (hsl.z >= GetA(60))
						o.Alpha = 0;
					else
						o.Alpha = 1;				
					
					if (c.r - c.g + c.b - c.g < 0.2f)
						o.Alpha = 1;
				}
				else
				{
					o.Alpha = 1;
				}
				
			}
		ENDCG
	} 
	FallBack "Diffuse"
} 	