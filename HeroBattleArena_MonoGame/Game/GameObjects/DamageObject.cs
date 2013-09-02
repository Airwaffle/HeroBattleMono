using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class DamageObject : Entity
    {
        private float m_Life = 0;
        private Unit m_Owner = null;
        private float m_Damage = 0;

        public Unit Owner { get { return m_Owner; } }
        public float Damage { get { return m_Damage; } set { m_Damage = value; } }
        public float Life { get { return m_Life; } set { m_Life = value; } }

        public DamageObject(Unit owner)
        {
            m_Owner = owner;
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "DamageObject";
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            m_Life -= delta;
            if (m_Life <= 0)
                Remove();
        }
    }
}
