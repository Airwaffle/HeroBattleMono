using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace HeroBattleArena.Game
{
    public enum MusicNames
    {

        Slugwar,
        RoadsideFragging,
        Charge,
        Burial,
        Ciak,

    }

    public enum SoundNames
    {
        InterfaceClickMove,
        InterfaceClickConfirm,
        InterfaceClickBack,
        InterfacePowerUpPickup,
        AllImpact,
        AllRespawn,

        Zombie,

        bossZombie,
        bossZombieDeath,
        bossZombieAttack,
        bossZombieSpawn,
        bossZombieDash,
        bossZombieLaser,

        ArthurAttack,
        ArthurCounterTeleport,
        ArthurCounterTeleportEnd,
        ArthurLaserBlade,
        ArthurLaserBladeProjectile,
        ArthurSilenceShine,
        ArthurDeath,

        AztecAttack,
        AztecGhostShield,
        AztecGhostShieldHit,
        AztecFlurry,
        AztecFlurrySwoosh,
        AztecFlurryLoop,
        AztecThrowSpear,
        AztecDeath,

        MageAttack,
        MageElectricCelerity,
        MageMicrobot,
        MageWhirlWind,
        MageDeath,

        SoldierAttack,
        SoldierFlashbang,
        SoldierLockOn,
        SoldierLockOnExplosion,
        SoldierStickyGrenadeBounce,
        SoldierStickyGrenadeStick,
        SoldierStickyGrenadeExplosion,
        SoldierDeath,

        ZombieAttack,
        ZombieDeath,
        ZombieSpawn,
        ZombieDrain,
        ZombieDrainBoost,
        ZombieDig,
    };

    public class SoundCenter
    {

        private AudioEngine m_AudioEngine;
        private SoundBank m_SoundBank;
        private WaveBank m_WaveBank;
        private WaveBank m_MusicBank;
        private SoundBank m_MusicSoundBank;

        private AudioCategory m_MusicSettings;
        private AudioCategory m_SoundSettings;

        private Random rand = new Random(2357);

        private float m_masterSoundVolume = 0.71f;
        private float masterMusicVolume = 0.71f;

        public float MasterMusicVolume { get { return masterMusicVolume; } set { masterMusicVolume = value; UpdateMusicVolume(); } }
        public float MasterSoundVolume { get { return m_masterSoundVolume; } set { m_masterSoundVolume = value; UpdateSoundVolume(); } }

        private List<SoundEffect> m_sounds = new List<SoundEffect>();
        private List<SoundEffect> m_music = new List<SoundEffect>();

        private static SoundCenter s_instance = null;

        private float m_GrenadeBounceDelay = 0;
        private float m_GrenadeBounceMaxDelay = 0;

        public SoundEffect backMusic;

        private void UpdateMusicVolume(){
            
            //if (masterMusicVolume < 0) masterMusicVolume = 0;
              //m_MusicSettings.SetVolume(masterMusicVolume);
        }

        private void UpdateSoundVolume(){
            //if (m_masterSoundVolume < 0) m_masterSoundVolume = 0;
             // m_SoundSettings.SetVolume(m_masterSoundVolume);
        }

        public SoundCenter()
        {
        }

        public static SoundCenter Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new SoundCenter();
                return s_instance;
            }
        }

        public void Initialize()
        {
#if XBOX
            m_GrenadeBounceMaxDelay = Configuration.GetValue("Soundbug");
#else
            m_GrenadeBounceMaxDelay = 0;
#endif
           
        }

        public void Load(ContentManager content)
        {
            m_AudioEngine = new AudioEngine(@"Content\\Audio\\Waves\\HBA_XACT.xgs");
            m_WaveBank = new WaveBank(m_AudioEngine, @"Content\\Audio\\Waves\\Wave Bank.xwb");
            m_MusicBank = new WaveBank(m_AudioEngine, @"Content\\Audio\\Waves\\Music Bank.xwb");
            m_SoundBank = new SoundBank(m_AudioEngine, @"Content\\Audio\\Waves\\Sound Bank.xsb");
            m_MusicSoundBank = new SoundBank(m_AudioEngine, @"Content\\Audio\\Waves\\Music Sound Bank.xsb");
            m_MusicSettings = m_AudioEngine.GetCategory("Music");
            m_SoundSettings = m_AudioEngine.GetCategory("Default");

            m_MusicSoundBank.PlayCue("MusicSlugwar");
        }

       
        private bool changeMusic = false;
        private MusicNames newMusic;
        private SoundEffectInstance currentMusicInstance = null;
        private int musicVolume = 1;
        private int musicChangeDirection = -1;

        public void PlayMusic(MusicNames music)
        {
            newMusic = music;

            Console.WriteLine("Music: " + newMusic + " played.");

            switch (newMusic)
            {

                //case MusicNames.Slugwar:
                //    m_MusicSoundBank.PlayCue("MusicSlugwar");
                //    break;
                //case MusicNames.RoadsideFragging:
                //    m_MusicSoundBank.PlayCue("MusicRoadsideFragging");
                //    break;
                //case MusicNames.Charge:
                //    m_MusicSoundBank.PlayCue("MusicCharge");
                //    break;
                //case MusicNames.Burial:
                //    m_MusicSoundBank.PlayCue("MusicBurial");
                //    break;
                //case MusicNames.Ciak:
                //    m_MusicSoundBank.PlayCue("MusicCiak");
                //break;
            }
        }

        public void Update(float delta)
        {
            if (m_GrenadeBounceDelay > 0)
            {
                m_GrenadeBounceDelay -= delta;
                if (m_GrenadeBounceDelay <= 0)
                    m_GrenadeBounceDelay = 0;
            }
        }

        public Cue Play(SoundNames sound)
        {
            SoundEffect.MasterVolume = m_masterSoundVolume;
            Console.WriteLine("Sound: " + sound + " played.");
            switch (sound)
            {
                // -- GENERAL SOUNDS -- //
                case SoundNames.InterfaceClickMove:
                    m_SoundBank.PlayCue("InterfaceClickMove");
                    break;
                case SoundNames.InterfaceClickConfirm:
                    m_SoundBank.PlayCue("InterfaceClickConfirm");
                    break;
                case SoundNames.InterfaceClickBack:
                    m_SoundBank.PlayCue("InterfaceClickBack");
                    break;
                case SoundNames.InterfacePowerUpPickup:
                    m_SoundBank.PlayCue("InterfacePowerUpPickup");
                    break;
                case SoundNames.AllImpact:
                    m_SoundBank.PlayCue("AllImpact");
                    break;
                case SoundNames.AllRespawn:
                    m_SoundBank.PlayCue("AllRespawn");
                    break;

                case SoundNames.Zombie:
                    m_SoundBank.PlayCue("Zombie");
                    break;

                // -- Arthur --//
                case SoundNames.ArthurAttack:
                    m_SoundBank.PlayCue("ArthurAttack");
                    break;
                case SoundNames.ArthurCounterTeleport:
                    m_SoundBank.PlayCue("ArthurCounterTeleport");
                    break;
                case SoundNames.ArthurCounterTeleportEnd:
                    m_SoundBank.PlayCue("ArthurCounterTeleportEnd");
                    break;
                case SoundNames.ArthurLaserBlade:
                    m_SoundBank.PlayCue("ArthurLaserBlade");
                    break;
                case SoundNames.ArthurLaserBladeProjectile:
                    {
                        Cue cue = m_SoundBank.GetCue("ArthurLaserBladeProjectile");
                        cue.Play();
                        return cue;
                    }  
                case SoundNames.ArthurSilenceShine:
                    m_SoundBank.PlayCue("ArthurSilenceShine");
                    break;
                case SoundNames.ArthurDeath:
                    m_SoundBank.PlayCue("ArthurDeath");
                    break;

                // -- Aztec -- //
                case SoundNames.AztecAttack:
                    m_SoundBank.PlayCue("AztecAttack");
                    break;
                case SoundNames.AztecGhostShield:
                    m_SoundBank.PlayCue("AztecGhostShield");
                    break;
                case SoundNames.AztecGhostShieldHit:
                    m_SoundBank.PlayCue("AztecGhostShield");
                    break;
                case SoundNames.AztecFlurry:
                    m_SoundBank.PlayCue("AztecFlurry");
                    break;
                case SoundNames.AztecFlurryLoop:
                    {
                        Cue cue = m_SoundBank.GetCue("AztecFlurryLoop");
                        cue.Play();
                        return cue;
                    }
                case SoundNames.AztecFlurrySwoosh:
                    m_SoundBank.PlayCue("AztecFlurrySwoosh");
                    break;
                case SoundNames.AztecThrowSpear:
                    m_SoundBank.PlayCue("AztecThrowSpear");
                    break;
                case SoundNames.AztecDeath:
                    m_SoundBank.PlayCue("AztecDeath");
                    break;

                // -- Mage -- //
                case SoundNames.MageAttack:
                    m_SoundBank.PlayCue("MageAttack");
                    break;
                case SoundNames.MageElectricCelerity:
                    m_SoundBank.PlayCue("MageElectricCelerity");
                    break;
                case SoundNames.MageMicrobot:
                    m_SoundBank.PlayCue("MageMicrobot");
                    break;
                case SoundNames.MageWhirlWind:
                    m_SoundBank.PlayCue("MageWhirlWind");
                    break;
                case SoundNames.MageDeath:
                    m_SoundBank.PlayCue("MageDeath");
                    break;

                // -- Soldier -- //
                case SoundNames.SoldierAttack:
                    m_SoundBank.PlayCue("SoldierAttack");
                    break;
                case SoundNames.SoldierFlashbang:
                    m_SoundBank.PlayCue("SoldierFlashBang");
                    break;
                case SoundNames.SoldierLockOn:
                    m_SoundBank.PlayCue("SoldierLockOn");
                    break;
                case SoundNames.SoldierLockOnExplosion:
                    m_SoundBank.PlayCue("SoldierLockOnExplosion");
                    break;
                case SoundNames.SoldierStickyGrenadeBounce:
                    if (m_GrenadeBounceDelay <= 0)
                    {
                        m_SoundBank.PlayCue("SoldierStickyGrenadeBounce");
                        m_GrenadeBounceDelay = m_GrenadeBounceMaxDelay;
                    }
                    break;
                case SoundNames.SoldierStickyGrenadeStick:
                    m_SoundBank.PlayCue("SoldierStickyGrenadeStick");
                    break;
                case SoundNames.SoldierStickyGrenadeExplosion:
                    m_SoundBank.PlayCue("SoldierStickyGrenadeExplosion");
                    break;
                case SoundNames.SoldierDeath:
                    m_SoundBank.PlayCue("SoldierDeath");
                    break;

                // Zombie Character
                case SoundNames.ZombieAttack:
                    m_SoundBank.PlayCue("ZombieAttack");
                    break;
                case SoundNames.ZombieDrain:
                    m_SoundBank.PlayCue("ZombieDrain");
                    break;
                case SoundNames.ZombieDrainBoost:
                    m_SoundBank.PlayCue("ZombieDrainBoost");
                    break;
                case SoundNames.ZombieDig:
                    m_SoundBank.PlayCue("ZombieDig");
                    break;
                case SoundNames.ZombieDeath:
                    m_SoundBank.PlayCue("ZombieDeath");
                    break;
                case SoundNames.ZombieSpawn:
                    m_SoundBank.PlayCue("ZombieSpawn");
                    break;

                // Zombie Boss
                case SoundNames.bossZombieAttack:
                    m_SoundBank.PlayCue("bossZombieAttack");
                    break;
                case SoundNames.bossZombie:
                    m_SoundBank.PlayCue("bossZombie");
                    break;
                case SoundNames.bossZombieSpawn:
                    m_SoundBank.PlayCue("bossZombieSpawn");
                    break;
                case SoundNames.bossZombieDeath:
                    m_SoundBank.PlayCue("bossZombieDeath");
                    break;
                case SoundNames.bossZombieDash:
                    m_SoundBank.PlayCue("bossZombieDash");
                    break;
                case SoundNames.bossZombieLaser:
                    m_SoundBank.PlayCue("bossZombieLaser");
                    break;

#if DEBUG
                default:
                    Console.WriteLine("Unimplemented sound being played: \"" + sound.ToString() + "\".");
                    break;
#endif
            }
            return null;
        }
    }
}
