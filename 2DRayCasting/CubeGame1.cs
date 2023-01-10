using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{
    public class CubeGame1 : Game
    {
        //Player
        private Vector3 _position;
        private Vector3 _rotation;
        private float _movementSpeed = 1.0f;

        //Game
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        //Resources
        private Effect _effect;
        private SpriteFont _font;
        private Texture2D _pixel;

        //Data
        private int[,,] _map;
        private VertexBuffer _vertexBuffer;

        private Matrix _world = Matrix.CreateTranslation(0, 0, 0);
        private Matrix _view = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        private Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

        private List<string> _debug = new List<string>();

        public CubeGame1()
        {
            Content.RootDirectory = "Content";
            
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            _graphicsDeviceManager.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font");
            _effect = Content.Load<Effect>("vpc");
            
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _map = new int[2, 10, 10] //z, y, x
            {
                {
                    { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 1, 1, 1, 1, 0, 0, 1 },
                    { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
                    { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
                    { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
                {
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 1, 0, 0, 1, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                }
            };

            VertexPositionColor[] vertices = new VertexPositionColor[6 * 6];

            //Facing Negativ X
            vertices[6 * 0 + 0] = new VertexPositionColor(new Vector3(+0, +0, +1), Color.White);
            vertices[6 * 0 + 1] = new VertexPositionColor(new Vector3(+0, -1, +1), Color.White);
            vertices[6 * 0 + 2] = new VertexPositionColor(new Vector3(+0, -1, +0), Color.White);

            vertices[6 * 0 + 3] = new VertexPositionColor(new Vector3(+0, -1, +0), Color.Gray);
            vertices[6 * 0 + 4] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.Gray);
            vertices[6 * 0 + 5] = new VertexPositionColor(new Vector3(+0, +0, +1), Color.Gray);

            //Facing Negativ Y
            vertices[6 * 1 + 0] = new VertexPositionColor(new Vector3(+0, -1, +1), Color.Red);
            vertices[6 * 1 + 1] = new VertexPositionColor(new Vector3(+1, -1, +1), Color.Red);
            vertices[6 * 1 + 2] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.Red);

            vertices[6 * 1 + 3] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.DarkRed);
            vertices[6 * 1 + 4] = new VertexPositionColor(new Vector3(+0, -1, +0), Color.DarkRed);
            vertices[6 * 1 + 5] = new VertexPositionColor(new Vector3(+0, -1, +1), Color.DarkRed);

            //Facing Positiv X
            vertices[6 * 2 + 0] = new VertexPositionColor(new Vector3(+1, -1, +1), Color.Blue);
            vertices[6 * 2 + 1] = new VertexPositionColor(new Vector3(+1, +0, +1), Color.Blue);
            vertices[6 * 2 + 2] = new VertexPositionColor(new Vector3(+1, +0, +0), Color.Blue);
                                                                       
            vertices[6 * 2 + 3] = new VertexPositionColor(new Vector3(+1, +0, +0), Color.DarkBlue);
            vertices[6 * 2 + 4] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.DarkBlue);
            vertices[6 * 2 + 5] = new VertexPositionColor(new Vector3(+1, -1, +1), Color.DarkBlue);

            //Facing Positiv Y
            vertices[6 * 3 + 0] = new VertexPositionColor(new Vector3(+1, +0, +1), Color.Yellow);
            vertices[6 * 3 + 1] = new VertexPositionColor(new Vector3(+0, +0, +1), Color.Yellow);
            vertices[6 * 3 + 2] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.Yellow);
                                                                          
            vertices[6 * 3 + 3] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.DarkGoldenrod);
            vertices[6 * 3 + 4] = new VertexPositionColor(new Vector3(+1, +0, +0), Color.DarkGoldenrod);
            vertices[6 * 3 + 5] = new VertexPositionColor(new Vector3(+1, +0, +1), Color.DarkGoldenrod);

            //Facing Positiv Z
            vertices[6 * 4 + 0] = new VertexPositionColor(new Vector3(+1, +0, +1), Color.Green);
            vertices[6 * 4 + 1] = new VertexPositionColor(new Vector3(+1, -1, +1), Color.Green);
            vertices[6 * 4 + 2] = new VertexPositionColor(new Vector3(+0, -1, +1), Color.Green);
                                                                               
            vertices[6 * 4 + 3] = new VertexPositionColor(new Vector3(+0, -1, +1), Color.DarkGreen);
            vertices[6 * 4 + 4] = new VertexPositionColor(new Vector3(+0, +0, +1), Color.DarkGreen);
            vertices[6 * 4 + 5] = new VertexPositionColor(new Vector3(+1, +0, +1), Color.DarkGreen);

            //Facing Negativ Z
            vertices[6 * 5 + 0] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.Purple);
            vertices[6 * 5 + 1] = new VertexPositionColor(new Vector3(+0, -1, +0), Color.Purple);
            vertices[6 * 5 + 2] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.Purple);
                                                                               
            vertices[6 * 5 + 3] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.MediumPurple);
            vertices[6 * 5 + 4] = new VertexPositionColor(new Vector3(+1, +0, +0), Color.MediumPurple);
            vertices[6 * 5 + 5] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.MediumPurple);

            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 6 * 6, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboard = Keyboard.GetState();
            
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            var mouse = Mouse.GetState();
            var screenCenter = GraphicsDevice.Viewport.Bounds.Center;
            var mouseDelta = (mouse.Position - screenCenter).ToVector2();

            if (mouseDelta.Length() != 0)
            {
                mouseDelta.Normalize();
            }

            _rotation.Z -= mouseDelta.X * delta;
            _rotation.Y += mouseDelta.Y * delta;

            Mouse.SetPosition(screenCenter.X, screenCenter.Y);

            var n_rotation =
                Matrix.CreateRotationX(_rotation.X) *
                Matrix.CreateRotationY(_rotation.Y) *
                Matrix.CreateRotationZ(_rotation.Z);

            var direction = Vector3.Transform(new Vector3(1, 0, 0), n_rotation);
            if (direction.Length() != 0)
            {
                direction.Normalize();
            }

            var movement = Vector3.Zero;

            if (keyboard.IsKeyDown(Keys.W))
            {
                movement += direction * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                movement -= direction * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                movement += Vector3.Cross(direction, new Vector3(0, 0, 1)) * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                movement -= Vector3.Cross(direction, new Vector3(0, 0, 1)) * _movementSpeed * delta;
            }

            if (movement.Length() != 0)
            {
                var next = _position + movement;

                if (!IsSolid(next))
                {
                    _position = next;
                }
                else
                {
                    if (!IsSolid(next.X, _position.Y, _position.Z))
                    {
                        _position = new Vector3(next.X, _position.Y, _position.Z);
                    }
                    if (!IsSolid(_position.X, next.Y, _position.Z))
                    {
                        _position = new Vector3(_position.X, next.Y, _position.Z);
                    }
                    if (!IsSolid(_position.X, _position.Y, next.Z))
                    {
                        _position = new Vector3(_position.X, _position.Y, next.Z);
                    }
                }
            }

            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.Viewport.AspectRatio, 0.01f, 100f);
            _view = Matrix.CreateLookAt(_position, _position + direction, new Vector3(0, 0, 1));

            _debug.Clear();
            _debug.Add($"{_position}");
            _debug.Add($"{_rotation}");
            _debug.Add($"{direction}");

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);

            /*RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;*/

            
            for (int z = 0; z < _map.GetLength(0); z++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    for (int x = 0; x < _map.GetLength(2); x++)
                    {
                        if (_map[z, y, x] == 0)
                        {
                            continue;
                        }
                        _world = Matrix.CreateTranslation(x, -y, z);
                        _effect.Parameters["WorldViewProjection"].SetValue(_world * _view * _projection);

                        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertexBuffer.VertexCount / 3);
                        }

                    }
                }
            }

            

           /* _spriteBatch.Begin();
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

            _spriteBatch.End();*/

            base.Draw(gameTime);
        }

        private bool IsSolid(float x, float y, float z)
        {
            int gridX = (int)(x);
            int gridY = (int)(y * -1);
            int gridZ = (int)(z);
            
            if (gridZ >= _map.GetLength(0) || gridZ < 0)
            {
                return false;
            }
            if (gridY >= _map.GetLength(1) || gridY < 0)
            {
                return false;
            }
            if (gridX >= _map.GetLength(2) || gridX < 0)
            {
                return false;
            }

            bool solid = _map[gridZ, gridY, gridX] != 0;
            return solid;
        }

        private bool IsSolid(Vector3 position)
        {
            return IsSolid(position.X, position.Y, position.Z);
        }

    }
}
