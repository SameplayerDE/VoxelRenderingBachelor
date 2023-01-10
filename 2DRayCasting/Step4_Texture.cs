using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static RayCasting.DDACalculator;

namespace RayCasting
{

    public class Step4_Texture : Game
    {
        private Texture2D _pixel;
        private Texture2D _texture;
        private SpriteFont _font;

        private Texture2D[] _textures;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int[,] _map;
        private int _cellSize = 20;
        private Player _player;

        private List<RayResult2D> _iterationResults;

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 480;
        private const int _virtualResolutionY = 480;

        const int ResolutionX = 720;
        const int ResolutionY = 720;
        private float _fov = 70;
        private int _rayResolution = 1;
        private bool _fishEye = true;
        private bool _textureEnabled = false;

        public Step4_Texture()
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
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
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
                { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,1},
                { 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1},
                { 1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1},
                { 1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1},
                { 1,4,0,4,0,0,0,0,4,0,0,0,0,2,2,2,0,0,0,0,0,0,0,1},
                { 1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };

            _iterationResults = new List<RayResult2D>();

            _player = new Player();
            _player.Position = new Vector2(3, 3);

            _textures = new Texture2D[5];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _virtualScreen = new RenderTarget2D(GraphicsDevice, _virtualResolutionX, _virtualResolutionY);

            _texture = Content.Load<Texture2D>("cobblestone");

            _textures[0] = Content.Load<Texture2D>("cobblestone");
            _textures[1] = Content.Load<Texture2D>("ass");
            _textures[2] = Content.Load<Texture2D>("oak_planks");
            _textures[3] = Content.Load<Texture2D>("iron_block");
            _textures[4] = Content.Load<Texture2D>("gold_block");

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                _fov--;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                _fov++;
            }

            _fov = Math.Clamp( _fov, 1, 360);

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _rayResolution--;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _rayResolution++;
            }

            _rayResolution = Math.Clamp(_rayResolution, 1, GraphicsDevice.Viewport.Width);

            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                _fishEye = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                _fishEye = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                _textureEnabled = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                _textureEnabled = true;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                var next = _player.Next(gameTime);
                if (!IsSolid(next.X, next.Y))
                {
                    _player.Move(gameTime);
                }
                else if (!IsSolid(next.X, _player.Position.Y))
                {
                    _player.Position = new Vector2(next.X, _player.Position.Y);
                }
                if (!IsSolid(_player.Position.X, next.Y))
                {
                    _player.Position = new Vector2(_player.Position.X, next.Y);
                }
            }

            _iterationResults.Clear();

            int w = GraphicsDevice.Viewport.Width;
            float ratio = GraphicsDevice.Viewport.AspectRatio;



            /*for (var x = 0; x < w; x += _rayResolution)
            {
                float xAng = (float)Math.Atan((x - w / 2) / 1000f);
                var position = _player.Position;
                var rotation = _player.Rotation + xAng;
                _iterationResults.Add(DDACalculator.RunIteration2D(IsSolid, GetSolid, position.X, position.Y, rotation, 0));
            }*/

            /*for (var x = 0; x < w; x++)
            {
                float cameraX = 2f * x / w - 1f; //x-coordinate in camera space
                
                var position = _player.Position;
                var rotation = _player.Rotation;
                _iterationResults.Add(DDACalculator.RunIteration2D(IsSolid, GetSolid, position.X, position.Y, rotation, 0, cameraX, 0, 0.66f));
            }*/

            for (var i = -MathHelper.ToRadians(_fov / 2); i < MathHelper.ToRadians(_fov / 2); i += (MathHelper.ToRadians(_fov) / GraphicsDevice.Viewport.Width) * _rayResolution) {
                float cameraX = 2 * i / w - 1; //x-coordinate in camera space
                var position = _player.Position;
                var rotation = _player.Rotation;
                _iterationResults.Add(DDACalculator.RunIteration2D(IsSolid, GetSolid, position.X, position.Y, rotation, i, cameraX, 0, 0.66f));
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
                    //Line(new Vector2(0, y), new Vector2(_virtualResolutionX, y), Color.White, _spriteBatch);
                }

                for (var x = 0; x < _virtualResolutionX; x += _cellSize)
                {
                    //Line(new Vector2(x, 0), new Vector2(x, _virtualResolutionY), Color.White, _spriteBatch);
                }

                for (int y = 0; y < 24; y++)
                {
                    for (int x = 0; x < 24; x++)
                    {
                        if (_map[x, y] == 0) { continue; }
                        Color c;
                        switch (_map[x, y])
                        {
                            case 1: c = Color.Red; break; //red
                            case 2: c = Color.Green; break; //green
                            case 3: c = Color.Blue; break; //blue
                            case 4: c = Color.White; break; //white
                            default: c = Color.Yellow; break; //yellow
                        }
                        _spriteBatch.Draw(_pixel, new Vector2(x, y) * _cellSize, null, c, 0f, new Vector2(0f, 0f), _cellSize, SpriteEffects.None, 1f);
                        
                        if (_textureEnabled)
                        {

                            int screenX = x * _cellSize;
                            int screenY = y * _cellSize;

                            _spriteBatch.Draw(_textures[_map[x, y] - 1], new Rectangle(screenX, screenY, _cellSize, _cellSize), Color.White);
                        }
                    }
                }
            }
            //

            DrawPlayer(_player, Color.Red, 10, _spriteBatch);
            DrawPlayerDirection(_player, Color.Red, 2, 2, _spriteBatch);
            DrawResults(Color.CornflowerBlue, 2, _spriteBatch);
            DrawPlayerFOV(_player, Color.Yellow, 2, 2, _spriteBatch);
           

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            //_spriteBatch.Draw(_pixel, new Rectangle(0, GraphicsDevice.Viewport.Height / 2, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2), Color.White);
            int iteration = 0;
            foreach (var result in _iterationResults)
            {

                var length = result.Length * Math.Cos(result.Rotation + result.Angle - _player.Rotation);
                
                int height = GraphicsDevice.Viewport.Height;
                //Calculate height of line to draw on screen
                int lineHeight = (int)(height / length);

                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + height / 2;
                //if (drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + height / 2;
                //if (drawEnd >= height) drawEnd = height - 1;


                Color c;
                switch (result.Id)
                {
                    case 1: c = Color.Red; break; //red
                    case 2: c = Color.Green; break; //green
                    case 3: c = Color.Blue; break; //blue
                    case 4: c = Color.White; break; //white
                    default: c = Color.Yellow; break; //yellow
                }

                if (result.Side == 1)
                {
                    switch (result.Id)
                    {
                        case 1: c = Color.DarkRed; break; //red
                        case 2: c = Color.DarkGreen; break; //green
                        case 3: c = Color.DarkBlue; break; //blue
                        case 4: c = Color.DarkGray; break; //white
                        default: c = Color.DarkGoldenrod; break; //yellow
                    }
                }
                if (result.Hit == 1)
                {
                    var alpha = 1 - (result.Length / 25f);
                    if (!_textureEnabled)
                    {
                        Line(new Vector2(iteration, drawStart), new Vector2(iteration, drawEnd), c * (alpha), _rayResolution, _spriteBatch);
                    }
                    else
                    {

                        int screenX = iteration;
                        int screenY = drawStart;

                        int sliceWidth = _rayResolution;
                        int sliceHeight = drawEnd - drawStart;

                        Rectangle sliceScreen = new Rectangle(screenX, screenY, sliceWidth, sliceHeight);

                        int textureID = result.Id - 1;
                        Texture2D texture = _textures[textureID];
                        
                        if (result.Side == 1)
                        {
                            _spriteBatch.Draw(texture, sliceScreen, new Rectangle((int)(texture.Width * ((result.To.X) - (int)(result.To.X))), 0, 1, texture.Height), Color.White * (alpha / 2));
                        }
                        if (result.Side == 0)
                        {
                            _spriteBatch.Draw(texture, sliceScreen, new Rectangle((int)(texture.Width * ((result.To.Y) - (int)(result.To.Y))), 0, 1, texture.Height), Color.White * (alpha / 1));
                        }

                        //var rect = new Rectangle((int)(texture.Width * ((result.To.X) - (int)(result.To.X))), 0, 1, texture.Height);
                        //_spriteBatch.Draw(texture, sliceScreen, rect, Color.White * (alpha / 2));

                    }
                }
                iteration += _rayResolution;
            }

            _spriteBatch.Draw(_virtualScreen, new Rectangle(0, 0, 256, 256), Color.White);
            //_spriteBatch.Draw(_texture, new Rectangle(0, 0, 256, 256), Color.White);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {

                _spriteBatch.Draw(_virtualScreen, GraphicsDevice.Viewport.Bounds, Color.White);

            }

          
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private bool IsSolid(float x, float y)
        {
            int gridX = (int)(x);
            int gridY = (int)(y);

            if (gridX >= _map.GetLength(0) || gridX < 0)
            {
                return false;
            }
            if (gridY >= _map.GetLength(1) || gridY < 0)
            {
                return false;
            }

            bool solid = _map[gridX, gridY] != 0;
            return solid;
        }

        private int GetSolid(float x, float y)
        {
            int gridX = (int)(x);
            int gridY = (int)(y);

            if (gridX >= _map.GetLength(0) || gridX < 0)
            {
                return 0;
            }
            if (gridY >= _map.GetLength(1) || gridY < 0)
            {
                return 0;
            }

            return _map[gridX, gridY];
        }

        public void DrawPlayer(Player player, Color color, int scale, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_pixel, player.Position * _cellSize, null, color, 0f, new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 1f);
        }

        public void DrawPlayerDirection(Player player, Color color, float thickness, float length, SpriteBatch spriteBatch)
        {
            LineAngle(player.Position * _cellSize, _player.Rotation, length * _cellSize, color, thickness, spriteBatch);
        }

        public void DrawPlayerFOV(Player player, Color color, float thickness, float length, SpriteBatch spriteBatch)
        {
            for (var i = -MathHelper.ToRadians(_fov / 2); i < MathHelper.ToRadians(_fov / 2); i += MathHelper.ToRadians(_fov) / GraphicsDevice.Viewport.Width)
            {
                var position = player.Position;
                var rotation = player.Rotation;
                LineAngle(position * _cellSize, rotation + i, length * _cellSize, color, thickness, spriteBatch);
            }
        }

        public void DrawResults(Color color, float thickness, SpriteBatch spriteBatch)
        {
            foreach (var result in _iterationResults)
            {
                float length = (float)(result.Length * (!_fishEye ? Math.Cos(result.Angle) : 1));

                LineAngle(result.From * _cellSize, result.Rotation, length * _cellSize, color, thickness, spriteBatch);
                //spriteBatch.Draw(_pixel, result.End * _cellSize, Color.Yellow);
            }

            foreach (var result in _iterationResults)
            {
                float length = (float)(result.Length * (!_fishEye ? Math.Cos(result.Angle) : 1));

                //LineAngle(result.Start * _cellSize, result.Rotation, length * _cellSize, color, thickness, spriteBatch);

                
                //_spriteBatch.Draw(_pixel, new Vector2(x, y) * _cellSize, null, c, 0f, new Vector2(0f, 0f), _cellSize, SpriteEffects.None, 1f);


                //spriteBatch.Draw(_pixel, result.End * _cellSize, c);
            }
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

        public Color Increase(Color color, byte value)
        {
            if (color.R >= color.G && color.R >= color.B)
                color.R += value;
            else if (color.G >= color.R && color.G >= color.B)
                color.G += value;
            else
                color.B += value;
            return color;
        }

        public Color Decrease(Color color, byte value)
        {
            if (color.R <= color.G && color.R <= color.B)
                color.R -= value;
            else if (color.G <= color.R && color.G <= color.B)
                color.G -= value;
            else
                color.B -= value;
            return color;
        }

    }
}