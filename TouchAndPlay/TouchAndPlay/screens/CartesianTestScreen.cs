using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouchAndPlay.screens
{
    class CartesianTestScreen:BasicScreen
    {
        private Texture2D logo;

        public CartesianTestScreen()
        {
            targetScreen = ScreenState.CARTESIAN_TEST_SCREEN;

            base.Initialize();
        }

        public void createScreen()
        {
            setScreenColor(Color.Aqua);

            addButton(30, 100, 100, 30, "Click me");
            addText(20, 40, "fdsfsdf", Color.Black);
            addText(20, 45, "Testing", Color.White); 
            addImage(logo, 70, 300);

            
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            //do not remove this
            base.LoadBasicContent(content);

            logo = content.Load<Texture2D>("logo");
        }

        public override void Update()
        {
            base.UpdateComponents();

            for (int index = 0; index < buttonsOnScreen.Count; index++)
            {
                if (buttonsOnScreen[index].isClicked())
                {
                    switch (buttonsOnScreen[index].label)
                    {

                        case "Click me":
                            ClearScreen();
                            break;
                    }

                }

            }
        }

    }
}
