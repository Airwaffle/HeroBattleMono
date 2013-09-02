using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
	public static class Effects
	{
		private static List<string>				s_Tags = new List<string>();
		private static List<EffectParameters>	s_EffectParams = new List<EffectParameters>();

        public static void Initialize()
        {
            string path = @"Configuration\Effects.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            XmlElement root = doc.DocumentElement;
            foreach (XmlElement element in root)
            {
                if (element.Name != "Effect") continue;

                EffectParameters param = new EffectParameters();
                string name = "";
                param.OffsetX = 0;
                param.OffsetY = 0;
                param.Loops = 1;
                param.FramesX = 1;
                param.FramesY = 1;
                param.NumberOfFrames = 0;

                foreach (XmlAttribute attr in element.Attributes)
                {
                    //if (attr.Name == "name")
                    //    name = attr.Value;
                    //else if (attr.Name == "layer")
                    //    int.TryParse(attr.Value, out param.Layer);
                    //else if (attr.Name == "draw_h")
                    //    int.TryParse(attr.Value, out param.DrawHeight);
                    //else if (attr.Name == "draw_w")
                    //    int.TryParse(attr.Value, out param.DrawWidth);
                    //else if (attr.Name == "frames_x")
                    //    int.TryParse(attr.Value, out param.FramesX);
                    //else if (attr.Name == "frames_y")
                    //    int.TryParse(attr.Value, out param.FramesY);
                    //else if (attr.Name == "frame_rate")
                    //    int.TryParse(attr.Value, out param.FrameRate);
                    //else if (attr.Name == "loops")
                    //    int.TryParse(attr.Value, out param.Loops);
                    //else if (attr.Name == "offset_x")
                    //    float.TryParse(attr.Value, NumberStyles.Float, nfi, out param.OffsetX);
                    //else if (attr.Name == "offset_y")
                    //    float.TryParse(attr.Value, NumberStyles.Float, nfi, out param.OffsetY);
                    //else if (attr.Name == "nrOfFrames")
                    //    int.TryParse(attr.Value, out param.NumberOfFrames);
                    if (attr.Name == "name")
                        name = attr.Value;
                    else if (attr.Name == "layer")
                        param.Layer = float.Parse(attr.Value, NumberStyles.Float, nfi);
                    else if (attr.Name == "draw_h")
                        param.DrawHeight = int.Parse(attr.Value);
                    else if (attr.Name == "draw_w")
                        param.DrawWidth = int.Parse(attr.Value);
                    else if (attr.Name == "frames_x")
                        param.FramesX = int.Parse(attr.Value);
                    else if (attr.Name == "frames_y")
                        param.FramesY = int.Parse(attr.Value);
                    else if (attr.Name == "frame_rate")
                        param.FrameRate = int.Parse(attr.Value);
                    else if (attr.Name == "loops")
                        param.Loops =int.Parse(attr.Value);
                    else if (attr.Name == "offset_x")
                        param.OffsetX = float.Parse(attr.Value, NumberStyles.Float, nfi);
                    else if (attr.Name == "offset_y")
                        param.OffsetY = float.Parse(attr.Value, NumberStyles.Float, nfi);
                    else if (attr.Name == "nrOfFrames")
                        param.NumberOfFrames = int.Parse(attr.Value);
				}

				if (name != "")
				{
					param.Texture = Graphics.GetTexture(name);
                    if (param.DrawHeight == 0 || param.DrawWidth == 0)
                    {
                        param.DrawHeight = param.Texture.Height;
                        param.DrawWidth = param.Texture.Width;
                    }
					s_Tags.Add(name);
					s_EffectParams.Add(param);
				}
			}
		}

		public static void Spawn(Vector2 position, string tag)
		{
			int i;
			int c = s_Tags.Count;
			for (i = 0; i < c; ++i)
			{
				if (s_Tags[i] == tag)
				{
					GameEffect effect = new GameEffect(s_EffectParams[i]);
					effect.Position = position;
					EntityManager.Spawn(effect);
				}
			}
		}

		public static void SpawnFollowing(Unit target, string tag, float life)
		{
			int i;
			int c = s_Tags.Count;
			for (i = 0; i < c; ++i)
			{
				if (s_Tags[i] == tag)
				{
					GameEffect effect = new GameEffect(s_EffectParams[i]);
					effect.SetFollow(target);
					EntityManager.Spawn(effect);
					if (life > 0)
						effect.SetLifeTime(life);
				}
			}
		}
	}
}
