namespace OptiLib
{
    public class QuadTree<T>
    {
        public List<QuadNode<T>> Nodes;
        public int Capacity;
        public TreeState State;
        public Bounds2D Bounds;

        public QuadTree(float x, float y, float width, float height, int capacity)
        {
            State = TreeState.Empty;
            Bounds = new Bounds2D(x, y, width, height);
            Capacity = capacity;
            Nodes = new List<QuadNode<T>>();
        }

        public void Insert(float x, float y, T value)
        {
            if (!Bounds.Contains(x, y))
            {
                return;
            }

            var node = new QuadNode<T>(x, y, value);
            if (Nodes.Count < Capacity)
            {
                Nodes.Add(node);
                return;
            }

        }
    }
}
