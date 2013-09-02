using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class JawelinProjectile : Projectile
    {
        public JawelinProjectile(Unit owner, Vector2 direction)
            : base(owner, direction)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
            Name = "Jawelin";
            Texture = Graphics.GetTexture("aztec_jawelin");

            Damage = Configuration.GetValue("Slow_Down_Spear_Damage");
	        Speed = Configuration.GetValue("Slow_Down_Spear_Speed");
	        Life = Configuration.GetValue("Slow_Down_Spear_Range")/Speed;
        	
	        Vector2 position = Position;

	        AABB boundingBox = new AABB();
	        boundingBox.Owner = this;
	        boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
	        boundingBox.MinX = position.X - 10;
	        boundingBox.MaxX = position.X + 10;
	        boundingBox.MinY = position.Y - 10;
	        boundingBox.MaxY = position.Y + 10;
	        AddAABB(boundingBox);

	        AnimationManager animations = new AnimationManager(100, 100, 3, 4);
	        animations.AddAnimation(new Animation(3,0,0,false,0)); 
	        animations.AddAnimation(new Animation(3,1,0,false,0)); 
	        animations.AddAnimation(new Animation(3,2,0,false,0)); 
	        animations.AddAnimation(new Animation(3,3,0,false,0)); 
	        Animations = animations;

	        if (Direction.Y < 0)
            {
		        Animations.ChangeAnimation(0);
	        } else if (Direction.Y > 0)
            {
                Animations.ChangeAnimation(2);
            }
            else if (Direction.X < 0)
            {
                Animations.ChangeAnimation(1);
	        }else {
                Animations.ChangeAnimation(3);
	        }
        	
	        animations.AnimationSpeed = 20;

	        DrawOffset = new Vector2(position.X - 50, position.Y - 50);
        }

        public override void Update(float delta)
        {
            Vector2 pos = Position;
            pos.X += Direction.X * Speed * delta;
            pos.Y += Direction.Y * Speed * delta;
            Position = pos;

            base.Update(delta);
        }
        public override void OnCollide(AABB other)
        {
            if (other.Owner == Owner) return;
            base.OnCollide(other);

            if (other.Owner is Unit)
            {
                Unit unit = other.Owner as Unit;
                if (unit != null){
                    if (unit.IsAlive)
                    {
                        unit.ApplyDamage(Damage, Owner);
                        unit.AddBuff(new SlowdownBuff(unit));
                        Remove();
                    }
                }
            }
        }
    }
}
