using System.Runtime.CompilerServices;

namespace QuadTreeTest
{
    public struct OctTreeBounds
    {
        public float X;
        public float Y;
        public float Z;
        public float Width;
        public float Height;
        public float Depth;

        public OctTreeBounds(float x, float y, float z, float width, float height, float depth)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public bool Contains(float x, float y, float z)
        {
            return x >= X && y >= Y && z >= Z && y < Y + Height && x < X + Width && z < Z + Depth;
        }
    }
}
