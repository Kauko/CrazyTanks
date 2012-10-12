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
using Microsoft.Xna.Framework;

namespace Solum.Utility
{
    public static class C
    {
        /* Variables for camera */

        public const float camKeyboardScrollSpeed = 10.0f; //bigger = faster
        public const float camKeyboardRotationSpeed = 0.05f; // bigger = faster
        public const float camMouseDragSpeed = 1.5f; //Negative = push, positive = drag. bigger = faster
        public const float camKeyboardCloseZoomSpeed = 0.05f; //bigger = faster
        public const float camKeyboardFarZoomSpeed = 0.02f; //bigger = faster
        public const float camMouseScrollBorderWidth = 45.0f; //Wider border means cam starts scrolling sooner

        public const int camZoomSpeedThreshold = 1; // bigger value means farZoom is triggered farther away
        public const float camMouseFarZoomSpeed = 20.0f; // Bigger = slower (if camFarZoomSpeed > camCloseZoomSpeed -> camera zooms slower if cam is far away)
        public const float camMouseCloseZoomSpeed = 10.0f; // Bigger = slower
        public const float camMouseRotateSpeed = 500.0f; //bigger = slower

        /* End of camera variables */

        /* Variables used in the menus */
        public const int menuButtonWidth = 150; //width of a menu button
        public const int menuButtonHeight = 50; // height of a menu button
        public const int menuButtonVerticalOffset = 170; //if this is 0, the first button will be in the middle of the screen. Higher value means the first button is towards the top of the screen
        public const int menuButtonVerticalAddition = menuButtonHeight + 30; //the space between the menu 
        public const int playerSelectionWidgetPositionX = 50;
        public const int playerSelectionWidgetPositionY = 50;
        public const int playerSelectionWidgetVerticalOffsetY = 400;
        public const int playerSelectionWidgetHorizontalOffsetX = 300;
        /* End of menu variables */
    }
}
