using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

using Spawner2.UserMap;

namespace Spawner2.UserGenerator
{
    [RequireComponent(typeof(Map))]
    class Generator : MonoBehaviour
    {
        AnimationCurve smoother = new AnimationCurve(new Keyframe(-0.2f, 3f), new Keyframe(1.2f, 0.8f));
        AnimationCurve lacunarity = new AnimationCurve(new Keyframe(-0.2f, 2f), new Keyframe(1.2f, 2f));
        AnimationCurve persistance = new AnimationCurve(new Keyframe(-0.2f, 0.45f), new Keyframe(1.2f, 0.55f));
        AnimationCurve amplitude = new AnimationCurve(new Keyframe(-0.2f, 0.2f), new Keyframe(1.2f, 0.6f));
        AnimationCurve eAmplitude = new AnimationCurve(new Keyframe(-0.2f, 0.2f), new Keyframe(1.2f, 0.8f)); //------
        AnimationCurve frequency = new AnimationCurve(new Keyframe(-0.2f, 0.6f), new Keyframe(1.2f, 0.602f));

        Variation[] layerVariations = new Variation[3] { new Variation(0, 5, 1, 1f, 0f, 10f),
                                                         new Variation(5, 1, 1, 1f, 10f, 26f),
                                                         new Variation(6, 6, 3, 1f, 36f, 18f) };

        Variation[] detailVariations = new Variation[1] { new Variation(11, 8, 8, 1f, 10f, 2f) };

        Variation[] treeVariations = new Variation[1] { new Variation(0, 4, 4, 0.01f, 20f, 3f) }; //0.0005

        Variation[] enemyVariations = new Variation[1] { new Variation(0, 8, 3, 0.00005f, 0f, 10f) };

        public void Generate(float worldX, float worldY, float[,] heightmap, float[,,] alphamap, int[][,] detailLayer, List<TreeInstance> instances, List<(int index, Vector3 position)> enemies, Vector3 chunkSize)
        {
            #region Initialize Local Vars
            //these vars are initialized in this method because several of these methods will run async at the same time.
            //this local declaration will prevent multiple async methods from manipulating the same vars.
            float result;
            float posX;
            float posY;

            float lValue;
            float lElevation;
            float lSmoother;
            float lFrequency;
            float lAmplitude;
            float lLacunarity;
            float lPersistance;          
          
            int bioX;
            int bioY;
            float slope = 0f;
            float gradientX = 0f;
            float gradientY = 0f;
            int bioIndex = -1;

            TreeInstance instance = new TreeInstance();
            instance.widthScale = 1f;
            instance.heightScale = 1f;
            instance.lightmapColor = Color.white;
            instance.color = Color.white;
            instances.Add(instance);
            #endregion

            for (int y = 0; y < Chunk.lineLength; y++)
                for (int x = 0; x < Chunk.lineLength; x++)
                {
                    #region Set In-Loop Vars    
                    //result holds the height data at a given coordinate
                    //world x/y is the location of this chunk.
                    //x/y is the local position of each coordinate
                    result = 0;
                    posX = worldX + x;
                    posY = worldY + y;
                    #endregion

                    #region Initialize Height Fields
                    //these dictate what data will be placed where.
                    //most of these curves are used for hightmap data (mountains, valleys, hills)
                    //this is also used to describe how types of trees, grass and textures/ layers change over distances
                    lValue = Mathf.PerlinNoise(posX * 0.6f * 0.01f, posY * 0.6f * 0.01f);
                    lElevation = Mathf.PerlinNoise(posX * 0.001f, posY * 0.001f);                  
                    lSmoother = smoother.Evaluate(lElevation);
                    lFrequency = frequency.Evaluate(lValue);
                    lAmplitude = amplitude.Evaluate(lValue);
                    lLacunarity = lacunarity.Evaluate(lValue);
                    lPersistance = persistance.Evaluate(lValue);
                    #endregion

                    //begin calculate height
                    #region Calculate Height
                    //this uses iteration from a noisemap calculator to decide heightmap data.  This works similar to how
                    //trees are made using recursion, or how fractal patterns are made.  Data is calculated repeatedly,
                    //each time, changing it values in a certain way, thus adding smaller and smaller details to the heightmap.
                    for (int i = 0; i < 10; i++)
                    {
                        result += Mathf.PerlinNoise(posX * lFrequency * 0.01f, posY * lFrequency * 0.01f) * lAmplitude;
                        lFrequency *= lLacunarity;
                        lAmplitude *= lPersistance;
                    }
                    heightmap[y, x] = (result * 0.1f + lElevation * 0.202f) / lSmoother;
                    #endregion
                    //end calculate height

                    if (x > 0 && y > 0)
                    {
                        #region Calculate Slope
                        //calculates the slope using trigonometric functions which returns a radian value.
                        //this radian value is then converted into a degree.
                        //slope is calculated for both the x and y axis.
                        //only the axis with the greatest (steepest) slope is used.
                        gradientX = Mathf.Atan(Mathf.Abs((heightmap[y, x] - heightmap[y, x - 1]) * Chunk.correctSize.y)) * Mathf.Rad2Deg;
                        gradientY = Mathf.Atan(Mathf.Abs((heightmap[y, x] - heightmap[y - 1, x]) * Chunk.correctSize.y)) * Mathf.Rad2Deg;
                        slope = Mathf.Max(gradientX, gradientY);
                        #endregion

                        #region Initialize Position
                        //positions are recalculated for the faces of the terrain (as opposed to vertices)
                        //recall that a plane's length will always have 1 less face than its lines.
                        posX = x + worldX - 1;
                        posY = y + worldY - 1;
                        bioX = x - 1;
                        bioY = y - 1;
                        #endregion

                        //begin calculate foilage
                        #region Calculate Layers (map textures)
                        //decides where to place map textures/ layers
                        bioIndex = -1;
                        foreach (Variation layer in layerVariations)
                        {
                            bioIndex = layer.GetPrototypeIndex(slope, lElevation);
                            if (bioIndex > -1)
                            {
                                alphamap[bioY, bioX, bioIndex] = 1f;
                                break;
                            }
                        }
                        #endregion


                        //only places trees and grass if they are above sea level
                        if (heightmap[y, x] * chunkSize.y > Chunk.seaLevel)
                        {
                            #region Calculate details (grass/ flowers/ small plants)
                            //decides where to place details
                            bioIndex = -1;
                            foreach (Variation detail in detailVariations)
                            {
                                bioIndex = detail.GetPrototypeIndex(slope, lElevation);
                                if (bioIndex > -1)
                                {
                                    detailLayer[bioIndex][bioY, bioX] = 14;
                                    break;
                                }
                            }
                            #endregion

                            #region Calculate Trees (trees/ bushes/ bolders/ giant objects)
                            //decides where to place trees
                            bioIndex = -1;
                            foreach (Variation tree in treeVariations)
                            {
                                bioIndex = tree.GetPrototypeIndex(slope, lElevation);
                                if (bioIndex > -1)
                                {
                                    instance.prototypeIndex = bioIndex;
                                    instance.position = new Vector3((float)x / Chunk.faceLength, 0f, (float)y / Chunk.faceLength);
                                    instances.Add(instance);
                                    break;
                                }
                            }
                            #endregion
                        }
                        //end calculate foilage


                        //Begin Calculate Enemies
                        #region Calculate Enemies that will spawn on this chunk
                        //decides where to place trees
                        bioIndex = -1;
                        foreach (Variation enemyVariation in enemyVariations)
                        {
                            bioIndex = enemyVariation.GetPrototypeIndex(slope, lElevation);
                            if (bioIndex > -1)
                            {
                                //enemies.Add((bioIndex, new Vector3(worldX + x, 0, worldY + y)));
                                break;
                            }
                        }
                        #endregion
                        //End Calculate Enemies
                    }
                }
           
        }
    }
}


