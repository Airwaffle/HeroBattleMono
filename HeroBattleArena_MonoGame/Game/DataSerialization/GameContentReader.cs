using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.DataSerialization
{
    public class GameContentLevel
    {
        public List<string> Tags = new List<string>();
        public List<string> Paths = new List<string>();
        public List<Texture2D> Textures = new List<Texture2D>();
    }

    public class GameContentReader
    {
        private int m_CurrentLevel = 0;
        private List<GameContentLevel> m_Levels = new List<GameContentLevel>();

        public GameContentReader(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlElement root = doc.DocumentElement;
            for(int i = 0; i < root.ChildNodes.Count; ++i)
            {
                if (root.ChildNodes[i].GetType() != typeof(XmlElement)) continue;
                XmlElement level = (XmlElement)root.ChildNodes[i];

                if (level.Name == "Level")
                {
                    m_Levels.Add(new GameContentLevel());
                    GameContentLevel CurrentLevel = m_Levels[m_Levels.Count - 1];

                    foreach (XmlNode data in level.ChildNodes)
                    {
                        if (data.Name == "Graphics")
                        {
                            string tag = "";
                            string path = "";

                            foreach (XmlAttribute attr in data.Attributes)
                            {
                                if (attr.Name == "tag" && tag == "")
                                    tag = attr.Value;
                                else if (attr.Name == "path" && path == "")
                                    path = attr.Value;
                            }

                            CurrentLevel.Tags.Add(tag);
                            CurrentLevel.Paths.Add(@"Textures\" + path);
                        }
                    }
                }
            }
        }

        public Texture2D GetTexture(string tag)
        {
            for (int i = m_CurrentLevel - 1; i >= 0; --i)
            {
                GameContentLevel level = m_Levels[i];
                for (int k = 0; k < level.Tags.Count; ++k)
                {
                    if (tag == level.Tags[k])
                    {
                        return level.Textures[k];
                    }
                }
            }

            throw new System.Exception("\"" + tag + "\" was not found.");
        }

        public void LoadNextLevel(ContentManager content)
        {
            for (int i = 0; i < m_Levels[m_CurrentLevel].Tags.Count; ++i)
            {
                m_Levels[m_CurrentLevel].Textures.Add(content.Load<Texture2D>(m_Levels[m_CurrentLevel].Paths[i]));
                
                
            }
            ++m_CurrentLevel;
        }
    }
}
