using System;

namespace QuadTreeTest
{
    public class QuadTree
    {
        public QuadTreeBounds Bounds;
        public QuadTreeState State;
        public QuadTree[] Divisions;
        public QuadTree Parent;

        public int Value;

        public int MaxLevel;
        public int Level;
        public bool IsDivided;

        public QuadTree(float x, float y, float size)
        {
            Bounds = new QuadTreeBounds(x, y, size, size);
            Divisions = new QuadTree[4];
            MaxLevel = (int)20;
        }

        public void CheckState()
        {
            
        }

        public void Insert(float x, float y, int value)
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
                if (Level < MaxLevel)
                {
                    Split();
                    foreach (var division in Divisions)
                    {
                        division.Insert(x, y, value);
                    }
                }
                else
                {
                    Value = value;
                }
            }
        }

        public void Combine()
        {
            if (IsDivided)
            {
                foreach (var division in Divisions)
                {
                    division.Combine();
                }
            }
            else
            {
                var parent = Parent;
                if (parent.AllNodesSameValue())
                {
                    parent.Collaps();
                }
            }
        }

        public bool AllNodesSameValue()
        {
            return Divisions[0].Value == Divisions[1].Value && Divisions[2].Value == Divisions[3].Value && Divisions[1].Value == Divisions[2].Value;
        }

        public int CollapsedValue()
        {
            var result = Divisions[0].Value + Divisions[1].Value + Divisions[2].Value + Divisions[3].Value;
            return result / 4;
        }

        public void Split()
        {
            var offset = Bounds.Width / 2;
            Divisions[0] = new QuadTree(Bounds.X, Bounds.Y, offset) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[1] = new QuadTree(Bounds.X + offset, Bounds.Y, offset) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[2] = new QuadTree(Bounds.X, Bounds.Y + offset, offset) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[3] = new QuadTree(Bounds.X + offset, Bounds.Y + offset, offset) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            IsDivided = true;
        }

        public void Collaps()
        {
            Value = CollapsedValue();
            IsDivided = false;
            //Divisions = new QuadTree[4];
        }

        public QuadTree Search(float x, float y)
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
