using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HuntTheWumpus
{
    class GUIStubb
    {
        public static bool ScreenChecker(string roomName, bool mainMenuBool, bool highScoresBool, bool loadMenuBool, bool pauseMenuBool, bool optionsMenuBool, bool triviaPopupBool, bool deathScreenBool, bool gameScreenBool, bool breachMinigameBool)
        {
            bool var = true;
            return var;
        }

        public float playerMover(int playerVector)
        {

            return playerVector;
        }

        public bool popUpMap(GameWindow myGamewindow, bool isOpen)
        {
            
            return isOpen;
        }

        public int WumpusRoomMover(int locationVar)
        {
            return locationVar;
        }

        public void drawDoor(int Door1, int Door2, int Door3)
        {

        }

        public int fireArrow(bool hasArrows, int ammo)
        {
            return ammo;
        }

        public void hullBreach(int location, bool isFixed)
        {
            
        }

    }
}
