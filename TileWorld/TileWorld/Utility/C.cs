using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tileworld.Utility
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
    }
}
