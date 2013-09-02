using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game.GameObjects
{
   
    public class ZombieEmergingFromGround : Behavior
    {
        float m_animationTime = 1;
        public ZombieEmergingFromGround(Enemy enemy)
            : base(enemy)
        {
            Zombie zombie = (enemy as Zombie);
            m_animationTime = zombie.UnburrowSpeed;
            if (zombie.Name != "Zombie_Minion")
            {
                enemy.ChangeAnimation(7 * 4 + (int)enemy.AnimationDirection);
            }
            else
            {
                enemy.ChangeAnimation(9 * 4 + (int)enemy.AnimationDirection);
            }
            
            enemy.Animations.SetAnimationDuration(m_animationTime);
            zombie.State = Zombie.ZombieStates.UNBURROW;
        }
        public override void Update(float delta)
        {
            m_animationTime -= delta;
            if (m_animationTime <= 0){
                Zombie zombie = (Enemy as Zombie);
                zombie.State = Zombie.ZombieStates.IDLE;
                if (zombie.Owner == null)
                    zombie.ChangeBehavior(new ZombieRoam(Enemy));
                else
                    zombie.ChangeBehavior(new ZombieBodyGuard(Enemy, zombie.Owner));
                
            }   
        }
    }
}
