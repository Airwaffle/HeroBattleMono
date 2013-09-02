using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework.GamerServices;


namespace HeroBattleArena.Game
{
    public class ScoreComponent
    {
        public ScoreComponent(string name, int score, int wave, string heroes, int kills)
        {
            Name = name;
            Score = score;
            Wave = wave;
            Heroes = heroes;
            Kills = kills;
        }
        public ScoreComponent(){ }
        public string Name = "";
        public int Score = 0;
        public int Wave = 0;
        public int Kills = 0;
        public string Heroes = "";
    }

    public class ScoreList
    {
        public ScoreComponent[] Components = new ScoreComponent[5];
        public ScoreList()
        {
            for (int i = 0; i < 5; i++)
                Components[i] = new ScoreComponent();
        }
    }

    public static class HighScoreList
    {
        private static string m_Path = @"Configuration\SaveFile.xml";
        public static ScoreList[] m_Lists = new ScoreList[4];

        public static void Initialize()
        {
            for (int i = 0; i < 4; i++)
                m_Lists[i] = new ScoreList();
            Load(m_Path);
        }

        public static int CheckNewComponent(int nrPlayers, ScoreComponent newComponent)
        {
            ScoreComponent[] list = m_Lists[nrPlayers-1].Components;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].Wave < newComponent.Wave)
                    return i;
                else if (list[i].Wave == newComponent.Wave)
                {
                    if (list[i].Kills < newComponent.Kills)
                        return i;
                    else if (list[i].Kills == newComponent.Kills && list[i].Score < newComponent.Score)
                        return i;
                }                      
            }
            return -1;
        }

        public static void AddNewComponent(int nrPlayers, ScoreComponent newComponent, int position)
        {
            ScoreComponent[] list = m_Lists[nrPlayers].Components;
            for (int i = 4; i > position; i--)
            {
                list[i] = list[i - 1];
            }
            list[position] = newComponent;
        }

        public static void ReplaceComponentName(int nrPlayers, string newName, int position)
        {
            ScoreComponent[] list = m_Lists[nrPlayers].Components;
            list[position].Name = newName;
            Save();
        }


        public static ScoreComponent GetComponent(int nrPlayers, int index)
        {
            return m_Lists[nrPlayers].Components[index];
        }

        public static void Save()
        {
            FileSaver.StartSaveData();
            /*
            XDocument doc = XDocument.Load(m_Path);
            XElement root = doc.Root;

            IEnumerable<XElement> ScoreLists = root.Elements(XName.Get("ScoreList"));
            int players = 0;

            foreach (XElement scoreList in ScoreLists)
            {
                IEnumerable<XElement> ScoreComponents = scoreList.Elements(XName.Get("List"));
                int components = 0;
                foreach (XElement scoreComponent in ScoreComponents)
                {
                IEnumerable<XAttribute> attributes = scoreComponent.Attributes();
                    foreach (XAttribute attr in attributes)
                    {
                        if (attr.Name == "name")
                            attr.Value = m_Lists[players].Components[components].Name;
                        else if (attr.Name == "wave")
                            attr.Value = m_Lists[players].Components[components].Wave.ToString();
                        else if (attr.Name == "total_score")
                            attr.Value = m_Lists[players].Components[components].Score.ToString();
                        else if (attr.Name == "heroes")
                            attr.Value = m_Lists[players].Components[components].Heroes;
                        else if (attr.Name == "kills")
                            attr.Value =  m_Lists[players].Components[components].Kills.ToString();
                    }
                    components++;
                }
                players++;
            }

            XElement unlock = root.Element(XName.Get("ZombieUnlocked"));
            XAttribute unlockAttr = unlock.Attribute(XName.Get("value"));
            unlockAttr.Value = EntityManager.GetUnlockedChar(0) ? "true" : "false";

            XElement musicVolume = root.Element(XName.Get("MusicVolume"));
            XAttribute musicVolumeAttr = musicVolume.Attribute(XName.Get("value"));
            musicVolumeAttr.Value = (SoundCenter.Instance.MasterMusicVolume).ToString();

            XElement soundVolume = root.Element(XName.Get("SoundVolume"));
            XAttribute soundVolumeAttr = soundVolume.Attribute(XName.Get("value"));
            soundVolumeAttr.Value = (SoundCenter.Instance.MasterSoundVolume).ToString();

            doc.Save(m_Path);
            */
        }

        private static void Load(string file)
        {
            FileSaver.StartLoadData();
            /*
            XDocument doc = XDocument.Load(file);
            XElement root = doc.Root;
            IEnumerable<XElement> ScoreLists = root.Elements(XName.Get("ScoreList"));
            int players = 0;

            foreach (XElement scoreList in ScoreLists)
            {
                IEnumerable<XElement> ScoreComponents = scoreList.Elements(XName.Get("List"));
                int components = 0;
                foreach (XElement scoreComponent in ScoreComponents)
                {
                    string name = "";
                    int wave = 0;
                    int score = 0;
                    int kills = 0;
                    string heroes = "";

                    IEnumerable<XAttribute> attributes = scoreComponent.Attributes();
                    foreach (XAttribute attr in attributes)
                    {
                        if (attr.Name == "name")
                            name = attr.Value;
                        else if (attr.Name == "wave")
                            wave = int.Parse(attr.Value);
                        else if (attr.Name == "total_score")
                            score = int.Parse(attr.Value);
                        else if (attr.Name == "heroes")
                            heroes = attr.Value;
                        else if (attr.Name == "kills")
                            kills = int.Parse(attr.Value);
                    }
                    m_Lists[players].Components[components] = new ScoreComponent(name, score, wave, heroes, kills);
                    components++;
                }
                players++;
            }

            XElement unlock = root.Element(XName.Get("ZombieUnlocked"));
            XAttribute unlockAttr = unlock.Attribute(XName.Get("value"));
            if (unlockAttr.Value == "true")
                EntityManager.UnlockChar(0);

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ",";

            XElement soundVolume = root.Element(XName.Get("SoundVolume"));
            XAttribute soundVolumeAttr = soundVolume.Attribute(XName.Get("value"));
            SoundCenter.Instance.MasterSoundVolume = float.Parse(soundVolumeAttr.Value, NumberStyles.Float, nfi);
            XElement musicVolume = root.Element(XName.Get("MusicVolume"));
            XAttribute musicVolumeAttr = musicVolume.Attribute(XName.Get("value"));
            SoundCenter.Instance.MasterMusicVolume = float.Parse(musicVolumeAttr.Value, NumberStyles.Float, nfi);
            */
        }
    }
}
