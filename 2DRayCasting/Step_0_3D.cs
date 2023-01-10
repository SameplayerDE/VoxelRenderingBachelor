using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{

    public struct RayResult
    {
        public int Hit;
        public int Side;
        public float Length;
        public Vector3 Direction;
        public Vector3 From;
        public Vector3 To;
        public float RotationZ;
    }

    public class Step0_3D : Game
    {
        private Texture2D _pixel;
        private SpriteFont _font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int[,,] _map;
        private int _cellSize = 20;

        //Player
        private Player3D _player;

        private int _rayResolution = 10;

        const int ResolutionX = 720;
        const int ResolutionY = 720;

        private List<RayResult> _results = new List<RayResult>();
        private List<string> _debug = new List<string>();

        VertexBuffer vertexBuffer;

        Effect _effect;
        Matrix world = Matrix.CreateTranslation(0, 0, 0);
        Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1));
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

        public Step0_3D()
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

            _player = new Player3D();
            _player.Position = new Vector3(2, 2, 1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _font = Content.Load<SpriteFont>("Font");

            _effect = Content.Load<Effect>("vpc");

            VertexPositionColor[] vertices = new VertexPositionColor[6 * 2];

            //Facing Negativ X
            vertices[6 * 0 + 0] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.White);
            vertices[6 * 0 + 1] = new VertexPositionColor(new Vector3(+1, +0, +0), Color.White);
            vertices[6 * 0 + 2] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.White);
                     
            vertices[6 * 0 + 3] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.White);
            vertices[6 * 0 + 4] = new VertexPositionColor(new Vector3(+0, -1, +0), Color.White);
            vertices[6 * 0 + 5] = new VertexPositionColor(new Vector3(+0, +0, +0), Color.White);

            //Facing Negativ X
            vertices[6 * 1 + 0] = new VertexPositionColor(new Vector3(+1, +0, +0), Color.Red);
            vertices[6 * 1 + 1] = new VertexPositionColor(new Vector3(+1, +0, +1), Color.Red);
            vertices[6 * 1 + 2] = new VertexPositionColor(new Vector3(+1, -1, +1), Color.Red);

            vertices[6 * 1 + 3] = new VertexPositionColor(new Vector3(+1, -1, +1), Color.Red);
            vertices[6 * 1 + 4] = new VertexPositionColor(new Vector3(+1, -1, +0), Color.Red);
            vertices[6 * 1 + 5] = new VertexPositionColor(new Vector3(+1, +0, +0), Color.Red);

            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 6 * 2, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

        }

        protected override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                var next = _player.Next(gameTime);
                if (!IsSolid(next.X, next.Y, next.Z))
                {
                    _player.Move(gameTime);
                }
                else
                {
                    if (!IsSolid(next.X, _player.Position.Y, _player.Position.Z))
                    {
                        _player.Position = new Vector3(next.X, _player.Position.Y, _player.Position.Z);
                    }
                    if (!IsSolid(_player.Position.X, next.Y, _player.Position.Z))
                    {
                        _player.Position = new Vector3(_player.Position.X, next.Y, _player.Position.Z);
                    }
                    if (!IsSolid(_player.Position.X, _player.Position.Y, next.Z))
                    {
                        _player.Position = new Vector3(_player.Position.X, _player.Position.Y, next.Z);
                    }
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                _player.Rotation.Y = 0;
            }

            _debug.Clear();
            _debug.Add($"{_player.Position}");
            _debug.Add($"{_player.Rotation}");
            _debug.Add($"{_player.Direction}");

            _results.Clear();
            for (float z = MathHelper.ToRadians(-45); z < MathHelper.ToRadians(45); z += MathHelper.ToRadians(90) / GraphicsDevice.Viewport.Height * _rayResolution)
            {
                for (float r = MathHelper.ToRadians(-45); r < MathHelper.ToRadians(45); r += MathHelper.ToRadians(90) / GraphicsDevice.Viewport.Width * _rayResolution)
                {
                    _results.Add(RunIteration(_player.Position.X, _player.Position.Y, _player.Position.Z, _player.Rotation.Z, _player.Rotation.Y * 0, r, z));
                }
            }

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.Viewport.AspectRatio, 0.01f, 100f);
            view = Matrix.CreateLookAt(_player.Position, _player.Position + _player.Direction, new Vector3(0, 0, 1));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {   
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                int iteration = 0;
                foreach (var result in _results)
                {
                    var length = result.Length;
                    int height = GraphicsDevice.Viewport.Height;

                    int lineHeight = (int)(height / length);

                    //calculate lowest and highest pixel to fill in current stripe
                    int drawStart = -lineHeight / 2 + height / 2;
                    if (drawStart < 0) drawStart = 0;
                    int drawEnd = lineHeight / 2 + height / 2;
                    if (drawEnd >= height) drawEnd = height - 1;

                    if (result.Hit == 1)
                    {
                        var color = Color.White;

                        if (result.Side == 0)
                        {
                            color.R = (byte)(color.R / 2);
                            color.G = (byte)(color.G / 2);
                            color.B = (byte)(color.B / 2);
                        }
                        if (result.Side == 1)
                        {
                            color.R = (byte)(color.R);
                            color.G = (byte)(color.G);
                            color.B = (byte)(color.B);
                        }
                        if (result.Side == 2)
                        {
                            color.R = (byte)(color.R / 2);
                            color.G = (byte)(color.G / 2);
                            color.B = (byte)(color.B / 2);
                        }
                        if (result.Side == 3)
                        {
                            color.R = (byte)(color.R / 4);
                            color.G = (byte)(color.G / 4);
                            color.B = (byte)(color.B / 4);
                        }

                        var alpha = 1 - (result.Length / 25f);
                        Line(new Vector2(iteration, drawStart), new Vector2(iteration, drawEnd), color * alpha, _rayResolution, _spriteBatch);
                    }
                    iteration += _rayResolution;
                }
            }
            else
            {
                int x = 0, y = 0;
                foreach (var result in _results)
                {

                    var length = result.Length;
                    int height = GraphicsDevice.Viewport.Height;

                    int lineHeight = (int)(height / length);
                
                    if (result.Hit == 1)
                    {
                        var color = Color.White;

                        if (result.Side == 0)
                        {
                            color.R = (byte)(color.R / 2);
                            color.G = (byte)(color.G / 2);
                            color.B = (byte)(color.B / 2);
                        }
                        if (result.Side == 1)
                        {
                            color.R = (byte)(color.R);
                            color.G = (byte)(color.G);
                            color.B = (byte)(color.B);
                        }
                        if (result.Side == 2)
                        {
                            color.R = (byte)(color.R / 2);
                            color.G = (byte)(color.G / 2);
                            color.B = (byte)(color.B / 2);
                        }
                        if (result.Side == 3)
                        {
                            color.R = (byte)(color.R / 4);
                            color.G = (byte)(color.G / 4);
                            color.B = (byte)(color.B / 4);
                        }

                        var alpha = 1 - (result.Length / 25f);
                        _spriteBatch.Draw(_pixel, new Vector2(x * _rayResolution, y * _rayResolution), null, color * alpha, 0f, new Vector2(.5f, .5f), new Vector2(_rayResolution, _rayResolution), SpriteEffects.None, 0f);
                    }
                    x++;
                    if (x * _rayResolution > GraphicsDevice.Viewport.Width)
                    {
                        x = 0;
                        y++;
                    }
                }
            }


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

            _spriteBatch.End();

            //world = Matrix.CreateTranslation(_player.Position);
            _effect.Parameters["WorldViewProjection"].SetValue(world * view * projection);

            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            }

            base.Draw(gameTime);
        }

        public void DrawMap()
        {

        }

        private RayResult RunIteration(float x, float y, float z, float rotZ, float rotY, float diffZ, float diffY)
        {
            float posX = x;
            float posY = y;
            float posZ = z;

            int mapX = (int)posX;
            int mapY = (int)posY;
            int mapZ = (int)posZ;

            var n_rotation =
                Matrix.CreateRotationX(0) *
                Matrix.CreateRotationY(rotY + diffY) *
                Matrix.CreateRotationZ(rotZ + diffZ);

            var rayDir = Vector3.Transform(new Vector3(1, 0, 0), n_rotation);

            float rayDirX = rayDir.X;
            float rayDirY = rayDir.Y;
            float rayDirZ = rayDir.Z;

            float deltaDistX = (float)Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
            float deltaDistY = (float)Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
            float deltaDistZ = (float)Math.Sqrt(1 + (rayDirZ * rayDirZ) / (rayDirZ * rayDirZ));

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

            /*if (rayDirZ < 0)
            {
                stepZ = -1;
                sideDistZ = (posZ - mapZ) * deltaDistZ;
            }
            else
            {
                stepZ = 1;
                sideDistZ = (mapZ + 1.0f - posZ) * deltaDistZ;
            }*/

            int distance = 0;
            int maxDistance = 100;
            int hit = 0;
            int side = 0;
            float rayLength = 0.0f;

            RayResult result = new RayResult();

            while (hit == 0 && distance < maxDistance)
            {
                if (sideDistX < sideDistY)
                {
                    if (sideDistX < sideDistZ)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        if (stepX > 0)
                        {
                            side = 0;
                        }
                        if (stepX < 0)
                        {
                            side = 2;
                        }
                    }
                    else
                    {
                        sideDistZ += deltaDistZ;
                        mapZ += stepZ;
                        if (stepZ > 0)
                        {
                            side = 4;
                        }
                        if (stepZ < 0)
                        {
                            side = 5;
                        }
                    }
                }
                else
                {
                    if (sideDistY < sideDistZ)
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        if (stepY > 0)
                        {
                            side = 1;
                        }
                        if (stepY < 0)
                        {
                            side = 3;
                        }
                    }
                    else
                    {
                        sideDistZ += deltaDistZ;
                        mapZ += stepZ;
                        if (stepZ > 0)
                        {
                            side = 4;
                        }
                        if (stepZ < 0)
                        {
                            side = 5;
                        }
                    }
                }

                if (IsSolid(mapX, mapY, mapZ))
                {
                    hit = 1;
                }
                distance++;
            }

            if (side == 0 || side == 2)
            {
                rayLength = sideDistX - deltaDistX;
            }
            else if (side == 1 || side == 3)
            {
                rayLength = sideDistY - deltaDistY;
            }
            else if (side == 4 || side == 5)
            {
                rayLength = sideDistZ - deltaDistZ;
            }


            result.Length = rayLength;
            result.Hit = hit;
            result.RotationZ = rotZ + diffZ;
            result.Side = side;

            return result;
        }

        private bool IsSolid(float x, float y, float z)
        {
            int gridX = (int)(x);
            int gridY = (int)(y);
            int gridZ = (int)(z);

            if (gridX >= _map.GetLength(2) || gridX < 0)
            {
                return false;
            }
            if (gridY >= _map.GetLength(1) || gridY < 0)
            {
                return false;
            }
            if (gridZ >= _map.GetLength(0) || gridZ < 0)
            {
                return false;
            }

            bool solid = _map[gridZ, gridY, gridX] != 0;
            return solid;
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