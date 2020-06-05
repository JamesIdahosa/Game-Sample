using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerShootManager :  NetworkBehaviour {
	public GameObject[] WeaponList;
	public GameObject[] KnifeList;
	public string activeGunName;
	public killfeedManager kfmanager;
	public GameObject activeWeapon;
	public GameObject activeSecondary;
	public GameObject activeKnife;
	public Camera scopecamera;
	public GameObject bulletHole;
	public GameObject bullet_trailPrefab;
	public GameObject dustparticlePrefab;
	public AudioClip hitmakerSound;
	public AudioSource hitmakerAudio;
	public hitmarkerUI hitmarkerscript;
	public UnityEngine.UI.Text leftoverammoText;
	public int gunNumber;
	public bool isAiming;
	public bool isRunning;
	public bool isReloading;
	public bool isMeleeing;
	public bool isswapOut;
	public bool isswapIn;
	public bool isPaused = false;
	public bool isClimbing;
	public float defaultcameraFov = 65f;
	private int cycleNum = 0;
	[Header("Perks")]
	[SyncVar(hook = "OnGhost")]
	public bool ghost;
	[SyncVar(hook = "OnFlack")]
	public bool flackjacket;
	public bool quickdraw;
	public bool toughness;
	public bool stock;
	public bool agility;
	public bool fastmag;
	public bool hardline;
	public bool longberrel;
	public bool negate;         //your radar cannot be jamed
	public bool grip;
	public bool recon;
	public bool steadyaim;
	public bool goggles;
	public bool rapidfire;
	public bool tacmask;
	public bool deadsilence;
	public bool awareness;
	public bool fmg;       //increase bullet penetration by 1, no bullet drop dramage throw objects
	public bool fasthands; //swap weapons faster, useequipment faster
	public bool momentum; //each kill restores your health by 25%
	public bool reset; //resets killstreak after you get lastone
	public bool time;// increase killstreak duration
	public bool reloadongo; //can reload when running



	[Command]
	public void CmdtellservertoGhost(bool ght){
		ghost = ght;
	}

	public void OnGhost(bool ght){
		ghost = ght;
	}

	[Command]
	public void CmdtellservertoFlackjJacket(bool flck){
		flackjacket = flck;
	}

	public void OnFlack(bool flck){
		flackjacket = flck;
	}
		
	[Command]
	public void CmdsetRecon(GameObject plyr){
		plyr.GetComponent<shotfired> ().reconnotice++;
	}



	void Start () {
		kfmanager = GameObject.Find ("Canvas").GetComponent<canvasManager> ().feedmng;
		if(isLocalPlayer){
			scopecamera.enabled = false;

			canvasManager cv = GameObject.Find ("Canvas").GetComponent<canvasManager> ();
			leftoverammoText = cv.leftoverammoText;
			hitmarkerscript = cv.hitmarkeruiscript;
			//loadperks ();




			//gunNumber = PlayerPrefs.GetInt("Weapon Choice");
			gunNumber = 0;
			if(gunNumber >= 0 && gunNumber < WeaponList.Length){
				if(WeaponList[gunNumber] != null){
					activeWeapon = WeaponList[gunNumber];
					activeWeapon.SetActive(true);
					activeWeapon.transform.GetComponent <gun2> ().init_Weapon ();
				}
			}

			if(KnifeList.Length > 0){
				if(KnifeList[0] != null){
					KnifeList[0].SetActive(true);
					activeKnife = KnifeList[0];
				}
			}	
		}
	}



	void Update(){
		if (isLocalPlayer) {

			if (activeKnife != null) {
				if (activeKnife.GetComponent <melee>() != null) {
					isMeleeing = activeKnife.GetComponent <melee>().hasSlash;
				}
			} 

			if (isAiming) {
				hitmarkerscript.aimcrosshairsalpha (0f);
			} else {
				hitmarkerscript.aimcrosshairsalpha (1f);
			}

			if (activeWeapon != null) {
				if (activeWeapon.transform.GetComponent <gun2>() != null) {
					activeGunName = activeWeapon.transform.GetComponent <gun2>().gunoptions.weaponName; 
					if (leftoverammoText != null) {
						leftoverammoText.text = activeWeapon.GetComponent<gun2> ().gunoptions.Ammo.ToString() + "/" + activeWeapon.GetComponent<gun2> ().gunoptions.leftoverAmmo.ToString();
					}
					isAiming = activeWeapon.transform.GetComponent <gun2>().isgunAiming ();
					isswapOut = activeWeapon.transform.GetComponent <gun2>().isSwapingOut ();
					isswapIn = activeWeapon.transform.GetComponent <gun2>().isSwapingInto ();
					isReloading = activeWeapon.transform.GetComponent <gun2>().isgunReloading ();
				}
			}




			if(Input.GetKeyDown("1")){
				cycleWeapon();
			}

			isClimbing = GetComponent <parkour2>().climb;
			GetComponent<myPlayer>().deadsilenceAttach (deadsilence);
			GetComponent<myPlayer>().agilityAttach(agility,stock);
			perkmanagament ();
		}
	}






	[Command]
	public void CmdTellServertoSpawnbulletHole(Vector3 hitStuff1, Vector3 hitStuff2){
		RpcspawnBulletHole (hitStuff1, hitStuff2);
	}

	[ClientRpc]
	public void RpcspawnBulletHole(Vector3 hitStuff1, Vector3 hitStuff2){
		GameObject holeInstance = Instantiate(bulletHole,hitStuff1 + (hitStuff2 * 0.01f), Quaternion.FromToRotation(Vector3.up, hitStuff2));
		Destroy (holeInstance, 5f);
	}


	[Command]
	public void CmdtellServerTospawnBulletTrail(Vector3 position, Vector3 direction,float velocity){
		RpcspawnBulletTrail(position,direction,velocity);
	}


	[ClientRpc]
	public void RpcspawnBulletTrail(Vector3 position, Vector3 direction, float velocity){
		GameObject trail = Instantiate(bullet_trailPrefab,position, Quaternion.LookRotation(direction));
		trail.GetComponent<bulletTracer>().Fire(position, direction, velocity);
		trail.GetComponent<bulletTracer>().enabled = true;
	}

	[Command]
	public void CmdtellServerToSpawndustParticles(Vector3 position){
		RpcspawndustParticles(position);
	}


	[ClientRpc]
	public void RpcspawndustParticles(Vector3 position){
		GameObject dust = Instantiate(dustparticlePrefab,position, Quaternion.identity);
		Destroy (dust, 5f);
	}



	[Command]
	public void CmdTellServerToDmgNearbyStuff(GameObject stuff, int dmg){
		stuff.GetComponent<NetworkEquipments> ().Health -= dmg;
	}

	[Command]
	public void CmdTellServerToSendShake(GameObject player, int shkamt){
		player.GetComponent<flinch> ().shaker += shkamt;
	}



	[Command]
	public void CmdTellServerToreducehealth(GameObject player, int Dmg, GameObject myPlayer, int weaponID, string bodyhit_type, string weaponType, Vector3 pos, float flinchAmount){
		if(player.GetComponent<myPlayer>().Health > 0){
			player.GetComponent<myPlayer>().Health -= Dmg;
			player.GetComponent<flinch>().flincher += flinchAmount;
			player.GetComponent<flinch>().enemyPosition = pos;
			if(player != myPlayer){player.GetComponent<pointSystem>().assistNetID = myPlayer.GetComponent<GamemodeManager>().myNetID;}
			if(player.GetComponent<myPlayer>().Health <= 0){
				player.GetComponent<stats>().death++;
				player.GetComponent<pointSystem>().revengeNetID = myPlayer.GetComponent<GamemodeManager>().myNetID;

				if(myPlayer != player){
					myPlayer.GetComponent<stats>().kills++;
					RpcsendFeed(myPlayer.transform.GetComponent<playerName>().pname, player.transform.GetComponent<playerName>().pname, weaponID,player.transform.position,weaponType, player.GetComponent<GamemodeManager>().myNetID, bodyhit_type);
				}
			}
		}
	}



	[ClientRpc]
	public void RpcsendFeed(string playerName, string deadName, int weaponID, Vector3 enemyposition, string weaponType, string emyNetID, string bodyhit_type){
		int w_id = weaponID;
		if(bodyhit_type == "head"){w_id = 6;}
		kfmanager.showkillfeed(playerName, GetComponent<GamemodeManager>().team, GetComponent<GamemodeManager>().myNetID, deadName, emyNetID, w_id);
		if(isLocalPlayer){
			GetComponent<streakcontrol>().GetStreak();
			GetComponent<pointSystem>().spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, true, deadName, 100,"");
			GetComponent<pointSystem>().awards(enemyposition,weaponType,emyNetID,bodyhit_type);
			GetComponent<GamemodeManager>().TDMKill();
		}
	}
		

	public void scope(float fov){
		scopecamera.enabled = true;
		scopecamera.fieldOfView = fov;
	}



	public void cycleWeapon(){
		cycleNum ++;
		if (cycleNum >= WeaponList.Length) {
			cycleNum = 0;
		}
		if(WeaponList.Length > cycleNum){
			activeSecondary = WeaponList [cycleNum];
			changeWeapon ();
		}
	}

	public void changeWeapon(){
		if(activeSecondary != null && activeWeapon != null){
			scopecamera.enabled = false;
			activeWeapon.SetActive(false);
			GameObject placeholder = activeWeapon;
			activeWeapon = activeSecondary;
			activeSecondary = placeholder; 
			activeWeapon.SetActive(true);
			perkmanagament ();
			if(activeWeapon.transform.GetComponent<gun2>() != null){activeWeapon.transform.GetComponent<gun2>().swapInto();}
			activeWeapon.transform.GetComponent<gun2> ().init_Weapon ();
			isswapOut = false;
		}else if(activeWeapon != null){
			activeWeapon.SetActive(false);
			GameObject placeholder1 = activeWeapon;
			activeWeapon = placeholder1;
			activeWeapon.SetActive(true);
			perkmanagament ();
			activeWeapon.transform.GetComponent<gun2>().swapInto();
			activeWeapon.transform.GetComponent<gun2> ().init_Weapon ();
			isswapOut = false;
		}
	}


	public void cancelAll(){
		if(activeWeapon != null){if(activeWeapon.transform.GetComponent<gun2>() != null){activeWeapon.transform.GetComponent<gun2>().CancelAllAnimations();}}
	}

	public void resetAll(){
		if(activeWeapon != null){if(activeWeapon.transform.GetComponent<gun2>() != null){activeWeapon.transform.GetComponent<gun2>().resetAll();}}
	}


	public bool swapWeapon(int weaponNum, int ammo, int leftoverammo){
		bool success =  false;
		if(WeaponList.Length > weaponNum){
			if(WeaponList[weaponNum] != null){

				if(activeWeapon != null){
					if(activeSecondary == null){
						cancelAll();
						resetAll();
						scopecamera.enabled = false;
						activeWeapon.SetActive(false);
						GameObject placeholder = activeWeapon;
						activeWeapon = WeaponList[weaponNum];
						activeSecondary = placeholder; 
						activeWeapon.transform.GetComponent<gun2>().setAmmo(ammo,leftoverammo);
						activeWeapon.transform.GetComponent<gun2>().swapInto();
					}else{
						cancelAll();
						resetAll();
						scopecamera.enabled = false;
						activeWeapon.SetActive(false);
						GameObject placeholder0 = activeWeapon;
						activeWeapon = WeaponList[weaponNum];
						placeholder0.SetActive(false);
						activeWeapon.transform.GetComponent<gun2>().setAmmo(ammo,leftoverammo);
						activeWeapon.transform.GetComponent<gun2>().swapInto();
					}
				}

				if(activeWeapon == null){
					activeWeapon = WeaponList[weaponNum];
					activeWeapon.transform.GetComponent<gun2>().setAmmo(ammo,leftoverammo);
					activeWeapon.transform.GetComponent<gun2>().swapInto();
				}

				success = true;
			}
		}

		return success;
	}


	public void SpawnbulletHole(Vector3 hitStuff1, Vector3 hitStuff2, Transform surfaceObj){
		if(bulletHole != null && surfaceObj != null){
			GameObject holeInstance = Instantiate(bulletHole,hitStuff1 + (hitStuff2 * 0.01f), Quaternion.FromToRotation(Vector3.up, hitStuff2));
			holeInstance.transform.SetParent(surfaceObj,true);
			Destroy(holeInstance, 5f);
		}
	}

	public void SpawndustParticles(Vector3 pos){
		if(dustparticlePrefab != null){
			GameObject dust = Instantiate(dustparticlePrefab,pos,Quaternion.identity);
			Destroy(dust,5f);
		}
	}

	public void spawnBulletTrail(Vector3 position, Vector3 direction,float velocity){
		if(bullet_trailPrefab != null){
			CmdtellServerTospawnBulletTrail (position, direction, velocity);
		}
	}




	public void showHitMarker(){
		hitmarkerscript.hitmarker_show ();
		playHitmarkerSound ();
	}

	public void showheadshotHitMarker(){
		hitmarkerscript.hitmarkerheadshot();
		playHitmarkerSound ();
	}

	public void showcrosshairshot(){
		hitmarkerscript.crosshair_show();
	}


	public void playHitmarkerSound(){
		if(hitmakerSound != null && hitmakerAudio != null){
			hitmakerAudio.PlayOneShot(hitmakerSound);
		}
	}


	public void refilAmmo(){
		if(activeWeapon != null){if(activeWeapon.transform.GetComponent<gun2>() != null){
				activeWeapon.transform.GetComponent<gun2>().refilAmmo();
			}}
	}

	public void perkmanagament(){
		GetComponent<streakcontrol> ().hashardlinePerk = hardline;
		GetComponent<flinch> ().focusAttachment (toughness);
		GetComponent<streakcontrol>().hastimePerk = time;
		if(activeWeapon != null){if(activeWeapon.transform.GetComponent<gun2>() != null){
				activeWeapon.transform.GetComponent<gun2>().gunPerks(fasthands,quickdraw, fastmag,longberrel,grip,steadyaim,rapidfire,reloadongo);
			}}

	}


	public void loadperks(){
		string pks = PlayerPrefs.GetString("perks");
		string[] pkslist = pks.Split(":"[0]);

		for(int i = 0; i < pkslist.Length; i++){
			if (pkslist [i] == "ghost") {CmdtellservertoGhost (true);}
			if (pkslist [i] == "flakjacket") {CmdtellservertoFlackjJacket (true);}
			if (pkslist [i] == "quickdraw") { quickdraw = true;}
			if (pkslist [i] == "fastmag") { fastmag = true;}
			if (pkslist [i] == "toughness") { toughness = true;}
			if (pkslist [i] == "steadyaim") { steadyaim = true;}
			if (pkslist [i] == "longberrel") { longberrel = true;}
			if (pkslist [i] == "grip") { grip = true;}
			if (pkslist [i] == "agility") { agility = true;}
			if (pkslist [i] == "stock") { stock = true;}
			if (pkslist [i] == "hardline") { hardline = true;}
			if (pkslist [i] == "deadsilence") { deadsilence = true;}
			if (pkslist [i] == "awareness") { awareness = true;}
			if (pkslist [i] == "time") { time = true;}
			if (pkslist [i] == "reset") { reset = true;}
			if (pkslist [i] == "momentum") { momentum = true;}
			if (pkslist [i] == "fasthands") { fasthands = true;}
			if (pkslist [i] == "fmg") { fmg = true;}
			if (pkslist [i] == "tacmask") { tacmask = true;}
			if (pkslist [i] == "rapidfire") { rapidfire = true;}
			if (pkslist [i] == "goggles") { goggles = true;}
			if (pkslist [i] == "recon") { recon = true;}
			if (pkslist [i] == "negate") { negate = true;}
			if (pkslist [i] == "reloadongo") { reloadongo = true;}
		}
	
	
	}
		
}
