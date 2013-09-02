using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HeroBattleArena.Game.DataSerialization;
using HeroBattleArena.Game.GameObjects;

namespace HeroBattleArena.Game
{
	class Sprite
	{
		public Texture2D Texture;
		public Vector2 Position;
		public Rectangle? Source;
		public Color BlendColor;
		public float Scale;
	}

	class TextSprite : Sprite
	{
		public SpriteFont Font;
		public string Text;
	}

	class SpriteBSTNode
	{
		public List<Sprite> Sprites = new List<Sprite>();
		public SpriteBSTNode Left;
		public SpriteBSTNode Right;
		public float Layer;
	}

	class SpriteBSTTree
	{
		private SpriteBatch m_SpriteBatch;
		private SpriteBSTNode m_Root = null;
		private float m_CurrentLayer;

		public SpriteBSTTree(SpriteBatch sprite)
		{
			m_SpriteBatch = sprite;
		}

		public void Add(Sprite sprite, float layer)
		{
			m_CurrentLayer = layer;
			m_Root = _AddRec(m_Root, sprite);
		}

		private SpriteBSTNode _AddRec(SpriteBSTNode node, Sprite sprite)
		{
			if (node == null)
			{
				node = new SpriteBSTNode();
				node.Layer = m_CurrentLayer;
				node.Left = null;
				node.Right = null;
				node.Sprites.Add(sprite);
				return node;
			}

			if (m_CurrentLayer < node.Layer)
				node.Left = _AddRec(node.Left, sprite);
			else if (m_CurrentLayer > node.Layer)
				node.Right = _AddRec(node.Right, sprite);
			else
				node.Sprites.Add(sprite);
			return node;
		}

		public void Clear()
		{
			_ClearRec(m_Root);
			m_Root = null;
			//GC.Collect();
		}

		private void _ClearRec(SpriteBSTNode node)
		{
			if (node == null)
				return;
			_ClearRec(node.Left); node.Left = null;
			_ClearRec(node.Right); node.Right = null;
			node.Sprites = null;
			node = null;
		}

		public void Draw()
		{
			_DrawRec(m_Root);
		}

		private void _DrawRec(SpriteBSTNode node)
		{
			if (node == null)
				return;
			_DrawRec(node.Left);

			foreach (Sprite sprite in node.Sprites)
			{
				if (sprite is TextSprite)
				{
					TextSprite text = sprite as TextSprite;
					m_SpriteBatch.DrawString(
						text.Font, 
						text.Text,
                        text.Position + ScreenBounce.Offset, 
						text.BlendColor, 
						0, Vector2.Zero, text.Scale, 
						SpriteEffects.None, node.Layer);
				}
				else
				{
					if (sprite.Texture != null)
					{
						m_SpriteBatch.Draw(
							sprite.Texture,
							sprite.Position + ScreenBounce.Offset,
							sprite.Source,
							sprite.BlendColor,
							0.0f, Vector2.Zero,
							sprite.Scale,
							SpriteEffects.None, node.Layer);
					}
				}
			}
			_DrawRec(node.Right);
		}
	}

	public enum DebugAABBMode
	{
		Feet,
		Body,
	}

    public class Graphics
    {
        public const float MAX_LAYER = 128;

        private static GameContentReader s_ContentReader;

        private static ContentManager s_Content;
        private static SpriteBatch s_SpriteBatch;
        private static SpriteFont s_Font;
        private static HeroBattleArena s_Game;
		private static SpriteBSTTree s_Tree;
		private static bool s_bShowAABB = false;

        public static SpriteBatch SpriteBatch
        {
            get { return s_SpriteBatch; }
        }

        public static void Initialize(HeroBattleArena game, ContentManager content, SpriteBatch sprite)
        {
            s_Game = game;
            s_Content = content;
            s_SpriteBatch = sprite;

			s_Tree = new SpriteBSTTree(sprite);

            s_Font = s_Content.Load<SpriteFont>(@"Fonts\DefaultFont");
            s_ContentReader = new GameContentReader(@"Configuration\GameContent.xml");
        }

        public static void Quit()
        {
            s_Content = null;
            s_SpriteBatch = null;
            s_Font = null;
            s_ContentReader = null;
            s_Game = null;
        }

        public static void LoadNextLevel()
        {
            s_ContentReader.LoadNextLevel(s_Content);
        }

        public static Texture2D GetTexture(string tag)
        {
            return s_ContentReader.GetTexture(tag);            
        }

        public static Texture2D LoadTexture(string asset)
        {
			try
			{
				return s_Content.Load<Texture2D>(@"Textures\" + asset);
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to load asset \"" + asset + "\": " + e.Message);
			}
			return null;
        }

        public static void Begin()
        {
			s_Tree.Clear();
            RasterizerState rast = new RasterizerState();
            rast.CullMode = CullMode.None;
            rast.DepthBias = 0;
            //s_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rast);

            s_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            SamplerState sampler = new SamplerState();
            sampler.Filter = TextureFilter.Point;
            sampler.AddressU = TextureAddressMode.Clamp;
            sampler.AddressV = TextureAddressMode.Clamp;
            sampler.AddressW = TextureAddressMode.Clamp;
            s_Game.GraphicsDevice.SamplerStates[0] = sampler;

            //s_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);
			//s_Game.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
			//s_Game.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
        }

		public static void ToggleAABB() { s_bShowAABB = !s_bShowAABB; }
		public static void DrawAABB(AABB aabb, DebugAABBMode colormode)
		{
			if (!s_bShowAABB) return;

			Texture2D tex;
			switch(colormode) 
			{
				case DebugAABBMode.Feet:
					tex = GetTexture("debug_feet");
					break;
				default:
					tex = GetTexture("debug_boundingbox");
					break;
			}
			Draw(tex, 
				new Vector2(aabb.MinX, aabb.MinY), 
				new Rectangle(0, 0, (int)aabb.Width, (int)aabb.Height), 
				10, Color.White);
		}

        public static void End()
        {
			s_Tree.Draw();
            s_SpriteBatch.End();
        }

		public static Texture2D Snapshot()
		{
            RenderTarget2D renderTar = new RenderTarget2D(s_Game.GraphicsDevice,
                s_Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                s_Game.GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

			/*ResolveTexture2D resolveTex = new ResolveTexture2D(
				s_Game.GraphicsDevice,
				s_Game.GraphicsDevice.PresentationParameters.BackBufferWidth, 
				s_Game.GraphicsDevice.PresentationParameters.BackBufferHeight, 
				1, s_Game.GraphicsDevice.PresentationParameters.BackBufferFormat);*/

			Texture2D tex = new Texture2D(s_Game.GraphicsDevice, 1024, 768);
			Color[] colors = new Color[1024 * 768];

			//s_Game.GraphicsDevice.ResolveBackBuffer(resolveTex);
            //s_Game.GraphicsDevice.GetBackBufferData(colors);

            s_Game.GraphicsDevice.SetRenderTarget(renderTar);
            ScreenManager.GetInstance().GetScreen(ScreenManager.GetInstance().NumScreens - 2).Draw();

            
            s_Game.GraphicsDevice.SetRenderTarget(null);

            renderTar.GetData<Color>(colors);
            //foreach (Color c in colors) Console.WriteLine(c);
           

			//resolveTex.GetData<Color>(colors);

			for (int i = 0; i < colors.Length; ++i)
			{
				byte temp = (byte)((int)(colors[i].R*0.3f + colors[i].G*0.59f + colors[i].B*0.11f));
				colors[i].G = colors[i].B = colors[i].R;


                
                float tempVal = (float)colors[i].R * 0.9f;
                
                colors[i].R = (byte)tempVal;


                colors[i].G = (byte)tempVal;
                tempVal = (float)colors[i].G* 0.6f;
                
                colors[i].G = (byte)tempVal;

                tempVal = (float)colors[i].B * 0.3f;
                colors[i].B = (byte)tempVal;

                /*colors[i].B = 50;
                float tempVal = (float)colors[i].G * 1.5f;
                if (tempVal > 255)
                {
                    tempVal = 255;
                }
                colors[i].G = (byte)tempVal;
                tempVal = (float)colors[i].R * 1.2f;
                if (tempVal > 200)
                {
                    tempVal = 200;
                }
                colors[i].R = (byte)tempVal;*/
			}
			tex.SetData<Color>(colors);


			return tex;
		}

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? source, float layer, Color blendColor)
        {
			Sprite sprite = new Sprite();
			sprite.Texture = texture;
			sprite.Position = position;
			sprite.Source = source;
			sprite.BlendColor = blendColor;
			sprite.Scale = 1;
			s_Tree.Add(sprite, layer / MAX_LAYER);
            //s_SpriteBatch.Draw(texture, position, source, blendColor, 0, Vector2.Zero, 1, SpriteEffects.None, layer/MAX_LAYER);
        }

		public static void Draw(Texture2D texture, Vector2 position, float scale, Rectangle? source, float layer, Color blendColor)
		{
			Sprite sprite = new Sprite();
			sprite.Texture = texture;
			sprite.Position = position;
			sprite.Source = source;
			sprite.BlendColor = blendColor;
			sprite.Scale = scale;
			s_Tree.Add(sprite, layer / MAX_LAYER);
			//s_SpriteBatch.Draw(texture, position, source, blendColor, 0, Vector2.Zero, scale, SpriteEffects.None, layer / MAX_LAYER);
		}

        public static void DrawText(string text, float layer, Vector2 position, Color color)
        {
			TextSprite sprite = new TextSprite();
			sprite.Texture = null;
			sprite.Position = position;
			sprite.Scale = 1;
			sprite.BlendColor = color;
			sprite.Source = null;
			sprite.Text = text;
			sprite.Font = s_Font;
			s_Tree.Add(sprite, layer/MAX_LAYER);
            //s_SpriteBatch.DrawString(s_Font, text, position, color);
            //s_SpriteBatch.DrawString(s_Font, text, position, color, 0, Vector2.Zero, 1, SpriteEffects.None, layer / MAX_LAYER);
        }
    }
}
