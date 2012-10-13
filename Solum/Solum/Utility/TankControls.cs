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
using Solum.Utility;
using Solum.Input;
using Solum.Logging;

namespace Solum.Utility
{
    public enum ControlSide
    {
        Right,
        Left
    }

    class TankControls
    {
        public ControlSide darkside;
        public Buttons shoot;
        public Buttons changeWeapon;
        public Buttons turretRotateCW;
        public Buttons turretRotateCCW;
        public Buttons reverse;

        public TankControls(ControlSide side)
        {
            darkside = side;

            if (darkside == ControlSide.Left)
            {
                turretRotateCW = Buttons.DPadLeft;
                turretRotateCCW = Buttons.DPadRight;
                shoot = Buttons.LeftShoulder;
                changeWeapon = Buttons.LeftTrigger;
                reverse = Buttons.LeftStick;
            }
            else
            {
                turretRotateCW = Buttons.B;
                turretRotateCCW = Buttons.X;
                shoot = Buttons.RightShoulder;
                changeWeapon = Buttons.RightTrigger;
                reverse = Buttons.RightStick;
            }
        }
    }
}
