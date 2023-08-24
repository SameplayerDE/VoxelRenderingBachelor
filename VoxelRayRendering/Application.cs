using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelRayRendering
{

    public class Application : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _rayCastRenderResult;
        const int ResolutionX = 680;
        const int ResolutionY = 420;


        public Application()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = ResolutionX;
            _graphics.PreferredBackBufferHeight = ResolutionY;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _rayCastRenderResult = new Texture2D(GraphicsDevice, ResolutionX, ResolutionY, ShaderAccess.ReadWrite);
        }

        protected override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            base.Draw(gameTime);
        }
    }
}
