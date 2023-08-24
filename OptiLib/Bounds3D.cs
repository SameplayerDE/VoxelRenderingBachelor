namespace OptiLib
{
    public struct Bounds3D
    {

        public float X;
        public float Y;
        public float Z;
        public float Width;
        public float Height;
        public float Depth;

        public Bounds3D(float x, float y, float z, float width, float height, float depth)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
            Depth = depth;
        }

        public Bounds3D(float x, float y, float z, float scale)
        {
            X = x;
            Y = y;
            Z = z;
            Width = scale;
            Height = scale;
            Depth = scale;
        }

        public bool Contains(float x, float y, float z)
        {
            return x >= X && y >= Y && z >= Z && x < X + Width && y < Y + Height && z < Z + Depth;
        }

    }
}