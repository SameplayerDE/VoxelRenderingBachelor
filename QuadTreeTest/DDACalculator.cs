using Microsoft.Xna.Framework;
using System;

namespace QuadTreeTest
{
    public enum Side
    {
        X, Y
    }

    public enum SideOrientation
    {
        Positiv, Negativ
    }

    public struct RayResult2D
    {
        public int Hit;
    }

    public static class DDACalculator
    {

        public static RayResult2D RunIteration2D(float x, float y, float r, QuadTree tree)
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

            var rayDir = new Vector2((float)Math.Cos(r), (float)Math.Sin(r));

            float deltaDistX = 0;
            float deltaDistY = 0;

            float sideDistX = 0;
            float sideDistY = 0;

            int distance = 0;
            int maxDistance = 100;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult2D result = new RayResult2D();

            while (hit == 0 && distance < maxDistance)
            {

                QuadTree cell = tree.Search(x, y);

                deltaDistX = (float)Math.Sqrt(cell.Bounds.Width + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
                deltaDistY = (float)Math.Sqrt(cell.Bounds.Height + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));

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

                //if (solid(mapX, mapY))
                //{
                //    hit = 1;
                //}
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

            return result;
        }
    }
}
