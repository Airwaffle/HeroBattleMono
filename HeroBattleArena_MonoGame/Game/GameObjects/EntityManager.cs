using System.Collections.Generic;
using System;

namespace HeroBattleArena.Game.GameObjects
{
    public static class EntityManager
    {

#region Static Fields

        private static List<IEntity> s_TempEntities = new List<IEntity>();
        private static List<IEntity> s_Entities = new List<IEntity>();
        private static List<IEntity> s_HiddenEntities = new List<IEntity>();
        private static List<Hero> s_Heroes = new List<Hero>();
        private static List<Enemy> s_Enemies = new List<Enemy>();
		private static List<Unit> s_Units = new List<Unit>();
        private static List<Enemy> s_Pets = new List<Enemy>();
        private static List<Boss> s_Bosses = new List<Boss>();
		private static List<Obstacle> s_Obstacles = new List<Obstacle>();

        private static bool[] s_UnlockedChar = { false, false };

#endregion

#region Static Properties

        public static List<IEntity> Entities { get { return s_Entities; } }
        public static List<Hero> Heroes { get { return s_Heroes; } }
        public static List<Enemy> Enemies { get { return s_Enemies; } }
		public static List<Unit> Units { get { return s_Units; } }
        public static List<Enemy> Pets { get { return s_Pets; } }
        public static List<Boss> Bosses { get { return s_Bosses; } }
		public static List<Obstacle> Obstacles { get { return s_Obstacles; } }

#endregion

#region Static Methods


        public static void HideEverything(){
            foreach (IEntity entity in s_Entities)
                s_HiddenEntities.Add(entity);
            Clear();
        }

        public static void ShowHidden()
        {
            foreach (IEntity entity in s_HiddenEntities)
            {
                s_Entities.Add(entity);
                if (entity is Hero)
                    s_Heroes.Add(entity as Hero);
                if (entity is Enemy)
                    if ((entity as Enemy).Owner == null)
                    {
                        s_Enemies.Add(entity as Enemy);
                        if (entity is Boss)
                            s_Bosses.Add(entity as Boss);
                    }
                    else
                        s_Pets.Add(entity as Enemy);

                if (entity is Unit)
                    s_Units.Add(entity as Unit);
                if (entity is Obstacle)
                    s_Obstacles.Add(entity as Obstacle);
            }
            s_HiddenEntities.Clear();
        }



        public static int AmountOfInstancesExisting<T>()
        {
            int amount = 0;

            for (int i = 0; i < s_Entities.Count; i++)
            {
                if (s_Entities[i] is T)
                {
                    amount++;
                }
            }
            return amount;
        }

        /// <summary>
        /// Clears the entity list by removing all
        /// entities.
        /// </summary>
        public static void Clear()
        {
			s_TempEntities.Clear();
            s_Heroes.Clear();
            s_Enemies.Clear();
			s_Units.Clear();
            s_Pets.Clear();
			s_Obstacles.Clear();
            s_Bosses.Clear();
			s_Entities.Clear();
        }
        /// <summary>
        /// Adds and initializes the entity to the game.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public static void Spawn(IEntity entity)
        {
            entity.Initialize();
            #if DEBUG
            Console.WriteLine(entity.Name + " created.");
            #endif
            s_TempEntities.Add(entity);

            if (entity is Unit)
            {
                s_Units.Add(entity as Unit);
                if (entity is Hero)
                {
                    s_Heroes.Add(entity as Hero);
                }
                else if (entity is Enemy)
                {
                    if ((entity as Enemy).Owner == null)
                    {
                        s_Enemies.Add(entity as Enemy);
                        if (entity is Boss)
                        {
                            s_Bosses.Add(entity as Boss);
                        }
                    }
                    else
                    {
                        s_Pets.Add(entity as Enemy);
                    }
                }
            }
            else if (entity is Obstacle)
            {
                s_Obstacles.Add(entity as Obstacle);
            }
        }

        /// <summary>
        /// Updates and removes entities in the Entity Manager.
        /// </summary>
        /// <param name="delta"></param>
        public static void Update(float delta)
        {
            // Add the temporary entities to the entity list.
            while (s_TempEntities.Count > 0)
            {
                s_Entities.Add(s_TempEntities[0]);
                s_TempEntities.RemoveAt(0);
            }

            float timeMod = 1;
            if (GameMode.Instance != null)
                timeMod = GameMode.Instance.TimeModifier;

            // Update entities.
            for(int i = 0; i < s_Entities.Count; ++i)
            {
                if (timeMod == 1)
                    s_Entities[i].Update(delta);
                else
                {
                    if(s_Entities[i] != GameMode.Instance.SafeTimePlayer)
                        s_Entities[i].Update(delta * timeMod);
                    else if(s_Entities[i] is Projectile &&
                        ((Projectile)s_Entities[i]).Owner != GameMode.Instance.SafeTimePlayer)
                        s_Entities[i].Update(delta * timeMod);
                    else
                        s_Entities[i].Update(delta);
                }
            }

			// Remove obstacles
			for (int i = 0; i < s_Obstacles.Count; )
			{
				if (s_Obstacles[i].IsRemoved)
					s_Obstacles.RemoveAt(i);
				else ++i;
			}

            // Remove heroes.
            for (int i = 0; i < s_Heroes.Count; )
            {
                if (s_Heroes[i].IsRemoved)
                    s_Heroes.RemoveAt(i);
                else ++i;
            }

            // Removes enemies.
            for (int i = 0; i < s_Enemies.Count; )
            {
                if (s_Enemies[i].IsRemoved)
                    s_Enemies.RemoveAt(i);
                else ++i;
            }

            // Removes bosses.
            for (int i = 0; i < s_Bosses.Count; )
            {
                if (s_Bosses[i].IsRemoved)
                    s_Bosses.RemoveAt(i);
                else ++i;
            }

            // Removes pets.
            for (int i = 0; i < s_Pets.Count; )
            {
                if (s_Pets[i].IsRemoved)
                    s_Pets.RemoveAt(i);
                else ++i;
            }
            
			// Remove units.
			for (int i = 0; i < s_Units.Count; )
			{
				if (s_Units[i].IsRemoved)
					s_Units.RemoveAt(i);
				else ++i;
			}

            // Remove entities marked for deletion.
            for(int i = 0; i < s_Entities.Count;)
            {
                if (s_Entities[i].IsRemoved)
                {
                    s_Entities[i].Dispose();
                    s_Entities.RemoveAt(i);
                }
                else ++i;
            }
        }

        public static void LateUpdate(float delta)
        {
            // Run LateUpdate on all entities.
            for (int i = 0; i < s_Entities.Count; ++i)
            {
                s_Entities[i].LateUpdate(delta);
            }
        }

        public static bool GetUnlockedChar(int id)
        {
            if (id >= 2) return false;
            return s_UnlockedChar[id];
        }

        public static void UnlockChar(int id)
        {
            if (id >= 2) return;
            s_UnlockedChar[id] = true;
        }

        /// <summary>
        /// Dum jäkla pissfunktion som måste finnas pg.a inkompentens från två parter '_'
        /// </summary>
        public static void SortHeroesAfterOwner()
        {
            int i = 0;
            while (i < s_Heroes.Count)
            {
                if (i <= 0)
                    i++;
                else if (s_Heroes[i].PlayerOwner < s_Heroes[i - 1].PlayerOwner)
                {
                    Hero tmp = s_Heroes[i];
                    s_Heroes[i] = s_Heroes[i - 1];
                    s_Heroes[--i] = tmp;
                }
                else
                    i++;
            }
        }

#endregion

    }
}
