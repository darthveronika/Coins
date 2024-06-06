using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Input;

namespace Coins
{
    public class Sprite
    {
        public Texture2D texture;
        public Rectangle rect;
        public Rectangle srect;
        public Vector2 velocity;
        public bool Grounded { get; set; }
        public int Direction { get; set; }
        private readonly int numberJumps;
        public int jumpCounter;

        public Sprite(Texture2D texture, Rectangle rect, Rectangle srect)
        {
            this.texture = texture;
            this.rect = rect;
            this.srect = srect;
            velocity = new();
            Grounded = false;
            Direction = -1;
            numberJumps = 2;
            jumpCounter = 0;
        }

        public void Update(KeyboardState keyState, KeyboardState prevKeyState, GameTime gameTime)
        {

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            int prevDir = Direction;

            velocity.Y += 35.0f * dt;

            velocity.Y = Math.Min(25.0f, velocity.Y);

            if (keyState.IsKeyDown(Keys.X))
            {
                velocity.X += 32 * dt;
                Direction = -1;
            }
            if (keyState.IsKeyDown(Keys.Z))
            {
                velocity.X += -32 * dt;
                Direction = 1;
            }

            velocity.X = Math.Max(-300, Math.Min(300, velocity.X));

            velocity.X *= 0.96f;

            if (jumpCounter < numberJumps && keyState.IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space))
            {
                velocity.Y = -630 * dt;
                jumpCounter++;
            }

            if (prevDir != Direction)
            {
                srect.X += srect.Width;
                srect.Width = -srect.Width;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, srect, Color.White);
        }
    }
}