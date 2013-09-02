using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
    class MicroBotAbility: Ability
    {
        public MicroBotAbility(Hero owner) : base(owner) 
{
}



public override void Initialize()
{
	Cooldown = Configuration.GetValue("ability_microbot_cooldown");
	ManaCost = Configuration.GetValue("ability_microbot_manacost");

    Icon = Graphics.GetTexture("gui_microbot_icon");
}

public override bool Activate()
{
	if(!base.Activate()) return false;

    SoundCenter.Instance.Play(SoundNames.MageMicrobot);

    foreach (Buff buff in Owner.Buffs)
        if (buff is MicroBotBuff)
            buff.Remove();
    Owner.AddBuff(new MicroBotBuff(Owner as Unit));
	
	return true;
}

/*SDL_Surface* MicrobotAbility::getIcon() const
{
	static SDL_Surface* icon = Graphics::getSurface("gui_microbot_icon");
	return icon;
}*/
    }
}
