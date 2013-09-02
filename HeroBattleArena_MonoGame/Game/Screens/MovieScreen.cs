using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game
{
    class MovieScreen : Screen
    {
        private float m_Lifetime = 1;
        private Texture2D[] m_Backgrounds = new Texture2D[4];
        private int m_Level = 0;

        public MovieScreen(int level){
            m_Level = level;
        }

        public override void Initialize()
        {
            base.Initialize();
            /*
            m_Backgrounds[0] = Graphics.GetTexture("load_arthur");
            m_Backgrounds[1] = Graphics.GetTexture("load_stryker");
            m_Backgrounds[2] = Graphics.GetTexture("load_quetzali");
            m_Backgrounds[3] = Graphics.GetTexture("load_nano");
             */
            m_Backgrounds[0] = Graphics.GetTexture("load_arthur");
            m_Backgrounds[1] = Graphics.GetTexture("load_arthur");
            m_Backgrounds[2] = Graphics.GetTexture("load_arthur");
            m_Backgrounds[3] = Graphics.GetTexture("load_arthur");
        }
        public override void Update(float delta)
        {
            base.Update(delta);
            m_Lifetime -= delta;
            if (m_Lifetime <= 0)
                Exit();
        }
        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(m_Backgrounds[m_Level], new Vector2(0, 0), null, 60, Color.Black);
        }
    }
}
