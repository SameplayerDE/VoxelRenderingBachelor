using Microsoft.Xna.Framework;

namespace RayCasting
{
    public enum Side
    {
        X, Y, Z
    }

    public enum SideOrientation
    {
        Positiv, Negativ
    }

    public struct RayResult2D
    {
        public int Hit;
        public int Side;
        public float Length;
        public Vector2 Direction;
        public Vector2 From;
        public Vector2 To;
        public float Rotation;
        public float Angle;
        public int Id;
    }

    public struct RayResult3D
    {
        public int Id;
        public int Hit;
        public int Side;
        public int SideOrientation;
        public float Length;
        public Vector3 Direction;
        public Vector3 From;
        public Vector3 To;
        public Vector3 Rotation;
        public Vector3 Angle;
    }

    public static class DDACalculator
    {

        public static RayResult2D RunIteration2D(Func<float, float, bool> solid, Func<float, float, int> type, float x, float y, float r, float a, float cx, float px, float py)
        {
            float posX = x;
            float posY = y;

            int mapX = (int)posX;
            int mapY = (int)posY;

            if (posX < 0)
            {
                mapX -= 1;
            }
            if (posY < 0)
            {
                mapY -= 1;
            }

            var rayDir = new Vector2((float)Math.Cos(r + a) + px * cx, (float)Math.Sin(r + a) + py * cx);

            float deltaDistX = (float)Math.Sqrt(1 + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));

            float sideDistX = 0;
            float sideDistY = 0;

            int stepX = 0;
            int stepY = 0;

            if (rayDir.X < 0)
            {
                stepX = -1;
                sideDistX = (posX - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0f - posX) * deltaDistX;
            }

            if (rayDir.Y < 0)
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
            float rayLength = 0.0f;

            RayResult2D result = new RayResult2D();

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

                if (solid(mapX, mapY))
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

            result.From = new Vector2(posX, posY);
            result.To = result.From + rayDir * rayLength;
            result.Length = rayLength;
            result.Hit = hit;
            result.Side = side;
            result.Direction = rayDir;
            result.Rotation = r;
            result.Angle = a;
            result.Id = result.Hit == 1 ? type(mapX, mapY) : 0;

            return result;
        }

        public static RayResult2D RunIteration2D(Func<float, float, bool> solid, Func<float, float, int> type, float x, float y, float rotation, float angle)
        {
            float posX = x;
            float posY = y;

            int mapX = (int)posX;
            int mapY = (int)posY;

            if (posX < 0)
            {
                mapX -= 1;
            }
            if (posY < 0)
            {
                mapY -= 1;
            }

            var rayDir = new Vector2((float)Math.Cos(rotation + angle), (float)Math.Sin(rotation + angle));

            float deltaDistX = (float)Math.Sqrt(1 + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));

            float sideDistX = 0;
            float sideDistY = 0;

            int stepX = 0;
            int stepY = 0;

            if (rayDir.X < 0)
            {
                stepX = -1;
                sideDistX = (posX - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0f - posX) * deltaDistX;
            }

            if (rayDir.Y < 0)
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
            float rayLength = 0.0f;

            RayResult2D result = new RayResult2D();

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

                if (solid(mapX, mapY))
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

            result.From = new Vector2(posX, posY);
            result.To = result.From + rayDir * rayLength;
            result.Length = rayLength;
            result.Hit = hit;
            result.Side = side;
            result.Direction = rayDir;
            result.Rotation = rotation;
            result.Angle = angle;
            result.Id = result.Hit == 1 ? type(mapX, mapY) : 0;

            return result;
        }

        public static RayResult3D RunIteration3D(Func<float, float, float, bool> solid, Func<float, float, float, int> type, float x, float y, float z, float rx, float ry, float rz, float ax, float ay, float az)
        {

            Vector3 position = new Vector3(x, y, z);

            float posX = x;
            float posY = y;
            float posZ = z;

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

            Vector3 mapPosition = new Vector3(mapX, mapY, mapZ);

            var n_rotation =
                Matrix.CreateRotationX(rx + ax) *
                Matrix.CreateRotationY(ry + ay) *
                Matrix.CreateRotationZ(rz + az);

            var rayDir = Vector3.Transform(Vector3.Backward, n_rotation);
            
            var deltaDist = new Vector3(rayDir.Length()) / rayDir;
            deltaDist = new Vector3(Math.Abs(deltaDist.X), Math.Abs(deltaDist.Y), Math.Abs(deltaDist.Z));

            float deltaDistX = deltaDist.X;
            float deltaDistY = deltaDist.Y;
            float deltaDistZ = deltaDist.Z;

            var raySign = new Vector3(Math.Sign(rayDir.X), Math.Sign(rayDir.Y), Math.Sign(rayDir.Z));
            Vector3 sideDist = (raySign * (mapPosition - position) + (raySign * 0.5f) + new Vector3(0.5f)) * deltaDist;
            Vector3 step = raySign;

            float sideDistX = sideDist.X;
            float sideDistY = sideDist.Y;
            float sideDistZ = sideDist.Z;

            int stepX = (int)step.X;
            int stepY = (int)step.Y;
            int stepZ = (int)step.Z;       

            int distance = 0;
            int maxDistance = 30;
            int hit = 0;
            Side side = 0;
            SideOrientation orientation = 0;
            float rayLength = 0.0f;

            RayResult3D result = new RayResult3D();

            while (hit == 0 && distance < maxDistance)
            {
                if (sideDistX < sideDistZ)
                {
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = Side.X;
                        orientation = stepX < 0 ? SideOrientation.Positiv : SideOrientation.Negativ;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = Side.Y;
                        orientation = stepY < 0 ? SideOrientation.Positiv : SideOrientation.Negativ;
                    }
                }
                else
                {
                    if (sideDistZ < sideDistY)
                    {
                        sideDistZ += deltaDistZ;
                        mapZ += stepZ;
                        side = Side.Z;
                        orientation = stepZ < 0 ? SideOrientation.Positiv : SideOrientation.Negativ;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = Side.Y;
                        orientation = stepY < 0 ? SideOrientation.Positiv : SideOrientation.Negativ;
                    }
                }

                if (solid(mapX, mapY, mapZ))
                {
                    hit = 1;
                }
                distance++;
            }

            if (side == Side.X)
            {
                rayLength = sideDistX - deltaDistX;
            }
            else if (side == Side.Y)
            {
                rayLength = sideDistY - deltaDistY;
            }
            else if (side == Side.Z)
            {
                rayLength = sideDistZ - deltaDistZ;
            }
            

            result.From = new Vector3(posX, posY, posZ);
            result.To = result.From + rayDir * rayLength;
            result.Length = rayLength;
            result.Hit = hit;
            result.Side = (int)side;
            result.SideOrientation = (int)orientation;
            result.Direction = rayDir;
            result.Rotation = new Vector3(rx, ry, rz);
            result.Angle = new Vector3(ax, ay, az);
            result.Id = result.Hit == 1 ? type(mapX, mapY, mapZ) : 0;

            return result;
        }
    }
}
