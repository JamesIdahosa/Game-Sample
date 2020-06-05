using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class gun2 : MonoBehaviour {
	public bool OneFire = false;
	public Transform target;
	public AnimationCurve recoilCurve;
	private float rTime;
	public float recoilRate = 5f;
	public float  recoilMultipliyer = 0.021f;
	private float centerCounter;







	[System.Serializable]
	public class objz{
		public GameObject myPlayer;                        
		public Transform weaponkickmod;
		public Transform runObject;
		public Transform recoilmod;
		public SimpleSmoothMouseLook sm;
		public GameObject bullet;
		public Transform spawnPoint;
		public Camera mainCam;
		public Camera weaponCam;
		public Transform adjust;
		public Transform bulletEjectPoint;
		public GameObject BulletShell;
		public Renderer muzzleFlesh;
		public Light muzzleLight;		
		public AudioSource weaponAudio;
		public Texture2D scopeTexture;
		public Renderer[] meshRenderers;
	}
	public objz Objects = new objz();

	[System.Serializable]
	public class shootxx{
		public string weaponName;
		public int weaponID;
		public int weaponIndex;
		public string specialAimFireName;
		public string specialHipFireName;
		public string specialReloadName;
		public string specialReloadFullName;
		public string specialIdleName;
		public string specialSwapOutName;
		public string specialSwapInName;
		public string specialAimInName;
		public string specialAimOutName;
		public string specialWalkName;

		public int Ammo= 67;
		public int clipSize =67;
		public int leftoverAmmo = 260;
		public int maxleftoverAmmo = 260;
		public float maxVelocity=600f;
		public float firespeed = 10.2f;
		public float aimSpeed = 0.25f;
		public float zoomSpeed = 0.2f;
		public float zoomFov= 40f;
		public float range = 30f;
		public int closeDamage = 25;
		public int longDamage = 25;
		public float headshotmultiplier= 1.2f;
		public float flinch = 1f;
		public float aimRotationX;
		public float pullUpDuration = 0.24f;
		public float pulldownDuration = 0.25f;
		public bool sniper= false;
		public bool semiAuto = false;
		public bool shotgun = false;
		public bool rpg= false;
		public bool burstFire= false;

		public enum mod{
			normal,
			auto
		};
		public mod burstMode = mod.normal;
		public int burstAmount = 3;
		public enum mod0{
			normal,
			auto
		};
		public mod0 shotgunMode = mod0.normal;

		[System.Serializable]
		public class timz{
			public float reloadTime;
			public float swapOutTime;
			public float swapInTime;
			public float reloadsound1Time;
			public float reloadsound2Time;
			public float reloadsound3Time;

		}
		public timz timeoptions = new timz();

		[System.Serializable]
		public class sesz{
			public AudioClip firesound;
			public AudioClip silencersound;
			public AudioClip aimsound;
			public AudioClip reloadsound1;
			public AudioClip reloadsound2;
			public AudioClip reloadsound3;
			public int firesoundID;
			public int supressorID;
		}
		public sesz sounds = new sesz();


		[System.Serializable]
		public class posz{
			public Vector3 aimposition = Vector3.zero;
			public Vector3 defposition = Vector3.zero;
			public Vector3 aimrotation = Vector3.zero;
		}
		public posz aimoptions = new posz();


		[System.Serializable]
		public class posrun{
			public Vector3 runposition = Vector3.zero;
			public Vector3 runrotation = Vector3.zero;
		}
		public posrun runoptions = new posrun();

		}

	public shootxx gunoptions = new shootxx();
	private int numShotgunPallet = 6;
	private float counter;
	private float muzzlecounter;
	private float swapCounter;
	private float swapToCounter;
	private bool isSwaping;
	private bool swapTo;


	private bool isreloading;
	private bool isAiming;
	private float  defcameraView = 60f;
	private float  muzzletime = 0.15f;
	private bool silencer;
	private float  silencerRangeDrop;
	private float  projectilesPerShot = 1f;
	private float  AnimationSpeed = 1f;
	private float  pullUpDur;
	private float  autoReloadCounter;
	private bool  autoRel;


	private float PULLUPDURATION; 
	private float RECOIL;
	private float pos;
	private float posx;
	private float RealRecoilxAim;
	private float RealRecoilyAim;
	private float RealRecoilxHip;
	private float RealRecoilyHip;
	private bool center;
	private float centerRate = 9.57f;
	private float recoilFadeDelay;
	private string typeOfWeapon;
	private float burstSpeed = 0.05f;
	private bool  AimbasedOnAmmo = false;
	private float PULLUPONRUN;
	private Vector3 RealAimPosition;
	private float RealZoomAmount;
	private float RealfireSpeed;
	private bool  sightsActive = false;
	private bool  canAutoBurst;
	private bool  canAutoShotgun;
	private float RealReloadTime;
	private float RealSwapOutTime;
	private float RealSwapInTime;
	private float AnimationSwapOutSpeed = 1f;
	private float AnimationSwapInTimeSpeed = 1f;
	private bool  CancelAll;
	private int   count = 0;
	private float burstcounter;
	private float RealRecoil;
	private float sightrotx;
	private float gripMultiplier;
	private float RealaimSpeed;
	private float RealzoomSpeed;
	private float RealpullupDuration;
	private float Realrange;
	private float Realreloadsound1Time;
	private float Realreloadsound2Time;
	private float Realreloadsound3Time;
	private bool  playedreloadsound1;
	private bool  playedreloadsound2;
	private bool  playedreloadsound3;
	private float scopeTime;
	private bool  inScope = false;
	private Vector3 weaponPos;
	private Vector3 curVect;
	private Vector2 weaponFovPos;
	private Vector2 curVecFov;
	private float initialdistance;
	private float aimpercentage;

	//weapon perks
	private bool fastswap; 
	private bool quickdraw; 
	private bool fastmag; 
	private bool longberrel; 
	private bool grip; 
	private bool steadyaim; 
	private bool rapidfire;
	private bool reloadongo;


	private float firstanimTime = 0.3666667f;
	private float secondanimTime = 0.7333333f;
	private float thirdanimTime = 0.5f;
	private float loopTimer;
	private float RealfirstanimTime;
	private float RealsecondanimTime;
	private float RealthirdanimTime;
	private bool firstanim = false;
	private bool secondanim = false;
	private bool thirdanim = false;
	private bool shotgunRel = false;
	private float AnimationfirstSpeed;
	private float AnimationsecondSpeed;
	private float AnimationthirdSpeed;

	private float zPosRec;
	private float recZDamp = 0.0f;
	private float zPosRecNext;
	private float shootstarttime;
	private float m_LastFrameShot = -10f;



	private Vector3 curVect_gun;
	private Vector3 cur_gun;
	private Vector3 curVect_gun_pos;
	private Vector3 cur_gun_pos;
	private bool run;
	[HideInInspector]
	public float recoilAmount_x = 0.5f;
	[HideInInspector]
	public float recoilAmount_y = 0.5f;
	[HideInInspector]
	public float recoilAmount_z;
	[Header("Recoil Not Aiming")]
	[Tooltip("Recoil amount on that AXIS while NOT aiming")]
	public float recoilAmount_x_non = 0.01f;
	[Tooltip("Recoil amount on that AXIS while NOT aiming")]
	public float recoilAmount_y_non = 0.01f;
	[Tooltip("Recoil amount on that AXIS while NOT aiming")]
	public float recoilAmount_z_non = 0.015f;
	[Tooltip("Recoil amount on that AXIS while aiming")]
	[Header("Recoil Aiming")]
	public float recoilAmount_x_ = 0.005f;
	[Tooltip("Recoil amount on that AXIS while aiming")]
	public float recoilAmount_y_ = 0.005f;
	[Tooltip("Recoil amount on that AXIS while aiming")]
	public float recoilAmount_z_;
	private Vector3 cv;
	private Vector3 tp;

	public float AimRecoilMultiplier = 0.3f;
	public float spreadAmountAim = 0f;
	public float spreadAmountHip = 6f;
	private float spreadMultiplier = 1f;
	public Animator anim;
	private Vector3 leanpos;
	private Vector3 cur_pos;
	private Vector2 shellforce_up = new Vector2(7f, 17f);
	private Vector2 shellforce_right= new Vector2(90f, 130f);

	public void init_Weapon(){
		defcameraView = Objects.myPlayer.GetComponent<PlayerShootManager>().defaultcameraFov;
		weaponPos = gunoptions.aimoptions.defposition;
		cur_pos = gunoptions.aimoptions.defposition;
		AnimationCurve ac = new AnimationCurve ();
		float t = 0f;
		for (int i = 0; i < 300; i++) {
			ac.AddKey (new Keyframe (t, Random.Range (-0.8f, 0.8f)));
			t += Random.Range (0.01f, 0.08f);
			if (t >= 1f) {

				break;
			}
		}
		//recoilCurve = ac;
		//recoilCurve.postWrapMode = WrapMode.Loop;
	}
		
	void Update (){
			
		if (!this.center){
			tp = Vector3.Lerp(tp, cv, Time.deltaTime / 0.05f);
			rTime += Time.deltaTime * Random.Range(recoilRate, recoilRate + 1.5f);
			if(target != null){
				target.localPosition = new Vector3(recoilCurve.Evaluate (rTime),0f,0f) * recoilMultipliyer;
			}
		}


		if(Objects.recoilmod != null){Objects.recoilmod.transform.localEulerAngles = this.tp;}
		defcameraView = Objects.myPlayer.GetComponent<PlayerShootManager>().defaultcameraFov;
		aimNzoom();
		swapingOut();
		swapouting();
		swapIntoing();
		autoReload();
		computerautoReload();
		aimAmmoCount();
		typeOfFire();
		fireBurst();
		cameraZoom();
		autoshotgunReload();
		quickdrawAttach();
		longberrelAttach();
		gripAttach();
		fastreloadAttach();
		hipaimAttah();
		rapidfireAttach();
		sniperEffect ();
		shootEmUp();
		runEffect ();

		if (this.shootstarttime + 0.1 > (double)Time.time){this.zPosRecNext = -this.recoilAmount_z;}else{this.zPosRecNext = 0f;}
		this.zPosRec = Mathf.SmoothDamp(this.zPosRec, this.zPosRecNext, ref this.recZDamp, 0.25f, float.PositiveInfinity, Time.deltaTime);
		if (this.Objects.weaponkickmod != null){this.Objects.weaponkickmod.localPosition = new Vector3(this.Objects.weaponkickmod.localPosition.x, this.Objects.weaponkickmod.localPosition.y, this.zPosRec);}



		if(!Objects.myPlayer.GetComponent<PlayerShootManager>().isRunning){CancelAll = false;}
		counter  -= Time.deltaTime * RealfireSpeed;
		centerCounter -= Time.deltaTime;
		if(!Objects.myPlayer.transform.GetComponent<PlayerShootManager>().isRunning){PULLUPDURATION -= Time.deltaTime;}






		//--------------recoilmoduprecoil-----------
		if(!Input.GetButton("Fire1") || isreloading || gunoptions.Ammo == 0 || Objects.myPlayer.transform.GetComponent<PlayerShootManager>().isRunning || Objects.myPlayer.GetComponent<PlayerShootManager>().isPaused || gunoptions.semiAuto || (gunoptions.shotgunMode == gun2.shootxx.mod0.normal  && gunoptions.shotgun) || (gunoptions.burstMode ==  gun2.shootxx.mod.normal && gunoptions.burstFire) || gunoptions.sniper || gunoptions.rpg){center = true;}else{center = false;}

		recoilFadeDelay -= Time.deltaTime;
		if (center && recoilFadeDelay <= 0f) {Objects.sm.recoilcooldown ();}


		if(gunoptions.burstMode == gun2.shootxx.mod.auto && gunoptions.burstFire && !gunoptions.semiAuto && !gunoptions.shotgun){if(Input.GetButton("Fire1")){canAutoBurst = true;}else{canAutoBurst = false;}}
		if(gunoptions.shotgunMode == gun2.shootxx.mod0.auto && gunoptions.shotgun && !gunoptions.semiAuto && !gunoptions.burstFire){if(Input.GetButton("Fire1")){canAutoShotgun = true;}else{canAutoShotgun = false;}}

		if ((!Input.GetButton("Fire1") || this.isreloading || this.gunoptions.Ammo == 0 || this.Objects.myPlayer.transform.GetComponent<PlayerShootManager>().isRunning || this.Objects.myPlayer.GetComponent<PlayerShootManager>().isPaused || this.gunoptions.semiAuto || (this.gunoptions.shotgunMode == gun2.shootxx.mod0.normal && this.gunoptions.shotgun) || (this.gunoptions.burstMode == gun2.shootxx.mod.normal && this.gunoptions.burstFire) || this.gunoptions.sniper || this.gunoptions.rpg) && centerCounter <= 0f){
			this.center = true;
		}else{
			this.center = false;
		}

		if (this.center){
			rTime = 0f;
			this.tp = Vector3.Lerp(this.tp, Vector3.zero, Time.deltaTime * 9f);
			if (target != null) {target.localPosition = Vector3.Lerp (target.localPosition, Vector3.zero, Time.deltaTime * 10f);}
		}

		this.pos = Mathf.Lerp(this.pos, 0f, Time.deltaTime * 2f);
		this.posx = Mathf.Lerp(this.posx, 0f, Time.deltaTime * 2f);


	}																																	

	void LateUpdate(){
		if(Objects.muzzleFlesh == null || Objects.muzzleLight == null ){ return;}
		if (m_LastFrameShot == Time.frameCount && !this.silencer){
			Objects.muzzleFlesh.transform.localRotation = Quaternion.AngleAxis(Random.Range(0,180), Vector3.forward);
			Objects.muzzleFlesh.enabled = true;
			Objects.muzzleLight.enabled = true;		
		}else{
			Objects.muzzleFlesh.enabled = false;
			Objects.muzzleLight.enabled = false;	
		}	
	}




	private Vector3 defpos;
	private Vector3 deflean;
	private bool hassetlean;

	public virtual void shootEmUp(){
		if (Input.GetButton("Fire2") && !this.isreloading && !this.AimbasedOnAmmo && !this.isSwaping && !this.swapTo && !((PlayerShootManager)this.Objects.myPlayer.GetComponent(typeof(PlayerShootManager))).isPaused){
			this.recoilAmount_x = this.RealRecoilxAim;
			this.recoilAmount_y = this.RealRecoilyAim;
			this.recoilAmount_z = this.recoilAmount_z_;
			if (this.Objects.adjust != null){this.Objects.adjust.localEulerAngles = new Vector3 (gunoptions.aimRotationX, 0f, 0f);}

			if (!this.isAiming){
				this.isAiming = true;
				if (this.gunoptions.sniper) {this.scopeTime = Time.time + this.RealaimSpeed;} else {this.scopeTime = 0f;}
				this.curVect = this.RealAimPosition - cur_pos;
			}
			if (Mathf.Abs(Vector3.Distance(cur_pos, this.RealAimPosition)) < this.curVect.magnitude / this.RealaimSpeed * Time.deltaTime){
				cur_pos = this.RealAimPosition;
			}else{
				cur_pos = cur_pos + this.curVect / this.RealaimSpeed * Time.deltaTime;
			}
				
			if (Time.time >= this.scopeTime && !this.inScope){this.inScope = true;}
		}
		if (!Input.GetButton("Fire2") || this.isSwaping || this.swapTo || ((PlayerShootManager)this.Objects.myPlayer.GetComponent(typeof(PlayerShootManager))).isPaused || this.isreloading){
			this.recoilAmount_x = this.RealRecoilxHip;
			this.recoilAmount_y = this.RealRecoilyHip;
			this.recoilAmount_z = this.recoilAmount_z_non;
			if (this.Objects.adjust != null){this.Objects.adjust.localEulerAngles = Vector3.zero;}
			if (this.isAiming){
				this.isAiming = false;
				this.inScope = false;
				this.curVect = this.gunoptions.aimoptions.defposition - cur_pos;
			}if (Mathf.Abs(Vector3.Distance(cur_pos, this.gunoptions.aimoptions.defposition)) < this.curVect.magnitude / this.gunoptions.aimSpeed * Time.deltaTime){
				cur_pos = this.gunoptions.aimoptions.defposition;
			}else{
				cur_pos = cur_pos + this.curVect / this.RealaimSpeed * Time.deltaTime;
			}
		}
		transform.localPosition = cur_pos;
	}

	public void RecoilMath(float centertimeAfterShot){
		this.pos -= Random.Range(-this.recoilAmount_x * 1.2f, this.recoilAmount_x * 1.8f);
		this.posx += Random.Range(-this.recoilAmount_y, this.recoilAmount_y);
		this.cv = new Vector3(Mathf.Clamp(this.pos, -60f, 0.1f), this.posx, 0f);
		centerCounter = centertimeAfterShot;
	}


	public float map(float x, float min, float max, float map_min, float map_max){
		return map_min + (x - min) * (map_max - map_min) / (max - min);
	}
		

	public void cameraZoom(){
		if(inScope){
			Objects.mainCam.fieldOfView -= defcameraView * Time.deltaTime / RealzoomSpeed;
			if (Objects.mainCam.fieldOfView < gunoptions.zoomFov){
				Objects.mainCam.fieldOfView = gunoptions.zoomFov;
			}
		}else{
			Objects.mainCam.fieldOfView += defcameraView * Time.deltaTime / RealzoomSpeed;
			if (Objects.mainCam.fieldOfView > defcameraView){
				Objects.mainCam.fieldOfView = defcameraView;
			}
		}  
	}



	public void sniperEffect(){
		if (gunoptions.sniper) {
			for(int i = 0; i < Objects.meshRenderers.Length; i++){
				if(Objects.meshRenderers[i] != null){
					Objects.meshRenderers[i].enabled = !inScope;
				}
			}
		}
	}




	public void OnGUI(){
		if (gunoptions.sniper && inScope) {
			GUI.DrawTexture (new Rect (0f, 0f, Screen.width, Screen.height), Objects.scopeTexture, ScaleMode.StretchToFill);
		}
	}





	public void typeOfFire(){
		if (counter <= 0 && gunoptions.Ammo > 0 && !isreloading && !Objects.myPlayer.GetComponent<PlayerShootManager> ().isRunning && PULLUPDURATION < 0 && !isSwaping && !swapTo && !Objects.myPlayer.GetComponent<PlayerShootManager> ().isPaused) {
			if ((!gunoptions.burstFire && !gunoptions.semiAuto && !gunoptions.shotgun && !gunoptions.sniper && !gunoptions.rpg)) {
				typeOfWeapon = "Assault";
				autoFire ();
			}
			if (gunoptions.burstFire && !gunoptions.semiAuto && !gunoptions.shotgun && !gunoptions.sniper && !gunoptions.rpg) {
				typeOfWeapon = "Burst";
				burstFire ();
			}

			if (gunoptions.semiAuto && !gunoptions.burstFire && !gunoptions.shotgun && !gunoptions.sniper && !gunoptions.rpg) {
				typeOfWeapon = "Semi";
				semiautoFire ();
			}

			if (gunoptions.shotgun && !gunoptions.semiAuto && !gunoptions.burstFire && !gunoptions.sniper && !gunoptions.rpg) {
				typeOfWeapon = "Shotgun";
				shotgunFire ();
			}	

			if (gunoptions.sniper && !gunoptions.semiAuto && !gunoptions.burstFire && !gunoptions.shotgun && !gunoptions.rpg) {
				typeOfWeapon = "Sniper";
				sniperFire ();
			}

			if (gunoptions.rpg && !gunoptions.semiAuto && !gunoptions.burstFire && !gunoptions.shotgun && !gunoptions.sniper) {
				typeOfWeapon = "Luncher";
				rpgFire ();
			}
		}
	}



	public void playShootAnim(){
		if (anim != null) {
			if (isAiming) {
				anim.Play (gunoptions.specialAimFireName, 0, 0);
				anim.speed = 1f;
			} else {
				anim.Play (gunoptions.specialHipFireName, 0, 0);
				anim.speed = 1f;
			}
		}
	}

	public void playHideAnim(){
		if (anim != null) {anim.Play (gunoptions.specialSwapOutName, 0, 0);anim.speed = AnimationSwapOutSpeed;}
	}
	public void playDrawAnim(){
		if (anim != null) {anim.Play (gunoptions.specialSwapInName, 0, 0);anim.speed = AnimationSwapInTimeSpeed;}
	}
	public void playIdleAnim(){
		if (anim != null) {anim.Play (gunoptions.specialIdleName, 0, 0); anim.speed = 1f;}
	}

	public void playAnim(string animname, float animspeed){
		if (anim != null) {anim.Play (animname, 0, 0); anim.speed = animspeed;}
	}

	public Vector2 getRandomVector(Vector2 min, Vector2 max){
		return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
	}


	public Vector3 getShootDirection(){
		Quaternion totalSpreadXY;
		Vector3 fireDirection = Objects.spawnPoint.forward;
		Quaternion fireRotation = Quaternion.LookRotation(fireDirection);
		float spreadx;
		float spready;

		if (isAiming) {
			spreadx = Random.Range (-spreadAmountAim, spreadAmountAim) * spreadMultiplier;
			spready = Random.Range (-spreadAmountAim, spreadAmountAim) * spreadMultiplier;
		} else {
			spreadx = Random.Range (-spreadAmountHip, spreadAmountHip) * spreadMultiplier;
			spready = Random.Range (-spreadAmountHip, spreadAmountHip) * spreadMultiplier;
		}
		totalSpreadXY = Quaternion.Euler (spreadx, spready, 0f);
		fireRotation *= totalSpreadXY;

		return fireRotation * Vector3.forward;
	}

	public void autoFire(){
		if((Input.GetButton("Fire1") || OneFire)){
			FireBullet(Objects.spawnPoint.transform.position,getShootDirection());
			Objects.myPlayer.GetComponent<PlayerShootManager>().spawnBulletTrail(Objects.spawnPoint.transform.position,getShootDirection(), gunoptions.maxVelocity);
			RecoilMath (0f);
			playShootAnim ();
			gunoptions.Ammo --;
			counter = 1;
			shootstarttime = Time.time;
			m_LastFrameShot = Time.frameCount;
			playfiresound();
			StartCoroutine(spawnBulletShells());
		}
		OneFire = false;
	}



	public void burstFire(){
		if((Input.GetButtonDown("Fire1") || OneFire || canAutoBurst)){
			count = gunoptions.burstAmount;
			if(count <= 0 || count > 4){count = 3;}
		}
		OneFire = false;
	}


	public void fireBurst(){
	if(count > 0){
		burstcounter -= Time.deltaTime;
		if(burstcounter < 0){
			count -= 1;
			fireingBurst();
			burstcounter = burstSpeed;
		}
	}else{burstcounter = 0f;}
	}


	public void fireingBurst(){
		if(gunoptions.Ammo > 0){
			FireBullet(Objects.spawnPoint.transform.position,getShootDirection());
			Objects.myPlayer.GetComponent<PlayerShootManager>().spawnBulletTrail(Objects.spawnPoint.transform.position,getShootDirection(), gunoptions.maxVelocity);
			RecoilMath (0.1f);
			playShootAnim ();
			gunoptions.Ammo --;
			counter = 1f;
			recoilFadeDelay = 0.1f;
			shootstarttime = Time.time;
			m_LastFrameShot = Time.frameCount;
			playfiresound();
			StartCoroutine(spawnBulletShells());
		}
	}




	public void semiautoFire(){
		if((Input.GetButtonDown("Fire1") || OneFire)){
			FireBullet(Objects.spawnPoint.transform.position,getShootDirection());
			Objects.myPlayer.GetComponent<PlayerShootManager>().spawnBulletTrail(Objects.spawnPoint.transform.position,getShootDirection(), gunoptions.maxVelocity);
			RecoilMath (0.1f);
			playShootAnim ();
			gunoptions.Ammo --;
			counter = 1;
			recoilFadeDelay = 0.1f;
			shootstarttime = Time.time;
			m_LastFrameShot = Time.frameCount;
			playfiresound();
			StartCoroutine(spawnBulletShells());				
		}
		OneFire = false;
	}


	public void shotgunFire(){
		if((Input.GetButtonDown("Fire1") || OneFire || canAutoShotgun) && !shotgunRel){
			for(int i = 0; i < numShotgunPallet; i++){
				FireBullet(Objects.spawnPoint.transform.position,getShootDirection());
				Objects.myPlayer.GetComponent<PlayerShootManager>().spawnBulletTrail(Objects.spawnPoint.transform.position,getShootDirection(), gunoptions.maxVelocity);
			}
			RecoilMath (0.3f);
			playShootAnim ();
			gunoptions.Ammo --;
			counter = 1;
			shootstarttime = Time.time;
			m_LastFrameShot = Time.frameCount;
			playfiresound();
			StartCoroutine(spawnBulletShells());
		}
		OneFire = false;
	}


	public void rpgFire(){
	if((Input.GetButtonDown("Fire1") || OneFire)){
		playShootAnim ();
		gunoptions.Ammo --;
		counter = 1f;
		playfiresound();
		RecoilMath (0.12f);
	}
	OneFire = false;
	}


	public void sniperFire(){
		if((Input.GetButtonDown("Fire1") || OneFire)){			
			FireBullet(Objects.spawnPoint.transform.position,getShootDirection());
			Objects.myPlayer.GetComponent<PlayerShootManager>().spawnBulletTrail(Objects.spawnPoint.transform.position,getShootDirection(), gunoptions.maxVelocity);
			RecoilMath (0.12f);
			playShootAnim ();
			gunoptions.Ammo --;
			counter = 1;
			shootstarttime = Time.time;
			m_LastFrameShot = Time.frameCount;
			playfiresound();
			StartCoroutine(spawnBulletShells());
		}
		OneFire = false;
	}






	public void FireBullet(Vector3 position,Vector3 direction) {
		if(Objects.bullet == null){return;}
		for(int i = 0; i < projectilesPerShot; i++) {
			GameObject b = Instantiate(Objects.bullet);
			b.transform.GetComponent<bullet>().Owner = Objects.myPlayer;
			b.transform.GetComponent<bullet>().Fire(position,direction, gunoptions.closeDamage,gunoptions.longDamage , gunoptions.headshotmultiplier,gunoptions.maxVelocity, gunoptions.weaponID , typeOfWeapon,Realrange, gunoptions.flinch);
			b.transform.GetComponent<bullet>().enabled = true;
		}
	}



	public bool isSwapingOut(){
		return isSwaping;
	}

	public bool isSwapingInto(){
		return swapTo;
	}


	public bool isgunAiming(){
		return this.isAiming;
	}

	public Vector3 realaimPosition(){
		return this.RealAimPosition;
	}


	public bool isgunReloading(){
		return this.isreloading;
	}


	public void hasSightsOn(bool sight){
		this.sightsActive = sight;
	}

	public void setOneFire(bool fir){
		OneFire = fir;
	}

	public float currentPullUpTime(){
		return this.PULLUPDURATION;
	}


	public void setAimNzoom(Vector3 aimPos, float zoomLvl){
		if(sightsActive){
			RealAimPosition = aimPos;
			RealZoomAmount = zoomLvl;
		}
	}

	public void aimNzoom(){
		if(!sightsActive){
			RealAimPosition = gunoptions.aimoptions.aimposition;
			RealZoomAmount = gunoptions.zoomFov;
		}
	}

	public void setSightRotx(float x){
		sightrotx  = x;
	}


	public IEnumerator spawnBulletShells(){
		if (Objects.BulletShell != null && Objects.bulletEjectPoint != null) {
			yield return new WaitForSeconds (0.05f);
			GameObject shell = Instantiate (Objects.BulletShell, Objects.bulletEjectPoint.position, Objects.bulletEjectPoint.rotation * Quaternion.Euler(0f,Random.Range(12f,16f),0f));
			Quaternion up = Quaternion.LookRotation(Objects.bulletEjectPoint.up);
			Quaternion right = Quaternion.LookRotation(Objects.bulletEjectPoint.right);
			up *= Quaternion.Euler(Random.Range(-0f, 0f), Random.Range(-0f, 0f), 0f);
			right *= Quaternion.Euler(Random.Range(-2f, 0f), Random.Range(-0f, 1f), 0f);
			Vector3 updir = up * Vector3.forward;
			Vector3 rightdir = right * Vector3.forward;

			shell.GetComponent<Rigidbody>().velocity = Objects.myPlayer.GetComponent<CharacterController>().velocity;
			shell.GetComponent<Rigidbody>().AddForce(updir * Random.Range(shellforce_up.x, shellforce_up.y));
			shell.GetComponent<Rigidbody>().AddForce(rightdir * Random.Range(shellforce_right.x, shellforce_right.y));
			Destroy(shell, 1f);
		}
	}

	public void playfiresound(){
	if(!silencer){
		Objects.myPlayer.GetComponent<shotfired>().shotsfired();
		Objects.myPlayer.GetComponent<shotfired>().CmdTellServertoPlayGunSound (gunoptions.sounds.firesoundID);
		Objects.weaponAudio.PlayOneShot(gunoptions.sounds.firesound);
	}else{
		Objects.weaponAudio.PlayOneShot(gunoptions.sounds.silencersound);
		Objects.myPlayer.GetComponent<shotfired>().CmdTellServertoPlayGunSound (gunoptions.sounds.supressorID);
	}
	}

	public void gunPerks(bool fastswap, bool quickdraw, bool fastmag, bool longberrel, bool grip, bool steadyaim, bool rapidfire, bool reloadgo){
		this.fastswap = fastswap;
		this.quickdraw =quickdraw; 
		this.fastmag = fastmag;
		this.longberrel=longberrel;
		this.grip = grip;
		this.steadyaim = steadyaim; 
		this.rapidfire = rapidfire;
		this.reloadongo = reloadgo;
	}


	public void fastSwaps(){
		if(fastswap){
			RealSwapOutTime = gunoptions.timeoptions.swapOutTime - (gunoptions.timeoptions.swapOutTime*0.50f);
			RealSwapInTime  = gunoptions.timeoptions.swapInTime - (gunoptions.timeoptions.swapInTime*0.50f);

			AnimationSwapOutSpeed =  gunoptions.timeoptions.swapOutTime/RealSwapOutTime;		
			AnimationSwapInTimeSpeed = gunoptions.timeoptions.swapInTime/RealSwapInTime;

		}else{
			AnimationSwapOutSpeed = 1f;
			AnimationSwapInTimeSpeed =1f;

			RealSwapOutTime = gunoptions.timeoptions.swapOutTime;
			RealSwapInTime  = gunoptions.timeoptions.swapInTime;

		}
	} 


	public void silencerAttach(bool sil){
		this.silencer = sil;
		if(sil){silencerRangeDrop = 0.15f;}else{silencerRangeDrop = 0f;}
	}


	public void gripAttach(){
		if(grip){
			gripMultiplier = 0.25f;
			float num = 0.5f;
			float num2 = 0.15f;
			this.RealRecoilxAim = this.recoilAmount_x_ - this.recoilAmount_x_ * num;
			this.RealRecoilyAim = this.recoilAmount_y_ - this.recoilAmount_y_ * num2;
		}else{
			gripMultiplier = 0f;
			this.RealRecoilxAim = this.recoilAmount_x_;
			this.RealRecoilyAim = this.recoilAmount_y_;
		}
	}



	public void hipaimAttah(){
		if(steadyaim){
			spreadMultiplier = spreadMultiplier = 0.50f;
			this.RealRecoilxHip = this.recoilAmount_x_non - (this.recoilAmount_x_non * 0.5f);
			this.RealRecoilyHip = this.recoilAmount_y_non - (this.recoilAmount_y_non * 0.5f);
		}else{
			spreadMultiplier = spreadMultiplier = 1f;
			this.RealRecoilxHip = this.recoilAmount_x_non;
			this.RealRecoilyHip = this.recoilAmount_y_non;
		}
	}




	public void quickdrawAttach(){
		if(quickdraw){
			RealaimSpeed = gunoptions.aimSpeed - (gunoptions.aimSpeed * 0.355f);
			RealzoomSpeed = gunoptions.zoomSpeed - (gunoptions.zoomSpeed * 0.355f);
			RealpullupDuration = gunoptions.pullUpDuration - (gunoptions.pullUpDuration * 0.305f);
		}else{
			RealaimSpeed = gunoptions.aimSpeed;
			RealzoomSpeed = gunoptions.zoomSpeed;
			RealpullupDuration = gunoptions.pullUpDuration;
		}
	}




	public void rapidfireAttach(){
		if(rapidfire){
			RealfireSpeed = gunoptions.firespeed + (gunoptions.firespeed * 0.10f);
		}else{
			RealfireSpeed = gunoptions.firespeed;
		}
	}




	public void longberrelAttach(){
		float cr = 0;
		if(longberrel){
			cr = gunoptions.range + (gunoptions.range * 0.50f);
		}else{
			cr = gunoptions.range;
		}
		Realrange = cr - (cr * silencerRangeDrop);
	}


	public void increaseAmmo(int amt){
		gunoptions.Ammo += amt;
		gunoptions.clipSize+=amt;
	}

	public void resetMagazine(){
		gunoptions.Ammo = gunoptions.clipSize;
		gunoptions.leftoverAmmo = gunoptions.maxleftoverAmmo;
	}

	public void refilAmmo(){
		gunoptions.leftoverAmmo = gunoptions.maxleftoverAmmo;
	}

	public void setAmmo(int ammo, int leftoverammo){
		gunoptions.Ammo = ammo;
		gunoptions.leftoverAmmo = leftoverammo;
	}

	public void aimAmmoCount(){
		if(gunoptions.Ammo == 0 && gunoptions.leftoverAmmo > 0){AimbasedOnAmmo = true;}else{AimbasedOnAmmo = false;}
	}

	public float weaponPullTransTime(){
		return RealpullupDuration;
	}

	public void pullUpAfterRun(){
		this.PULLUPDURATION = RealpullupDuration;
	}

	public float weaponPullDuration(){
		return this.PULLUPDURATION;
	}




	public void fastreloadAttach(){
		if(fastmag){
			float relpercent = 0.65f; 
			RealReloadTime = gunoptions.timeoptions.reloadTime - (gunoptions.timeoptions.reloadTime*relpercent);  //before 0.50f firty percent increase
			AnimationSpeed = gunoptions.timeoptions.reloadTime/RealReloadTime;
			Realreloadsound1Time = RealReloadTime - (gunoptions.timeoptions.reloadsound1Time - (gunoptions.timeoptions.reloadsound1Time*relpercent));
			Realreloadsound2Time = RealReloadTime - (gunoptions.timeoptions.reloadsound2Time - (gunoptions.timeoptions.reloadsound2Time*relpercent));
			Realreloadsound3Time = RealReloadTime - (gunoptions.timeoptions.reloadsound3Time - (gunoptions.timeoptions.reloadsound3Time*relpercent));


			RealfirstanimTime = firstanimTime - (firstanimTime * relpercent);
			RealsecondanimTime = secondanimTime - (secondanimTime * relpercent);
			RealthirdanimTime = thirdanimTime - (thirdanimTime * relpercent);

			AnimationfirstSpeed = firstanimTime/RealfirstanimTime;
			AnimationsecondSpeed = secondanimTime/RealsecondanimTime;
			AnimationthirdSpeed = thirdanimTime/RealthirdanimTime;


		}else{
			RealReloadTime = gunoptions.timeoptions.reloadTime;
			AnimationSpeed = 1f;
			Realreloadsound1Time = gunoptions.timeoptions.reloadTime - gunoptions.timeoptions.reloadsound1Time;
			Realreloadsound2Time = gunoptions.timeoptions.reloadTime - gunoptions.timeoptions.reloadsound2Time;
			Realreloadsound3Time = gunoptions.timeoptions.reloadTime - gunoptions.timeoptions.reloadsound3Time;

			RealfirstanimTime = firstanimTime;
			RealsecondanimTime = secondanimTime;
			RealthirdanimTime = thirdanimTime;

			AnimationfirstSpeed = 1f;
			AnimationsecondSpeed = 1f;
			AnimationthirdSpeed = 1f;
		}
	}

	public void swapingOut(){
		if((Input.GetKeyDown("c") || Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0) && !Objects.myPlayer.GetComponent<PlayerShootManager>().isPaused ){
			fastSwaps();
			if(!isSwaping){
				isSwaping = true;
				isreloading = false;
				isAiming = false;
				autoRel = false;
				swapTo = false;
				playedreloadsound1=false;
				playedreloadsound2=false;
				playedreloadsound3=false;
				zPosRecNext =0f;
				zPosRec =0f;
				swapCounter = RealSwapOutTime;
				playHideAnim ();
			}
		}
	}

	public void swapouting(){
		if(isSwaping){
			swapCounter -= Time.deltaTime;
			if(swapCounter < 0){
				inScope = false;
				CancelAll = true;
				counter = 0;
				transform.localEulerAngles = new Vector3 (0,0,0);
				Objects.mainCam.fieldOfView = Objects.myPlayer.GetComponent<PlayerShootManager>().defaultcameraFov;;
				transform.localPosition = gunoptions.aimoptions.defposition;
				this.gameObject.SetActive(false);
				Objects.myPlayer.GetComponent<PlayerShootManager>().changeWeapon();
			}
		}
	}	


	public void swapInto(){
	fastSwaps();
	CancelAll = true;
	swapTo = true;
	inScope = false;
	isAiming = false;
	counter = 0;
	zPosRecNext =0f;
	zPosRec =0f;
	isSwaping = false;
	isreloading = false;
	autoRel = false;
	playedreloadsound1=false;
	playedreloadsound2=false;
	playedreloadsound3=false;
	swapToCounter = RealSwapInTime;
	Objects.mainCam.fieldOfView = Objects.myPlayer.GetComponent<PlayerShootManager>().defaultcameraFov;
	transform.localPosition = gunoptions.aimoptions.defposition;
	this.gameObject.SetActive(true);
	playDrawAnim ();
	}


	public void swapIntoing(){
		if(swapTo){
			swapToCounter -= Time.deltaTime;
			if(swapToCounter < 0f){
				swapTo = false;
			}
		}
	}


	public void reloadCancel(){
		pullUpAfterRun();
		if (reloadongo) {return;}
		realoadCancelBonus();
		isreloading = false;
		autoRel = false;
		playedreloadsound1=false;
		playedreloadsound2=false;
		playedreloadsound3=false;
		shotgunRel = false;
		firstanim = false;
		secondanim = false;
		thirdanim = false;
		if(!CancelAll){
			CancelAll = true;
			isSwaping = false;
			swapTo = false;
			playIdleAnim ();
		}
	}


	public void CancelAllAnimations(){
		counter = 0;
		isreloading = false;
		autoRel = false;
		isSwaping = false;
		swapTo = false;
		playedreloadsound1=false;
		playedreloadsound2=false;
		playedreloadsound3=false;
		shotgunRel = false;
		firstanim = false;
		secondanim = false;
		thirdanim = false;
		playIdleAnim ();
	}


	public void realoadCancelBonus(){
		if(isreloading){
			if(autoReloadCounter <= (RealReloadTime - (RealReloadTime * 0.85f)) && !gunoptions.shotgun){
				realReaload();
			}else{
				Objects.weaponAudio.Stop();
			}
		}
	}


	public void autoReload(){
		if(autoRel == true){
			autoReloadCounter -= Time.deltaTime;

			if(autoReloadCounter <= Realreloadsound1Time && !playedreloadsound1 && gunoptions.sounds.reloadsound1 != null){Objects.weaponAudio.PlayOneShot(gunoptions.sounds.reloadsound1); playedreloadsound1 = true;}
			if(autoReloadCounter <= Realreloadsound2Time && !playedreloadsound2 && gunoptions.sounds.reloadsound2 != null){Objects.weaponAudio.PlayOneShot(gunoptions.sounds.reloadsound2); playedreloadsound2 = true;}
			if(autoReloadCounter <= Realreloadsound3Time && !playedreloadsound3 && gunoptions.sounds.reloadsound3 != null){Objects.weaponAudio.PlayOneShot(gunoptions.sounds.reloadsound3); playedreloadsound3 = true;}

			if(autoReloadCounter < 0){
				playedreloadsound1=false;
				playedreloadsound2=false;
				playedreloadsound3=false;
				realReaload();
				autoRel = false;
				isreloading = false;
			}
		}		
	}



	public void realReaload(){
		int takeAmmo = 0;
		if(gunoptions.Ammo < gunoptions.clipSize){
			takeAmmo = gunoptions.clipSize - gunoptions.Ammo;
		}
		if(gunoptions.leftoverAmmo >= takeAmmo){
			gunoptions.Ammo += takeAmmo;
			gunoptions.leftoverAmmo = gunoptions.leftoverAmmo-takeAmmo;
		}else{
			gunoptions.Ammo += gunoptions.leftoverAmmo;
			gunoptions.leftoverAmmo=0;
		}
	}



	public void computerautoReload(){
		if((((Input.GetKey("r") || Input.GetButton("Reload")) && gunoptions.Ammo < gunoptions.clipSize && gunoptions.leftoverAmmo != 0 && !Objects.myPlayer.GetComponent<PlayerShootManager>().isPaused) || (gunoptions.Ammo <= 0 && gunoptions.leftoverAmmo > 0)) && !isreloading && !autoRel && !shotgunRel && (!Objects.myPlayer.transform.GetComponent<PlayerShootManager>().isRunning || reloadongo) && !swapTo && !isSwaping && count <= 0){
			if(!gunoptions.shotgun){
				if(gunoptions.Ammo > 0){
					playAnim (gunoptions.specialReloadName, AnimationSpeed);
				}else{
					playAnim (gunoptions.specialReloadFullName, AnimationSpeed);
				}
				autoRel = true;
				isreloading = true;
				autoReloadCounter = RealReloadTime;
			}else{
				shotgunRel = true;
				isreloading = true;
				loopTimer = RealfirstanimTime;
				playAnim ("StartReload", AnimationfirstSpeed);
			}
		}
	}






	public void autoshotgunReload(){
		if(shotgunRel == true){

			loopTimer -= Time.deltaTime;
			if(loopTimer <= Realreloadsound1Time && firstanim && !secondanim && !playedreloadsound1 && gunoptions.sounds.reloadsound1 != null){Objects.weaponAudio.PlayOneShot(gunoptions.sounds.reloadsound1);playedreloadsound1=true;}
			if(loopTimer <= Realreloadsound2Time && secondanim && firstanim && !playedreloadsound2 && gunoptions.sounds.reloadsound2 != null){Objects.weaponAudio.PlayOneShot(gunoptions.sounds.reloadsound2); playedreloadsound2=true;}

			if(loopTimer < 0 && !firstanim){ 
				firstanim = true;  
				loopTimer = RealsecondanimTime;
				playAnim ("Insert", AnimationsecondSpeed);
				//if(animations.weaponAnimation != null){animations.weaponAnimation.Play("ShellInLoop"); animations.weaponAnimation["ShellInLoop"].speed = AnimationsecondSpeed;}
			}

			if(loopTimer < 0 && firstanim && !secondanim){
				if(gunoptions.leftoverAmmo > 0 && gunoptions.Ammo < gunoptions.clipSize){
					gunoptions.Ammo += 1;
					gunoptions.leftoverAmmo -= 1;
					if(gunoptions.leftoverAmmo <= 0 || gunoptions.Ammo == gunoptions.clipSize){
						secondanim = true;
						loopTimer = RealthirdanimTime;
						playAnim ("AfterReload", AnimationthirdSpeed);
					}else{
						loopTimer = RealsecondanimTime;
						playAnim ("Insert", AnimationsecondSpeed);
						playedreloadsound1=false;
					}
				}
			}

			if(loopTimer < 0 && secondanim && firstanim && !thirdanim){
				thirdanim = true;
				isreloading = false;
				shotgunRel = false;
				firstanim = false;
				secondanim = false;
				thirdanim = false;
				playedreloadsound1=false;
				playedreloadsound2=false;
			}
		}
	}

	public void resetAll(){
		CancelAll = true;
		inScope = false;
		counter = 0;
		zPosRecNext =0f;
		zPosRec =0f;
		isSwaping = false;
		isreloading = false;
		autoRel = false;
		playedreloadsound1=false;
		playedreloadsound2=false;
		playedreloadsound3=false;
		Objects.mainCam.fieldOfView = Objects.myPlayer.GetComponent<PlayerShootManager>().defaultcameraFov;
		transform.localPosition = gunoptions.aimoptions.defposition;
	}



	public void runEffect(){
		if(Objects.runObject == null){return;}
		if(Objects.myPlayer.transform.GetComponent<PlayerShootManager>().isRunning){
			if(run == false){
				run = true;
				curVect_gun = gunoptions.runoptions.runrotation - cur_gun;
				curVect_gun_pos = gunoptions.runoptions.runposition - cur_gun_pos;
			}

			if(Mathf.Abs(Vector3.Distance(cur_gun_pos, gunoptions.runoptions.runposition)) < curVect_gun_pos.magnitude / gunoptions.pulldownDuration * Time.deltaTime){
				cur_gun_pos = gunoptions.runoptions.runposition;
			}else{
				cur_gun_pos += curVect_gun_pos / gunoptions.pulldownDuration * Time.deltaTime;
			}

			if(Mathf.Abs(Vector3.Distance(cur_gun, gunoptions.runoptions.runrotation)) < curVect_gun.magnitude / gunoptions.pulldownDuration * Time.deltaTime){
				cur_gun = gunoptions.runoptions.runrotation;
			}else{
				cur_gun += curVect_gun / gunoptions.pulldownDuration * Time.deltaTime;
			}

		}else{
			if(run == true){
				run = false;
				curVect_gun = Vector3.zero - cur_gun;
				curVect_gun_pos = Vector3.zero - cur_gun_pos;
			}
			if(Mathf.Abs(Vector3.Distance(cur_gun_pos, Vector3.zero)) < curVect_gun_pos.magnitude / RealpullupDuration * Time.deltaTime){
				cur_gun_pos = Vector3.zero;
			}else{
				cur_gun_pos += curVect_gun_pos / RealpullupDuration * Time.deltaTime;
			}

			if(Mathf.Abs(Vector3.Distance(cur_gun, Vector3.zero)) < curVect_gun.magnitude / RealpullupDuration * Time.deltaTime){
				cur_gun = Vector3.zero;
			}else{
				cur_gun += curVect_gun / RealpullupDuration * Time.deltaTime;
			}
		}

		Objects.runObject.localEulerAngles = cur_gun;
		Objects.runObject.localPosition = cur_gun_pos;
	}

}








