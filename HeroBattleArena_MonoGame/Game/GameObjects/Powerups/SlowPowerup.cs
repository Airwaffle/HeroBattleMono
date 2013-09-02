using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class SlowPowerup : Powerup
    {
        private Texture2D m_TexShadow;

        public override void Initialize()
        {
            base.Initialize();

            Name = "Slow Powerup";
            Texture = Graphics.GetTexture("powerup_slowdown");
            DrawOffset = new Microsoft.Xna.Framework.Vector2(
                Position.X - 25, Position.Y - 25);
            Animations = new AnimationManager(50, 50, 6, 1);
            Animations.AddAnimation(new Animation(6, 0, 0, false, 0));

            m_TexShadow = Graphics.GetTexture("shadow_below_unit");
        }

        public override void Draw()
        {
            base.Draw();

            Graphics.Draw(
                m_TexShadow,
                Position + new Vector2(-12, 4),
                null, 1.5f, Color.White);
        }


        public override void OnPickedUp(Hero hero)
        {
            base.OnPickedUp(hero);

            hero.AddBuff(new SlowPUBuff(hero));
        }
    }
}
