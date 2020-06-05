using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChooperGunner : NetworkBehaviour {
	[SyncVar(hook = "OnstreakTeam")]
	public string streakTeam;
	[SyncVar(hook = "OnHealth")]
	public int Health  = 500;
	public string streakName;
	public int streakID;
	public int Damage = 15;
	public float firerate = 15f;
	public float duration = 1f;
	public GameObject parent;
	public Camera cam;
	public Transform camshake;
	public GameObject bullet_trailPrefab;
	public Transform bullet_trailSpawnPoint;
	public AudioClip chooperShot;
	public AudioClip hitmarkersound;
	public AudioSource hitmakerAudio;
	public AudioSource chooperaudio;
	public AudioSource  chooperNetaudio;
	public AudioSource  chooperEngineaudio;
	private float counter;
	private float defaultcamview = 60f;
	private float aimview = 55f;
	private float aimspeed = 15f;
	private float distance = 1000f;
	public float chooperrecoilSideWays = 1.5f;
	public float chooperrecoilUpwords = 1.5f;
	private float camreturn = 35.0f;
	private bool isDone = false;
	private RaycastHit hit;
	private bool isTeamGame;
	private string myTeam;
	public float rotationSpeed = 0.5f;
	public float forwardSpeed = 25f;
	private Transform[] waypoints;
	private int currentwaypoint = 0;
	private float distancetowaypoint;
	private Vector3 direction;
	private canvasManager cv ;
	public enum stype{
		ac130,
		reaper,
	};
	public stype streaktype = stype.ac130;




	public override void OnStartAuthority(){
		parent = ClientScene.readyConnection.playerControllers[0].gameObject;

		if(parent != null){
			GetComponent<AudioListener>().enabled = true;
			isTeamGame = parent.transform.GetComponent<GamemodeManager>().isTeamGame;
			myTeam = parent.transform.GetComponent<GamemodeManager>().team;
			if(camshake != null){cam.gameObject.SetActive(true);}
			if(cam != null){cam.enabled = true;}
			cv = GameObject.Find ("Canvas").GetComponent<canvasManager> ();
			waypoints = cv.ac130waypoints;
			cv.ac130crossHairs.enabled = true;
			CmdTellServerTosetStreakTeam(this.transform.gameObject,  myTeam);
			isDone = false;
		}
	}








	[ClientCallback]
	void Update () {
		if(isClient && hasAuthority && parent != null){
			movement();
			camshake.localRotation = Quaternion.Slerp(camshake.localRotation, Quaternion.identity, Time.deltaTime * camreturn);
			counter -= Time.deltaTime * firerate;
			duration  -= Time.deltaTime;
			if(!isDone){shootemup();}


			if (Input.GetButton ("Fire2")) {
				cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, aimview, Time.deltaTime * aimspeed);
			} else {
				cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,defaultcamview,Time.deltaTime * aimspeed);
			}



			if ((duration <= 0 || Health <= 0) && !isDone) {
				parent.transform.GetComponent<streakcontrol>().controllingStreak = false;
				GetComponent<AudioListener>().enabled = false;
				cam.enabled = false;
				cv.ac130crossHairs.enabled = false;
				StartCoroutine (destroyPlane ());
				isDone = true;
			}
		}
	}



	public void movement(){
		if(waypoints.Length > currentwaypoint){
			if(waypoints[currentwaypoint] != null){
				direction =  waypoints[currentwaypoint].position - transform.position;
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);

				distancetowaypoint = Vector3.Distance(transform.position, waypoints[currentwaypoint].position);
				if(distancetowaypoint < forwardSpeed + 5f){
					currentwaypoint ++;
					if(currentwaypoint >= waypoints.Length){ currentwaypoint = 0; }
				}
				transform.Translate(0f,0f, Time.deltaTime * forwardSpeed);
			}
		}
	}


	public void shootemup(){
		if(Input.GetButton("Fire1") && counter <= 0){
			counter = 1;
			if (chooperaudio != null) {chooperaudio.PlayOneShot (chooperShot);}
			CmdtellServertoPlayChooperSound(GetComponent<NetworkIdentity>().netId);
			float spreadY = Random.Range(-chooperrecoilSideWays,chooperrecoilSideWays);
			float spreadX = Random.Range(-chooperrecoilUpwords,0);
			var gunspread = Quaternion.Euler(spreadX,spreadY,0);
			camshake.localRotation *= gunspread;
			if(bullet_trailSpawnPoint !=null && bullet_trailPrefab != null){CmdtellServertoChooperbulletTrail(bullet_trailSpawnPoint.position, camshake.forward, 600f);}


			if(Physics.Raycast(camshake.transform.position,camshake.transform.forward,out hit,distance)){
				if (hit.transform.tag == "wall" || hit.transform.tag == "block" || hit.transform.tag == "Ground"){
					parent.transform.GetComponent<PlayerShootManager>().CmdtellServerToSpawndustParticles(hit.point);
				}	

				for(int i = 0 ; i < parent.transform.GetComponent<playerName>().players.Length; i++){
					if(parent.transform.GetComponent<playerName>().players[i] != null){
						if(canDealDamage(parent.transform.GetComponent<playerName>().players[i])){
							if(Vector3.Distance(hit.point, parent.transform.GetComponent<playerName>().players[i].transform.position) < 3f){
								if(parent.transform.GetComponent<playerName>().players[i].GetComponent<myPlayer>().Health > 0){
									parent.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(parent.transform.GetComponent<playerName>().players[i],Damage, parent, streakID, "Rank", "Streak", transform.position, 0.34f);
									parent.transform.GetComponent<PlayerShootManager>().showHitMarker();
									playhitAudioClip ();
								}
							}
						}


						if(Vector3.Distance(hit.point, parent.transform.GetComponent<playerName>().players[i].transform.position) < 20f){
							parent.transform.GetComponent<PlayerShootManager> ().CmdTellServerToSendShake (parent.transform.GetComponent<playerName>().players[i], 1);
						}
					}
				}
			}					
		}
	}



	public bool canDealDamage(GameObject playerHit){
		if(!isTeamGame){return true;}
		if(isTeamGame){
			if(playerHit == parent){return true;}
			if(playerHit.transform.GetComponent<GamemodeManager>().team != parent.transform.GetComponent<GamemodeManager>().team){return true;}
		}
		return false;
	}



	[Command]
	public void CmdtellServertoPlayChooperSound(NetworkInstanceId netinstanceid) {
		RpcplayshootingSound(netinstanceid);
	}

	[ClientRpc]
	public void RpcplayshootingSound(NetworkInstanceId netinstanceid){
		if(GetComponent<NetworkIdentity>().netId == netinstanceid && !hasAuthority){
			playshootAudioClip();
		}
	}


	public void playshootAudioClip(){
		if(chooperNetaudio !=null){chooperNetaudio.PlayOneShot(chooperShot);}
	}

	public void playhitAudioClip(){
		if(hitmakerAudio !=null){hitmakerAudio.PlayOneShot(hitmarkersound);}
	}

	[Command]
	public void CmdtellServertoChooperbulletTrail(Vector3 position, Vector3 direction, float velocity) {
		RpcspawnChooperbulletTrail(position,direction,velocity);
	}

	[ClientRpc]
	public void RpcspawnChooperbulletTrail(Vector3 position, Vector3 direction, float velocity){
		GameObject cg_trail = Instantiate(bullet_trailPrefab,position, Quaternion.LookRotation(direction));
		cg_trail.GetComponent<bulletTracer>().Fire(position,direction,velocity);	
		cg_trail.GetComponent<bulletTracer>().enabled = true;
	}

	public IEnumerator destroyPlane(){
		if (GetComponent<syncmovement> () != null) {
			GetComponent <syncmovement> ().enabled = false;
		}
		if (GetComponent<syncrotation> () != null) {
			GetComponent <syncrotation> ().enabled = false;
		}
		yield return new WaitForSeconds(10f);
		CmdTellServerToPlane();
	}
		

	[Command]
	public void CmdTellServerToPlane(){
		NetworkServer.Destroy(this.transform.gameObject);
	}




	[Command]
	public void CmdTellServerTosetStreakTeam(GameObject strk,  string t){
		strk.GetComponent<ChooperGunner> ().streakTeam = t;
	}


	public void OnHealth (int h ){
		Health = h;
		if (h <= 0) {
			if (isClient && hasAuthority && parent != null) {

			}
		}
	}


	public void OnstreakTeam (string t ){
		streakTeam = t;
	}




}
