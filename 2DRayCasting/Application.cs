using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{

    public class Application : Game
    {
        private Texture2D _pixel;

        private const float _rotationSpeed = 180f;
        private const float _movementSpeed = 32f;

        private Vector2 _position;
        private Point _mapPosition;
        private Point _gridPosition;

        private Vector2 _direction;
        private float _facing;

        private int _resolution = 10;
        private float _ratio = 0;

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 400;
        private const int _virtualResolutionY = 400;

        const int ResolutionX = 400;
        const int ResolutionY = 400;

        GraphicsDeviceManager graphics;
        private SpriteBatch _spriteBatch;

        private int[,] _map;
        private const int _tileSize = 8;

        private SpriteFont _font;
        private List<string> _debug;

        private List<Point> _hits;

        public Application()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = ResolutionX;
            graphics.PreferredBackBufferHeight = ResolutionY;
            graphics.ApplyChanges();

            if (_resolution > _virtualResolutionX || _resolution <= 0)
            {
                _ratio = 1;
            }
            else
            {
                _ratio = _virtualResolutionX / _resolution;
            }

            _map = new int[5, 5]
            {
                {1, 1, 1, 1, 1 },
                {1, 0, 0, 0, 1 },
                {1, 0, 0, 0, 1 },
                {1, 1, 0, 0, 1 },
                {1, 1, 1, 1, 1 },
            };

            _debug = new List<string>();
            _hits = new List<Point>();

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
            _debug.Clear();
            var keyboard = Keyboard.GetState();

            int mapX = (int)_position.X;
            int mapY = (int)_position.Y;

            int gridX = mapX / _tileSize;
            int gridY = mapY / _tileSize;

            _mapPosition = new Point(mapX, mapY);
            _gridPosition = new Point(gridX, gridY);

            _debug.Add($"{_position.X}, {_position.Y}");
            _debug.Add($"{mapX}, {mapY}");
            _debug.Add($"{gridX}, {gridY}");

            _facing = MathHelper.ToDegrees(_facing);
            if (keyboard.IsKeyDown(Keys.Left))
            {
                _facing -= _rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                _facing += _rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            
            if (_facing < 0)
            {
                _facing = _facing + 360f;
            }
            if (_facing >= 360f)
            {
                _facing = _facing - 360f;
            }

            _facing = Math.Clamp(_facing, 0f, 360f);
            _facing = (int)_facing;
            _facing = MathHelper.ToRadians(_facing);

            _direction = Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(_facing));

            if (keyboard.IsKeyDown(Keys.Up))
            {
                _position += _direction * (_movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            /*if (keyboard.IsKeyDown(Keys.Down))
            {
                _position -= _direction * (_movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }*/

            Console.WriteLine(_facing);
            
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(_virtualScreen);
            GraphicsDevice.Clear(Color.Red);

            _spriteBatch.Begin();

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            
            _spriteBatch.Draw(_virtualScreen, new Rectangle(0, 0, ResolutionX, ResolutionY), Color.White);

            for (var i = 0; i < _debug.Count; i++)
            {
                var line = _debug[i];

                _spriteBatch.Draw(_pixel, new Rectangle(new Vector2(0, 50 + _font.LineSpacing * i).ToPoint(), _font.MeasureString(line).ToPoint()), Color.Black);
                _spriteBatch.DrawString(_font, line, new Vector2(0, 50 + _font.LineSpacing * i), Color.White);
            }

            
            _spriteBatch.End();


            base.Draw(gameTime);
        }

        private bool IsSolid(float x, float y)
        {
            int gridX = (int)(x / _tileSize);
            int gridY = (int)(y / _tileSize);

            if (gridX >= _map.GetLength(1) || gridX < 0)
            {
                return true;
            }
            if (gridY >= _map.GetLength(0) || gridY < 0)
            {
                return true;
            }

            bool solid = _map[gridY, gridX] != 0;
            return solid;
        }
    }
}
/*float centerX = _virtualResolutionX / 2f;
            float centerY = _virtualResolutionY / 2f;

            var count = 0;
            foreach (var hit in _hits)
            {
                float height = _virtualResolutionY - Vector2.Distance(_position, hit.ToVector2());
                float width = _ratio;
                float xx = count;
                float yy = centerY - height / 2f;

                var gray = xx / _virtualResolutionX;
                gray = 0;

                if (IsSolid(hit.X, hit.Y))
                {
                    _spriteBatch.Draw(_pixel, new Rectangle((int)xx, (int)yy, (int)width, (int)height), new Color(gray, gray, gray));

                    count++;
                }
            }*/