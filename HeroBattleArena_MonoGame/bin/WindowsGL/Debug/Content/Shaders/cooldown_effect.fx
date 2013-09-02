uniform extern texture IconTexture;
uniform extern texture CooldownMask;
uniform extern float Cooldown;

sampler IconSampler = sampler_state
{
	Texture = <IconTexture>;	
};

sampler MaskSampler = sampler_state
{
	Texture = <CooldownMask>;	
};

float4 CooldownShader(float2 coords : TEXCOORD0) : COLOR0
{
	float4 iconColor = tex2D(IconSampler, coords);
	float4 maskColor = tex2D(MaskSampler, coords);
	
	float instensity = (iconColor.r*0.3f + iconColor.g*0.59f + iconColor.b*0.11f)/2;
	
	float4 grayscale = float4(instensity,instensity,instensity,iconColor.a);
	return lerp(grayscale, iconColor, step(Cooldown, maskColor.r));
    //return float4(1, 0, 0, 1);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 CooldownShader();
    }
}
