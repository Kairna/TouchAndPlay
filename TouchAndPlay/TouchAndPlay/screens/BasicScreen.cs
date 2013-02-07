using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TouchAndPlay.userinterface;
using Microsoft.Xna.Framework;

namespace TouchAndPlay.screens
{
    class BasicScreen
    {
        private Texture2D basicBox;

        protected Color screenColor;
        

        protected List<BasicButton> buttonsOnScreen;
        protected List<BasicText> textLabelsOnScreen;

        protected List<BasicComponent> componentsOnScreen;

        private Vector2 screenScale;

        private SpriteFont buttonFontStyle1;
        private SpriteFont buttonFontStyle2;
        
        internal ScreenState targetScreen;

        internal BasicScreenState transitionState;
        internal float transitionBoxAlpha;

        public void Initialize()
        {
            if (screenColor == null)
            {
                screenColor = Color.Black;
            }

            buttonsOnScreen = new List<BasicButton>();
            textLabelsOnScreen = new List<BasicText>();
            componentsOnScreen = new List<BasicComponent>();

            this.transitionBoxAlpha = 1.0f;

            this.transitionState = BasicScreenState.TRANSITION_IN;
        }

        public virtual void LoadContent(ContentManager content)
        {
            this.LoadBasicContent(content);
        }

        internal void LoadBasicContent(ContentManager content)
        {
            basicBox = content.Load<Texture2D>("stageboxes/basic_box_white");
            buttonFontStyle1 = content.Load<SpriteFont>("fonts/Button_FontStyle1");
            buttonFontStyle2 = content.Load<SpriteFont>("fonts/Button_FontStyle2");

            screenScale = new Vector2(GameConfig.APP_WIDTH / (float)basicBox.Width, GameConfig.APP_HEIGHT / (float)basicBox.Height);
        }

        public virtual void Update()
        {   //override this
            throw new NotImplementedException();
        }

        internal void UpdateComponents()
        {
            if (transitionState == BasicScreenState.ON_SCREEN_ACTIVE)
            {
                for (int count = 0; count < componentsOnScreen.Count; count++)
                {
                    componentsOnScreen[count].Update();
                }
            }
        }

        public virtual void Draw(SpriteBatch sprite){
            //draw background color
            sprite.Draw(basicBox, Vector2.Zero, null, screenColor, 0f, Vector2.Zero, screenScale, SpriteEffects.None, 1f);
            
            //draw all components
            for (int count = 0; count < componentsOnScreen.Count; count++)
            {
                componentsOnScreen[count].Draw(sprite);
            }

            //draw transition box
            DrawTransitionBox(sprite);
        }

        internal void DrawTransitionBox(SpriteBatch sprite)
        {
            switch( transitionState ){
                case BasicScreenState.TRANSITION_IN:
                    transitionBoxAlpha -= 0.05f;
                    if (transitionBoxAlpha <= 0)
                    {
                        transitionState = BasicScreenState.ON_SCREEN_ACTIVE;
                    }
                    break;
                case BasicScreenState.ON_SCREEN_ACTIVE:
                    transitionBoxAlpha = 0;
                    return;
                case BasicScreenState.TRANSITION_OUT:
                    transitionBoxAlpha += 0.05f;
                    if (transitionBoxAlpha >= 1.0f)
                    {
                        transitionState = BasicScreenState.GO_TO_TARGET_SCREEN;
                    }
                    break;
            }

            sprite.Draw(basicBox, Vector2.Zero, null, Color.Black * transitionBoxAlpha, 0f, Vector2.Zero, screenScale, SpriteEffects.None, 1f);
        }

        /* ==========================================================
         * PUBLIC METHODS
         * ==========================================================
         */

        public void addButton(int xPos, int yPos, int width, int height, string label = "", bool showText = true, StringAlignment alignment = StringAlignment.CENTER)
        {
            BasicButton button = new BasicButton(xPos, yPos, width, height, basicBox, buttonFontStyle1, label, alignment, showText);

            componentsOnScreen.Add(button);
            buttonsOnScreen.Add(button);
        }

        public void addImageButton(int xPos, int yPos, Texture2D image, string label)
        {
            ImageButton imageButton = new ImageButton(xPos, yPos, image, basicBox, buttonFontStyle1, label);

            componentsOnScreen.Add(imageButton);
            buttonsOnScreen.Add(imageButton);
        }

        public void addText(int xPos, int yPos, string text, Color color, BasicFonts font = BasicFonts.CG_14_REGULAR)
        {

            BasicText textToAdd = new BasicText(xPos, yPos, getSpriteFont(font), text, color);

            componentsOnScreen.Add(textToAdd);
            textLabelsOnScreen.Add(textToAdd);
        }

        private SpriteFont getSpriteFont(BasicFonts font)
        {
            switch (font)
            {
                case BasicFonts.CG_14_REGULAR:
                    return this.buttonFontStyle1;
                case BasicFonts.CG_12_REGULAR:
                    return this.buttonFontStyle2;
                default:
                    return this.buttonFontStyle1;
            }
        }

        public void addTextCenteredHorizontal(int xPos, int yPos, string text, Color color, BasicFonts font = BasicFonts.CG_14_REGULAR)
        {
            BasicText textToAdd = new BasicText(xPos, yPos, getSpriteFont(font), text, Color.White, StringAlignment.CENTER);

            componentsOnScreen.Add(textToAdd);
            textLabelsOnScreen.Add(textToAdd);
        }

        public void addTextCenteredVertical(int xPos, string text, Color color)
        {
            BasicText textToAdd = new BasicText(xPos, GameConfig.APP_HEIGHT/2, buttonFontStyle1, text, Color.White);

            componentsOnScreen.Add(textToAdd);
            textLabelsOnScreen.Add(textToAdd);
        }

        public void addFadeInTextOnScreen(string text, Color color)
        {

        }

        

        public void addTextCenteredOnScreen(string text, Color color)
        {
            BasicText textToAdd = new BasicText(GameConfig.APP_WIDTH/2, GameConfig.APP_HEIGHT / 2, buttonFontStyle1, text, Color.White, StringAlignment.CENTER);

            componentsOnScreen.Add(textToAdd);
            textLabelsOnScreen.Add(textToAdd);
        }

        public void addImage(Texture2D texture, float xPos, float yPos)
        {
            componentsOnScreen.Add(new BasicImage(texture,xPos,yPos));
        }

        public void addImage(Texture2D texture, float xPos, float yPos, float scale)
        {
            componentsOnScreen.Add(new BasicImage(texture, xPos, yPos, scale));
        }

        public void setScreenColor(Color color)
        {
            screenColor = color;
        }

        public BasicScreenState getTransitionState()
        {
            return transitionState;
        }

        public void ClearScreen()
        {

        }

        
    }
}
