using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;

using Spawner2.UserAssets;
using Spawner2.UserGenerator;

namespace Spawner2.UserMap
{
    [RequireComponent(typeof(Assets), typeof(Generator))]
    class Map : MonoBehaviour
    {
        [SerializeField]
        Transform subject;

        int indexX = 0;
        int indexY = 0;

        const int maxChunksFromPlayer = 3;
        public const int maxViewDistanceFromPlayer = 768;
        
        Dictionary<(int, int), Chunk> chunks = new Dictionary<(int, int), Chunk>();
        List<(int, int)> keys = new List<(int, int)> { };
        List<(int, int)> keysToRemove = new List<(int, int)> { };

        public void Update()
        {
            Vector3 position = subject.position;

            indexX = Mathf.RoundToInt(position.x / Chunk.faceLength);
            indexY = Mathf.RoundToInt(position.z / Chunk.faceLength);

            for (int x = indexX - maxChunksFromPlayer; x <= indexX + maxChunksFromPlayer; x++)
            {
                for (int y = indexY - maxChunksFromPlayer; y <= indexY + maxChunksFromPlayer; y++)
                {
                    if (!chunks.ContainsKey((x, y)) && isWithinPlayerRange((x, y), position, 0f))
                    {
                        keys.Add((x, y));
                        chunks.Add((x, y), new Chunk(GetComponent<Assets>(), GetComponent<Generator>()));
                        chunks[(x, y)].LoadAsync(x, y);
                    }
                }
            }

            keysToRemove.Clear();
            foreach ((int, int) key in keys)
            {
                if (chunks[key].isInstantiated && !isWithinPlayerRange(key, position, 64f))
                {
                    chunks[key].Destroy();
                    chunks.Remove(key);
                    keysToRemove.Add(key);
                }
            }
            foreach ((int, int) keyToRemove in keysToRemove)
                keys.Remove(keyToRemove);
        }

        public bool isWithinPlayerRange((int, int) key, Vector3 pos, float offsetDistance)
        {
            return Vector2.Distance(new Vector2(key.Item1 * Chunk.faceLength, key.Item2 * Chunk.faceLength), new Vector2(pos.x, pos.z)) < maxViewDistanceFromPlayer + offsetDistance;
        }
    }
}
