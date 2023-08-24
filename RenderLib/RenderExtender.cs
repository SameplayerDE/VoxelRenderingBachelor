using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OptiLib;
using System;

namespace RenderLib
{
    public static class RenderExtender
    {
        public static void DrawRectOutLined(this SpriteBatch spriteBatch, Texture2D pixel, Rectangle rectangle, Color color)
        {
            var X = (int)rectangle.X;
            var Y = (int)rectangle.Y;
            var Width = (int)rectangle.Width;
            var Height = (int)rectangle.Height;
            var minX = X;
            var minY = Y;
            var maxX = X + Width;
            var maxY = Y + Height;

            spriteBatch.Draw(pixel, new Rectangle(minX, minY, Width, 1), color);
            spriteBatch.Draw(pixel, new Rectangle(maxX - 1, minY, 1, Height), color);
            spriteBatch.Draw(pixel, new Rectangle(minX, minY, 1, Height), color);
            spriteBatch.Draw(pixel, new Rectangle(minX, maxY - 1, Width, 1), color);
        }

        public static void DrawBounds2DOutLined(this SpriteBatch spriteBatch, Texture2D pixel, Bounds2D bounds, Color color)
        {
            var X = (int)bounds.X;
            var Y = (int)bounds.Y;
            var Width = (int)bounds.Width;
            var Height = (int)bounds.Height;
            var minX = X;
            var minY = Y;
            var maxX = X + Width;
            var maxY = Y + Height;

            spriteBatch.Draw(pixel, new Rectangle(minX, minY, Width, 1), color);
            spriteBatch.Draw(pixel, new Rectangle(maxX - 1, minY, 1, Height), color);
            spriteBatch.Draw(pixel, new Rectangle(minX, minY, 1, Height), color);
            spriteBatch.Draw(pixel, new Rectangle(minX, maxY - 1, Width, 1), color);
        }

        public static void DrawRegionTree<T>(this SpriteBatch spriteBatch, Texture2D pixel, RegionQuadTree<T> regionQuadTree, Color color)
        {
            if (!regionQuadTree.IsDivided)
            {
                if (regionQuadTree.Nodes.Count == 0)
                {
                    DrawBounds2DOutLined(spriteBatch, pixel, regionQuadTree.Bounds, color);
                }
                else
                {
                    //DrawBounds2DOutLined(spriteBatch, pixel, regionQuadTree.Bounds, Color.Black);
                    spriteBatch.Draw(pixel, new Vector2(regionQuadTree.Nodes[0].X, regionQuadTree.Nodes[0].Y), Color.Gold);
                }
            }
            else
            {
                foreach (var division in regionQuadTree.Divisions)
                {
                    DrawRegionTree(spriteBatch, pixel, division, color);
                }
            }
        }

        public static void DrawStupidTree(this SpriteBatch spriteBatch, Texture2D pixel, StupidTree tree, Color color)
        {
            for (int i = 0; i < tree.S; i++)
            {
                for (int j = 0; j < tree.S; j++)
                {
                    var leaf = tree.Leaves[i, j];
                    DrawRectOutLined(spriteBatch, pixel, new Rectangle(tree.X + leaf.X * StupidLeaf.Size, tree.Y + leaf.Y * StupidLeaf.Size, StupidLeaf.Size, StupidLeaf.Size), color);
                    DrawStupidLeaf(spriteBatch, pixel, leaf, color);
                }
            }
        }

        public static void DrawStupidLeaf(this SpriteBatch spriteBatch, Texture2D pixel, StupidLeaf leaf, Color color)
        {
            if (leaf.State == TreeState.Empty)
            {
                spriteBatch.Draw(pixel, new Rectangle(leaf.Parent.X + leaf.X * StupidLeaf.Size, leaf.Parent.Y + leaf.Y * StupidLeaf.Size, StupidLeaf.Size, StupidLeaf.Size), Color.DarkBlue);
                return;
            }
            if (leaf.State == TreeState.Full)
            {
                spriteBatch.Draw(pixel, new Rectangle(leaf.Parent.X + leaf.X * StupidLeaf.Size, leaf.Parent.Y + leaf.Y * StupidLeaf.Size, StupidLeaf.Size, StupidLeaf.Size), Color.DarkRed);
                return;
            }
            if (leaf.State == TreeState.Partial)
            {
                for (int i = 0; i < StupidLeaf.Size; i++)
                {
                    for (int j = 0; j < StupidLeaf.Size; j++)
                    {
                        var value = leaf.Fruits[i, j];

                        if (value != 0)
                        {
                            spriteBatch.Draw(pixel, new Vector2(leaf.Parent.X + leaf.X * StupidLeaf.Size + j, leaf.Parent.Y + leaf.Y * StupidLeaf.Size + i), new Color(value, value, value));
                        }
                    }
                }
            }
        }

        public static void LineAngle(this SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, float angle, float length, Color color, float thickness)
        {
            spriteBatch.Draw(pixel, start, new Rectangle(0, 0, 1, 1), color, angle, new Vector2(0, .5f), new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public static void LineAngle(this SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, float angle, float length, Color color)
        {
            spriteBatch.Draw(pixel, start, new Rectangle(0, 0, 1, 1), color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public static void Line(this SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color, float thickness)
        {
            LineAngle(spriteBatch, pixel, start, (float)Math.Atan2(end.Y - start.Y, end.X - start.X), Vector2.Distance(start, end), color, thickness);
        }

        public static void Line(this SpriteBatch spriteBatch, Texture2D pixel, Vector2 start, Vector2 end, Color color)
        {
            LineAngle(spriteBatch, pixel, start, (float)Math.Atan2(end.Y - start.Y, end.X - start.X), Vector2.Distance(start, end), color);
        }
    }
}
