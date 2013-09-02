using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    public class VampyricBoost : Ability
    {
        public VampyricBoost(Hero owner) :
			base(owner)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
            Icon = Graphics.GetTexture("gui_vampyric_icon");
            Cooldown = Configuration.GetValue("Vampyric_Cooldown");
            ManaCost = Configuration.GetValue("Vampyric_ManaCost");

        }
        public override bool Activate()
        {
            if (base.Activate())
            {
                SoundCenter.Instance.Play(SoundNames.ZombieDrainBoost);
                Owner.HeroState = HeroStates.SPECIAL6;
                Owner.Animations.ChangeAnimation(12 * 4 + (int)Owner.AnimationDirection);
                EntityManager.Spawn(new VampyricHit(Owner));
                return true;
            }
            return false;
        }
    }
}
