using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class DamagePowerup : Powerup
    {
        private Vector2 m_StartPos = Vector2.Zero;
        private float m_SinTime = 0.0f;
        private Texture2D m_TexShadow;

        public override void Initialize()
        {
            base.Initialize();

            m_StartPos.X = Position.X;
            m_StartPos.Y = Position.Y;

            Name = "Damage Powerup";
            Texture = Graphics.GetTexture("powerup_damage");
            DrawOffset = new Microsoft.Xna.Framework.Vector2(
                Position.X - 37, Position.Y - 70);
            Animations = new AnimationManager(75, 75, 1, 1);
            Animations.AddAnimation(new Animation(1, 0, 0, false, 0));

            m_TexShadow = Graphics.GetTexture("shadow_below_unit");
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            Position = new Vector2(Position.X, m_StartPos.Y + (float)Math.Sin(m_SinTime) * 10.0f);
            m_SinTime += delta*2;
        }

        public override void OnPickedUp(Hero hero)
        {
            base.OnPickedUp(hero);

            hero.AddBuff(new DamagePUBuff(hero));
        }

        public override void Draw()
        {
            base.Draw();

            Graphics.Draw(
                m_TexShadow,
                m_StartPos + new Vector2(-12, 4),
                null, 1.5f, Color.White);
        }
    }
}
