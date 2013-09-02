using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class Wall
	{
		public Texture2D Texture = null;
        public string Name = "";
        public Vector2 Position = new Vector2();
        public Vector2 Middle = new Vector2();
        public float Opacity = 1.0f;
        public float WantedOpacity = 0.0f;
        private float fadeSpeed = 0.1f;
        public float Layer = 2;
        public Wall(String name, Vector2 pos, Vector2 middle, float layer) { Name = name; Texture = Graphics.GetTexture(name); Position = pos; Middle = middle; Layer = layer; }
        public void UpdateOpacity()
        {
            Opacity = Opacity * (1 - fadeSpeed) + WantedOpacity * fadeSpeed;
        }
	}

    public class NanoFence : Obstacle
    {
        private bool m_horizontal = true;
        private List<Wall> walls = new List<Wall>();

        public bool Horizontal { set { m_horizontal = value; } }

        public NanoFence():base(null) {}
        public void AddWall(string texture, Vector2 pos)
        {
            Vector2 middle = pos - new Vector2(-50, -125);
            float layer = 2 + pos.Y/768.0f;
            if (texture == "fence_left_start" || texture == "fence_right_start" || texture == "fence_middle_1"){
                layer += 97/768.0f;
            } else if (texture == "fence_middle_3"){
                layer += 87/768.0f;
            } else if (texture == "fence_middle_4" || texture == "fence_middle_5"){
                layer += 100/768.0f;
            } 
            walls.Add(new Wall(texture, pos, middle, layer));
        }


        public override void Initialize()
        {
            float xMin = 40000;
            float xMax = -40000;
            float yMin = 40000;
            float yMax = -40000;

            if (m_horizontal)
            {
                foreach (Wall wall in walls)
                {
                    if (wall.Position.X + 33 < xMin)
                        xMin = wall.Position.X + 33;
                    if (wall.Position.X + 116 > xMax)
                        xMax = wall.Position.X + 116;
                    if (wall.Position.Y + 82 < yMin)
                        yMin = wall.Position.Y + 82;
                }
                yMax = yMin + 18;
                if (walls.Count > 2)
                {
                    yMax += 18;
                    yMin += 18;
                }
            }
            else
            {
                foreach (Wall wall in walls)
                {
                    if (wall.Position.X + 15 < xMin)
                        xMin = wall.Position.X + 15;
                    if (wall.Position.X + 30 > xMax)
                        xMax = wall.Position.X + 30;
                    if (wall.Name == "fence_vert_1")
                    {
                        if (wall.Position.Y + 145 < yMin)
                            yMin = wall.Position.Y + 145;
                        if (wall.Position.Y + 260 > yMax)
                            yMax = wall.Position.Y + 260;

                        if (yMin < 90)
                        {
                            yMin = 0;
                        }
                    }
                    else
                    {
                        if (wall.Position.Y + 76 < yMin)
                            yMin = wall.Position.Y + 76;
                        if (wall.Position.Y + 356 > yMax)
                            yMax = wall.Position.Y + 356;
                    }
                }
            }

            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerStaticObject;
            boundingBox.CollisionMask = AABBLayers.CollisionStaticObject;
            boundingBox.MinY = yMin;
            boundingBox.MaxY = yMax;
            boundingBox.MinX = xMin;
            boundingBox.MaxX = xMax;
            AddAABB(boundingBox);

            AnimationManager animations = null;
            

            if (m_horizontal)
                animations = new AnimationManager(150, 150, 4, 1);
            else
                animations = new AnimationManager(50, 300, 4, 1);

            
            animations.AddAnimation(new Animation(4, 0, 0, false, 0));
            animations.AnimationSpeed = 8;
            Animations = animations;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            List<Hero> heroes = EntityManager.Heroes;
            List<Boss> bosses = EntityManager.Bosses;

            for (int i = 0; i < walls.Count; i++ )
            {
                float dist = 999999;
                for (int j = 0; j < heroes.Count; j++)
                {
                    float tempdist = (walls[i].Middle - heroes[j].Position).Length();
                    if (tempdist < dist){
                        dist = tempdist;
                     }
                }

                for (int j = 0; j < bosses.Count; j++)
                {
                    float tempdist = (walls[i].Middle - bosses[j].Position).Length();
                    if (tempdist < dist)
                    {
                        dist = tempdist;
                    }
                }

                if (dist < 200)
                {
                    walls[i].WantedOpacity = 1;
                }
                /*else if (dist < 120)
                {
                    walls[i].Opacity = 0.8f;
                    //walls[i].Opacity = 100 / dist;
                }
                else if (dist < 150)
                {
                    walls[i].Opacity = 0.5f;
                }
                else if (dist < 200)
                {
                    walls[i].Opacity = 0.2f;
                }*/
                else
                {
                    walls[i].WantedOpacity = 0;
                }

                walls[i].UpdateOpacity();
            }
        }
        
        public override void Draw()
        {
            foreach (Wall wall in walls)
            {
                Color color = new Color(new Vector4(wall.Opacity, wall.Opacity, wall.Opacity, wall.Opacity));
                Graphics.Draw(wall.Texture, wall.Position, Animations.Rectangle, wall.Layer, color);
            }
#if DEBUG
            foreach (AABB aabb in BoundingBoxes)
                Graphics.DrawAABB(aabb, DebugAABBMode.Body);
#endif
        }
        
        
    }
}
