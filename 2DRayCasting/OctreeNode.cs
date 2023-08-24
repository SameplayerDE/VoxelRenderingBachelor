using Microsoft.Xna.Framework;

namespace _2DRayCasting
{
    public class OctreeNode
    {
        public Vector3 Center { get; set; }
        public float HalfSize { get; set; }
        public int[] Data { get; set; }
        public OctreeNode[] Children { get; set; }

        public OctreeNode(Vector3 center, float halfSize)
        {
            Center = center;
            HalfSize = halfSize;
            Children = new OctreeNode[8];
        }
    }
}
