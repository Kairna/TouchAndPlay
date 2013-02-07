using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TouchAndPlay.utils;

namespace TouchAndPlay.screens
{
    class AboutScreen:BasicScreen
    {
        private Texture2D dot;
        private List<Vector2> points;

        public AboutScreen()
        {
            this.targetScreen = ScreenState.ABOUT_SCREEN;

            this.points = Randomizer.getEvenListOfPoints(300, 5, 5);

            Initialize();
        }

        public void createSreen()
        {
            setScreenColor(Color.Green);

            
            for (int count = 0; count < points.Count; count++)
            {
                addImage(dot, points[count].X + 300, points[count].Y + 200, 0.5f);
            }
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadBasicContent(content);

            dot = content.Load<Texture2D>("effects/basic_particle");
        }

        public override void Update()
        {
            base.UpdateComponents();
        }
    }
}
