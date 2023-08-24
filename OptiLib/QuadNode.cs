namespace OptiLib
{
    public struct QuadNode<T>
    {

        public float X;
        public float Y;
        public T Value;

        public QuadNode(float x, float y, T value)
        {
            X = x;
            Y = y;
            Value = value;
        }

    }
}
