using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerNetworkManager : NetworkBehaviour {
	public GameObject mybody;
	public GameObject teamNameInfo;
	public GameObject emyNameInfo;
	public GameObject playerNameIcon;


	void Awake(){
		if (isLocalPlayer) {
		
		}
	}

	void Start(){
		if (isLocalPlayer) {
			GetComponent<Collider> ().enabled = true;
			GetComponent<CharacterController> ().enabled = true;
			GetComponent<targetFrameRate> ().enabled = true;
			GetComponent<parkour2> ().enabled = true;
			teamNameInfo.SetActive(false);
			emyNameInfo.SetActive(false);
			mybody.SetActive(true);
			GetComponent<AudioListener>().enabled = true;
			playerNameIcon.SetActive(false);
			GetComponent<MeshRenderer> ().enabled = false;
			GetComponent<exp> ().enabled = true;
		} else {
			GetComponent<Collider> ().enabled = false;
			GetComponent<CharacterController> ().enabled = false;
			mybody.SetActive(false);
			GetComponent<MeshRenderer> ().enabled = false;
			GetComponent<targetFrameRate> ().enabled = false;
			GetComponent<parkour2> ().enabled = false;
			GetComponent<AudioListener>().enabled  = false;
			playerNameIcon.SetActive(true);
			GetComponent<exp> ().enabled = false;
		}
	}


}
