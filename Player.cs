using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SideScrollerGame_Maniacs;

public class Player
{
    // Physics
    public Vector2 Position;
    public Vector2 Velocity;

    // Player dimensions
    public int Width = 32;
    public int Height = 48;

    // Movement constants
    private const float MoveSpeed = 200f;
    private const float Gravity = 800f;
    private const float JumpForce = -350f;
    private const float MaxFallSpeed = 500f;

    // Variable jump constants
    private const float JumpCutMultiplier = 0.5f;  // Cut velocity to this when releasing jump early
    private const float HoldJumpGravityMultiplier = 0.5f;  // Reduced gravity while holding jump and rising

    // State
    public bool IsOnGround;
    private bool _isJumping;  // True while jump button held and still rising from a jump
    private KeyboardState _previousKeyboard;

    // Visual (placeholder rectangle)
    private Texture2D _texture;

    public Rectangle Bounds => new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        Width,
        Height
    );

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
        Velocity = Vector2.Zero;
        IsOnGround = false;
        _isJumping = false;
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        // Create a simple colored rectangle as placeholder
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.Red });
    }

    public void Update(GameTime gameTime, KeyboardState keyboard)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        bool jumpPressed = keyboard.IsKeyDown(Keys.Space) ||
                          keyboard.IsKeyDown(Keys.W) ||
                          keyboard.IsKeyDown(Keys.Up);
        bool jumpWasPressed = _previousKeyboard.IsKeyDown(Keys.Space) ||
                             _previousKeyboard.IsKeyDown(Keys.W) ||
                             _previousKeyboard.IsKeyDown(Keys.Up);

        // Horizontal movement
        Velocity.X = 0;

        if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
        {
            Velocity.X = -MoveSpeed;
        }
        if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
        {
            Velocity.X = MoveSpeed;
        }

        // Start jump (only on initial press)
        if (jumpPressed && !jumpWasPressed && IsOnGround)
        {
            Velocity.Y = JumpForce;
            IsOnGround = false;
            _isJumping = true;
        }

        // Variable jump: if player releases jump while rising, cut the velocity
        if (!jumpPressed && _isJumping && Velocity.Y < 0)
        {
            Velocity.Y *= JumpCutMultiplier;
            _isJumping = false;
        }

        // Stop "jumping" state when we start falling
        if (Velocity.Y >= 0)
        {
            _isJumping = false;
        }

        // Apply gravity
        if (!IsOnGround)
        {
            // Use reduced gravity while holding jump and rising (for floatier feel)
            float gravityMultiplier = (_isJumping && jumpPressed && Velocity.Y < 0)
                ? HoldJumpGravityMultiplier
                : 1f;

            Velocity.Y += Gravity * gravityMultiplier * deltaTime;

            // Cap fall speed
            if (Velocity.Y > MaxFallSpeed)
            {
                Velocity.Y = MaxFallSpeed;
            }
        }

        // Apply velocity to position
        Position += Velocity * deltaTime;

        _previousKeyboard = keyboard;
    }

    /// <summary>
    /// Bounce the player upward (used when stomping enemies)
    /// </summary>
    public void Bounce()
    {
        Velocity.Y = JumpForce * 0.7f;  // Smaller bounce than full jump
        _isJumping = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Bounds, Color.Red);
    }
}
