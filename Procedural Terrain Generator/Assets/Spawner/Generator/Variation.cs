using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner2.UserGenerator
{
    //this class manages what prototypes (map textures, grasses, trees) will be placed and where they will be placed on the game map.
    //it also determins how many will be placed, the range of prototypes that might be placed, and how prototypes will change over large map distances.
    class Variation
    {
        //start index of prototype range
        int startIndex;
        //end index of prototype range
        int endIndex;
        //curve used to determin the rate of change between start and end indices (the rate of prototype changes over large map distances)
        AnimationCurve curve;
        //how many prototypes are within this range
        int prototypeCount;
        //how many prototypes are used at one time in a smaller area of the map
        int varietyCount;
        //a normalized value indicating how many instances from this range are placed on the map:
        //a value of "zero" means none are placed, a value of "one" means foilage will be placed at EVERY grid/ face of the terrain geometry
        //NOTE use an extremely small value for trees.  Do not exceed 0.002f for trees unless you want to make the game unplayable.
        float abundance;
        //the slope value (measured in degrees ranging from 0 to 90) that the foilage is potentially placed
        float startDegree;
        //this value increments startDegree a varietyCount amount of times.  Used to indicate the slope range inrease for each instance that might be expressed for a small map area.
        float degreeInterval;

        public Variation(int startIndex, int prototypeCount, int varietyCount, float abundance, float startDegree, float degreeInterval)
        {
            this.startIndex = startIndex;
            this.endIndex = startIndex + prototypeCount - 1;
            curve = new AnimationCurve(new Keyframe(0, startIndex), new Keyframe(1, endIndex));
            curve.preWrapMode = WrapMode.Clamp;
            curve.postWrapMode = WrapMode.Clamp;
            this.prototypeCount = prototypeCount;
            this.varietyCount = varietyCount;
            this.abundance = abundance;
            this.startDegree = startDegree;
            this.degreeInterval = degreeInterval;
        }//

        //this function determins what prototype (type of map texture, grass or tree) will be returned, or returns -1 if no prototype is returned
        //used to determin what foilage prototype gets placed where on the game map
        //this function takes a slope arg which determins if a slope arg is within the proper range specified.
        //this function takes a value which determins what range of prototype indexes might be returned
        public int GetPrototypeIndex(float slope, float value)
        {       
            #region Initialize Vars
            float minimum = 0f;
            int index = -1;
            int start = Mathf.RoundToInt(curve.Evaluate(value));
            #endregion

            for (int i = 0; i < varietyCount; i++)
            {
                #region calculate prototype index
                //checks if start is within slope range, if true, then break out of loop and return this prototype index
                minimum = startDegree + (i * degreeInterval);
                if (slope >= minimum && slope < minimum + (degreeInterval * abundance))
                {
                    index = start;
                    break;
                }
                #endregion

                #region increment start
                //increments start, but ensures it stays within range of the startIndex and endIndex by repetition.
                start -= startIndex;
                start++;
                start = start % prototypeCount;
                start += startIndex;
                #endregion
            }
            return index;
        }
    }
}
