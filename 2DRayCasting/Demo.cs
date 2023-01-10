using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace RayCasting
{

    public class Demo : Game
    {
        private Texture2D _pixel;
        private SpriteFont _font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int[,] _map;
        private List<string> _debug;
        private List<float> _angles;

        private Vector2 _pointA = new Vector2(16, 16); //left

        private int _cellSize = 20; //pixels per unit
        private float _angle; //radiant

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 360;
        private const int _virtualResolutionY = 360;

        const int ResolutionX = 720;
        const int ResolutionY = 720;

        public Demo()
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
            _debug = new List<string>();
            _angles = new List<float>();

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
            var mouse = Mouse.GetState();

            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Left))
            {
                _angle -= MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                _angle += MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            var direction = Vector2.Transform(new Vector2(1, 0), Matrix.CreateRotationZ(_angle));

            if (keyboard.IsKeyDown(Keys.Up))
            {
                var next = _pointA + direction * (1 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                
                _pointA += direction * (1 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            _debug.Add($"{_angle}");
            _debug.Add($"{MathHelper.ToDegrees(_angle)}");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_virtualScreen);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //Map
            if (false)
            {
                for (var y = 0; y < _graphics.GraphicsDevice.Viewport.Height; y += _cellSize)
                {
                    Line(new Vector2(0, y), new Vector2(_graphics.GraphicsDevice.Viewport.Width, y), Color.White, _spriteBatch);
                }

                for (var x = 0; x < _graphics.GraphicsDevice.Viewport.Width; x += _cellSize)
                {
                    Line(new Vector2(x, 0), new Vector2(x, _graphics.GraphicsDevice.Viewport.Height), Color.White, _spriteBatch);
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
            double planeX = 0;
            double planeY = 0.66;
            int w = GraphicsDevice.Viewport.Width;
            int h = GraphicsDevice.Viewport.Height;
            _angles.Clear();
            for (int x = 0; x < w; x++)
            {
                var loopAngle = _angle - MathHelper.ToRadians(45) + MathHelper.ToRadians(x);
                var dirX = Math.Cos(loopAngle);
                var dirY = Math.Sin(loopAngle);
                //double rayAngle = (float)Math.Atan2(dirX, dirY);

                //calculate ray position and direction
                double cameraX = 2 * x / (double)GraphicsDevice.Viewport.Width - 1; //x-coordinate in camera space

                double rayDirX = dirX;// + planeX * cameraX;
                double rayDirY = dirY;// + planeY * cameraX;
                
                double rayAngle = (float)Math.Atan2(rayDirY, rayDirX);

                var deltaDistX = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
                var deltaDistY = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));

                double sideDistX;
                double sideDistY;
                double perpWallDist;

                float posX = _pointA.X;
                float posY = _pointA.Y;
                int mapX = (int)_pointA.X;
                int mapY = (int)_pointA.Y;

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
                    sideDistX = (mapX + 1.0 - posX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (posY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - posY) * deltaDistY;
                }

                int hit = 0; //was there a wall hit?
                int side = 0; //was a NS or a EW wall hit?

                int maxDistance = 100;
                int distance = 0;

                while (hit == 0 && maxDistance > distance)
                {
                    //jump to next map square, either in x-direction, or in y-direction
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }
                    //Check if ray has hit a wall
                    if (mapX >= 0 && mapX < 24 && mapY >= 0 && mapY < 24)
                    {
                        if (_map[mapX, mapY] > 0) hit = 1;
                    }
                    distance++;
                    //Console.WriteLine("loop");
                }

                if (side == 0)
                {
                    perpWallDist = (sideDistX - deltaDistX);
                }
                else
                {
                    perpWallDist = (sideDistY - deltaDistY);
                }

                //double angleDiffX = _angle - rayAngle;
                //perpWallDist = perpWallDist * Math.Cos(angleDiffX);

                //Calculate height of line to draw on screen
                int lineHeight = (int)(h / perpWallDist);

                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + h / 2;
                if (drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + h / 2;
                if (drawEnd >= h) drawEnd = h - 1;
                
                Color c = Color.DarkGreen;

                LineAngle(_pointA * _cellSize, (float)Math.Atan2(rayDirY, rayDirX), (float)perpWallDist * _cellSize, Color.Red, _spriteBatch);

                _spriteBatch.Draw(_pixel, (_pointA + new Vector2((float)rayDirX, (float)rayDirY) * (float)perpWallDist) * _cellSize, null, Color.Black, 0f, new Vector2(0.5f, 0.5f), 10f, SpriteEffects.None, 1f);
                _spriteBatch.Draw(_pixel, (_pointA + new Vector2((float)rayDirX, (float)rayDirY) * (float)perpWallDist) * _cellSize, null, Color.Goldenrod, 0f, new Vector2(0.5f, 0.5f), 5f, SpriteEffects.None, 1f);

                if (side == 1) { c = Color.DarkOrange; }
                if (hit == 1)
                {
                    Line(new Vector2(x, drawStart), new Vector2(x, drawEnd), c * (  1 - (distance / 25f)), _spriteBatch);
                }
            }
            
            
            //_spriteBatch.Draw(_pixel, _pointA * _cellSize, null, Color.Green, 0f, new Vector2(0.5f, 0.5f), 5f, SpriteEffects.None, 1f);
            
            //LineAngle(_pointA * _cellSize, _angle, 100, Color.Red, _spriteBatch);
            
            for (var i = 0; i < _angles.Count; i++)
            {
                var angle = _angles[i];
                //LineAngle(_pointA * _cellSize, angle, 100, Color.Blue, _spriteBatch);
            }
            if (true)
            {
                for (var i = 0; i < _debug.Count; i++)
                {
                    var line = _debug[i];
                    var arr = line.ToCharArray();
                    for (var j = 0; j < arr.Count(); j++)
                    {
                        if (!_font.Characters.Contains(arr[j]))
                        {
                            arr[j] = 'i';
                        }
                    }
                    line = new string(arr);
                    _spriteBatch.Draw(_pixel, new Rectangle(new Vector2(0, _font.LineSpacing * i).ToPoint(), _font.MeasureString(line).ToPoint()), Color.Black * 0.5f);
                    _spriteBatch.DrawString(_font, line, new Vector2(0, _font.LineSpacing * i), Color.White * 0.5f);
                }
            }

            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(_virtualScreen, new Rectangle(0, 0, ResolutionX, ResolutionY), Color.White);
            _spriteBatch.Draw(_pixel, new Rectangle(0, 0, ResolutionX, ResolutionY).Center.ToVector2(), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
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