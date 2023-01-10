using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelMeshRendering
{

    public class Application : Game
    {
        const int ResolutionX = 680;
        const int ResolutionY = 420;

        GraphicsDeviceManager graphics;

        public Application()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = ResolutionX;
            graphics.PreferredBackBufferHeight = ResolutionY;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {

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
