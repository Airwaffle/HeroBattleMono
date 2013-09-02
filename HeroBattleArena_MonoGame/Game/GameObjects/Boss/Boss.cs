using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class Boss : Enemy
    {
        public enum BossStates
        {
            WALKING,
            IDLE,
            ATTACKING, 
            DASH,
            SPAWNING,
            LASERWALK
        }

        private BossStates m_state = BossStates.WALKING;
        private BossLaserProjectile m_LaserProjectile = null;
        private BossFollowHit m_HitFollow = null;
        private float m_LaserCounter = 0;
        private float m_LaserCounterMax = 0.1f;
        private int m_level = 0;

        private float m_LaserMoveSpeed = 0.5f;
        private Random m_Random = new Random();

        public BossStates State { get { return m_state; } set { m_state = value; } }
        public int Level { get { return m_level; } }

        private const int DYING = 5;
        private const int DEAD = 6;
        private const int LASERSTART = 7;
        private const int LASERLOOP = 8;

        List<Vector2> m_dragBehind = new List<Vector2>();
        float m_dragDelay = 0.05f;
        float m_dragCounter = 0;

        public Boss(int level, Vector2 pos)
        {
            int currentMap = AIManager.GetMapNumber();

            switch (currentMap)
            {
                case 0:
                    break;
                case 1:
                    pos = new Vector2(612, 0);
                    break;
            }

            int additionalBosses = 0;
            if (level >= 10)
            {
                while (level >= 10)
                {
                    level -= 10;
                    additionalBosses += 2;
                }
                int offset = 400 / additionalBosses;
                for (int i = additionalBosses / 2 * -1; i < (additionalBosses / 2) + 1; i++)
                {
                    if (i != 0)
                    {
                        Boss boss = new Boss(level, pos + new Vector2(i * offset, 0));
                        EntityManager.Spawn(boss);
                    }
                }
            }
            m_level = level;
            Position = pos;

            if (m_level >= 6)
            {
                m_LaserMoveSpeed = 1.25f;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            Scale = 2;
            Layer = 2.01f;

            Vector2 position = Position;

            // Set feet bounding box.
            AABB movementBB = MovementBB;
            movementBB.MinX = position.X - 20 - 20 + 20 - 10;
            movementBB.MaxX = position.X + 20 - 20 + 20 + 10;
            movementBB.MinY = position.Y - 10 - 20 + 66 - 10 - 50;
            movementBB.MaxY = position.Y + 10 - 20 + 66;

            // Create collision bounding box.
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerHeroBody;
            boundingBox.CollisionMask = AABBLayers.CollisionHeroBody;
            boundingBox.MinX = position.X - 20 - 20 + 20 - 20;
            boundingBox.MaxX = position.X + 20 - 20 + 20 + 20;
            boundingBox.MinY = position.Y - 40 - 20 + 26 - 20;
            boundingBox.MaxY = position.Y + 10 - 20 + 26 + 40;
            AddAABB(boundingBox);

            m_HitFollow = new BossFollowHit(this);
            EntityManager.Spawn(m_HitFollow);

            DrawOffset = new Vector2(position.X - 100, position.Y - 100);

            /////////////////////////////////////////////////////////////////////////////////
            AnimationManager animations = new AnimationManager(100, 100, 16, 12);

            //IDLE
            animations.AddAnimation(new Animation(1, 0, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 1, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 2, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 3, 0, false, (int)Directions.Up));

            //WALK
            animations.AddAnimation(new Animation(16, 0, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(16, 1, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(16, 2, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(16, 3, 0, false, (int)Directions.Left));

            //ATTACK
            animations.AddAnimation(new Animation(6, 5, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(7, 5, 6, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(7, 5, 13, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(7, 6, 4, false, (int)Directions.Right));

            //SUMMON
            animations.AddAnimation(new Animation(14, 4, 0, true, (int)Directions.Up));
            animations.AddAnimation(new Animation(14, 4, 0, true, (int)Directions.Left));
            animations.AddAnimation(new Animation(14, 4, 0, true, (int)Directions.Down));
            animations.AddAnimation(new Animation(14, 4, 0, true, (int)Directions.Right));

            //DASH
            animations.AddAnimation(new Animation(1, 6, 11, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 6, 12, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 6, 13, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 6, 14, false, (int)Directions.Right));

            //DYING
            animations.AddAnimation(new Animation(15, 7, 0, true, DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(15, 7, 0, true, DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(15, 7, 0, true, DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(15, 7, 0, true, DEAD * 4 + (int)Directions.Right));

            //DEAD
            animations.AddAnimation(new Animation(1, 7, 14, false, DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 7, 14, false, DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 7, 14, false, DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 7, 14, false, DEAD * 4 + (int)Directions.Right));

            //LASERSTART
            animations.AddAnimation(new Animation(4, 0, 0, true, LASERLOOP * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 8, 0, true, LASERLOOP * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 9, 0, true, LASERLOOP * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 10, 0, true, LASERLOOP * 4 + (int)Directions.Right));

            //LASERLOOP
            animations.AddAnimation(new Animation(12, 0, 4, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(12, 8, 4, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(12, 9, 4, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(12, 10, 4, false, (int)Directions.Right));

            animations.StandardAnimationSpeed = 16;
            Animations = animations;
            ///////////////////////////////////////////////////////////////////////////////

            DefaultArmor = 0;
            MaxHealth = Configuration.GetValue("Boss_Health");
            DefaultMoveSpeed = 120;
            DefaultAttackDamage = Configuration.GetValue("Boss_Attack_Damage");
            DefaultAttackSpeed = Configuration.GetValue("Boss_Attack_Time"); 
            AttackRange = Configuration.GetValue("Zombie_Attack_Range");
            Health = MaxHealth;
            DecayTime = 5; //Configuration.GetValue("Zombie_Decay_Time");
            SetInvincibility(Configuration.GetValue("Zombie_Invincible_Spawn"));

            if (AIManager.GetMapNumber() == 3 || AIManager.GetMapNumber() == 2)
            {
                ChangeBehavior(new BossDash(this, true));
            }
            else
            {
                ChangeBehavior(new ZombieOutsideScreen(this));
            }
            
            //Texture = Graphics.GetTexture("boss_sprite");
            Texture = Graphics.LoadTexture("Characters/ZombieBoss/BossSheet");
            Name = "ZombieBoss lvl: " + m_level;

            HealthBar.Offset = new Vector2(-25, 65);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (m_state == BossStates.LASERWALK) MoveSpeed = DefaultMoveSpeed * m_LaserMoveSpeed;

            if (m_state == BossStates.DASH)
            {
                m_dragCounter += delta;
                if (m_dragCounter > m_dragDelay)
                {
                    m_dragCounter = 0;
                    m_dragBehind.Add(new Vector2(DrawOffset.X, DrawOffset.Y));
                    if (m_dragBehind.Count > 10)
                    {
                        m_dragBehind.RemoveAt(0);
                    }
                }
            }
            else
            {
                m_dragBehind.Clear();
            }

            if (m_state == BossStates.WALKING || m_state == BossStates.IDLE)
            {

                if (IsAlive)
                {
                    if (!IsStunned)
                    {

                        Vector2 movement = Movement;
                        if (Math.Abs(movement.Y) > Math.Abs(movement.X))
                        {
                            if (movement.Y > 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Down);
                                ChangeDirection(Directions.Down);

                            }
                            else if (movement.Y < 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Up);
                                ChangeDirection(Directions.Up);
                            }
                        }
                        else
                        {
                            if (movement.X > 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Right);
                                ChangeDirection(Directions.Right);

                            }
                            else if (movement.X < 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Left);
                                ChangeDirection(Directions.Left);

                            }
                        }

                    } // !isStunned()
                    else
                    {
                        ChangeAnimation((int)AnimationDirection); // IDLE
                    }
                } // !isAlive()
            }
        }

        public override void Draw()
        {
            base.Draw();
            for (int i = 0; i < m_dragBehind.Count; i++)
            {
                Graphics.Draw(
                    Texture,
                    m_dragBehind[i],
                    2.0f,
                    Animations.Rectangle,
                    1.98f + (float)Position.Y / 768.0f, Color.DarkRed);

            }
        }

        public void CheckDash(float delta)
        {
            if (m_level < 1) return;
            if (m_state == BossStates.WALKING || m_state == BossStates.IDLE)
            {
                if (m_LaserCounter >= m_LaserCounterMax && m_LaserProjectile == null)
                {
                    m_LaserCounter = 0;
                    Vector2 pPos = Position;
                    AABB[] collisionDirs = {
                new AABB(
                    pPos.X - 50, 
                    pPos.Y - 1000, 
                    pPos.X + 50,
                    pPos.Y),          // Up
                new AABB(
                    pPos.X - 1000, 
                    pPos.Y - 50, 
                    pPos.X, 
                    pPos.Y + 50),    // Left
                new AABB(
                    pPos.X - 50, 
                    pPos.Y, 
                    pPos.X + 50, 
                    pPos.Y + 1000),  // Down
                new AABB(
                    pPos.X, 
                    pPos.Y - 50, 
                    pPos.X + 1000, 
                    pPos.Y + 50)    // Right
                };
                    for (int i = 0; i < 4; i++)
                    {
                        collisionDirs[i].CollisionMask = AABBLayers.CollisionProjectile;
                        collisionDirs[i].LayerMask = AABBLayers.LayerProjectile;
                    }

                    //if (m_LaserCounter == 0)
                    //    for (int f = 0; f < 4; f++)
                    //        Enemy.BoundingBoxes.Add(collisionDirs[f]);
                    List<AABB> boxes = Collision.GetCollidingBoxes(collisionDirs[(int)AnimationDirection]);

                    foreach (AABB aabb in boxes)
                    {
                        if (aabb.Owner is Hero)
                        {
                            if (m_Random.Next(5) >= 3 && m_level > 2)
                            {
                                StartLaser();
                            }
                            else
                            {
                                ChangeBehavior(new BossDash(this, false));
                            }
                        }
                    }
                }
                else
                {
                    m_LaserCounter += delta;
                }
            }
        }

        public void CheckForTargets(float delta){

            if (m_state == BossStates.WALKING || m_state == BossStates.IDLE)
            {
                List<Hero> heroes = EntityManager.Heroes;
                float length = 100;

                foreach (Hero hero in heroes)
                {
                    if (!hero.IsAlive) continue;
                    Vector2 armPosition = new Vector2(Position.X + Movement.X * 100, Position.Y + Movement.Y * 100);
                    if ((hero.Position - armPosition).Length() < length)
                    {
                        if (Math.Abs(hero.Position.X - Position.X) > Math.Abs(hero.Position.Y - Position.Y))
                        {
                            if (hero.Position.X > Position.X)
                            {
                                ChangeDirection(Directions.Right);
                            }
                            else
                            {
                                ChangeDirection(Directions.Left);
                            }
                        }
                        else
                        {
                            if (hero.Position.Y > Position.Y)
                            {
                                ChangeDirection(Directions.Down);
                            }
                            else
                            {
                                ChangeDirection(Directions.Up);
                            }
                        }
                        ChangeBehavior(new BossAttack(this));
                    }
                }
            }
        }

        public void HitSurround(bool value)
        {
            m_HitFollow.Disabled = !value;
        }

        public void StartLaser()
        {
            m_LaserProjectile = new BossLaserProjectile(this, Direction, 3/16.0f);
            EntityManager.Spawn(m_LaserProjectile);
            m_state = BossStates.LASERWALK;
            ChangeAnimation(LASERSTART * 4 + (int)AnimationDirection);

            if (m_level >= 8 && EntityManager.AmountOfInstancesExisting<BossTravelingLaser>() == 0)
            {
                for (int i = -1; i < 2; i += 2)
                {
                    Vector2 travelDir = Direction.X != 0 ? new Vector2(0, i) : new Vector2(i, 0);
                    Vector2 shootDir = Direction.X != 0 ? new Vector2(1, 0) : new Vector2(0, 1);

                    int travelSpeed = 75;
                    BossTravelingLaser travelLaser = new BossTravelingLaser(this, shootDir, travelDir * travelSpeed);
                    EntityManager.Spawn(travelLaser);

                    if (i == 1)
                    {
                        travelLaser.Position = new Vector2(-10 - 100, -10 - 100);
                    }
                    else
                    {
                        travelLaser.Position = travelDir.X != 0 ? new Vector2(1084 - 100, 0 - 100) : new Vector2(0 - 100, 800 - 100);
                        /*
                        if (travelDir.X != 0)
                        {
                            travelLaser.Position = new Vector2(1084 - 100, 0 - 100);
                            travelLaser.Life = 5;
                        }
                        else
                        {
                            travelLaser.Position = new Vector2(0 - 100, 800 - 100);
                            travelLaser.Life = 3;
                        }*/
                    }
                    travelLaser.Life = travelDir.X != 0 ? 5 : 3;
                }
            }
        }

        public void RemoveLaser()
        {
            m_LaserProjectile = null;
            m_state = BossStates.WALKING;
        }

        public override void Die()
        {
            base.Die();
            if (m_LaserProjectile != null) m_LaserProjectile.Remove();
            SoundCenter.Instance.Play(SoundNames.bossZombieDeath);
            Animations.ChangeAnimation(DYING * 4 + (int)Directions.Up);
            Animations.StandardAnimationSpeed = 8;

            List<Enemy> enemies = EntityManager.Enemies;
            foreach (Enemy enemy in enemies)
            {
                if (enemy is Zombie)
                {
                    if ((enemy as Zombie).BossOwner == this)
                        enemy.Die();
                }
            }

            HitSurround(false);
            if (m_HitFollow != null)
                m_HitFollow.Remove();
        }
    }
}
