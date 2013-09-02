using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HeroBattleArena.Game.GameObjects;

namespace HeroBattleArena.Game
{
    public class Map
    {
        #region Static Fields

        private static List<Map> s_Maps = new List<Map>();
        private static NumberFormatInfo s_FormatInfo = new NumberFormatInfo();

        #endregion

        #region Fields

        private string m_Path;
        private Texture2D m_Background;
        private List<Vector2> m_SpawnPoints = new List<Vector2>();

        private List<ObstacleParameters> m_ObstacleParams = new List<ObstacleParameters>();
        private List<string> m_ObstacleTags = new List<string>();

        #endregion

        #region Static Properties

        public static List<Map> Maps { get { return s_Maps; } }

        #endregion

        #region Properties

        public Texture2D BackgroundTexture { get { return m_Background; } }
        public List<Vector2> SpawnPoints { get { return m_SpawnPoints; } }

        #endregion

        #region Initialization

        private Map(string path)
        {
            m_Path = path;
        }

        public static void Initialize()
        {
            s_FormatInfo.NumberDecimalSeparator = ".";

            s_Maps.Add(new Map(@"Maps\CamelotSquare.xml"));
            s_Maps.Add(new Map(@"Maps\Checkpoint.xml"));
            s_Maps.Add(new Map(@"Maps\SacredChamber.xml"));
            s_Maps.Add(new Map(@"Maps\XClassCruiser.xml"));
        }

        public void Load()
        {
            // First clear old data.
            Clear();
            // Now load the new data.
            ProcessFile();
        }

        #endregion

        #region Methods

        private void Clear()
        {
            m_SpawnPoints.Clear();
            m_ObstacleParams.Clear();
            m_ObstacleTags.Clear();
        }

        private void ProcessFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(m_Path);

            XmlElement root = doc.DocumentElement;
            if (root.Name != "Map")
                throw new Exception("The root node must be \"Map\" for \"" + m_Path + "\".");

            // Get the background texture.
            m_Background = Graphics.LoadTexture(root.GetAttributeNode("background").Value);

            // Process all parent nodes containing prefabs.
            XmlNodeList prefabsElements = root.GetElementsByTagName("Prefabs");
            foreach (XmlElement prefabsElement in prefabsElements)
                ProcessPrefabs(prefabsElement);

            // Process all object nodes containing objects.
            XmlNodeList objectsElements = root.GetElementsByTagName("Objects");
            foreach (XmlElement objectsElement in objectsElements)
                ProcessObjects(objectsElement);

            // Process the spawn points.
            XmlNodeList spawnPointsElements = root.GetElementsByTagName("SpawnPoints");
            foreach (XmlElement spawnPointsElement in spawnPointsElements)
                ProcessSpawnPoints(spawnPointsElement);
        }

        #region Prefabs

        private void ProcessPrefabs(XmlElement prefabsElement)
        {
            // Process obstacles.
            XmlNodeList obstacleElements = prefabsElement.GetElementsByTagName("Obstacle");
            foreach (XmlElement obstacleElement in obstacleElements)
                ProcessObstacle(obstacleElement);

            // Other types of objects here... (Add a new method for processing it)
        }

        private void ProcessObstacle(XmlElement obstacleElement)
        {
            // Setup a new obstacle parameter for the obstacle being processed.
            ObstacleParameters parameters = new ObstacleParameters();

            // Get the obstacle tag.
            string tag = "";
            foreach (XmlAttribute obstacleAttribute in obstacleElement.Attributes)
            {
                if (obstacleAttribute.Name == "tag")
                    tag = obstacleAttribute.Value;
            }
            // If there was no tag attribute, ignore this obstacle.
            if (tag == "")
                return;

            // Process obstacle bounding boxes.
            XmlNodeList AABBElements = obstacleElement.GetElementsByTagName("AABB");
            foreach (XmlElement AABBElement in AABBElements)
            {
                AABB aabb = new AABB();
                foreach (XmlAttribute AABBAttribute in AABBElement.Attributes)
                {
                    //if (AABBAttribute.Name == "minX")
                    //    float.TryParse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo, out aabb.MinX);
                    //else if (AABBAttribute.Name == "minY")
                    //    float.TryParse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo, out aabb.MinY);
                    //else if (AABBAttribute.Name == "maxX")
                    //    float.TryParse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo, out aabb.MaxX);
                    //else if (AABBAttribute.Name == "maxY")
                    //    float.TryParse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo, out aabb.MaxY);
                    if (AABBAttribute.Name == "minX")
                        aabb.MinX = float.Parse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo);
                    else if (AABBAttribute.Name == "minY")
                        aabb.MinY = float.Parse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo);
                    else if (AABBAttribute.Name == "maxX")
                        aabb.MaxX = float.Parse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo);
                    else if (AABBAttribute.Name == "maxY")
                        aabb.MaxY = float.Parse(AABBAttribute.Value, NumberStyles.Float, s_FormatInfo);
                }
                // Adds the boundingbox the to creational parameters.
                parameters.BoundingBoxes.Add(aabb);
            }

            XmlNodeList textureElements = obstacleElement.GetElementsByTagName("Texture");
            foreach (XmlElement textureElement in textureElements)
            {
                // Try to load the texture.
                string texturePath = textureElement.InnerText;
                parameters.Texture = Graphics.LoadTexture(texturePath);

                // Process texture attributes.
                foreach (XmlAttribute textureAttribute in textureElement.Attributes)
                {
                    // Most of the parameters are read directly into the parameters object.
                    //if(textureAttribute.Name == "layer")
                    //    float.TryParse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo, out parameters.Layer);
                    //else if(textureAttribute.Name == "offsetX")
                    //    float.TryParse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo, out parameters.OffsetX);
                    //else if (textureAttribute.Name == "offsetY")
                    //    float.TryParse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo, out parameters.OffsetY);
                    //else if (textureAttribute.Name == "depth")
                    //    float.TryParse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo, out parameters.DepthOffset);
                    if (textureAttribute.Name == "layer")
                        parameters.Layer = float.Parse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo);
                    else if (textureAttribute.Name == "offsetX")
                        parameters.OffsetX = float.Parse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo);
                    else if (textureAttribute.Name == "offsetY")
                        parameters.OffsetY = float.Parse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo);
                    else if (textureAttribute.Name == "depth")
                        parameters.DepthOffset = float.Parse(textureAttribute.Value, NumberStyles.Float, s_FormatInfo);
                }

                // At the moment only one texture is allowed.
                break;
            }

            // Process animation cycles.
            XmlNodeList animationCyclesElements = obstacleElement.GetElementsByTagName("AnimationCycles");
            foreach (XmlElement animationCycleElement in animationCyclesElements)
            {
                // Create the animation parameters...
                AnimationManagerParameters animationsParams = new AnimationManagerParameters();

                // Get animation parameters...
                foreach (XmlAttribute animationCycleAttribute in animationCycleElement.Attributes)
                {
                    //if (animationCycleAttribute.Name == "frameRate")
                    //    int.TryParse(animationCycleAttribute.Value, out animationsParams.FrameRate);
                    //else if (animationCycleAttribute.Name == "frameWidth")
                    //    int.TryParse(animationCycleAttribute.Value, out animationsParams.FrameWidth);
                    //else if (animationCycleAttribute.Name == "frameHeight")
                    //    int.TryParse(animationCycleAttribute.Value, out animationsParams.FrameHeight);
                    //else if (animationCycleAttribute.Name == "framesX")
                    //    int.TryParse(animationCycleAttribute.Value, out animationsParams.FramesX);
                    //else if (animationCycleAttribute.Name == "framesY")
                    //    int.TryParse(animationCycleAttribute.Value, out animationsParams.FramesY);
                    if (animationCycleAttribute.Name == "frameRate")
                        animationsParams.FrameRate = int.Parse(animationCycleAttribute.Value);
                    else if (animationCycleAttribute.Name == "frameWidth")
                        animationsParams.FrameWidth = int.Parse(animationCycleAttribute.Value);
                    else if (animationCycleAttribute.Name == "frameHeight")
                        animationsParams.FrameHeight = int.Parse(animationCycleAttribute.Value);
                    else if (animationCycleAttribute.Name == "framesX")
                        animationsParams.FramesX = int.Parse(animationCycleAttribute.Value);
                    else if (animationCycleAttribute.Name == "framesY")
                        animationsParams.FramesY = int.Parse(animationCycleAttribute.Value);
                }

                // Load the animation cycle parameters.
                XmlNodeList animationCycleElements = animationCycleElement.GetElementsByTagName("Cycle");
                foreach (XmlElement cycleElement in animationCycleElements)
                {
                    // Create a new animation cycle parameters object.
                    AnimationCycleParameters cycleParams = new AnimationCycleParameters();

                    // Fill the parameters object.
                    foreach (XmlAttribute cycleAttribute in cycleElement)
                    {
                        //if (cycleAttribute.Name == "startX")
                        //    int.TryParse(cycleAttribute.Value, out cycleParams.StartX);
                        //else if (cycleAttribute.Name == "startY")
                        //    int.TryParse(cycleAttribute.Value, out cycleParams.StartY);
                        //else if (cycleAttribute.Name == "numFrames")
                        //    int.TryParse(cycleAttribute.Value, out cycleParams.NumFrames);
                        if (cycleAttribute.Name == "startX")
                            cycleParams.StartX = int.Parse(cycleAttribute.Value);
                        else if (cycleAttribute.Name == "startY")
                            cycleParams.StartY = int.Parse(cycleAttribute.Value);
                        else if (cycleAttribute.Name == "numFrames")
                            cycleParams.NumFrames = int.Parse(cycleAttribute.Value);
                    }

                    // Add the cycle parameters to the animation parameters.
                    animationsParams.Cycles.Add(cycleParams);
                }

                // Use the animations cycles parameters in the obstacle parameters.
                parameters.AnimationParameters = animationsParams;

                // Only supports one animation cycles node.
                break;
            }

            // Other type of obstacle nodes can be added here...

            // Save the obstacle parameter object and it's tag to be acessed later.
            m_ObstacleTags.Add(tag);
            m_ObstacleParams.Add(parameters);
        }

        #endregion

        private void ProcessObjects(XmlElement objectsElement)
        {
            // Iterate over all obstacle elements.
            XmlNodeList obstacleElements = objectsElement.GetElementsByTagName("Obstacle");
            foreach (XmlElement obstacleElement in obstacleElements)
            {
                string tag = "";
                float x = 0;
                float y = 0;

                // Get the attribute values.
                foreach (XmlAttribute obstacleAttribute in obstacleElement.Attributes)
                {
                    //if (obstacleAttribute.Name == "tag")
                    //    tag = obstacleAttribute.Value;
                    //else if (obstacleAttribute.Name == "x")
                    //    float.TryParse(obstacleAttribute.Value, NumberStyles.Float, s_FormatInfo, out x);
                    //else if (obstacleAttribute.Name == "y")
                    //    float.TryParse(obstacleAttribute.Value, NumberStyles.Float, s_FormatInfo, out y);
                    if (obstacleAttribute.Name == "tag")
                        tag = obstacleAttribute.Value;
                    else if (obstacleAttribute.Name == "x")
                        x = float.Parse(obstacleAttribute.Value, NumberStyles.Float, s_FormatInfo);
                    else if (obstacleAttribute.Name == "y")
                        y = float.Parse(obstacleAttribute.Value, NumberStyles.Float, s_FormatInfo);
                }

                // Ignore the obstacle if tag is not set.
                if (tag == "")
                    continue;

                ObstacleParameters obstacleParameters = null;

                // Try to find the objactle tag...
                for (int i = 0; i < m_ObstacleTags.Count; ++i)
                {
                    if (m_ObstacleTags[i] == tag)
                    {
                        obstacleParameters = m_ObstacleParams[i];
                        break;
                    }
                }

                if (obstacleParameters != null)
                {
                    // Spawn the obstacle to world.
                    Obstacle obstacle = new Obstacle(obstacleParameters);
                    obstacle.Position = new Vector2(x, y);
                    EntityManager.Spawn(obstacle);
                }
                else
                {
                    // We didn't find the tag, output a warning to console.
                    Console.WriteLine("Tag \"" + tag + "\" was not found. Make sure the tag is specified in a prefabs section.");
                }
            }

            // Add a new type here if needed...
        }

        private void ProcessSpawnPoints(XmlElement parent)
        {
            XmlNodeList spawnPoints = parent.GetElementsByTagName("SpawnPoint");
            foreach (XmlElement spawnPoint in spawnPoints)
            {
                Vector2 position = Vector2.Zero;
                foreach (XmlAttribute attr in spawnPoint.Attributes)
                {
                    //if (attr.Name.ToLower() == "x")
                    //    float.TryParse(attr.Value, NumberStyles.Float, s_FormatInfo, out position.X);
                    //else if(attr.Name.ToLower() == "y")
                    //    float.TryParse(attr.Value, NumberStyles.Float, s_FormatInfo, out position.Y);
                    if (attr.Name.ToLower() == "x")
                        position.X = float.Parse(attr.Value, NumberStyles.Float, s_FormatInfo);
                    else if (attr.Name.ToLower() == "y")
                        position.Y = float.Parse(attr.Value, NumberStyles.Float, s_FormatInfo);
                }
                m_SpawnPoints.Add(position);
            }
        }

        #endregion
    }
}
