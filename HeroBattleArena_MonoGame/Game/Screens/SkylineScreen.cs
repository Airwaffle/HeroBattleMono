using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace HeroBattleArena.Game.Screens
{
    public class SkylineScreen : Screen
    {
        private static bool s_bFirstPanorama = true;

        private float m_Vobble          = 2;
        private float m_YPos            = 768-972;
        private float m_Speed           = 200;
        private Texture2D m_Background  = null;

        private static bool s_bVisible = true;
        public static bool Visible { get { return s_bVisible; } set { s_bVisible = value; } }

        private bool addedMainOnce = false;
        public SkylineScreen()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            m_Background = Graphics.GetTexture("introsplash");
            SoundCenter.Instance.PlayMusic(MusicNames.Slugwar);
	        DrawInBackground = true;
	        UpdateInBackground = true;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (ScreenManager.IsTrial && !addedMainOnce)
            {
                ScreenManager.GetInstance().Add(new MainMenu());
                addedMainOnce = true;
            }
            

            if (s_bVisible)
            {
                if (s_bFirstPanorama)
                {
                    m_YPos += m_Speed*delta*m_YPos*-0.01f;
                    if(m_YPos >= -5.5f)
                    {
                        s_bFirstPanorama = false;
                        if (!addedMainOnce)
                        {
                            ScreenManager.GetInstance().Add(new MainMenu());
                            addedMainOnce = true;
                        }
                    }
                }
                else
                {
                    m_Vobble += 0.5f*delta;
                    m_YPos = -50 + (float)Math.Sin((double)m_Vobble)*50.0f;
                }
            }
            if (ScreenManager.GetInstance().NumScreens == 1 && !s_bFirstPanorama)
            {
                Exit();
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (!s_bVisible) return;

            Graphics.SpriteBatch.Draw(m_Background, Vector2.Zero, new Rectangle(0, (int)m_YPos, 1024, 768), Color.White);
        }
    }
}
