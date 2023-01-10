using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{
    public class DDA3D_Compute_Final : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;
        private Texture2D _computeTexture;
        private Texture3D _textureAtlas;

        private SpriteFont _font;
        private Effect _effect;

        private List<RayResult3D> _results = new List<RayResult3D>();

        private Effect _computeShader;
        const int ComputeGroupSize = 64;
        private StructuredBuffer _rayResultBuffer;
        private StructuredBuffer _shaderMap;
        private int _maxCount = 1_000_000;
        private int[] _sahderMap = new int[2304];

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 240;
        private const int _virtualResolutionY = 240;

        private RenderTarget2D _rayCastTarget;
        private const int _rayCastTargetResolutionX = 1920 / 10;
        private const int _rayCastTargetResolutionY = 1080 / 10;

        private const int _mapX = 24;
        private const int _mapY = 6;
        private const int _mapZ = 24;

        private int[,,] _map;

        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        private Texture2D[] _textures;

        //Cube
        private VertexBuffer _vertexBuffer;

        //Player
        private Vector3 _position = new Vector3(2, 1.5f, 2);
        private Vector3 _rayPosition = new Vector3(2, 1.5f, 2);
        private Vector3 _rotation;
        private Vector3 _rayRotation;
        private Vector3 _direction;
        private float _movementSpeed = 5.0f;

        private float _xxx = 1.0f;
        private float _fov = 90;
        private float _rayCasterFOV = 360;
        private int _rayResolution = 1;
        private bool _requestRender = false;

        private KeyboardState _prevState;
        private KeyboardState _currState;

        private bool _centerMouse = false;

        public DDA3D_Compute_Final()
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
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(16);

            _graphics.ApplyChanges();
            
            _textures = new Texture2D[5];
            _textures[0] = Content.Load<Texture2D>("cobblestone");
            _textures[1] = Content.Load<Texture2D>("dirt");
            _textures[2] = Content.Load<Texture2D>("oak_planks");
            _textures[3] = Content.Load<Texture2D>("iron_block");
            _textures[4] = Content.Load<Texture2D>("gold_block");

            _computeTexture = new Texture2D(GraphicsDevice, _rayCastTargetResolutionX, _rayCastTargetResolutionY, false, SurfaceFormat.Color, ShaderAccess.ReadWrite);
            _textureAtlas = new Texture3D(GraphicsDevice, 16, 16, 5, false, SurfaceFormat.Color, ShaderAccess.ReadWrite);
            
            _virtualScreen = new RenderTarget2D(GraphicsDevice, _virtualResolutionX, _virtualResolutionY, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
            _rayCastTarget = new RenderTarget2D(GraphicsDevice, _rayCastTargetResolutionX, _rayCastTargetResolutionY, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);


            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _font = Content.Load<SpriteFont>("Font");
            _effect = Content.Load<Effect>("vpc");
            
            _computeShader = Content.Load<Effect>("Ray3D");
            var texture = Content.Load<Texture2D>("dirt");

            _computeShader.Parameters["Input"].SetValue(texture);
            _computeShader.Parameters["InputW"].SetValue(texture.Width);
            _computeShader.Parameters["InputH"].SetValue(texture.Height);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });

            _map = new int[_mapY, _mapZ, _mapX] // y, z, x
            {
                {
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,2,3,1,2,3,2,3,2,1,2,3,2,1,2,3,1,2,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
                },
                {
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,2,0,0,0,0,0,0,0,0,3,0,3,0,3,0,0,0,1 },
                    { 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1 },
                    { 1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,1 },
                    { 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,4,0,0,0,0,4,0,0,0,0,2,2,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
                },
                {
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,4,0,0,0,0,4,0,0,0,0,2,2,2,0,0,0,0,0,0,0,1 },
                    { 1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
                },
                {
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,1,2,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,1,2,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
                },
                {
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
                },
                {
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                    { 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 }
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

            _rayResultBuffer = new StructuredBuffer(GraphicsDevice, typeof(RayResult3D), _maxCount, BufferUsage.None, ShaderAccess.ReadWrite);
            _shaderMap = new StructuredBuffer(GraphicsDevice, typeof(int), _mapX * _mapY * _mapZ, BufferUsage.None, ShaderAccess.ReadWrite);

            Random rand = new Random();
            int[] tempMap = new int[_mapX * _mapZ * _mapY];
            for (int y = 0; y < _mapY; y++)
            {
                for (int z = 0; z < _mapZ; z++)
                {
                    for (int x = 0; x < _mapX; x++)
                    {
                        int id = rand.Next(0, 4);
                        id = _map[y, z, x];
                        int index = x + _mapX * z + _mapX * _mapZ * y;
                        tempMap[index] = id;
                        _map[y, z, x] = id;
                    }
                }
            }

            Color[] atlas = new Color[16 * 16 * 5];
            for (int i = 0; i < 5; i++)
            {
                Color[] copy = new Color[16 * 16];
                _textures[i].GetData(copy);
                for (int y = 0; y < 16; y++)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        int index3D = x + 16 * y + 16 * 16 * i;
                        int index2D = x + 16 * y;
                        atlas[index3D] = copy[index2D];
                    }
                }
            }

            _computeShader.Parameters["MapMaxX"].SetValue(_mapX);
            _computeShader.Parameters["MapMaxY"].SetValue(_mapY);
            _computeShader.Parameters["MapMaxZ"].SetValue(_mapZ);

            _shaderMap.SetData(tempMap);
            _textureAtlas.SetData(atlas);

            _computeShader.Parameters["Input"].SetValue(_textureAtlas);

            _computeShader.Parameters["InputW"].SetValue(_textureAtlas.Width);
            _computeShader.Parameters["InputH"].SetValue(_textureAtlas.Height);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _prevState = _currState;
            _currState = Keyboard.GetState();

            var keyboard = _currState;

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (_centerMouse)
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

                /*int rotationYKeys = (keyboard.IsKeyDown(Keys.Left) ? -1 : 0) + (keyboard.IsKeyDown(Keys.Right) ? 1 : 0);
                int rotationXKeys = (keyboard.IsKeyDown(Keys.Down) ? -1 : 0) + (keyboard.IsKeyDown(Keys.Up) ? 1 : 0);

                _rotation.Y -= MathHelper.ToRadians(90f) * delta * rotationYKeys;
                _rotation.X -= MathHelper.ToRadians(90f) * delta * rotationXKeys;*/

                if (_rotation.Y < 0)
                {
                    _rotation.Y += MathHelper.TwoPi;
                }
                else if (_rotation.Y >= MathHelper.TwoPi)
                {
                    _rotation.Y -= MathHelper.TwoPi;
                }

                _rotation.X = Math.Clamp(_rotation.X, MathHelper.ToRadians(-80f), MathHelper.ToRadians(80f));

                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }

            if (_currState.IsKeyDown(Keys.E) && _prevState.IsKeyUp(Keys.E))
            {
                _centerMouse = !_centerMouse;
            }

            if (_currState.IsKeyDown(Keys.T) && _prevState.IsKeyUp(Keys.T))
            {
                _computeShader.Parameters["LightPosition"].SetValue(_position);
            }

            if (keyboard.IsKeyDown(Keys.R))
            {
                _rotation.X = 0;
            }

            _computeShader.Parameters["RotationMatrix"].SetValue(Matrix.CreateRotationX(_rotation.X) * Matrix.CreateRotationY(_rotation.Y) * Matrix.CreateRotationZ(_rotation.Z));

            var n_rotation =
                Matrix.CreateRotationX(_rotation.X * 0) *
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
                movement += _direction * new Vector3(1, 0, 1) * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                movement -= _direction * new Vector3(1, 0, 1) * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                movement += Vector3.Cross(_direction * new Vector3(1, 0, 1), new Vector3(0, 1, 0)) * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                movement -= Vector3.Cross(_direction * new Vector3(1, 0, 1), new Vector3(0, 1, 0)) * _movementSpeed * delta;
            }

            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                movement += Vector3.Up * _movementSpeed * delta;
            }
            if (keyboard.IsKeyDown(Keys.LeftControl))
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
            }

            _world = Matrix.Identity;
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), GraphicsDevice.Viewport.AspectRatio, 0.01f, 100f);
            _view = Matrix.CreateLookAt(_position, _position + _direction, new Vector3(0, 1, 0));

            if (!keyboard.IsKeyDown(Keys.Q))
            {
                _rayPosition = _position;
                _rayRotation = _rotation;



                _computeShader.Parameters["iTime"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
                ComputeRays();

                //var results = new RayResult3D[_rayCastTargetResolutionX * _rayCastTargetResolutionY];
                //_rayResultBuffer.GetData(results, 0, _rayCastTargetResolutionX * _rayCastTargetResolutionY);
                //
                //_results.Clear();
                //_results.AddRange(results);
                //
                //_requestRender = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_rayCastTarget);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(depthStencilState: DepthStencilState.Default, samplerState: SamplerState.PointWrap);
            _spriteBatch.Draw(_computeTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            //GraphicsDevice.SetRenderTarget(_virtualScreen);
            //GraphicsDevice.Clear(Color.White);
            //
            //for (int y = 0; y < _map.GetLength(0); y++)
            //{
            //    for (int z = 0; z < _map.GetLength(1); z++)
            //    {
            //        for (int x = 0; x < _map.GetLength(2); x++)
            //        {
            //            if (!IsSolid(x, y, z))
            //            {
            //                continue;
            //            }
            //            DrawCube(x, y + 1, z, 1f);
            //        }
            //    }
            //}
            
            //foreach (var result in _results)
            //{
            //    DrawLine(result.From - new Vector3(0, 0.05f, 0), result.To - new Vector3(0, 0.05f, 0));
            //}

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(depthStencilState: DepthStencilState.Default, samplerState: SamplerState.PointWrap);

            //_spriteBatch.Draw(_virtualScreen, GraphicsDevice.Viewport.Bounds, Color.White);
            //_spriteBatch.Draw(_rayCastTarget, new Rectangle(0, 0, 256, 256), Color.White);
            _spriteBatch.Draw(_rayCastTarget, GraphicsDevice.Viewport.Bounds, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private int GetSolid(float x, float y, float z)
        {
            var map = ToGrid(x, y, z);
            int gridX = (int)map.X;
            int gridY = (int)map.Y;
            int gridZ = (int)map.Z;


            if (gridY >= _map.GetLength(0) || gridY < 0)
            {
                return 0;
            }
            if (gridZ >= _map.GetLength(1) || gridZ < 0)
            {
                return 0;
            }
            if (gridX >= _map.GetLength(2) || gridX < 0)
            {
                return 0;
            }

            return _map[gridY, gridZ, gridX];
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

        private bool IsSolid(float x, float y, float z)
        {
            var map = ToGrid(x, y, z);
            int gridX = (int)map.X;
            int gridY = (int)map.Y;
            int gridZ = (int)map.Z;
            
            if (gridY >= _mapY || gridY < 0)
            {
                return false;
            }
            if (gridZ >= _mapZ || gridZ < 0)
            {
                return false;
            }
            if (gridX >= _mapX || gridX < 0)
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

        private void ComputeRays()
        {

            //_computeShader.Parameters["CPUMap"].SetValue(_sahderMap);
            _computeShader.Parameters["Position"].SetValue(_rayPosition);
            _computeShader.Parameters["Rotation"].SetValue(_rayRotation);
            
            //_computeShader.Parameters["LightDirection"].SetValue(_direction);
            
            _computeShader.Parameters["Output"].SetValue(_computeTexture);
            _computeShader.Parameters["Width"].SetValue(_rayCastTargetResolutionX);
            _computeShader.Parameters["Height"].SetValue(_rayCastTargetResolutionY);

            _computeShader.Parameters["Results"].SetValue(_rayResultBuffer);
            _computeShader.Parameters["CPUMap"].SetValue(_shaderMap);

            double count = (_rayCastTargetResolutionX / _rayResolution) * (_rayCastTargetResolutionY / _rayResolution);
            int groupCount = (int)Math.Ceiling((double)count / ComputeGroupSize);

            foreach (var pass in _computeShader.CurrentTechnique.Passes)
            {
                pass.ApplyCompute();
                GraphicsDevice.DispatchCompute(groupCount, 1, 1);
            }
        }

    }
}
