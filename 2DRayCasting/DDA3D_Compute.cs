﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RayCasting
{
    public class DDA3D_Compute : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _pixel;
        private Texture2D _computeTexture;
        private SpriteFont _font;
        private Effect _effect;

        private Effect _computeShader;
        const int ComputeGroupSize = 8;
        private StructuredBuffer _rayResultBuffer;
        private int _maxCount = 500_000;
        private int[] _sahderMap = new int[2 * 24 * 24]
        {
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,2,0,0,0,0,0,0,0,0,3,0,3,0,3,0,0,0,1,
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
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,1,
            1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,
            1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,
            1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,
            1,4,0,4,0,0,0,0,4,0,0,0,0,2,2,2,0,0,0,0,0,0,0,1,
            1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,1,
            1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,
            1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,
            1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1,
            1,4,0,4,0,0,0,0,4,0,0,0,0,2,2,2,0,0,0,0,0,0,0,1,
            1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
        };

        private RenderTarget2D _virtualScreen;
        private const int _virtualResolutionX = 240;
        private const int _virtualResolutionY = 240;

        private RenderTarget2D _rayCastTarget;
        private const int _rayCastTargetResolutionX = 80;
        private const int _rayCastTargetResolutionY = 80;

        private int[,,] _map;

        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        private Texture2D[] _textures;

        private List<RayResult3D> _results = new List<RayResult3D>();

        //Cube
        private VertexBuffer _vertexBuffer;

        //Player
        private Vector3 _position = new Vector3(2, 1.5f, 2);
        private Vector3 _rayPosition = new Vector3(2, 1.5f, 2);
        private Vector3 _rotation;
        private Vector3 _rayRotation;
        private Vector3 _direction;
        private float _movementSpeed = 1.0f;

        private float _xxx = 1.0f;
        private float _fov = 90;
        private float _rayCasterFOV = 360;
        private int _rayResolution = 1;
        private bool _requestRender = false;

        private KeyboardState _prevState;
        private KeyboardState _currState;

        private bool _centerMouse = false;

        public DDA3D_Compute()
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
            _textures[0] = Content.Load<Texture2D>("ass");
            _textures[1] = Content.Load<Texture2D>("dirt");
            _textures[2] = Content.Load<Texture2D>("oak_planks");
            _textures[3] = Content.Load<Texture2D>("iron_block");
            _textures[4] = Content.Load<Texture2D>("gold_block");

            _computeTexture = new Texture2D(GraphicsDevice, _rayCastTargetResolutionX, _rayCastTargetResolutionY, false, SurfaceFormat.Color, ShaderAccess.ReadWrite);
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

            _map = new int[4, 24, 24] // y, z, x
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
                //_rotation.X += mouseDelta.Y * delta;
            

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

            if (keyboard.IsKeyDown(Keys.R))
            {
                _rotation.X = 0;
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

            int w = GraphicsDevice.Viewport.Width;
            float ratio = GraphicsDevice.Viewport.AspectRatio;

            //if (keyboard.IsKeyDown(Keys.Q))
            //{
            //    _rayPosition = _position;
            //    _rayRotation = _rotation;
            //
            //    //_results.Clear();
            //    //_results.Add(DDACalculator.RunIteration3D(IsSolid, GetSolid, _rayPosition.X, _rayPosition.Y, _rayPosition.Z, _rayRotation.X, _rayRotation.Y, _rayRotation.Z, 0, 0, 0));
            //    //var a = _results[0].Direction.Length();
            //}
            //    _results2.Clear();
            //
            //    float xxx = 180;
            //
            //    for (var r = -MathHelper.ToRadians(xxx / 2); r < MathHelper.ToRadians(xxx / 2); r += (MathHelper.ToRadians(xxx) / _rayCastTargetResolutionY) * _rayResolution)
            //    {
            //        for (var i = -MathHelper.ToRadians(_rayCasterFOV / 2); i < MathHelper.ToRadians(_rayCasterFOV / 2); i += (MathHelper.ToRadians(_rayCasterFOV) / _rayCastTargetResolutionX) * _rayResolution)
            //        {
            //            _results.Add(DDACalculator.RunIteration3D(IsSolid, GetSolid, _rayPosition.X, _rayPosition.Y, _rayPosition.Z, _rayRotation.X, _rayRotation.Y, _rayRotation.Z, r, i, 0));
            //        }
            //    }
            //
            //    _rayPosition += _direction * 0.1f;
            //
            //
            //    for (var r = -MathHelper.ToRadians(xxx / 2); r < MathHelper.ToRadians(xxx / 2); r += (MathHelper.ToRadians(xxx) / _rayCastTargetResolutionY) * _rayResolution)
            //    {
            //        for (var i = -MathHelper.ToRadians(_rayCasterFOV / 2); i < MathHelper.ToRadians(_rayCasterFOV / 2); i += (MathHelper.ToRadians(_rayCasterFOV) / _rayCastTargetResolutionX) * _rayResolution)
            //        {
            //            _results2.Add(DDACalculator.RunIteration3D(IsSolid, GetSolid, _rayPosition.X, _rayPosition.Y, _rayPosition.Z, _rayRotation.X, _rayRotation.Y, _rayRotation.Z, r, i, 0));
            //        }
            //    }
            //
            //    for (float r = 0; r < _rayCastTargetResolutionY; r += _rayResolution)
            //    {
            //        for (var i = -MathHelper.ToRadians(_fov / 2); i < MathHelper.ToRadians(_fov / 2); i += (MathHelper.ToRadians(_fov) / _rayCastTargetResolutionX) * _rayResolution)
            //        {
            //            //_results.Add(DDACalculator.RunIteration3D(IsSolid, GetSolid, _rayPosition.X, _rayPosition.Y, _rayPosition.Z, _rayRotation.X, _rayRotation.Y, _rayRotation.Z, 0, i, 0));
            //        }
            //    }
            //
            //
            //    for (float r = 0; r < _rayCastTargetResolutionY; r += _rayResolution)
            //    {
            //        for (float i = 0; i < _rayCastTargetResolutionX; i += _rayResolution)
            //        {
            //            float center = i - (w / 2);
            //            float xAng = (float)Math.Atan(center / 100f);
            //            //_results.Add(DDACalculator.RunIteration3D(IsSolid, GetSolid, _rayPosition.X, _rayPosition.Y, _rayPosition.Z, _rayRotation.X, _rayRotation.Y, _rayRotation.Z, 0, xAng, 0));
            //        }
            //    }
            //
            //    for (var i = -MathHelper.ToRadians(_fov / 2); i < MathHelper.ToRadians(_fov / 2); i += (MathHelper.ToRadians(_fov) / _rayCastTargetResolutionX) * _rayResolution)
            //    {
            //        //_results.Add(DDACalculator.RunIteration3D(IsSolid, GetSolid, _rayPosition.X, _rayPosition.Y, _rayPosition.Z, _rayRotation.X, _rayRotation.Y, _rayRotation.Z, 0, i, 0));
            //    }
            //
            //    _requestRender = true;
            //}

            _world = Matrix.Identity;
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), GraphicsDevice.Viewport.AspectRatio, 0.01f, 100f);
            _view = Matrix.CreateLookAt(_position, _position + _direction, new Vector3(0, 1, 0));

            //ang = ray angle from the look direction, whose ray passes through the central x coordinate of the screen;
            //opp = opposite side(equivalent to the distance of the screen X coordinate through which the ray passes from the screen X coordinate of the central pixel);
            //adj = adjacent side(equivalent to the distance from the point of view to the projection surface, which will be predetermined in code somewhere);

            //tan(ang) = opp / adj
            //ang = atan(opp / adj)
            //ang = atan((pixel x coord - half screen width) / dist to projection surface )

            //double dirX = -1, dirY = 0; //initial direction vector
            //double planeX = 0, planeY = 0.66; //the 2d raycaster version of camera plane

            /*for (int x = 0; x < w; x++)
            {
                //calculate ray position and direction
                double cameraX = 2 * x / -1; //x-coordinate in camera space
                double rayDirX = dirX + planeX * cameraX;
                double rayDirY = dirY + planeY * cameraX;

                //for (var x = 0; x < w; x++)
                // {
                //float center = x - (w / 2);
                //float xAng = (float)Math.Atan(center / 100f);
                _results.Add(DDACalculator.RunIteration3D(IsSolid, GetSolid, _position.X, _position.Y, _position.Z, _rotation.X, _rotation.Y, _rotation.Z, 0, xAng, 0));
            }*/

            if (keyboard.IsKeyDown(Keys.Q))
            {
                _rayPosition = _position;
                _rayRotation = _rotation;
                ComputeRays();

                var results = new RayResult3D[_rayCastTargetResolutionX * _rayCastTargetResolutionY];
                _rayResultBuffer.GetData(results, 0, _rayCastTargetResolutionX * _rayCastTargetResolutionY);
                
                _results.Clear();
                _results.AddRange(results);
                _requestRender = true;
            }

            base.Update(gameTime);
        }

        private void RenderEye(List<RayResult3D> rays, RenderTarget2D target)
        {
            GraphicsDevice.SetRenderTarget(target);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(depthStencilState: DepthStencilState.Default, samplerState: SamplerState.PointWrap);

            int ittX = 0;
            int ittY = 0;

            foreach (var result in rays)
            {
                if (result.Hit == 1)
                {
                    var alpha = 1 - (result.Length / 25f);
                    var depth = 1 - (result.Length / 25f);
                    var color = new Color(depth, depth, depth);

                    var texture = _textures[result.Id - 1];
                    //var texture = _pixel;
                    var textureWidth = texture.Width;
                    var textureHeight = texture.Height;

                    int screenX = _rayCastTargetResolutionX - ittX * _rayResolution;
                    int screenY = ittY * _rayResolution;

                    var xRatio = result.To.X - (int)result.To.X;
                    var yRatio = 1 - (result.To.Y - (int)result.To.Y);
                    var zRatio = result.To.Z - (int)result.To.Z;
                    Rectangle slice = Rectangle.Empty;

                    if (result.Side == (int)Side.X)
                    {
                        slice = new Rectangle((int)(textureWidth * zRatio), (int)(textureHeight * yRatio), 1, 1);
                    }
                    if (result.Side == (int)Side.Y)
                    {
                        slice = new Rectangle((int)(textureWidth * xRatio), (int)(textureHeight * zRatio), 1, 1);
                    }
                    if (result.Side == (int)Side.Z)
                    {
                        slice = new Rectangle((int)(textureWidth * xRatio), (int)(textureHeight * yRatio), 1, 1);
                    }

                    float scale = _rayResolution;// (1f - (result.Length / 25f)) * (float)_rayResolution;

                    _spriteBatch.Draw(texture, new Vector2(screenX, screenY), slice, color, 0f, new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 1f);
                    //_spriteBatch.Draw(texture, new Rectangle(drawStart), slice, color, 0f, new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 1f);
                }

                var countX = _rayCastTargetResolutionX;

                ittX++;
                if (ittX >= countX)
                {
                    ittX = 0;
                    ittY++;
                }
            }
            _spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_requestRender)
            {

                //RenderEye(_results, _rayCastTarget);
                //RenderEye(_results2, _rayCastTarget2);

                //GraphicsDevice.SetRenderTarget(_vrView);
                //GraphicsDevice.Clear(Color.White);
                //
                //_spriteBatch.Begin();
                //_spriteBatch.Draw(_rayCastTarget, new Vector2(0, _rayCastTarget2ResolutionY), Color.White);
                //_spriteBatch.Draw(_rayCastTarget2, new Vector2(0, 0), Color.White);
                //_spriteBatch.End();
                //
                //_requestRender = false;
                //
                //Stream stream = File.Create("file.png");
                //_vrView.SaveAsPng(stream, _vrView.Width, _vrView.Height);
                //stream.Dispose();
            }

            GraphicsDevice.SetRenderTarget(_virtualScreen);
            GraphicsDevice.Clear(Color.White);

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
            foreach (var result in _results)
            {
                DrawLine(result.From - new Vector3(0, 0.01f, 0), result.To);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(depthStencilState: DepthStencilState.Default, samplerState: SamplerState.PointWrap);

            _spriteBatch.Draw(_virtualScreen, GraphicsDevice.Viewport.Bounds, Color.White);
            //_spriteBatch.Draw(_computeTexture, new Rectangle(0, 0, 512, 512), Color.White);
            _spriteBatch.Draw(_computeTexture, Vector2.Zero, Color.White);

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
            
            if (gridY >= _map.GetLength(0) || gridY < 0)
            {
                return false;
            }
            if (gridZ >= _map.GetLength(1) || gridZ < 0)
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

        private void ComputeRays()
        {

            _computeShader.Parameters["CPUMap"].SetValue(_sahderMap);
            _computeShader.Parameters["Position"].SetValue(_rayPosition);
            _computeShader.Parameters["Rotation"].SetValue(_rayRotation);
            _computeShader.Parameters["Output"].SetValue(_computeTexture);
            _computeShader.Parameters["Width"].SetValue(_rayCastTargetResolutionX);
            _computeShader.Parameters["Height"].SetValue(_rayCastTargetResolutionY);

            _computeShader.Parameters["Results"].SetValue(_rayResultBuffer);

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
