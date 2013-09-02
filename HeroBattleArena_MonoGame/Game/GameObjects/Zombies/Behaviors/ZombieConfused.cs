using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class ZombieConfused : Behavior
    {
        private float m_confusedTime = Configuration.GetValue("Zombie_Puke_Time");

        public ZombieConfused(Enemy enemy)
            : base(enemy)
        {
            (enemy as Zombie).State = Zombie.ZombieStates.CONFUSED;
            enemy.ChangeAnimation(8 * 4 + (int)enemy.AnimationDirection);
            enemy.Animations.SetAnimationDuration(m_confusedTime);
        }

        public override void Update(float delta)
        {
            Enemy enemy = Enemy;

            enemy.Movement = new Vector2(0, 0);
            m_confusedTime -= delta;

            if (m_confusedTime <= 0)
            {
                enemy.ChangeBehavior(new ZombieRoam(enemy));
                (enemy as Zombie).State = Zombie.ZombieStates.IDLE;
            }
        }
    }
}
