using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace HeroBattleArena.Game.GameObjects
{
    public class ZombieBodyGuard : Behavior
    {
        private float m_heroCheck = 0;
        private const float HERO_CHECK_INTERVALS = 0.5f;
        private int m_width = 40;
        private int m_maxWalkRange = 50;
        private int m_walkRangeVariation = 30;
        private int m_walkRange = 50;
        private float m_walkTime = 0;
        private int m_reduceRate = 10;
        private Vector2 currentDir = new Vector2();
        private float m_waitTime = Configuration.GetValue("Zombie_Wait_Time");
        private float m_waitCounter = 0;
        private float m_ignoreAggroCounter = 0;

        private Unit m_Owner = null;

        private Vector2[] m_dirs = 
        {
        new Vector2(0, -1),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0) 
        };

        public ZombieBodyGuard(Enemy enemy, Unit owner)
            : base(enemy)
        {
            m_Owner = owner;
            m_maxWalkRange += m_walkRangeVariation / 2 - Enemy.RANDOM.Next(0, m_walkRangeVariation);
        }

        public int[] SortDirections()
        {
            int[] returnDirs = new int[4];
            bool[] alreadyChecked = new bool[4];

            for (int k = 0; k < 4; k++)
            {
                float closestDist = 999999;
                int chosen = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (!alreadyChecked[i])
                    {
                        Vector2 destination = (m_dirs[i] * m_maxWalkRange) + Enemy.Position;
                        float length = (destination - m_Owner.Position).Length();
                        if (length < closestDist)
                        {
                            closestDist = length;
                            chosen = i;
                        }
                    }
                }
                alreadyChecked[chosen] = true;
                returnDirs[k] = chosen;
            }

            return returnDirs;
        }

        public override void Update(float delta)
        {
            Enemy enemy = Enemy;
            Zombie zombie = enemy as Zombie;
            if (zombie.State == Zombie.ZombieStates.IDLE)
            {

                Vector2 pPos = enemy.Position;

                AABB[] collisionDirs = {
                   new AABB(
                       pPos.X - m_width / 2, 
                       pPos.Y - m_maxWalkRange, 
                       pPos.X + m_width / 2,
                       pPos.Y), // Up
                   new AABB(
                       pPos.X - m_maxWalkRange, 
                       pPos.Y - m_width/2, 
                       pPos.X, 
                       pPos.Y + m_width/2), // Left
                   new AABB(
                       pPos.X - m_width / 2, 
                       pPos.Y, 
                       pPos.X + m_width /2, 
                       pPos.Y + m_maxWalkRange), // Down
                   new AABB(
                       pPos.X, 
                       pPos.Y - m_width/2, 
                       pPos.X + m_maxWalkRange, 
                       pPos.Y + m_width/2) // Right
                  };

                for (int i = 0; i < 4; i++)
                {
                    collisionDirs[i].CollisionMask = AABBLayers.CollisionHeroFeet;
                    collisionDirs[i].LayerMask = AABBLayers.LayerHeroFeet;
                }

                m_walkRange = m_maxWalkRange;
                bool foundWay = false;
                int nrOfTimes = m_maxWalkRange / m_reduceRate;
                int[] priority = SortDirections();

                while (!foundWay && nrOfTimes > 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        List<AABB> boxes = Collision.GetCollidingBoxes(collisionDirs[priority[i]]);

                        //Checks if the only colliding box is the zombie itself
                        if (boxes.Count <= 1)
                        {
                            zombie.State = Zombie.ZombieStates.WALKING;
                            currentDir = m_dirs[priority[i]];
                            m_walkTime = m_walkRange / zombie.MoveSpeed;
                            foundWay = true;
                            break;
                        }
                    }

                    if (foundWay)
                    {
                        break;
                    }
                    // reduce walk range and repeat!
                    collisionDirs[0].MinY -= m_reduceRate;
                    collisionDirs[1].MinX -= m_reduceRate;
                    collisionDirs[2].MaxY -= m_reduceRate;
                    collisionDirs[3].MaxX -= m_reduceRate;
                    m_walkRange -= m_reduceRate;
                    nrOfTimes--;

                    if (nrOfTimes <= 0)
                    {
                        // Cant find a way (so we walk throught the wall, wtf?)
                        enemy.Movement = new Vector2(0, 1);
                    }

                }
            }
            else if (zombie.State == Zombie.ZombieStates.WALKING)
            {
                zombie.Movement = currentDir;
                m_walkTime -= delta;
                if (m_walkTime <= 0)
                {
                    m_walkTime = 0;
                    m_waitCounter = m_waitTime;
                    zombie.State = Zombie.ZombieStates.FROZEN;
                }
            }
            else if (zombie.State == Zombie.ZombieStates.FROZEN)
            {
                m_waitCounter -= delta;
                zombie.Movement = new Vector2();
                if (m_waitCounter <= 0)
                {
                    m_waitCounter = 0;
                    zombie.State = Zombie.ZombieStates.IDLE;
                }
            }


            // Check for nearby players.
            if (m_ignoreAggroCounter <= 0)
            {
                m_heroCheck += delta;
                if (m_heroCheck > HERO_CHECK_INTERVALS)
                {
                    m_heroCheck -= HERO_CHECK_INTERVALS;


                    // If the zombie got a owner, we want it to aggro all the units, 
                    // if it doesn't we only want it to aggro heroes.
                    List<Unit> units = new List<Unit>();
                    if (zombie.Owner == null)
                    {
                        List<Hero> heroes = EntityManager.Heroes;
                        List<Enemy> pets = EntityManager.Pets;

                        for (int i = 0; i < heroes.Count; i++)
                        {
                            units.Add(heroes[i] as Unit);
                        }
                        for (int i = 0; i < pets.Count; i++)
                        {
                            units.Add(pets[i] as Unit);
                        }
                    }
                    else
                    {
                        units = EntityManager.Units;
                    }

                    Vector2 pos = enemy.Position;

                    foreach (Unit unit in units)
                    {
                        if (unit.IsAlive)
                        {
                            Vector2 dist = pos - unit.Position;
                            if (dist.Length() < 200 && unit.IsVisible && unit.IsAlive)
                            {
                                if (unit is Hero)
                                {
                                    if (enemy.CheckAggro(unit as Hero))
                                    {
                                        enemy.IsSolid = true;
                                        enemy.ChangeBehavior(new ZombieChasePlayer(Enemy, unit));
                                        break;
                                    }
                                }
                                else if (unit is Enemy)
                                {
                                    // We dont want the zombies that are owned by a hero to aggro eachother.
                                    if ((unit as Enemy).Owner != enemy.Owner)
                                    {
                                        enemy.IsSolid = true;
                                        enemy.ChangeBehavior(new ZombieChasePlayer(Enemy, unit));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                m_ignoreAggroCounter -= delta;
            }
        }
    }
}
