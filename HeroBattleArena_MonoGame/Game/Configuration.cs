using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace HeroBattleArena.Game
{
    public static class Configuration
    {
        private static List<float>  m_Values    = new List<float>();
        private static List<string> m_Tags      = new List<string>();

        public static void Initialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"Configuration\Configuration.xml");

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            XmlElement root = doc.DocumentElement;
            bool bDebug = root.Attributes[0].Value == "true" ? true : false;
            for(int i = 0; i < root.ChildNodes.Count; ++i)
            {
                if (root.ChildNodes[i].Name == "Field")
                {
                    XmlElement value = root.ChildNodes[i] as XmlElement;
                    string tag = "";
                    float val = 0;

                    foreach (XmlAttribute attr in value.Attributes)
                    {
                        if (attr.Name == "tag")
                            tag = attr.Value;
                        else if (attr.Name == "value" && !bDebug)
                            val = float.Parse(attr.Value, NumberStyles.Float, nfi);
                        else if(attr.Name == "debug_value" && bDebug)
                            val = float.Parse(attr.Value, NumberStyles.Float, nfi);
                    }
                    m_Tags.Add(tag);
                    m_Values.Add(val);
                }
            }
        }

        public static float GetValue(string tag)
        {
            for (int i = 0; i < m_Tags.Count; ++i)
            {
                if (m_Tags[i] == tag)
                    return m_Values[i];
            }
#if DEBUG
            Console.WriteLine("Tag \"" + tag + "\" not found in Configuration.xml");
#endif
            return 0;
        }
    }
}
