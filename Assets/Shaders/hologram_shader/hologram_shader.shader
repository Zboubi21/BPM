// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Community/TFHC/Force Shield"
{
	Properties
	{
		_Normal("Normal", 2D) = "bump" {}
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		_lines("lines", 2D) = "white" {}
		_AnimSpeed(" Anim Speed", Range( -10 , 10)) = 3
		_Vglitch("V!glitch", Range( 0 , 1)) = 0
		_VGlitch_speed("VGlitch_speed", Range( 0 , 10)) = 0
		_VGlitch_tiling("VGlitch_tiling", Range( 0 , 10)) = 0
		_VGlitch_amount("VGlitch_amount", Range( 0 , 1)) = 0
		_vertexoffsetscale("vertexoffsetscale", Range( 0 , 1)) = 0.1
		_line_color("line_color", Color) = (0,0,0,0)
		_fresnel_color("fresnel_color", Color) = (0,0,0,0)
		_chromas_shiftingspeed("chromas_shiftingspeed", Float) = 4
		_glitch("glitch", 2D) = "white" {}
		_chromapropability("chromapropability", Range( 0 , 1)) = 0
		_Texture("Texture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _VGlitch_tiling;
		uniform float _VGlitch_speed;
		uniform float _VGlitch_amount;
		uniform float _Vglitch;
		uniform float _vertexoffsetscale;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Texture;
		uniform float _chromapropability;
		uniform sampler2D _glitch;
		uniform float4 _glitch_ST;
		uniform float _chromas_shiftingspeed;
		uniform sampler2D _lines;
		uniform float _AnimSpeed;
		uniform float4 _line_color;
		uniform float4 _fresnel_color;
		uniform float _Opacity;


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 temp_cast_0 = (_VGlitch_tiling).xx;
			float mulTime299 = _Time.y * _VGlitch_speed;
			float2 temp_cast_1 = (floor( mulTime299 )).xx;
			float2 uv_TexCoord303 = v.texcoord.xy * temp_cast_0 + temp_cast_1;
			float simplePerlin2D304 = snoise( uv_TexCoord303 );
			simplePerlin2D304 = simplePerlin2D304*0.5 + 0.5;
			float temp_output_295_0 = ( ( _Vglitch * 2.0 ) - 1.0 );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime286 = _Time.y * -1.0;
			float2 temp_cast_2 = (sin( ( ( ase_worldPos.y + mulTime286 ) * 2.0 ) )).xx;
			float simplePerlin2D290 = snoise( temp_cast_2 );
			simplePerlin2D290 = simplePerlin2D290*0.5 + 0.5;
			float smoothstepResult291 = smoothstep( temp_output_295_0 , ( temp_output_295_0 + 0.1 ) , simplePerlin2D290);
			float4 appendResult313 = (float4(( max( step( 0.0 , ( simplePerlin2D304 - ( 1.0 - _VGlitch_amount ) ) ) , 0.0 ) * abs( smoothstepResult291 ) ) , 0.0 , 0.0 , 0.0));
			float3 viewToObjDir315 = mul( UNITY_MATRIX_T_MV, float4( appendResult313.xyz, 0 ) ).xyz;
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float3 worldToViewDir319 = mul( UNITY_MATRIX_V, float4( ase_worldNormal, 0 ) ).xyz;
			float dotResult322 = dot( worldToViewDir319 , float3(1,0,0) );
			float smoothstepResult324 = smoothstep( 0.5 , 0.7 , saturate( dotResult322 ));
			float3 varvertexoffset325 = ( viewToObjDir315 * _vertexoffsetscale * smoothstepResult324 );
			v.vertex.xyz += varvertexoffset325;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 Normal282 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			o.Normal = Normal282;
			float temp_output_361_0 = ( 1.0 - _chromapropability );
			float mulTime353 = _Time.y * 5.0;
			float2 temp_cast_0 = (mulTime353).xx;
			float simplePerlin2D354 = snoise( temp_cast_0 );
			simplePerlin2D354 = simplePerlin2D354*0.5 + 0.5;
			float2 appendResult363 = (float2(step( temp_output_361_0 , sin( simplePerlin2D354 ) ) , step( temp_output_361_0 , sin( ( ( simplePerlin2D354 * 0.71 ) + 0.32 ) ) )));
			float2 uv0_glitch = i.uv_texcoord * _glitch_ST.xy + _glitch_ST.zw;
			float mulTime342 = _Time.y * _chromas_shiftingspeed;
			float2 appendResult346 = (float2(0.0 , ( floor( mulTime342 ) * 0.28 )));
			float2 varchroma_UV367 = ( float2( 1,1 ) * -0.01 * appendResult363 * (-1.0 + (tex2D( _glitch, (uv0_glitch*1.0 + appendResult346) ).r - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) );
			float4 appendResult380 = (float4(tex2D( _Texture, ( i.uv_texcoord + varchroma_UV367 ) ).r , tex2D( _Texture, ( i.uv_texcoord + ( varchroma_UV367 * float2( 2,2 ) ) ) ).g , tex2D( _Texture, ( i.uv_texcoord + ( varchroma_UV367 * float2( 4,4 ) ) ) ).b , 0.0));
			float4 final_sprite390 = ( tex2D( _Texture, i.uv_texcoord ) + appendResult380 );
			o.Albedo = final_sprite390.rgb;
			float3 ase_worldPos = i.worldPos;
			float4 ShieldSpeed84 = ( _Time * _AnimSpeed );
			float2 appendResult46 = (float2(float2( 1,0 ).x , ShieldSpeed84.x));
			float4 ShieldPattern17 = tex2D( _lines, ( ase_worldPos.y + appendResult46 ) );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV335 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode335 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV335, 5.0 ) );
			float4 Varemissive339 = ( ( ShieldPattern17 * _line_color ) + ( fresnelNode335 * _fresnel_color ) );
			o.Emission = Varemissive339.rgb;
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17101
7;73;1507;938;2994.313;2012.769;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;341;-5042.87,3180.441;Inherit;False;Property;_chromas_shiftingspeed;chromas_shiftingspeed;11;0;Create;True;0;0;False;0;4;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;342;-4680.616,3159.025;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;353;-4477.609,2620.202;Inherit;False;1;0;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;354;-4134.583,2634.357;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;343;-4410.617,3157.025;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;344;-4228.617,3156.025;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.28;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;355;-3788.34,2691.892;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.71;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;346;-3952.616,3193.025;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;360;-3854.789,2417.421;Inherit;False;Property;_chromapropability;chromapropability;13;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;347;-4182.539,2968.965;Inherit;False;0;350;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;356;-3521.711,2704.012;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;298;-5471.864,-1875.576;Inherit;False;Property;_VGlitch_speed;VGlitch_speed;5;0;Create;True;0;0;False;0;0;7.43;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;299;-5140.154,-1720.716;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;357;-3325.375,2713.708;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;348;-3735.616,3017.025;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;285;-5640.363,-1362.172;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;267;-3897.343,-2192.569;Inherit;False;830.728;358.1541;Comment;4;35;34;36;84;Animation Speed;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;361;-3307.869,2495.124;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;286;-5964.406,-952.9674;Inherit;False;1;0;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;358;-3697.46,2577.508;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;350;-3421.167,3017.338;Inherit;True;Property;_glitch;glitch;12;0;Create;True;0;0;False;0;48a1ffa5cf1870e42bcb3b67d849e0ae;42b47aae2aac2a24290231855dece289;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;34;-3840.751,-2152.57;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;-3847.343,-1949.416;Float;False;Property;_AnimSpeed; Anim Speed;3;0;Create;True;0;0;False;0;3;3.8;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;302;-5462.01,-2046.032;Inherit;False;Property;_VGlitch_tiling;VGlitch_tiling;6;0;Create;True;0;0;False;0;0;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;359;-3033.375,2673.919;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;362;-3038.076,2426.556;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;300;-4929.154,-1728.716;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;293;-5497.933,-621.9135;Inherit;False;Property;_Vglitch;V!glitch;4;0;Create;True;0;0;False;0;0;0.676;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;287;-5252.282,-1038.202;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;288;-5023.788,-1080.851;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-3531.215,-2019.157;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;366;-2503.019,2231.827;Inherit;False;Constant;_Vector2;Vector 2;20;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;365;-2584.474,2445.256;Inherit;False;Constant;_Float0;Float 0;20;0;Create;True;0;0;False;0;-0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;352;-2969.208,3000.796;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;363;-2636.374,2564.355;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;305;-4925.343,-1608.956;Inherit;False;Property;_VGlitch_amount;VGlitch_amount;7;0;Create;True;0;0;False;0;0;0.477;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;294;-5050.899,-644.2884;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;303;-4878.154,-1924.716;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;306;-4575.264,-1628.82;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;295;-4750.809,-657.3315;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;289;-4722.953,-1180.963;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;364;-2299.673,2491.555;Inherit;True;4;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;304;-4624.154,-1898.716;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;-3314.615,-2008.021;Float;False;ShieldSpeed;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;290;-4386.973,-1268.911;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;276;-2836.338,-1925.072;Inherit;False;1504.24;684.7161;Comment;5;46;1;17;330;331;Shield Main Pattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;44;-3061.699,-1376.665;Float;False;Constant;_Vector0;Vector 0;6;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;85;-3008.542,-1523.443;Inherit;False;84;ShieldSpeed;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;296;-4363.775,-609.3339;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;307;-4384.264,-1734.82;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;367;-1998.166,2462.686;Inherit;False;varchroma_UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;-2581.665,-1364.085;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;330;-2676.676,-1663.891;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StepOpNode;308;-4086.264,-1648.82;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;383;-4029.092,947.7144;Inherit;False;367;varchroma_UV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;291;-4082.792,-830.3151;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;318;-3439.793,-677.0265;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;-3605.508,1048.964;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;2,2;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;388;-3552.85,1427.562;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;4,4;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TransformDirectionNode;319;-3141.393,-673.0265;Inherit;False;World;View;False;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;376;-3893.041,524.8715;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;331;-2252.567,-1706.639;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector3Node;321;-3149.187,-500.605;Inherit;False;Constant;_Vector1;Vector 1;22;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMaxOpNode;309;-3835.264,-1632.82;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;297;-3792.664,-1053.655;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;322;-2810.187,-613.605;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;382;-3271.46,1009.39;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;378;-3372.615,655.6035;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1940.885,-1493.537;Inherit;True;Property;_lines;lines;2;0;Create;True;0;0;False;0;None;017af206dac594d4496e2f0f3067d73b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;386;-3305.347,1317.286;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;310;-3561.604,-1385.974;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;369;-3095.682,364.3069;Inherit;True;Property;_Texture;Texture;14;0;Create;True;0;0;False;0;None;b1acf8ea0bccac64c832e71652eb3f2d;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;370;-2989.145,617.1179;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1560.685,-1491.737;Float;False;ShieldPattern;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;381;-2649.042,1102.777;Inherit;True;Property;_TextureSample3;Texture Sample 3;21;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;377;-2670.561,793.1819;Inherit;True;Property;_TextureSample2;Texture Sample 2;21;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;323;-2591.187,-592.605;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;389;-2627.16,1409.431;Inherit;True;Property;_TextureSample4;Texture Sample 4;20;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;313;-3219.689,-1108.016;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TransformDirectionNode;315;-2969.351,-1216.828;Inherit;False;View;Object;False;Fast;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;380;-2133.507,898.1104;Inherit;True;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;336;-2798.124,-2314.429;Inherit;False;Property;_fresnel_color;fresnel_color;10;0;Create;True;0;0;False;0;0,0,0,0;0.7048772,0.9056604,0.9001577,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;332;-2674.645,-2861.601;Inherit;False;17;ShieldPattern;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;317;-3049.41,-856.4832;Inherit;False;Property;_vertexoffsetscale;vertexoffsetscale;8;0;Create;True;0;0;False;0;0.1;0.06;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;284;-4932.896,-201.3846;Inherit;False;837.0001;689.9695;Comment;2;282;281;Textures;1,1,1,1;0;0
Node;AmplifyShaderEditor.FresnelNode;335;-2815.8,-2527.623;Inherit;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;371;-2622.945,403.5178;Inherit;True;Property;_TextureSample0;Texture Sample 0;21;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;324;-2354.187,-639.605;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.7;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;334;-2691.642,-2753.969;Inherit;False;Property;_line_color;line_color;9;0;Create;True;0;0;False;0;0,0,0,0;0.3632075,0.8970982,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;373;-1943.263,388.3715;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;281;-4882.896,258.5853;Inherit;True;Property;_Normal;Normal;0;0;Create;True;0;0;False;0;None;22de039aa4bad8b4dbd2bd011a38c7ea;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;316;-2414.791,-1167.828;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;333;-2228.452,-2766.173;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;337;-2374.461,-2419.346;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;390;-1617.508,422.554;Inherit;True;final_sprite;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;282;-4495.696,266.5853;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;325;-2064.519,-1119.485;Inherit;False;varvertexoffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;338;-1947.308,-2423.833;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;283;-1151.15,-1549.683;Inherit;False;282;Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;339;-1668.723,-2503.635;Inherit;False;Varemissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;326;-1491.913,-1147.463;Inherit;False;325;varvertexoffset;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-893.5145,-1351.604;Float;False;Property;_Opacity;Opacity;1;0;Create;True;0;0;False;0;0.5;0.361;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;391;-1058.517,-1702.759;Inherit;False;390;final_sprite;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-483.0419,-1564.427;Float;False;True;2;ASEMaterialInspector;0;0;Standard;ASESampleShaders/Community/TFHC/Force Shield;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;-1;7;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;342;0;341;0
WireConnection;354;0;353;0
WireConnection;343;0;342;0
WireConnection;344;0;343;0
WireConnection;355;0;354;0
WireConnection;346;1;344;0
WireConnection;356;0;355;0
WireConnection;299;0;298;0
WireConnection;357;0;356;0
WireConnection;348;0;347;0
WireConnection;348;2;346;0
WireConnection;361;0;360;0
WireConnection;358;0;354;0
WireConnection;350;1;348;0
WireConnection;359;0;361;0
WireConnection;359;1;357;0
WireConnection;362;0;361;0
WireConnection;362;1;358;0
WireConnection;300;0;299;0
WireConnection;287;0;285;2
WireConnection;287;1;286;0
WireConnection;288;0;287;0
WireConnection;36;0;34;0
WireConnection;36;1;35;0
WireConnection;352;0;350;1
WireConnection;363;0;362;0
WireConnection;363;1;359;0
WireConnection;294;0;293;0
WireConnection;303;0;302;0
WireConnection;303;1;300;0
WireConnection;306;0;305;0
WireConnection;295;0;294;0
WireConnection;289;0;288;0
WireConnection;364;0;366;0
WireConnection;364;1;365;0
WireConnection;364;2;363;0
WireConnection;364;3;352;0
WireConnection;304;0;303;0
WireConnection;84;0;36;0
WireConnection;290;0;289;0
WireConnection;296;0;295;0
WireConnection;307;0;304;0
WireConnection;307;1;306;0
WireConnection;367;0;364;0
WireConnection;46;0;44;0
WireConnection;46;1;85;0
WireConnection;308;1;307;0
WireConnection;291;0;290;0
WireConnection;291;1;295;0
WireConnection;291;2;296;0
WireConnection;385;0;383;0
WireConnection;388;0;383;0
WireConnection;319;0;318;0
WireConnection;331;0;330;2
WireConnection;331;1;46;0
WireConnection;309;0;308;0
WireConnection;297;0;291;0
WireConnection;322;0;319;0
WireConnection;322;1;321;0
WireConnection;382;0;376;0
WireConnection;382;1;385;0
WireConnection;378;0;376;0
WireConnection;378;1;383;0
WireConnection;1;1;331;0
WireConnection;386;0;376;0
WireConnection;386;1;388;0
WireConnection;310;0;309;0
WireConnection;310;1;297;0
WireConnection;17;0;1;0
WireConnection;381;0;369;0
WireConnection;381;1;382;0
WireConnection;377;0;369;0
WireConnection;377;1;378;0
WireConnection;323;0;322;0
WireConnection;389;0;369;0
WireConnection;389;1;386;0
WireConnection;313;0;310;0
WireConnection;315;0;313;0
WireConnection;380;0;377;1
WireConnection;380;1;381;2
WireConnection;380;2;389;3
WireConnection;371;0;369;0
WireConnection;371;1;370;0
WireConnection;324;0;323;0
WireConnection;373;0;371;0
WireConnection;373;1;380;0
WireConnection;316;0;315;0
WireConnection;316;1;317;0
WireConnection;316;2;324;0
WireConnection;333;0;332;0
WireConnection;333;1;334;0
WireConnection;337;0;335;0
WireConnection;337;1;336;0
WireConnection;390;0;373;0
WireConnection;282;0;281;0
WireConnection;325;0;316;0
WireConnection;338;0;333;0
WireConnection;338;1;337;0
WireConnection;339;0;338;0
WireConnection;0;0;391;0
WireConnection;0;1;283;0
WireConnection;0;2;339;0
WireConnection;0;9;28;0
WireConnection;0;11;326;0
ASEEND*/
//CHKSM=20D78904AFCC81834C7AD21DA8BED3F82F2673F7