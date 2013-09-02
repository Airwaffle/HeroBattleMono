using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;

namespace HeroBattleArena.Game
{
    public class ScreenManager
    {
        private static ScreenManager s_Instance = null;
        private Screen m_LastActiveScreen = null;
        private List<Screen> m_Screens = new List<Screen>();

#if XBOX
        private static bool s_IsTrial = false;//Guide.IsTrialMode;
#else
        private static bool s_IsTrial = false;
#endif

        public static bool IsTrial { get { return s_IsTrial; } }

        private ScreenManager()
        {
        }

        public static ScreenManager GetInstance()
        {
            if (s_Instance == null)
            {
                s_Instance = new ScreenManager();
            }
            return s_Instance;
        }

        public void Update(float delta)
        {
            SoundCenter.Instance.Update(delta);

            #if XBOX
                //s_IsTrial = Guide.IsTrialMode;
            #endif

            // Remove screens marked for deletion
            for (int i = m_Screens.Count - 1; i >= 0; --i)
            {
                if(m_Screens[i].IsRemoved)
                    m_Screens.RemoveAt(i);
            }
            GC.Collect();
            // Detect when new a screen becomes active.
            if (m_Screens.Count > 0)
            {
                if (m_LastActiveScreen != m_Screens[m_Screens.Count - 1])
                {
                    m_LastActiveScreen = m_Screens[m_Screens.Count - 1];
                    m_LastActiveScreen.OnBecomeActive();
                }
            }

            // Update screens.
            int lastScreen = m_Screens.Count - 1;
            for (int i = 0; i < m_Screens.Count; ++i)
            {
                if (i != lastScreen && m_Screens[i].UpdateInBackground)
                {
                    m_Screens[i].Update(delta);
                }
                else if (i == lastScreen)
                {
                    m_Screens[i].Update(delta);
                    if (m_Screens[i].ExitOnEscape && Input.AnyWasPressed(InputCommand.MenuBack))
                    {
                        m_Screens[i].Exit();
                        SoundCenter.Instance.Play(SoundNames.InterfaceClickBack);
                    }
                }
            }
        }

        public void Draw()
        {
            int lastScreen = m_Screens.Count - 1;
            for (int i = 0; i < m_Screens.Count; ++i)
            {
                Graphics.Begin();

                if (i != lastScreen && m_Screens[i].DrawInBackground)
                {
                    m_Screens[i].Draw();
                }
                else if (i == lastScreen)
                {
                    m_Screens[i].Draw();
                }

                Graphics.End();
            }
        }

        public void Add(Screen screen)
        {
            screen.Initialize();
            m_Screens.Add(screen);
        }

        public void ClearScreens()
        {
            ClearScreens(0);
        }

        public void ClearScreens(int depth)
        {
            for (int i = depth; i < m_Screens.Count; ++i)
            {
                m_Screens[i].Exit();
            }
        }

        public int NumScreens
        {
            get { return m_Screens.Count; }
        }
        public Screen GetScreen(int nr)
        {
            if (nr < m_Screens.Count) return m_Screens[nr];
            else return null;
        }
    }
}
