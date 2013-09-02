using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class MicroBotBuff: Buff
    {
        private List<MicroBot> m_microbots = new List<MicroBot>();

        public MicroBotBuff(Unit target): base(target)
{
}

~MicroBotBuff()
{
	// This doesn't memory leak, microbots are deleted
	// in entity manager.
    m_microbots.Clear();
}

public override void Activate()
{
	LifeTime = Configuration.GetValue("ability_microbot_duration");

	MicroBot microbot;
	microbot = new MicroBot(Target as Hero, true);
	EntityManager.Spawn(microbot);
	m_microbots.Add(microbot);
    microbot = new MicroBot(Target as Hero, false);
	EntityManager.Spawn(microbot);
	m_microbots.Add(microbot);
}

public override void OnTargetAttack()
{
	for(int i = 0; i < m_microbots.Count; ++i)
	{
		m_microbots[i].Attack();
	}
}
public override void Deactivate()
{
	for(int i = 0; i < m_microbots.Count; ++i)
	{
		m_microbots[i].Remove();
	}
}


    }
}
