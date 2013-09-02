using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class MiddleWalkSpawn : Behavior
    {
        private Vector2 m_Destination = new Vector2(1024 / 2.0f, 768 / 2.0f);
        private bool m_InMiddle = false;
        private float m_SpawnCounter = 0;
        private float m_PositionCounter = 0;
        private float m_SpawnDistance = 100;
        private float m_DistanceIncrease = 20;
        private float m_SpawnDifference = 0.2f;
        private float m_SpawnInterval = 0.07f;
        private float m_NumberOfSpawns = 12;
        private bool m_Once = true;

        private int m_Formation = 0;

        private Vector2[] m_Dirs = 
        {
        new Vector2(0, -1),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(1, 0) 
        };
        private Random m_Random = new Random();

        public MiddleWalkSpawn(Enemy enemy)
            : base(enemy)
        {
            m_Formation = 2;

            int currentMap = AIManager.GetMapNumber();
            if (currentMap == 0)
            {
                m_Destination.Y = 180;
            }
            
            /*m_Formation = (enemy as Boss).Level;
            if (m_Formation > 5)
            {
                m_NumberOfSpawns = 16;
            }*/
             
        }
        public override void Update(float delta)
        {
            Enemy enemy = Enemy;
            if (!m_InMiddle)
            {
                //AABB aabb = enemy.MovementBB;
	            /*
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
                }
                */
                Enemy.IsSolid = false;

                Vector2 movement = m_Destination - enemy.Position;
                if (movement.Length() < 10)
                {
                    m_InMiddle = true;
                    Enemy.StopMoving();
                    Enemy.Animations.ChangeAnimation(3*4 + (int)Enemy.AnimationDirection);
                    return;
                }
                movement.Normalize();
                Enemy.Movement = movement;
            }
            else if (m_NumberOfSpawns > 0)
            {
                m_SpawnCounter += delta;
                if (m_SpawnCounter > m_SpawnInterval)
                {
                    if (m_Once)
                    {
                        SoundCenter.Instance.Play(SoundNames.bossZombieSpawn);
                        m_Once = false;
                    }

                    switch (m_Formation){
                        case 2:
                            m_PositionCounter += 0.2f * 3;
                            m_SpawnDistance += 0.2f * m_DistanceIncrease;
                            m_NumberOfSpawns--;
                            SpawnZombie(enemy.Position + new Vector2((float)Math.Sin(m_PositionCounter) * m_SpawnDistance, (float)Math.Cos(m_PositionCounter) * m_SpawnDistance));
                        break;
                        case 3:
                             m_PositionCounter += 0.2f * 3;
                             m_SpawnDistance += 0.2f * m_DistanceIncrease;
                             m_NumberOfSpawns-=2;
                             for (int i = -1; i < 2; i+=2)
                             {
                                 SpawnZombie(enemy.Position + new Vector2((float)Math.Sin(m_PositionCounter) * m_SpawnDistance * i, (float)Math.Cos(m_PositionCounter) * m_SpawnDistance * i));
                             }
                        break;
                        case 4:
                            
                            for (int i = -1; i < 2; i += 2)
                            {
                                for (int j = -1; j < 2; j+=2)
                                {
                                    int spawn = (int)(m_NumberOfSpawns/4);
                                    Vector2 offset = new Vector2(200*i - 50*spawn, 200*j * 50*spawn);
                                    SpawnZombie(enemy.Position + offset);
                                }
                            }
                            m_NumberOfSpawns -= 4;
                        break;

                    }

                    m_SpawnCounter = 0;
                }
            }
            else
            {
                Enemy.IsSolid = true;
                enemy.ChangeBehavior(new BossIdle(enemy, 10));
                //if ((enemy as Boss).Level > 2)
                //{
                //   (enemy as Boss).StartLaser();
                //}
            }
        }


        private void SpawnZombie(Vector2 pos)
        {
            Boss boss = Enemy as Boss;
            if (EntityManager.Enemies.Count < 50)
            {
                Zombie zombie = new Zombie(boss);
                EntityManager.Spawn(zombie);
                zombie.setStats(2, 10, 120, 3, 1, 1);
                zombie.CreateAtPosition(pos);

                int level = boss.Level;
                if (level == 2 || level > 6 && m_Random.Next(4) == 1)
                {
                    StickyGrenadeProjectile grenade = new StickyGrenadeProjectile(Enemy, m_Dirs[m_Random.Next(4)], true);
                    grenade.Position = pos;
                    EntityManager.Spawn(grenade);
                    if (level > 6)
                    {
                        grenade.Life = 2;
                    }
                }
            }
        }
    }
}
