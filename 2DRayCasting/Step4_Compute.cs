using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace RayCasting
{
    public class Step4_Compute : Game
    {
        private Stopwatch _debugUpdate;
        private Stopwatch _debugDraw;

        private Texture2D _pixel;
        private Texture2D _texture;
        private SpriteFont _font;

        private Effect _effect;
        const int ComputeGroupSize = 1;
        private StructuredBuffer _iterationResultBuffer;
        private int _maxCount = 50000;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int[] _map;
        private int _cellSize = 20;
        private Player _player;

        private List<IterationResult2D> _iterationResults;

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 480;
        private const int _virtualResolutionY = 480;

        const int ResolutionX = 720;
        const int ResolutionY = 720;
        private float _fov = 70;
        private int _rayResolution = 1;
        private bool _fishEye = true;

        public Step4_Compute()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {

            _debugDraw = new Stopwatch();
            _debugUpdate = new Stopwatch();

            _graphics.PreferredBackBufferWidth = ResolutionX;
            _graphics.PreferredBackBufferHeight = ResolutionY;
            _graphics.ApplyChanges();

            _map = new int[24 * 24]
            {
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1,
                1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1,
                1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
                1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
            };

            _iterationResults = new List<IterationResult2D>();

            _player = new Player();
            _player.Position = new Vector2(3, 3);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _virtualScreen = new RenderTarget2D(GraphicsDevice, _virtualResolutionX, _virtualResolutionY);

            _texture = Content.Load<Texture2D>("cobblestone");
            _effect = Content.Load<Effect>("Effect");
            _font = Content.Load<SpriteFont>("Font");

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _iterationResultBuffer = new StructuredBuffer(GraphicsDevice, typeof(IterationResult2D), _maxCount, BufferUsage.None, ShaderAccess.ReadWrite);
        }

        protected override void Update(GameTime gameTime)
        {
            _debugUpdate.Restart();
            _player.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                _fov--;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                _fov++;
            }

            _fov = Math.Clamp( _fov, 0, 360);

            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                _fishEye = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                _fishEye = false;
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

            base.Update(gameTime);
            _debugUpdate.Stop();
        }

        protected override void Draw(GameTime gameTime)
        {
            _debugDraw.Restart();
            ComputeRays();
            
            var results = new IterationResult2D[GraphicsDevice.Viewport.Width];
            _iterationResultBuffer.GetData(results, 0, GraphicsDevice.Viewport.Width);

            _iterationResults.Clear();
            _iterationResults.AddRange(results);

            GraphicsDevice.SetRenderTarget(_virtualScreen);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawMap(_spriteBatch);
            DrawPlayer(_player, Color.Red, 10, _spriteBatch);
            DrawPlayerDirection(_player, Color.Red, 2, 2, _spriteBatch);
            DrawResults(Color.CornflowerBlue, 2, _spriteBatch);
            DrawPlayerFOV(_player, Color.Red, 2, 2, _spriteBatch);
           

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            
            int iteration = 0;
            foreach (var result in _iterationResults)
            {

                var length = result.Length * (!_fishEye ? Math.Cos(-result.RotationDifference) : 1);
                
                int height = GraphicsDevice.Viewport.Height;
                //Calculate height of line to draw on screen
                int lineHeight = (int)(height / length);

                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + height / 2;
                if (drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + height / 2;
                if (drawEnd >= height) drawEnd = height - 1;


                Color c;
                switch (result.ID)
                {
                    case 1: c = Color.Red; break; //red
                    case 2: c = Color.Green; break; //green
                    case 3: c = Color.Blue; break; //blue
                    case 4: c = Color.White; break; //white
                    default: c = Color.Yellow; break; //yellow
                }

                if (result.Side == 1)
                {
                    switch (result.ID)
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
                    Line(new Vector2(iteration, drawStart), new Vector2(iteration, drawEnd), c * (alpha), _rayResolution, _spriteBatch);
                }
                iteration += _rayResolution;
            }

            _spriteBatch.Draw(_virtualScreen, new Rectangle(0, 0, 256, 256), Color.White);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _spriteBatch.Draw(_virtualScreen, GraphicsDevice.Viewport.Bounds, Color.White);
            }

            _spriteBatch.DrawString(_font, _debugUpdate.Elapsed.TotalMilliseconds + "", Vector2.Zero, Color.White);
            _spriteBatch.End();
            
            base.Draw(gameTime);
            _debugDraw.Stop();
        }

        private bool IsSolid(float x, float y)
        {
            int gridX = (int)(x);
            int gridY = (int)(y);

            if (gridX >= 24 || gridX < 0)
            {
                return false;
            }
            if (gridY >= 24 || gridY < 0)
            {
                return false;
            }

            bool solid = _map[gridX * 24 + gridY] != 0;
            return solid;
        }

        public void DrawMap(SpriteBatch spriteBatch)
        {
            //Map
            if (true)
            {
                for (int y = 0; y < 24; y++)
                {
                    for (int x = 0; x < 24; x++)
                    {
                        if (_map[x * 24 + y] == 0) { continue; }
                        Color c;
                        switch (_map[x * 24 + y])
                        {
                            case 1: c = Color.Red; break; //red
                            case 2: c = Color.Green; break; //green
                            case 3: c = Color.Blue; break; //blue
                            case 4: c = Color.White; break; //white
                            default: c = Color.Yellow; break; //yellow
                        }
                        spriteBatch.Draw(_pixel, new Vector2(x, y) * _cellSize, null, c, 0f, new Vector2(0f, 0f), _cellSize, SpriteEffects.None, 1f);
                    }
                }
            }
            //
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
                float length = (float)(result.Length * (!_fishEye ? Math.Cos(result.RotationDifference) : 1));

                LineAngle(result.Start * _cellSize, result.Rotation, length * _cellSize, color, thickness, spriteBatch);
            }

            foreach (var result in _iterationResults)
            {
                if (result.Hit == 1)
                {
                    float length = (float)(result.Length * (!_fishEye ? Math.Cos(result.RotationDifference) : 1));
                    spriteBatch.Draw(_pixel, result.End * _cellSize, Color.Yellow);
                }
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

        private void ComputeRays()
        {
            _effect.Parameters["Position"].SetValue(_player.Position);
            _effect.Parameters["Rotation"].SetValue(_player.Rotation);
            _effect.Parameters["FOV"].SetValue(_fov);
            _effect.Parameters["ResX"].SetValue(GraphicsDevice.Viewport.Width);
            _effect.Parameters["CPUMap"].SetValue(_map);

            _effect.Parameters["Results"].SetValue(_iterationResultBuffer);

            int groupCount = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Width / ComputeGroupSize);

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.ApplyCompute();
                GraphicsDevice.DispatchCompute(groupCount, 1, 1);
            }
        }

    }
}