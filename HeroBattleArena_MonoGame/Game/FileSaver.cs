using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
//using System.Xml.Linq;
using HeroBattleArena.Game.GameObjects;

namespace HeroBattleArena.Game
{

    public class Settings
    {
        public float SoundVolume = 0.7f;
        public float MusicVolume = 0.7f;
        public bool ZombieUnlocked = false;
    }

    class FileSaver
    {
        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,

            ReadyToOpenStorageContainer,    // once we have a storage device start here
            OpeningStorageContainer,
            ReadyToSave
        }

        static private SavingState savingState = SavingState.NotSaving;
        static private string filename = "savegame.sav";
        static private string filenameSettings = "settings.sav";
        static private StorageContainer storageContainer;
        static private StorageDevice storageDevice;
        static private ScoreComponent scoreComp;
        static private PlayerIndex playerIndex = PlayerIndex.One;
        static private IAsyncResult asyncResult;
        static private bool isSaving = true;

        public static void Initialize()
        {
            scoreComp = new ScoreComponent("hej", 5, 5, "012", 5);
            //Components.Add(new GamerServicesComponent(this));
        }

        public static void Update()
        {
            switch (savingState)
            {
                case SavingState.ReadyToSelectStorageDevice:
#if XBOX
                    if (!Guide.IsVisible)
#endif
                    {
                        asyncResult = StorageDevice.BeginShowSelector(playerIndex, null, null);
                        savingState = SavingState.SelectingStorageDevice;
                    }
                    break;
                case SavingState.SelectingStorageDevice:
                    if (asyncResult.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(asyncResult);
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    break;

                case SavingState.ReadyToOpenStorageContainer:
                    if (storageDevice == null || !storageDevice.IsConnected)
                    {
                        savingState = SavingState.ReadyToSelectStorageDevice;
                    }
                    else
                    {
                        asyncResult = storageDevice.BeginOpenContainer("Game1StorageContainer", null, null);
                        savingState = SavingState.OpeningStorageContainer;
                    }
                    break;

                case SavingState.OpeningStorageContainer:
                    if (asyncResult.IsCompleted)
                    {
                        storageContainer = storageDevice.EndOpenContainer(asyncResult);
                        savingState = SavingState.ReadyToSave;
                    }
                    break;

                case SavingState.ReadyToSave:
                    if (storageContainer == null)
                    {
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    else
                    {
                        try
                        {
                            if (isSaving)
                            {
                                DeleteExisting();
                                Save();
                            }
                            else
                            {
                                Load();
                            }
                        }
                        catch (IOException e)
                        {
                            // Replace with in game dialog notifying user of error
                            Debug.WriteLine(e.Message);
                        }
                        finally
                        {
                            storageContainer.Dispose();
                            storageContainer = null;
                            savingState = SavingState.NotSaving;
                        }
                    }
                    break;
            }
        }

        public static void StartSaveData()
        {
            if (savingState == SavingState.NotSaving)
            {
                savingState = SavingState.ReadyToOpenStorageContainer;
            }
            isSaving = true;
        }

        public static void StartLoadData()
        {
            if (savingState == SavingState.NotSaving)
            {
                savingState = SavingState.ReadyToOpenStorageContainer;
            }
            isSaving = false;
            /*
            StorageDevice device = StorageDevice.EndShowSelector(result);
            IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(r);
            result.AsyncWaitHandle.Close();
            if (container.FileExists(filename))
            {
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                SaveGame SaveData = (SaveGame)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();
                //Update the game based on the save game file
                gamePlayer.Position = SaveData.PlayerPosition;
            }
             */
        }

        private static void Save()
        {
            using (Stream stream = storageContainer.CreateFile(filename))
            {
                /*
                XmlSerializer serializer = new XmlSerializer(typeof(ScoreComponent));

                ScoreList[] lists =  HighScoreList.m_Lists;
                for (int i = 0; i < lists.Length; i++)
                {
                    ScoreComponent[] comps = lists[i].Components;
                    for (int j = 0; j < comps.Length; j++)
                    {
                        serializer.Serialize(stream, comps[j]);
                    }
                }
                 */
                XmlSerializer serializer = new XmlSerializer(typeof(ScoreList[]));
                serializer.Serialize(stream, HighScoreList.m_Lists);

                /*
                serializer = new XmlSerializer(typeof(float));
                serializer.Serialize(stream, EntityManager.GetUnlockedChar(0) ? 1.0f : 0.0f);
                serializer.Serialize(stream, SoundCenter.Instance.MasterMusicVolume);
                serializer.Serialize(stream, SoundCenter.Instance.MasterSoundVolume);
                 */
                
            }

            Settings settings = new Settings();
            settings.MusicVolume = SoundCenter.Instance.MasterMusicVolume;
            settings.SoundVolume = SoundCenter.Instance.MasterSoundVolume;
            settings.ZombieUnlocked = EntityManager.GetUnlockedChar(0);
            using (Stream stream = storageContainer.CreateFile(filenameSettings))
            {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(stream, settings);               
            }
        }

        private static void Load()
        {
            Settings settings = new Settings();
            if (storageContainer.FileExists(filenameSettings))
            {
                using (Stream stream = storageContainer.OpenFile(filenameSettings, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    settings = (Settings)serializer.Deserialize(stream);
                }
            }
            SoundCenter.Instance.MasterMusicVolume = settings.MusicVolume;
            SoundCenter.Instance.MasterSoundVolume = settings.SoundVolume;
            if (settings.ZombieUnlocked)
                EntityManager.UnlockChar(0);

            if (storageContainer.FileExists(filename))
            {
                using (Stream stream = storageContainer.OpenFile(filename, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ScoreList[]));
                    HighScoreList.m_Lists = (ScoreList[])serializer.Deserialize(stream);
                }
            }
        }

        private static void DeleteExisting()
        {
            if (storageContainer.FileExists(filename))
                storageContainer.DeleteFile(filename);

            if (storageContainer.FileExists(filenameSettings))
                storageContainer.DeleteFile(filenameSettings);
        }
    }
}
