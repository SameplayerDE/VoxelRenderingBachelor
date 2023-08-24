using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptiLib
{
    public class StupidLeaf
    {

        public StupidTree Parent;
        public TreeState State = TreeState.Empty;
        public int X;
        public int Y;

        public const int Size = 32;

        public int[,] Fruits;

        public StupidLeaf(int x, int y, StupidTree parent)
        {
            X = x;
            Y = y;
            Fruits = new int[Size, Size];
            Parent = parent;
        }

        public void Insert(int x, int y, int value)
        {
            Fruits[y, x] = value;
            int lastValue = Fruits[0, 0];
            for (int i = 1; i < Size * Size; i++)
            {
                if (lastValue != Fruits[i / Size, i % Size])
                {
                    State = TreeState.Partial;
                    return;
                }
            }
            if (lastValue == 0)
            {
                State = TreeState.Empty;
            }
            else
            {
                State = TreeState.Full;
            }
        }
    }
}
