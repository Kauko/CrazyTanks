using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileWorld.Camera;

namespace TileWorld.Utility
{
    /*  Utility class for global variables
     * some of these should be set only once (such as the camera variable)
     * and some are changed throughout the game
     */
    public static class G
    {
        /* One time init variables (can a variable be called a variable if it's not varied?)*/
        public static Camera2d cam;
        /* End of one time init variables */

        /* Global variables */

        /* End of global variables */
    }
}
