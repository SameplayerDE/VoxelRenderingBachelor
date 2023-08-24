using InputLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OptiLib;
using RenderLib;
using System;

namespace QuadTreeDDA
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;

        private Random _random;
        private RegionQuadTree<byte> _regionQuadTree;

        private StupidTree _stupidTree;

        //Player
        private Vector2 _position;
        private Vector2 _direction;
        private float _rotation;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(16);

            _random = new Random();

            _stupidTree = new StupidTree(0, 0, 10);

            _regionQuadTree = new RegionQuadTree<byte>(0, 0, 1024);

            var noiseGenerator = new FastNoiseLite();

            for (int y = 96; y < 1024 - 96; y++)
            {
                for (int x = 96; x < 1024 - 96; x++)
                {
                    byte rndV = ((byte)(Math.Min(0 ,noiseGenerator.GetNoise(x / 1.5f, y / 1.5f)) * byte.MaxValue));
                    
                    if (rndV != 0)
                    {
                        _regionQuadTree.Insert(x, y, rndV);
                    }
                }
            }

            for (int y = 0; y < StupidLeaf.Size * _stupidTree.S; y++)
            {
                for (int x = 0; x < StupidLeaf.Size * _stupidTree.S; x++)
                {
                    byte rndV = ((byte)(Math.Min(0, noiseGenerator.GetNoise(x / 2f, y / 2f)) * byte.MaxValue));

                    if (rndV != 0)
                    {
                        _stupidTree.Insert(x, y, 1);
                    }
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardHandler.Update(gameTime);
            MouseHandler.Update(gameTime);

            if (KeyboardHandler.IsKeyDownOnce(Keys.Enter))
            {
                byte rndV = (byte)_random.Next(byte.MinValue, byte.MaxValue);
                float rndX = _random.NextSingle() * _regionQuadTree.Bounds.Width - 1;
                float rndY = _random.NextSingle() * _regionQuadTree.Bounds.Height - 1;
                _regionQuadTree.Insert(rndX, rndY, rndV);
                _stupidTree.Insert((int)rndX, (int)rndY, rndV);
            }

            if (MouseHandler.IsLeftDown())
            {
                byte rndV = (byte)_random.Next(byte.MinValue, byte.MaxValue);
                float x = MouseHandler.CurrPosition.X;
                float y = MouseHandler.CurrPosition.Y;
                _regionQuadTree.Insert(x, y, rndV);
            }

            if (KeyboardHandler.IsKeyDown(Keys.Left))
            {
                _rotation -= MathHelper.PiOver2 * delta;
            }
            if (KeyboardHandler.IsKeyDown(Keys.Right))
            {
                _rotation += MathHelper.PiOver2 * delta;
            }

            if (_rotation >= MathHelper.TwoPi)
            {
                _rotation -= MathHelper.TwoPi;
            }
            if (_rotation < 0)
            {
                _rotation += MathHelper.TwoPi;
            }
            _direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));
            if (_direction.Length() != 0)
            {
                _direction.Normalize();
                _direction *= 100f;
            }
            if (KeyboardHandler.IsKeyDown(Keys.Up))
            {
                _position += _direction * delta;
            }
            if (KeyboardHandler.IsKeyDown(Keys.Down))
            {
                _position -= _direction * delta;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var result = DDACalculator.RunIteration2D(_position.X, _position.Y, _rotation, _stupidTree);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            //_spriteBatch.DrawRegionTree(_pixel, _regionQuadTree, Color.Red);
            _spriteBatch.LineAngle(_pixel, _position, _rotation, result.SideDistY, Color.Red);
            _spriteBatch.LineAngle(_pixel, _position, _rotation, result.SideDistX, Color.Green);
            _spriteBatch.LineAngle(_pixel, _position, _rotation, result.Length, result.Hit == 1 ? Color.Yellow : Color.Lime);

            _spriteBatch.DrawStupidTree(_pixel, _stupidTree, Color.DarkGreen);

            _spriteBatch.End();

            base.Draw(gameTime);
        }


    }
}