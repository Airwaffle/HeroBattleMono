using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class BossDash : Behavior
    {
        private float m_DashTime = 1.15f;
        private float m_DashSpeed = 1500;
        private float m_DashSlowdown = 800;
        private Texture2D m_BossSheet;

        public BossDash(Enemy enemy, bool forceDashThrough): base(enemy)
        {
            enemy.ChangeAnimation((int)enemy.AnimationDirection+ 4*4);
            Boss boss = (enemy as Boss);
            boss.State = Boss.BossStates.DASH;
            //m_BossSheet = Graphics.GetTexture("boss_sprite");
            m_BossSheet = Graphics.LoadTexture("Characters/ZombieBoss/BossSheet");
            boss.HitSurround(true);

            if (boss.Level >= 4 || forceDashThrough)
            {
                boss.IsSolid = false;
            }
            else
            {
                boss.IsSolid = true;
            }

            if (boss.Level >= 6)
            {
                m_DashSpeed = 2500;
                m_DashTime = 0.7f;
            }
        }

        public override void Update(float delta)
        {
            if (m_DashTime > 0)
            {
                m_DashTime -= delta;
            }
            else
            {
                Enemy.ChangeBehavior(new BossIdle(Enemy, 5));
                Enemy.ChangeAnimation((int)Enemy.AnimationDirection);
                (Enemy as Boss).State = Boss.BossStates.WALKING;
            }
            Enemy.Movement = Enemy.Direction;
            Enemy.MoveSpeed = m_DashSpeed;
            m_DashSpeed *= (1.0f - (1.0f / (delta * m_DashSlowdown)));
        }
    }
}
