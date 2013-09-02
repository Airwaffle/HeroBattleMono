using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game
{
    public static class ScreenBounce
    {
        private static Vector2 m_Offset = new Vector2();
        private static float m_Amount = 0;
        private static float m_decreaseSpeed = 100;

        public static float Amount { set { m_Amount = value; } }
        public static Vector2 Offset { get { return m_Offset; } }
        public static void Reset(){m_Amount = 0; m_Offset = new Vector2();}

        //public static ScreenBounce() { }

        public static void Update(float delta)
        {

            if (m_Amount > 0){
                m_Amount -= delta * m_decreaseSpeed;

                m_Amount = Math.Abs(m_Amount);

                Random rnd = new Random();
                m_Offset.X = rnd.Next(0, (int)m_Amount) - m_Amount / 2;
                m_Offset.Y = rnd.Next(0, (int)m_Amount) - m_Amount / 2;
            }
        }
    }
}
