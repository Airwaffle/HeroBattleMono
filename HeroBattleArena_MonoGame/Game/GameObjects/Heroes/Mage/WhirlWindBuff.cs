using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class WhirlWindBuff : Buff
    {

        private float m_speed;

        private Vector2 m_move = new Vector2();

        public Vector2 Movement { get { return m_move * m_speed; } }

        public WhirlWindBuff(Unit target): base(target)
    {

	m_speed = Configuration.GetValue("Whirl_Wind_Speed");
	LifeTime = Configuration.GetValue("Whirl_Wind_Duration");

    }

public override void Update( float delta)
{
	base.Update(delta);

	Unit target = Target;
	target.Movement = (m_move*m_speed*delta);
}


public Vector2 DirectionVector{ get { return m_move; } set { m_move = value; } }



public override void Activate()
{
	Target.Control = false;

	

}


public override void Deactivate()
{
	Target.Control = true;
}



    }
}
