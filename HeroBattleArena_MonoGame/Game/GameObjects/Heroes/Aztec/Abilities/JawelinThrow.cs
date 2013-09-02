using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace HeroBattleArena.Game.GameObjects
{
    public class JawelinThrow : Ability
    {
        public JawelinThrow(Hero owner) :
			base(owner)
		{
		}
        public override void Initialize()
        {
          base.Initialize();
          Icon = Graphics.GetTexture("gui_aztecspear_icon");
          Cooldown = Configuration.GetValue("Slow_Down_Spear_Cooldown");
	      ManaCost = (int)Configuration.GetValue("Slow_Down_Spear_ManaCost");
          
        }
        public override bool Activate()
        {
            if (base.Activate())
            {
				SoundCenter.Instance.Play(SoundNames.AztecThrowSpear);
	        	Owner.StopMoving();
		        Owner.HeroState = HeroStates.SPECIAL5;
                return true;
            }
            return false;
        }
    }
}
