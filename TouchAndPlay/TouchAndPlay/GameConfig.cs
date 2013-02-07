using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouchAndPlay
{
    static class GameConfig
    {
        public const int BUBBLE_WIDTH = 100;

        public const int SETBUBBLE_DURATION = 1200;
        public const int DRAGBUBBLE_DURATION = 1200;
        public static int BUBBLE_SOLO_DURATION = 1200;

        public const float LINE_ALPHA = 0.60f;

        public const int DRAG_BUBBLE_INTERVAL = 1200;
        public const int SOLO_BUBBLE_INTERVAL = 300;
        public const int BUBBLE_SET_INTERVAL = 700;

        public const int SOLO_BUBBLES_TO_POP = 5;
        public const int BUBBLE_SETS_TO_POP = 5;
        public const int DRAG_BUBBLES_TO_POP = 10;

        public const int MIN_GAP_BETWEEN_BUBBLES = 5;

        public const int DRAG_BUBBLE_SPEED = 3;
        public const float DRAG_BUBBLE_RADIAN_RANGE = (float)Math.PI/12f;

        public readonly static Color DEFAULT_EFFECT_COLOR = Color.LightBlue;
 
        public const int LOCK_COUNT = 10;

        public static int DRAG_BUBBLE_POPCOUNT = 5;

        public static int BUBBLE_POP_TIME = 5;

        public static int MESSAGE_SWITCH_TIME = 40;
        public static int ANGLE_ADJUST_WAIT_TIME = 70;

        public static int RESUME_COUNT = 320;

        public const int APP_WIDTH = 640;
        public const int APP_HEIGHT = 480;

        
    }
}
