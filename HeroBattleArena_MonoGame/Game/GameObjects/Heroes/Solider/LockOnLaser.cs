using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    class LockOnLaser : HitObject
    {
        private Vector2 laserOffsetUp =   new Vector2();
        private Vector2 laserOffsetDown = new Vector2();
        private Vector2 laserOffsetLeft = new Vector2(-20,-10);
        private Vector2 laserOffsetRight= new Vector2(20,-10);

        private float m_laserCounter = 0;
	    private float m_existedCounter = 0;
	    private float m_timeNeeded = Configuration.GetValue("Lock_On_Time");
	    private float m_rememberTargetDelay = Configuration.GetValue("Lock_On_Remember_Delay");
	    private float m_rememberTargetCounter = 0;
	    private int m_maxRange = (int)Configuration.GetValue("Lock_On_Range");
	    private int m_range = (int)Configuration.GetValue("Lock_On_Range");
	    private Unit m_target = null;
	    private Unit m_targetRemembered = null;
	    private Ability m_ability = null;
	    private float m_manaDrain = Configuration.GetValue("Lock_On_ManaDrain_Low");
	    private float m_highManaDrain = Configuration.GetValue("Lock_On_ManaDrain_High");
	    private bool m_done = false;
	    private AnimationManager m_aimAnimation = null;
        private Texture2D laserTexture = Graphics.GetTexture("ability_lazer_aim");

        public Ability Ability { set { m_ability = value; } }

        public LockOnLaser(Unit owner, Vector2 direction)
            : base(owner)
		{
            Direction = direction;
		}
        public override void Initialize()
        {
            Damage = Configuration.GetValue("Lock_On_Damage");
	        Texture = Graphics.GetTexture("ability_lazer_seg");
            Life = 1000;

            /*AABB* boundingBox = new AABB();
	        boundingBox->owner = this;
	        boundingBox->layerMask = AABB::PROJECTILES_LAYER;
	        boundingBox->collisionMask = AABB::PROJECTILES_COL;
	        addBoundingBox(boundingBox);*/

	        AnimationManager animations = new AnimationManager(5, 5, 3, 2);
	        // Y animation.
	        animations.AddAnimation(new Animation(3,0,0,false,0)); 
	        // X animation.
	        animations.AddAnimation(new Animation(3,1,0,false,0)); 
	        
	        // Animation used when in Y direction.
	        if (Direction.Y != 0){
		        animations.ChangeAnimation(0);
	        }
	        // Animation used when in X direction.
	        else {
		        animations.ChangeAnimation(1);
	        }

            DrawOffset = new Vector2(Owner.Position.X, Owner.Position.Y);
        	
	        m_aimAnimation = new AnimationManager(160, 160, 4, 1);
	        m_aimAnimation.AddAnimation(new Animation(4, 0, 0, false, 0));
	        m_aimAnimation.StandardAnimationSpeed = 14;
            Animations = animations;
        }
        public override void Update(float delta)
        {
            Position = Owner.Position;
	        m_existedCounter += delta;
	        m_ability.Use();
            Hero ownerAsHero = Owner as Hero;

	        // Handles the removement of the object if mana is low, the hero has hit it's target or the player has let the button go. 
	        if (Owner is Hero){
                
                #if XBOX
                        if (!ownerAsHero.GetSpecialDown(0) || ownerAsHero.Mana < 5 || m_done || !Owner.IsAlive || Owner.IsStunned){
                #else
                        if (!ownerAsHero.GetSpecialDown(1) || ownerAsHero.Mana < 5 || m_done || !Owner.IsAlive || Owner.IsStunned){
                #endif
			        Hide();
			        m_target = null;
			        ownerAsHero.HeroState = HeroStates.IDLE;
			        ownerAsHero.CanAttack = true;
			        ownerAsHero.IsStrafing = false;
			        Remove();
			        return;
		        }
	        }

	        // Updates the object.
	        if (!IsRemoved){

		        // Decreases the mana depending if the player got (or not got) a target.
		        if (m_target != null){
			        if (ownerAsHero.HeroState == HeroStates.SPECIAL7){
				        ownerAsHero.HeroState = HeroStates.SPECIAL8;
			        }
			        ownerAsHero.DecreaseMana(m_highManaDrain*delta);
			        m_laserCounter += delta;

			        if (m_laserCounter >= m_timeNeeded){
				        m_done = true;
                        LockOnExplosion explo = new LockOnExplosion(Owner, new Vector2(0,0));
                        explo.Position = m_target.Position;
                        explo.Damage = Damage;
                        EntityManager.Spawn(explo);
                        SoundCenter.Instance.Play(SoundNames.SoldierLockOnExplosion);
			        }
		        } else {
			        ownerAsHero.DecreaseMana(m_manaDrain*delta);
			        if (ownerAsHero.HeroState == HeroStates.SPECIAL7){
				        ownerAsHero.HeroState = HeroStates.SPECIAL8;
			        }
			        m_rememberTargetCounter -= delta;
			        if (m_rememberTargetCounter <= 0){
				        m_laserCounter = 0;
			        }
		        }
        		
		        CheckCollision();
        		
		        base.Update(delta);
		        Animations.Update(delta);
		        m_aimAnimation.Update(delta);
	        }
        }
        public void CheckCollision()
        {
            Vector2 pos = Position;
            Vector2 dir = Direction;

            AABB testRect = new AABB();
            testRect.LayerMask = AABBLayers.LayerProjectile;
            testRect.CollisionMask = AABBLayers.CollisionProjectile;
            testRect.Owner = this;

                        // Create a temporary collision box to check
            // laser collision.
            if (dir.Y > 0){
	            // Down
	            testRect.MinX = pos.X - 5;
	            testRect.MaxX = pos.X + 5;
	            testRect.MinY = pos.Y;
	            testRect.MaxY = pos.Y + m_maxRange;
            } else if (dir.Y < 0){
	            // Up
	            testRect.MinX = pos.X - 5;
	            testRect.MaxX = pos.X + 5;
	            testRect.MinY = pos.Y - m_maxRange;
	            testRect.MaxY = pos.Y;
            }
            else if (dir.X > 0){
	            // Right
	            testRect.MinX = pos.X;
	            testRect.MaxX = pos.X + m_maxRange;
	            testRect.MinY = pos.Y - 17;
	            testRect.MaxY = pos.Y - 7;
            }
            else if (dir.X < 0){
	            // Left
	            testRect.MinX = pos.X - m_maxRange;
	            testRect.MaxX = pos.X;
	            testRect.MinY = pos.Y - 17;
	            testRect.MaxY = pos.Y - 7;
            }
        	
            m_range = m_maxRange;
            m_target = null;
            AABB hitBox = null;

            List<AABB> boxes = Collision.GetCollidingBoxes(testRect);
            // Sort the collided boxes and make sure we only use 
            // the closest one.
            foreach(AABB box in boxes) {
	            if(box.Owner is Unit) {
		            Unit unit = box.Owner as Unit;
		            // Dont collide with ourselves.
		            if(unit == Owner)
			            continue;

		            // Only collide if the unit is alive and
		            // if it is vurnable.
		            if(!unit.IsAlive || unit.IsInvincible)
			            continue;
	            }

	            if(dir.Y > 0) { // && pos.y > box->minY) {
		            // Down
		            float dif = Math.Abs(pos.Y - box.MinY);
		            if(dif < (float)m_range) {
			            m_range = (int)dif;
			            hitBox = box;
		            }
	            }
	            else if(dir.Y < 0) { // && pos.y < box->maxY) {
		            // Up
		            float dif = Math.Abs(box.MaxY - pos.Y);
		            if(dif < (float)m_range) {
			            m_range = (int)dif;
			            hitBox = box;
		            }
	            }
	            else if(dir.X > 0) { // && pos.x > box->minX) {
		            // Right
		            float dif = Math.Abs(pos.X - box.MinX);
		            if(dif < (float)m_range) {
			            m_range = (int)dif;
			            hitBox = box;
		            }
	            }
	            else if(dir.X < 0) { // && pos.x < box->maxX) {
		            // Left
		            float dif = Math.Abs(box.MaxX - pos.X);
		            if(dif < (float)m_range) {
			            m_range = (int)dif;
			            hitBox = box;
		            }
	            }
            }

            if(hitBox != null) {
	            // Now our hitBox variable is the object our laser hits.
	            // Lock on the hitBox owner if its a unit.
	            if (hitBox.Owner is Unit) {
		            Unit unit = hitBox.Owner as Unit;
		            // Our target is a unit.
		            m_target = unit;
		            if (m_targetRemembered != m_target){
			            m_laserCounter = 0;
			            m_targetRemembered = m_target;
		            }
		            m_rememberTargetCounter = m_rememberTargetDelay;
	            }
            }
        }
        public override void Draw()
        {
        if (!IsRemoved) {
		        // Revalue the laser range depending on our our firing
		        // offset.
		        float drawRange = (float)m_range;
		        Vector2 dir = Direction;
		        if(dir.Y > 0) // Down
			        drawRange -= laserOffsetDown.Y;
		        else if(dir.Y < 0) // Up
			        drawRange += laserOffsetUp.Y;
		        else if(dir.X > 0) // Right
			        drawRange -= laserOffsetRight.X;
		        else // Left
			        drawRange += laserOffsetLeft.X;
		        // Calculate how many segments which needs to be drawn.
		        int segments = (int)Math.Floor(drawRange/5.0f)+1;
		        // Our position is used as a center for the offsets.
		        Vector2 position = Owner.Position;
		        // Create the drawing rectangle which is used to draw
		        // the laser segments.
		        Vector2 dest = new Vector2(position.X, position.Y);
		        if(dir.Y > 0) {
			        dest.X += laserOffsetDown.X;
			        dest.Y += laserOffsetDown.Y;
		        }
		        else if(dir.Y < 0) {
			        dest.X += laserOffsetUp.X;
			        dest.Y += laserOffsetUp.Y;
		        }
		        else if(dir.X > 0) {
			        dest.X += laserOffsetRight.X;
			        dest.Y += laserOffsetRight.Y;
		        }
		        else {
			        dest.X += laserOffsetLeft.X;
			        dest.Y += laserOffsetLeft.Y;
		        }

		        float layer = (float)Layer;
		        float sm = 1.0f/(float)768;
		        int dx = (int)dir.X;
		        int dy = (int)dir.Y;
		        for (int i = 0; i < segments; ++i) {	
			        Graphics.Draw(
				        Texture,
				        dest,
                        1.0f, 
                        Animations.Rectangle,
				        layer + dest.Y*sm, Color.White);
        			
			        dest.X += dx*5;
			        dest.Y += dy*5;
		        }
        		
		        if (m_target != null){
			        Rectangle sourceRect = new Rectangle(0,0,160,160);
			        if ((int)(m_laserCounter*10) % 2 == 1){
				        sourceRect.X = 160;
			        }
			        dest.X = m_target.Position.X - 80;
			        dest.Y = m_target.Position.Y - 80;


			        Graphics.Draw(
                        laserTexture,
				        dest,
                        1.0f,
				        m_aimAnimation.Rectangle,
				        5, Color.White);
		        }
	        }
        }
    }
}
