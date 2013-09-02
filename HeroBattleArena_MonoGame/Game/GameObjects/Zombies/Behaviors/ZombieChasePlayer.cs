using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace HeroBattleArena.Game.GameObjects
{
    public class ZombieChasePlayer : Behavior
    {
        private Unit m_chased;
        private const float HERO_CHECK_NEW_INTERVALS = 0.5f;
        private const int DIAGONAL_BIAS = 10;
        private float m_heroCheck = 0;
        private float s_maxFollowDistanceSq = Configuration.GetValue("Zombie_Max_Follow_Distance");
        
        // zombie will get bored if walking into a wall for a certain time.
        private float m_standingStill = Configuration.GetValue("Zombie_Walk_Into_Wall_Time");
        private float m_standingStillCounter = 0;
        private Vector2 m_previousPosition = new Vector2();
        private float m_previousDelta = 0;

        public ZombieChasePlayer(Enemy enemy, Unit chased) : base(enemy)
        {
            m_chased = chased;

            // the position last frame
            m_previousPosition = new Vector2(enemy.Position.X, enemy.Position.Y);
            (enemy as Zombie).State = Zombie.ZombieStates.WALKING;
        }
        public override void Update(float delta)
        {
	        Enemy enemy = Enemy;

            // Increases the speed.
            enemy.MoveSpeed *= (enemy as Zombie).ChaseMulti;

	        Vector2 direction = m_chased.Position - enemy.Position;
            float dirLength = direction.Length();

            // Check so the zombie isn't stuck
            if (enemy.Position != m_previousPosition + enemy.Movement * enemy.MoveSpeed * m_previousDelta)
                m_standingStillCounter += delta;
            else
                m_standingStillCounter = 0;
            if (m_standingStillCounter >= m_standingStill)
            {
                enemy.ChangeBehavior(new ZombieRoam(enemy, Configuration.GetValue("Zombie_Ignore_After_Wall")));
                (enemy as Zombie).State = Zombie.ZombieStates.IDLE;
            }

	        // Check if the player is too far away...
            if (dirLength > s_maxFollowDistanceSq || !m_chased.IsAlive)
            {
                enemy.ChangeBehavior(new ZombieConfused(enemy));
		        //enemy.ChangeBehavior( new ZombieRoam( enemy ) );
                //(enemy as Zombie).State = Zombie.ZombieStates.IDLE;
            }
            else if (dirLength < enemy.AttackRange)
            {
                // the zombie won't attack when standing diagonally towards the enemy.
                if (direction.X < DIAGONAL_BIAS || direction.Y < DIAGONAL_BIAS)
                {
                    enemy.ChangeBehavior(new ZombieAttack(enemy, m_chased));
                } 
            } else 
            {
                m_heroCheck += delta;
                if (m_heroCheck >= HERO_CHECK_NEW_INTERVALS)
                {
                    m_heroCheck = 0;
                    List<Hero> heroes = EntityManager.Heroes;
                    Vector2 pos = enemy.Position;
                    float chasedDist = (pos - m_chased.Position).Length();
                    foreach (Hero hero in heroes)
                    {
                        if (hero != m_chased && hero.IsAlive && hero.IsVisible)
                        {
                            Vector2 dist = pos - hero.Position;
                            if (dist.Length() < chasedDist)
                            {
                                if (enemy.CheckAggro(hero))
                                {
                                    m_chased = hero;
                                    chasedDist = (pos - m_chased.Position).Length();
                                }
                            }
                        }
                    }
                }
            }
	        // Set movement.
	        direction.Normalize( );
	        enemy.Movement = direction;

            // Store position.
            m_previousPosition = new Vector2(enemy.Position.X, enemy.Position.Y);
            m_previousDelta = delta;
        }
    }
}
