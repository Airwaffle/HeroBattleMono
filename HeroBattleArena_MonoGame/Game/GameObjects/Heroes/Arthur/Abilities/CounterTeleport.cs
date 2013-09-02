using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
	public class CounterTeleport : Ability
	{
		public CounterTeleport(Hero owner) :
			base(owner)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			Icon = Graphics.GetTexture("gui_counterteleport_icon");

			Cooldown = Configuration.GetValue("Arthur_Counter_Cooldown");
			ManaCost = Configuration.GetValue("Arthur_Counter_ManaCost");
		}

		public override bool Activate()
		{
			if (base.Activate())
			{
                SoundCenter.Instance.Play(SoundNames.ArthurCounterTeleport);
				Owner.AddBuff(new CounterTeleportBuff(Owner));
				Owner.SetInvincibility(999);

				return true;
			}
			return false;
		}
	}
}
