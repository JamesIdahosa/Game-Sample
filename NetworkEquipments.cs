using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkEquipments : NetworkBehaviour {
	[SyncVar(hook = "OnDoneUsage")]
	public bool exploded;
	[SyncVar(hook = "OnOwnerNetID")]
	public string ownerNetID  = "";
	[SyncVar(hook = "OnEquipmentTeam")]
	public string equipmentTeam;
	[SyncVar(hook = "OnHealth")]
	public int Health  = 5;
	[SyncVar(hook = "OnEmped")]
	public int emped;
	public string equipmentName;
	public int equipmentID=0;
	public int idimage = 0;
	public int closeDamage= 1000;
	public int midDamage = 1000;
	public int longDamage = 1000;
	public float closerange = 10f;
	public float midrange = 15f;
	public float longrange = 20f;
	public float flinch = 5f;
	public float timeBeforeExplode = 10f;
	public float explosionRadius = 50f;
	public float portRadarRadius = 25f;
	public float shockchargeRadius = 5f;
	public float distanceTopickUpEquipment = 2f;
	public GameObject parent;
	public Collider grenadeCollider;
	public Rigidbody mRigidBody;
	public GameObject explosionPrefab;
	public AudioClip explosiveSound ;
	public AudioClip grenadeHitSound;
	public AudioSource explodeAudioSource;
	public AudioSource hitAudioSource;
	public AudioClip beepingSound;
	private GameObject[] playerList;
	public MeshRenderer[] meshRenderers;
	private int numBounces = 1;
	private int currentBounces = 0;
	private float durationTimer = 10f;
	private bool isTeamGame;
	private float beepTimer;
	private RaycastHit hit2;
	public  RaycastHit pickuphit;
	private bool tripClayMine = false;
	private bool hasReconPerk = false;
	private bool finish = false;
	private float portCounter;
	private float portswipetime = 1.5f;
	private float tempdisableDuration = 5f;
	private float tempdisableTimer;

	private int RealcloseDamage;
	private int RealmidDamage;
	private int ReallongDamage;

	private float stunempcounter;

	public enum ntype{
		grenade,
		semtex,
		flashbang,
		stun,
		empgrenade,
		c4,
		claymine,
		throwing,
		portable_radar,
		shock_charge
	};

	public ntype nadetype = ntype.grenade;
	public LayerMask pickupLayerMask;

	public override void OnStartAuthority(){
		parent = ClientScene.readyConnection.playerControllers[0].gameObject;
		if(parent != null){
			mRigidBody = GetComponent<Rigidbody>();
			mRigidBody.useGravity = true;
			mRigidBody.isKinematic = false;
			if(nadetype == NetworkEquipments.ntype.grenade || nadetype == NetworkEquipments.ntype.flashbang){
				mRigidBody.AddForce(parent.GetComponent<playerequipments>().bodyrotator.forward * parent.GetComponent<playerequipments>().nadethrowforce);
			}

			if(nadetype == NetworkEquipments.ntype.stun || nadetype == NetworkEquipments.ntype.empgrenade){
				mRigidBody.AddForce(parent.GetComponent<playerequipments>().bodyrotator.forward * parent.GetComponent<playerequipments>().nadethrowforce);
				stunempcounter = 5f;
			}

			if(nadetype == NetworkEquipments.ntype.semtex || nadetype == NetworkEquipments.ntype.shock_charge){
				mRigidBody.AddForce(parent.GetComponent<playerequipments>().bodyrotator.forward * parent.GetComponent<playerequipments>().semtexthrowforce);
			}

			if(nadetype == NetworkEquipments.ntype.c4){
				mRigidBody.AddForce(parent.GetComponent<playerequipments>().bodyrotator.forward * parent.GetComponent<playerequipments>().c4throwforce);
			}

			if(nadetype == NetworkEquipments.ntype.throwing){
				mRigidBody.AddForce(parent.GetComponent<playerequipments>().bodyrotator.forward * parent.GetComponent<playerequipments>().knifethrowforce);
			}

			if(nadetype == NetworkEquipments.ntype.portable_radar || nadetype == NetworkEquipments.ntype.claymine){
				mRigidBody.useGravity = false;
				mRigidBody.isKinematic = true;
				transform.position = parent.transform.TransformPoint (0, -1.05f, 1);
				transform.rotation = Quaternion.LookRotation (parent.transform.forward);
				if (nadetype == NetworkEquipments.ntype.portable_radar) {
					GameObject.Find ("Canvas").GetComponent<minimapManager0> ().trackMe (this.gameObject, "myteam", "portable_radar");
				}
			}



			isTeamGame = parent.transform.GetComponent<GamemodeManager>().isTeamGame;
			playerList = parent.transform.GetComponent<playerName>().players;
			CmdTellServerTosetEquipmentTeam(this.transform.gameObject,  parent.transform.GetComponent<GamemodeManager>().team);
			CmdTellServerTosetownerID(this.transform.gameObject,  parent.transform.GetComponent<GamemodeManager>().myNetID);
			durationTimer = timeBeforeExplode;
			grenadeCollider = GetComponent<Collider>();
			hasReconPerk = parent.transform.GetComponent<PlayerShootManager>().recon;
		}
	} 




	void Update(){
		playBeepingSound ();
		if(isClient && hasAuthority && parent != null){
			durationTimer -= Time.deltaTime;
			tempdisableTimer -= Time.deltaTime;
			stunempcounter -= Time.deltaTime;

			if(!exploded && (nadetype == NetworkEquipments.ntype.claymine || nadetype == NetworkEquipments.ntype.c4 || nadetype == NetworkEquipments.ntype.portable_radar || nadetype == NetworkEquipments.ntype.shock_charge)){
				if (parent.GetComponent<myPlayer> ().Health <= 0 && !finish) {
					StartCoroutine(destroyGreanade());
					Cmdtellservergrenadeisdone (this.gameObject);
					finish = true;
				}

				if (Vector3.Distance (transform.position, parent.transform.position) <= distanceTopickUpEquipment) {
					if (Physics.Raycast (transform.position, parent.transform.position - transform.position, out pickuphit, 15f, pickupLayerMask)) {
						if (pickuphit.transform.gameObject == parent) {
							if (Input.GetKeyDown ("q")) {
								finish = true;
								StartCoroutine (destroyGreanade ());
								Cmdtellservergrenadeisdone (this.gameObject);
								parent.GetComponent<playerequipments> ().increaseEquipmentAmount (1);
							}
						}
					}
				}
			}

			if (nadetype == NetworkEquipments.ntype.portable_radar && !exploded && tempdisableTimer <= 0f) {
				portCounter -= Time.deltaTime;
				if (portCounter <= 0f) {
					portableRadar (transform.position);
					portCounter = portswipetime;
				}
			}
				

			if (nadetype == NetworkEquipments.ntype.shock_charge && !exploded && !finish && tempdisableTimer <= 0f) {
				shockCharge (transform.position);
			}
				

			if (durationTimer <= 0 && !exploded && !finish && tempdisableTimer <= 0f && (nadetype == NetworkEquipments.ntype.stun || nadetype == NetworkEquipments.ntype.empgrenade)) {
				if(currentBounces >= 1){
					blowup ();
				}
			}


			if (stunempcounter <= 0f && (nadetype == NetworkEquipments.ntype.stun || nadetype == NetworkEquipments.ntype.empgrenade)) {
				if(!exploded && !finish){blowup ();}else{StartCoroutine (destroyGreanade ()); Cmdtellservergrenadeisdone (this.gameObject);}
			}



			if(durationTimer <= 0 && !exploded && nadetype == NetworkEquipments.ntype.throwing){StartCoroutine (destroyGreanade ()); Cmdtellservergrenadeisdone (this.gameObject);}

			if(durationTimer <= 0 && !exploded && tempdisableTimer <= 0f && (nadetype == NetworkEquipments.ntype.semtex || nadetype == NetworkEquipments.ntype.grenade || nadetype == NetworkEquipments.ntype.flashbang)){ blowup();}

			if(nadetype == NetworkEquipments.ntype.c4){if(Input.GetKeyDown("t") && !exploded && tempdisableTimer <= 0f){blowup();}}

			if(nadetype == NetworkEquipments.ntype.claymine && !exploded && !tripClayMine){                                                 
				if(Physics.Raycast(transform.position,transform.forward,out hit2,5f)){
					if(hit2.transform.tag == "Player" && hit2.transform.gameObject != parent && tempdisableTimer <= 0f){
						if (canDealDamage (hit2.transform.gameObject)) {
							tripClayMine = true;
							playGreanadeImpactSound ();
							StartCoroutine(delayblowup (0.25f));
						}
					}
				}
			}
		}
	}
		



	public void OnCollisionEnter(Collision col){
		playGreanadeImpactSound();
		if (isClient && hasAuthority && parent != null) {
			if (nadetype == NetworkEquipments.ntype.throwing && !exploded) {
				processKnifeHit (col.transform.gameObject);
			}

			if (nadetype == NetworkEquipments.ntype.semtex || nadetype == NetworkEquipments.ntype.c4 || nadetype == NetworkEquipments.ntype.shock_charge) { 
				processContact (col);
			}
			currentBounces++;
		}
	}
		

	public void playBeepingSound(){
		if(beepingSound != null && hitAudioSource != null && nadetype == NetworkEquipments.ntype.semtex && !exploded){
			beepTimer -= Time.deltaTime;
			if(beepTimer < 0){
				hitAudioSource.PlayOneShot(beepingSound);
				hitAudioSource.loop = false;
				beepTimer = 0.14f;
			}		
		}
	}

	public void playExplosionSound(){
		if(explosiveSound != null && explodeAudioSource != null){
			explodeAudioSource.PlayOneShot(explosiveSound);
			explodeAudioSource.loop = false;
		}
	}

	public void playGreanadeImpactSound(){
		if(grenadeHitSound != null && hitAudioSource != null){
			hitAudioSource.pitch = Random.Range(0.8f,1.01f);
			hitAudioSource.PlayOneShot(grenadeHitSound);
			hitAudioSource.loop = false;
		}
	}

	public void stopRenderer(){
		for(int i = 0; i < meshRenderers.Length; i++){
			if(meshRenderers[i] != null){
				meshRenderers[i].enabled = false;
			}
		}
	}



	public IEnumerator delayblowup(float delayTime){
		yield return new WaitForSeconds (delayTime);
		blowup();
	}



	void blowup(){
		if(isClient && hasAuthority && parent != null && !finish){
			finish = true;
			Vector3 positionHit = this.transform.position;
			foreach(GameObject player in playerList){
				if(player != null){
					float playerDistance = Vector3.Distance(player.transform.position, positionHit);
					if(canDealDamage(player)){impactVibration(playerDistance, player);}
					if(playerDistance  <= longrange){
						Vector3 direction = player.transform.position - positionHit;
						RaycastHit[] hit;
						hit = Physics.RaycastAll(positionHit,direction,100f);
						float blockingDistance = Mathf.Infinity;
						float wallDistance = Mathf.Infinity;

						for(int j = 0; j < hit.Length; j++){
							if(hit[j].transform.tag == "block" || hit[j].transform.tag == "wall" || hit[j].transform.tag == "Metal"){
								if(hit[j].distance < blockingDistance){
									blockingDistance = hit[j].distance;
								}
							}
						}


						if(canDealDamage(player)){
							if(playerDistance < blockingDistance){
								applyDamage(player,playerDistance, positionHit);
							}
						}												
					}
				}
			}

			CmdspawnExplosionEffect (transform.position);
			CmdTellServertoPlayExplodeSound ();
			StartCoroutine(destroyGreanade());
			Cmdtellservergrenadeisdone(this.transform.gameObject);
			if(nadetype == NetworkEquipments.ntype.semtex || nadetype == NetworkEquipments.ntype.c4 || nadetype == NetworkEquipments.ntype.claymine || nadetype == NetworkEquipments.ntype.grenade){damageOtherEquipments ();}
			if(nadetype == NetworkEquipments.ntype.empgrenade){empOtherEquipments ();}
			if(nadetype == NetworkEquipments.ntype.flashbang || nadetype == NetworkEquipments.ntype.stun) {tempdisableOtherEquipment ();}
		}
	}			


	public void applyDamage(GameObject hitPlayer, float PlayerDistance, Vector3 pos){
		if(hitPlayer.transform.GetComponent<myPlayer>().Health <= 0){return;}
		if (nadetype == NetworkEquipments.ntype.empgrenade || nadetype == NetworkEquipments.ntype.shock_charge) {if(hitPlayer == parent){return;}}
		playerFlackJacket (hitPlayer);
		if(PlayerDistance < longrange){
			if(parent != hitPlayer){
				parent.transform.GetComponent<PlayerShootManager> ().showHitMarker ();
				if (nadetype == NetworkEquipments.ntype.empgrenade) {CmdtellserverflashOrStunPlayer (hitPlayer, 7);}
				if (nadetype == NetworkEquipments.ntype.shock_charge) {CmdtellserverflashOrStunPlayer (hitPlayer, 8);}
				if(hasReconPerk){ if(!isTeamGame){hitPlayer.transform.GetComponent<shotfired>().dealRecon();}else{parent.transform.GetComponent<PlayerShootManager>().CmdsetRecon(hitPlayer);}}
			}
			if(PlayerDistance < midrange){
				if(PlayerDistance < closerange){
					parent.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(hitPlayer,RealcloseDamage ,parent, equipmentID, "Rank", "Streak", pos,flinch);
					if (nadetype == NetworkEquipments.ntype.flashbang) {CmdtellserverflashOrStunPlayer (hitPlayer, 1);}
					if (nadetype == NetworkEquipments.ntype.stun) {CmdtellserverflashOrStunPlayer (hitPlayer, 4);}
				}else{
					parent.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(hitPlayer,RealmidDamage ,parent, equipmentID, "Rank", "Streak", pos,flinch * 0.5f);
					if (nadetype == NetworkEquipments.ntype.flashbang) {CmdtellserverflashOrStunPlayer (hitPlayer, 2);}
					if (nadetype == NetworkEquipments.ntype.stun) {CmdtellserverflashOrStunPlayer (hitPlayer, 5);}
				}
			}else{
				parent.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(hitPlayer,ReallongDamage ,parent, equipmentID, "Rank", "Streak", pos,flinch * 0.2f);
				if (nadetype == NetworkEquipments.ntype.flashbang) {CmdtellserverflashOrStunPlayer (hitPlayer, 3);}
				if (nadetype == NetworkEquipments.ntype.stun) {CmdtellserverflashOrStunPlayer (hitPlayer, 6);}
			}
		}
	}
		

	[Command]
	public void CmdtellserverflashOrStunPlayer(GameObject player, int amtfs){
		player.GetComponent<myPlayer> ().flashstun += amtfs;
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
	public void Cmdtellservergrenadeisdone(GameObject mnade){
		mnade.GetComponent<NetworkEquipments>().exploded = true;
	}

	public void OnDoneUsage(bool done){
		exploded = done;
		if (done) {
			GetComponent<Collider>().enabled=false;
			stopRenderer();
		}
	}

	public void OnHealth (int h ){
		Health = h;
		if (h <= 0) {
			if (isClient && hasAuthority && parent != null && !exploded && !finish) {
				if (nadetype != NetworkEquipments.ntype.throwing && nadetype != NetworkEquipments.ntype.portable_radar && tempdisableTimer <= 0f) {
					GetComponent<Collider> ().enabled = false;
					stopRenderer ();
					blowup ();
					finish = true;
				} else {
					StartCoroutine(destroyGreanade());
					Cmdtellservergrenadeisdone (this.gameObject);
					stopRenderer ();
					GetComponent<Collider> ().enabled = false;
					finish = true;
				}
			}
		}
	}

	public void OnEquipmentTeam(string x){
		equipmentTeam = x;
		if (nadetype == NetworkEquipments.ntype.semtex || nadetype == NetworkEquipments.ntype.grenade || nadetype == NetworkEquipments.ntype.stun || nadetype == NetworkEquipments.ntype.empgrenade) {
			GameObject.Find ("Canvas").GetComponent<hudManager> ().trackMe (this.gameObject, equipmentTeam, idimage, longrange);
		}
	}

	public void OnEmped(int totalemp){
		int ep = totalemp - emped;
		emped = totalemp;
		if (isClient && hasAuthority && parent != null && !exploded && !finish) {
			switch (ep) {
			case 1:
				StartCoroutine(destroyGreanade());
				Cmdtellservergrenadeisdone (this.gameObject);
				stopRenderer ();
				finish = true;
				GetComponent<Collider> ().enabled = false;
				break;
			case 2:
				tempdisableTimer = tempdisableDuration;
				break;
			}
		}
	}
		
	public void OnOwnerNetID(string x){
		ownerNetID = x;
	}

	public IEnumerator destroyGreanade(){
		if (GetComponent<syncmovement> () != null) {
			GetComponent <syncmovement> ().enabled = false;
		}
		if (GetComponent<syncrotation> () != null) {
			GetComponent <syncrotation> ().enabled = false;
		}
		yield return new WaitForSeconds(9f);
		CmdTellServerToDestroyNade();
	}

	[Command]
	public void CmdTellServerToEmpEquipment(GameObject eqpmt, int epamt){
		eqpmt.transform.GetComponent<NetworkEquipments> ().emped += epamt;
	}

	[Command]
	public void CmdTellServerToDestroyNade(){
		NetworkServer.Destroy(this.transform.gameObject);
	}

	[Command]
	public void CmdTellDmgEquipment(GameObject stuff, int dmg){
		stuff.GetComponent<NetworkEquipments> ().Health -= dmg;
	}

	[Command]
	public void CmdTellServerTosetEquipmentTeam(GameObject e , string t){
		e.GetComponent<NetworkEquipments>().equipmentTeam = t;
	}


	[Command]
	public void CmdspawnExplosionEffect(Vector3 pos){
		RpcspawnExplosionEffect (pos);
	}

	[ClientRpc]
	public void RpcspawnExplosionEffect(Vector3 pos){
		GameObject effect = Instantiate(explosionPrefab,pos ,Quaternion.identity);
		Destroy (effect, 7f);
	}
		
	[Command]
	public void CmdTellServerTosetownerID (GameObject obj, string nid){
		obj.GetComponent<NetworkEquipments> ().ownerNetID = nid;
	}
		
	public void damageOtherEquipments(){
		GameObject[] stf = GameObject.FindGameObjectsWithTag("Equipment");
		for(int i = 0; i < stf.Length; i++){
			float equipmentDistance = Vector3.Distance(transform.position, stf[i].transform.position);
			if (equipmentDistance <= longrange) {
				if (stf [i] != this.gameObject && !stf[i].GetComponent<NetworkEquipments> ().exploded) {
					RaycastHit[] hit;
					Vector3 direction = stf [i].transform.position - transform.position;
					float blockingDistance = Mathf.Infinity;
					hit = Physics.RaycastAll(transform.position,direction,longrange);

					for(int j = 0; j < hit.Length; j++){
						if(hit[j].transform.tag == "block" || hit[j].transform.tag == "wall" || hit[j].transform.tag == "Metal"){
							if(hit[j].distance < blockingDistance){
								blockingDistance = hit[j].distance;
							}
						}
					}
						
					if(equipmentDistance < blockingDistance){
						if (!isTeamGame) {
							CmdTellDmgEquipment (stf [i], 6);
						} else {
							if (stf [i].GetComponent<NetworkEquipments> ().equipmentTeam != equipmentTeam) {
									CmdTellDmgEquipment (stf [i], 6);
							}
							if (stf [i].GetComponent<NetworkEquipments> ().ownerNetID == parent.GetComponent<GamemodeManager> ().myNetID) {
									CmdTellDmgEquipment (stf [i], 6);
							}
						}
					}
				}
			}
		}
	}


	public void empOtherEquipments(){
		GameObject[] stf = GameObject.FindGameObjectsWithTag("Equipment");
		for(int i = 0; i < stf.Length; i++){
			if (stf [i] != this.gameObject && !stf[i].GetComponent<NetworkEquipments> ().exploded) {
				if (Vector3.Distance (stf [i].transform.position, transform.position) <= longrange) {
					if (!isTeamGame) {
						if(stf [i].GetComponent<NetworkEquipments> ().ownerNetID != parent.GetComponent<GamemodeManager> ().myNetID){CmdTellServerToEmpEquipment (stf [i], 1);}
					} else {
						
						if (stf [i].GetComponent<NetworkEquipments> ().equipmentTeam != equipmentTeam) {
							CmdTellServerToEmpEquipment (stf [i],1);
						}
					}
				}
			}
		}
	}

	public void tempdisableOtherEquipment(){
		GameObject[] stf = GameObject.FindGameObjectsWithTag("Equipment");
		for(int i = 0; i < stf.Length; i++){
			float equipmentDistance = Vector3.Distance(transform.position, stf[i].transform.position);
			if (equipmentDistance <= longrange) {
				if (stf [i] != this.gameObject && !stf[i].GetComponent<NetworkEquipments> ().exploded && stf [i].GetComponent<NetworkEquipments> ().ownerNetID != parent.GetComponent<GamemodeManager> ().myNetID) {
					RaycastHit[] hit;
					Vector3 direction = stf [i].transform.position - transform.position;
					float blockingDistance = Mathf.Infinity;
					hit = Physics.RaycastAll(transform.position,direction,longrange);

					for(int j = 0; j < hit.Length; j++){
						if(hit[j].transform.tag == "block" || hit[j].transform.tag == "wall" || hit[j].transform.tag == "Metal"){
							if(hit[j].distance < blockingDistance){
								blockingDistance = hit[j].distance;
							}
						}
					}
						
					if(equipmentDistance < blockingDistance){
						if (!isTeamGame) {
							CmdTellServerToEmpEquipment (stf [i], 2);
						} else {
							if (stf [i].GetComponent<NetworkEquipments> ().equipmentTeam != equipmentTeam) {
								CmdTellServerToEmpEquipment (stf [i], 2);
							}
						}
					}
				}
			}
		}
	}


	[Command]
	public void CmdTellServertoPlayExplodeSound(){
		RpcplayExplodeSound();
	}

	[ClientRpc]
	public void RpcplayExplodeSound(){
		playExplosionSound ();
	}

	public void impactVibration(float playerdistance, GameObject player){
		if(nadetype == NetworkEquipments.ntype.semtex || nadetype == NetworkEquipments.ntype.c4 || nadetype == NetworkEquipments.ntype.claymine || nadetype == NetworkEquipments.ntype.grenade){
			if(playerdistance <= explosionRadius){
				parent.transform.GetComponent<PlayerShootManager> ().CmdTellServerToSendShake (player, 4);
			}
		}
	}


	public void portableRadar(Vector3 pos){
		for(int i = 0 ; i < playerList.Length; i++){
			if(playerList[i] != null){
				if (playerList [i] != parent) {
					if (canDealDamage (playerList [i])) {
						if (Vector3.Distance (playerList [i].transform.position, pos) <= portRadarRadius) {
							playerList [i].transform.GetComponent<shotfired>().dealRecon();
						}
					}
				}
			}
		}	
	}




	public void shockCharge(Vector3 pos){
		for(int i = 0 ; i < playerList.Length; i++){
			if(playerList[i] != null){
				if (playerList [i] != parent) {
					if (canDealDamage (playerList [i])) {
						if (Vector3.Distance (playerList [i].transform.position, pos) <= shockchargeRadius) {
							blowup ();
							break;
						}
					}
				}
			}
		}	
	}


	public void processKnifeHit(GameObject hitobj){
		if(hitobj != parent && currentBounces <= numBounces){
			if (hitobj.transform.tag == "Player") {
				if (canDealDamage (hitobj)) {
					parent.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(hitobj,100 ,parent, equipmentID, "Rank", "Streak", transform.position, flinch);
					StartCoroutine (destroyGreanade ());
					Cmdtellservergrenadeisdone (this.gameObject);
				} 
			} 
		
			if (hitobj.transform.tag == "Equipment") {
				if (!isTeamGame) {
					if(hitobj.GetComponent<NetworkEquipments> ().ownerNetID != parent.GetComponent<GamemodeManager> ().myNetID){ CmdTellDmgEquipment (hitobj, 6);  }
				} else {

					if (hitobj.GetComponent<NetworkEquipments> ().equipmentTeam != equipmentTeam) {
						CmdTellDmgEquipment (hitobj, 6);
					}
				}
			}
		}
	}



	public void processContact(Collision col){
		if(col.transform.gameObject != parent){
			if (nadetype == NetworkEquipments.ntype.semtex || nadetype == NetworkEquipments.ntype.c4) { 
				this.transform.position = col.contacts [0].point + (col.contacts [0].normal * 0.1f);
				this.transform.rotation = Quaternion.LookRotation (col.contacts [0].normal);
				transform.RotateAround (transform.position, col.contacts [0].normal, Random.Range (0, 360));
				this.transform.SetParent (col.transform, true);
				mRigidBody.useGravity = false;
				mRigidBody.isKinematic = true;
			}
			if(nadetype == NetworkEquipments.ntype.shock_charge){
				this.transform.position = col.contacts [0].point + (col.contacts [0].normal * 0.1f);
				this.transform.rotation = Quaternion.LookRotation (col.contacts [0].normal);
				this.transform.SetParent (col.transform, true);
				mRigidBody.useGravity = false;
				mRigidBody.isKinematic = true;
			}
		}
	}


	public void playerFlackJacket(GameObject player){
		float percent = 0.5f;
		if (player.GetComponent<PlayerShootManager> ().flackjacket) {
			RealcloseDamage = (int)(closeDamage - (closeDamage * percent));
			RealmidDamage  = (int)(midDamage - (midDamage * percent));
			ReallongDamage = (int)(longDamage - (longDamage * percent));
		} else {
			RealcloseDamage = closeDamage;
			RealmidDamage  = midDamage ;
			ReallongDamage = longDamage;
		}
	}

		
}
