Shader "Unity Shaders Book/Chapter 12/Motion Blur"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurAmount ("Blur Amount", Float) = 1.0
	}
	SubShader
	{
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex; // 表示当前渲染结果
		fixed _BlurAmount; // 实际上表示当前渲染结果的权重

		struct v2f
		{
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f vert(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		fixed4 fragRGB(v2f i) : SV_Target
		{
			return fixed4(tex2D(_MainTex, i.uv).rgb, _BlurAmount);
		}

		half4 fragA(v2f i) : SV_Target
		{
			return tex2D(_MainTex, i.uv);
		}
		ENDCG

		ZTest Always
		Cull Off // ???
		ZWrite Off // 不需要写入, 每个像素只会执行一次

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha // dest 是 accumulationTexture
			ColorMask RGB
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragRGB
			ENDCG
		}

		Pass
		{
			// accumulationTexture 会被输出到屏幕上,
			// 而上一个 Pass 无法得到正确的深度,
			// 所以需要这个 Pass 来正确设置深度
			Blend One Zero
			ColorMask A
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragA
			ENDCG
		}
	}
	Fallback Off
}
