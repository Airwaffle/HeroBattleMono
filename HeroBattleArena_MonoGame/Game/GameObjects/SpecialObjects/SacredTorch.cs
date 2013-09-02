using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    class SacredTorch : DamageObject
    {
        Texture2D BackTexture = null;
        public SacredTorch() : base(null)
		{
			
		}

		public override void Initialize()
		{
            base.Initialize();
            

            Damage = Configuration.GetValue("Aztec_Fire_Damage");
            Name = "SacredTorch";
            Layer = 2 + 145/768.0f;
            Texture = Graphics.GetTexture("special_fire");
            BackTexture = Graphics.GetTexture("special_fire_back");

            // Add BoundingBox
            Vector2 position = Position;
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            boundingBox.MinY = position.Y + 100;
            boundingBox.MaxY = position.Y + 150;
            boundingBox.MinX = position.X + 25;
            boundingBox.MaxX = position.X + 75;
            AddAABB(boundingBox);

            // Animation
            AnimationManager animations = new AnimationManager(100, 200, 4, 1);
            animations.AddAnimation(new Animation(4, 0, 0, false, 0));
            animations.AnimationSpeed = 8;
            Animations = animations;
        }

        public override void Update(float delta)
		{
            Animations.Update(delta);
        }

        public override void Draw()
        {
            base.Draw();
            Graphics.Draw(BackTexture, Position, Animations.Rectangle, 2, Color.White);
        }
        public override void OnCollide(AABB other)
        {
            base.OnCollide(other);

            if (other.Owner is Hero)
            {
                Hero hero = other.Owner as Hero;
                if (hero.IsAlive)
                {
                    hero.ApplyDamage(Damage, Owner);
                }
            }

            // fire should kill grenades.
            if (other.Owner is StickyGrenadeProjectile)
            {
                StickyGrenadeProjectile grenade = other.Owner as StickyGrenadeProjectile;
                grenade.Life = 0;
                //grenade.->setVictim(0);
            }
        }
    }
}
