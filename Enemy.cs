using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScrollerGame_Maniacs;

public class Enemy
{
    public Vector2 Position;
    public Vector2 Velocity;
    public bool IsAlive = true;

    public int Width = 32;
    public int Height = 32;

    private float _moveSpeed = 80f;
    private const float Gravity = 800f;

    // Patrol bounds
    private float _leftBound;
    private float _rightBound;

    private Texture2D _texture;

    public Rectangle Bounds => new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        Width,
        Height
    );

    public Enemy(Vector2 startPosition, float patrolDistance = 100f)
    {
        Position = startPosition;
        Velocity = new Vector2(-_moveSpeed, 0); // Start moving left
        _leftBound = startPosition.X - patrolDistance;
        _rightBound = startPosition.X + patrolDistance;
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public void Update(GameTime gameTime, int groundY)
    {
        if (!IsAlive) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Apply gravity
        Velocity.Y += Gravity * deltaTime;

        // Apply velocity
        Position += Velocity * deltaTime;

        // Ground collision
        if (Position.Y + Height >= groundY)
        {
            Position.Y = groundY - Height;
            Velocity.Y = 0;
        }

        // Patrol: reverse direction at bounds
        if (Position.X <= _leftBound)
        {
            Position.X = _leftBound;
            Velocity.X = _moveSpeed;
        }
        else if (Position.X + Width >= _rightBound)
        {
            Position.X = _rightBound - Width;
            Velocity.X = -_moveSpeed;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsAlive) return;

        // Draw enemy as orange rectangle
        spriteBatch.Draw(_texture, Bounds, Color.Orange);
    }

    /// <summary>
    /// Check collision with player. Returns collision type.
    /// </summary>
    public CollisionType CheckPlayerCollision(Player player)
    {
        if (!IsAlive) return CollisionType.None;

        if (!Bounds.Intersects(player.Bounds))
            return CollisionType.None;

        // Check if player is falling and hitting from above (stomp)
        // Player's feet should be near enemy's head
        float playerBottom = player.Position.Y + player.Height;
        float enemyTop = Position.Y;
        float playerPreviousBottom = playerBottom - player.Velocity.Y * 0.016f; // Approximate previous position

        // If player is moving downward and their bottom is near enemy top
        if (player.Velocity.Y > 0 && playerPreviousBottom <= enemyTop + 10)
        {
            return CollisionType.Stomped;
        }

        // Otherwise, player got hit from the side
        return CollisionType.HurtPlayer;
    }

    public void Stomp()
    {
        IsAlive = false;
    }
}

public enum CollisionType
{
    None,
    Stomped,
    HurtPlayer
}
