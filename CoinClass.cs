using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Coins
{
    public class CoinClass
    {
        public Texture2D texture;
        public Rectangle rect;
        public Rectangle srect;

        public CoinClass(Texture2D texture, Rectangle rect, Rectangle srect)
        {
            this.texture = texture;
            this.rect = rect;
            this.srect = srect;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, srect, Color.White);
        }

    }
}
