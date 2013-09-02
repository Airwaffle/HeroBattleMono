using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
	public class ArthurSword : HitObject
	{
		public ArthurSword(Unit owner, Vector2 direction)
			: base(owner)
		{
			Direction = direction;
		}

		public override void Initialize()
		{
			base.Initialize();

			Name = "ArthurSword";

			Hide();
			AnimationManager animations = null;
			if (Direction.Y != 0)
				animations = new AnimationManager(20, 30, 1, 1);
			else
				animations = new AnimationManager(30, 20, 1, 1);
			animations.AddAnimation(new Animation(1, 0, 0, false, 0)); 
			Animations = animations;

			Range = Configuration.GetValue("Arthur_Sword_Range");

			Vector2 position = Position;

			// Add BoundingBox.
			AABB boundingBox = new AABB();
			boundingBox.Owner = this;
			boundingBox.LayerMask = AABBLayers.LayerProjectile;
			boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
			if (Direction.Y > 0)
			{
				boundingBox.MinY = position.Y - 75 + 80;
				boundingBox.MaxY = position.Y - 40 + 80;
				boundingBox.MinX = position.X - 40;
				boundingBox.MaxX = position.X + 40;
			} 
			else if (Direction.Y < 0)
			{
				boundingBox.MinY = position.Y - 75 + 25;
				boundingBox.MaxY = position.Y - 40 + 25;
				boundingBox.MinX = position.X - 40;
				boundingBox.MaxX = position.X + 40;
			} 
			else if (Direction.X > 0)
			{
				boundingBox.MinX = position.X + 2;
				boundingBox.MaxX = position.X + 37;
				boundingBox.MinY = position.Y - 35;
				boundingBox.MaxY = position.Y + 45;
			} 
			else if (Direction.X < 0)
			{
				boundingBox.MinX = position.X - 37;
				boundingBox.MaxX = position.X - 2;
				boundingBox.MinY = position.Y - 35;
				boundingBox.MaxY = position.Y + 45;
			}
			AddAABB(boundingBox);
		}

		public override void Update(float delta)
		{
			base.Update(delta);

			Position = Owner.Position;
		}

		public override void OnCollide(AABB other)
		{
			if (other.Owner == Owner) return;
			base.OnCollide(other);
			
			if (other.Owner is Unit)
			{
				Unit unit = other.Owner as Unit;
				if (unit.IsAlive)
				{
					unit.ApplyDamage(Damage, Owner);
					Remove();
				}
			}
		}
	}
}
