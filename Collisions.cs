using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Coins
{
    public class Collisions
    {
        
        public static List<Rectangle> getIntersectingTilesHorizontal(Rectangle target)
        {
            int tileSize = 64;
            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % tileSize)) / tileSize;
            int heightInTiles = (target.Height - (target.Height % tileSize)) / tileSize;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {
                    intersections.Add(new Rectangle(
                        (target.X + x * tileSize) / tileSize,
                        (target.Y + y * (tileSize - 1)) / tileSize,
                        tileSize, tileSize
                        ));
                }
            }
            return intersections;
        }

        public static List<Rectangle> getIntersectingTilesVertical(Rectangle target)
        {
            int tileSize = 64;
            List<Rectangle> intersections = new();

            int widthInTiles = (target.Width - (target.Width % tileSize)) / tileSize;
            int heightInTiles = (target.Height - (target.Height % tileSize)) / tileSize;

            for (int x = 0; x <= widthInTiles; x++)
            {
                for (int y = 0; y <= heightInTiles; y++)
                {
                    intersections.Add(new Rectangle(
                        (target.X + x * (tileSize - 1)) / tileSize,
                        (target.Y + y * tileSize) / tileSize,
                        tileSize, tileSize
                        ));
                }
            }
            return intersections;
        }
    }
}
