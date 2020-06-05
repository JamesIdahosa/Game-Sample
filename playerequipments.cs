using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class playerequipments : NetworkBehaviour {

	public GameObject grenadePrefab;
	public GameObject semtexPrefab;
	public GameObject flashbangPrefab;
	public GameObject c4Prefab;
	public GameObject clayminePrefab;
	public GameObject knifePrefab;
	public float nadethrowforce = 1500f;
	public float semtexthrowforce = 1500f;
	public float c4throwforce = 1500f;
	public float knifethrowforce = 1500f;

	public enum equim{
		grenade,
		semtex,
		flashbang,
		c4,
		claymine,
		knife	
	};

	public equim equipmentType = equim.grenade;
	public GameObject weaponHolder;
	public Animation equipmentanimation;
	public Transform bodyrotator;
	public int numEquipment = 2;
	public float animationTime = 1f;
	public float pointTime = 0.30f;
	private float useTimer;
	private bool isAnimating;
	private bool forceUse;
	private float throwincrease;
	private bool isHolding;

	void Start(){
		if(isLocalPlayer){
			loadequipments ();
		}
	}
	void Update () {
		if (isLocalPlayer) {
			if (Input.GetKey ("f") && !isAnimating && numEquipment > 0 && !GetComponent<parkour2> ().climb && !GetComponent<PlayerShootManager> ().isMeleeing && useTimer <= 0) {
				hold ();
			}

			if (isHolding && GetComponent<PlayerShootManager> ().isPaused) {
				forceUse = true;
			}
			useTimer -= Time.deltaTime;

			if (useTimer <= 0f && isAnimating) {
				releasehold ();
			}

			if ((Input.GetKeyUp ("f") && numEquipment > 0 && !isAnimating && !GetComponent<parkour2> ().climb) && !GetComponent<PlayerShootManager> ().isMeleeing || forceUse && useTimer <= 0) {
				useTimer = animationTime;
				isAnimating = true;
				isHolding = false;
				forceUse = false;
				throwincrease = 0f;

				if (equipmentType == equim.grenade && grenadePrefab != null) {
					if (equipmentanimation != null) {
						equipmentanimation.Play ("throwing");
					}
					StartCoroutine (throwstuff ());
				}
				if (equipmentType == equim.semtex && semtexPrefab != null) {
					if (equipmentanimation != null) {
						equipmentanimation.Play ("throwing");
					}
					StartCoroutine (throwstuff ());
				}

				if (equipmentType == equim.flashbang && flashbangPrefab != null) {
					if (equipmentanimation != null) {
						equipmentanimation.Play ("throwing");
					}
					StartCoroutine (throwstuff ());
				}

				if (equipmentType == equim.c4 && c4Prefab != null) {
					if (equipmentanimation != null) {
						equipmentanimation.Play ("throwing");
					}
					StartCoroutine (throwstuff ());
				}

				if (equipmentType == equim.claymine && clayminePrefab != null) {
					if (equipmentanimation != null) {
						equipmentanimation.Play ("throwing");
					}
					StartCoroutine (throwstuff ());
				}

				if (equipmentType == equim.knife && knifePrefab != null) {
					if (equipmentanimation != null) {
						equipmentanimation.Play ("throwing");
					}
					StartCoroutine (throwstuff ());
				}

			}
		}
	}


	private IEnumerator throwstuff(){
		yield return new WaitForSeconds(pointTime);
		CmdtelltoSpawnNade (bodyrotator.TransformPoint (0f, 0.85f, 0.85f), this.gameObject);
	}


	[Command]
	public void CmdtelltoSpawnNade(Vector3 pos, GameObject myPlayer){
		GameObject nd = Instantiate(grenadePrefab, pos, Quaternion.identity);
		NetworkServer.SpawnWithClientAuthority(nd, myPlayer);		
	}



	public void hold(){
		isHolding = true;
		GetComponent<PlayerShootManager>().cancelAll();
		if(weaponHolder != null){weaponHolder.SetActive(false);}
		throwincrease += Time.deltaTime * 10f;
		if(throwincrease >= 10){throwincrease = 10;}
		//mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, GetComponent(PlayerShootManager).defaultcamerafFov, Time.deltaTime * 16f);
	}


	public void releasehold(){
		useTimer = 0f;
		isAnimating = false;
		if(equipmentanimation != null){equipmentanimation.Play("none_equipment");}
		if(weaponHolder != null){weaponHolder.SetActive(true);}
		if(weaponHolder != null){weaponHolder.GetComponent<Animator>().Play("weaponReady");}
	}

	public void animCancel(){
		useTimer = 0f;
		throwincrease = 0;
		isAnimating = false;
		if(equipmentanimation != null){equipmentanimation.Play("none_equipment");}
		if(weaponHolder != null){weaponHolder.SetActive(true);}
		if(weaponHolder != null){weaponHolder.GetComponent<Animator>().Play("weaponReady");}
	}



	public void increaseEquipmentAmount(int x){
		numEquipment += x;
	}



	public void loadequipments(){
		string eqms = PlayerPrefs.GetString("playerequipments");
		string[] elist = eqms.Split(":"[0]);

		for(int i = 0; i < elist.Length; i++){
			if (elist [i] == "grenade") { equipmentType = equim.grenade;  }
			if (elist [i] == "semtex") {  equipmentType = equim.semtex; }
			if (elist [i] == "flashbang") {  equipmentType = equim.flashbang; }
			if (elist [i] == "knife") {   equipmentType = equim.knife;}
			if (elist [i] == "c4") {  equipmentType = equim.c4; }
			if (elist [i] == "claymine") {  equipmentType = equim.claymine; }
		}


	}








}
