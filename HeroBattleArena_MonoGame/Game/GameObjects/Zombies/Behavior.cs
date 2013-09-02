using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class Behavior
    {
	    private Enemy m_enemy;
        protected Enemy Enemy { get { return m_enemy; } }

        public Behavior(Enemy enemy)
		{
			m_enemy = enemy;
		}
        public virtual void Update(float delta) { }
    }
}
