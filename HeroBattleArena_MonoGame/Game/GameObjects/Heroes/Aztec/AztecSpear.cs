using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class AztecSpear : HitObject
    {
        public AztecSpear(Unit owner, Vector2 direction)
			: base(owner)
		{
			Direction = direction;
		}

		public override void Initialize()
		{
			base.Initialize();
            Hide();
			Name = "Aztec Spear";
            Range = Configuration.GetValue("Aztec_Spear_Range");
            //const int halfWidth = 8;			

			AnimationManager animations = null;
			if (Direction.Y != 0)
				animations = new AnimationManager(20, 30, 1, 1);
			else
				animations = new AnimationManager(30, 20, 1, 1);
			animations.AddAnimation(new Animation(1, 0, 0, false, 0)); 
			Animations = animations;

            

			Vector2 position = Position;

			// Add BoundingBox.
			AABB boundingBox = new AABB();
			boundingBox.Owner = this;
			boundingBox.LayerMask = AABBLayers.LayerProjectile;
			boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            if (Direction.Y > 0)
            {
                boundingBox.MinX = position.X - 00;
                boundingBox.MaxX = position.X + 20;
                boundingBox.MinY = position.Y - 45;
                boundingBox.MaxY = position.Y + 5;
            }
            else if (Direction.Y < 0)
            {
                boundingBox.MinX = position.X - 20;
                boundingBox.MaxX = position.X + 00;
                boundingBox.MinY = position.Y - 20;
                boundingBox.MaxY = position.Y + 30;
            }
            else if (Direction.X > 0)
            {
                boundingBox.MinX = position.X - 30;
                boundingBox.MaxX = position.X + 20;
                boundingBox.MinY = position.Y - 10;
                boundingBox.MaxY = position.Y + 10;
            }
            else if (Direction.X < 0)
            {
                boundingBox.MinX = position.X - 20;
                boundingBox.MaxX = position.X + 30;
                boundingBox.MinY = position.Y - 10;
                boundingBox.MaxY = position.Y + 10;
            }
			AddAABB(boundingBox);
		}

		public override void Update(float delta)
		{
			base.Update(delta);
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
					SoundCenter.Instance.Play(SoundNames.AllImpact);
					unit.ApplyDamage(Damage, Owner);
					Remove();
				}
			}
		}
    }
}
