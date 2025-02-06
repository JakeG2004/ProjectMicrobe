// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "Landon/Distortion" {
Properties {
	[PowerSlider(2.0)]_BumpAmt  ("Distortion", range (-256,256)) = 3
	_Mask ("Mask", 2D) = "black" {}
}

Category {

	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent" "RenderType"="Opaque" }
	ZWrite Off
	Cull Off
	
	SubShader {

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		GrabPass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
		}
		
		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			
CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
};

struct v2f {
	float4 vertex : SV_POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvmask : TEXCOORD1;
	UNITY_FOG_COORDS(3)
};

float _BumpAmt;
float4 _BumpMap_ST;
float4 _Mask_ST;


v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	o.uvmask = TRANSFORM_TEX( v.texcoord, _Mask );
	UNITY_TRANSFER_FOG(o,o.vertex);
	return o;
}

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;

			sampler2D _Mask;
			
			float2 UV_TPR(float2 u, float4 t, float4 p, float4 r)			
			{
				float2 v = float2((r.x-((u.x-r.x)*cos((r.z*_Time.y)+(3.14159265359f*(1+(r.w/180))))-(u.y-r.y)*sin((r.z*_Time.y)+(3.14159265359f*(1+(r.w/180))))))+(p.x*_Time.y),(r.y-((u.x-r.x)*sin((r.z*_Time.y)+(3.14159265359f*(1+(r.w/180))))+(u.y-r.y)*cos((r.z*_Time.y)+(3.14159265359f*(1+(r.w/180))))))+(p.y*_Time.y));
				v = float2(v.x/t.x+floor(_Time.y*t.z+t.w)/t.x,(v.y+t.y-1)/t.y-floor(floor(_Time.y*t.z+t.w)/t.x)/t.y);
				return  v;
			}

half4 frag (v2f i) : SV_Target{
	
	half4 mask = tex2D(_Mask, i.uvmask);

	//half3 flatBump = UnpackNormal(half4(.5,.5,1,1));
	//half3 distortBump =UnpackNormal(half4(0,1,0,1));
	//half3 bump = lerp(flatBump, distortBump, mask.rgb);

	half2 flat = half2(0,0);
	half2 angle = half2(0,1);
	half2 bump = lerp(flat,angle,mask.r);

	float2 offset = bump.rg * _BumpAmt * _BumpAmt * _GrabTexture_TexelSize.xy;
	
	i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
		
	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}
ENDCG
		}
	}


}

}
