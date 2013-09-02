using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HeroBattleArena.Game;
using HeroBattleArena.Game.GameObjects;

namespace HeroBattleArena.Game
{
    public static class Collision
    {

        public static List<AABB> GetCollidingBoxes(AABB test)
        {
            List<AABB> collidingBoxes = new List<AABB>();
	        List<IEntity> entities = EntityManager.Entities;
	        for(int i = 0; i < entities.Count; ++i) 
            {
		        // We don't want to collide with the owner of the
		        // box, since that's the most usual case.
		        if(entities[i] == test.Owner) continue;

		        // Don't collide with entities that explicitly tells
		        // us not to collide with it.
		        if(!entities[i].IsSolid) continue;

		        // Check all boxes of every entity.
                List<AABB> boxes = entities[i].BoundingBoxes;
		        for(int k = 0; k < boxes.Count; ++k) 
                {
			        AABB box = boxes[k];

			        if ((box.LayerMask & test.CollisionMask) == 0 && (box.CollisionMask & test.LayerMask) == 0) continue;
				
			        // Single out collision cases fast.
			        if(box.MaxX < test.MinX) continue;
			        if(box.MinX > test.MaxX) continue;
			        if(box.MaxY < test.MinY) continue;
			        if(box.MinY > test.MaxY) continue;

                    collidingBoxes.Add(box);
		        }
	        }
	        return collidingBoxes;
        }

        public static void Update(float delta)
        {
            CheckNormalCollision();
            CheckUnitMovement(delta);
        }

        private static void CheckNormalCollision()
        {
            List<IEntity> entities = EntityManager.Entities;
	        int i = 0;
	        int j = 0;

	        while (i < entities.Count && j < entities.Count)
            {	
		        if (i != j)
		        {
			        List<AABB> boxesFirst	= entities[i].BoundingBoxes;
			        List<AABB> boxesSecond	= entities[j].BoundingBoxes;

			        // Check all collision boxes for every entity against each other.
					foreach(AABB first in boxesFirst) {
						foreach(AABB second in boxesSecond) {
					        if(first == second) continue;
					        if(IntersectBoxes(first, second)) 
							{
						        entities[i].OnCollide(second);
						        entities[j].OnCollide(first);
					        }
				        }
			        }
			        j++;
		        } 
		        else 
		        {	
			        i++;
			        j=0;
		        }
	        }
        }

		public static bool IntersectBoxes(AABB first, AABB second)
		{
			if ((first.LayerMask & second.CollisionMask) == 0 && (first.CollisionMask & second.LayerMask) == 0) return false;

			if (first.MaxX < second.MinX) return false;
			if (first.MinX > second.MaxX) return false;
			if (first.MaxY < second.MinY) return false;
			if (first.MinY > second.MaxY) return false;

			return true;
		}

		private static void CheckUnitMovement(float delta)
		{
			List<Unit> units = EntityManager.Units;
			List<Obstacle> obstacles = EntityManager.Obstacles;

			// Check collision with units' feet against the world objects.
			foreach(Unit unit in units)
			{
				if(unit == null) continue;
				if(!unit.IsAlive) continue;

				// Calculate the units movement vector.
                Vector2 movement = Vector2.Zero;
                if(unit != GameMode.Instance.SafeTimePlayer)
				    movement = unit.Movement * unit.MoveSpeed * delta * GameMode.Instance.TimeModifier;
                else
                    movement = unit.Movement * unit.MoveSpeed * delta;

				// Check collision with other units.
				if(unit.IsSolid) 
				{
					foreach(Unit unit2 in units)
					{
						if(unit == unit2) continue;
						if(unit2 == null) continue;
						if(!unit2.IsAlive) continue;
						if(!unit2.IsSolid) continue;
						movement = MoveUnit(unit.MovementBB, unit2.MovementBB, movement);
					}
				}

				if(!unit.IsSolid) 
				{
					// NO obstacle collision - check world borders...
					AABB aabb = unit.MovementBB;
					if		(movement.X < 0 && aabb.MinX + movement.X < 0) movement.X = -aabb.MinX;
					else if	(movement.X > 0 && aabb.MaxX + movement.X > 1024) movement.X = 1024 - aabb.MaxX;
					if		(movement.Y < 0 && aabb.MinY + movement.Y < 0) movement.Y = -aabb.MinY;
					else if	(movement.Y > 0 && aabb.MaxY + movement.Y > 768) movement.Y = 768 - aabb.MaxY;
				}
				else if (unit.IsSolid)
				{
					// Check collision against obstacles.
					for(int k = 0; k < obstacles.Count; ++k)
					{
						Obstacle obstacle = obstacles[k];
						if(obstacle == null) continue;

						List<AABB> boxes = obstacle.BoundingBoxes;
						// Check all collision boxes of the obstacles.
						foreach(AABB box in boxes)
						{
							movement = MoveUnit(unit.MovementBB, box, movement);
						}
					}
				}

				unit.Position += movement;
			}
		}

		private static Vector2 MoveUnit(AABB box, AABB box2, Vector2 movement)
		{
			if(movement.X == 0 && movement.Y == 0) return movement;
			Vector2 pos = new Vector2((box.MaxX+box.MinX)/2.0f,(box.MaxY+box.MinY)/2.0f);

			// Chose whether to check x or y and cancel the other out
			bool cancelX = true;
			if(box.MaxY < box2.MinY || box2.MaxY < box.MinY)
				cancelX = false;
			bool bCollided = false;

			if(movement.X > 0)
			{
				if(pos.X <= box2.MinX && box.MaxX + movement.X >= box2.MinX)
				{
					if((box2.MinY >= box.MinY && box2.MinY <= box.MaxY) || (box2.MaxY >= box.MinY && box2.MaxY <= box.MaxY))
					{
						// Calculate new movement.
						movement.X = box2.MinX - box.MaxX;
						bCollided = true;
					}
					else if((box.MinY >= box2.MinY && box.MinY <= box2.MaxY) || (box.MaxY >= box2.MinY && box.MaxY <= box2.MaxY))
					{
						// Calculate new movement.
						movement.X = box2.MinX - box.MaxX;
						bCollided = true;
					}
				}
			}
			else if(movement.X < 0)
			{
				if(pos.X >= box2.MaxX && box.MinX + movement.X <= box2.MaxX)
				{
					if((box2.MinY >= box.MinY && box2.MinY <= box.MaxY) || (box2.MaxY >= box.MinY && box2.MaxY <= box.MaxY))
					{
						// Calculate new movement.
						movement.X = box2.MaxX - box.MinX;
						bCollided = true;
					}
					else if((box.MinY >= box2.MinY && box.MinY <= box2.MaxY) || (box.MaxY >= box2.MinY && box.MaxY <= box2.MaxY))
					{
						// Calculate new movement.
						movement.X = box2.MaxX - box.MinX;
						bCollided = true;
					}
				}
			}
			// Move Y
			if(movement.Y > 0)
			{
				if(pos.Y <= box2.MinY && box.MaxY + movement.Y >= box2.MinY)
				{
					if((box2.MinX >= box.MinX && box2.MinX <= box.MaxX) || (box2.MaxX >= box.MinX && box2.MaxX <= box.MaxX))
					{
						// Calculate new movement.
						movement.Y = box2.MinY - box.MaxY;
						bCollided = true;
					}
					else if((box.MinX >= box2.MinX && box.MinX <= box2.MaxX) || (box.MaxX >= box2.MinX && box.MaxX <= box2.MaxX))
					{
						// Calculate new movement.
						movement.Y = box2.MinY - box.MaxY;
						bCollided = true;
					}
				}
			}
			else if(movement.Y < 0)
			{
				if(pos.Y >= box2.MaxY && box.MinY + movement.Y <= box2.MaxY)
				{
					if((box2.MinX >= box.MinX && box2.MinX <= box.MaxX) || (box2.MaxX >= box.MinX && box2.MaxX <= box.MaxX))
					{
						// Calculate new movement.
						movement.Y = box2.MaxY - box.MinY;
						bCollided = true;
					}
					else if((box.MinX >= box2.MinX && box.MinX <= box2.MaxX) || (box.MaxX >= box2.MinX && box.MaxX <= box2.MaxX))
					{
						// Calculate new movement.
						movement.Y = box2.MaxY - box.MinY;
						bCollided = true;
					}
				}
			}

			if(bCollided)
			{
				if(cancelX)
					movement.X = 0;
				else
					movement.Y = 0;
			}
			return movement;
		}
    }
}
