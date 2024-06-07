using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Coins
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Sprite playerSprite;

        private readonly int tileSize = 64;
        private Texture2D textureAtlas;
        private readonly Dictionary<Vector2, int> tilemap;
        private readonly List<Rectangle> textureStore;
        private List<Rectangle> intersections;
        private KeyboardState prevKeyState;

        private Texture2D background;
        private CoinClass redCoin;
        private CoinClass yellowCoin;
        private CoinClass azureCoin;
        private CoinClass grayCoin;
        private CoinClass blueCoin;
        private CoinClass greenCoin;
        private CoinClass blackCoin;
        private CoinClass violetCoin;
        private readonly List<CoinClass> coinList = new();

        private bool gameOver = false;
        private SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            tilemap = LoadMap("/Coins/Content/map.csv");
            textureStore = new()
            {
                new Rectangle(0,0,8,8),
                new Rectangle(0,8,8,8),
                new Rectangle(24,0,8,8),
            };
            intersections = new();

        }

        private static Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new();

            StreamReader reader = new(filepath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        if (value > 0)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }

                y++;
            }

            return result;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferHeight = tileSize * 23;
            _graphics.PreferredBackBufferWidth = tileSize * 13;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerSprite = new Sprite(
                Content.Load<Texture2D>("player1"),
                new Rectangle(tileSize + 1, tileSize * 20 + 1, tileSize, tileSize),
                new Rectangle(0, 0, 80, 90));

            textureAtlas = Content.Load<Texture2D>("atlas");

            background = Content.Load<Texture2D>("sky");

            CoinClass CreateCoin(Texture2D texture, int x, int y)
            {
                return new CoinClass(
                    texture,
                    new Rectangle(x * tileSize + 1, y * tileSize + 1, tileSize, tileSize),
                    new Rectangle(0, 0, tileSize, tileSize)
                );
            }

            redCoin = CreateCoin(Content.Load<Texture2D>("redCoin"), 10, 20);
            yellowCoin = CreateCoin(Content.Load<Texture2D>("yellowCoin"), 1, 16);
            azureCoin = CreateCoin(Content.Load<Texture2D>("azureCoin"), 7, 15);
            grayCoin = CreateCoin(Content.Load<Texture2D>("grayCoin"), 9, 11);
            blueCoin = CreateCoin(Content.Load<Texture2D>("blueCoin"),11, 8);
            blackCoin = CreateCoin(Content.Load<Texture2D>("blackCoin"), 2, 4);
            greenCoin = CreateCoin(Content.Load<Texture2D>("greenCoin"), 3, 11);
            violetCoin = CreateCoin(Content.Load<Texture2D>("violetCoin"), 7, 7);

            font = Content.Load<SpriteFont>("File");
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameOver)
            {
                return;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            playerSprite.Update(Keyboard.GetState(), prevKeyState, gameTime);

            prevKeyState = Keyboard.GetState();

            playerSprite.rect.X += (int)playerSprite.velocity.X;

            intersections = Collisions.getIntersectingTilesHorizontal(playerSprite.rect);

            foreach (var rect in intersections)
            {
                if (tilemap.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {
                    Rectangle collision = new(
                        rect.X * tileSize,
                        rect.Y * tileSize,
                        tileSize, tileSize);

                    if (!playerSprite.rect.Intersects(collision))
                    {
                        continue;
                    }

                    if (playerSprite.velocity.X > 0.0f)
                    {
                        playerSprite.rect.X = collision.Left - playerSprite.rect.Width;
                    }
                    else if (playerSprite.velocity.X < 0.0f)
                    {
                        playerSprite.rect.X = collision.Right;
                    }
                }
            }

            playerSprite.rect.Y += (int)playerSprite.velocity.Y;
            intersections = Collisions.getIntersectingTilesVertical(playerSprite.rect);

            playerSprite.Grounded = false;

            foreach (var rect in intersections)
            {
                if (tilemap.TryGetValue(new Vector2(rect.X, rect.Y), out int _val))
                {
                    Rectangle collision = new(
                                rect.X * tileSize,
                                rect.Y * tileSize,
                                tileSize, tileSize);

                    if (!playerSprite.rect.Intersects(collision))
                    {
                        continue;
                    }

                    if (playerSprite.velocity.Y > 0.0f)
                    {
                        playerSprite.rect.Y = collision.Top - playerSprite.rect.Height;
                        playerSprite.velocity.Y = 0.01f;
                        playerSprite.Grounded = true;
                        playerSprite.jumpCounter = 0;
                    }
                    else if (playerSprite.velocity.Y < 0.0f)
                    {
                        playerSprite.rect.Y = collision.Bottom;
                    }
                }
            }

            if (!playerSprite.Grounded && playerSprite.jumpCounter == 0)
            {
                playerSprite.jumpCounter++;
            }

            coinList.AddRange(new[] { redCoin, yellowCoin, azureCoin, grayCoin,
                blueCoin, blackCoin, greenCoin, violetCoin });

            foreach (var coin in coinList)
            {
                if (coin.rect.Intersects(playerSprite.rect))
                {
                    coin.rect = playerSprite.rect;
                }
            }

            Rectangle chest = new(tileSize * 7 + 1, tileSize * 2 + 1, tileSize, tileSize);

            if ((playerSprite.rect.Intersects(chest) && (redCoin.rect == playerSprite.rect) 
                && (yellowCoin.rect == playerSprite.rect) && (azureCoin.rect == playerSprite.rect) 
                && (grayCoin.rect == playerSprite.rect) && (blueCoin.rect == playerSprite.rect) 
                && (blackCoin.rect == playerSprite.rect) && (greenCoin.rect == playerSprite.rect) 
                && (violetCoin.rect == playerSprite.rect)) || (playerSprite.rect.Y > tileSize * 22))
            {
                gameOver = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(background, new Rectangle(0, 0, tileSize * 13, tileSize * 23), Color.White);

            foreach (var item in tilemap)
            {
                Rectangle dest = new(
                    (int)item.Key.X * tileSize,
                    (int)item.Key.Y * tileSize,
                    tileSize, tileSize
                    );
                Rectangle src = textureStore[item.Value - 1];
                _spriteBatch.Draw(textureAtlas, dest, src, Color.White);
            }

            redCoin.Draw(_spriteBatch);
            yellowCoin.Draw(_spriteBatch);
            azureCoin.Draw(_spriteBatch);
            grayCoin.Draw(_spriteBatch);
            blueCoin.Draw(_spriteBatch);
            blackCoin.Draw(_spriteBatch);
            greenCoin.Draw(_spriteBatch);
            violetCoin.Draw(_spriteBatch);

            playerSprite.Draw(_spriteBatch);

            _spriteBatch.DrawString(font, "Hi! I need to collect all coins", new Vector2(10, 10), Color.Yellow);
            _spriteBatch.DrawString(font, "and bring them to the chest!", new Vector2(10, 50), Color.Yellow);
            _spriteBatch.DrawString(font, "Use 'Z', 'X' and Space to move", new Vector2(10, 90), Color.YellowGreen);

            _spriteBatch.End();

            if (gameOver)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(background, new Rectangle(0, 0, tileSize * 13, tileSize * 23), Color.BlueViolet);
                _spriteBatch.DrawString(font, "Game over!", new Vector2(tileSize * 5, tileSize * 16), Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
