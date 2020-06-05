using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class shotfired : NetworkBehaviour {
	[SyncVar(hook = "OnFireShot")]
	public bool isShooting;
	[SyncVar (hook = "OnReconed")]
	public int reconnotice;
	private float shoot;
	private float duration = 0.45f;
	private float reconDuration = 5f;
	public float reconTimer;
	public AudioSource netgunAudioSource;
	public AudioClip[] netgunsounds;

	void Start(){
		if(isLocalPlayer){
			shoot=0.45f;
		}    
	}

	void Update () {
		reconTimer -= Time.deltaTime;  
		if(isLocalPlayer){
			if(isShooting == true){
				shoot -= Time.deltaTime;
				if(shoot < 0){
					CmdStopshotFire(this.transform.gameObject);
					shoot=duration;
				}
			}
		}	
	}

	[Command]
	public void CmdTellServertoPlayGunSound(int id){
		RpcPlayGunSoundToClients(id);
	}

	[ClientRpc]
	public void RpcPlayGunSoundToClients(int id){
		if(!isLocalPlayer){
			playgunsound(id);
		}
	}

	public void playgunsound(int id){
		if(id >= 0 && id < netgunsounds.Length){
			if(netgunsounds[id] != null){
				netgunAudioSource.PlayOneShot(netgunsounds[id]);
			}
		}
	}
		

	//gun calls this
	public void shotsfired(){
		if(isShooting){shootingUpgun();}
		if(!isShooting){CmdTellServerTotellOthersOfshoots(this.gameObject);}
	}


	public void shootingUpgun(){
		shoot = duration;
	}
		
	[Command]
	public void CmdTellServerTotellOthersOfshoots(GameObject myPlayer){
		myPlayer.transform.GetComponent<shotfired>().isShooting = true;
	}
	[Command]
	public void CmdStopshotFire(GameObject myPlayer){
		myPlayer.transform.GetComponent<shotfired>().isShooting = false;
	}	

	public void OnFireShot(bool fireing){
		isShooting = fireing;
	}

	public void OnReconed(int x){
		reconnotice = x;
		dealRecon();
	}

	public void dealRecon(){
		reconTimer = reconDuration;
	}



}
