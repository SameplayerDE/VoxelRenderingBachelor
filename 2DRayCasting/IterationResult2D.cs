using Microsoft.Xna.Framework;

namespace RayCasting
{
    public struct IterationResult2D
    {

        public float DeltaSideX;
        public float DeltaSideY;
        public float SideX;
        public float SideY;
        public float StepX;
        public float StepY;

        public float RotationDifference;
        public float Rotation;

        public Vector2 Start;
        public Vector2 End;

        public int Hit;
        public float Length;
        public int Side;
        public int ID;


    }
}
