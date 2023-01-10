using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{
    public class CubeGame3 : Game
    {
        //Player
        private Vector3 _position = new Vector3(2, 1, 2);
        private Vector3 _rotation;
        private Vector3 _direction;
        private float _movementSpeed = 1.0f;

        //Game
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        //Resources
        private Effect _effect;
        private SpriteFont _font;
        private Texture2D _pixel;

        //Data
        private int _cellSize = 10;
        private int[,,] _map;
        private VertexBuffer _vertexBuffer;
        private VertexBuffer _rayBuffer;

        private Matrix _world = Matrix.CreateTranslation(0, 0, 0);
        private Matrix _view = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        private Matrix _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

        private Vector3 _rayCaster = Vector3.Zero;
        private float _rayCasterResolution = 1f;

        private List<string> _debug = new List<string>();
        private List<RayResult> _hits = new List<RayResult>();

        public CubeGame3()
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
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(16);
            
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

            _map = new int[5, 10, 10] //z, y, x
            {
                {
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
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
                },
                {
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
                },
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
                mouseDelta /= 10f;
                //mouseDelta.Normalize();
            }

            _rotation.Y -= mouseDelta.X * delta;
            //_rotation.X += mouseDelta.Y * delta;

            if (_rotation.Y < 0)
            {
                _rotation.Y += MathHelper.TwoPi;
            }
            else if (_rotation.Y >= MathHelper.TwoPi)
            {
                _rotation.Y -= MathHelper.TwoPi;
            }

            _rotation.Z = Math.Clamp(_rotation.Z, MathHelper.ToRadians(-80f), MathHelper.ToRadians(80f));

            if (!keyboard.IsKeyDown(Keys.E))
            {
                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }
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

            if (keyboard.IsKeyDown(Keys.W))
            {
                movement += _direction * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                movement -= _direction * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                movement += Vector3.Cross(_direction, new Vector3(0, 1, 0)) * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                movement -= Vector3.Cross(_direction, new Vector3(0, 1, 0)) * _movementSpeed * delta;
            }

            if (keyboard.IsKeyDown(Keys.Space))
            {
                movement += Vector3.Up * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                movement += Vector3.Down * _movementSpeed * delta;
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
                //_position = next;
            }

            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.Viewport.AspectRatio, 0.01f, 100f);
            _view = Matrix.CreateLookAt(_position, _position + _direction, new Vector3(0, 1, 0));

            _debug.Clear();
            var map = ToGrid(_position);
            _debug.Add($"x: {(int)map.X}, y: {(int)map.Y}, z: {(int)map.Z}");
            _debug.Add($"{_position}");
            _debug.Add($"{_rotation}");
            _debug.Add($"{_direction}");

            if (keyboard.IsKeyDown(Keys.Q))
            {
                _rayCaster = _position;
                _hits.Clear();

                for (float a = 0; a < 360; a += 1 / _rayCasterResolution)
                {
                    var resultXZ = RunIterationXZ(_rayCaster - new Vector3(0, 0.1f, 0), _rotation, new Vector3(MathHelper.ToRadians(0), MathHelper.ToRadians(a), MathHelper.ToRadians(0)));
                    if (resultXZ.Hit == 1)
                    {
                        _hits.Add(resultXZ);
                    }
                    var resultYZ = RunIterationYZ(_rayCaster - new Vector3(0, 0.1f, 0), _rotation, new Vector3(MathHelper.ToRadians(a), MathHelper.ToRadians(0), MathHelper.ToRadians(0)));
                    if (resultYZ.Hit == 1)
                    {
                        _hits.Add(resultYZ);
                    }
                    var resultYX = RunIterationYX(_rayCaster - new Vector3(0, 0.1f, 0), _rotation, new Vector3(MathHelper.ToRadians(0), MathHelper.ToRadians(0), MathHelper.ToRadians(a)));
                    if (resultYX.Hit == 1)
                    {
                        _hits.Add(resultYX);
                    }
                }     
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            /*RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;*/
            
            for (int y = 0; y < _map.GetLength(0); y++)
            {
                for (int z = 0; z < _map.GetLength(1); z++)
                {
                    for (int x = 0; x < _map.GetLength(2); x++)
                    {
                        if (!IsSolid(x, y, z))
                        { 
                            continue;
                        }

                        DrawCube(x, y + 1, z, 1f);

                    }
                }
            }


            foreach (var result in _hits)
            {
                DrawCube(result.To - new Vector3(0.05f, -0.05f, 0.05f), 0.1f, true);
                DrawLine(result.From, result.To);
            }

            var dir = RunIteration(_position, _rotation, Vector3.Zero);
            DrawLine(dir.From - new Vector3(0, 0.01f, 0), dir.To);
            DrawCube(dir.To - new Vector3(0.05f, -0.05f, 0.05f), 0.1f, true);

            _spriteBatch.Begin(depthStencilState: DepthStencilState.Default);

            int mapY = (int)ToGrid(_position).Y;
            for (int z = 0; z < _map.GetLength(1); z++)
            {
                for (int x = 0; x < _map.GetLength(2); x++)
                {
                    if (IsSolid(x, mapY, z))
                    {
                        _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, z * _cellSize, _cellSize, _cellSize), Color.White);
                    }
                    else
                    {
                        _spriteBatch.Draw(_pixel, new Rectangle(x * _cellSize, z * _cellSize, _cellSize, _cellSize), Color.Black);
                    }
                }
            }

            _spriteBatch.Draw(_pixel, new Rectangle((int)_position.X * _cellSize, (int)_position.Z * _cellSize, _cellSize, _cellSize), Color.Red);
            _spriteBatch.Draw(_pixel, new Rectangle((int)(_position.X * _cellSize - _cellSize / 4), (int)(_position.Z * _cellSize - _cellSize / 4), _cellSize / 2, _cellSize / 2), Color.White);

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
                _spriteBatch.Draw(_pixel, new Rectangle(new Vector2(0, 150 + _font.LineSpacing * i).ToPoint(), _font.MeasureString(line).ToPoint()), Color.Black);
                _spriteBatch.DrawString(_font, line, new Vector2(0, 150 + _font.LineSpacing * i), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool IsSolid(float x, float y, float z)
        {
            var map = ToGrid(x, y, z);
            int gridX = (int)map.X;
            int gridY = (int)map.Y;
            int gridZ = (int)map.Z;
            
            if (gridZ >= _map.GetLength(1) || gridZ < 0)
            {
                return false;
            }
            if (gridY >= _map.GetLength(0) || gridY < 0)
            {
                return false;
            }
            if (gridX >= _map.GetLength(2) || gridX < 0)
            {
                return false;
            }

            bool solid = _map[gridY, gridZ, gridX] != 0;
            return solid;
        }

        private bool IsSolid(Vector3 position)
        {
            return IsSolid(position.X, position.Y, position.Z);
        }

        private void DrawCube(float x, float y, float z, float scale, bool red = false)
        {
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            _world = Matrix.CreateScale(scale) * Matrix.CreateTranslation(x, y, z);
            _effect.Parameters["WorldViewProjection"].SetValue(_world * _view * _projection);
            _effect.Parameters["Overwrite"].SetValue(red);

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertexBuffer.VertexCount / 3);
            }
            _effect.Parameters["Overwrite"].SetValue(false);
        }

        private void DrawCube(Vector3 position, float scale, bool red = false)
        {
            DrawCube(position.X, position.Y, position.Z, scale, red);
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
                    new VertexPositionColor(from, Color.CornflowerBlue),
                    new VertexPositionColor(to, Color.Black)
                }, 0, 1);
            }
        }

        private RayResult RunIterationYX(Vector3 position, Vector3 rotation, Vector3 angle)
        {
            return RunIterationYX(position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, angle.X, angle.Y, angle.Z);
        }

        private RayResult RunIterationXZ(Vector3 position, Vector3 rotation, Vector3 angle)
        {
            return RunIterationXZ(position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, angle.X, angle.Y, angle.Z);
        }

        private RayResult RunIterationYZ(Vector3 position, Vector3 rotation, Vector3 angle)
        {
            return RunIterationYZ(position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, angle.X, angle.Y, angle.Z);
        }

        private RayResult RunIteration(Vector3 position, Vector3 rotation, Vector3 angle)
        {
            return RunIteration(position.X, position.Y, position.Z, rotation.X, rotation.Y, rotation.Z, angle.X, angle.Y, angle.Z);
        }

        private RayResult RunIteration(float x, float y, float z, float rotX, float rotY, float rotZ, float diffX, float diffY, float diffZ)
        {

            float posX = x;
            float posZ = z;
            float posY = y;

            var map = ToGrid(posX, posY, posZ);

            int mapX = (int)map.X;
            int mapZ = (int)map.Z;
            int mapY = (int)map.Y;
                       
            var n_rotation =
                Matrix.CreateRotationX(rotX + diffX) *
                Matrix.CreateRotationY(rotY + diffY) *
                Matrix.CreateRotationZ(rotZ + diffZ);

            var rayDir = Vector3.Transform(Vector3.Backward, n_rotation);

            float rayDirX = rayDir.X;
            float rayDirY = rayDir.Y;
            float rayDirZ = rayDir.Z;

            float deltaDistX = (float)Math.Sqrt(1 + (rayDirZ * rayDirZ) / (rayDirX * rayDirX));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDirZ * rayDirZ) / (rayDirY * rayDirY));
            float deltaDistZ = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirZ * rayDirZ));

            float sideDistX = 0;
            float sideDistY = 0;
            float sideDistZ = 0;

            int stepX = 0;
            int stepY = 0;
            int stepZ = 0;

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

            if (rayDirZ < 0)
            {
                stepZ = -1;
                sideDistZ = (posZ - mapZ) * deltaDistZ;
            }
            else
            {
                stepZ = 1;
                sideDistZ = (mapZ + 1.0f - posZ) * deltaDistZ;
            }

            int distance = 0;
            int maxDistance = 25;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult result = new RayResult();

            while (hit == 0 && distance < maxDistance)
            {
                /*if (sideDistZ < sideDistY)
                {
                    if (sideDistZ < sideDistX)
                    {
                        sideDistZ += deltaDistZ;
                        mapZ += stepZ;
                        side = 1;
                    }
                    else
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                }
                else
                {
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
                        side = 2;
                    }
                }*/

                if (sideDistZ < sideDistX)
                {
                    sideDistZ += deltaDistZ;
                    mapZ += stepZ;
                    side = 1;
                }
                else
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    side = 0;
                }

                if (IsSolid(mapX, mapY, mapZ))
                {
                    hit = 1;
                }
                distance++;
            }

            if (side == 0)
            {
                rayLength = sideDistX - deltaDistX;
            }
            else if (side == 1)
            {
                rayLength = sideDistZ - deltaDistZ;
            }
            else if (side == 2)
            {
                rayLength = sideDistY - deltaDistY;
            }

            result.From = new Vector3(posX, posY, posZ);
            result.To = result.From + rayDir * rayLength;
            result.Length = rayLength;
            result.Hit = hit;
            result.RotationZ = rotZ + diffZ;
            result.Side = side;
            result.Direction = rayDir;

            return result;
        }

        private RayResult RunIterationXZ(float x, float y, float z, float rotX, float rotY, float rotZ, float diffX, float diffY, float diffZ)
        {

            float posX = x;
            float posZ = z;
            float posY = y;

            var map = ToGrid(posX, posY, posZ);

            int mapX = (int)map.X;
            int mapZ = (int)map.Z;
            int mapY = (int)map.Y;

            var n_rotation =
                Matrix.CreateRotationX(rotX + diffX) *
                Matrix.CreateRotationY(rotY + diffY) *
                Matrix.CreateRotationZ(rotZ + diffZ);

            var rayDir = Vector3.Transform(Vector3.Backward, n_rotation);

            float rayDirX = rayDir.X;
            float rayDirY = rayDir.Y;
            float rayDirZ = rayDir.Z;

            float deltaDistX = (float)Math.Sqrt(1 + (rayDirZ * rayDirZ) / (rayDirX * rayDirX));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDirZ * rayDirZ) / (rayDirY * rayDirY));
            float deltaDistZ = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirZ * rayDirZ));

            float sideDistX = 0;
            float sideDistY = 0;
            float sideDistZ = 0;

            int stepX = 0;
            int stepY = 0;
            int stepZ = 0;

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

            if (rayDirZ < 0)
            {
                stepZ = -1;
                sideDistZ = (posZ - mapZ) * deltaDistZ;
            }
            else
            {
                stepZ = 1;
                sideDistZ = (mapZ + 1.0f - posZ) * deltaDistZ;
            }

            int distance = 0;
            int maxDistance = 25;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult result = new RayResult();

            while (hit == 0 && distance < maxDistance)
            {
                /*if (sideDistZ < sideDistY)
                {
                    if (sideDistZ < sideDistX)
                    {
                        sideDistZ += deltaDistZ;
                        mapZ += stepZ;
                        side = 1;
                    }
                    else
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                }
                else
                {
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
                        side = 2;
                    }
                }*/

                if (sideDistZ < sideDistX)
                {
                    sideDistZ += deltaDistZ;
                    mapZ += stepZ;
                    side = 1;
                }
                else
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    side = 0;
                }

                if (IsSolid(mapX, mapY, mapZ))
                {
                    hit = 1;
                }
                distance++;
            }

            if (side == 0)
            {
                rayLength = sideDistX - deltaDistX;
            }
            else if (side == 1)
            {
                rayLength = sideDistZ - deltaDistZ;
            }
            else if (side == 2)
            {
                rayLength = sideDistY - deltaDistY;
            }

            result.From = new Vector3(posX, posY, posZ);
            result.To = result.From + rayDir * rayLength;
            result.Length = rayLength;
            result.Hit = hit;
            result.RotationZ = rotZ + diffZ;
            result.Side = side;
            result.Direction = rayDir;

            return result;
        }

        private RayResult RunIterationYZ(float x, float y, float z, float rotX, float rotY, float rotZ, float diffX, float diffY, float diffZ)
        {

            float posX = x;
            float posZ = z;
            float posY = y;

            var map = ToGrid(posX, posY, posZ);

            int mapX = (int)map.X;
            int mapZ = (int)map.Z;
            int mapY = (int)map.Y;

            var n_rotation =
                Matrix.CreateRotationX(rotX + diffX) *
                Matrix.CreateRotationY(rotY + diffY) *
                Matrix.CreateRotationZ(rotZ + diffZ);

            var rayDir = Vector3.Transform(Vector3.Backward, n_rotation);

            float rayDirX = rayDir.X;
            float rayDirY = rayDir.Y;
            float rayDirZ = rayDir.Z;

            float deltaDistX = (float)Math.Sqrt(1 + (rayDirZ * rayDirZ) / (rayDirX * rayDirX));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDirZ * rayDirZ) / (rayDirY * rayDirY));
            float deltaDistZ = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirZ * rayDirZ));

            float sideDistX = 0;
            float sideDistY = 0;
            float sideDistZ = 0;

            int stepX = 0;
            int stepY = 0;
            int stepZ = 0;

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

            if (rayDirZ < 0)
            {
                stepZ = -1;
                sideDistZ = (posZ - mapZ) * deltaDistZ;
            }
            else
            {
                stepZ = 1;
                sideDistZ = (mapZ + 1.0f - posZ) * deltaDistZ;
            }

            int distance = 0;
            int maxDistance = 25;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult result = new RayResult();

            while (hit == 0 && distance < maxDistance)
            {
                if (sideDistZ < sideDistY)
                {
                    sideDistZ += deltaDistZ;
                    mapZ += stepZ;
                    side = 1;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                    side = 2;
                }

                if (IsSolid(mapX, mapY, mapZ))
                {
                    hit = 1;
                }
                distance++;
            }

            if (side == 0)
            {
                rayLength = sideDistX - deltaDistX;
            }
            else if (side == 1)
            {
                rayLength = sideDistZ - deltaDistZ;
            }
            else if (side == 2)
            {
                rayLength = sideDistY - deltaDistY;
            }

            result.From = new Vector3(posX, posY, posZ);
            result.To = result.From + rayDir * rayLength;
            result.Length = rayLength;
            result.Hit = hit;
            result.RotationZ = rotZ + diffZ;
            result.Side = side;
            result.Direction = rayDir;

            return result;
        }

        private RayResult RunIterationYX(float x, float y, float z, float rotX, float rotY, float rotZ, float diffX, float diffY, float diffZ)
        {

            float posX = x;
            float posZ = z;
            float posY = y;

            var map = ToGrid(posX, posY, posZ);

            int mapX = (int)map.X;
            int mapZ = (int)map.Z;
            int mapY = (int)map.Y;

            var n_rotation =
                Matrix.CreateRotationX(rotX + diffX) *
                Matrix.CreateRotationY(rotY + diffY) *
                Matrix.CreateRotationZ(rotZ + diffZ);

            var rayDir = Vector3.Transform(new Vector3(1, 0, 0), n_rotation);

            float rayDirX = rayDir.X;
            float rayDirY = rayDir.Y;
            float rayDirZ = rayDir.Z;

            float deltaDistX = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
            float deltaDistZ = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirZ * rayDirZ));

            float sideDistX = 0;
            float sideDistY = 0;
            float sideDistZ = 0;

            int stepX = 0;
            int stepY = 0;
            int stepZ = 0;

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

            if (rayDirZ < 0)
            {
                stepZ = -1;
                sideDistZ = (posZ - mapZ) * deltaDistZ;
            }
            else
            {
                stepZ = 1;
                sideDistZ = (mapZ + 1.0f - posZ) * deltaDistZ;
            }

            int distance = 0;
            int maxDistance = 25;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult result = new RayResult();

            while (hit == 0 && distance < maxDistance)
            {
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
                    side = 2;
                }

                if (IsSolid(mapX, mapY, mapZ))
                {
                    hit = 1;
                }
                distance++;
            }

            if (side == 0)
            {
                rayLength = sideDistX - deltaDistX;
            }
            else if (side == 1)
            {
                rayLength = sideDistZ - deltaDistZ;
            }
            else if (side == 2)
            {
                rayLength = sideDistY - deltaDistY;
            }

            result.From = new Vector3(posX, posY, posZ);
            result.To = result.From + rayDir * rayLength;
            result.Length = rayLength;
            result.Hit = hit;
            result.RotationZ = rotZ + diffZ;
            result.Side = side;
            result.Direction = rayDir;

            return result;
        }

        private Vector3 ToGrid(Vector3 position)
        {
            var (x, y, z) = position;
            if (x < 0)
            {
                x -= 1;
            }
            if (y < 0)
            {
                y -= 1;
            }
            if (z < 0)
            {
                z -= 1;
            }

            int gridX = (int)(x);
            int gridY = (int)(y);
            int gridZ = (int)(z);

            return new Vector3(gridX, gridY, gridZ);
        }

        private Vector3 ToGrid(float x, float y, float z)
        {
            return ToGrid(new Vector3(x, y, z));
        }

    }
}
