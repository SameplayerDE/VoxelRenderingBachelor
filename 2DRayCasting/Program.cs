using System;

namespace RayCasting
{
    public static class Program
    {
        static void Main()
        {
            using (var app = new DDA3D())
            {
                //app.Run();
            }
            using (var app = new Step5())
            {
                //app.Run();
            }
            using (var app = new DDA3D_Compute_Final())
            {
                app.Run();
            }
        }
    }
}
