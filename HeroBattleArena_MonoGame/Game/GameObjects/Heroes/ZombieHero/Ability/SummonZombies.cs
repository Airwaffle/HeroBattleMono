using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class SummonZombies : Ability
    {
        public SummonZombies(Hero owner) :
			base(owner)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
#warning Needs summon zombie icon.
            Icon = Graphics.GetTexture("gui_summon_icon");
            Cooldown = Configuration.GetValue("Summon_Zombies_Cooldown");
            ManaCost = Configuration.GetValue("Summon_Zombies_ManaCost");
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
