using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class myPlayer : NetworkBehaviour {
	public int MaxHealth = 100;
	[SyncVar(hook = "OnhealthChange")]
	public int Health = 100;
	[SyncVar(hook = "OnFlashNStun")]
	public int flashstun;




	public CharacterController cc;
	public canvasManager cmanager;
	public parkour2 parkourScript;
	public SimpleSmoothMouseLook simplemouse;
	public Animation camerarunAnimation;
	public Animation gunrunAnimation;
	public Animator pullupAnimation;
	public AudioSource walkAudio;
	public AudioSource netfootstepAudioSource;
	public LayerMask layerMask;
	public float speed;
	public float walkspeed = 4f;
	public float runspeed = 6f;
	public float aimspeedreduction= 1f;
	public float jumpspeed;
	public float gravity;
	private float localfootvolume = 0.2f;
	private float netfootvolume = 0.5f;
	public bool enemyawarenessperk = false;


	private Vector3 moveDirection = Vector3.zero;
	private bool canRun;
	private bool shootRun;
	private bool joystickRun;
	private float transDur = 0.2f;
	private bool isRunning;
	public bool isStun;
	public bool isEmp;



	private float rayDistance = 500f;
	public AudioClip[] dirtfootClip;
	public AudioClip[] woodfootClip;
	public AudioClip[] metalfootClip;
	public AudioClip[] concretefootClip;
	private float footSpeed = 0.6f;
	private float walkTimer = 0.0f;
	private RaycastHit hit;
	private int soundID;
	private bool OnGround;
	private bool OnMetal;
	private bool OnWood;
	private bool OnConcrete;
	private bool airbound;
	private float RealAimSpeed;	
	private float RealWalkSpeed;    
	private float RealRunSpeed;


	public Texture2D screenbloodTexture;
	private float HealthRegenTimer = 1f;
	private float screenbloodTimer;
	private bool reducebloodAlpha = false;
	private float alphaPlus = 0.25f;
	private float bloodAlpha;
	public float bloodDownRate = 0.52f;
	public float healWaitTime = 5f;
	public float regenRate = 0.0098f;
	public GameObject deathCam;
	public Transform bodyRotator;
	private bool isDead = false;
	private float crouchdecrease = 0f;


	public Texture2D flashTexture;
	private float flashDuration = 1f;
	private float flashTime;
	private bool flashOn;
	private float flashAlpha;
	private float flashspeed= 4f;
	private float flashdecreasespeed = 0.71f;
	private int counter;
	private bool a;
	private float rate = -0.03f;
	private float d;



	//crouch
	public GameObject crouchObject;
	private float Speed;
	private float defcameraHeight;
	private float tagetCameraHeight;
	private float crouchHeight ;
	private float standardHeight;
	private bool crouching = false;
	private float yh;
	private RaycastHit hit2;
	private float rayDistance2 = 1f;


	private float stuntime = 0f;
	private float stunpercent = 1f;
	private float empduration;



	public void OnFlashNStun(int flashstuntotal){
		int fs = flashstuntotal - flashstun;
		flashstun = flashstuntotal;
		if (isLocalPlayer) {
			switch (fs) {
			case 1:
				flashplayer (5f);
				break;
			case 2:
				flashplayer (1.5f);
				break;
			case 3:
				flashplayer (0.25f);
				break;
			case 4:
				stunplayer (0f,5f);
				flashplayer (0.05f);
				break;
			case 5:
				stunplayer (0f, 3f);
				flashplayer (0.05f);
				break;
			case 6:
				stunplayer (0f, 2f);
				flashplayer (0.05f);
				break;
			case 7:
				empplayer (11f);
				break;
			case 8:
				stunplayer (0f, 5f);
				flashplayer (0.05f);
				break;
			}
		}
	}
		


	public void stunplayer(float sp, float t){
		if (GetComponent<PlayerShootManager> ().tacmask) {t = t - (t * 0.80f);}
		stunpercent = sp;
		stuntime = t;
		isStun = true;
	}

	public void stuneffect(){
		stunpercent += Time.deltaTime / stuntime;
		if (stunpercent > 1f) {stunpercent = 1f; isStun = false;}
		simplemouse.sensitivity.x = PlayerPrefs.GetFloat("Sensativity") * stunpercent;
		simplemouse.sensitivity.y = PlayerPrefs.GetFloat("Sensativity") * stunpercent;
	}

	public void flashplayer(float alp){
		if(flashAlpha <= 0){flashAlpha = 0;}
		if (GetComponent<PlayerShootManager> ().tacmask) {d = alp - (alp * 0.95f);} else {d = alp;}
		a = true;
		flashOn = true;
	}

	public void empplayer(float dur){
		empduration = dur;
		isEmp = true;
	}

	public void empeffect(){
		empduration -= Time.deltaTime;
		if (empduration <= 0) {isEmp = false;}
	}

	public void OnhealthChange(int hith){
		if(isLocalPlayer && hith < Health){bloodScreen(Health - hith);}
		Health = hith;
		cheackhealth ();
	}


	public void cheackhealth(){
		if(isLocalPlayer){
			if(Health <= 0){
				GetComponent<pointSystem>().countdown = 0;
				GetComponent<streakcontrol>().streak = 0;
				GetComponent<pointSystem>().deathLimit += 1;
				//deathCameraPosition();
			}
		}
	}



	[Command]
	public void Cmdreviveme(int amount){
		Health = amount;
	}



	[Command] 
	public void CmdHealthRegen(GameObject myPlayer, int amount, int max){
		myPlayer.GetComponent<myPlayer>().Health += amount;
		if(myPlayer.GetComponent<myPlayer>().Health > max){
			myPlayer.GetComponent<myPlayer>().Health = max;
		}
	}
		



	public void flashAnimationCode(){
		flashTime -= Time.deltaTime;
		if(flashTime < 0f){flashOn=false;}else{flashOn = true;}
		if(flashOn){if(flashAlpha  < 1.0f){flashAlpha  += Time.deltaTime * flashspeed;}}
		if(!flashOn){flashAlpha  -= Time.deltaTime * flashdecreasespeed;}

		if(a==true){
			if(flashTime < rate && counter < 2f){
				flashOn = true;
				flashTime = 0.1f;
				counter += 1;
			}
			if(counter >= 2){
				counter = 0;
				flashTime = 0.1f;
				a = false;	
				flashAlpha  += d;
			}
		}
	}


	public void deadsilenceAttach(bool ds){
		if (ds) {
			netfootvolume = netfootvolume - (netfootvolume * 0.90f);
			localfootvolume = 0f;
		} else {
			netfootvolume = 0.5f;
			localfootvolume = 0.2f;
		}
	}

	public void agilityAttach( bool agli, bool stck){
		if(isCrouch()){
			crouchdecrease = 0.35f;
		}else{ crouchdecrease = 0;}
		if(agli){
			RealWalkSpeed = (walkspeed + (walkspeed * (0.12f - crouchdecrease)) * stunpercent);
			RealRunSpeed = (runspeed + (runspeed * 0.12f) * stunpercent);
		}else{
			RealWalkSpeed = (walkspeed - (walkspeed * crouchdecrease) * stunpercent);	    
			RealRunSpeed = (runspeed * stunpercent);
		}

		if (stck) {
			RealAimSpeed = aimspeedreduction + (aimspeedreduction * 0.80f);
		} else {
			RealAimSpeed = aimspeedreduction;
		}
	}





	public void dmgPlayer(int dmg){
		bloodScreen(dmg);
		Health -= dmg;
		if(Health <= 0 && !isDead){
			isDead = true;
		}
	}

	public bool isCrouch(){
		return crouching; 
	}

	public bool canUnCrouch(){    
		bool crh;
		if(Physics.Raycast(transform.position,Vector3.up, out hit2, rayDistance2,layerMask) && isCrouch()){
			crh =  false;
		}else{crh = true;}
		return crh;
	}
		

	void Start(){
		if (isLocalPlayer) {
			Cmdreviveme (MaxHealth);
			cc = GetComponent<CharacterController> ();
			standardHeight = cc.height;
			crouchHeight = cc.height / 2f;
			Speed = speed;
			crouching = false;
			defcameraHeight = crouchObject.transform.localPosition.y;
			tagetCameraHeight = defcameraHeight - crouchHeight;
		} 
	}

	public void crouchcontrol(){
		if (Input.GetKeyDown ("v") && !GetComponent<PlayerShootManager>().isPaused && canUnCrouch()) {
			crouching = !crouching;
		}
		if(!crouching){
			cc.height = standardHeight ;
			cc.center = new Vector3 (0f, 0f, 0f);
			GetComponent<CapsuleCollider>().height=standardHeight;
			GetComponent<CapsuleCollider>().center = new Vector3 (0f, 0f, 0f);
			yh = Mathf.Lerp(crouchObject.transform.localPosition.y,defcameraHeight, 7f * Time.deltaTime );
			crouchObject.transform.localPosition = new Vector3 (0,yh,0);
		}

		if(crouching){
			cc.height = crouchHeight;
			cc.center = new Vector3 (0f, -0.5f, 0f);
			GetComponent<CapsuleCollider>().height= crouchHeight;
			GetComponent<CapsuleCollider>().center = new Vector3 (0f, -0.5f, 0f);;
			yh = Mathf.Lerp(crouchObject.transform.localPosition.y,tagetCameraHeight, 7f * Time.deltaTime );
			crouchObject.transform.localPosition = new Vector3 (0,yh,0);
		}
	}
		
	void applyGravity()
	{
		moveDirection += Vector3.down * gravity * Time.fixedDeltaTime;
		cc.Move(moveDirection * Time.fixedDeltaTime);
	}


	void Update() {
		if(isLocalPlayer){
			screenblood();
			flashAnimationCode();
			stuneffect ();
			empeffect ();
			crouchcontrol ();
			weaponPullonRun();
			findGroundType();
			shotWhileRunning();
			GetComponent<PlayerShootManager>().isRunning = isRunning;

			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");


			if(parkourScript.isClimbingladder()){
				if(camerarunAnimation != null){camerarunAnimation.Play("IdleAnimation");}
				isRunning = false;
				GetComponent<PlayerShootManager>().isRunning = false;
				if(gunrunAnimation != null){gunrunAnimation.CrossFade("none",transDur);}
				pullupAnimation.CrossFade("nonepullup",0);
				return;
			}

			if(cc.isGrounded){
				walkSound();	
				moveDirection = new Vector3(h,0f,v);
				moveDirection = transform.TransformDirection(moveDirection);
				moveDirection *= speed;
				clampSpeed (speed);
			}

			run();
			jumpNparkour();


			if(!cc.isGrounded && !parkourScript.climb){
				moveDirection += Vector3.down * gravity * Time.deltaTime;
			}else if (!Input.GetButtonDown("Jump")){
				if(cc.isGrounded){moveDirection = new Vector3(moveDirection.x,-1,moveDirection.z);}
			}
				
			if(!parkourScript.climb && !GetComponent<PlayerShootManager> ().isPaused){
				cc.Move(moveDirection * Time.deltaTime * 5f);
			}
		}
	}




	public void run(){
		if(Input.GetButton("Run") && Input.GetKey("w") && !Input.GetButton("Fire2") && !Input.GetButton("Fire1") && !isCrouch() && shootRun == false && !GetComponent<PlayerShootManager>().isMeleeing && !airbound && !isStun && !GetComponent<PlayerShootManager>().isPaused && !GetComponent<PlayerShootManager>().activeWeapon.GetComponent<gun2>().OneFire){
			if(!parkourScript.climb){if(camerarunAnimation != null){camerarunAnimation.Play("CameraRun");}}else{if(camerarunAnimation != null){camerarunAnimation.Play("IdleAnimation");}}
			if(!parkourScript.climb || !GetComponent<PlayerShootManager>().isswapOut || !GetComponent<PlayerShootManager>().isswapIn){if(gunrunAnimation != null){gunrunAnimation.CrossFade("WeaponRun2",0.25f);gunrunAnimation["WeaponRun2"].speed=1f;}}else{if(gunrunAnimation != null){gunrunAnimation.CrossFade("none",0.15f);}}
			isRunning = true;
			if(GetComponent<PlayerShootManager>().activeWeapon != null){if(GetComponent<PlayerShootManager>().activeWeapon.transform.GetComponent<gun2>() != null){GetComponent<PlayerShootManager>().activeWeapon.transform.GetComponent<gun2>().reloadCancel();}}
			speed = RealRunSpeed;
			footSpeed = 0.3f;
		}else{
			if(camerarunAnimation != null){camerarunAnimation.Play("IdleAnimation");}
			if(GetComponent<PlayerShootManager>().isMeleeing){
				if(gunrunAnimation != null){gunrunAnimation.CrossFade("none", transDur);}
			}else{
				if(!GetComponent<PlayerShootManager>().isswapOut){
					if(gunrunAnimation != null){gunrunAnimation.CrossFade("none",transDur);}
				}
			}


			isRunning = false;
			if(GetComponent<PlayerShootManager>().isAiming){
				speed = RealAimSpeed;
				footSpeed = 0.9f;
			}else{
				speed = RealWalkSpeed;
				footSpeed = 0.5f;
			}
		}
	}




	public void clampSpeed(float ToSpeed){
		if(moveDirection.magnitude > ToSpeed && (moveDirection.y != jumpspeed)){
			Vector3 move = Vector3.ClampMagnitude(moveDirection,ToSpeed);
			moveDirection = move; 
		}
	}

	public Vector3 mVelocity(){
		return new Vector3 (moveDirection.x, 0f, moveDirection.z);
	}

	public void shotWhileRunning(){
		if(GetComponent<PlayerShootManager>().activeWeapon != null){
			if(GetComponent<PlayerShootManager>().activeWeapon.GetComponent<gun2>() != null){
				if(Input.GetButton("Fire1") && GetComponent<PlayerShootManager>().activeWeapon.GetComponent<gun2>().currentPullUpTime() > 0f){shootRun = true;}
			
				if (shootRun) {
					if(!GetComponent<PlayerShootManager>().activeWeapon.activeInHierarchy){shootRun = false;}
					if(GetComponent<PlayerShootManager>().activeWeapon.GetComponent<gun2>().currentPullUpTime() <= 0.0f){
						GetComponent<PlayerShootManager>().activeWeapon.GetComponent<gun2>().setOneFire(true);
						shootRun = false;
					}
				}
			}
		}	
	}	



	public void weaponPullonRun(){
		if(GetComponent<PlayerShootManager>().activeWeapon != null){
			if(GetComponent<PlayerShootManager>().activeWeapon.GetComponent<gun2>()!= null){transDur = GetComponent<PlayerShootManager>().activeWeapon.GetComponent<gun2>().weaponPullTransTime();}
		}
	}	


	public void walkSound(){
		if(cc.velocity.magnitude > 0.1f && walkAudio != null){
			walkTimer -= Time.deltaTime;
			walkAudio.volume = localfootvolume;
			if(walkTimer < 0){
				int footnum;
				if(OnGround){
					CmdTellServertoPlayFootStepSound(0, GetComponent<NetworkIdentity>().netId, netfootvolume);
					footnum =  Random.Range(0,dirtfootClip.Length);
					if(dirtfootClip.Length > 0){if(dirtfootClip[footnum] != null){ walkAudio.PlayOneShot(dirtfootClip[footnum]); }}

				}
				if(OnMetal){
					CmdTellServertoPlayFootStepSound(1, GetComponent<NetworkIdentity>().netId, netfootvolume);
					footnum =  Random.Range(0,metalfootClip.Length);
					if(metalfootClip.Length > 0){if(metalfootClip[footnum] != null){walkAudio.PlayOneShot(metalfootClip[footnum]); }}

				}
				if(OnWood){
					CmdTellServertoPlayFootStepSound(2, GetComponent<NetworkIdentity>().netId, netfootvolume);
					footnum =  Random.Range(0,woodfootClip.Length);
					if(woodfootClip.Length > 0){if(woodfootClip[footnum] != null){walkAudio.PlayOneShot(woodfootClip[footnum]); }}

				}
				if(OnConcrete){
					CmdTellServertoPlayFootStepSound(3, GetComponent<NetworkIdentity>().netId, netfootvolume);
					footnum =  Random.Range(0,concretefootClip.Length);
					if(concretefootClip.Length > 0){if(concretefootClip[footnum] != null){walkAudio.PlayOneShot(concretefootClip[footnum]);}}

				}
				walkTimer = footSpeed;
			}
		}
	}



	public void findGroundType(){
		if(Physics.Raycast(transform.position,Vector3.down,out hit,rayDistance,layerMask) && cc.isGrounded){
			if(hit.transform.tag == "Ground"){
				OnGround = true;
				OnMetal = false;
				OnWood = false;
				OnConcrete = false;
			}

			if(hit.transform.tag == "Metal"){
				OnMetal = true;
				OnGround = false;
				OnWood = false;
				OnConcrete = false;
			}


			if(hit.transform.tag == "Wood"){
				OnWood = true;
				OnGround = false;
				OnMetal = false;
				OnConcrete = false;
			}


			if(hit.transform.tag == "Concrete"){
				OnConcrete = true;
				OnGround = false;
				OnMetal = false;
				OnWood = false;
			}

			if(hit.transform.tag != "Ground" && hit.transform.tag != "Metal" && hit.transform.tag != "Wood" && hit.transform.tag != "Concrete"){
				OnConcrete = false;
				OnGround = false;
				OnMetal = false;
				OnWood = false;
			}
		}
	}


	public void jumpNparkour(){
		if(!cc.isGrounded && parkourScript.canClimb && airbound && Input.GetKey("w") && !parkourScript.climb){
			moveDirection = Vector3.zero;
			parkourScript.climb = true;
			pullupAnimation.CrossFade("pullup",0);

		}
		if(cc.isGrounded){
			airbound = false;
		}

		if(Input.GetButtonDown("Jump")){
			if(cc.isGrounded && parkourScript.canClimb && (Input.GetKey("w") || Input.GetAxis("Vertical") > 0f) && !parkourScript.climb){
				moveDirection = Vector3.zero;
				parkourScript.climb = true;
				pullupAnimation.Play("pullup");

			}else{
				if(!parkourScript.climb && cc.isGrounded){
					clampSpeed(speed/1.5f);
					moveDirection.y += jumpspeed;
					airbound = true;
				}
			}
		}

		if(parkourScript.climb){
			GetComponent<PlayerShootManager>().cancelAll();
		}else{
			pullupAnimation.Play("nonepullup");
		}
	}



	public void screenblood(){
		screenbloodTimer -= Time.deltaTime;
		if(screenbloodTimer < 0){reducebloodAlpha = true;}else{reducebloodAlpha = false;}
		if(reducebloodAlpha){bloodAlpha -= Time.deltaTime * bloodDownRate;}
		if(bloodAlpha < 0){bloodAlpha = 0;}
		if(screenbloodTimer < 0 && Health < MaxHealth){
			HealthRegenTimer -= Time.deltaTime;
			if(HealthRegenTimer < 0){
				CmdHealthRegen(this.transform.gameObject,1,MaxHealth);
				HealthRegenTimer = regenRate;
			}
		}
	}    

	public void bloodScreen(int dmg){
		if(dmg <= 10){alphaPlus = 0.1f;}
		if(dmg > 10 && dmg <= 49){alphaPlus = 0.25f;}
		if(dmg > 49 && dmg <= 75){alphaPlus = 0.50f;}
		if(dmg > 75){alphaPlus = 0.85f;}
		if(bloodAlpha <= 1){bloodAlpha += alphaPlus;}else{bloodAlpha=1;}
		screenbloodTimer = healWaitTime;
	}

	public void resetScreen(){
		bloodAlpha = 0;
		screenbloodTimer = 0;
		flashAlpha = 0;
	}


	void OnGUI(){
		if (isLocalPlayer) {
			GUI.color = new Color (1f, 1f, 1f, flashAlpha);
			if (flashAlpha > 0) {
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), flashTexture, ScaleMode.StretchToFill);
			}
			GUI.color = new Color (1f, 1f, 1f, bloodAlpha);
			if (bloodAlpha > 0) {
				GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), screenbloodTexture, ScaleMode.StretchToFill);
			}
		}
	}




	[Command]
	public void CmdTellServertoPlayFootStepSound(int id, NetworkInstanceId netinstanceid, float volume){
		RpcSendfootstepSoundToClients(id,netinstanceid,volume);
	}		


	[ClientRpc]
	public void RpcSendfootstepSoundToClients(int id, NetworkInstanceId netinstanceid, float volume){
		if(GetComponent<NetworkIdentity>().netId == netinstanceid && !isLocalPlayer){
			playfootstepsound(id, volume);
		}
	}				


	public void playfootstepsound(int id, float volume){
		if(netfootstepAudioSource != null){
			netfootstepAudioSource.volume = volume;
			if (enemyawarenessperk) { netfootstepAudioSource.volume = volume + (volume * 0.5f);}
			//netfootstepAudioSource.minDistance = 1f;
			//netfootstepAudioSource.maxDistance = 20f;
			int footnum = 0;
			if (id == 0) {
				footnum =  Random.Range(0,dirtfootClip.Length);
				if(dirtfootClip.Length > 0){if(dirtfootClip[footnum] != null){ netfootstepAudioSource.PlayOneShot(dirtfootClip[footnum]); }}
			}
			if (id == 1) {
				footnum =  Random.Range(0,metalfootClip.Length);
				if(metalfootClip.Length > 0){if(metalfootClip[footnum] != null){netfootstepAudioSource.PlayOneShot(metalfootClip[footnum]); }}
			}
			if (id == 2) {
				footnum =  Random.Range(0,woodfootClip.Length);
				if(woodfootClip.Length > 0){if(woodfootClip[footnum] != null){netfootstepAudioSource.PlayOneShot(woodfootClip[footnum]); }}
			}
			if (id == 3) {
				footnum =  Random.Range(0,concretefootClip.Length);
				if(concretefootClip.Length > 0){if(concretefootClip[footnum] != null){netfootstepAudioSource.PlayOneShot(concretefootClip[footnum]);}}
			}
		}
	}																						









}
