using System;
using System.Collections.Generic;

namespace QuadTreeTest
{
    public struct Node
    {
        public float X;
        public float Y;
        public float Z;
        public byte Value;
    }


    public class OctTree
    {
        public OctTreeBounds Bounds;
        public OctTreeState State;
        public OctTree[] Divisions;
        public OctTree Parent;

        public List<Node> Values;

        public int MaxLevel;
        public int Level;
        public bool IsDivided;

        public OctTree(float x, float y, float z, float size)
        {
            Bounds = new OctTreeBounds(x, y, z, size, size, size);
            Divisions = new OctTree[8];
            MaxLevel = (int)20;
            Values = new List<Node>();
        }

        public void Insert(float x, float y, float z, byte value)
        {
            if (!Bounds.Contains(x, y, z))
            {
                return;
            }

            if (IsDivided)
            {
                foreach (var division in Divisions)
                {
                    if (division != null)
                    {
                        division.Insert(x, y, z, value);
                    }
                }
            }
            else
            {
                if (Level < MaxLevel)
                {
                    Split();
                    foreach (var division in Divisions)
                    {
                        if (division != null)
                        {
                            division.Insert(x, y, z, value);
                        }
                    }
                }
                else
                {
                    Values.Add(new Node()
                    {
                        X = x,
                        Y = y,
                        Z = z,
                        Value = value,
                    });
                    //Console.WriteLine($"{x}, {y}, {z}");
                }
            }
        }

        public void Split()
        {
            var scale = Bounds.Width / 2;

            var minX = Bounds.X;
            var minY = Bounds.Y;
            var minZ = Bounds.Z;

            var maxX = minX + Bounds.Width / 2;
            var maxY = minY + Bounds.Height / 2;
            var maxZ = minZ + Bounds.Depth / 2;

            Divisions[0] = new OctTree(minX, minY, minZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[1] = new OctTree(maxX, minY, minZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[2] = new OctTree(minX, minY, maxZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[3] = new OctTree(maxX, minY, maxZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };

            Divisions[4] = new OctTree(minX, maxY, minZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[5] = new OctTree(maxX, maxY, minZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[6] = new OctTree(minX, maxY, maxZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };
            Divisions[7] = new OctTree(maxX, maxY, maxZ, scale) { Parent = this, Level = Level + 1, MaxLevel = MaxLevel };

            IsDivided = true;
        }

    }
}
