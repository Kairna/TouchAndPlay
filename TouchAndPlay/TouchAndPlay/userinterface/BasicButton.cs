using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TouchAndPlay.input;

namespace TouchAndPlay.userinterface
{
    class BasicButton:BasicComponent
    {
        Texture2D buttonTexture;

        private int width;
        private int height;

        private Vector2 scale; 
        internal Rectangle collisionBox;

        internal ButtonState currentState;

        private int xMargin;
        private int yMargin;

        SpriteFont fontStyle;
        internal string label;
        private Vector2 stringOrigin;
        private Vector2 stringPosition;
        private StringAlignment alignment;
        private bool showText;

        public BasicButton(int xPos, int yPos, int width, int height, Texture2D texture, SpriteFont fontStyle, string label="", StringAlignment alignment = StringAlignment.CENTER, bool showText= true)
        {
            buttonTexture = texture;

            this.xPos = xPos;
            this.yPos = yPos;

            this.width = width;
            this.height = height;

            this.label = label;
            this.fontStyle = fontStyle;
            this.alignment = alignment;

            this.xMargin = 5;
            this.yMargin = 5;

            this.showText = showText;
            currentState = ButtonState.MOUSE_OUT;

            Initialize();
        }

        public void Initialize()
        {
            scale = new Vector2(this.width / (float)buttonTexture.Width, this.height / (float)buttonTexture.Height);
            //scale = new Vector2(1, 1);
            position = new Vector2(xPos, yPos);
            collisionBox = new Rectangle((int)xPos, (int)yPos, width, height);

            switch (alignment)
            {
                case StringAlignment.LEFT_JUSTIFIED:
                    stringOrigin = Vector2.Zero;
                    stringPosition = new Vector2(xPos + xMargin, yPos + yMargin);
                    break;
                case StringAlignment.CENTER:
                    stringOrigin = fontStyle.MeasureString(label) / 2;
                    stringPosition = new Vector2(xPos + width / 2, yPos + height / 2);
                    break;
                case StringAlignment.RIGHT_JUSTIFIED:
                    stringOrigin = fontStyle.MeasureString(label);
                    stringPosition = new Vector2(xPos + width - xMargin, yPos + yMargin);
                    break;
            }
        }

        public override void Update()
        {
            
            switch( currentState ) {
                case ButtonState.MOUSE_OUT:
                    if (MyMouse.isColliding(collisionBox))
                    {
                        currentState = ButtonState.HOVERED;
                        showText = true;
                    }
                    break;
                case ButtonState.HOVERED:
                    if (!MyMouse.isColliding(collisionBox))
                    {
                        currentState = ButtonState.MOUSE_OUT;
                        showText = false;
                    }
                    break;
  
            }
        }


        public override void Draw(SpriteBatch sprite)
        {
            //draw the box
            //if hovered, show text at center
            switch (currentState)
            {
                case ButtonState.MOUSE_OUT:
                    sprite.Draw(buttonTexture, position, null, Color.Transparent, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    break;
                case ButtonState.HOVERED:
                    sprite.Draw(buttonTexture, position, null, Color.Transparent, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    //if hovered show text
                    break;
            }

            //draw the text
            if (this.showText)
            {
                switch (currentState)
                {
                    case ButtonState.MOUSE_OUT:
                        sprite.DrawString(fontStyle, label, stringPosition, Color.Black, 0f, stringOrigin, 1f, SpriteEffects.None, 0f);
                        break;
                    case ButtonState.HOVERED:
                        sprite.DrawString(fontStyle, label, stringPosition, Color.White, 0f, stringOrigin, 1f, SpriteEffects.None, 0f);
                        break;
                }
            }
                     

            
        }

        internal bool isClicked()
        {
            if (currentState == ButtonState.HOVERED)
            {
                if (MyMouse.leftClicked())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
