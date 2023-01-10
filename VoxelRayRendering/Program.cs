using System;

namespace VoxelRayRendering
{
    public static class Program
    {
        static void Main()
        {
            using (var app = new Application())
            {
                app.Run();
            }
        }
    }
}
