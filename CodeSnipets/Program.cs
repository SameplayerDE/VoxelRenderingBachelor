//void CastRay(float rayStartX, float rayStartY, float rayAngle)
//{
//    float worldX = rayStartX;
//    float worldY = rayStartY;
//
//    int mapX = (int)worldX;
//    int mapY = (int)worldY;
//
//    if (worldX < 0)
//    {
//        mapX -= 1;
//    }
//    if (worldY < 0)
//    {
//        mapY -= 1;
//    }
//
//    float rayDirX = (float)Math.Cos(rayAngle);
//    float rayDirY = (float)Math.Sin(rayAngle);
//
//    float deltaDistX = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
//    float deltaDistY = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
//
//    float sideDistX = 0;
//    float sideDistY = 0;
//
//    int stepX = 0;
//    int stepY = 0;
//
//    if (rayDirX < 0)
//    {
//        stepX = -1;
//        sideDistX = (worldX - mapX) * deltaDistX;
//    }
//    else
//    {
//        stepX = 1;
//        sideDistX = (mapX + 1.0f - worldX) * deltaDistX;
//    }
//
//    if (rayDirY < 0)
//    {
//        stepY = -1;
//        sideDistY = (worldY - mapY) * deltaDistY;
//    }
//    else
//    {
//        stepY = 1;
//        sideDistY = (mapY + 1.0f - worldY) * deltaDistY;
//    }
//
//}
//
//void CastRay(float rayStartX, float rayStartY, float rayAngle)
//{
//    float worldX = rayStartX;
//    float worldY = rayStartY;
//
//    int mapX = (int)Math.Floor(worldX);
//    int mapY = (int)Math.Floor(worldY);
//
//    float rayDirX = (float)Math.Cos(rayAngle);
//    float rayDirY = (float)Math.Sin(rayAngle);
//
//    float deltaDistX = (float)Math.Abs(1 / rayDirX);
//    float deltaDistY = (float)Math.Abs(1 / rayDirY);
//
//    int raySignX = Math.Sign(rayDirX);
//    int raySignY = Math.Sign(rayDirY);
//
//    int stepX = raySignX;
//    int stepY = raySignY;
//
//    float sideDistX = (raySignX * (mapX - worldX) + (raySignX * 0.5f) + 0.5f) * deltaDistX;
//    float sideDistY = (raySignY * (mapY - worldY) + (raySignY * 0.5f) + 0.5f) * deltaDistY;
//}
//
//CastRay(5.15f, -0.1f, (float)Math.PI / 2f);

int width = 480;
int height = 270;
float aspectRatio = (float)height / width;

Console.WriteLine(aspectRatio);