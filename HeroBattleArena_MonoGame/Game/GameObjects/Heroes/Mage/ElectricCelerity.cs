using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class ElectricCelerity: Ability
    {
    
  
        public ElectricCelerity(Hero owner): base(owner)
        {
        }

        public override void Initialize()
        {
	        Icon = Graphics.GetTexture("gui_electriccelerity_icon");
            Cooldown = Configuration.GetValue("ElectricCelerity_CoolDown");
            ManaCost = Configuration.GetValue("ElectricCelerity_ManaCost");
        }

        public override bool Activate()
        {
	        if(base.Activate())
	        {
                SoundCenter.Instance.Play(SoundNames.MageElectricCelerity);
		        Owner.AddBuff(new ElectricCelerityBuff(Owner));
		        Owner.HeroState = (HeroStates.SPECIAL1);
		        ElectricCelerityHit hit = new ElectricCelerityHit(Owner);
		        EntityManager.Spawn(hit);
		        hit.Position = Owner.Position;
		        return true;
	        }
	        return false;
        }
    }
}