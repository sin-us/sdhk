float3 LightDirection; // world space direction
float3 LightPosition; // world space position
float4 LightAmbient;
float4 LightDiffuse;
float4 LightSpecular;
float LightSpotInnerCone; // spot light inner cone (theta) angle
float LightSpotOuterCone; // spot light outer cone (phi) angle
float LightRadius; // applies to point and spot lights only

float4 MaterialAmbient;
float4 MaterialDiffuse;
float4 MaterialSpecular;
float MaterialShininess;

//-----------------------------------------------------------------------------
// Globals.
//-----------------------------------------------------------------------------

float4x4 World;
float4x4 WorldInverseTranspose;
float4x4 WorldViewProjection;

float3 CameraPosition;
float4 GlobalAmbient;

//-----------------------------------------------------------------------------
// Textures.
//-----------------------------------------------------------------------------

texture Texture;
sampler2D colorMap = sampler_state
{
    Texture = <Texture>;
    MagFilter = Linear;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MaxAnisotropy = 16;
};

//-----------------------------------------------------------------------------
// Vertex Shaders.
//-----------------------------------------------------------------------------

struct VsInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VsOutputDirectional
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
    float3 HalfVector : TEXCOORD1;
    float3 LightDirection : TEXCOORD2;
    float3 Normal : TEXCOORD3;
    float4 Diffuse : COLOR0;
    float4 Specular : COLOR1;
};

struct VsOutputPoint
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
    float3 ViewDirection : TEXCOORD1;
    float3 LightDirection : TEXCOORD2;
    float3 Normal : TEXCOORD3;
    float4 Diffuse : COLOR0;
    float4 Specular : COLOR1;
};

struct VsOutputSpot
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
    float3 ViewDirection : TEXCOORD1;
    float3 LightDirection : TEXCOORD2;
    float3 SpotDirection : TEXCOORD3;
    float3 Normal : TEXCOORD4;
    float4 Diffuse : COLOR0;
    float4 Specular : COLOR1;
};

VsOutputDirectional VsDirectionalLighting(VsInput input)
{
    VsOutputDirectional output;

    float3 worldPosition = mul(float4(input.Position, 1.0f), World).xyz;
    float3 viewDirection = CameraPosition - worldPosition;
		
    output.Position = mul(float4(input.Position, 1.0f), WorldViewProjection);
    output.TextureCoordinates = input.TextureCoordinates;
    output.LightDirection = -LightDirection;
    output.HalfVector = normalize(normalize(output.LightDirection) + normalize(viewDirection));
    output.Normal = mul(input.Normal, (float3x3) WorldInverseTranspose);
    output.Diffuse = MaterialDiffuse * LightDiffuse;
    output.Specular = MaterialSpecular * LightSpecular;
        
    return output;
}

VsOutputPoint VsPointLighting(VsInput input)
{
    VsOutputPoint output;

    float3 worldPosition = mul(float4(input.Position, 1.0f), World).xyz;
    
    output.Position = mul(float4(input.Position, 1.0f), WorldViewProjection);
    output.TextureCoordinates = input.TextureCoordinates;
    output.ViewDirection = CameraPosition - worldPosition;
    output.LightDirection = (LightPosition - worldPosition) / LightRadius;
    output.Normal = mul(input.Normal, (float3x3) WorldInverseTranspose);
    output.Diffuse = MaterialDiffuse * LightDiffuse;
    output.Specular = MaterialSpecular * LightSpecular;
	
    return output;
}

VsOutputSpot VsSpotLighting(VsInput input)
{
    VsOutputSpot output;
    
    float3 worldPosition = mul(float4(input.Position, 1.0f), World).xyz;
	       
    output.Position = mul(float4(input.Position, 1.0f), WorldViewProjection);
    output.TextureCoordinates = input.TextureCoordinates;
    output.ViewDirection = CameraPosition - worldPosition;
    output.LightDirection = (LightPosition - worldPosition) / LightRadius;
    output.SpotDirection = LightDirection;
    output.Normal = mul(input.Normal, (float3x3) WorldInverseTranspose);
    output.Diffuse = MaterialDiffuse * LightDiffuse;
    output.Specular = MaterialSpecular * LightSpecular;
       
    return output;
}

//-----------------------------------------------------------------------------
// Pixel Shaders.
//-----------------------------------------------------------------------------

float4 PsDirectionalLighting(VsOutputDirectional input) : COLOR0
{
    float3 n = normalize(input.Normal);
    float3 h = normalize(input.HalfVector);
    float3 l = normalize(input.LightDirection);
    
    float nDotL = saturate(dot(n, l));
    float nDotH = saturate(dot(n, h));
    float power = (nDotL == 0.0f) ? 0.0f : pow(nDotH, MaterialShininess);

    float4 color = (MaterialAmbient * (GlobalAmbient + LightAmbient)) + (input.Diffuse * nDotL) + (input.Specular * power);

    return color * tex2D(colorMap, input.TextureCoordinates);
}

float4 PsPointLighting(VsOutputPoint input) : COLOR0
{
    float atten = saturate(1.0f - dot(input.LightDirection, input.LightDirection));

    float3 n = normalize(input.Normal);
    float3 l = normalize(input.LightDirection);
    float3 v = normalize(input.ViewDirection);
    float3 h = normalize(l + v);
    
    float nDotL = saturate(dot(n, l));
    float nDotH = saturate(dot(n, h));
    float power = (nDotL == 0.0f) ? 0.0f : pow(nDotH, MaterialShininess);
    
    float4 color = (MaterialAmbient * (GlobalAmbient + (atten * LightAmbient))) + (input.Diffuse * nDotL * atten) + (input.Specular * power * atten);
                   
    return color * tex2D(colorMap, input.TextureCoordinates);
}

float4 PsSpotLighting(VsOutputSpot input) : COLOR0
{
    float atten = saturate(1.0f - dot(input.LightDirection, input.LightDirection));
    
    float3 l = normalize(input.LightDirection);
    float2 cosAngles = cos(float2(LightSpotOuterCone, LightSpotInnerCone) * 0.5f);
    float spotDot = dot(-l, normalize(input.SpotDirection));
    float spotEffect = smoothstep(cosAngles[0], cosAngles[1], spotDot);
    
    atten *= spotEffect;
                                
    float3 n = normalize(input.Normal);
    float3 v = normalize(input.ViewDirection);
    float3 h = normalize(l + v);
    
    float nDotL = saturate(dot(n, l));
    float nDotH = saturate(dot(n, h));
    float power = (nDotL == 0.0f) ? 0.0f : pow(nDotH, MaterialShininess);
    
    float4 color = (MaterialAmbient * (GlobalAmbient + (atten * LightAmbient))) + (input.Diffuse * nDotL * atten) + (input.Specular * power * atten);
    
    return color * tex2D(colorMap, input.TextureCoordinates);
}

//-----------------------------------------------------------------------------
// Techniques.
//-----------------------------------------------------------------------------

technique PerPixelDirectionalLighting
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VsDirectionalLighting();
        PixelShader = compile ps_4_0 PsDirectionalLighting();
    }
}

technique PerPixelPointLighting
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VsPointLighting();
        PixelShader = compile ps_4_0 PsPointLighting();
    }
}

technique PerPixelSpotLighting
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VsSpotLighting();
        PixelShader = compile ps_4_0 PsSpotLighting();
    }
}