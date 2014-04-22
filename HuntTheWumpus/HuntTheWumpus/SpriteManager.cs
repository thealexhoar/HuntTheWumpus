using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace HuntTheWumpus
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SpriteManager : DrawableGameComponent
    {
        
        List<Sprite> spriteList = new List<Sprite>();
        int arrowSpeed = 3;

        public SpriteManager(Game game, Player player)
            : base(game)
        {
            // TODO: Construct any child components here
        }
        
        public void SpawnArrow(Player player)
        {
            Vector2 speed = new Vector2(3,3);
            Vector2 position = player.position;

            Point frameSize = new Point(50,20);

            Sprite sprite = new Sprite(Game1.gameControl.arrow,
                    position, new Point(10, 3), 10, new Point(0, 0),
                    new Point(1, 1), new Vector2(0,0));
            Console.WriteLine("Added arrow");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            SpawnArrow(GameControl.player);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game1.spriteBatch.Begin();

            foreach (Sprite s in spriteList)
            {
                s.Draw(gameTime, Game1.spriteBatch);
                Console.WriteLine("IT DRAWS");
            }

            Game1.spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
