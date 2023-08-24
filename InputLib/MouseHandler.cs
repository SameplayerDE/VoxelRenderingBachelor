using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputLib
{
    public static class MouseHandler
    {
        private static MouseState _currMouseState;
        private static MouseState _prevMouseState;

        public static Point CurrPosition;
        public static Point PrevPosition;

        public static void Update(GameTime gameTime)
        {
            _prevMouseState = _currMouseState;
            _currMouseState = Mouse.GetState();

            CurrPosition = _currMouseState.Position;
            PrevPosition = _prevMouseState.Position;
        }

        public static bool IsLeftDownOnce()
        {
            return IsLeftDown() && WasLeftUp();
        }

        public static bool IsLeftDown()
        {
            return _currMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool WasLeftUp()
        {
            return _prevMouseState.LeftButton == ButtonState.Released;
        }
    }
}
