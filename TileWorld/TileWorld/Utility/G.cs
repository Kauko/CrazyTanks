using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tileworld.Camera;
using Tileworld;
using Tileworld.Logging;

namespace Tileworld.Utility
{
    /*  Utility class for global variables
     * some of these should be set only once (such as the camera variable)
     * and some are changed throughout the game
     */
    public static class G
    {
        /* One time init variables (can a variable be called a variable if it's not varied?)*/
        //Moved this to services for now
        /* End of one time init variables */

        /* Global variables */
        public static GameState gameState;

        /* End of global variables */
    }
}
