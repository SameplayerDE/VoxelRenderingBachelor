using static System.Formats.Asn1.AsnWriter;

namespace OptiLib
{
    public class RegionQuadTree<T>
    {
        public List<QuadNode<T>> Nodes;
        public TreeState State;
        public Bounds2D Bounds;

        public RegionQuadTree<T>[] Divisions;
        public RegionQuadTree<T>? Parent;
        public bool IsDivided;

        public int Level;

        public RegionQuadTree(float x, float y, float scale, RegionQuadTree<T>? parent = null)
        {
            State = TreeState.Empty;
            Bounds = new Bounds2D(x, y, scale);
            Nodes = new List<QuadNode<T>>();
            IsDivided = false;
            Divisions = new RegionQuadTree<T>[4];
            Parent = parent;
        }

        public void Insert(float x, float y, T value)
        {
            if (!Bounds.Contains(x, y))
            {
                return;
            }

            if (IsDivided)
            {
                foreach (var division in Divisions)
                {
                    division.Insert(x, y, value);
                }
            }
            else
            {
                if (Level < (int)Math.Log2(Bounds.Width * Math.Pow(2, Level)) - 2)
                {
                    Split();
                    State = TreeState.Partial;
                    foreach (var division in Divisions)
                    {
                        division.Insert(x, y, value);
                    }
                }
                else
                {
                    Nodes.Add(new QuadNode<T>(x, y, value));
                    State = TreeState.Full;
                }
            }
        }

        public void Remove(float x, float y)
        {
            if (!Bounds.Contains(x, y))
            {
                return;
            }

            if (IsDivided)
            {
                foreach (var division in Divisions)
                {
                    division.Remove(x, y);
                }
            }
            else
            {
                
            }
        }

        public void Split()
        {
            var scale = Bounds.Width / 2;

            var minX = Bounds.X;
            var minY = Bounds.Y;

            var maxX = minX + scale;
            var maxY = minY + scale;

            Divisions[0] = new RegionQuadTree<T>(minX, minY, scale, this) { Level = Level + 1 };
            Divisions[1] = new RegionQuadTree<T>(maxX, minY, scale, this) { Level = Level + 1 };
            Divisions[2] = new RegionQuadTree<T>(minX, maxY, scale, this) { Level = Level + 1 };
            Divisions[3] = new RegionQuadTree<T>(maxX, maxY, scale, this) { Level = Level + 1 };
            IsDivided = true;
        }

        public RegionQuadTree<T>? Search(float x, float y)
        {
            if (Bounds.Contains(x, y))
            {
                if (IsDivided)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var node = Divisions[i];
                        var result = node.Search(x, y);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
                return this;
            }
            return null;
        }

    }
}
