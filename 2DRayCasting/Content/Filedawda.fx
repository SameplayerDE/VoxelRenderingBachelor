#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

int getSolid(int x, int y, int z) {

    int gridX = (int)x;
    int gridY = (int)y;
    int gridZ = (int)z;

    return 0;
};

int isBlocked(float3 a, float3 b) {

    float3 dir = b - a;
    float len = length(dir);
    dir = normalize(dir);

    float posX = a.x;
    float posY = a.y;
    float posZ = a.z;

    int mapX = (int)posX;
    int mapY = (int)posY;
    int mapZ = (int)posZ;

    if (posX < 0)
    {
        mapX -= 1;
    }
    if (posY < 0)
    {
        mapY -= 1;
    }
    if (posZ < 0)
    {
        mapZ -= 1;
    }

    float3 mapPosition = float3(mapX, mapY, mapZ);

    int stepSize = 1;

    float3 rayPosition = a;
    float3 rayDir = dir;
    float rayDirLength = length(rayDir);

    float3 deltaDist = float3(rayDirLength, rayDirLength, rayDirLength) / rayDir;
    deltaDist = float3(abs(deltaDist.x), abs(deltaDist.y), abs(deltaDist.z));



    float deltaDistX = deltaDist.x;
    float deltaDistY = deltaDist.y;
    float deltaDistZ = deltaDist.z;

    float3 raySign = float3(sign(rayDir.x), sign(rayDir.y), sign(rayDir.z));

    float3 sideDist = (raySign * (mapPosition - rayPosition) + (raySign * 0.5f) + float3(0.5f, 0.5f, 0.5f)) * deltaDist;
    float3 step = raySign;

    float sideDistX = sideDist.x;
    float sideDistY = sideDist.y;
    float sideDistZ = sideDist.z;

    int stepX = (int)step.x;
    int stepY = (int)step.y;
    int stepZ = (int)step.z;

    int distance = 0;
    int maxDistance = 100;
    int side = 0;
    float rayLength = 0.0f;
    int hit = 0;

    while (hit == 0 && distance < maxDistance)
    {
        if (sideDistX < sideDistZ)
        {
            if (sideDistX < sideDistY)
            {
                sideDistX += deltaDistX;
                mapX += stepX;
                side = 0;
            }
            else
            {
                sideDistY += deltaDistY;
                mapY += stepY;
                side = 1;
            }
        }
        else
        {
            if (sideDistZ < sideDistY)
            {
                sideDistZ += deltaDistZ;
                mapZ += stepZ;
                side = 2;
            }
            else
            {
                sideDistY += deltaDistY;
                mapY += stepY;
                side = 1;
            }
        }

        if (getSolid(mapX, mapY, mapZ) != 0)
        {
            hit = 1;
        }
        distance++;
    }

    if (side == 0)
    {
        rayLength = sideDistX - deltaDistX;
    }
    else if (side == 1)
    {
        rayLength = sideDistY - deltaDistY;
    }
    else if (side == 2)
    {
        rayLength = sideDistZ - deltaDistZ;
    }

    if (rayLength == len && hit == 0) {
        return 0;
    }
    return 1;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}



technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};