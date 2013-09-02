using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class ZombieAttack : Behavior
    {
        private float m_attackTime = 0;
        private Unit m_chased;

        public ZombieAttack(Enemy enemy, Unit chased)
            : base(enemy)
        {
            m_chased = chased;
            m_attackTime = enemy.AttackSpeed;

            ZombieAttackHit hit = new ZombieAttackHit(enemy, enemy.Direction);

            hit.Position = enemy.Position + enemy.Direction * 25;
            hit.Life = m_attackTime;
            hit.Damage = enemy.AttackDamage;
            EntityManager.Spawn(hit);
            
            (enemy as Zombie).State = Zombie.ZombieStates.ATTACKING;
            enemy.ChangeAnimation(2 * 4 + (int)enemy.AnimationDirection);
            enemy.Animations.SetAnimationDuration(m_attackTime);
        }

        public override void Update(float delta)
        {
            Enemy enemy = Enemy;
            
            enemy.Movement = new Vector2(0, 0);
            m_attackTime -= delta;
            
            if (m_attackTime <= 0){
                enemy.ChangeBehavior(new ZombieChasePlayer(enemy, m_chased));
                (enemy as Zombie).State = Zombie.ZombieStates.WALKING;
            }
        }
    }
}