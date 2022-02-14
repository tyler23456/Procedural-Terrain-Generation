using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner2
{
    [System.Serializable]
    public struct Tree
    {
        [SerializeField]
        GameObject tree;

        public TreePrototype GetTreePrototype()
        {
            TreePrototype treePrototype = new TreePrototype();
            treePrototype.prefab = tree;
            return treePrototype;
        }//
    }
}