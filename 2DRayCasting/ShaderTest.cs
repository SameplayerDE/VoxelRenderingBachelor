using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RayCasting
{
    public class ShaderTest : Game
    {

        const int ResolutionX = 680;
        const int ResolutionY = 420;

        private GraphicsDeviceManager _graphicsDeviceManager;
        private Effect _effect;
        private Texture2D _texture;
        private SpriteBatch _spriteBatch;

        public ShaderTest()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _graphicsDeviceManager.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(16);

            _graphicsDeviceManager.PreferredBackBufferWidth = ResolutionX;
            _graphicsDeviceManager.PreferredBackBufferHeight = ResolutionY;
            _graphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _effect = Content.Load<Effect>("Effect2");
            _texture = Content.Load<Texture2D>("ass");
        }

        protected override void Update(GameTime gameTime)
        {
            //_effect.Parameters["Seconds"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            //_effect.Parameters["Milliseconds"].SetValue((uint)gameTime.TotalGameTime.TotalMilliseconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(effect: _effect);

            _spriteBatch.Draw(_texture, Vector2.Zero, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
