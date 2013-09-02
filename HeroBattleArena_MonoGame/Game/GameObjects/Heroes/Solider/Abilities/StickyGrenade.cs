using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class StickyGrenade : Ability
    {
        public StickyGrenade(Hero owner) :
			base(owner)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
            Icon = Graphics.GetTexture("gui_grenade_icon");
            Cooldown = Configuration.GetValue("Sticky_Grenade_Cooldown");
            ManaCost = Configuration.GetValue("Sticky_Grenade_ManaCost");
        }
        public override bool Activate()
        {
            if (base.Activate())
            {
                Owner.HeroState = HeroStates.SPECIAL1;
		        return true;
            }
            return false;
        }
    }
}
