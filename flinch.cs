using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class flinch : NetworkBehaviour {
	[SyncVar (hook = "OnFlinch")]
	public float flincher = 0f;
	[SyncVar (hook = "OnShake")]
	public int shaker;
	[SyncVar(hook = "OnShowDirection") ]
	public Vector3 enemyPosition;
	public hudManager hudmanager;
	private float fposx = 0f;
	private float fposz = 0f;
	private float maxFlinchAllowedCam;
	private float maxNoFocusFlinch = -12f;
	private float maxFocusFlinch = -2.0f;
	public float testFlinch = 10f;
	public float rate = 11.5f;
	private float flinchDecressRate = 12.5f;
	private Quaternion target = Quaternion.Euler(0f,0f,0f);
	private int shaketype = 1;
	public Transform flinchCam;
	public Transform testShakeObj;
	public Transform testShakeObj2;
	private float counter;
	private float shakeDuration = 0.5f;
	private float RealflinchAmount;
	private int RealShakeAmount;

	[System.Serializable]
	public class camshak{
		public 	float maxRotationX=0.1f;
		public 	float minRotationX=0.1f;
		public 	float maxRotationY=0.98f;
		public 	float minRotationY=0.8f;
		public 	float maxRotationZ=7.1f;
		public 	float minRotationZ=6.76f;
		public 	float shakeincreaserate= 3f;
		public 	float shakedecreaserate= 6f;
	}
	public camshak cameraShakeOptions = new camshak();

	[System.Serializable]
	public class wpshak{
		public 	float maxRotationX=0.1f;
		public 	float minRotationX=0.1f;
		public 	float maxRotationY=0.98f;
		public 	float minRotationY=0.8f;
		public 	float maxRotationZ=7.1f;
		public 	float minRotationZ=6.76f;
		public 	float shakeincreaserate = 3f;
		public 	float shakedecreaserate = 6f;
	}
	public wpshak weaponShakeOptions = new wpshak();

	void Start(){
		if(isLocalPlayer){
			hudmanager = GameObject.Find ("Canvas").GetComponent<canvasManager> ().hud;
		}
	}
		

	void Update(){
		if (isLocalPlayer) {
			focusAttachment (false);

			if (Input.GetKeyDown ("b")) {
				//flinching (testFlinch, transform.position + (transform.forward * 2));
			}

			if (fposx < 0) {fposx += Time.deltaTime * flinchDecressRate;}
			if (fposz < 0) {fposz += Time.deltaTime * flinchDecressRate;}
			if (fposx > 0) {fposx = 0;}
			if (fposz > 0) {fposz = 0;}

			target = Quaternion.Euler (fposx, 0, fposz);
			flinchCam.localRotation = Quaternion.Slerp (flinchCam.localRotation, target, Time.deltaTime * 30f);

			counter -= Time.deltaTime;
			if (counter < 0) {
				testShakeObj.localRotation = Quaternion.Slerp (testShakeObj.localRotation, Quaternion.Euler (0, 0, 0), Time.deltaTime * cameraShakeOptions.shakedecreaserate);
				testShakeObj2.localRotation = Quaternion.Slerp (testShakeObj2.localRotation, Quaternion.Euler (0, 0, 0), Time.deltaTime * weaponShakeOptions.shakedecreaserate);
			}
			if (counter > 0) {	
				testShakeObj.localRotation = Quaternion.Slerp (testShakeObj.localRotation, Quaternion.Euler (Random.Range (-cameraShakeOptions.minRotationX, cameraShakeOptions.maxRotationX), Random.Range (-cameraShakeOptions.minRotationY, cameraShakeOptions.maxRotationY), Random.Range (-cameraShakeOptions.minRotationZ, cameraShakeOptions.maxRotationZ)), Time.deltaTime * cameraShakeOptions.shakeincreaserate);
				testShakeObj2.localRotation = Quaternion.Slerp (testShakeObj2.localRotation, Quaternion.Euler (Random.Range (-weaponShakeOptions.minRotationX, weaponShakeOptions.maxRotationX), Random.Range (-weaponShakeOptions.minRotationY, weaponShakeOptions.maxRotationY), Random.Range (-weaponShakeOptions.minRotationZ, weaponShakeOptions.maxRotationZ)), Time.deltaTime * weaponShakeOptions.shakeincreaserate);
			}
		}
	}






	public void OnFlinch(float flch){
		RealflinchAmount = flch - flincher;
		flincher = flch;
	}

	public void OnShowDirection(Vector3 x){
		enemyPosition = x;
		Vector3 pos = x;
		if (isLocalPlayer) {
			if (enemyPosition.x != 0f && enemyPosition.y != 0f && enemyPosition.z != 0f) {
				flinching(RealflinchAmount, pos);
				hudmanager.showDamageDirection(pos);
				Cmdreverse (this.gameObject);
			}
		}
	}


	[Command]
	public void Cmdreverse(GameObject myPlayer){
		myPlayer.transform.GetComponent<flinch>().enemyPosition = new Vector3(0f,0f,0f);
	}







	public void flinching(float flinchAmount, Vector3 hitfromposition){
		Vector3 Direction = hitfromposition - transform.position;
		float AngleFromMe =  Vector3.Angle(transform.forward, Direction);
		float Angle = AngleFromMe;


		if(Angle <= 53f){
			fposx -= flinchAmount;
			if(fposx < maxFlinchAllowedCam){fposx = maxFlinchAllowedCam;}
		}

		if(Angle > 53f && Angle <= 115f){
			fposz -= flinchAmount;
			if(fposz < maxFlinchAllowedCam){fposz = maxFlinchAllowedCam;}
		}

		if(Angle > 115f){
			fposz -= 1.7f;
			if(fposz < maxFlinchAllowedCam){fposz = maxFlinchAllowedCam;}
		}

	}




	public void focusAttachment(bool focus){
		if(focus){
			maxFlinchAllowedCam = maxFocusFlinch;
		}else{
			maxFlinchAllowedCam = maxNoFocusFlinch;
		}
	}



	public void OnShake(int x){
		RealShakeAmount = x - shaker;
		shaker = x;
		if(isLocalPlayer){
			shakeItUp (RealShakeAmount, 1f);
		}
	}





	public void shakeItUp(int st, float durution){
		counter = durution;
		//default shake
		if (shaketype == 1) {
			cameraShakeOptions.minRotationX = 2f;
			cameraShakeOptions.maxRotationX = 2f;
			cameraShakeOptions.minRotationY = 1f;
			cameraShakeOptions.maxRotationY = 1f;
			cameraShakeOptions.minRotationZ = 6.2f;
			cameraShakeOptions.maxRotationZ = 6.1f;
			cameraShakeOptions.shakeincreaserate = 9f; 
			cameraShakeOptions.shakedecreaserate = 6f;

			weaponShakeOptions.minRotationX = 0.2f;
			weaponShakeOptions.maxRotationX = 0.2f;
			weaponShakeOptions.minRotationY = 0.98f;
			weaponShakeOptions.maxRotationY = 0.8f;
			weaponShakeOptions.minRotationZ = 7.1f;
			weaponShakeOptions.maxRotationZ = 6.76f;
			weaponShakeOptions.shakeincreaserate = 3f;
			weaponShakeOptions.shakedecreaserate = 6f;
		}

		//c4 shake
		if(shaketype == 2){
			cameraShakeOptions.minRotationX = 3f;
			cameraShakeOptions.maxRotationX =3f;
			cameraShakeOptions.minRotationY =1f;
			cameraShakeOptions.maxRotationY =1f;
			cameraShakeOptions.minRotationZ =6.2f;
			cameraShakeOptions.maxRotationZ =6.1f;
			cameraShakeOptions.shakeincreaserate =9f; 
			cameraShakeOptions.shakedecreaserate =6f;

			weaponShakeOptions.minRotationX =0.6f;
			weaponShakeOptions.maxRotationX =0.6f;
			weaponShakeOptions.minRotationY =0.9f;
			weaponShakeOptions.maxRotationY =0.8f;
			weaponShakeOptions.minRotationZ =7.1f;
			weaponShakeOptions.maxRotationZ =6.76f;
			weaponShakeOptions.shakeincreaserate = 9f;
			weaponShakeOptions.shakedecreaserate = 6f;
		}

		//semtex shake
		if(shaketype == 3){
			cameraShakeOptions.minRotationX = 4.5f;
			cameraShakeOptions.maxRotationX =4.5f;
			cameraShakeOptions.minRotationY =1.8f;
			cameraShakeOptions.maxRotationY =1.3f;
			cameraShakeOptions.minRotationZ =7.5f;
			cameraShakeOptions.maxRotationZ =6.9f;
			cameraShakeOptions.shakeincreaserate =10.1f; 
			cameraShakeOptions.shakedecreaserate =6f;

			weaponShakeOptions.minRotationX =0.8f;
			weaponShakeOptions.maxRotationX =0.8f;
			weaponShakeOptions.minRotationY =0.99f;
			weaponShakeOptions.maxRotationY =0.81f;
			weaponShakeOptions.minRotationZ =7.7f;
			weaponShakeOptions.maxRotationZ =6.76f;
			weaponShakeOptions.shakeincreaserate = 8f;
			weaponShakeOptions.shakedecreaserate = 6f;
		}


		//grenade shake
		if(shaketype == 4){
			cameraShakeOptions.minRotationX = 3.5f;
			cameraShakeOptions.maxRotationX =3.5f;
			cameraShakeOptions.minRotationY =1f;
			cameraShakeOptions.maxRotationY =1f;
			cameraShakeOptions.minRotationZ =6.5f;
			cameraShakeOptions.maxRotationZ =6.8f;
			cameraShakeOptions.shakeincreaserate =10f; 
			cameraShakeOptions.shakedecreaserate =6.1f;

			weaponShakeOptions.minRotationX =0.7f;
			weaponShakeOptions.maxRotationX =0.7f;
			weaponShakeOptions.minRotationY =0.92f;
			weaponShakeOptions.maxRotationY =0.82f;
			weaponShakeOptions.minRotationZ =7.1f;
			weaponShakeOptions.maxRotationZ =6.71f;
			weaponShakeOptions.shakeincreaserate = 5f;
			weaponShakeOptions.shakedecreaserate = 6f;
		}

	}
















}
