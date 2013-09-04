//#define USE_SHADER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace HeroBattleArena.Game
{
    // Singleton.
    public class GUI
    {
        private const int PORTRAIT_WIDTH = 94;
        private const int PORTRAIT_HEIGHT = 94;
        private const int COOLDOWN_WIDTH = 40;
        private const int COOLDOWN_HEIGHT = 40;

        private static GUI s_instance = null;
#if USE_SHADER
		private Effect m_statusEffect;
		private Effect m_cooldownEffect;
#else
        private Texture2D m_TexCooldownSheet;
#endif
        private GraphicsDevice m_device;
        private SpriteBatch m_sprite;
        private RenderTarget2D[] m_portraitTargets = new RenderTarget2D[4];
        private RenderTarget2D[] m_abilityTargets = new RenderTarget2D[12];
        private bool m_bActive = false;

        public bool Active { get { return m_bActive; } set { m_bActive = value; } }

        private Texture2D m_TexScoreScreen;

        private Texture2D[] m_TexPortrait = new Texture2D[4];
        private Texture2D[] m_TexAbilities = new Texture2D[12];
        private Texture2D m_TexOverlay;
        private Texture2D m_TexBars;
        private Texture2D m_TexManaMask;
        private Texture2D m_TexHealthMask;
        private Texture2D m_TexAbilityMask;
        private Texture2D[] m_TexAbilityOverlay = new Texture2D[3];

        private Texture2D m_TexLives;
        private Texture2D m_TexDeaths;
        private Texture2D m_TexZombies;
        private Texture2D m_TexBlue;
        private Texture2D m_TexRed;

        private Vector2[] m_PortraitPositions = new Vector2[4];
        private Vector2[] m_AbilityPositions = new Vector2[12];

        private Vector2[] m_TextOffset = new Vector2[4];
        private Vector2[] m_SingleKillPos = new Vector2[4];

        private GUI() { }

        public void Load(ContentManager content)
        {
            m_sprite = Graphics.SpriteBatch;
            m_device = m_sprite.GraphicsDevice;

            // Setup render targets...
            for (int i = 0; i < 4; ++i)
            {

                m_portraitTargets[i] = new RenderTarget2D(
                    m_device,
                    PORTRAIT_WIDTH, PORTRAIT_HEIGHT);

                /* m_portraitTargets[i] = new RenderTarget2D(
                     m_device,
                     PORTRAIT_WIDTH, PORTRAIT_HEIGHT, 1,
                     m_device.PresentationParameters.BackBufferFormat);
                 */

                for (int k = 0; k < 3; ++k)
                    m_abilityTargets[i * 3 + k] = new RenderTarget2D(
                        m_device,
                        COOLDOWN_WIDTH, COOLDOWN_HEIGHT);
                /*m_abilityTargets[i * 3 + k] = new RenderTarget2D(
                    m_device,
                    COOLDOWN_WIDTH, COOLDOWN_HEIGHT, 1,
                    m_device.PresentationParameters.BackBufferFormat);
                 * */
            }

#if USE_SHADER
            m_statusEffect = content.Load<Effect>(@"Shaders\status_effect");
            m_cooldownEffect = content.Load<Effect>(@"Shaders\cooldown_effect");
#else
            m_TexCooldownSheet = content.Load<Texture2D>(@"Textures\GUI\cooldown_spritesheet");
#endif
            LoadTextures(content);
            InitializePositions();
        }

        public static GUI Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new GUI();
                return s_instance;
            }
        }

        private void LoadTextures(ContentManager content)
        {
            m_TexScoreScreen = content.Load<Texture2D>(@"Textures\GUI\scoreboard");

            m_TexBars = content.Load<Texture2D>(@"Textures\GUI\GUI_bars");
            m_TexOverlay = content.Load<Texture2D>(@"Textures\GUI\GUI_overlay");
            m_TexManaMask = content.Load<Texture2D>(@"Textures\GUI\manaMask");
            m_TexHealthMask = content.Load<Texture2D>(@"Textures\GUI\healthMask");
            m_TexAbilityMask = content.Load<Texture2D>(@"Textures\GUI\ability_cooldown_mask");
            m_TexAbilityOverlay[1] = content.Load<Texture2D>(@"Textures\GUI\actionbar_blue");
            m_TexAbilityOverlay[2] = content.Load<Texture2D>(@"Textures\GUI\actionbar_yellow");
            m_TexAbilityOverlay[0] = content.Load<Texture2D>(@"Textures\GUI\actionbar_red");

            m_TexLives = content.Load<Texture2D>(@"Textures\GUI\Life_count");
            m_TexDeaths = content.Load<Texture2D>(@"Textures\GUI\Death_count");
            m_TexZombies = content.Load<Texture2D>(@"Textures\GUI\zombiecount");
            m_TexBlue = content.Load<Texture2D>(@"Textures\Menus\CharacterSelection\BlueTeam");
            m_TexRed = content.Load<Texture2D>(@"Textures\Menus\CharacterSelection\RedTeam");
        }

        private void InitializePositions()
        {
            m_PortraitPositions[0].X = 0;
            m_PortraitPositions[0].Y = 0;
            m_PortraitPositions[1].X = 1024 - m_TexOverlay.Width;
            m_PortraitPositions[1].Y = 0;
            m_PortraitPositions[2].X = 0;
            m_PortraitPositions[2].Y = 768 - m_TexOverlay.Height;
            m_PortraitPositions[3].X = 1024 - m_TexOverlay.Width;
            m_PortraitPositions[3].Y = 768 - m_TexOverlay.Height;

            int[] offX = { 100, 155, 210 };
            int sw = 1024, sh = 768;
            int w = 52, h = 52;
            int offY = 5;
            // Player 1
            m_AbilityPositions[0].X = offX[0];
            m_AbilityPositions[0].Y = offY;
            m_AbilityPositions[1].X = offX[1];
            m_AbilityPositions[1].Y = offY;
            m_AbilityPositions[2].X = offX[2];
            m_AbilityPositions[2].Y = offY;
            // Player 2
            m_AbilityPositions[3].X = sw - w - offX[2];
            m_AbilityPositions[3].Y = offY;
            m_AbilityPositions[4].X = sw - w - offX[1];
            m_AbilityPositions[4].Y = offY;
            m_AbilityPositions[5].X = sw - w - offX[0];
            m_AbilityPositions[5].Y = offY;
            // Player 3
            m_AbilityPositions[6].X = offX[0];
            m_AbilityPositions[6].Y = sh - h - offY;
            m_AbilityPositions[7].X = offX[1];
            m_AbilityPositions[7].Y = sh - h - offY;
            m_AbilityPositions[8].X = offX[2];
            m_AbilityPositions[8].Y = sh - h - offY;
            // Player 4
            m_AbilityPositions[9].X = sw - w - offX[2];
            m_AbilityPositions[9].Y = sh - h - offY;
            m_AbilityPositions[10].X = sw - w - offX[1];
            m_AbilityPositions[10].Y = sh - h - offY;
            m_AbilityPositions[11].X = sw - w - offX[0];
            m_AbilityPositions[11].Y = sh - h - offY;

            int scoreOffX = 5;
            int scoreOffY = 100;
            // Score positions.
            m_SingleKillPos[0].X = (float)scoreOffX;
            m_SingleKillPos[0].Y = (float)scoreOffY;
            m_SingleKillPos[1].X = (float)sw - (float)scoreOffX - 30;
            m_SingleKillPos[1].Y = (float)scoreOffY;
            m_SingleKillPos[2].X = (float)scoreOffX;
            m_SingleKillPos[2].Y = (float)sh - (float)scoreOffY - 60;
            m_SingleKillPos[3].X = (float)sw - (float)scoreOffX - 30;
            m_SingleKillPos[3].Y = (float)sh - (float)scoreOffY - 60;

            m_TextOffset[0].X = 32;
            m_TextOffset[0].Y = 4;
            m_TextOffset[1].X = -20;
            m_TextOffset[1].Y = 4;
            m_TextOffset[2].X = 32;
            m_TextOffset[2].Y = 4;
            m_TextOffset[3].X = -20;
            m_TextOffset[3].Y = 4;
        }

        public void Update(float delta)
        {
        }

        /// <summary>
        /// Called directly from the main loop,
        /// this utilizes pixelshader so rendertargets
        /// are needed.
        /// </summary>
        public void PreDraw()
        {
#if USE_SHADER
            List<GameObjects.Hero> heroes = GameObjects.EntityManager.Heroes;
            for (int i = 0; i < 4 && i < heroes.Count; ++i)
            {
                m_device.SetRenderTarget(m_portraitTargets[i]);
                //m_device.SetRenderTarget(0, m_portraitTargets[i]);

                m_device.Clear(Color.Transparent);
                //m_device.Clear(Color.TransparentBlack);

                //m_sprite.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.None);
                //m_sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                m_sprite.Begin(0, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, m_statusEffect);
                
                m_sprite.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                //m_statusEffect.Begin();

                m_statusEffect.Parameters["ManaMask"].SetValue(m_TexManaMask);
                m_statusEffect.Parameters["HealthMask"].SetValue(m_TexHealthMask);
                m_statusEffect.Parameters["ManaPercent"].SetValue(heroes[i].ManaFactor);
                m_statusEffect.Parameters["HealthPercent"].SetValue(heroes[i].HealthFactor);

                //m_statusEffect.CurrentTechnique.Passes[0].Begin();
                m_statusEffect.CurrentTechnique.Passes[0].Apply();

                m_sprite.Draw(m_TexBars, Vector2.Zero, Color.White);

                //m_statusEffect.CurrentTechnique.Passes[0].End();
                //m_statusEffect.End();
                m_sprite.End();

                List<GameObjects.Ability> abilities = heroes[i].Abilities;
                for (int k = 0; k < 3 && k < abilities.Count; ++k)
                {
                    m_device.SetRenderTarget(m_abilityTargets[i * 3 + k]);
                    //m_device.SetRenderTarget(0, m_abilityTargets[i * 3 + k]);

                    //m_device.Clear(Color.TransparentBlack);
                    m_device.Clear(Color.Transparent);

                    //m_sprite.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                    m_sprite.Begin(0, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, m_cooldownEffect);
                    m_sprite.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                    //m_sprite.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                    //m_cooldownEffect.Begin();
                    m_cooldownEffect.Parameters["CooldownMask"].SetValue(m_TexAbilityMask);
                    m_cooldownEffect.Parameters["Cooldown"].SetValue(abilities[k].Cooldown);

                    //m_cooldownEffect.CurrentTechnique.Passes[0].Begin();
                    m_statusEffect.CurrentTechnique.Passes[0].Apply();

                    m_sprite.Draw(abilities[k].Icon, Vector2.Zero, Color.White);

                    //m_cooldownEffect.CurrentTechnique.Passes[0].End();
                    //m_cooldownEffect.End();
                    m_sprite.End();

                    //m_device.SetRenderTarget(0, null);
                    m_device.SetRenderTarget(null);

                    //m_TexAbilities[i * 3 + k] = m_abilityTargets[i * 3 + k].GetTexture();
                    m_TexAbilities[i * 3 + k] = m_abilityTargets[i * 3 + k];
                }

                //m_device.SetRenderTarget(0, null);
                m_device.SetRenderTarget(null);

                //m_TexPortrait[i] = m_portraitTargets[i].GetTexture();
                m_TexPortrait[i] = m_portraitTargets[i];
            }
#endif
        }

        /// <summary>
        /// Draw the gui to the screen,
        /// the spritebatch should be started
        /// prior to use (Begin())
        /// </summary>
        public void Draw()
        {
            if (!GameMode.Instance.GameOver)
                DrawHeroInterface();
        }

        private void DrawHeroInterface()
        {
            List<GameObjects.Hero> heroes = GameObjects.EntityManager.Heroes;
            for (int i = 0; i < heroes.Count && i < 4; ++i)
            {
                // Draw hp, mana and overlay GUI...
                Graphics.Draw(m_TexPortrait[i],
                    m_PortraitPositions[i],
                    null, 30, Color.White);
                // Draw portraits
                Graphics.Draw(heroes[i].Portrait,
                    m_PortraitPositions[i], null,
                    31, Color.White);

                // Draw abilities
                List<GameObjects.Ability> abilities = heroes[i].Abilities;
                for (int k = 0; k < 3 && k < abilities.Count; ++k)
                {
#if !USE_SHADER
                    Graphics.Draw(abilities[k].Icon, m_AbilityPositions[i * 3 + k], null, 28, Color.White);
                    int image = (int)(abilities[k].Cooldown * 9);
                    Graphics.Draw(m_TexCooldownSheet, m_AbilityPositions[i * 3 + k] + new Vector2(-2, -6), new Rectangle(image * 50, 0, 50, 52), 28, Color.White);
#endif
                    Graphics.Draw(m_TexAbilities[i * 3 + k], m_AbilityPositions[i * 3 + k],
                        null, 29, Color.White);
                    Graphics.Draw(m_TexAbilityOverlay[k],
                        m_AbilityPositions[i * 3 + k] - new Vector2(4, 6),
                        null, 30, Color.White);
                }
            }

            GameMode mode = GameMode.Instance;
            if (mode is GM_FFA)
            {
                GM_FFA FFAGame = mode as GM_FFA;
                if (FFAGame.TeamGame)
                {
                    List<TeamInfo> infos = FFAGame.TeamInfos;

                    if (FFAGame.LifeCount > 0)
                    {
                        for (int i = 1; i < infos.Count; ++i)
                        {
                            Texture2D teamTex = m_TexBlue;
                            if (i == 2)
                            {
                                teamTex = m_TexRed;
                            }
                            Graphics.Draw(teamTex, new Vector2(1024 / 2 - 45, (i - 1) * (768 - 125) + 30 - 40), 0.5f, null, 100, Color.White);

                            Graphics.Draw(m_TexDeaths, new Vector2(1024 / 2 - 200 + 100, 60 + (i - 1) * (768 - 120)), null, 100, Color.White);
                            Graphics.Draw(m_TexDeaths, new Vector2(1024 / 2 - 195 + 100, 60 + (i - 1) * (768 - 120) + 3), null, 99, Color.White);
                            Graphics.DrawText("" + (infos[i].Score), 50, new Vector2(1024 / 2 - 150 + 100, 70 + (i - 1) * (768 - 125)), Color.White);

                            Graphics.Draw(m_TexLives, new Vector2(1024 / 2 - 60 + 100, 60 + (i - 1) * (768 - 120)), null, 100, Color.White);
                            Graphics.Draw(m_TexLives, new Vector2(1024 / 2 - 55 + 100, 60 + (i - 1) * (768 - 120) + 3), null, 99, Color.White);
                            Graphics.DrawText("" + (FFAGame.LifeCount - infos[i].Deaths), 50, new Vector2(1024 / 2 - 15 + 100, 70 + (i - 1) * (768 - 125)), Color.White);

                        }
                    }
                    else
                    {
                        for (int i = 1; i < infos.Count; ++i)
                        {
                            Graphics.DrawText("Team " + i + " Kills: " + infos[i].Score,
                                50, new Vector2(30, 20 + i * 24), Color.White);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < heroes.Count && i < 4; ++i)
                    {
                        PlayerInfo info = FFAGame.GetPlayerInfo(heroes[i]);

                        /*Graphics.DrawText(
                            info.HeroKills.ToString(), 10,
                            m_SingleKillPos[i] + m_TextOffset[i],
                            Color.White);*/
                        Graphics.Draw(m_TexLives, m_SingleKillPos[i] + new Vector2(0, 30), null, 100, Color.White);

                        string stat = "";
                        if (FFAGame.LifeCount > 0)
                        {
                            stat = (FFAGame.LifeCount - info.Deaths).ToString();
                        }
                        else
                        {
                            stat = info.Deaths.ToString();
                        }

                        Graphics.DrawText(stat, 50, m_SingleKillPos[i] + new Vector2(0, 30) + m_TextOffset[i], Color.White);

                        if (GameMode.Instance is GM_Zombie)
                        {
                            Graphics.Draw(m_TexZombies, m_SingleKillPos[i], null, 100, Color.White);
                            Graphics.DrawText(info.OtherKills.ToString(), 50, m_SingleKillPos[i] + new Vector2(0, 0) + m_TextOffset[i], Color.White);
                        }
                        else
                        {
                            Graphics.Draw(m_TexDeaths, m_SingleKillPos[i], null, 100, Color.White);
                            Graphics.DrawText(info.HeroKills.ToString(), 50, m_SingleKillPos[i] + new Vector2(0, 0) + m_TextOffset[i], Color.White);

                        }
                    }
                }
            }
        }
    }
}
