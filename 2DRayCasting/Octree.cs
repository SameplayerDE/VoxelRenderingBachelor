using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DRayCasting
{
    public class Octree
    {
        private int MaxDepth = 5;
        private int MaxPointsPerNode = 8;
        private OctreeNode root;

        public Octree(Vector3 center, float halfSize)
        {
            root = new OctreeNode(center, halfSize);
        }

        public void Build(int[] mapData, int mapWidth, int mapHeight, int mapDepth)
        {
            BuildNode(root, mapData, 0, 0, 0, mapWidth, mapHeight, mapDepth, MaxDepth, mapWidth, mapHeight, mapDepth);
        }

        private int CountSolidPointsInNode(int[] mapData, int startX, int startY, int startZ, int width, int height, int depth, int mapWidth, int mapHeight)
        {
            int count = 0;
            for (int z = startZ; z < startZ + depth; z++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    for (int x = startX; x < startX + width; x++)
                    {
                        int index = x + mapWidth * (y + mapHeight * z);
                        if (mapData[index] != 0)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        private void BuildNode(OctreeNode node, int[] mapData, int startX, int startY, int startZ, int width, int height, int depth, int depthLimit, int mapWidth, int mapHeight, int mapDepth)
        {
            if (depth >= depthLimit || node.HalfSize < 1)
                return;

            int numSolidPoints = CountSolidPointsInNode(mapData, startX, startY, startZ, width, height, depth, mapWidth, mapHeight);
            if (numSolidPoints <= MaxPointsPerNode)
                return;

            float newHalfSize = node.HalfSize * 0.5f;
            int newWidth = width / 2;
            int newHeight = height / 2;
            int newDepth = depth / 2; // Corrected this line

            node.Children[0] = new OctreeNode(node.Center + new Vector3(newHalfSize, newHalfSize, newHalfSize), newHalfSize);
            node.Children[1] = new OctreeNode(node.Center + new Vector3(newHalfSize, newHalfSize, -newHalfSize), newHalfSize);
            node.Children[2] = new OctreeNode(node.Center + new Vector3(newHalfSize, -newHalfSize, newHalfSize), newHalfSize);
            node.Children[3] = new OctreeNode(node.Center + new Vector3(newHalfSize, -newHalfSize, -newHalfSize), newHalfSize);
            node.Children[4] = new OctreeNode(node.Center + new Vector3(-newHalfSize, newHalfSize, newHalfSize), newHalfSize);
            node.Children[5] = new OctreeNode(node.Center + new Vector3(-newHalfSize, newHalfSize, -newHalfSize), newHalfSize);
            node.Children[6] = new OctreeNode(node.Center + new Vector3(-newHalfSize, -newHalfSize, newHalfSize), newHalfSize);
            node.Children[7] = new OctreeNode(node.Center + new Vector3(-newHalfSize, -newHalfSize, -newHalfSize), newHalfSize);

            // Call BuildNode for each child with its appropriate region in the map
            BuildNode(node.Children[0], mapData, startX, startY, startZ, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
            BuildNode(node.Children[1], mapData, startX + newWidth, startY, startZ, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
            BuildNode(node.Children[2], mapData, startX, startY + newHeight, startZ, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
            BuildNode(node.Children[3], mapData, startX + newWidth, startY + newHeight, startZ, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
            BuildNode(node.Children[4], mapData, startX, startY, startZ + newDepth, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
            BuildNode(node.Children[5], mapData, startX + newWidth, startY, startZ + newDepth, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
            BuildNode(node.Children[6], mapData, startX, startY + newHeight, startZ + newDepth, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
            BuildNode(node.Children[7], mapData, startX + newWidth, startY + newHeight, startZ + newDepth, newWidth, newHeight, newDepth, depthLimit, mapWidth, mapHeight, mapDepth);
        }
    }
}
