using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SideScrollerGame_Maniacs;

public class Platform
{
    public Rectangle Bounds { get; private set; }
    private Texture2D _texture;

    public Platform(int x, int y, int width, int height)
    {
        Bounds = new Rectangle(x, y, width, height);
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Bounds, Color.ForestGreen);
    }
}
