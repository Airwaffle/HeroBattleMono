using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class Buff
    {
        
#region Fields

        private Unit m_Target;
        private float m_LifeTime = 1.0f;
        private bool m_bRemoved = false;

#endregion

#region Properties

        public bool IsRemoved { get { return m_bRemoved;  } }
        public Unit Target { get { return m_Target; } protected set { m_Target = value; } }
        public float LifeTime { get { return m_LifeTime; } protected set { m_LifeTime = value; } }

#endregion

#region Initialization

        public Buff(Unit target)
        {
            m_Target = target;
        }

        public virtual void Activate() { }
        public virtual void Deactivate() { }

#endregion

#region Methods

        public virtual void OnTargetCollide(AABB aabb) { }
        public virtual void OnTargetAttack() { }

        public virtual void Update(float delta)
        {
            m_LifeTime -= delta;
            if (m_LifeTime <= 0)
            {
                m_LifeTime = 0;
                Remove();
            }
        }

        public virtual void Remove()
        {
            m_bRemoved = true;
        }

#endregion

    }
}
