using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace QuadTreeTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;

        private SpriteFont _font;
        private Texture2D _pixel;
        private Effect _effect;

        private KeyboardState _currKeyboardState;
        private KeyboardState _prevKeyboardState;

        private MouseState _currMouseState;
        private MouseState _prevMouseState;

        private Random _random;

        private QuadTree _quadTree;
        private OctTree _octTree;

        private const int _mapWidth = 2048;
        private const int _mapHeight = 2048;
        private int _details = 7;
        private int[,] _map;

        private Matrix _world = Matrix.CreateTranslation(0, 0, 0);
        private Matrix _view = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        private Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);
        private VertexBuffer _vertexBuffer;
        private VertexBuffer _vertexBoundsBuffer;

        //Player
        private Vector3 _position = new Vector3(16, 16, 16);
        private Vector3 _rotation;
        private Vector3 _direction;
        private float _movementSpeed = 10.0f;

        private bool _mouseControl = false;

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
            _quadTree = new QuadTree(0, 0, _mapWidth);
            _octTree = new OctTree(0, 0, 0, 8);

            _map = new int[_mapHeight, _mapWidth];

            //var noiseGenerator = new FastNoiseLite();
            //noiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            //
            //for (int y = 0; y < _mapHeight; y++)
            //{
            //    for (int x = 0; x < _mapWidth; x++)
            //    {
            //        var noiseValue = Math.Max(noiseGenerator.GetNoise(x, y), 0);
            //        if (noiseValue > 0f)
            //        {
            //            _map[y, x] = 1;
            //            _quadTree.Insert(x, y, 1);
            //        }
            //        else
            //        {
            //            _map[y, x] = 0;
            //        }
            //    }
            //}

            Console.WriteLine($"Start");

            //float step = 0.05f;
            //float xxx = 4;
            //for (float y = 0; y < xxx; y += step)
            //{
            //    for (float z = 0; z < xxx; z += step)
            //    {
            //        for (float x = 0; x < xxx; x += step)
            //        {
            //            var noiseValue = Math.Max(noiseGenerator.GetNoise(x * 10, y * 10, z * 10), 0);
            //            if (noiseValue > 0f)
            //            {
            //                DrawCube
            //                //_octTree.Insert(x, y, z, 1);
            //            }
            //        }
            //    }
            //
            //    Console.WriteLine($"{y} / 8");
            //}

            Console.WriteLine($"Ende");

            var result = DDACalculator.RunIteration2D(0, 0, 0, _quadTree);

            VertexPositionColor[] vertices = new VertexPositionColor[6 * 6];

            float val = 1f;

            //Facing Negativ X
            vertices[6 * 0 + 0] = new VertexPositionColor(new Vector3(+0, +0, +val), Color.White);
            vertices[6 * 0 + 1] = new VertexPositionColor(new Vector3(+0, -val, +val), Color.White);
            vertices[6 * 0 + 2] = new VertexPositionColor(new Vector3(+0, -val, +0), Color.White);

            vertices[6 * 0 + 3] = new VertexPositionColor(new Vector3(+0, -val, +0), Color.White);
            vertices[6 * 0 + 4] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.White);
            vertices[6 * 0 + 5] = new VertexPositionColor(new Vector3(+0, +0, +val), Color.White);

            //Facing Negativ Y
            vertices[6 * 1 + 0] = new VertexPositionColor(new Vector3(+0, -val, +val), Color.Red);
            vertices[6 * 1 + 1] = new VertexPositionColor(new Vector3(+val, -val, +val), Color.Red);
            vertices[6 * 1 + 2] = new VertexPositionColor(new Vector3(+val, -val, +0), Color.Red);

            vertices[6 * 1 + 3] = new VertexPositionColor(new Vector3(+val, -val, +0), Color.Red);
            vertices[6 * 1 + 4] = new VertexPositionColor(new Vector3(+0, -val, +0), Color.Red);
            vertices[6 * 1 + 5] = new VertexPositionColor(new Vector3(+0, -val, +val), Color.Red);

            //Facing Positiv X
            vertices[6 * 2 + 0] = new VertexPositionColor(new Vector3(+val, -val, +val), Color.Blue);
            vertices[6 * 2 + 1] = new VertexPositionColor(new Vector3(+val, +0, +val), Color.Blue);
            vertices[6 * 2 + 2] = new VertexPositionColor(new Vector3(+val, +0, +0), Color.Blue);

            vertices[6 * 2 + 3] = new VertexPositionColor(new Vector3(+val, +0, +0), Color.Blue);
            vertices[6 * 2 + 4] = new VertexPositionColor(new Vector3(+val, -val, +0), Color.Blue);
            vertices[6 * 2 + 5] = new VertexPositionColor(new Vector3(+val, -val, +val), Color.Blue);

            //Facing Positiv Y
            vertices[6 * 3 + 0] = new VertexPositionColor(new Vector3(+val, +0, +val), Color.Yellow);
            vertices[6 * 3 + 1] = new VertexPositionColor(new Vector3(+0, +0, +val), Color.Yellow);
            vertices[6 * 3 + 2] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.Yellow);

            vertices[6 * 3 + 3] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.Yellow);
            vertices[6 * 3 + 4] = new VertexPositionColor(new Vector3(+val, +0, +0), Color.Yellow);
            vertices[6 * 3 + 5] = new VertexPositionColor(new Vector3(+val, +0, +val), Color.Yellow);

            //Facing Positiv Z
            vertices[6 * 4 + 0] = new VertexPositionColor(new Vector3(+val, +0, +val), Color.Green);
            vertices[6 * 4 + 1] = new VertexPositionColor(new Vector3(+val, -val, +val), Color.Green);
            vertices[6 * 4 + 2] = new VertexPositionColor(new Vector3(+0, -val, +val), Color.Green);

            vertices[6 * 4 + 3] = new VertexPositionColor(new Vector3(+0, -val, +val), Color.Green);
            vertices[6 * 4 + 4] = new VertexPositionColor(new Vector3(+0, +0, +val), Color.Green);
            vertices[6 * 4 + 5] = new VertexPositionColor(new Vector3(+val, +0, +val), Color.Green);

            //Facing Negativ Z
            vertices[6 * 5 + 0] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.Purple);
            vertices[6 * 5 + 1] = new VertexPositionColor(new Vector3(+0, -val, +0), Color.Purple);
            vertices[6 * 5 + 2] = new VertexPositionColor(new Vector3(+val, -val, +0), Color.Purple);

            vertices[6 * 5 + 3] = new VertexPositionColor(new Vector3(+val,  -val, +0), Color.Purple);
            vertices[6 * 5 + 4] = new VertexPositionColor(new Vector3(+val, +0, +0), Color.Purple);
            vertices[6 * 5 + 5] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.Purple);

            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 6 * 6, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(GraphicsDevice, _mapWidth, _mapHeight, false, _graphics.PreferredBackBufferFormat, DepthFormat.Depth24Stencil8);

            _font = Content.Load<SpriteFont>("Font");
            _effect = Content.Load<Effect>("Effect");

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });


        }

        protected override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _prevKeyboardState = _currKeyboardState;
            _prevMouseState = _currMouseState;

            _currKeyboardState = Keyboard.GetState();
            _currMouseState = Mouse.GetState();

            if (_currKeyboardState.IsKeyDown(Keys.E) && _prevKeyboardState.IsKeyUp(Keys.E))
            {
                _mouseControl = !_mouseControl;
            }

            if (_currKeyboardState.IsKeyDown(Keys.Enter) && _prevKeyboardState.IsKeyUp(Keys.Enter))
            {
                var rndX = _random.Next(0, _mapWidth);
                var rndY = _random.Next(0, _mapHeight);
                
                _map[rndY, rndX] = 1;
                _quadTree.Insert(rndX, rndY, 1);

                //var rndX = _random.Next(0, 10);
                //var rndY = _random.Next(0, 10);
                //var rndZ = _random.Next(0, 10);
                //
                //_octTree.Insert(rndX, rndY, rndZ, 1);

            }

            //if (_currKeyboardState.IsKeyDown(Keys.S) && _prevKeyboardState.IsKeyUp(Keys.S))
            //{
            //    Stream stream = File.Create("image_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".png");
            //    _renderTarget.SaveAsPng(stream, _renderTarget.Width, _renderTarget.Height);
            //    stream.Dispose();
            //    _renderTarget.Dispose();
            //
            //    Exit();
            //}
            

            if (_currKeyboardState.IsKeyDown(Keys.P) && _prevKeyboardState.IsKeyUp(Keys.P))
            {
                _details += 1;
            }
            if (_currKeyboardState.IsKeyDown(Keys.M) && _prevKeyboardState.IsKeyUp(Keys.M))
            {
                _details -= 1;
            }
            _details = Math.Clamp(_details, 1, 100);

            if (_currMouseState.LeftButton == ButtonState.Pressed)
            {
                var x = _currMouseState.Position.X;
                var y = _currMouseState.Position.Y;

                for (int ry = 0; ry < 10; ry++)
                {
                    for (int rx = 0; rx < 10; rx++)
                    {
                        x += rx;
                        y += ry;
                        if (x >= 0 && y >= 0 && y < _mapHeight && x < _mapWidth)
                        {

                            _map[y, x] = 1;
                        }
                        _quadTree.Insert(x, y, 1);
                        x -= rx;
                        y -= ry;
                    }
                }

            }

            if (_mouseControl)
            {

                var mouse = Mouse.GetState();
                var screenCenter = GraphicsDevice.Viewport.Bounds.Center;
                var mouseDelta = (mouse.Position - screenCenter).ToVector2();

                if (mouseDelta.Length() != 0)
                {
                    mouseDelta /= 10f;
                    //mouseDelta.Normalize();
                }

                _rotation.Y -= mouseDelta.X * delta;
                _rotation.X += mouseDelta.Y * delta;
                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }

            if (_rotation.Y < 0)
            {
                _rotation.Y += MathHelper.TwoPi;
            }
            else if (_rotation.Y >= MathHelper.TwoPi)
            {
                _rotation.Y -= MathHelper.TwoPi;
            }

            _rotation.X = Math.Clamp(_rotation.X, MathHelper.ToRadians(-80f), MathHelper.ToRadians(80f));

            var n_rotation =
                Matrix.CreateRotationX(_rotation.X) *
                Matrix.CreateRotationY(_rotation.Y) *
                Matrix.CreateRotationZ(_rotation.Z);

            _direction = Vector3.Transform(Vector3.Backward, n_rotation);
            if (_direction.Length() != 0)
            {
                _direction.Normalize();
            }
            var movement = Vector3.Zero;

            if (_currKeyboardState.IsKeyDown(Keys.W))
            {
                movement += _direction * new Vector3(1, 0, 1) * _movementSpeed * delta;
            }
            if (_currKeyboardState.IsKeyDown(Keys.S))
            {
                movement -= _direction * new Vector3(1, 0, 1) * _movementSpeed * delta;
            }
            if (_currKeyboardState.IsKeyDown(Keys.D))
            {
                movement += Vector3.Cross(_direction * new Vector3(1, 0, 1), new Vector3(0, 1, 0)) * _movementSpeed * delta;
            }
            if (_currKeyboardState.IsKeyDown(Keys.A))
            {
                movement -= Vector3.Cross(_direction * new Vector3(1, 0, 1), new Vector3(0, 1, 0)) * _movementSpeed * delta;
            }

            if (_currKeyboardState.IsKeyDown(Keys.Space))
            {
                movement += Vector3.Up * _movementSpeed * delta;
            }
            if (_currKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                movement += Vector3.Down * _movementSpeed * delta;
            }
            if (movement.Length() != 0)
            {
                var next = _position + movement;
                _position = next;
            }

            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1f, 0.01f, 100f);
            _view = Matrix.CreateLookAt(new Vector3(16, 16, 16), new Vector3(4, 4, 4), new Vector3(0, 1, 0));


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
           
           GraphicsDevice.SetRenderTarget(_renderTarget);
           GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin();
            //DrawMap(_spriteBatch);
            //DrawQuadTree(_spriteBatch, _quadTree);
            //DrawQuadTree(_spriteBatch, _quadTree, _details);
            DrawQuadTreeOutLined(_spriteBatch, _quadTree, _details);
            DrawBounds(_octTree.Bounds, Color.Red);
            DrawTree(_octTree, _details);
            _spriteBatch.End();
            //var noiseGenerator = new FastNoiseLite();
            //noiseGenerator.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            //float step = 0.05f;
            //float xxx = 4;
            //for (float y = 0; y < xxx; y += step)
            //{
            //    for (float z = 0; z < xxx; z += step)
            //    {
            //        for (float x = 0; x < xxx; x += step)
            //        {
            //            var noiseValue = Math.Max(noiseGenerator.GetNoise(x * 10, y * 10, z * 10), 0);
            //            if (noiseValue > 0f)
            //            {
            //                DrawCube(x, y, z, step);
            //                //_quadTree.Insert(x, y, 1);
            //            }
            //        }
            //    }
            //}

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //DrawBounds(_octTree.Bounds, Color.Red);
            //DrawTree(_octTree, _details);

            //_spriteBatch.Begin(samplerState: SamplerState.PointClamp, depthStencilState: DepthStencilState.Default);
            ////_spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, 256, 256), Color.White);
            ////_spriteBatch.DrawString(_font, "Rotation: " + _rotation, new Vector2(0, _font.LineSpacing * 0), Color.White);
            ////_spriteBatch.DrawString(_font, "MaxLevel: " + _quadTree.MaxLevel, new Vector2(0, _font.LineSpacing * 0), Color.White);
            ////_spriteBatch.DrawString(_font, "Details: " + _details, new Vector2(0, _font.LineSpacing * 1), Color.White);
            //_spriteBatch.End();

            //instancing.Draw(ref _view, ref _projection, GraphicsDevice);

            //Stream stream = File.Create("image_" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".png");
            //_renderTarget.SaveAsPng(stream, _renderTarget.Width, _renderTarget.Height);
            //stream.Dispose();
            //_renderTarget.Dispose();
            //
            //Exit();

            base.Draw(gameTime);
        }

        protected void DrawMap(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    var value = _map[y, x];
                    if (value != 0)
                    {
                        spriteBatch.Draw(_pixel, new Vector2(x, y), Color.Red);
                    }
                }
            }
        }

        protected void DrawQuadTree(SpriteBatch spriteBatch, QuadTree quadTree, int depth = 10)
        {
            if (!quadTree.IsDivided)
            {
                var rect = new Rectangle();
                rect.X = (int)quadTree.Bounds.X;
                rect.Y = (int)quadTree.Bounds.Y;
                rect.Width = (int)quadTree.Bounds.Width;
                rect.Height = (int)quadTree.Bounds.Height;
                var color = Color.LimeGreen;
                if (depth >= quadTree.Level)
                {
                    var alpha = quadTree.Level / (float)quadTree.MaxLevel;
                    spriteBatch.Draw(_pixel, rect, color * alpha);
                }
                else
                {
                    spriteBatch.Draw(_pixel, rect, color);
                }
            }
            else
            {
                foreach (var division in quadTree.Divisions)
                {
                    DrawQuadTree(spriteBatch, division, depth);
                }
            }
        }

        protected void DrawQuadTreeOutLined(SpriteBatch spriteBatch, QuadTree quadTree, int depth = 10)
        {
            if (!quadTree.IsDivided)
            {
                var rect = new Rectangle();
                rect.X = (int)quadTree.Bounds.X;
                rect.Y = (int)quadTree.Bounds.Y;
                rect.Width = (int)quadTree.Bounds.Width;
                rect.Height = (int)quadTree.Bounds.Height;
                
                if (depth >= quadTree.Level)
                {
                    DrawRectOutLined(spriteBatch, rect, Color.White);
                    if(quadTree.Value != 0)
                    {
                        //spriteBatch.Draw(_pixel, rect, Color.CornflowerBlue);
                    }
                }
            }
            else
            {
                foreach (var division in quadTree.Divisions)
                {
                    DrawQuadTreeOutLined(spriteBatch, division, depth);
                }
            }
        }

        protected void DrawQuadTree3D(QuadTree quadTree, int depth = 10)
        {
            if (!quadTree.IsDivided)
            {

                if (depth >= quadTree.Level)
                {
                    DrawBounds(quadTree.Bounds, Color.White);
                }
            }
            else
            {
                foreach (var division in quadTree.Divisions)
                {
                    DrawQuadTree3D(division, depth);
                }
            }
        }

        protected void DrawTree(OctTree tree, int depth = 0)
        {
            
            if (tree.IsDivided)
            {
                if (depth > tree.Level)
                {
                    foreach (var division in tree.Divisions)
                    {
                        DrawTree(division, depth);
                    }
                }
                else
                {
                    DrawCube(tree.Bounds.X, tree.Bounds.Y, tree.Bounds.Z, tree.Bounds.Width);
                    //if (tree.Value != 0)
                    //{
                    //    
                    //}
                    //else
                    //{
                    //    if (tree.Level <= 5)
                    //    {
                    //
                    //    }
                    //    //DrawBounds(tree.Bounds, Color.White);
                    //}
                }
            }
            else
            {
                if (tree.Values.Count != 0)
                {
                    DrawCube(tree.Bounds.X, tree.Bounds.Y, tree.Bounds.Z, tree.Bounds.Width);
                }
                else
                {
                    if (tree.Level <= 5)
                    {

                    }
                    //DrawBounds(tree.Bounds, Color.White);
                }
            }
        }

        protected void DrawRectOutLined(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            var X = (int)rectangle.X;
            var Y = (int)rectangle.Y;
            var Width = (int)rectangle.Width;
            var Height = (int)rectangle.Height;
            var minX = X;
            var minY = Y;
            var maxX = X + Width;
            var maxY = Y + Height;

            spriteBatch.Draw(_pixel, new Rectangle(minX, minY, Width, 1), color);
            spriteBatch.Draw(_pixel, new Rectangle(maxX - 1, minY, 1, Height), color);
            spriteBatch.Draw(_pixel, new Rectangle(minX, minY, 1, Height), color);
            spriteBatch.Draw(_pixel, new Rectangle(minX, maxY - 1, Width, 1), color);
        }

        protected void DrawBounds(OctTreeBounds bounds, Color color)
        {
            var X = bounds.X;
            var Y = bounds.Y;
            var Z = bounds.Z;
            var Width = bounds.Width;
            var Height = bounds.Height;
            var Depth = bounds.Depth;

            var minX = X;
            var minY = Y;
            var minZ = Z;
            var maxX = X + Width;
            var maxY = Y + Height;
            var maxZ = Z + Depth;

            var list = new List<VertexPositionColor>();
            list.AddRange(GenerateLine(new Vector3(minX, minY, minZ), new Vector3(maxX, minY, minZ)));
            list.AddRange(GenerateLine(new Vector3(maxX, minY, minZ), new Vector3(maxX, maxY, minZ)));
            list.AddRange(GenerateLine(new Vector3(maxX, maxY, minZ), new Vector3(minX, maxY, minZ)));
            list.AddRange(GenerateLine(new Vector3(minX, maxY, minZ), new Vector3(minX, minY, minZ)));

            list.AddRange(GenerateLine(new Vector3(minX, minY, maxZ), new Vector3(maxX, minY, maxZ)));
            list.AddRange(GenerateLine(new Vector3(maxX, minY, maxZ), new Vector3(maxX, maxY, maxZ)));
            list.AddRange(GenerateLine(new Vector3(maxX, maxY, maxZ), new Vector3(minX, maxY, maxZ)));
            list.AddRange(GenerateLine(new Vector3(minX, maxY, maxZ), new Vector3(minX, minY, maxZ)));

            list.AddRange(GenerateLine(new Vector3(minX, minY, minZ), new Vector3(minX, minY, maxZ)));
            list.AddRange(GenerateLine(new Vector3(maxX, minY, minZ), new Vector3(maxX, minY, maxZ)));
            list.AddRange(GenerateLine(new Vector3(minX, maxY, minZ), new Vector3(minX, maxY, maxZ)));
            list.AddRange(GenerateLine(new Vector3(maxX, maxY, minZ), new Vector3(maxX, maxY, maxZ)));

            _world = Matrix.Identity;
            _effect.Parameters["WorldViewProjection"].SetValue(_world * _view * _projection);
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, list.ToArray(), 0, 12);
            }

            //DrawLine(new Vector3(minX, minY, minZ), new Vector3(maxX, minY, minZ));
            //DrawLine(new Vector3(maxX, minY, minZ), new Vector3(maxX, maxY, minZ));
            //DrawLine(new Vector3(maxX, maxY, minZ), new Vector3(minX, maxY, minZ));
            //DrawLine(new Vector3(minX, maxY, minZ), new Vector3(minX, minY, minZ));
            ////
            //DrawLine(new Vector3(minX, minY, maxZ), new Vector3(maxX, minY, maxZ));
            //DrawLine(new Vector3(maxX, minY, maxZ), new Vector3(maxX, maxY, maxZ));
            //DrawLine(new Vector3(maxX, maxY, maxZ), new Vector3(minX, maxY, maxZ));
            //DrawLine(new Vector3(minX, maxY, maxZ), new Vector3(minX, minY, maxZ));
            ////
            //DrawLine(new Vector3(minX, minY, minZ), new Vector3(minX, minY, maxZ));
            //DrawLine(new Vector3(maxX, minY, minZ), new Vector3(maxX, minY, maxZ));
            //DrawLine(new Vector3(minX, maxY, minZ), new Vector3(minX, maxY, maxZ));
            //DrawLine(new Vector3(maxX, maxY, minZ), new Vector3(maxX, maxY, maxZ));
        }

        protected void DrawBounds(QuadTreeBounds bounds, Color color)
        {
            var X = bounds.X;
            var Y = bounds.Y;
            var Z = 0;
            var Width = bounds.Width;
            var Height = bounds.Height;

            var minX = X;
            var minY = Y;
            var maxX = X + Width;
            var maxY = Y + Height;

            DrawLine(new Vector3(minX, minY, Z), new Vector3(maxX, minY, Z));
            DrawLine(new Vector3(maxX, minY, Z), new Vector3(maxX, maxY, Z));
            DrawLine(new Vector3(maxX, maxY, Z), new Vector3(minX, maxY, Z));
            DrawLine(new Vector3(minX, maxY, Z), new Vector3(minX, minY, Z));
        }

        private void DrawLine(Vector3 from, Vector3 to)
        {
            _world = Matrix.Identity;
            _effect.Parameters["WorldViewProjection"].SetValue(_world * _view * _projection);
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, new VertexPositionColor[]
                {
                    new VertexPositionColor(from, Color.Red),
                    new VertexPositionColor(to, Color.Red)
                }, 0, 1);
            }
        }

        private VertexPositionColor[] GenerateLine(Vector3 from, Vector3 to)
        {
            return new VertexPositionColor[]
                {
                    new VertexPositionColor(from, Color.Red),
                    new VertexPositionColor(to, Color.Red)
                };
        }

        private void DrawCube(float x, float y, float z, float scale)
        {
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            _world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(x, y + scale, z);
            _effect.Parameters["WorldViewProjection"].SetValue(_world * _view * _projection);

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertexBuffer.VertexCount / 3);
            }
        }

        private void DrawCube(Vector3 position, float scale)
        {
            DrawCube(position.X, position.Y, position.Z, scale);
        }

    }
}