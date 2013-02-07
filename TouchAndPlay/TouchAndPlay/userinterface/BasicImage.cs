using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TouchAndPlay.userinterface
{
    class BasicImage:BasicComponent
    {
        private Texture2D imageTexture;
        private float scale;

        public BasicImage(Texture2D imageTexture, float xPos, float yPos, float scale = 1.0f)
        {
            this.imageTexture = imageTexture;
            this.xPos = xPos;
            this.yPos = yPos;
            this.scale = scale;

            this.position = new Vector2(xPos, yPos);
        }

        public override void Update()
        {


        }

        public override void Draw(SpriteBatch sprite)
        {
            sprite.Draw(imageTexture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
    }
}
