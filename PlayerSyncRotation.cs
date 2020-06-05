using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSyncRotation : NetworkBehaviour {
	[SyncVar]
	private Quaternion syncPlayerPos;
	[SerializeField]
	public Transform myTransform;
	[SerializeField]
	public float lerpRate = 15;
	private float timer;
	private float updaterate = 0.1f;
	// Use this for initialization

	void Start(){
		if(myTransform == null){
			myTransform = this.transform;
		}
	}


	void FixedUpdate(){
		lerpRotaion();
		TransmitRotation();
	}

	void lerpRotaion(){
		if(!isLocalPlayer && myTransform != null){
			myTransform.rotation = Quaternion.Lerp(myTransform.rotation,syncPlayerPos, Time.deltaTime * lerpRate);
		}
	}

	[Command]
	void CmdProvideRotationToServer(Quaternion rotPlayer){
		syncPlayerPos = rotPlayer;
	}

	[ClientCallback]
	void TransmitRotation(){
		if(isLocalPlayer && myTransform != null){
			timer -= Time.deltaTime;
			if(timer <= 0f){
				CmdProvideRotationToServer(myTransform.rotation);
				timer = updaterate;
			}
		}
	}
}
