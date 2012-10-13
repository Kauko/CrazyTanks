/// By Teemu Kaukoranta, member of the Oulu GamedevClub Stage
/// http://www.gamedevcenter.org
/// 
/// Part of the S.o.l.u.m project
/// Licensed under WTFPL - Do What The Fuck You Want To Public License
/// It would be nice if you don't remove this comment section though

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Solum.Camera;
using Solum;
using Solum.Logging;
using Solum.Input;

namespace Solum.Utility
{
    /*  Utility class for global variables
     * some of these should be set only once (such as the camera variable)
     * and some are changed throughout the game
     */
    public static class G
    {
        /* One time init variables (can a variable be called a variable if it's not varied?)*/
        //Gamepads have to be here instead of services, services cannot be used by instance     
        public static GamepadDevice gamePadOne;
        public static GamepadDevice gamePadTwo;
        public static GamepadDevice gamePadThree;
        public static GamepadDevice gamePadFour;
        public static List<LevelLibrary.Level> levels = new List<LevelLibrary.Level>();
        /* End of one time init variables */

        /* Global variables */
        public static GameState gameState;
        public static List<GamepadDevice> activeGamepads = new List<GamepadDevice>();

        /* Frame variables*/
        public static float redOneShield;
        public static float redTwoShield;
        public static float blueOneShield;
        public static float blueTwoShield;
        public static float greenOneShield;
        public static float greenTwoShield;
        public static float yellowOneShield;
        public static float yellowTwoShield;

        public static int redKills = 0;
        public static int redDeaths = 0;
        public static int blueKills = 0;
        public static int blueDeaths = 0;
        public static int greenKills = 0;
        public static int greenDeaths = 0;
        public static int yellowKills = 0;
        public static int yellowDeaths = 0;
        /* End of global variables */

        
    }
}
