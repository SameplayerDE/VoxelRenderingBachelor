using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{

    public class Step2 : Game
    {
        private Texture2D _pixel;
        private SpriteFont _font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int[,] _map;
        private int _cellSize = 20;
        private Player _player;

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 480;
        private const int _virtualResolutionY = 480;

        const int ResolutionX = 720;
        const int ResolutionY = 720;

        public Step2()
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
            Window.AllowUserResizing = true;
            _graphics.ApplyChanges();

            _map = new int[24, 24]
            {
                { 0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
                { 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
                { 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };

            _player = new Player();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _virtualScreen = new RenderTarget2D(GraphicsDevice, _virtualResolutionX, _virtualResolutionY);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                var next = _player.Next(gameTime);
                if (!IsSolid(next.X, next.Y))
                {
                    _player.Move(gameTime);
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_virtualScreen);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            //Map
            if (true)
            {
                for (var y = 0; y < _virtualResolutionY; y += _cellSize)
                {
                    Line(new Vector2(0, y), new Vector2(_virtualResolutionX, y), Color.White, _spriteBatch);
                }

                for (var x = 0; x < _virtualResolutionX; x += _cellSize)
                {
                    Line(new Vector2(x, 0), new Vector2(x, _virtualResolutionY), Color.White, _spriteBatch);
                }

                for (int y = 0; y < 24; y++)
                {
                    for (int x = 0; x < 24; x++)
                    {
                        if (_map[x, y] == 0) { continue; }
                        _spriteBatch.Draw(_pixel, new Vector2(x, y) * _cellSize, null, Color.White, 0f, new Vector2(0f, 0f), _cellSize, SpriteEffects.None, 1f);
                    }
                }
            }
            //

            DrawPlayer(_player, Color.Red, 10, _spriteBatch);
            DrawPlayerDirection(_player, Color.Red, 2, 10, _spriteBatch);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_virtualScreen, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private bool IsSolid(float x, float y)
        {
            int gridX = (int)(x);
            int gridY = (int)(y);

            if (gridX >= _map.GetLength(0) || gridX < 0)
            {
                return true;
            }
            if (gridY >= _map.GetLength(1) || gridY < 0)
            {
                return true;
            }

            bool solid = _map[gridX, gridY] != 0;
            return solid;
        }

        public void DrawPlayer(Player player, Color color, int scale, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_pixel, player.Position * _cellSize, null, color, 0f, new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 1f);
        }

        public void DrawPlayerDirection(Player player, Color color, float thickness, float length, SpriteBatch spriteBatch)
        {
            LineAngle(player.Position * _cellSize, _player.Rotation, length * _cellSize, color, thickness, spriteBatch);
        }


        public void LineAngle(Vector2 start, float angle, float length, Color color, float thickness, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(_pixel, start, new Rectangle(0, 0, 1, 1), color, angle, new Vector2(0, .5f), new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public void LineAngle(Vector2 start, float angle, float length, Color color, SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(_pixel, start, new Rectangle(0, 0, 1, 1), color, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public void Line(Vector2 start, Vector2 end, Color color, float thickness, SpriteBatch _spriteBatch)
        {
            LineAngle(start, (float)Math.Atan2(end.Y - start.Y, end.X - start.X), Vector2.Distance(start, end), color, thickness, _spriteBatch);
        }

        public void Line(Vector2 start, Vector2 end, Color color, SpriteBatch _spriteBatch)
        {
            LineAngle(start, (float)Math.Atan2(end.Y - start.Y, end.X - start.X), Vector2.Distance(start, end), color, _spriteBatch);
        }
    }
}