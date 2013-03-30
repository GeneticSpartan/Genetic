float timer;
float blurDistance = 0.002f;
sampler textureSampler : register(s0);

float4 GrayscalePS(float2 texCoords : TEXCOORD0) : COLOR
{	
	float4 color = tex2D(textureSampler, texCoords);
	
	// Wiggle Distort
	//texCoords.x += sin(timer + texCoords.y * 10) * 0.01f;
    //texCoords.y += cos(timer + texCoords.x * 10) * 0.01f;
	//float4 color = tex2D(textureSampler, texCoords);

	// Water Reflection
	//texCoords.x += texCoords.y * sin(timer * 5 + (texCoords.y * 50)) * 0.01f;
	//texCoords.y = 1 - texCoords.y;
	//float4 color = tex2D(textureSampler, texCoords);

	//float4 color = tex2D(textureSampler, float2(texCoords.x + blurDistance, texCoords.y + blurDistance));
	//color += tex2D(textureSampler, float2(texCoords.x - blurDistance, texCoords.y - blurDistance));
	//color += tex2D(textureSampler, float2(texCoords.x + blurDistance/2, texCoords.y + blurDistance/2));
	//color += tex2D(textureSampler, float2(texCoords.x - blurDistance/2, texCoords.y - blurDistance/2));
	//color += tex2D(textureSampler, float2(texCoords.x + blurDistance/4, texCoords.y + blurDistance/4));
	//color += tex2D(textureSampler, float2(texCoords.x - blurDistance/4, texCoords.y - blurDistance/4));
	//color /= 6;

	// Grayscale
	//float4 color = tex2D(textureSampler, texCoords);
	//color = dot(color, float3(0.3f, 0.59f, 0.11f));

	// Set the color's alpha back to 1.
	//color.a = 1.0f;

	// Scan Lines
	//float4 color = tex2D(textureSampler, texCoords);
	//float waveColor = sin(texCoords.y * 400);
	//waveColor = max(0.8f, waveColor);

	//color.rgb *= waveColor;

	return color;
}

technique Grayscale
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 GrayscalePS();
	}
}
