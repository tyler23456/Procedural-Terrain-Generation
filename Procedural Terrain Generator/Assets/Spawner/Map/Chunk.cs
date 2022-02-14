using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using Spawner2.UserAssets;
using Spawner2.UserGenerator;
using Spawner2.UserSnap;

namespace Spawner2.UserMap
{
	class Chunk
	{
		public const int layer = 6;
		public const float seaLevel = 30f;
		GameObject waterObject;

		float[,] heightmap;
		float[,,] alphamapLayers;
		int[][,] detailLayers;
		List<TreeInstance> trees;
		public static List<(int index, Vector3 position)> enemies = new List<(int index, Vector3 position)>();

		GameObject terrainObject;
		Terrain terrain;
		TerrainData data;

		public const int lineLength = 513;
		public static readonly Vector3 correctSize = new Vector3(32, 1200, 32);
		public const int faceLength = 512;
		public const int halfFaceLength = faceLength / 2;

		Assets assets;
		Generator generator;
		Snap snap;

		public bool isInstantiated
        {
			get { return terrainObject != null; }
        }

        public Chunk(Assets assets, Generator generator, Snap snap)
        {
			this.assets = assets;
			this.generator = generator;
			this.snap = snap;
		}

        public async void LoadAsync(int indexX, int indexY)
		{
            #region Initialize Terrain Data Components
            data = new TerrainData();
			data.terrainLayers = assets.GetLayers(Resources.LoadAll("Map/Layers", typeof(TerrainLayer)));
			data.detailPrototypes = assets.GetDetails(Resources.LoadAll("Map/Grasses", typeof(Sprite)));
			data.treePrototypes = assets.GetTrees(Resources.LoadAll("Map/Trees", typeof(GameObject)));
			data.size = correctSize;
			data.heightmapResolution = lineLength;
			data.alphamapResolution = faceLength;
			data.SetDetailResolution(faceLength, 8);
			data.wavingGrassAmount = 0.2f;
			data.wavingGrassStrength = 0.5f;
			#endregion

			#region Generate Terrain Data Information Asynchronously
			detailLayers = new int[data.detailPrototypes.Length][,];
			trees = new List<TreeInstance> { };
			heightmap = new float[lineLength, lineLength];
			alphamapLayers = new float[faceLength, faceLength, data.terrainLayers.Length];
			for (int i = 0; i < detailLayers.Length; i++)
				detailLayers[i] = new int[faceLength, faceLength];
			trees.Clear();
            var task2 = await Task.Run(() =>
			{
				generator.Generate(indexX * faceLength, indexY * faceLength, heightmap, alphamapLayers, detailLayers, trees, enemies, correctSize);
				return 0;
			});
            #endregion

            #region Spawn Terrain Data
            data.SetHeights(0, 0, heightmap);
			data.SetAlphamaps(0, 0, alphamapLayers);
			for (int i = 0; i < detailLayers.Length; i++)
            {
				data.SetDetailLayer(0, 0, i, detailLayers[i]);
			}
			data.SetTreeInstances(trees.ToArray(), true);
			#endregion

			//#region Spawn Enemies
			//GameObject[] enemyObjs = assets.GetGameObject(Resources.LoadAll("Enemies", typeof(GameObject)));
			//while (enemies.Count > 0)
			//{
				//GameObject obj = GameObject.Instantiate(enemyObjs[enemies[0].index], enemies[0].position + new Vector3(0, 600, 0), Quaternion.identity);
				//enemies.RemoveAt(0);
				//snap.Add(obj.GetComponent<CharacterController>());
			//}
            //#endregion

            #region Instantiate Terrain Gameobject
            terrainObject = Terrain.CreateTerrainGameObject(data);
			SetCenterPosition(new Vector3(indexX * faceLength, 0f, indexY * faceLength));
			terrainObject.layer = layer;
			terrainObject.isStatic = true;
            #endregion

            #region Set Terrain Settings
            terrain = terrainObject.GetComponent<Terrain>();
			terrain.detailObjectDistance = 250f;
			terrain.treeBillboardDistance = 500f;
			terrain.treeCrossFadeLength = 100f;
			terrain.heightmapPixelError = 5f;
			#endregion

			#region Refresh Terrain
			terrain.Flush();
			data.RefreshPrototypes();
            #endregion
        }//

        public void Destroy()
        {
			GameObject.Destroy(terrainObject);
        }

		void SetCenterPosition(Vector3 pos)
		{
			terrainObject.transform.position = new Vector3(pos.x - halfFaceLength, pos.y, pos.z - halfFaceLength);
		}//
	}
}
