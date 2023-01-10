using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{
    public class Player3D
    {
        public Vector3 Position;
        public Vector3 Direction;
        public float RotationZ; //Radiant
        public float RotationY; //Radiant


        public Vector3 Rotation = Vector3.Zero;

        private const float _rotationSpeed = MathHelper.Pi;
        private const float _movementSpeed = 5f;

        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Left))
            {
                Rotation.Z -= _rotationSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                Rotation.Z += _rotationSpeed * delta;
            }

            if (keyboard.IsKeyDown(Keys.Up))
            {
                Rotation.Y -= _rotationSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                Rotation.Y += _rotationSpeed * delta;
            }

            if (Rotation.Z < 0)
            {
                Rotation.Z += MathHelper.TwoPi;
            }
            else if (Rotation.Z >= MathHelper.TwoPi)
            {
                Rotation.Z -= MathHelper.TwoPi;
            }
            Rotation.Y = Math.Clamp(Rotation.Y, -MathHelper.PiOver2, MathHelper.PiOver2);

            var n_rotation =
                Matrix.CreateRotationX(Rotation.X) *
                Matrix.CreateRotationY(Rotation.Y) *
                Matrix.CreateRotationZ(Rotation.Z);
            
            Direction = Vector3.Transform(new Vector3(1, 0, 0), n_rotation);
            
            if (Direction.Length() != 0)
            {
                Direction.Normalize();
            }
        }

        public void Move(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Direction * _movementSpeed * delta;
        }

        public Vector3 Next(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            return Position + Direction * _movementSpeed * delta;
        }
    }
}
