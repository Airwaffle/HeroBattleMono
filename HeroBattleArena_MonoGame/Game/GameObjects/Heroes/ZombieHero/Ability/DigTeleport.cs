using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
namespace HeroBattleArena.Game.GameObjects
{
    public class DigTeleport : Ability
    {

        public DigTeleport(Hero owner) :
			base(owner)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
            Icon = Graphics.GetTexture("gui_gravedigger_icon");
            Cooldown = Configuration.GetValue("Dig_Teleport_Cooldown");
            ManaCost = Configuration.GetValue("Dig_Teleport_ManaCost");

        }
        public override bool Activate()
        {
            if (base.Activate())
            {
                Owner.HeroState = HeroStates.SPECIAL3;
                return true;
            }
            return false;
        }
    }
}
