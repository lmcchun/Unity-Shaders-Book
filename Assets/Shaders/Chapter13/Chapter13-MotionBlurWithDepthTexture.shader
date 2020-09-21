Shader "Unity Shaders Book/Chapter 13/Motion Blur With Depth Texture"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurSize ("Blur Size", Float) = 1.0
	}
	SubShader
	{
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex; // 表示当前渲染结果
		half4 _MainTex_TexelSize; // 对深度纹理的采样坐标进行平台差异化处理

		sampler2D _CameraDepthTexture;

		float4x4 _CurrentViewProjectionInverseMatrix;
		float4x4 _PreviousViewProjectionMatrix;

		fixed _BlurSize; // 实际上表示当前渲染结果的权重

		struct v2f
		{
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half2 uv_depth : TEXCOORD1;
		};

		v2f vert(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.uv_depth = v.texcoord;
			#if UNITY_UV_STARTS_AT_TOP // 表示 DirectX-Like 平台
			if (_MainTex_TexelSize.y < 0.0)
			{
				// ??? Unity 没有对屏幕图像纹理进行翻转
				o.uv_depth.y = 1.0 - o.uv_depth.y;
			}
			#endif
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			// Get the depth buffer value at this pixel.
			float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth);
			// H is the viewport position at this pixel in the range -1 to 1.
			float4 H = float4(i.uv.x * 2.0 - 1.0, i.uv.y * 2.0 - 1.0, d * 2.0 - 1.0, 1.0);
			// Transform by the view-projection inverse.
			float4 D = mul(_CurrentViewProjectionInverseMatrix, H);
			// Divide by w to get the world position.
			float4 worldPos = D / D.w;

			// Current viewport position
			float4 currentPos = H;
			// Use the world position, and transform by the previous view-projection matrix.
			float4 previousPos = mul(_PreviousViewProjectionMatrix, worldPos);
			// Convert to nonhomogeneous points [-1, 1] by dividing by w.
			previousPos /= previousPos.w;

			// Use this frame's position and last frame'sto compute the pixel velocity.
			float2 velocity = (currentPos.xy - previousPos.xy) / 2.0;

			float2 uv = i.uv;
			float4 c = tex2D(_MainTex, uv);
			uv += velocity * _BlurSize;
			for (int it = 1; it < 3; ++it, uv += velocity * _BlurSize)
			{
				float4 currentColor = tex2D(_MainTex, uv);
				c += currentColor;
			}
			c /= 3.0;

			return fixed4(c.rgb, 1.0);
		}

		ENDCG

		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	Fallback Off
}
