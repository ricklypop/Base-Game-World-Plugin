using UnityEngine;
using System.Collections;

public class GameWorldData : WorldObject {
	public int terrain = -1;
	public string biome = "";

	#region Final Vars
	protected const string 
	TERRAIN = "t", 
	BIOME = "b";
	#endregion

	public override void Start(){
		base.Start ();

		RegisterVar (TERRAIN);
		RegisterVar (BIOME);

	}



	public virtual void Update () {

		//If a world is being created, and the terrain var has been set, write the world id
		if(terrain != -1) {

			SetAttribute (TERRAIN, terrain.ToString ());
			terrain = -1;

		}

		//If type is not an empty string, write the biome type of the world
		if (biome != "") {
			
			SetAttribute (BIOME, biome);
			biome = "";
				
		}

		//If this terrain object has been written to, and the world is empty, create the world
		if (World.currentTerrain == null && GetAttribute(TERRAIN) != "") {
			
			World.currentTerrain = (Transform)Instantiate (
				
				TerrainConverter.main.terrain [int.Parse (GetAttribute(TERRAIN))],
				new Vector3 (0, 0, 0), Quaternion.identity

			);

		}

		World.currentTerrain.SetParent (transform);


	}
}
