using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchAndPlay.userinterface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TouchAndPlay.screens
{
    class MenuScreen:BasicScreen
    {
        public Texture2D gameLogo;
        //private Texture2D menuScreenBG;
        private Texture2D welcomeLogo;
        private Texture2D startGameIcon;
        private Texture2D statIcon;
        private Texture2D optIcon;
        private Texture2D exitIcon;

        public MenuScreen(Color screenColor)
        {
            this.screenColor = screenColor;

            this.targetScreen = ScreenState.MENU_SCREEN;
            Initialize();
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadBasicContent(content);

            gameLogo = content.Load<Texture2D>("logo");
            welcomeLogo = content.Load<Texture2D>("screens/menuscreen/img_welcomebanner");
            startGameIcon = content.Load<Texture2D>("screens/menuscreen/icon_startgame");
            statIcon = content.Load<Texture2D>("screens/menuscreen/icon_statistics");
            optIcon = content.Load<Texture2D>("screens/menuscreen/icon_options");
            exitIcon = content.Load<Texture2D>("screens/menuscreen/icon_exit");
        }

        public void createScreen(){
            //change background color
            setScreenColor(Color.Black);

            //add images
            addImage(welcomeLogo, 0, 30);
            addImage(gameLogo, GameConfig.APP_WIDTH / 2 - gameLogo.Width / 2, 60);
            //addImage(startGameIcon, 185, 300);
            //addImage(statIcon, 260, 300);
            //addImage(optIcon, 335, 300);
            //addImage(exitIcon, 410, 300);
            
            //add buttons
            //addButton(185, 300, 56, 53, "START GAME", false, StringAlignment.CENTER);
            addImageButton(185, 300, startGameIcon, "START GAME");
            addImageButton(260, 300, statIcon, "STATISTICS");
            addImageButton(335, 300, optIcon, "OPTIONS");
            addImageButton(410, 300, exitIcon, "QUIT TAP");

            //
            
            //add text
            addText(10, 37, "HI, ABCDEFGHIJKLMN!", Color.White, BasicFonts.CG_14_REGULAR);
            addText(GameConfig.APP_WIDTH / 2, 460, "Copyright Kunwari 2012", Color.White, BasicFonts.CG_12_REGULAR);
        }

        public override void Update()
        {
            UpdateComponents();
            //update button states

            for (int count = 0; count < buttonsOnScreen.Count; count++)
            {
                if (buttonsOnScreen[count].isClicked())
                {
                    switch (buttonsOnScreen[count].label)
                    {
                        case "START GAME":
                            targetScreen = ScreenState.STAGE_SCREEN;
                            transitionState = BasicScreenState.TRANSITION_OUT;
                            break;
                        case "STATISTICS":
                            break;
                        case "EXIT":ss
                            break;
                    }
                }
            }
            
        }
    }
}
