namespace OptiLib
{
    public struct Bounds2D
    {

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Bounds2D(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Bounds2D(float x, float y, float scale)
        {
            X = x;
            Y = y;
            Width = scale;
            Height = scale;
        }

        public bool Contains(float x, float y)
        {
            return x >= X && y >= Y && x < X + Width && y < Y + Height;
        }

    }
}
