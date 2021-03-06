using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {
	public GameObject Owner;
	public Transform mTrans;
	public LayerMask layerMask;
	private Vector3 mInitialPosition;
	private Vector3 mDirection;
	private Vector3 mGravity= new Vector3(0f, -9.8f, 0f);
	private float mVelocity;
	private Vector3 mCurrentVelocity;
	private RaycastHit hit;
	private RaycastHit hit2;
	private int mcloseDamage;
	private int mlongDamage;
	private bool mResetInvoked;
	private int mweaponID;
	private string mweaponType;
	private string mOwnerRank;
	private string mNetID;
	private string mTeam;
	private float range;
	private float gunFlinch;
	private bool isTeamGame;
	private bool hasfmgPerk;
	private bool hasReconPerk;
	private float mheadshotmultiplier =  1.0f;
	private float mweaponheadshotmultiplier =  1.0f;
	private int totalnumPenetration = 2;   
	private int bPenetrationCount = 0;
	private GameObject objHitFront = null;
	private GameObject objHitBack = null;
	private int impactReduction = 0;
	private string bodyhit_type = "";
	void  Awake(){
		this.mTrans = this.transform;
		Destroy(this.gameObject,5f);
	}


	public void Fire(Vector3 position,Vector3 direction, int closedamage, int longdamage, float headmultiplier, float velocity, int weaponID, string weaponType, float range, float flinchamt){
		PlayerShootManager psm = Owner.GetComponent<PlayerShootManager> ();
		if (psm.fmg) {
			totalnumPenetration++;
			hasfmgPerk = true;
		}

		this.mVelocity = velocity;
		this.mInitialPosition = position;
		this.mDirection = direction;
		this.mTrans.position = position;
		this.mCurrentVelocity = direction * mVelocity;
		this.mcloseDamage = closedamage;
		this.mlongDamage = longdamage;
		this.mweaponheadshotmultiplier = headmultiplier;
		this.range = range;
		this.gunFlinch = flinchamt;
		this.mweaponID = weaponID;
		this.mweaponType = weaponType;
		this.isTeamGame = Owner.GetComponent<GamemodeManager>().isTeamGame;
		this.mTeam =  Owner.GetComponent<GamemodeManager>().team;
		this.mNetID = Owner.GetComponent<GamemodeManager>().myNetID;
		this.hasReconPerk = Owner.GetComponent<PlayerShootManager>().recon;
		this.mResetInvoked = false;
	}

	void Update(){
		if (mResetInvoked){
			return;
		}

		this.mCurrentVelocity += mGravity * Time.deltaTime;				
		Vector3 vector = this.mCurrentVelocity * Time.deltaTime;
		float magnitude  = vector.magnitude;
		this.mTrans.rotation = Quaternion.LookRotation(mCurrentVelocity);
		if (Physics.Raycast(mTrans.position, vector, out hit, magnitude, layerMask) ){
			if(this.bPenetrationCount < totalnumPenetration){
				this.transform.position = (hit.point + (hit.normal * -0.0001f));

				if (hit.collider.transform.tag == "BodyPart") {
					if (hit.collider.transform.GetComponent<playerbodyScript> () != null) {
						if (hit.collider.transform.GetComponent<playerbodyScript> ().Owner != objHitFront) {
							OnHit (hit, vector.normalized);
							objHitFront = hit.collider.transform.GetComponent<playerbodyScript> ().Owner;
						}
					}
				
				} else {
					if (hit.transform.gameObject != objHitFront) { 
						OnHit (hit, vector.normalized); 
						objHitFront = hit.transform.gameObject;
					} 
				}
		
			}else{
				mResetInvoked = true;    
			}
		}else{
			Transform expr_156 = this.mTrans;
			expr_156.position = expr_156.position + vector;
		}

		if (Physics.Raycast(mTrans.position, -vector, out hit2, magnitude, layerMask)){
			if ((hit2.collider.transform.tag == "wall" || hit2.collider.transform.tag == "Metal") && bPenetrationCount < totalnumPenetration){
				if(hit2.transform.gameObject != objHitBack){Owner.transform.GetComponent<PlayerShootManager>().SpawnbulletHole(hit2.point ,hit2.normal, hit2.transform); objHitBack=hit2.transform.gameObject;}
			}
		}

	}



	void  OnHit(RaycastHit hit, Vector3 direction){
		if(hit.transform.gameObject == Owner){return;}


		if (hit.collider.transform.tag == "wall" || hit.collider.transform.tag == "Metal" || hit.collider.transform.tag == "Wood" ){
			Owner.transform.GetComponent<PlayerShootManager>().CmdTellServertoSpawnbulletHole(hit.point ,hit.normal);
			Owner.transform.GetComponent<PlayerShootManager>().CmdtellServerToSpawndustParticles(hit.point);
			this.bPenetrationCount ++;
			return;
		}

		if (hit.collider.transform.tag == "block" ||  hit.collider.transform.tag == "Ground" || hit.collider.transform.tag == "Rock"){
			Owner.transform.GetComponent<PlayerShootManager>().CmdTellServertoSpawnbulletHole(hit.point ,hit.normal);
			Owner.transform.GetComponent<PlayerShootManager>().CmdtellServerToSpawndustParticles(hit.point);
			this.bPenetrationCount = this.totalnumPenetration;
			return;
		}

		if (hit.collider.transform.tag == "Equipment" ){
			Owner.transform.GetComponent<PlayerShootManager>().CmdtellServerToSpawndustParticles(hit.point);
			if (!isTeamGame) {
				Owner.transform.GetComponent<PlayerShootManager>().showHitMarker();
				Owner.transform.GetComponent<PlayerShootManager> ().CmdTellServerToDmgNearbyStuff (hit.transform.gameObject, mcloseDamage);
			} else {
				if (hit.transform.gameObject.GetComponent<NetworkEquipments> ().ownerNetID == mNetID) {
					Owner.transform.GetComponent<PlayerShootManager>().showHitMarker();
					Owner.transform.GetComponent<PlayerShootManager> ().CmdTellServerToDmgNearbyStuff (hit.transform.gameObject, mcloseDamage);
				}
				if (hit.transform.gameObject.GetComponent<NetworkEquipments> ().equipmentTeam != mTeam) {
					Owner.transform.GetComponent<PlayerShootManager>().showHitMarker();
					Owner.transform.GetComponent<PlayerShootManager> ().CmdTellServerToDmgNearbyStuff (hit.transform.gameObject, mcloseDamage);
				}
			}
			return;
		}



		if (hit.collider.transform.tag == "body" ||  hit.collider.transform.tag == "leg" || hit.collider.transform.tag == "head"){
			Owner.transform.GetComponent<PlayerShootManager>().CmdTellServertoSpawnbulletHole(hit.point ,hit.normal);
			Owner.transform.GetComponent<PlayerShootManager>().CmdtellServerToSpawndustParticles(hit.point);
			Owner.transform.GetComponent<PlayerShootManager>().showHitMarker();
			this.bPenetrationCount = this.totalnumPenetration;
			return;
		}
			

		if (hit.collider.transform.tag == "BodyPart") {
			if (hit.collider.transform.GetComponent<playerbodyScript> () != null) {
				switch (hit.collider.transform.GetComponent<playerbodyScript> ().body_type) {
				case("body"):
					bodyhit_type = "body";
					break;
				case("arm"):
					bodyhit_type = "arm";
					break;
				case("head"):
					bodyhit_type = "head";
					break;
				case("leg"):
					bodyhit_type = "leg";
					break;
				}
				dealdamage (hit.collider.transform.GetComponent<playerbodyScript> ().Owner);
				this.bPenetrationCount ++;
				return;
			}
		}
			
	this.mResetInvoked = true;
	}








	void dealdamage(GameObject player){
		if(bPenetrationCount > 0 && !hasfmgPerk){impactReduction = 2;}else{impactReduction = 0;}
		if (bodyhit_type != "head") {mheadshotmultiplier = 1f;}else{mheadshotmultiplier = mweaponheadshotmultiplier;}


		if(isTeamGame){
			if(player.GetComponent<GamemodeManager>().team != mTeam && player.GetComponent<myPlayer>().Health > 0){
				if (bodyhit_type == "head") {Owner.transform.GetComponent<PlayerShootManager> ().showheadshotHitMarker ();} else {Owner.transform.GetComponent<PlayerShootManager> ().showHitMarker ();}
				if(hasReconPerk){ Owner.GetComponent<PlayerShootManager>().CmdsetRecon(player);}
				if((player.transform.position- Owner.transform.position).magnitude >= range){
					Owner.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(player,(int)(mlongDamage * mheadshotmultiplier ) - impactReduction, Owner, mweaponID , bodyhit_type , mweaponType, Owner.transform.position, gunFlinch);	
				}else{
					Owner.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(player,(int)(mcloseDamage * mheadshotmultiplier ) - impactReduction, Owner, mweaponID  , bodyhit_type , mweaponType, Owner.transform.position, gunFlinch);	
				}	
			}
		}
			
		if(!isTeamGame && player.GetComponent<myPlayer>().Health > 0){
			if (bodyhit_type == "head") {Owner.transform.GetComponent<PlayerShootManager> ().showheadshotHitMarker ();} else {Owner.transform.GetComponent<PlayerShootManager> ().showHitMarker ();}
			if(hasReconPerk){player.GetComponent<shotfired>().dealRecon();}
			if((player.transform.position - Owner.transform.position).magnitude >= range){
				Owner.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(player, (int)(mlongDamage * mheadshotmultiplier ) - impactReduction, Owner, mweaponID  , bodyhit_type , mweaponType, Owner.transform.position, gunFlinch);	
			}else{
				Owner.transform.GetComponent<PlayerShootManager>().CmdTellServerToreducehealth(player, (int)(mcloseDamage * mheadshotmultiplier ) - impactReduction, Owner, mweaponID  , bodyhit_type , mweaponType, Owner.transform.position, gunFlinch);	
			}
		}
	}
		
		
}


