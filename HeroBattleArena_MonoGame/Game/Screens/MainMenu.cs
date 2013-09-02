using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.Screens
{
    public class MainMenu : Screen
    {
        private const int       _ITEMS = 4;
        private static float[]   _OFFSETS = { 155, 130, 130, 75 };
        private const float     _HEIGHT = 425;

        private int m_selected = 0;
        private Texture2D m_TexLogo = null;
        private Texture2D m_TexSelected = null;
        private Texture2D m_TexUnselected = null;

        public MainMenu()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            m_TexLogo          = Graphics.GetTexture("intrologo");
            m_TexSelected      = Graphics.GetTexture("menu_main_select");
            m_TexUnselected    = Graphics.GetTexture("menu_main_noselect");
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (ScreenManager.IsTrial)
            {
                ScreenManager.GetInstance().Add(new ModeSelectionMenu());
                return;
            }
    

            m_selected = GetEntryChanged(m_selected, _ITEMS, delta);

            if (GetEntrySelected())
            {
                switch (m_selected)
                {
                    case 0:
                        ScreenManager.GetInstance().Add(new ModeSelectionMenu());
                        break;
                    case 1:
						ScreenManager.GetInstance().Add(new OptionsMenu());
						break;
                    case 2:
						ScreenManager.GetInstance().Add(new CreditsScreen());
						break;
                    case 3:
                        Exit();
                        break;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            // Draw logo.
            Graphics.SpriteBatch.Draw(m_TexLogo, new Vector2(200, 50), Color.White);

            // Draw menu items.
            for (int i = 0; i < _ITEMS; ++i)
            {
                if (i == m_selected)
                {
                    Graphics.SpriteBatch.Draw(
                        m_TexSelected, 
                        new Vector2(512 - _OFFSETS[i], _HEIGHT + i * 60), 
                        new Rectangle(0, i*45, 453,45), Color.White);
                }
                else
                {
                    Graphics.SpriteBatch.Draw(
                        m_TexUnselected, 
                        new Vector2(512 - _OFFSETS[i], _HEIGHT + i * 60), 
                        new Rectangle(0, i * 45, 453, 45), Color.White);
                }
            }
        }
    }
}
