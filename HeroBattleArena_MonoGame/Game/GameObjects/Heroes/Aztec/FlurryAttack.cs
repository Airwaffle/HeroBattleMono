using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using HeroBattleArena.Game.Screens;

namespace HeroBattleArena.Game.GameObjects
{
    class FlurryAttack : HitObject
    {
        float m_time = 0;
        float m_manaDrain = Configuration.GetValue("Flurry_Mana_Drain");
        float m_teleportDistance = Configuration.GetValue("Flurry_Tele_Distance");
        float m_teleportCoolDown = Configuration.GetValue("Flurry_Tele_CoolDown");
        float m_teleportCounter = Configuration.GetValue("Flurry_Tele_Cooldown_Start");
        float m_teleportManaCost = Configuration.GetValue("Flurry_Mana_Tele_Cost");
        float m_SoundTimer = 0;
        int m_soundChannel = -1;
        AABB m_boundingBox = new AABB();
        Ability m_ability;
        int m_teleportExtraGraphic = (int)Configuration.GetValue("Flurry_Extra_Graphic");
        float m_removeExtraSpeed = Configuration.GetValue("Flurry_Graphic_Remove_Speed");
        float m_extraGraphicCounter = 0;
        private Cue m_LoopInstance = null;
        private bool m_ShallRemove = false;
        public void RemoveNext()
        {
            m_ShallRemove = true;
        }
        public FlurryAttack(Unit owner, Vector2 direction, Ability ability)
			: base(owner)
		{
			Direction = direction;
            //Texture = Graphics.GetTexture("aztec_sprite");
            Texture = Graphics.LoadTexture("Characters/Quentzali/Sprite_Sheet_Aztec");
            m_ability = ability;
		}

        ~FlurryAttack()
        {
            (Owner as Hero).HeroState = HeroStates.IDLE;
        }

        public override void Initialize()
        {
	        SoundCenter.Instance.Play(SoundNames.AztecFlurry);

            if (ScreenManager.GetInstance().GetScreen(ScreenManager.GetInstance().NumScreens - 1) is TutorialScreen){
                m_teleportExtraGraphic = 0;
            };
                
	        // Get configuraton settings.
	        Damage = Configuration.GetValue("Flurry_Damage");
	        Range = 40;
            Layer = 2;
	        Vector2 position = Position;

            DrawOffset = new Vector2(position.X - 20,  position.Y - 20);
            Life = 1000;
        	
	        // Create the attack AABB.
            m_boundingBox.Owner = this;
            m_boundingBox.LayerMask = AABBLayers.LayerProjectile;
            m_boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            UpdateCollisionBox();
            AddAABB(m_boundingBox);
        }

        public override void Update(float delta)
        {
            base.Update(delta);


            bool stopMoving = true;
            for (int i = 0; i < Owner.Buffs.Count; i++)
            {
                if (Owner.Buffs[i] is WhirlWindBuff)
                {
                    stopMoving = false;
                }
            }

            if (stopMoving) Owner.StopMoving();

	        if(m_SoundTimer < 0.5f) 
            {
		        m_SoundTimer += delta;
		        if(m_SoundTimer > 0.5f) {
			        m_LoopInstance = SoundCenter.Instance.Play(SoundNames.AztecFlurryLoop);
		        }
	        }

            if (m_extraGraphicCounter > 0)
            {
                m_extraGraphicCounter -= m_removeExtraSpeed*delta;
            }

            Hero hero = Owner as Hero;
            #if XBOX
                  if (!hero.GetSpecialDown(1) || hero.Mana < 5 || hero.IsStunned || !hero.IsAlive || m_ShallRemove)
            #else
                  if (!hero.GetSpecialDown(2) || hero.Mana < 5 || hero.IsStunned || !hero.IsAlive || m_ShallRemove)
            #endif
            
	        {
		        Remove();
                hero.CanAttack = true;
	            hero.HeroState = HeroStates.IDLE;
                if (m_SoundTimer > 0.5f)
                    m_LoopInstance.Stop(AudioStopOptions.AsAuthored);
                // m_AnimationEnd = null;
	            //if(m_SoundTimer > 0.5f)
		            //SoundCenter::fadeChannel(m_soundChannel, 500);
	        }
	        Position = Owner.Position;
	        hero.DecreaseMana(m_manaDrain*delta);
            


	        if (m_teleportCounter <= 0){
		        if(Input.GetPlayerState(hero.PlayerOwner).WasPressed(InputCommand.Right))
                {
			        Teleport(1,0); // Teleport right.
		        }
		        if(Input.GetPlayerState(hero.PlayerOwner).WasPressed(InputCommand.Left))
                {
			        Teleport(-1,0); // Teleport left.
		        }
		        if(Input.GetPlayerState(hero.PlayerOwner).WasPressed(InputCommand.Up))
                {
			        Teleport(0,-1); // Teleport up.
		        }
		        if(Input.GetPlayerState(hero.PlayerOwner).WasPressed(InputCommand.Down))
                {
			        Teleport(0,1); // Teleport down.
		        }
	        } else {
		        m_teleportCounter -= delta;
	        }
        }


        public void Teleport(int x, int y)
        {
	        // Play the teleport sound.
	        SoundCenter.Instance.Play(SoundNames.AztecFlurrySwoosh);

	        // Set a cooldown on the teleport.
	        m_teleportCounter = m_teleportCoolDown;

	        Direction = new Vector2((float)x,(float)y);
	        // Our collisionbox should update since we changed direction.
	        UpdateCollisionBox();

	        // Set direction and animation of the aztec, and also
	        // set the temporary bounding box properties for checking 
	        // teleport distance values.
	        AABB teleportAABB = m_boundingBox.Copy();
	        teleportAABB.Owner = Owner;
	        Vector2 pos = Owner.Position;

	        float ignoreForward = 40;

	        if(x > 0) { // Right
                Owner.ChangeDirection(Directions.Right);
                Owner.ChangeAnimation(8*4 + (int)Owner.AnimationDirection);
		        teleportAABB.MaxX = pos.X + m_teleportDistance;
		        teleportAABB.MinX = pos.X + ignoreForward;
		        teleportAABB.MinY += 5;
		        teleportAABB.MaxY -= 5;
	        } else if(x < 0) { // Left
                Owner.ChangeDirection(Directions.Left);
                Owner.ChangeAnimation(8*4 + (int)Owner.AnimationDirection);
		        teleportAABB.MaxX = pos.X - ignoreForward;
		        teleportAABB.MinX = pos.X - m_teleportDistance;
		        teleportAABB.MinY += 5;
		        teleportAABB.MaxY -= 5;
	        } else if(y < 0) { // Up
                Owner.ChangeDirection(Directions.Up);
                Owner.ChangeAnimation(8*4 + (int)Owner.AnimationDirection);
		        teleportAABB.MinX += 5;
		        teleportAABB.MaxX -= 5;
		        teleportAABB.MaxY = pos.Y - ignoreForward;
		        teleportAABB.MinY = pos.Y - m_teleportDistance;
	        } else { // Down
                Owner.ChangeDirection(Directions.Down);
                Owner.ChangeAnimation(8*4 + (int)Owner.AnimationDirection);
		        teleportAABB.MinX += 5;
		        teleportAABB.MaxX -= 5;
		        teleportAABB.MaxY = pos.Y + m_teleportDistance;
		        teleportAABB.MinY = pos.Y + ignoreForward;
	        }

	        // It costs mana to teleport.
	        (Owner as Hero).DecreaseMana(m_teleportManaCost);

	        // Check if we collide with someone on the way.
            List<AABB> boxes = Collision.GetCollidingBoxes(teleportAABB);
	        if(boxes.Count() > 0 ) {
		        // The nearest value depends on which direction we jump.
		        float nearest;
		        if(x < 0 || y < 0)
			        nearest = -9999999;
		        else
			        nearest = 9999999;

		        foreach(AABB box in boxes) {
			        // Only teleport to units and obstacles.
			        if(box.Owner is Obstacle || box.Owner is Unit ) {
				        bool isObstacle = box.Owner is Obstacle;
				        // Check if the nearest value should be updated.
				        if(x < 0) {
					        // Allow jumping near obstacles.
					        if(isObstacle) { 
						        if(pos.Y > box.MaxY || pos.Y < box.MinY)
							        continue;
					        }
					        if(box.MaxX > nearest) { 
						        nearest = box.MaxX; 
					        } 
				        }
				        else if(x > 0) {
					        if(isObstacle) { 
						        if(pos.Y > box.MaxY || pos.Y < box.MinY)
							        continue;
					        }
					        if(box.MinX < nearest) { 
						        nearest = box.MinX; 
					        } 
				        }
				        else if(y < 0) {
					        if(isObstacle) { 
						        if(pos.X > box.MaxX || pos.X < box.MinX)
							        continue;
					        }
					        if(box.MaxY > nearest) { 
						        nearest = box.MaxY; 
					        } 
				        }
				        else {
					        if(isObstacle) { 
						        if(pos.X > box.MaxX || pos.X < box.MinX)
							        continue;
					        }
					        if(box.MinY < nearest) { 
						        nearest = box.MinY; 
					        } 
				        }
			        }
		        }

		        if(nearest < -999999 || nearest > 999999) {
			        // We didn't find a good target, just teleport normally.
			        Owner.Position += Direction *m_teleportDistance;

			        // Black magic.
			        m_extraGraphicCounter = m_teleportExtraGraphic;
		        } else {
			        // We found something to teleport to...
			        float distance = 0;
                    float w = Owner.MovementBB.Width;
			        float h = Owner.MovementBB.Height; 
        			
			        // Calculate the distance to teleport.
			        if(x < 0)		distance = pos.X - nearest - w;
			        else if(x > 0)	distance = nearest - pos.X - w;
			        else if(y < 0)	distance = pos.Y - nearest - h;
			        else			distance = nearest - pos.Y - h;

			        // We don't want to teleport backwards.
			        if(distance < 0) distance = 0;

			        Owner.Position = pos + Direction*distance;

			        // Black magic.
			        m_extraGraphicCounter = (int)(m_teleportExtraGraphic * (distance/m_teleportDistance));
		        }
	        } else {
		        // We didn't collide on the way, teleport all the way.
		        Owner.Position += Direction * m_teleportDistance;

		        // Black magic.
		        m_extraGraphicCounter = m_teleportExtraGraphic;
	        }
        }

        public override void OnCollide(AABB other)
        {
            if (other.Owner == Owner) return;

            if (other.Owner is Unit)
            {
                Unit unit = other.Owner as Unit;
                if (unit.IsAlive)
                {
                    unit.ApplyDamage(Damage, Owner);
                }
            }
        }

        public void UpdateCollisionBox()
        {
            Vector2 position = Position;
           
            if (Direction.Y > 0)
            {
	            m_boundingBox.MinY = position.Y - 75 + 80;
	            m_boundingBox.MaxY = position.Y - 40 + 80;
	            m_boundingBox.MinX = position.X - 40;
	            m_boundingBox.MaxX = position.X + 40;
            }
            else if (Direction.Y < 0)
            {
	            m_boundingBox.MinY = position.Y - 75;
	            m_boundingBox.MaxY = position.Y - 40;
	            m_boundingBox.MinX = position.X - 40;
	            m_boundingBox.MaxX = position.X + 40;
            } 
            else if (Direction.X > 0)
            {
	            m_boundingBox.MinX = position.X + 30;
	            m_boundingBox.MaxX = position.X + 65;
	            m_boundingBox.MinY = position.Y - 50;
	            m_boundingBox.MaxY = position.Y + 30;
            }
            else if (Direction.X < 0)
            {
	            m_boundingBox.MinX = position.X - 70;
	            m_boundingBox.MaxX = position.X - 35;
	            m_boundingBox.MinY = position.Y - 50;
	            m_boundingBox.MaxY = position.Y + 30;
            }
        }
        public override void Draw()
        {
	        if (m_extraGraphicCounter > 0){

		        Vector2 pos = Owner.DrawOffset;

		        pos.X -= m_extraGraphicCounter * m_teleportDistance/m_teleportExtraGraphic * Direction.X;
		        pos.Y -= m_extraGraphicCounter * m_teleportDistance/m_teleportExtraGraphic * Direction.Y;

                int roundedCounter = (int)Math.Floor(m_extraGraphicCounter);

                for (int i = roundedCounter; i > 0; i--)
                {
			        pos.X += Direction.X*(m_teleportDistance / m_teleportExtraGraphic);
			        pos.Y += Direction.Y*(m_teleportDistance / m_teleportExtraGraphic);
			        
                    Graphics.Draw(
			        Texture,
			        pos,
                    2.0f,
                    Owner.Animations.Rectangle,
                    Layer + (float)Position.Y / 768.0f, Color.White);
		        }
	        }
        }
    }
}