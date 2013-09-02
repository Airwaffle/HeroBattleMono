using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class ZombieHero : Hero
    {

        float summonZombieSpeed = Configuration.GetValue("Summon_Zombies_animationSpeed");
        float summonZombieDelay = Configuration.GetValue("Summon_Zombies_delay");

        float digSpeed = Configuration.GetValue("Dig_Teleport_Down_Animation_Speed");
        float undergroundTime = Configuration.GetValue("Dig_Teleport_Time_Underground");
        float digUpSpeed = Configuration.GetValue("Dig_Teleport_Up_Animation_Speed");
        float digButtonBias = Configuration.GetValue("Dig_Teleport_Button_Bias");
        float digDist = Configuration.GetValue("Dig_Teleport_Dist_To_Victim");

        float m_DrainTime = Configuration.GetValue("Vampyric_Stuck_Time");

        float minionMoveSpeed = Configuration.GetValue("Summon_Minion_MoveSpeed");
        float minionAttackSpeed = Configuration.GetValue("Summon_Zombies_AttackSpeed");
        float minionDamage = Configuration.GetValue("Summon_Minion_Damage");
        float minionHealth = Configuration.GetValue("Summon_Minion_Health");
        float minionUnburrowSpeed = Configuration.GetValue("Summon_Zombies_UnborrowSpeed");
        int minionMaxNumber = (int)Configuration.GetValue("Summon_Zombies_Max_Number");
        private const int SUMMONZOMBIES = 8;
        private const int DIG = 9;
        private const int VAMPYRIC = 10;
        private const int DRAINBLOOD = 8;
        private const int UNBORROWFAST = 11;
        private const int VAMPBOOST = 12;

        private List<Zombie> m_Minions = new List<Zombie>();

        public void EnptyMinionList()
        {
            m_Minions = new List<Zombie>();
        }

        public void MinionDied()
        {
            for (int i = 0; i < m_Minions.Count;)
            {
                if (!m_Minions[i].IsAlive)
                m_Minions.RemoveAt(i);
                else ++i;
            }
        }

        public ZombieHero()
        {
          
        }
        public override void Initialize()
        {
            base.Initialize();

            Name = "Playable Zombie";
            CharacterID = 4;
            Scale = 2;

            Vector2 position = Position;
            // Set feet bounding box.
            AABB movementBB = MovementBB;
            movementBB.MinX = position.X - 20 - 20 + 20;
            movementBB.MaxX = position.X + 20 - 20 + 20;
            movementBB.MinY = position.Y - 10 - 20 + 26;
            movementBB.MaxY = position.Y + 10 - 20 + 26;

            // Create collision bounding box.
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerHeroBody;
            boundingBox.CollisionMask = AABBLayers.CollisionHeroBody;
            boundingBox.MinX = position.X - 20 - 20 + 20;
            boundingBox.MaxX = position.X + 20 - 20 + 20;
            boundingBox.MinY = position.Y - 40 - 20 + 26;
            boundingBox.MaxY = position.Y + 10 - 20 + 26;
            AddAABB(boundingBox);

            DrawOffset = new Vector2(position.X - 80, position.Y - 94);

            /////////////////////////////////////////////////////////////////////////////////

            AnimationManager animations = new AnimationManager(80, 80, 13, 20);

            //IDLE
            animations.AddAnimation(new Animation(1, 0, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 1, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 2, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 3, 0, false, (int)Directions.Right));

            //WALK
            animations.AddAnimation(new Animation(12, 0, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(13, 1, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(12, 2, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(13, 3, 0, false, (int)Directions.Right));

            //ATTACK 
            animations.AddAnimation(new Animation(5, 7, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(5, 7, 5, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(5, 8, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(5, 8, 5, false, (int)Directions.Right));

            //DYING
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Right));

            //DEAD
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Right));

            //DYING2
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Right));

            //DEAD2
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Right));

            //UNBURROW
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Right));

            //SUMMON
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Right));

            //DIGDOWN
            animations.AddAnimation(new Animation(11, 10, 6, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(11, 10, 6, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(11, 10, 6, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(11, 10, 6, false, (int)Directions.Right));

            //VAMPYRIC 
            animations.AddAnimation(new Animation(13, 11, 5, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(16, 12, 5, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(15, 13, 8, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(16, 14, 10, false, (int)Directions.Right));

            //UNBURROWFAST
            animations.AddAnimation(new Animation(9, 16, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(9, 16, 9, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(9, 17, 5, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(9, 18, 1, false, (int)Directions.Right));

            //VAMPBOOST
            animations.AddAnimation(new Animation(1, 7, 10, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 7, 11, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 7, 12, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 8, 10, false, (int)Directions.Right));

            //VICTORY
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(12, 19, 0, false, (int)Directions.Right));

            VictoryAnimation = 13;
            VictoryExclamation = "ex_zombie_wins";

            Animations = animations;
            /////////////////////////////////////////////////////////////////////////////////

            DefaultMoveSpeed = Configuration.GetValue("Playable_Zombie_Move_Speed");
            DefaultAttackDamage = Configuration.GetValue("Playable_Zombie_Attack_Damage");
            DefaultAttackSpeed = Configuration.GetValue("Playable_Zombie_Attack_Speed");
            DefaultArmor = Configuration.GetValue("Playable_Zombie_Armor");
            MaxMana = Configuration.GetValue("Playable_Zombie_Max_Mana");
            Mana = Configuration.GetValue("Playable_Zombie_Max_Mana");
            Health = Configuration.GetValue("Playable_Zombie_Max_Health");
            MaxHealth = Configuration.GetValue("Playable_Zombie_Max_Health");
            ManaRegenerationRate = Configuration.GetValue("Playable_Zombie_Mana_Regen_Rate");

            //Texture = Graphics.GetTexture("zombie_sprite2");
            Texture = Graphics.LoadTexture("Characters/Zombie/Sprite_Sheet_Zombie_2");

            Portrait = Graphics.GetTexture("zombie_portrait");
            WinnerPortrait = Graphics.GetTexture("score_icon_zombie");

            ChangeAnimation(2);

            AddAbility(new SummonZombies(this));
            AddAbility(new DigTeleport(this));
            AddAbility(new VampyricBoost(this));
            
            
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (GameMode.Instance != null)
                if (GameMode.Instance.GameWon) return;

            AnimationSync += delta;

            if (IsAlive)
            {
                // WALK
                if (HeroState == HeroStates.WALKING || HeroState == HeroStates.IDLE)
                {
                    Vector2 movement = Movement;

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
                    else if (movement.X > 0)
                    {
                        ChangeAnimation(1 * 4 + (int)Directions.Right);
                        ChangeDirection(Directions.Right);
                    }
                    else if (movement.X < 0)
                    {
                        ChangeAnimation(1 * 4 + (int)Directions.Left);
                        ChangeDirection(Directions.Left);
                    }
                    else
                    {
                        if (Animations.GetCurrentAnimation() >= 4)
                            Animations.ReturnToIdle();
                    }
                }
                else if (HeroState == HeroStates.ATTACKING)
                {
                    if (AttackCounter == 0)
                    {
                        Animations.ChangeAnimation(2 * 4 + (int)AnimationDirection);
                        Animations.SetAnimationDuration(AttackSpeed);
                        Attack();
                    }
                    AttackCounter += delta;

                    if (AttackCounter >= AttackSpeed)
                    {
                        AttackCounter = 0;
                        HeroState = HeroStates.IDLE;
                        Animations.ReturnToIdle();
                    }
                }
                // Summon zombie ability
                else if (HeroState == HeroStates.SPECIAL1)
                {
                    AttackCounter = 0;
                    Animations.ChangeAnimation(SUMMONZOMBIES * 4 + (int)AnimationDirection);
                    Animations.SetAnimationDuration(summonZombieSpeed);
                    HeroState = HeroStates.SPECIAL2;
                    StopMoving();
                }
                else if (HeroState == HeroStates.SPECIAL2)
                {                   
                    AttackCounter += delta;

                    if (AttackCounter >= summonZombieDelay && AttackCounter < summonZombieDelay + 1)
                    {
                        SoundCenter.Instance.Play(SoundNames.ZombieSpawn);
                        AttackCounter ++;
                        Vector2[] zombiePos = {
                        new Vector2(1,0),
                        new Vector2(-1,0),
                        new Vector2(0,-1),
                        new Vector2(0,1),
                        };

                        for (int i = 0; i < 4; i++)
                        {
                            if (m_Minions.Count >= minionMaxNumber) break; 
                            Zombie zombie = new Zombie(null);
                            zombie.Owner = this;
                            EntityManager.Spawn(zombie);
                            zombie.setStats(4, minionHealth, minionMoveSpeed, minionDamage, minionAttackSpeed, minionUnburrowSpeed);
                            zombie.CreateAtPosition(Position + zombiePos[i] * 40);

                            // By setting the owner, we declare that the zombie should aggro all units that doesn't belong to this hero.
                            zombie.AddWontAggro(this);
                            m_Minions.Add(zombie);
                        }

                        if (GameMode.Instance.TeamGame || GameMode.Instance is GM_Zombie)
                        {
                            if (!GameMode.Instance.FriendlyFire)
                            {
                                List<Hero> heroes = EntityManager.Heroes;
                                foreach (Hero hero in heroes)
                                    if (hero != this && hero.Team == Team)
                                        foreach (Zombie zombie in m_Minions)
                                            zombie.AddWontAggro(hero);
                            }
                        }
                    }
                    if (AttackCounter >= summonZombieSpeed + 1)
                    {
                        AttackCounter = 0;
                        HeroState = HeroStates.IDLE;
                        ChangeDirection(Directions.Down);
                        ChangeAnimation((int)AnimationDirection);                      
                    }
                }
                // Dig ability
                else if (HeroState == HeroStates.SPECIAL3)
                {
                    SoundCenter.Instance.Play(SoundNames.ZombieDig);
                    AttackCounter = 0;
                    Animations.ChangeAnimation(DIG * 4 + (int)AnimationDirection);
                    Animations.SetAnimationDuration(digSpeed);

                    // Hides the grenade if it's on the zombie, it looks bad otherwise
                    foreach (Buff buff in Buffs)
                        if (buff is StickyGrenadeBuff)
                            (buff as StickyGrenadeBuff).Grenade.Hide();

                    HeroState = HeroStates.SPECIAL4;
                    StopMoving();
                }
                else if (HeroState == HeroStates.SPECIAL4)
                {
                    AttackCounter += delta;
                    if (AttackCounter >= digSpeed && IsVisible == true)
                    {
                        Hide();
                        SetInvincibility(digSpeed + undergroundTime + digUpSpeed);
                        IsSolid = false;
                        Shadow = false;
                    }
                    else if (AttackCounter >= digSpeed + undergroundTime)
                    {
                        AttackCounter = 0;
                        HeroState = HeroStates.SPECIAL5;
                        Show();
                        IsSolid = true;
                        Shadow = true;
                        
                        Vector2 pos = Position;

                        if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Down))
                        {
                            pos.Y += digButtonBias;
                        }
                        if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Up))
                        {
                            pos.Y -= digButtonBias;
                        }
                        if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Right))
                        {
                            pos.X += digButtonBias;
                        }
                        if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Left))
                        {
                            pos.X -= digButtonBias;
                        }


                        float length = 999999;
                        Unit victim = null;
                        List<Unit> victims = new List<Unit>();

                        if (GameMode.Instance is GM_Zombie)
                        {
                            List<Enemy> enemies = EntityManager.Enemies;
                            foreach (Enemy enemy in enemies)
                                victims.Add(enemy);
                        }
                        else
                        {
                            List<Hero> heroes = EntityManager.Heroes;
                            foreach (Hero hero in heroes)
                                victims.Add(hero);
                        }


                        foreach(Unit potVictim in victims){
                            if (potVictim != this && potVictim.IsAlive)
                            {
                                float thisLength = Vector2.Distance(pos, potVictim.Position);
                                if (length > thisLength)
                                {
                                    length = thisLength;
                                    victim = potVictim;
                                }
                            }
                        }

                        if (victim != null)
                        {
                            Vector2 newPosition = new Vector2(victim.Position.X, victim.Position.Y);
                            ChangeDirection(victim.AnimationDirection);
                            newPosition -= victim.Direction * digDist;
                            victim.SetStun(Configuration.GetValue("Dig_Teleport_Stun_Victim_Time"));
                            Position = newPosition;
                        }
                        else
                        {
                            // We didn't find anyone to teleport to
                            ChangeDirection(Directions.Down);
                        }
                        ChangeAnimation(UNBORROWFAST * 4 + (int)AnimationDirection);
                        Animations.SetAnimationDuration(digUpSpeed);
                    }
                }       
                else if (HeroState == HeroStates.SPECIAL5)
                {
                    AttackCounter += delta;
                    if (AttackCounter >= digUpSpeed)
                    {
                        AttackCounter = 0;
                        HeroState = HeroStates.IDLE;
                        ChangeAnimation((int)AnimationDirection);

                        // Show the grenade again if we hid it before we dug down
                        foreach (Buff buff in Buffs)
                            if (buff is StickyGrenadeBuff)
                                (buff as StickyGrenadeBuff).Grenade.Show();
                    }
                }
                // VAMPYRIC
                else if (HeroState == HeroStates.SPECIAL6){
                }
                else if (HeroState == HeroStates.SPECIAL7)
                {
                    SoundCenter.Instance.Play(SoundNames.ZombieDrain);
                    AttackCounter = 0;
                    HeroState = HeroStates.SPECIAL8;
                    ChangeAnimation(VAMPYRIC * 4 + (int)AnimationDirection);
                    Animations.SetAnimationDuration(m_DrainTime);
                }
                else if (HeroState == HeroStates.SPECIAL8)
                {
                    AttackCounter += delta;
                    if (AttackCounter >= m_DrainTime)
                    {
                        AttackCounter = 0;
                        HeroState = HeroStates.IDLE;
                    }
                }
            }
            else
            {
                Animations.Update(delta);
            }
        }
        public override void Attack()
        {
            ZombieAttackHit hit = new ZombieAttackHit(this, Direction);

            SoundCenter.Instance.Play(SoundNames.ZombieAttack);
            hit.Position = (Position + Direction) * 25;
            hit.Life = AttackSpeed;
            hit.Damage = AttackDamage;
            EntityManager.Spawn(hit);
            StopMoving();
        }

        public override void Die()
        {
            SoundCenter.Instance.Play(SoundNames.ZombieDeath);
            base.Die();
            Animations.ChangeAnimation(3 * 4 + (int)Directions.Down);
        }
    }
}
