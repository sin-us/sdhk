float3 LightDirection; // world space direction
float4 LightAmbient;
float4 LightDiffuse;
float4 LightSpecular;

float4 MaterialAmbient;
float4 MaterialDiffuse;
float4 MaterialSpecular;
float MaterialShininess;

float4x4 World;
float4x4 WorldInverseTranspose;
float4x4 WorldViewProjection;

float4 GlobalAmbient;

texture Texture;
sampler2D colorMap = sampler_state
{
    Texture = <Texture>;
    MagFilter = Linear;
    MinFilter = Anisotropic;
    MipFilter = Linear;
    MaxAnisotropy = 16;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinates : TEXCOORD1;
    float3 LightDirection : TEXCOORD2;
    float3 HalfVector : TEXCOORD3;
    float4 Diffuse : COLOR0;
    float4 Specular : COLOR1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float3 worldPosition = mul(input.Position, World).xyz;
    float3 viewDirection = -worldPosition;

    output.Position = mul(input.Position, WorldViewProjection);
    output.TextureCoordinates = input.TextureCoordinates;
    output.LightDirection = -LightDirection;
    output.HalfVector = normalize(normalize(output.LightDirection) + normalize(viewDirection));
    output.Normal = mul((float3)input.Normal, (float3x3) WorldInverseTranspose);
    output.Diffuse = MaterialDiffuse * LightDiffuse;
    output.Specular = MaterialSpecular * LightSpecular;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
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

technique SpecularPerPixel
{
    pass Pass1
    {
        VertexShader = compile vs_5_0 VertexShaderFunction();
        PixelShader = compile ps_5_0 PixelShaderFunction();
    }
}