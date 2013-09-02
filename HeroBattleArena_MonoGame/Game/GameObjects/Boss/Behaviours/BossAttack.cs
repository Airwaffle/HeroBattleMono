using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class BossAttack : Behavior
    {

        private float m_AttackTime = Configuration.GetValue("Boss_Attack_Time");
        private float m_StunRadius = Configuration.GetValue("Boss_Attack_Stun_Radius");
        private float m_StunTime = Configuration.GetValue("Boss_Attack_Stun_Time");

        private float m_ImpactDelay = 0.5f;
        private float m_Counter = 0;

        private static int s_ProjectileSpeed = (int)Configuration.GetValue("Boss_Projectile_Speed");
        
        public BossAttack(Enemy enemy)
            : base(enemy)
        {
            m_ImpactDelay *= m_AttackTime;
            enemy.ChangeAnimation(2 * 4 + (int)enemy.AnimationDirection);
            enemy.Animations.SetAnimationDuration(m_AttackTime);
            (enemy as Boss).State = Boss.BossStates.ATTACKING;
        }

        public override void Update(float delta)
        {
            Boss boss = Enemy as Boss;

            boss.Movement = new Vector2(0, 0);
            m_Counter += delta;

            if (m_Counter >= m_ImpactDelay)
            {
                SoundCenter.Instance.Play(SoundNames.bossZombieAttack);
                m_ImpactDelay = 1000; // Makes so this will only happen once
                BossHit hit = new BossHit(boss, boss.Direction);
                hit.Position = boss.Position + boss.Direction * 30;
                hit.Life = m_AttackTime;
                hit.Damage = boss.AttackDamage;
                EntityManager.Spawn(hit);

                if (boss.Level >= 5)
                {
                    BossBullet proj = new BossBullet(boss, boss.Direction);
                    proj.Position = boss.Position + boss.Direction * 30;
                    proj.Damage = boss.AttackDamage;
                    proj.Speed = s_ProjectileSpeed;
                    EntityManager.Spawn(proj);
                }

                /*
                List<Hero> heroes = EntityManager.Heroes;
                foreach (Hero hero in heroes)
                {
                    if ((hero.Position - hit.Position).Length() < m_StunRadius)
                    {
                        hero.SetStun(m_StunTime);
                        Effects.SpawnFollowing(hero as Unit, "Stunned_Effect", m_StunTime);
                    }
                }
                */
            }
            else if (m_Counter >= m_AttackTime)
            {
                boss.ChangeBehavior(new BossIdle(boss, 5));
                (boss as Boss).State = Boss.BossStates.WALKING;
            }
        }
    }
}
