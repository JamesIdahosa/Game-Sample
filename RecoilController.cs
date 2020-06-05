using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RecoilController : MonoBehaviour {
	private float recoil = 0.0f;
	private float recoilReturnSpeed = 4f;
	private Vector3 maxRecoil;
	private Vector3 currentRecoil;

	public void StartRecoil (float recoilParam, float maxRecoil_xParam, float maxRecoil_YParam, float recoilSpeedParam){
		if (recoil <= (recoilParam - (recoilParam * 1f))) {
			recoil = recoilParam;
			maxRecoil  += new Vector3 (-maxRecoil_xParam, Random.Range (-maxRecoil_YParam, maxRecoil_YParam), 0f);
		}
	}

	void recoiling (){
		if (recoil >= 0f) {
			currentRecoil = Vector3.Lerp (currentRecoil, maxRecoil, 0.15f);
			transform.localEulerAngles = currentRecoil;
			recoil -= Time.deltaTime ;
		} else {
			recoil = 0;
		}
	}

	// Update is called once per frame
	void Update (){
		recoiling ();
	}


	public void returnnormal(){
		maxRecoil = Vector3.Lerp  (maxRecoil, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
		currentRecoil = Vector3.Lerp (currentRecoil, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
		transform.localRotation = Quaternion.Slerp (transform.localRotation, Quaternion.identity, Time.deltaTime * recoilReturnSpeed);
	}
}