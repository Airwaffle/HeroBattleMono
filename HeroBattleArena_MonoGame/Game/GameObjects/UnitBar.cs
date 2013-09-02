using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game
{
    public class UnitBar
    {
        private float m_ShowTime = 0;
        private float m_AmountRatio = 0;
        private Vector2 m_Offset = new Vector2(-25, 14);
        private Vector2 m_Size = new Vector2(45, 20);
        private Texture2D m_Texture = null;

        //public Vector2 Offset { set { m_Offset = value; } }
        public Vector2 Size { set { m_Size = value; } }
        public Vector2 Offset { set { m_Offset = value; } }  
        public UnitBar(Texture2D texture)
        {
            m_Texture = texture;
        }

        public void Show(float time)
        {
            m_ShowTime = time;
        }

        public void Hide()
        {
            m_ShowTime = 0;
        }

        public void Update(float delta)
        {
            if (m_ShowTime > 0)
            {
                m_ShowTime -= delta;
            }
        }

        /// <summary>
        /// Draws the bar only if private member "showTime" is bigger than 0.
        /// </summary>
        /// <param name="amount">The current amount. Ex: Health/Mana.</param>
        /// <param name="position">The units position.</param>

        public void Draw(Vector2 position, float amount, float maxAmount)
        {
            
            if (m_ShowTime > 0)
            {
                m_AmountRatio = amount / maxAmount;
                // Draw the HP/manaBar
                
                    Graphics.Draw(
                        m_Texture,
                        position + m_Offset,
                        new Rectangle((int)m_Size.X, 0, (int)m_Size.X, (int)m_Size.Y),
                        2.01f, Color.White);

                    Graphics.Draw(
                            m_Texture,
                            position + m_Offset + new Vector2(m_Size.X*(1-m_AmountRatio), 0),
                            new Rectangle(0, 0, (int)(m_Size.X * (m_AmountRatio)), (int)m_Size.Y),
                            2.01f, Color.White);
                
            }
        }
    }
}
