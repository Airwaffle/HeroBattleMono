using System;

namespace HeroBattleArena.Game.GameObjects
{
    public class AABBLayers
    {
        public const UInt16 Zero = 0x0000;
#region Layers
        public const UInt16 LayerHeroFeet               = 0x01;
        public const UInt16 LayerHeroBody               = 0x02;
        public const UInt16 LayerProjectile             = 0x04;
        public const UInt16 LayerHit                    = 0x08;
        public const UInt16 LayerStaticObject           = 0x10;
        public const UInt16 LayerPickup                 = 0x20;
        public const UInt16 LayerSensitiveProjectile    = 0x40;
#endregion
        public const UInt16 CollisionHeroFeet               = LayerStaticObject | LayerHeroFeet;
        public const UInt16 CollisionHeroBody               = 0;
        public const UInt16 CollisionProjectile             = LayerHeroBody | LayerStaticObject;
        public const UInt16 CollisionSensitiveProjectile    = LayerHeroBody | LayerStaticObject | LayerProjectile;
        public const UInt16 CollisionHit                    = LayerHeroBody | LayerStaticObject;
        public const UInt16 CollisionStaticObject           = 0;
        public const UInt16 CollisionPickup                 = LayerHeroFeet;
    }

    public class AABB
    {
#region Fields
        public float MinX = -10;
        public float MinY = -10;
        public float MaxX = 10;
        public float MaxY = 10;
        public UInt16 LayerMask = 0;
        public UInt16 CollisionMask = 0;
        public IEntity Owner = null;
#endregion
#region Properties
        /// <summary>
        /// Gets the width of the bounding box.
        /// </summary>
        public float Width { get { return Math.Abs(MaxX - MinX); } }
        /// <summary>
        /// Gets the height of the bounding box.
        /// </summary>
        public float Height { get { return Math.Abs(MaxY - MinY); } }
#endregion
        /// <summary>
        /// Create a new AABB with all fields set to 0.
        /// </summary>
        public AABB() { }
        /// <summary>
        /// Create a new AABB, specifying box dimensions.
        /// </summary>
        public AABB(float minX, float minY, float maxX, float maxY)
        {
            this.MinX = minX;
            this.MinY = minY;
            this.MaxX = maxX;
            this.MaxY = maxY;
        }
		public AABB Copy()
		{
			AABB copy = new AABB();
			copy.LayerMask = this.LayerMask;
			copy.CollisionMask = this.CollisionMask;
			copy.MinX = this.MinX;
			copy.MinY = this.MinY;
			copy.MaxX = this.MaxX;
			copy.MaxY = this.MaxY;
			copy.Owner = this.Owner;
			return copy;
		}
    }
}
