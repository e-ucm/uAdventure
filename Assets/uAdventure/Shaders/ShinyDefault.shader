// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 //Kaan Yamanyar,Levent Seckin
 Shader "Sprites/ShinyDefault"
 {
     Properties
     {
         [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		 [PerRendererData] _TransitionTex("Transition Texture", 2D) = "black" {}
		 [PerRendererData] _TransitionSet("Second Texture Set", Int) = 0
         _Color("Tint", Color) = (1,1,1,1)
	     _DirectionX("Direction X", Range(-1,1)) = 0
	     _DirectionY("Direction Y", Range(-1,1)) = 0
		 _Progress("Progress", Range(0,1)) = 0
		 _Blend("Blend", Range(0,1)) = 0
         _ShineLocation("ShineLocation", Range(0,1)) = 0
         _ShineWidth("ShineWidth", Range(0,1)) = 0
         [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
     }
 
         SubShader
     {
         Tags
     {
         "Queue" = "Transparent"
         "IgnoreProjector" = "True"
         "RenderType" = "Transparent"
         "PreviewType" = "Plane"
         "CanUseSpriteAtlas" = "True"
     }
 
         Cull Off
         Lighting Off
         ZWrite Off
         Blend One OneMinusSrcAlpha
 
         Pass
     {
         CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile _ PIXELSNAP_ON
 #include "UnityCG.cginc"
 
     struct appdata_t
     {
         float4 vertex   : POSITION;
         float4 color    : COLOR;
         float2 texcoord : TEXCOORD0;
     };
 
     struct v2f
     {
         float4 vertex   : SV_POSITION;
         fixed4 color : COLOR;
         float2 texcoord  : TEXCOORD0;
     };
 
     fixed4 _Color;
 
     v2f vert(appdata_t IN)
     {
         v2f OUT;
         OUT.vertex = UnityObjectToClipPos(IN.vertex);
         OUT.texcoord = IN.texcoord;
         OUT.color = IN.color * _Color;
 #ifdef PIXELSNAP_ON
         OUT.vertex = UnityPixelSnap(OUT.vertex);
 #endif
 
         return OUT;
     }
 
     sampler2D _MainTex;
	 sampler2D _TransitionTex;
	 int _TransitionSet;
     sampler2D _AlphaTex;
	 float _DirectionX;
	 float _DirectionY;
	 float _Progress;
	 float _Blend;
     float _AlphaSplitEnabled;
     float _ShineLocation;
     float _ShineWidth;
 
     fixed4 SampleSpriteTexture(float2 uv)
     {
		 float2 direction = float2(_DirectionX, _DirectionY);
		 float2 movedUv = uv + direction * _Progress;
		 float2 otherUv = float2((1 + movedUv.x) % 1, (1 + movedUv.y) % 1);

         fixed4 color1 = tex2D(_MainTex, movedUv);
		 fixed4 color2 = tex2D(_TransitionTex, otherUv);
 
 #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
		 if (_AlphaSplitEnabled) {
			 color1.a = tex2D(_AlphaTex, movedUv).r;
			 color2.a = tex2D(_AlphaTex, otherUv).r;
		 }
 #endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

		 fixed4 color = lerp(color1, color2, _Blend);
		 if (_TransitionSet == 0) {
			 color = fixed4(color1.x, color1.y, color1.z, color1.w - _Blend);
		 }

		 if (movedUv.x < 0 || movedUv.x >= 1 || movedUv.y < 0 || movedUv.y >= 1) {
			 color = color2; 
		 }

		 float lowLevel = _ShineLocation - _ShineWidth;
		 float highLevel = _ShineLocation + _ShineWidth;
		 float currentDistanceProjection = (uv.x + uv.y) / 2;
		 if (currentDistanceProjection > lowLevel && currentDistanceProjection < highLevel) {
			 float whitePower = 1 - (abs(currentDistanceProjection - _ShineLocation) / _ShineWidth);
			 color.rgb += color.a * whitePower;
		 }
         
         return color;
     }
 
     fixed4 frag(v2f IN) : SV_Target
     {
         fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
         c.rgb *= c.a;
 
     return c;
     }
         ENDCG
     }
     }
 }