﻿/// By Teemu Kaukoranta, member of the Oulu GamedevClub Stage
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
        /* End of one time init variables */

        /* Global variables */
        public static GameState gameState;
        /* End of global variables */

        
    }
}
