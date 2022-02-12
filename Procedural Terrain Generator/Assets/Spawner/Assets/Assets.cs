using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

namespace Spawner2.UserAssets
{
    class Assets : MonoBehaviour
    {
        public TerrainLayer[] GetLayers(Object[] details)
        {
            TerrainLayer[] prototypes = new TerrainLayer[details.Length];

            for (int i = 0; i < details.Length; i++)
            {
                prototypes[i] = new TerrainLayer();
                prototypes[i].diffuseTexture = ((TerrainLayer)details[i]).diffuseTexture;
            }
            return prototypes;
        }

        public DetailPrototype[] GetDetails(Object[] details)
        {
            DetailPrototype[] prototypes = new DetailPrototype[details.Length];

            for (int i = 0; i < details.Length; i++)
            {
                prototypes[i] = new DetailPrototype();
                prototypes[i].prototypeTexture = ((Sprite)details[i]).texture;
                prototypes[i].minWidth = 1f;
                prototypes[i].maxWidth = 1.2f;
                prototypes[i].minHeight = 0.6f;
                prototypes[i].maxHeight = 0.8f;
            }
            return prototypes;
        }

        public TreePrototype[] GetTrees(Object[] trees)
        {
            TreePrototype[] prototypes = new TreePrototype[trees.Length];

            for (int i = 0; i < trees.Length; i++)
            {
                prototypes[i] = new TreePrototype();
                prototypes[i].prefab = (GameObject)trees[i];

            }
            return prototypes;
        }

        public GameObject[] GetGameObject(Object[] obj)
        {
            GameObject[] prototypes = new GameObject[obj.Length];

            for (int i = 0; i < obj.Length; i++)
            {
                prototypes[i] = new GameObject();
                prototypes[i] = (GameObject)obj[i];

            }
            return prototypes;
        }
    }
}
