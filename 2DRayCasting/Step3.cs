using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{

    public class Step3 : Game
    {
        private Texture2D _pixel;
        private SpriteFont _font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int[,] _map;
        private int _cellSize = 20;
        private Player _player;
        private bool _hit;
        private float _length = 0f;
        private int _side = 0;

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 480;
        private const int _virtualResolutionY = 480;

        const int ResolutionX = 720;
        const int ResolutionY = 720;

        public Step3()
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

            float rayDirX = _player.Direction.X;// + planeX * cameraX;
            float rayDirY = _player.Direction.Y;// + planeY * cameraX;

            float deltaDistX = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));

            float sideDistX;
            float sideDistY;
            float posX = _player.Position.X;
            float posY = _player.Position.Y;
            int mapX = (int)posX;
            int mapY = (int)posY;

            int stepX = 0;
            int stepY = 0; 

            //calculate step and initial sideDist
            if (rayDirX < 0)
            {
                stepX = -1;
                sideDistX = (posX - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0f - posX) * deltaDistX;
            }

            if (rayDirY < 0)
            {
                stepY = -1;
                sideDistY = (posY - mapY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                sideDistY = (mapY + 1.0f - posY) * deltaDistY;
            }

            int distance = 0;
            int maxDistance = 1;
            _hit = false;

            while (!_hit && distance < maxDistance)
            {
                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    _side = 0;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                    _side = 1;
                }

                if (IsSolid(mapX, mapY))
                {
                    _hit = true;
                }
                distance++;
            }

            if (_side == 0)
            {
                _length = (sideDistX - deltaDistX);
            }
            else
            {
                _length = (sideDistY - deltaDistY);
            }


            Console.WriteLine("rayX " + rayDirX);
            Console.WriteLine("deltaX " + deltaDistX);
            Console.WriteLine("SideX " + sideDistX);
            Console.WriteLine("StepX " + stepX);
            Console.WriteLine("-------------");

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
            DrawPlayerDirection(_player, _hit ? Color.Red : Color.White, 2, _length, _spriteBatch);

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