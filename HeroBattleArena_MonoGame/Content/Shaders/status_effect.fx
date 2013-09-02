uniform extern texture BarsTexture;
uniform extern texture ManaMask;
uniform extern texture HealthMask;
uniform extern float ManaPercent;
uniform extern float HealthPercent;

sampler BarsSampler = sampler_state
{
	Texture = <BarsTexture>;	
};

sampler ManaSampler = sampler_state
{
	Texture = <ManaMask>;	
};

sampler HealthSampler = sampler_state
{
	Texture = <HealthMask>;	
};

float4 StatusPixelShader(float2 coords: TEXCOORD0) : COLOR0
{
    // Input textures...
    float4 barColor = tex2D(BarsSampler, coords);
    float4 manaColor = tex2D(ManaSampler, coords);
    float4 healthColor = tex2D(HealthSampler, coords);
    
	float manaAlpha = step(manaColor.r, ManaPercent);
	float healthAlpha = step(healthColor.r, HealthPercent);

	barColor.a *= saturate(manaAlpha*manaColor.a+healthAlpha*healthColor.a);
    return barColor;

    //return float4(1, 0, 0, 1);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 StatusPixelShader();
    }
}
