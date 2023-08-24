namespace OptiLib
{
    public class StupidTree
    {

        public int X, Y, S;
        public StupidLeaf[,] Leaves;

        public StupidTree(int x, int y, int s)
        {
            X = x;
            Y = y;
            S = s;
            Leaves = new StupidLeaf[s, s];
            for (int i = 0; i < s; i++)
            {
                for (int j = 0; j < s; j++)
                {
                    Leaves[i, j] = new StupidLeaf(j, i, this);
                }
            }
        }

        public void Insert(int x, int y, int value)
        {
            int minX = X;
            int minY = Y;
            int maxX = X + S * StupidLeaf.Size;
            int maxY = Y + S * StupidLeaf.Size;

            if (minX <= x && x < maxX)
            {
                if (minY <= y && y < maxY)
                {
                    Leaves[y / StupidLeaf.Size, x / StupidLeaf.Size].Insert(x % StupidLeaf.Size, y % StupidLeaf.Size, value);
                }
            }
        }
    }
}
