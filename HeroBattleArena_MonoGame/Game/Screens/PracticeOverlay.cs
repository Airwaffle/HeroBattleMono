using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.Screens
{
    class PracticeOverlay : Entity
    {
        private float m_BackAlpha = 0;
        private float m_BackAlphaWanted = 1;
        private float m_AlphaBlendSpeed = 0.02f;
        private float m_BackYPos = 300;
        private Vector2 m_MainPos = new Vector2(-100, 340);
        private Vector2 m_MainPosWanted = new Vector2(300, 340);
        private Vector2 m_StartPos = new Vector2(900, 385);
        private Vector2 m_StartPosWanted = new Vector2(340, 385);
        private float m_TextureSpeed = 0.05f;

        private Vector2 m_ReadyOffset = new Vector2(-50,-70);
        private Vector2 m_StartOffset = new Vector2(-15, -70);

        private bool[] m_HasAgreedToStart;
        private Texture2D m_BackTexture = null;
        private Texture2D m_MainTexture = null;
        private Texture2D m_StartTexture = null;
        private Texture2D m_ReadyButtonTexture = null;
        private Texture2D m_StartButtonTexture = null;


        public PracticeOverlay(int heroes) { 
            m_HasAgreedToStart = new bool[heroes];
        }

        public void PressedStart(int hero){
            m_HasAgreedToStart[hero] = true;
        }

        public void MoveOut()
        {
            m_BackAlphaWanted = 0;
            m_MainPosWanted.X = 1200;
            m_StartPosWanted.X = -400;
        }

        public override void Initialize()
        {
            m_BackTexture = Graphics.GetTexture("practice_strip");
            m_MainTexture = Graphics.GetTexture("practice_main");
            m_StartTexture = Graphics.GetTexture("practice_start");
            m_ReadyButtonTexture = Graphics.GetTexture("avatar_ready");
            m_StartButtonTexture = Graphics.GetTexture("avatar_start");
        }
        public override void Update(float delta)
        {
            m_BackAlpha = m_BackAlphaWanted * m_AlphaBlendSpeed + m_BackAlpha * (1 - m_AlphaBlendSpeed);
            m_MainPos.X = m_MainPosWanted.X * m_TextureSpeed + m_MainPos.X * (1 - m_TextureSpeed);
            m_StartPos.X = m_StartPosWanted.X * m_TextureSpeed + m_StartPos.X * (1 - m_TextureSpeed);

            if (m_BackAlpha <= 0.001)
            {
                Remove();
            }
        }
        public override void Draw(){

            Color backColor = new Color(new Vector4(1,1,1, m_BackAlpha));

            Graphics.Draw(m_BackTexture, new Vector2(0, m_BackYPos), null, 40, backColor);
            Graphics.Draw(m_MainTexture, m_MainPos, null, 41, Color.White);
            Graphics.Draw(m_StartTexture, m_StartPos, null, 41, Color.White);
            List<Hero> heroes = EntityManager.Heroes;
            if (m_BackAlphaWanted == 1)
            {
                for (int i = 0; i < heroes.Count; i++)
                {
                    if (m_HasAgreedToStart[i])
                        Graphics.Draw(m_ReadyButtonTexture, heroes[i].Position + m_ReadyOffset, null, 39, Color.White);
                    else
                        Graphics.Draw(m_StartButtonTexture, heroes[i].Position + m_StartOffset, null, 39, Color.White);
                }
            }
        }
    }
}
