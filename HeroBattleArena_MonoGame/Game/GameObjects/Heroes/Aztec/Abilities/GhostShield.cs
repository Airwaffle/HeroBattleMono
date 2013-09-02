using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class GhostShield: Ability
    {
        public GhostShield(Hero owner): base(owner) {}

        public override void Initialize()
        {
            Cooldown = Configuration.GetValue("GhostShield_CoolDown");
            ManaCost = Configuration.GetValue("GhostShield_ManaCost");

            Icon = Graphics.GetTexture("gui_ghostshield_icon");
        }

        public override bool Activate()
        {
	        if(base.Activate())
            {
	        Owner.AddBuff(new GhostShieldBuff(Owner as Hero));
            SoundCenter.Instance.Play(SoundNames.AztecGhostShield);
		        return true;
	        }

	       return false;
        }

    }
}
