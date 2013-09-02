using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class ElectricCelerityHit: HitObject
    {

        public ElectricCelerityHit(Unit owner): base(owner)
        {

        }

        public override void Initialize()
        {
            Hide();
            Life = Configuration.GetValue("ElectricCelerityBuff_Duration");
            Damage = Configuration.GetValue("ElectricCelerityHit_Damage");

            Vector2 position = Position;

            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            boundingBox.MinX = position.X - 20;
            boundingBox.MaxX = position.X + 20;
            boundingBox.MinY = position.Y - 25;
            boundingBox.MaxY = position.Y + 25;
            AddAABB(boundingBox);

            AnimationManager animations = new AnimationManager(20, 20, 1, 1);
            animations.AddAnimation(new Animation(1,0,0,false,0)); 
            Animations = animations;
        }

        public override void Update(float delta){
            base.Update(delta);
            Position = Owner.Position;
        }

        public override void OnCollide(AABB other){
            if (other.Owner == Owner) return;
            Unit Other = (other.Owner as Unit);
            if(Other != null)
            {
	            if(Other.IsAlive){
		            Other.ApplyDamage(Damage, Owner);
	            }
            }
        }
    }
}
