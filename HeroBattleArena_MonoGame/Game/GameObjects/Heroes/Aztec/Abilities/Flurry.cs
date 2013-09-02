using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class Flurry : Ability
    {

        public Flurry(Hero owner) :
			base(owner)
		{
		}

        public override void Initialize()
        {
	        Cooldown = Configuration.GetValue("Flurry_CoolDown");
	        ManaCost = ((int)Configuration.GetValue("Flurry_ManaCost"));
            Icon = Graphics.GetTexture("gui_flurry_icon");
        }

        public override bool Activate()
        {
	        if(base.Activate())
	        {
		        FlurryAttack flurryAttack = new FlurryAttack(Owner, Owner.Direction, this);
                flurryAttack.Position = Owner.Position;
                Owner.CanAttack = false;
                Owner.HeroState = HeroStates.SPECIAL1;
		        EntityManager.Spawn(flurryAttack);
        		
		        return true;
	        }
	        return false;
        }
    }
}
