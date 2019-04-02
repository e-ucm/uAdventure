Shader "Hidden/TransitionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _TransitionTex("Transition Texture", 2D) = "black" {}
		_DirectionX("Direction X", Range(-1,1)) = 0
		_DirectionY("Direction Y", Range(-1,1)) = 0
		_Progress("Progress", Range(0,1)) = 0
		_Blend("Blend", Range(0,1)) = 0
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _TransitionTex;
			float _DirectionX;
			float _DirectionY;
			float _Progress;
			float _Blend;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 direction = float2(_DirectionX, _DirectionY);
				float2 movedUv = i.uv + direction * _Progress;
				float2 otherUv = float2((1 + movedUv.x) % 1, (1 + movedUv.y) % 1);

				fixed4 color1 = tex2D(_MainTex, movedUv);
				fixed4 color2 = tex2D(_TransitionTex, otherUv);
				fixed4 color = lerp(color1, color2, _Blend);

				if (movedUv.x < 0 || movedUv.x >= 1 || movedUv.y < 0 || movedUv.y >= 1) {
					color = color2;
				}

				return color;
			}
			ENDCG
		}
	}
}
