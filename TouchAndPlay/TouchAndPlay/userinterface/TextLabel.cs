using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TouchAndPlay.userinterface
{
    class TextLabel
    {
        private int xPos;
        private int yPos;

        private SpriteFont fontStyle;
        private string label;
        private StringAlignment alignment;

        public TextLabel(int xPos, int yPos, SpriteFont fontStyle, string label = "", StringAlignment alignment = StringAlignment.CENTER)
        {
            this.xPos = xPos;
            this.yPos = yPos;

            this.fontStyle = fontStyle;
            this.alignment = alignment;

            Initialize();
            
        }

        private void Initialize()
        {
           
        }

        
    }
}
