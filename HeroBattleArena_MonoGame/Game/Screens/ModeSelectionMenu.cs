using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HeroBattleArena.Game.GameObjects;

namespace HeroBattleArena.Game.Screens
{
    public class ModeSelectionMenu : Screen
    {
        private const int _ITEMS = 4;
        private const float _HEIGHT = 425;
        private static float[] _OFFSETS = { 225, 175, 190, 155 };

        private int m_Selected = 0;

        private Texture2D TexLogo;
        private Texture2D TexSelected;
        private Texture2D TexUnselected;

        public ModeSelectionMenu()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            TexLogo = Graphics.GetTexture("intrologo");
            TexSelected = Graphics.GetTexture("menu_mode_select");
            TexUnselected = Graphics.GetTexture("menu_mode_noselect");
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (ScreenManager.IsTrial)
            {
                GameMode.Instance = new GM_FFA();
                GameMode.Instance.PrepareGame();
                GC.Collect();
                ScreenManager.GetInstance().Add(new CharacterSelectionMenu(Input.LastSelectID));
                return;
            }

	        m_Selected = GetEntryChanged(m_Selected, _ITEMS, delta);
	        if(GetEntrySelected()) 
            {
		        switch(m_Selected)
		        {
		        case 0:
			        GameMode.Instance = new GM_FFA();
			        break;
		        case 1:
                    GameMode.Instance = new GM_Team();
			        break;
		        case 2:
                    GameMode.Instance = new GM_Zombie();
			        break;
		        case 3:
                    ScreenManager.GetInstance().Add(new TutorialScreen(Input.LastSelectID));
			        return; // Return istead of break, since this is an oddity.
		        }
                GameMode.Instance.PrepareGame();
                GC.Collect();

                ScreenManager.GetInstance().Add(new CharacterSelectionMenu(Input.LastSelectID));
	        }
        }

        public override void Draw()
        {
            base.Draw();

	        // Draw the game logo.
	        Graphics.Draw(TexLogo, new Vector2(200,50), null, 0, Color.White);

	        // Draw the stuff.
	        for(int i = 0; i < _ITEMS; ++i) {
		        if(i == m_Selected) {
			        Graphics.Draw(
				        TexSelected,
				        new Vector2(512 - _OFFSETS[i], _HEIGHT + i * 60), // pos
				        new Rectangle(0, i * 45, 453, 45), // src
				        0.5f, Color.White);
		        } else {
			        Graphics.Draw(
				        TexUnselected,
				        new Vector2(512 - _OFFSETS[i], _HEIGHT + i * 60), // pos
				        new Rectangle(0, i * 45, 453, 45), // src
				        0.5f, Color.White);
		        }
	        }
        }
    }
}
