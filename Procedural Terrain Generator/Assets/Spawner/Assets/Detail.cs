using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner2
{
    [System.Serializable]
    public struct Detail
    {
        [SerializeField]
        Texture2D detailTexture;
        [SerializeField]
        float minWidth;
        [SerializeField]
        float maxWidth;
        [SerializeField]
        float minHeight;
        [SerializeField]
        float maxHeight;

        public DetailPrototype GetDetailTexture()
        {
            DetailPrototype detailPrototype = new DetailPrototype();
            detailPrototype.prototypeTexture = detailTexture;
            detailPrototype.minWidth = minWidth;
            detailPrototype.maxWidth = maxWidth;
            detailPrototype.minHeight = minHeight;
            detailPrototype.maxHeight = maxHeight;
            detailPrototype.healthyColor = Color.white;
            detailPrototype.dryColor = Color.white;
            return detailPrototype;
        }
    }
}