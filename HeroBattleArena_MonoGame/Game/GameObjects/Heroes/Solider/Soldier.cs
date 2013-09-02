using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    class Soldier : Hero
    {
        private const int THROWING = 7;
        private const int FLASHBANG = 6;
        private const int LASERSTART = 8;
        private const int LASERIDLE = 9;
        private const int LASERIDLEGLOWING = 10;
        private const int LASERWALKING = 11;
        private const int LASERWALKINGGLOWING = 12;

        private float m_bulletSpeed = Configuration.GetValue("Solider_Bullet_Speed");
        private float m_stickyThrowSpeed = Configuration.GetValue("Sticky_Grenade_TrowSpeed");
        private float m_stickyCreateDelay = Configuration.GetValue("Sticky_Animation_Create_Delay");
        private float m_invisibleAnimationDuration = Configuration.GetValue("Invisible_Animation_Time");
        private float m_lockOnActivateDuration = Configuration.GetValue("Lock_On_Activate_Animation");


        public Soldier()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "Stryker";
            CharacterID = 1;
            Scale = 2;

            Vector2 position = Position;
            // Set feet bounding box.
            MovementBB.MinX = position.X - 15;
            MovementBB.MaxX = position.X + 15;
            MovementBB.MinY = position.Y - 10;
            MovementBB.MaxY = position.Y + 15;

            // Create collision bounding box.
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerHeroBody;
            boundingBox.CollisionMask = AABBLayers.CollisionHeroBody;
            boundingBox.MinX = position.X - 15;
            boundingBox.MaxX = position.X + 15;
            boundingBox.MinY = position.Y - 45;
            boundingBox.MaxY = position.Y + 10;
            AddAABB(boundingBox);

            DrawOffset = new Vector2(-80, -95);
            WeaponOffset = new Vector2(0, -15);

            /////////////////////////////////////////////////////////////////////////////////////////////////
            AnimationManager animations = new AnimationManager(80, 80, 9, 16);

            //IDLE
            animations.AddAnimation(new Animation(1, 0, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 0, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 0, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 1, 5, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //BLINK
            animations.AddAnimation(new Animation(1, 0, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 1, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 1, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 2, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //WALK
            animations.AddAnimation(new Animation(4, 0, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 0, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 0, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 1, 3, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //ATTACK
            animations.AddAnimation(new Animation(9, 3, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(8, 2, 1, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(9, 4, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(8, 5, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //DYING
            animations.AddAnimation(new Animation(4, 5, 8, true, (int)HeroStates.DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 5, 8, true, (int)HeroStates.DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 5, 8, true, (int)HeroStates.DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 5, 8, true, (int)HeroStates.DEAD * 4 + (int)Directions.Right));

            //DEAD
            animations.AddAnimation(new Animation(1, 6, 2, false, (int)HeroStates.DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 6, 2, false, (int)HeroStates.DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 6, 2, false, (int)HeroStates.DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 6, 2, false, (int)HeroStates.DEAD * 4 + (int)Directions.Right));

            //FLAHBANG
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //THROWING
            animations.AddAnimation(new Animation(4, 6, 3, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(3, 6, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(5, 7, 2, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(3, 7, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //LASER_START
            animations.AddAnimation(new Animation(3, 8, 2, true, LASERIDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 8, 7, true, LASERIDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 9, 4, true, LASERIDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 10, 1, true, LASERIDLE * 4 + (int)Directions.Right));

            //LASERIDLE
            animations.AddAnimation(new Animation(1, 8, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 9, 1, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 9, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 10, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //LASERIDLEGLOWING
            animations.AddAnimation(new Animation(2, 8, 5, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(2, 9, 2, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(2, 9, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(2, 10, 5, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //WALKINGLASER
            animations.AddAnimation(new Animation(4, 10, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 11, 2, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 11, 6, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 12, 1, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //GLOWINGWALKINGLASER
            animations.AddAnimation(new Animation(4, 12, 5, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 13, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 13, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 14, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //VICTORY
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(7, 15, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            VictoryAnimation = 13;
            VictoryExclamation = "ex_stryker_wins";
            Animations = animations;
            /////////////////////////////////////////////////////////////////////////////////

            DefaultMoveSpeed = Configuration.GetValue("Solider_Walk_Speed");
            DefaultAttackDamage = Configuration.GetValue("Solider_Attack_Damage");
            DefaultAttackSpeed = Configuration.GetValue("Solider_Attack_Speed");
            DefaultArmor = Configuration.GetValue("Solider_Armor");
            MaxMana = Configuration.GetValue("Solider_Max_Mana");
            Mana = Configuration.GetValue("Solider_Max_Mana");
            Health = Configuration.GetValue("Solider_Max_Health");
            MaxHealth = Configuration.GetValue("Solider_Max_Health");
            ManaRegenerationRate = Configuration.GetValue("Solider_Mana_Regen_Rate");

            // Outdated
            /*
            if (HeroColor == 0)
                Texture = Graphics.GetTexture("soldier_sprite");
            else if (HeroColor == 1)
                Texture = Graphics.GetTexture("soldier_sprite_v2");
            else if (HeroColor == 2)
                Texture = Graphics.GetTexture("soldier_sprite_v3");
            else
                Texture = Graphics.GetTexture("soldier_sprite_v4");
            */

            if (HeroColor == 0)
                Texture = Graphics.LoadTexture("Characters/Stryker/Sprite_Sheet_Soldier");
            else if (HeroColor == 1)
                Texture = Graphics.LoadTexture("Characters/Stryker/Sprite_Sheet_Soldier_Green");
            else if (HeroColor == 2)
                Texture = Graphics.LoadTexture("Characters/Stryker/Sprite_Sheet_Soldier_Black");
            else
                Texture = Graphics.LoadTexture("Characters/Stryker/Sprite_Sheet_Soldier_Pink");


            Portrait = Graphics.GetTexture("stryker_portrait");
            WinnerPortrait = Graphics.GetTexture("score_icon_stryker");

            ChangeAnimation(2);

            AddAbility(new StickyGrenade(this));
            AddAbility(new LockOn(this));
            AddAbility(new Flashbang(this));
            
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
                    if (!IsStrafing)
                    {
                        if (movement.Y > 0)
                        {
                            ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Down);
                            ChangeDirection(Directions.Down);
                        }
                        else if (movement.Y < 0)
                        {
                            ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Up);
                            ChangeDirection(Directions.Up);
                        }
                        else if (movement.X > 0)
                        {
                            ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Right);
                            ChangeDirection(Directions.Right);
                        }
                        else if (movement.X < 0)
                        {
                            ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Left);
                            ChangeDirection(Directions.Left);
                        }
                        else
                        {
                            if (Animations.GetCurrentAnimation() >= 8)
                                Animations.ReturnToIdle();
                        }
                    }
                    else
                    {
                        if (movement.Y != 0 || movement.X != 0)
                        {
                            Animations.ChangeAnimation((int)HeroStates.WALKING * 4 + (int)AnimationDirection);
                        }
                        else
                        {
                            Animations.ReturnToIdle();
                        }

                    }

                }
                else if (HeroState == HeroStates.ATTACKING)
                {
                    if (AttackCounter == 0)
                    {
                        Animations.ChangeAnimation((int)HeroStates.ATTACKING * 4 + (int)AnimationDirection);
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
                else if (HeroState == HeroStates.SPECIAL1)
                {
                    Animations.ChangeAnimation(THROWING * 4 + (int)AnimationDirection);
                    Animations.SetAnimationDuration(m_stickyThrowSpeed);
                    HeroState = HeroStates.SPECIAL2;
                    StopMoving();
                }
                else if (HeroState == HeroStates.SPECIAL2)
                {
                    AttackCounter += delta;

                    if (AttackCounter >= m_stickyCreateDelay && AttackCounter <= m_stickyCreateDelay + 1)
                    {
                        AttackCounter ++;


                        if (!(ScreenManager.GetInstance().GetScreen(ScreenManager.GetInstance().NumScreens - 1) is TutorialScreen && !TutorialScreen.CanThrowGrenade()))
                        {
                            if (EntityManager.AmountOfInstancesExisting<StickyGrenadeProjectile>() < 15)
                            {
                                StickyGrenadeProjectile grenade = new StickyGrenadeProjectile(this, Direction, false);
                                grenade.Position = Position;
                                EntityManager.Spawn(grenade);
                            }
                        }
                    }
                    if (AttackCounter >= m_stickyThrowSpeed + 1)
                    {
                        AttackCounter = 0;
                        HeroState = HeroStates.IDLE;
                        Animations.ReturnToIdle();
                    }
                }
                else if (HeroState == HeroStates.SPECIAL3)
                {
                    Animations.ChangeAnimation(FLASHBANG * 4 + (int)AnimationDirection);
                    Animations.SetAnimationDuration(m_invisibleAnimationDuration);
			        HeroState = HeroStates.SPECIAL4;
			        StopMoving();
                }
                else if (HeroState == HeroStates.SPECIAL4)
                {
                   AttackCounter += delta;
                   if (AttackCounter >= m_invisibleAnimationDuration)
                   {
                       AttackCounter = 0;
				       HeroState = HeroStates.IDLE;
                       AddBuff(new Invisbuff(this));
				       Animations.ReturnToIdle();
			       }
                }
                // LOCK ON
                else if (HeroState == HeroStates.SPECIAL5)
                {
                    Animations.ChangeAnimation(LASERSTART * 4 + (int)AnimationDirection);
                    Animations.SetAnimationDuration(m_lockOnActivateDuration);
			        HeroState = HeroStates.SPECIAL6;
			        StopMoving();
                }
                else if (HeroState == HeroStates.SPECIAL6)
                {
                    AttackCounter += delta;
                    if (AttackCounter >= m_lockOnActivateDuration)
                    {
                        AttackCounter = 0;
				        HeroState = HeroStates.SPECIAL7;
			        }
                }
                else if (HeroState == HeroStates.SPECIAL7)
                {
                    Vector2 movement = Vector2.Zero;
			        bool moving = false;
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Down))
			        {
				        movement.Y = 1;
				        moving = true;
			        }
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Up))
			        {
				        movement.Y = -1;
				        moving = true;
			        }
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Right))
			        {
				        movement.X = 1;
				        moving = true;
			        }
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Left))
			        {
				        movement.X = -1;
				        moving = true;
			        }

			        if (moving){
                        Animations.ChangeAnimation(LASERWALKING * 4 + (int)AnimationDirection);
                        movement.Normalize();
                    } else {
                        Animations.ChangeAnimation(LASERIDLE * 4 + (int)AnimationDirection);
			        }
                    Movement = movement;
                }
                else if (HeroState == HeroStates.SPECIAL8)
                {
                    Vector2 movement = Vector2.Zero;
			        bool moving = false;
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Down))
			        {
				        movement.Y = 1;
				        moving = true;
			        }
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Up))
			        {
				        movement.Y = -1;
				        moving = true;
			        }
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Right))
			        {
				        movement.X = 1;
				        moving = true;
                    }
                    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Left))
			        {
				        movement.X = -1;
				        moving = true;
			        }

			        if (moving){
                        Animations.ChangeAnimation(LASERWALKINGGLOWING * 4 + (int)AnimationDirection);
                        movement.Normalize();
                    } else {
                        Animations.ChangeAnimation(LASERIDLEGLOWING * 4 + (int)AnimationDirection);
			        }
			        
                    Movement = movement;
                }
#warning Testa om soldaten kan blinka!
                //if (HeroState == HeroStates.IDLE && AnimationSync >= 1.0f && !IsStunned
                    //Animations.ChangeAnimation((int)HeroStates.BLINK * 4 + (int)AnimationDirection);
            }
            else
            {
                Animations.Update(delta);
            }

            if (AnimationSync >= 1.0f)
                AnimationSync -= 1.0f;
        }
        public override void Attack()
        {
			SoundCenter.Instance.Play(SoundNames.SoldierAttack);

            Bullet bullet = new Bullet(this, Direction);
            bullet.Position = Position + WeaponOffset;
            bullet.Damage = AttackDamage;
            bullet.Speed = m_bulletSpeed;
            EntityManager.Spawn(bullet);
            StopMoving();
        }

        public override void Die()
        {
			SoundCenter.Instance.Play(SoundNames.SoldierDeath);
            base.Die();
            Animations.ChangeAnimation((int)HeroStates.DYING * 4 + (int)Directions.Down);
        }
    }
}
