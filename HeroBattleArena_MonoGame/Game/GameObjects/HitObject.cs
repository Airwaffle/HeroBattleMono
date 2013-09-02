using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class HitObject : DamageObject
    {
        private Vector2 m_Direction = Vector2.Zero;
        private float m_Range = 0;

        public float Range { get { return m_Range; } protected set { m_Range = value; } }
        public Vector2 Direction { get { return m_Direction; } protected set { m_Direction = value; } }

        public HitObject(Unit owner):
            base(owner)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "HitObject";
        }

        public override void Update(float delta)
        {
            base.Update(delta);
        }
    }
}
