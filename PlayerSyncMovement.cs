using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncMovement : NetworkBehaviour {
	[SyncVar]
	private Vector3 syncPos;
	[SerializeField]
	public Transform myTransform;
	[SerializeField]
	public float lerpRate = 9;
	private float timer;
	private float updaterate = 0.095f;

	void Start(){
		if(myTransform == null){
			myTransform = this.transform;
		}
	}

	void Update(){
		lerpPosition();
	}
	void FixedUpdate () {
		TransmitPosition();
	}

	void lerpPosition(){
		if(!isLocalPlayer && myTransform != null){
			myTransform.position = Vector3.Lerp(myTransform.position,syncPos,Time.deltaTime * lerpRate);
		}
	}

	[Command]
	void CmdProvidePostionToServer(Vector3 pos){
		syncPos = pos;
	}


	[ClientCallback]
	void TransmitPosition(){
		if(isLocalPlayer){
			timer -= Time.deltaTime;
			if(timer <= 0f){
				CmdProvidePostionToServer(myTransform.position);
				timer = updaterate;
			}
		}
	}

}