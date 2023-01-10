#define GroupSize 1
static const float PI = 3.14159265f;

struct IterationResult2D
{
    float DeltaSideX;
    float DeltaSideY;
    float SideX;
    float SideY;
    float StepX;
    float StepY;

    float RotationDifference;
    float Rotation;

    float2 Start;
    float2 End;

    int Hit;
    float Length;
    int Side;
    int ID;
};

RWStructuredBuffer<IterationResult2D> Results;
int map[1] = { 1 };

float2 Position;
float Rotation;
float FOV;
int ResX;
int CPUMap[24 * 24];

[numthreads(GroupSize, 1, 1)]
void CS(uint3 localID : SV_GroupThreadID, uint3 groupID : SV_GroupID,
    uint  localIndex : SV_GroupIndex, uint3 globalID : SV_DispatchThreadID)
{
    IterationResult2D result = Results[globalID.x];

    float fovHalf = radians(FOV / 2);
    float step = radians(FOV) / ResX;

    float rotation = -fovHalf + Rotation + step * globalID.x;

    float rayDirX = cos(rotation);
    float rayDirY = sin(rotation);

    float deltaDistX = sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
    float deltaDistY = sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));

    float sideDistX;
    float sideDistY;
    float posX = Position.x;
    float posY = Position.y;
    int mapX = (int)posX;
    int mapY = (int)posY;

    int stepX = 0;
    int stepY = 0;

    if (rayDirX < 0)
    {
        stepX = -1;
        sideDistX = (posX - mapX) * deltaDistX;
    }
    else
    {
        stepX = 1;
        sideDistX = (mapX + 1.0f - posX) * deltaDistX;
    }

    if (rayDirY < 0)
    {
        stepY = -1;
        sideDistY = (posY - mapY) * deltaDistY;
    }
    else
    {
        stepY = 1;
        sideDistY = (mapY + 1.0f - posY) * deltaDistY;
    }

    int distance = 0;
    int maxDistance = 100;
    int hit = 0;
    int side = 0;
    float length = 0.0;

    while (hit == 0 && distance < maxDistance)
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

        if (CPUMap[mapX * 24 + mapY] != 0)
        {
            hit = 1;
        }
        distance++;
    }

    if (side == 0)
    {
        length = (sideDistX - deltaDistX);
    }
    else
    {
        length = (sideDistY - deltaDistY);
    }

    result.ID = CPUMap[mapX * 24 + mapY];
    result.Hit = hit;
    result.Length = length;
    result.Side = side;

    result.StepX = stepX;
    result.StepY = stepY;
    result.SideX = sideDistX;
    result.SideY = sideDistY;
    result.DeltaSideX = deltaDistX;
    result.DeltaSideY = deltaDistY;

    result.Rotation = rotation;
    result.Start = Position;
    result.End = Position + (float2(rayDirX, rayDirY) * length);

    /*
    float rayDirX = (float)Math.Cos(rotation + diff);// + planeX * cameraX;
    float rayDirY = (float)Math.Sin(rotation + diff);// + planeY * cameraX;

    float deltaDistX = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
    float deltaDistY = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));

    float sideDistX;
    float sideDistY;
    float posX = position.X;
    float posY = position.Y;
    int mapX = (int)posX;
    int mapY = (int)posY;

    int stepX = 0;
    int stepY = 0;

    //calculate step and initial sideDist
    if (rayDirX < 0)
    {
        stepX = -1;
        sideDistX = (posX - mapX) * deltaDistX;
    }
    else
    {
        stepX = 1;
        sideDistX = (mapX + 1.0f - posX) * deltaDistX;
    }

    if (rayDirY < 0)
    {
        stepY = -1;
        sideDistY = (posY - mapY) * deltaDistY;
    }
    else
    {
        stepY = 1;
        sideDistY = (mapY + 1.0f - posY) * deltaDistY;
    }

    int distance = 0;
    int maxDistance = 100;
    bool hit = false;
    int side = 0;
    float length = 0.0f;

    IterationResult2D iterationResult = new IterationResult2D();

    while (!hit && distance < maxDistance)
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

        if (IsSolid(mapX, mapY))
        {
            hit = true;
        }
        distance++;
    }

    if (side == 0)
    {
        length = (sideDistX - deltaDistX);
    }
    else
    {
        length = (sideDistY - deltaDistY);
    }

    iterationResult.StepX = stepX;
    iterationResult.StepY = stepY;
    iterationResult.SideX = sideDistX;
    iterationResult.SideY = sideDistY;
    iterationResult.DeltaSideX = deltaDistX;
    iterationResult.DeltaSideY = deltaDistY;

    iterationResult.RotationDifference = diff;
    iterationResult.Rotation = rotation + diff;
    iterationResult.Start = position;
    iterationResult.End = position + (new Vector2(rayDirX, rayDirY) * length);

    iterationResult.Length = length;
    iterationResult.Hit = hit ? 1 : 0;
    iterationResult.Side = side;
    iterationResult.ID = GetSolid(mapX, mapY);

    _iterationResults.Add(iterationResult);
    */

    Results[globalID.x] = result;
}

technique Tech0
{
    pass Pass0
    {
        ComputeShader = compile cs_5_0 CS();
    }
}