using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class GameWorldPlayer : Entity {
	public float movementVelocity = BaseGameConstants.DEFAULT_MOVEMENT_VELOCITY;

	private float time;

	#region Final Vars
	protected const string 
	PLAYER_X = "px", 
	PLAYER_Y = "py";
		
	#endregion

	public override void Start(){
		base.Start();

		RegisterMethod(ClientMovePoint);

		RegisterVar(PLAYER_X);
		RegisterVar(PLAYER_Y);

	}

	#region Action implementation
	public override void Act ()
	{
		if (Input.GetMouseButtonUp (0)) {
			Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			LocalMovePoint (new Vector2 (point.x, point.z), movementVelocity);
		}
			
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			Vector3 point = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
			LocalMovePoint (new Vector2 (point.x, point.z), movementVelocity);
		}

	}
	#endregion

	public override void Update(){
		base.Update ();

		if (GetAttribute (PLAYER_X) != "" && GetAttribute (PLAYER_Y) != "" && time >= BaseGameConstants.POINT_UPDATE_TIME) {
			
			UpdatePoint ();
			time = 0;

		} else if (time < BaseGameConstants.POINT_UPDATE_TIME) {
			
			time += Time.deltaTime;

		}

		if (GetAttribute (PLAYER_X) != "" && GetAttribute (PLAYER_Y) != "")
			CheckForStop ();

		if (playerID == SystemInfo.deviceUniqueIdentifier)
			Camera.main.GetComponent<FollowCamera> ().follow = transform;
		
	}

	/// <summary>
	/// Move the local entity to point when the local player commands. Then sends this action to all other clients.
	/// </summary>
	/// <param name="newVelocity">New velocity.</param>
	/// <param name="direction">Direction.</param>
	public void LocalMovePoint(Vector2 point, float newVelocity){

		Parameters p = new Parameters();
		p.AddParam (VELOCITY, newVelocity.ToString());
		p.AddParam(X, point.x.ToString());
		p.AddParam(Y, point.y.ToString());

		ClientMovePoint (p);

		QueueChange (ClientMovePoint, p);

	}

	/// <summary>
	/// Moves the local entity to point when commanded from the ObjectCommunicator, as commanded by another client.
	/// </summary>
	/// <param name="par">Parameters.</param>
	public void ClientMovePoint(Parameters par){


		SetAttribute (PLAYER_X, par.GetParamValue (X));
		SetAttribute (PLAYER_Y, par.GetParamValue (Y));
		SetAttribute (VELOCITY, par.GetParamValue (VELOCITY));
		
		SetAttribute (ROTATION, 
			JsonConvert.SerializeObject (
				
				new SerializableTransform (World.FindRotation (new Vector3 (transform.position.x, 0, transform.position.z),
				new Vector3 (par.GetParamValueAsFloat (X), 0, par.GetParamValueAsFloat (Y))).eulerAngles)
				
			));
		
	}

	void UpdatePoint(){


		SetAttribute (ROTATION, 
			JsonConvert.SerializeObject (

				new SerializableTransform (World.FindRotation (new Vector3 (transform.position.x, 0, transform.position.z),
				new Vector3 (GetAttributeFloat(PLAYER_X), 0, GetAttributeFloat(PLAYER_Y))).eulerAngles)
			
			));


	}

	void CheckForStop(){
		
		Vector2 currentPos = new Vector2 (transform.position.x, transform.position.z);
		Vector2 goToPos = new Vector2 (GetAttributeFloat(PLAYER_X), GetAttributeFloat(PLAYER_Y));
		
		if (Vector2.Distance (currentPos, goToPos) <= BaseGameConstants.POINT_RELATIVE_DISTANCE) {
			
			SetAttribute (PLAYER_X, "");
			SetAttribute (PLAYER_Y, "");
			SetAttribute (VELOCITY, "0");

		}

	}
}
