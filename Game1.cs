using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SideScrollerGame_Maniacs;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    // Game objects
    private Player _player;
    private List<Platform> _platforms;
    private List<Enemy> _enemies;
    private Texture2D _pixelTexture;

    // Ground level
    private int _groundY;

    // Player state
    private Vector2 _playerStartPosition;
    private int _score;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Set window size
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 480;
        _graphics.ApplyChanges();

        // Ground is near bottom of screen
        _groundY = _graphics.PreferredBackBufferHeight - 64;

        // Player start position
        _playerStartPosition = new Vector2(100, _groundY - 48);
        _player = new Player(_playerStartPosition);

        // Create platforms
        _platforms = new List<Platform>
        {
            new Platform(200, 320, 120, 20),  // Low platform
            new Platform(400, 250, 120, 20),  // Middle platform
            new Platform(550, 180, 120, 20),  // High platform
            new Platform(150, 180, 100, 20),  // High left platform
        };

        // Create enemies
        _enemies = new List<Enemy>
        {
            new Enemy(new Vector2(400, _groundY - 32), 80f),   // Ground enemy
            new Enemy(new Vector2(420, 250 - 32), 50f),       // Enemy on middle platform
        };

        _score = 0;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create a 1x1 white pixel for drawing rectangles
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        // Load player content
        _player.LoadContent(GraphicsDevice);

        // Load platform content
        foreach (var platform in _platforms)
        {
            platform.LoadContent(GraphicsDevice);
        }

        // Load enemy content
        foreach (var enemy in _enemies)
        {
            enemy.LoadContent(GraphicsDevice);
        }
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var keyboard = Keyboard.GetState();

        // Update player
        _player.Update(gameTime, keyboard);

        // Update enemies
        foreach (var enemy in _enemies)
        {
            enemy.Update(gameTime, _groundY);
        }

        // Reset ground state before collision checks
        _player.IsOnGround = false;

        // Ground collision
        if (_player.Position.Y + _player.Height >= _groundY)
        {
            _player.Position.Y = _groundY - _player.Height;
            _player.Velocity.Y = 0;
            _player.IsOnGround = true;
        }

        // Platform collision (only when falling)
        if (_player.Velocity.Y >= 0)
        {
            foreach (var platform in _platforms)
            {
                if (CheckPlatformCollision(_player, platform))
                {
                    _player.Position.Y = platform.Bounds.Y - _player.Height;
                    _player.Velocity.Y = 0;
                    _player.IsOnGround = true;
                    break;
                }
            }
        }

        // Enemy collision
        foreach (var enemy in _enemies)
        {
            var collision = enemy.CheckPlayerCollision(_player);

            if (collision == CollisionType.Stomped)
            {
                enemy.Stomp();
                _player.Bounce();
                _score += 100;
            }
            else if (collision == CollisionType.HurtPlayer)
            {
                // Reset player position (simple "death")
                _player.Position = _playerStartPosition;
                _player.Velocity = Vector2.Zero;
            }
        }

        // Keep player in bounds (horizontal)
        if (_player.Position.X < 0)
            _player.Position.X = 0;
        if (_player.Position.X + _player.Width > _graphics.PreferredBackBufferWidth)
            _player.Position.X = _graphics.PreferredBackBufferWidth - _player.Width;

        // Fall off screen = reset
        if (_player.Position.Y > _graphics.PreferredBackBufferHeight)
        {
            _player.Position = _playerStartPosition;
            _player.Velocity = Vector2.Zero;
        }

        base.Update(gameTime);
    }

    private bool CheckPlatformCollision(Player player, Platform platform)
    {
        Rectangle playerBounds = player.Bounds;
        Rectangle platformBounds = platform.Bounds;

        // Check if player is horizontally aligned with platform
        bool horizontalOverlap = playerBounds.Right > platformBounds.Left &&
                                  playerBounds.Left < platformBounds.Right;

        // Check if player's feet are at or just above platform surface
        float playerBottom = player.Position.Y + player.Height;
        float platformTop = platformBounds.Y;

        // Player must be falling onto the platform (feet near platform top)
        bool verticalContact = playerBottom >= platformTop &&
                               playerBottom <= platformTop + 20; // Small tolerance

        return horizontalOverlap && verticalContact;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw ground
        _spriteBatch.Draw(
            _pixelTexture,
            new Rectangle(0, _groundY, _graphics.PreferredBackBufferWidth, 64),
            Color.SaddleBrown
        );

        // Draw platforms
        foreach (var platform in _platforms)
        {
            platform.Draw(_spriteBatch);
        }

        // Draw enemies
        foreach (var enemy in _enemies)
        {
            enemy.Draw(_spriteBatch);
        }

        // Draw player
        _player.Draw(_spriteBatch);

        // Draw score (simple text placeholder - just a colored box for now)
        // TODO: Add proper font rendering for score display

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
