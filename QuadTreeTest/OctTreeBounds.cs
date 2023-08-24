using System.Runtime.CompilerServices;

namespace QuadTreeTest
{
    public struct QuadTreeBounds
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public QuadTreeBounds(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(float x, float y)
        {
            return x >= X && y >= Y && y < Y + Height && x < X + Width;
        }
    }
}
