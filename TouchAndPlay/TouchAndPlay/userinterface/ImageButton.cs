using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TouchAndPlay.input;

namespace TouchAndPlay.userinterface
{
    class ImageButton:BasicButton
    {
        private Texture2D image;

        private Vector2 imagePos;

        public ImageButton(int xPos, int yPos, Texture2D image, Texture2D basicBox, SpriteFont spriteFont, string label = "")
            :base(xPos - (int)spriteFont.MeasureString(label).X/2 + image.Width/2, yPos + image.Height + 5, image.Width, image.Height, basicBox, spriteFont, label, StringAlignment.LEFT_JUSTIFIED)
        {
            this.image = image;
            this.imagePos = new Vector2(xPos, yPos);

            base.Initialize();

            base.collisionBox = new Rectangle(xPos, yPos, image.Width, image.Height);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch sprite)
        {
            base.Draw(sprite);

            switch (currentState)
            {
                case ButtonState.MOUSE_OUT:
                    sprite.Draw(image, imagePos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    break;
                case ButtonState.HOVERED:
                    sprite.Draw(image, imagePos, null, Color.White, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0f);
                    break;

            }

            
        }


    }
}
