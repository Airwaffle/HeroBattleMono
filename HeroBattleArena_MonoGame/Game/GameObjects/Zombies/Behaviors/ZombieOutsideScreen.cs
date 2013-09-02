using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class ZombieOutsideScreen : Behavior
    {
        public ZombieOutsideScreen(Enemy enemy)
            : base(enemy)
        {
        
        }

        public override void Update(float delta)
        {
	        Enemy enemy = Enemy;

	        // Ignore collision when outside screen...
	        AABB aabb = enemy.MovementBB;
	        if(aabb.MinX < 0)
                Enemy.IsSolid = false;
            else if (aabb.MaxX > 1024)
                Enemy.IsSolid = false;
            else if (aabb.MinY < 0)
                Enemy.IsSolid = false;
            else if (aabb.MaxY > 768)
                Enemy.IsSolid = false;
            else
            {
                Enemy.IsSolid = true;
                if (enemy is Zombie)
                {
                    enemy.ChangeBehavior(new ZombieRoam(Enemy));
                    (enemy as Zombie).State = Zombie.ZombieStates.IDLE;
                }
                else if (enemy is Boss)
                {
                    enemy.ChangeBehavior(new BossIdle(Enemy, 1));
                }
            }
	        Vector2 movement = new Vector2(1024/2.0f, 768/2.0f) - enemy.Position;
	        movement.Normalize();	
	        Enemy.Movement = movement;
        }
    }
}
