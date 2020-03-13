// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_chromas_shiftingspeed("chromas_shiftingspeed", Float) = 4
		_glitch("glitch", 2D) = "white" {}
		_chromapropability("chromapropability", Range( 0 , 1)) = 0
		_pub("pub", 2D) = "white" {}
		_LCD("LCD", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 4.6
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _pub;
		uniform float _chromapropability;
		uniform sampler2D _glitch;
		uniform float _chromas_shiftingspeed;
		uniform sampler2D _LCD;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutput o )
		{
			float temp_output_14_0 = ( 1.0 - _chromapropability );
			float mulTime3 = _Time.y * 5.0;
			float2 temp_cast_0 = (mulTime3).xx;
			float simplePerlin2D4 = snoise( temp_cast_0 );
			simplePerlin2D4 = simplePerlin2D4*0.5 + 0.5;
			float2 appendResult22 = (float2(step( temp_output_14_0 , sin( simplePerlin2D4 ) ) , step( temp_output_14_0 , sin( ( ( simplePerlin2D4 * 0.71 ) + 0.32 ) ) )));
			float mulTime2 = _Time.y * _chromas_shiftingspeed;
			float2 appendResult10 = (float2(0.0 , ( floor( mulTime2 ) * 0.28 )));
			float2 varchroma_UV24 = ( float2( 1,1 ) * -0.01 * appendResult22 * (-1.0 + (tex2D( _glitch, (i.uv_texcoord*1.0 + appendResult10) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) );
			float4 appendResult38 = (float4(tex2D( _pub, ( i.uv_texcoord + varchroma_UV24 ) ).r , tex2D( _pub, ( i.uv_texcoord + ( varchroma_UV24 * float2( 2,2 ) ) ) ).g , tex2D( _pub, ( i.uv_texcoord + ( varchroma_UV24 * float2( 4,4 ) ) ) ).b , 0.0));
			float4 break43 = ( tex2D( _pub, i.uv_texcoord ) + appendResult38 );
			float4 tex2DNode47 = tex2D( _LCD, ( i.uv_texcoord * float2( 100,100 ) ) );
			float4 appendResult51 = (float4(( break43.r * tex2DNode47.r ) , ( break43.g * tex2DNode47.g ) , ( break43.b * tex2DNode47.b ) , break43.a));
			float4 clampResult56 = clamp( ( appendResult51 * 1.5 ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			o.Emission = clampResult56.xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
7;127;1507;884;6417.897;3934.065;8.162013;True;True
Node;AmplifyShaderEditor.RangedFloatNode;1;-3069.782,480.4317;Inherit;False;Property;_chromas_shiftingspeed;chromas_shiftingspeed;0;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-2504.521,-79.8073;Inherit;False;1;0;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;2;-2707.528,459.0156;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;5;-2437.529,457.0156;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;4;-2161.495,-65.6523;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1815.252,-8.117193;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.71;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-2255.529,456.0156;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.28;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-2209.451,268.9558;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;10;-1979.528,493.0156;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;-1548.623,4.002702;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1881.701,-282.5883;Inherit;False;Property;_chromapropability;chromapropability;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;13;-1762.528,317.0156;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;14;-1334.781,-204.8852;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;15;-1724.372,-122.5012;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;12;-1352.287,13.69881;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;16;-1064.988,-273.4533;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;-1448.079,317.3286;Inherit;True;Property;_glitch;glitch;1;0;Create;True;0;0;False;0;48a1ffa5cf1870e42bcb3b67d849e0ae;48a1ffa5cf1870e42bcb3b67d849e0ae;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;18;-1060.287,-26.09029;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;21;-996.1198,300.7866;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-783.3859,-417.7531;Inherit;True;Constant;_Float0;Float 0;20;0;Create;True;0;0;False;0;-0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-663.2858,-135.6543;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;19;-127.3567,-710.5926;Inherit;False;Constant;_Vector2;Vector 2;20;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;586.7826,-290.7006;Inherit;False;4;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;931.5775,-271.9532;Inherit;False;varchroma_UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-1525.627,-1476.539;Inherit;False;24;varchroma_UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1049.386,-996.6918;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;4,4;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1102.044,-1375.289;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-1389.576,-1899.382;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;30;-1188.668,-2134.377;Inherit;True;Property;_pub;pub;3;0;Create;True;0;0;False;0;84508b93f15f2b64386ec07486afc7a3;98bcf1f0c4a490b4585e3aad8cc78137;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-708.377,-1689.428;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;32;-767.9956,-1414.863;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-801.8826,-1106.967;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;35;747.8699,-1619.085;Inherit;True;Property;_TextureSample6;Texture Sample 6;21;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;769.3889,-1309.49;Inherit;True;Property;_TextureSample0;Texture Sample 0;21;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-714.6272,-1873.561;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;791.271,-1002.836;Inherit;True;Property;_TextureSample7;Texture Sample 7;20;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;38;1124.698,-1569.666;Inherit;True;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;46;1929.345,-2108.141;Inherit;False;Constant;_Vector0;Vector 0;5;0;Create;True;0;0;False;0;100,100;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;44;1626.815,-2261.393;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;795.4858,-2008.75;Inherit;True;Property;_TextureSample2;Texture Sample 2;21;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;2436.949,-2375.921;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;1338.867,-2001.414;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;47;2664.602,-2372.319;Inherit;True;Property;_LCD;LCD;4;0;Create;True;0;0;False;0;8307a42eb19660a49b06075a9d60fcc0;8307a42eb19660a49b06075a9d60fcc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;43;2383.302,-2811.172;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;3130.799,-2684.997;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;3113.4,-2424.212;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;3103.239,-2154.959;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;3746.735,-2388.65;Inherit;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;False;0;1.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;51;3557.073,-2624.034;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;3977.795,-2564.959;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;56;4384.32,-2588.836;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;4573.975,-1331.4;Float;False;True;6;ASEMaterialInspector;0;0;Lambert;New Amplify Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;0
WireConnection;5;0;2;0
WireConnection;4;0;3;0
WireConnection;7;0;4;0
WireConnection;6;0;5;0
WireConnection;10;1;6;0
WireConnection;8;0;7;0
WireConnection;13;0;11;0
WireConnection;13;2;10;0
WireConnection;14;0;9;0
WireConnection;15;0;4;0
WireConnection;12;0;8;0
WireConnection;16;0;14;0
WireConnection;16;1;15;0
WireConnection;17;1;13;0
WireConnection;18;0;14;0
WireConnection;18;1;12;0
WireConnection;21;0;17;1
WireConnection;22;0;16;0
WireConnection;22;1;18;0
WireConnection;23;0;19;0
WireConnection;23;1;20;0
WireConnection;23;2;22;0
WireConnection;23;3;21;0
WireConnection;24;0;23;0
WireConnection;27;0;25;0
WireConnection;26;0;25;0
WireConnection;31;0;28;0
WireConnection;31;1;25;0
WireConnection;32;0;28;0
WireConnection;32;1;26;0
WireConnection;29;0;28;0
WireConnection;29;1;27;0
WireConnection;35;0;30;0
WireConnection;35;1;31;0
WireConnection;34;0;30;0
WireConnection;34;1;32;0
WireConnection;36;0;30;0
WireConnection;36;1;29;0
WireConnection;38;0;35;1
WireConnection;38;1;34;2
WireConnection;38;2;36;3
WireConnection;37;0;30;0
WireConnection;37;1;33;0
WireConnection;45;0;44;0
WireConnection;45;1;46;0
WireConnection;39;0;37;0
WireConnection;39;1;38;0
WireConnection;47;1;45;0
WireConnection;43;0;39;0
WireConnection;48;0;43;0
WireConnection;48;1;47;1
WireConnection;49;0;43;1
WireConnection;49;1;47;2
WireConnection;50;0;43;2
WireConnection;50;1;47;3
WireConnection;51;0;48;0
WireConnection;51;1;49;0
WireConnection;51;2;50;0
WireConnection;51;3;43;3
WireConnection;53;0;51;0
WireConnection;53;1;52;0
WireConnection;56;0;53;0
WireConnection;0;2;56;0
ASEEND*/
//CHKSM=820E6D1771F233A7A6A80A105BA997AA5C996792