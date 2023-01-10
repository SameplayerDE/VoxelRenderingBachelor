using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{
    //Player that only moves in the direction he is facing
    public class Player
    {
        public Vector2 Position;
        public Vector2 Direction;

        public float Rotation; //Radiant

        private const float _rotationSpeed = MathHelper.Pi;
        private const float _movementSpeed = 5f;

        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboard.IsKeyDown(Keys.Left))
            {
                Rotation -= _rotationSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                Rotation += _rotationSpeed * delta;
            }

            if (Rotation < 0)
            {
                Rotation += MathHelper.TwoPi;
            }
            else if (Rotation >= MathHelper.TwoPi)
            {
                Rotation -= MathHelper.TwoPi;
            }

            Direction.X = (float)Math.Cos(Rotation);
            Direction.Y = (float)Math.Sin(Rotation);
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

        public Vector2 Next(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            return Position + Direction * _movementSpeed * delta;
        }
    }
}
