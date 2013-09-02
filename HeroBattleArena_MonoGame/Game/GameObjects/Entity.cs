using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class Entity : IEntity
    {

#region Static Properties
        public static SpriteBatch SpriteBatch { get; set; }
#endregion

#region Fields
        private List<AABB> m_BoundingBoxes  = new List<AABB>();
        private Vector2 m_Position          = Vector2.Zero;

        private bool m_bRemoved = false;
        private bool m_bVisible = true;
        private bool m_bSolid   = true;

		private Color m_Color = Color.White;

        private float m_Layer = 1;
        private Texture2D m_Texture = null;
	
	    private Vector2	m_DrawOffset = Vector2.Zero;
		private float m_Scale = 1.0f;

	    private AnimationManager m_Animations = new AnimationManager();
#endregion

#region Properties
		/// <summary>
		/// The color channel to use when rendering.
		/// </summary>
		public Color Color { get { return m_Color; } set { m_Color = value; } }
		/// <summary>
		/// The entity's name. Used for logging/debugging purposes.
		/// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Gets the entity's texture.
        /// Protected set for the texture.
        /// </summary>
        public Texture2D Texture { get { return m_Texture; } protected set { m_Texture = value; } }
		/// <summary>
		/// Gets or sets the scale used to draw the entity.
		/// </summary>
		public float Scale { get { return m_Scale; } protected set { m_Scale = value; } }
		/// <summary>
        /// Gets the entity's bounding boxes.
        /// Call .AddAABB(...) to add more boxes. 
        /// </summary>
        public List<AABB> BoundingBoxes { get { return m_BoundingBoxes; } protected set { m_BoundingBoxes = value; } }
        /// <summary>
        /// Gets whether the entity has been marked for removal or not.
        /// </summary>
        public bool IsRemoved { get { return m_bRemoved; } }
        /// <summary>
        /// Gets if the entity should be drawn or not.
        /// </summary>
        public bool IsVisible { get { return m_bVisible; } }
        /// <summary>
        /// Gets or sets the base layer for the entity when it's drawn.
        /// </summary>
        public float Layer { get { return m_Layer; } protected set { m_Layer = value; } }
        /// <summary>
        /// Gets or sets the current position of the entity.
        /// </summary>
        public virtual Vector2 Position
        {
            get { return m_Position; }
            set 
            {
                Vector2 difference = value - m_Position;
                for (int i = 0; i < m_BoundingBoxes.Count; ++i)
                {
                    m_BoundingBoxes[i].MinX += difference.X;
                    m_BoundingBoxes[i].MaxX += difference.X;
                    m_BoundingBoxes[i].MinY += difference.Y;
                    m_BoundingBoxes[i].MaxY += difference.Y;
                }
				m_DrawOffset.X += difference.X;
				m_DrawOffset.Y += difference.Y;
                m_Position = value; 
            }
        }
        /// <summary>
        /// The the offset from the entitys position that the sprite
        /// will be drawn.
        /// </summary>
        public Vector2 DrawOffset { get { return m_DrawOffset; } protected set { m_DrawOffset = value; } }
        /// <summary>
        /// Gets or sets whether this entity is solid or not.
        /// If the entity is not solid, it won't collide with other entities.
        /// </summary>
        public bool IsSolid { get { return m_bSolid; } set { m_bSolid = value; } }
        /// <summary>
        /// The animations associated with the entity.
        /// </summary>
        public AnimationManager Animations
        {
            get { return m_Animations; }
            protected set { m_Animations = value; }
        }

#endregion

#region Initialization
        /// <summary>
        /// Called just before the entity is removed.
        /// </summary>
        public virtual void Dispose() { }
        /// <summary>
        /// Creates a new entity.
        /// </summary>
        public Entity() { }
        ~Entity() 
        {
#if DEBUG
            Console.WriteLine(Name + " removed.");
#endif
            m_BoundingBoxes.Clear();
        }

        /// <summary>
        /// Initializes the entity, this is called automatically when using EntityManager.Spawn(...).
        /// </summary>
        public virtual void Initialize() { Name = "Entity"; }

#endregion

#region Protected
        /// <summary>
        /// Adds a new bounding box to the entitity.
        /// The owner is set automatically.
        /// The boundingbox will update its position when the entitys
        /// position is changed.
        /// </summary>
        /// <param name="aabb">The bounding box to add.</param>
        protected void AddAABB(AABB aabb)
        {
            aabb.Owner = this;
            m_BoundingBoxes.Add(aabb);
        }

#endregion

#region Public Methods

        /// <summary>
        /// Marks the entity for removal, the entity will be cleared
        /// during the next frame.
        /// </summary>
        public virtual void Remove() { m_bRemoved = true; }
        /// <summary>
        /// Disables drawing of the entity.
        /// </summary>
        public void Hide() { m_bVisible = false; }
        /// <summary>
        /// Enables drawing of the entity.
        /// </summary>
        public void Show() { m_bVisible = true; }
        /// <summary>
        /// Update the entity's state my moving it forward by
        /// delta in time.
        /// Called automatically from the EntityManager.
        /// </summary>
        /// <param name="delta">The time elapsed since last frame.</param>
        public virtual void Update(float delta) 
        {
            if (m_bVisible)
                m_Animations.Update(delta);
        }
        /// <summary>
        /// LateUpdate is called after Update and after collision
        /// has been checked.
        /// </summary>
        /// <param name="delta"></param>
        public virtual void LateUpdate(float delta) { }
        /// <summary>
        /// Draws the entity to the screen.
        /// This is called automatically from the EntityManager.
        /// </summary>
        public virtual void Draw()
        {
            if (m_bVisible && m_Texture != null)
            {
				Graphics.Draw(
					m_Texture,
					m_DrawOffset,
					m_Scale,
					m_Animations.Rectangle,
					m_Layer + m_Position.Y / 768.0f, m_Color);
            }
#if DEBUG
			foreach (AABB aabb in m_BoundingBoxes)
				Graphics.DrawAABB(aabb, DebugAABBMode.Body);
#endif
        }
        /// <summary>
        /// Collision handling with another AABB, be sure to check
        /// its owner.
        /// </summary>
        /// <param name="other">The AABB which we collide with.</param>
        public virtual void OnCollide(AABB other) { }
        /// <summary>
        /// Change the animation played in Animations.
        /// </summary>
        /// <param name="animation">The animation ID to play.</param>
        public void ChangeAnimation(int animation)
        {
            m_Animations.ChangeAnimation(animation);
        }

#endregion
    }
}
