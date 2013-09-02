using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

public class Wave
{
    public bool isSpawning;
    public float spawn_per_second;
    public float spawning_below_ground;

    public int[] zombie_amount = { 0, 0, 0, 0 };
    public float[] zombie_speed = { 0, 0, 0, 0 };
    public float[] zombie_attackspeed = { 0, 0, 0, 0 };
    public float[] zombie_damage = { 0, 0, 0, 0 };
    public float[] zombie_health = { 0, 0, 0, 0 };
    public float[] zombie_unborrow = { 0, 0, 0, 0 };

    public Wave()
    {

    }
    public Wave(Wave wave)
    {
        isSpawning = wave.isSpawning;

        spawn_per_second = wave.spawn_per_second;
        spawning_below_ground = wave.spawning_below_ground;

        Array.Copy(wave.zombie_amount, zombie_amount, 4);
        Array.Copy(wave.zombie_speed, zombie_speed, 4);
        Array.Copy(wave.zombie_attackspeed, zombie_attackspeed, 4);
        Array.Copy(wave.zombie_damage, zombie_damage, 4);
        Array.Copy(wave.zombie_health, zombie_health, 4);
        Array.Copy(wave.zombie_unborrow, zombie_unborrow, 4);
    }
}


namespace HeroBattleArena.Game.GameObjects
{
    public class WaveController
    {
        private static List<Wave> m_waves = new List<Wave>();
        private static int m_currentWave = 0;
        private static int m_currentRevolution = 0;
        private static int m_BossWave = (int)Configuration.GetValue("Waves_Between_Bosses");
        private static int m_BossesMet = 0;

        public static Wave CurrentWave { get { return new Wave(m_waves[m_currentWave]); } set { m_waves[m_currentWave] = value; } }
        public static int CurrentWaveNumber { get { return m_currentWave + m_BossesMet + m_currentRevolution * (m_waves.Count); } }
        public static int ModuluWave { get { return m_currentWave; } }
        public enum Message { Next, Boss, End };

        public static void NextRevolution() { 
            m_currentRevolution++;
            m_currentWave = 0;// -1; //Becomes 0 once nextwave is called
        }

        public static Message NextWave()
        {
            if ((CurrentWaveNumber + 2) % m_BossWave == 0)
            {
                m_BossesMet++;
                return Message.Boss;
            }
            else if (++m_currentWave >= m_waves.Count) 
            {
                WaveController.NextRevolution();
                return Message.Next;
            }
            //else if (m_currentWave == m_waves.Count) return Message.Boss;
            else return Message.Next;
        }

        public WaveController()
        {

        }

        public static void Initialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"Configuration\WaveData.xml");

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            XmlElement root = doc.DocumentElement;

            m_waves.Clear();
            for (int i = 0; i < root.ChildNodes.Count; ++i)
            {
                if (root.ChildNodes[i].Name == "Wave")
                {
                    XmlElement value = root.ChildNodes[i] as XmlElement;
                    Wave wave = new Wave();
                    wave.zombie_amount = new int[4] { 0, 0, 0, 0 };

                    wave.spawn_per_second = 1 / Configuration.GetValue("Zombie_Spawn_Rate");
                    wave.spawning_below_ground = 0.5f;

                    wave.zombie_speed = new float[4] {
                        Configuration.GetValue("Zombie1_Move_Speed"),
                        Configuration.GetValue("Zombie2_Move_Speed"),
                        Configuration.GetValue("Zombie3_Move_Speed"),
                        Configuration.GetValue("Zombie4_Move_Speed")
                    };

                    wave.zombie_attackspeed = new float[4]{
                        Configuration.GetValue("Zombie1_Attack_Speed"),
                        Configuration.GetValue("Zombie2_Attack_Speed"),
                        Configuration.GetValue("Zombie3_Attack_Speed"),
                        Configuration.GetValue("Zombie4_Attack_Speed")
                    };

                    wave.zombie_damage = new float[4]{
                        Configuration.GetValue("Zombie1_Attack_Damage"),
                        Configuration.GetValue("Zombie2_Attack_Damage"),
                        Configuration.GetValue("Zombie3_Attack_Damage"),
                        Configuration.GetValue("Zombie4_Attack_Damage")
                    };

                    wave.zombie_health = new float[4]{
                        Configuration.GetValue("Zombie1_Max_Health"),
                        Configuration.GetValue("Zombie2_Max_Health"),
                        Configuration.GetValue("Zombie3_Max_Health"),
                        Configuration.GetValue("Zombie4_Max_Health")
                    };

                    wave.zombie_unborrow = new float[4]{
                        1,
                        1,
                        1,
                        1
                    };

                    foreach (XmlAttribute attr in value.Attributes)
                    {
                        if (attr.Name == "zombie1_amount")
                            wave.zombie_amount[0] = int.Parse(attr.Value);
                        else if (attr.Name == "zombie2_amount")
                            wave.zombie_amount[1] = int.Parse(attr.Value);
                        else if (attr.Name == "zombie3_amount")
                            wave.zombie_amount[2] = int.Parse(attr.Value);
                        else if (attr.Name == "zombie4_amount")
                            wave.zombie_amount[3] = int.Parse(attr.Value);
                        else if (attr.Name == "spawn_per_second")
                            wave.spawn_per_second = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "spawning_below_ground")
                            wave.spawning_below_ground = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie1_speed")
                            wave.zombie_speed[0] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie1_attackspeed")
                            wave.zombie_attackspeed[0] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie1_damage")
                            wave.zombie_damage[0] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie1_health")
                            wave.zombie_health[0] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie1_unborrow_speed")
                            wave.zombie_unborrow[0] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie2_speed")
                            wave.zombie_speed[1] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie2_attackspeed")
                            wave.zombie_attackspeed[1] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie2_damage")
                            wave.zombie_damage[1] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie2_health")
                            wave.zombie_health[1] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie2_unborrow_speed")
                            wave.zombie_unborrow[1] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie3_speed")
                            wave.zombie_speed[2] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie3_attackspeed")
                            wave.zombie_attackspeed[2] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie3_damage")
                            wave.zombie_damage[2] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie3_health")
                            wave.zombie_health[2] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie3_unborrow_speed")
                            wave.zombie_unborrow[2] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie4_speed")
                            wave.zombie_speed[3] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie4_attackspeed")
                            wave.zombie_attackspeed[3] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie4_damage")
                            wave.zombie_damage[3] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie4_health")
                            wave.zombie_health[3] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "mod_zombie4_unborrow_speed")
                            wave.zombie_unborrow[3] = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if (attr.Name == "change_map_to")
                            AIManager.AddMapChange(m_waves.Count, int.Parse(attr.Value));

                    }
                    wave.isSpawning = true;
                    m_waves.Add(wave);
                }
            }
        }

        public static void ResetWaves()
        {
            m_currentWave = 0;
            m_currentRevolution = 0;
            m_BossesMet = 0;
            AIManager.Reset();
        }
    }
}
