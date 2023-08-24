using Microsoft.Xna.Framework;
using OptiLib;
using SharpDX.Direct2D1;
using SharpDX.Direct3D9;
using System;
using System.Windows.Forms;

namespace QuadTreeDDA
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
        public float Length;
        public float DeltaDistX;
        public float DeltaDistY;
        public float SideDistX;
        public float SideDistY;
    }

    public static class DDACalculator
    {

        public static RayResult2D RunIteration2D<T>(float x, float y, float r, RegionQuadTree<T> tree)
        {

            float posX = x;
            float posY = y;

            RegionQuadTree<T> startCell = tree.Search(x, y);

            if (startCell != null)
            {
                posX /= startCell.Bounds.Width;
                posY /= startCell.Bounds.Height;
            }

            var rayDir = new Vector2((float)Math.Cos(r), (float)Math.Sin(r));

            float deltaDistX = 0;
            float deltaDistY = 0;

            deltaDistX = (float)Math.Sqrt(1f + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
            deltaDistY = (float)Math.Sqrt(1f + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));

            float sideDistX = 0;
            float sideDistY = 0;

            int mapX = (int)(posX);
            int mapY = (int)(posY);

            if (posX < 0)
            {
                mapX -= 1;
            }
            if (posY < 0)
            {
                mapY -= 1;
            }

            int stepX = 0;
            int stepY = 0;

            int distance = 0;
            int maxDistance = 1;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult2D result = new RayResult2D();

            while (hit == 0 && distance < maxDistance)
            {

                RegionQuadTree<T> cell = tree.Search(posX, posY);

                if (cell != null)
                {
                    
                    if (rayDir.X < 0)
                    {
                        stepX = -1;
                        sideDistX = (posX - mapX) * deltaDistX * cell.Bounds.Width;
                    }
                    else
                    {
                        stepX = 1;
                        sideDistX = (mapX + 1.0f - posX) * deltaDistX * cell.Bounds.Height;
                        Console.WriteLine(sideDistX);
                    }

                    if (rayDir.Y < 0)
                    {
                        stepY = -1;
                        sideDistY = (posY - mapY) * deltaDistY * cell.Bounds.Width;
                    }
                    else
                    {
                        stepY = 1;
                        sideDistY = (mapY + 1.0f - posY) * deltaDistY * cell.Bounds.Height;
                    }


                    //Console.WriteLine(sideDistX);
                    //Console.WriteLine(sideDistY);

                    //if (sideDistX < sideDistY)
                    //{
                    //    sideDistX += deltaDistX;
                    //    mapX += stepX;
                    //    side = 0;
                    //}
                    //else
                    //{
                    //    sideDistY += deltaDistY;
                    //    mapY += stepY;
                    //    side = 1;
                    //}

                    //if (solid(mapX, mapY))
                    //{
                    //    hit = 1;
                    //}

                }


                distance++;
            }

            result.DeltaDistX = deltaDistX;
            result.DeltaDistY = deltaDistY;

            result.SideDistX = sideDistX;
            result.SideDistY = sideDistY;

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

        public static RayResult2D RunIteration2D(float x, float y, float r, StupidTree tree)
        {

            float posX = x / StupidLeaf.Size;
            float posY = y / StupidLeaf.Size;

            var rayDir = new Vector2((float)Math.Cos(r), (float)Math.Sin(r));

            float deltaDistX = 0;
            float deltaDistY = 0;

            deltaDistX = (float)Math.Sqrt(1f + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
            deltaDistY = (float)Math.Sqrt(1f + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));

            float sideDistX = 0;
            float sideDistY = 0;

            int mapX = (int)(posX);
            int mapY = (int)(posY);

            if (posX < 0)
            {
                mapX -= 1;
            }
            if (posY < 0)
            {
                mapY -= 1;
            }

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

            sideDistX *= StupidLeaf.Size;
            sideDistY *= StupidLeaf.Size;


            int distance = 0;
            int maxDistance = 10;
            int hit = 0;
            int side = 0;
            int orientation = 0;
            float rayLength = 0.0f;

            RayResult2D result = new RayResult2D();

            while (hit == 0 && distance < maxDistance)
            {


                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    side = 0;
                    //orientation = stepX < 0 ? 1 : 0;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                    side = 1;
                    //orientation = stepY < 0 ? 1 : 0;
                }

                try
                {
                    if (tree.Leaves[mapY, mapX].State == TreeState.Full)
                    {
                        hit = 1;
                    }

                    if (tree.Leaves[mapY, mapX].State == TreeState.Partial)
                    {
                        //if (side == 0)
                        //{
                        //    rayLength = sideDistX - deltaDistX;
                        //}
                        //else if (side == 1)
                        //{
                        //    rayLength = sideDistY - deltaDistY;
                        //}
                        //var start = new Vector2(x, y) + rayDir * rayLength;
                        //RayResult2D innerResult;
                        //if (side == 0)
                        //{
                        //    start.X = orientation * StupidLeaf.Size;
                        //    start.Y %= StupidLeaf.Size;
                        //}
                        //if (side == 1)
                        //{
                        //    start.X %= StupidLeaf.Size;
                        //    start.Y = orientation * StupidLeaf.Size;
                        //}
                        //innerResult = RunIteration2D(start.X, start.Y, r, tree.Leaves[mapY, mapX]);
                        //if (innerResult.Hit == 1)
                        //{
                        //    //sideDistX += innerResult.Length;
                        //}
                    }

                }
                catch(Exception exception)
                {

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

            result.DeltaDistX = deltaDistX;
            result.DeltaDistY = deltaDistY;

            result.SideDistX = sideDistX;
            result.SideDistY = sideDistY;

            result.Length = rayLength;
            result.Hit = hit;

            return result;
        }

        public static RayResult2D RunIteration2D(float x, float y, float r, StupidLeaf leaf)
        {

            float posX = x / StupidLeaf.Size;
            float posY = y / StupidLeaf.Size;

            var rayDir = new Vector2((float)Math.Cos(r), (float)Math.Sin(r));

            float deltaDistX = 0;
            float deltaDistY = 0;

            deltaDistX = (float)Math.Sqrt(1f + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X));
            deltaDistY = (float)Math.Sqrt(1f + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y));

            float sideDistX = 0;
            float sideDistY = 0;

            int mapX = (int)(posX);
            int mapY = (int)(posY);

            if (posX < 0)
            {
                mapX -= 1;
            }
            if (posY < 0)
            {
                mapY -= 1;
            }

            int stepX = 0;
            int stepY = 0;

            int distance = 0;
            int maxDistance = 10;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult2D result = new RayResult2D();

            while (hit == 0 && distance < maxDistance)
            {

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

                distance++;
            }

            result.DeltaDistX = deltaDistX;
            result.DeltaDistY = deltaDistY;

            result.SideDistX = sideDistX;
            result.SideDistY = sideDistY;

            if (side == 0)
            {
                rayLength = sideDistX - deltaDistX;
            }
            else if (side == 1)
            {
                rayLength = sideDistY - deltaDistY;
            }

            result.Length = rayLength;
            result.Hit = hit;

            return result;
        }

    }
}
