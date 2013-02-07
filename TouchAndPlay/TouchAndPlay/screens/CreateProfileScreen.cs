using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouchAndPlay.screens
{
    class CreateProfileScreen:BasicScreen
    {
        public CreateProfileScreen()
        {
            this.targetScreen = ScreenState.CREATE_PROFILE_SCREEN;
            base.Initialize();
        }

        public void createScreen()
        {
            setScreenColor(Color.Black);

            //addTextCenteredHorizontal(40, "CREATE NEW PROFILE", Color.White);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.LoadContent(content);
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
