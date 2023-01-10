using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCasting
{
    public struct IterationResult3D
    {

        public float DeltaSideX;
        public float DeltaSideY;
        public float DeltaSideZ;
        public float SideX;
        public float SideY;
        public float SideZ;
        public float StepX;
        public float StepY;
        public float StepZ;

        public bool Hit;
        public float Length;
        public int Side;
        public int ID;

        public float RotationDifference;
        public float Rotation;

        public Vector2 Start;
        public Vector2 End;

    }
}
