using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class BossIdle : Behavior
    {
        private Vector2[] m_Dirs = { new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0) };
        int m_CurrentDir = 0;
        float m_MovementFaultTolerance = 0.2f;
        Vector2 m_PreviousPosition = Vector2.Zero;
        Random rand = new Random();

        float m_SpawnCooldown = 5;
        float m_SpawnCooldownCounter = 0;
        float m_TurnCounter = 1.0f;
        int preferMiddle = 85;

        public BossIdle(Enemy enemy, float spawncooldown)
            : base(enemy)
        {
            Vector2 pos = enemy.Position;
            m_PreviousPosition = pos;
            (enemy as Boss).HitSurround(false);
            m_SpawnCooldown = spawncooldown;

            if ((enemy as Boss).Level >= 4)
            {
                enemy.IsSolid = false;
            }
            else
            {
                enemy.IsSolid = true;
            }
        }
        public override void Update(float delta)
        {
            Boss boss = Enemy as Boss;
            if (boss.State == Boss.BossStates.WALKING || boss.State == Boss.BossStates.LASERWALK)
            {
                m_TurnCounter -= delta;
                bool turn = false;
                if (m_TurnCounter <= 0)
                {
                    m_TurnCounter = 1.0f;
                    turn = true;
                }

                // If we run into a wall of some sort, or if random, change direction
                if ((m_PreviousPosition - boss.Position).Length() < delta * boss.MoveSpeed * m_MovementFaultTolerance || turn)
                {

                    Vector2 midde = new Vector2(1024 / 2, 786 / 2);

                    if (rand.Next(100) < preferMiddle)
                    {
                        int leftTurn = (m_CurrentDir + 1) % 4;
                        int rightTurn = (m_CurrentDir - 1 + 4) % 4;
                        float lengthToMiddle1 = (boss.Position + m_Dirs[leftTurn] - midde).Length();
                        float lengthToMiddle2 = (boss.Position + m_Dirs[rightTurn] - midde).Length();
                        if (lengthToMiddle1 < lengthToMiddle2)
                        {
                            m_CurrentDir++;
                        }
                        else
                        {
                            m_CurrentDir--;
                        }

                    }
                    else
                    {
                        if (rand.Next(2) == 0)
                            m_CurrentDir++;
                        else
                            m_CurrentDir--;
                    }
                    m_CurrentDir = (m_CurrentDir + 4) % 4;
                }

                // Set the movement
                boss.Movement = m_Dirs[m_CurrentDir];

                // Checks if we want to go into the middle and spawn zombies
                if (m_SpawnCooldownCounter >= m_SpawnCooldown && boss.State != Boss.BossStates.LASERWALK && boss.Level >= 2)
                {
                    if (boss.Position.Y > 95 &&
                        boss.Position.Y < 560 &&
                        boss.Position.X > 400 &&
                        boss.Position.X < 600
                        )
                    {
                        Enemy.ChangeBehavior(new MiddleWalkSpawn(Enemy));
                    }
                }
                else
                {
                    m_SpawnCooldownCounter += delta;
                }

                m_PreviousPosition = boss.Position;

                // Checks for players to hit
                boss.CheckForTargets(delta);

                boss.CheckDash(delta);
            }
        }
    }
}
