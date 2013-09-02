using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    class TutorialScreen : Screen
    {
        private int m_Selected = 0;
        private float m_Pressed = 0;
        private float m_Delaytime = 1;
        private int m_ControllingPlayer = 0;

        private Hero m_Hero = null;
        private Vector2 m_HeroPosition = new Vector2(507, 260);

        private Texture2D[] m_GlowingButtons = new Texture2D[4];
        private bool[] m_PressedButtons = new bool[4];

        private Texture2D[] m_LeftRight = new Texture2D[2];
        private Texture2D[] m_Unselected = new Texture2D[5];
        private Texture2D[] m_Names = new Texture2D[5];
        private Texture2D m_AttackDescriptionPlate;
        private Texture2D m_Plate;
        private Texture2D m_AttackPlate;
        private Texture2D m_Controller;
        private Texture2D m_TutorialText;
        private Vector2 m_TutorialTextPosition = new Vector2(352, 48);

        private Vector2 m_ControllerPosition = new Vector2((1024 - 410) / 2, 420);
        private Vector2 m_PlatePosition = new Vector2(382, 154); 
        private float m_PlateOffset = 290;

        private int m_CurrentAttack = -1;
        Texture2D[,] m_AttackNames = new Texture2D[5, 3];
        Texture2D[,] m_AttackDescriptions = new Texture2D[5, 3];

        private Vector2 m_LeftThumbStickPos = new Vector2();
        private Vector2 m_RightThumbStickPos = new Vector2();
        private Texture2D m_LeftStick;
        private Texture2D m_RightStick;
        private Texture2D m_Unavailible;

        private float m_ChangeCharacterCounter = 0;

        private static float m_CantGrenadeCounter = 0;
        private static float m_CantGrenadeTime = Configuration.GetValue("Sticky_Grenade_Life");


        private Vector2 m_PlateInfoOffset = new Vector2(5, 95);

        private Plate[] m_Plates = new Plate[5];

        public static bool CanThrowGrenade(){
            if (m_CantGrenadeCounter <= 0)
            {
                m_CantGrenadeCounter = m_CantGrenadeTime;
                return true;
            }
            return false;       
        }

        public class Plate
        {
            public float m_Scale;
            public float m_WantedScale;
            public Vector2 m_Pos;
            public Vector2 m_WantedPos;
            public Plate(float scale, Vector2 pos){
                m_Scale = scale;
                m_WantedScale = scale;
                m_Pos = pos;
                m_WantedPos = pos;
            }
            public void Update(){
                m_Scale = m_Scale * (1 - 0.12f) + m_WantedScale * 0.12f;
                m_Pos = m_Pos * (1 - 0.12f) + m_WantedPos * 0.12f;
            }
        }

        public TutorialScreen(int controllingPlayer)
        {
            m_ControllingPlayer = controllingPlayer;
        }

        public TutorialScreen(int controllingPlayer, int playerCharacter)
        {
            m_ControllingPlayer = controllingPlayer;
            m_Selected = playerCharacter;
        }

        public override void Initialize()
        {
            EntityManager.Clear();
            base.Initialize();

            // First hero to create is Arthur
            SelectNewHero();

            // Initalize textures
            m_GlowingButtons[0] = Graphics.GetTexture("a_button_glow");
            m_GlowingButtons[1] = Graphics.GetTexture("x_button_glow");
            m_GlowingButtons[2] = Graphics.GetTexture("y_button_glow");
            m_GlowingButtons[3] = Graphics.GetTexture("b_button_glow");

            m_LeftRight[0] = Graphics.GetTexture("l_trigger");
            m_LeftRight[1] = Graphics.GetTexture("r_trigger");

            m_Names[0] = Graphics.GetTexture("arthur_name");
            m_Names[1] = Graphics.GetTexture("stryker_name");
            m_Names[2] = Graphics.GetTexture("quetzali_name");
            m_Names[3] = Graphics.GetTexture("nano_name");
            m_Names[4] = Graphics.GetTexture("zombie_name");

            m_Unselected[0] = Graphics.GetTexture("arthur_not_selected");
            m_Unselected[1] = Graphics.GetTexture("stryke_not_selected");
            m_Unselected[2] = Graphics.GetTexture("quetzali_not_selected");
            m_Unselected[3] = Graphics.GetTexture("nano_not_selected");
            m_Unselected[4] = Graphics.GetTexture("zombie_not_selected");

            m_Plate = Graphics.GetTexture("character_plate");
            m_AttackPlate = Graphics.GetTexture("attack_plate");
            m_Controller = Graphics.GetTexture("xbox_controller");
            m_TutorialText = Graphics.GetTexture("tutorial_text");

            m_AttackNames[0, 0] = Graphics.GetTexture("B_arthur_attack");
            m_AttackNames[0, 1] = Graphics.GetTexture("X_arthur_attack");
            m_AttackNames[0, 2] = Graphics.GetTexture("Y_arthur_attack");
            m_AttackNames[1, 0] = Graphics.GetTexture("B_stryker_attack");
            m_AttackNames[1, 1] = Graphics.GetTexture("X_stryker_attack");
            m_AttackNames[1, 2] = Graphics.GetTexture("Y_stryker_attack");
            m_AttackNames[2, 0] = Graphics.GetTexture("B_quetzali_attack");
            m_AttackNames[2, 1] = Graphics.GetTexture("X_quetzali_attack");
            m_AttackNames[2, 2] = Graphics.GetTexture("Y_quetzali_attack");
            m_AttackNames[3, 0] = Graphics.GetTexture("B_nano_attack");
            m_AttackNames[3, 1] = Graphics.GetTexture("X_nano_attack");
            m_AttackNames[3, 2] = Graphics.GetTexture("Y_nano_attack");
            m_AttackNames[4, 0] = Graphics.GetTexture("B_zombie_attack");
            m_AttackNames[4, 1] = Graphics.GetTexture("X_zombie_attack");
            m_AttackNames[4, 2] = Graphics.GetTexture("Y_zombie_attack");

            m_LeftStick = Graphics.GetTexture("controller_Lstick");
            m_RightStick = Graphics.GetTexture("controller_Rstick");
            m_Unavailible = Graphics.GetTexture("unavailible_tutorial");

            //Attack Plate Description//

            m_AttackDescriptionPlate = Graphics.GetTexture("attack_description_plate");


            //Attack Description Info//
            m_AttackDescriptions[0, 0] = Graphics.GetTexture("arthur_shield_description");
            m_AttackDescriptions[0, 1] = Graphics.GetTexture("arthur_silence_description");
            m_AttackDescriptions[0, 2] = Graphics.GetTexture("arthur_laser_description");

            m_AttackDescriptions[1, 0] = Graphics.GetTexture("stryker_grenade_description");
            m_AttackDescriptions[1, 1] = Graphics.GetTexture("stryker_lockon_description");
            m_AttackDescriptions[1, 2] = Graphics.GetTexture("stryker_flash_description");

            m_AttackDescriptions[2, 0] = Graphics.GetTexture("aztec_slow_description");
            m_AttackDescriptions[2, 1] = Graphics.GetTexture("aztec_shield_description");
            m_AttackDescriptions[2, 2] = Graphics.GetTexture("aztec_flurry_description");

            m_AttackDescriptions[3, 0] = Graphics.GetTexture("nano_shock_description");
            m_AttackDescriptions[3, 1] = Graphics.GetTexture("nano_electic_description");
            m_AttackDescriptions[3, 2] = Graphics.GetTexture("nano_microbots_description");

            m_AttackDescriptions[4, 0] = Graphics.GetTexture("zombie_vampyric_description");
            m_AttackDescriptions[4, 1] = Graphics.GetTexture("zombie_summon_description");
            m_AttackDescriptions[4, 2] = Graphics.GetTexture("zombie_dig_description");

                           
            int nr = m_Selected;
            m_Plates[nr++] = new Plate(1, m_PlatePosition);
            nr = nr % 5;
            m_Plates[nr++] = new Plate(0.75f, m_PlatePosition + new Vector2(m_PlateOffset, 0));
            nr = nr % 5;
            m_Plates[nr++] = new Plate(0, m_PlatePosition + new Vector2(m_PlateOffset + m_Plate.Width, 0));
            nr = nr % 5;
            m_Plates[nr++] = new Plate(0, m_PlatePosition + new Vector2(-m_PlateOffset, 0));
            nr = nr % 5;
            m_Plates[nr++] = new Plate(0.75f, m_PlatePosition + new Vector2(-m_PlateOffset * 0.75f, 0));
        }

        public override void Update(float delta)
        {

            if (m_CantGrenadeCounter > 0)
            {
                m_CantGrenadeCounter -= delta;
            }

            m_Delaytime -= delta;

            base.Update(delta);
            ScreenBounce.Update(delta);

            for (int i = 0; i < 5; i++){
                m_Plates[i].Update();
            }

			// Get how the sticks on the controller is bent
             m_LeftThumbStickPos = Input.GetPlayerState(m_ControllingPlayer).ThumbStickLeftPos();
             m_RightThumbStickPos = Input.GetPlayerState(m_ControllingPlayer).ThumbStickRightPos();

            // Removes stuff from the earlier specialattack if a new one is activated (Ex; Zombies/orbs/etc)
            for (int i = 0; i < 3; i++)
            {
                if (Input.GetPlayerState(m_ControllingPlayer).WasPressed((InputCommand)(i+(int)InputCommand.Special1)))
                {
                    m_Hero.DispellBuffs();
                    m_Hero.HeroState = HeroStates.IDLE;
                    List<IEntity> entities = EntityManager.Entities;
                    foreach (IEntity entity in entities)
                        if (!(entity is StickyGrenadeProjectile))
                            entity.Remove();
                    m_CurrentAttack = i;
                    m_Delaytime = 1;
                }
            }
            
            // Updates hero so he can preform attacks
            if (m_ChangeCharacterCounter <= 0)
                m_Hero.Update(delta);

            // Maximizes his mana and removes cooldowns
            m_Hero.MaximizeEverything(false); 

            // Updates everything else
            EntityManager.Update(delta);

            // Snaps the hero to a specific position
            m_Hero.Position = m_HeroPosition;

            // Ensures we cant spam the shoulder buttons too much
            if (m_Pressed > 0)
            {
                m_Pressed -= delta;
                if (m_Pressed <= 0)
                {
                    m_Pressed = 0;
                }
            }

            if (Input.GetPlayerState(m_ControllingPlayer).WasPressed(InputCommand.MenuSelect))
            {
                m_Delaytime = 10000;
            }
            if (m_ChangeCharacterCounter <= 0)
            {
                // Change the hero
                if (Input.GetPlayerState(m_ControllingPlayer).WasPressed(InputCommand.RightShoulder) || Input.GetPlayerState(m_ControllingPlayer).WasPressed(InputCommand.RightTrigger))
                {
                    if (!ScreenManager.IsTrial || m_Selected == 0)
                    {
                        m_Selected++;
                        m_Selected = m_Selected % 5;
                        m_Pressed = 0.1f;
                        SelectNewHero();
                        MovePlates(-1);
                    }
                }
                if (Input.GetPlayerState(m_ControllingPlayer).WasPressed(InputCommand.LeftShoulder) || Input.GetPlayerState(m_ControllingPlayer).WasPressed(InputCommand.LeftTrigger))
                {
                    if (!ScreenManager.IsTrial || m_Selected == 1)
                    {
                        m_Selected--;
                        m_Selected += 5;
                        m_Selected = m_Selected % 5;
                        m_Pressed = 0.1f;
                        SelectNewHero();
                        MovePlates(1);
                    }
                }
            }
            else
            {
                m_ChangeCharacterCounter -= delta;
            }

            // Enables zombie to create as many zombies as he likes
            if (m_Hero is ZombieHero)
            {
               (m_Hero as ZombieHero).EnptyMinionList();
            }


            for (int i = 0; i < 4; i++){
                if (Input.GetPlayerState(m_ControllingPlayer).IsPressed((InputCommand)(i+4)))
                    m_PressedButtons[i] = true;
                else
                    m_PressedButtons[i] = false;
            }
        }

        public void SelectNewHero()
        {
            // Removes everything belonging to the previous hero.
            List<IEntity> entities = EntityManager.Entities;
            foreach (IEntity entity in entities)
                entity.Remove();

            // Check which hero to create.
            if (m_Selected == 0)
            {
                m_Hero = new Arthur();
            } 
            else if (m_Selected == 1)
            {
                m_Hero = new Soldier();
            } 
            else if (m_Selected == 2)
            {
                m_Hero = new Aztec();
            } 
            else if (m_Selected == 3)
            {
                m_Hero = new Mage();
            }
            else if (m_Selected == 4)
            {
                m_Hero = new ZombieHero();
            }
            
            // Initialize new hero.
            m_Hero.Initialize();
            m_Hero.PlayerOwner = m_ControllingPlayer;
            m_Hero.HeroColor = 0;
            m_CurrentAttack = -1;
        }

        public void MovePlates(int dir)
        {
            float[] tempScale = new float[5];
            Vector2[] tempPos = new Vector2[5];

            for (int i = 0; i < 5; i++)
            {
                tempScale[i] = m_Plates[i].m_WantedScale;
                tempPos[i] = m_Plates[i].m_WantedPos;
            }
            for (int i = 0; i < 5; i++)
            {
                m_Plates[i].m_WantedScale = tempScale[(i + dir + m_Plates.Length) % m_Plates.Length];
                m_Plates[i].m_WantedPos = tempPos[(i + dir + m_Plates.Length) % m_Plates.Length];
            }
            m_ChangeCharacterCounter = 0.4f;
        }

        public override void Draw()
        {
            base.Draw();

            if (m_ChangeCharacterCounter <= 0)
            {
                m_Hero.Draw();
                Graphics.Draw(m_Names[m_Selected], new Vector2(502 - m_Names[m_Selected].Width / 2, 135) + ScreenBounce.Offset, null, 5, Color.White);
            }
            // Draw all 
            List<IEntity> entities = EntityManager.Entities;
            foreach (IEntity entity in entities)
                entity.Draw();  


            Graphics.Draw(m_Controller, m_ControllerPosition, null, 51, Color.White);
            Graphics.Draw(m_TutorialText, m_TutorialTextPosition, null, 5, Color.White);

            for (int i = 0; i < 2; i++)
                Graphics.Draw(m_LeftRight[i], new Vector2(250 + 450 * i, 52), null, 5, Color.White);
            for (int i = 0; i < 4; i++)
                if (m_PressedButtons[i] == true)
                    Graphics.Draw(m_GlowingButtons[i], m_ControllerPosition, null, 52, Color.White);
            // Draw Sticks
            Graphics.Draw(m_LeftStick, m_ControllerPosition + m_LeftThumbStickPos * 13, null, 52, Color.White);
            Graphics.Draw(m_RightStick, m_ControllerPosition + m_RightThumbStickPos * 13, null, 52, Color.White);

            for (int i = 0; i < m_Plates.Length; i++){
                Graphics.Draw(m_Plate, m_Plates[i].m_Pos, m_Plates[i].m_Scale, null, m_Plates[i].m_Scale/2, Color.White);
            }

            for (int i = 0; i < 5; i++)
            {
                
                    
                int cheating = 0;
                if (i == 3) cheating = 60;
                if (m_ChangeCharacterCounter > 0 || i != m_Selected){
                    Graphics.Draw(m_Unselected[i], m_Plates[i].m_Pos + new Vector2((m_Plate.Width - 74) / 2 * m_Plates[i].m_Scale, (m_Plate.Height - (65 + cheating)) / 2 * m_Plates[i].m_Scale), m_Plates[i].m_Scale, null, 1 + m_Plates[i].m_Scale, Color.White);
                    if (ScreenManager.IsTrial && i > 1)
                    {
                        Graphics.Draw(m_Unavailible, new Vector2(-25) + m_Plates[i].m_Pos + new Vector2((m_Plate.Width - 74) / 2 * m_Plates[i].m_Scale, (m_Plate.Height - (65 + cheating)) / 2 * m_Plates[i].m_Scale), m_Plates[i].m_Scale, null, 1 + m_Plates[i].m_Scale, Color.White);
                    }
                }
            }

            /*for (int i = -1; i <= 1; i+=2)
                Graphics.Draw(m_Unselected[(m_Selected + i + 5) % 5], m_HeroPosition + new Vector2(i * m_PlateOffset - 40, -70), null, 1.8f, Color.White);
            */

            if (m_Selected == 0)
            {

                if (m_CurrentAttack == 0 && m_Delaytime <0)
                {

                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(355, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                    
                }
                if (m_CurrentAttack == 1 && m_Delaytime < 0)
                {

                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(350, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                    
                }

                if (m_CurrentAttack == 2 && m_Delaytime < 0)
                {

                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(365, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                   
                }
            }

            if (m_Selected == 1)
            {

                if (m_CurrentAttack == 0 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(365, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(296, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }
                if (m_CurrentAttack == 1 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(410, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(295, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }

                if (m_CurrentAttack == 2 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(387, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(295, 236) + m_PlateInfoOffset, null, 52, Color.White);
                }
            }

            if (m_Selected == 2)
            {

                if (m_CurrentAttack == 0 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(367, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }
                if (m_CurrentAttack == 1 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(410, 270) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }

                if (m_CurrentAttack == 2 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(420, 270) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }
            }

            if (m_Selected == 3)
            {

                if (m_CurrentAttack == 0 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(385, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }
                if (m_CurrentAttack == 1 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(400, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }

                if (m_CurrentAttack == 2 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(385, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }
            }

            if (m_Selected == 4)
            {

                if (m_CurrentAttack == 0 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(415, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }
                if (m_CurrentAttack == 1 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(360, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }

                if (m_CurrentAttack == 2 && m_Delaytime < 0)
                {
                    Graphics.Draw(m_AttackDescriptionPlate, new Vector2(300, 250) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(398, 260) + m_PlateInfoOffset, null, 52, Color.White);
                    Graphics.Draw(m_AttackDescriptions[m_Selected, m_CurrentAttack], new Vector2(290, 235) + m_PlateInfoOffset, null, 52, Color.White);
                }
            }


            if (m_CurrentAttack != -1)
            {
                Graphics.Draw(m_AttackPlate, new Vector2(382, 340), null, 1, Color.White);
                Graphics.Draw(m_AttackNames[m_Selected, m_CurrentAttack], new Vector2(506 - m_AttackNames[m_Selected, m_CurrentAttack].Width / 2, 355), null, 2, Color.White);
            }
            
        }

        public override void Exit()
        {
            m_CantGrenadeCounter = 0;
            EntityManager.Clear();
            base.Exit();
        }
    }
}
