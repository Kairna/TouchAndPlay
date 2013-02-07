using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouchAndPlay.userinterface
{
    class BasicComponent
    {
        protected float xPos;
        protected float yPos;

        protected Vector2 position;

        public Vector2 getPos()
        {
            return position;
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual void Draw(SpriteBatch sprite)
        {
            throw new NotImplementedException();
        }
    }
}
