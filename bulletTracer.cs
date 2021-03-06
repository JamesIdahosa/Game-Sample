using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletTracer : MonoBehaviour {

	public Transform mTrans;
	public AudioSource impactAudioSource;
	public GameObject trailObject;
	public AudioClip[] defaultImpactSounds;
	public AudioClip[] fleshImpactSounds;
	public AudioClip[] metalImpactSounds;
	public AudioClip[] woodImpactSounds;
	public AudioClip[] glassImpactSounds;
	public AudioClip[] waterImpactSounds;
	private Vector3 mInitialPosition;
	private Vector3 mDirection;
	private Vector3 mGravity = new Vector3(0f, -9.8f, 0f);
	private float mVelocity;
	private Vector3 mCurrentVelocity;
	private RaycastHit hit;
	private bool mResetInvoked;
	private int mNumPenetration = 1;
	private int currentPenetrationCount = 0;
	private int num= 0;
	private bool hasPlayedClip;
	public GameObject hitAudioObj;
	public LayerMask layerMask;


	void  Awake(){
		this.mTrans = this.transform;
		currentPenetrationCount = 0;
		StartCoroutine(stop());
	}

	public void Fire(Vector3 position, Vector3 direction, float velocity){
		this.mVelocity = velocity;
		this.mInitialPosition = position;
		this.mDirection = direction;
		this.mTrans.position = position;
		this.mCurrentVelocity = direction * mVelocity;
		this.mResetInvoked = false;
	}

	void Update(){
		if (mResetInvoked){
			diableTrailEffect ();
			return;
		}
		this.mCurrentVelocity += mGravity * Time.deltaTime;				
		Vector3 vector = this.mCurrentVelocity * Time.deltaTime;
		float magnitude = vector.magnitude;
		this.mTrans.rotation = Quaternion.LookRotation(mCurrentVelocity);
		if (Physics.Raycast(mTrans.position, vector, out hit, magnitude, layerMask)){
			if(currentPenetrationCount < mNumPenetration){
				this.transform.position = (hit.point + (hit.normal * -0.0001f));
				OnHit(hit);
				currentPenetrationCount++;
			}else{
				mResetInvoked = true;
			}
		}else{
			Transform expr_156 = this.mTrans;
			expr_156.position = expr_156.position + vector;
		}
	}


	public void  OnHit(RaycastHit hit){
		if(impactAudioSource != null && !hasPlayedClip && hitAudioObj != null){
			if (hit.collider.transform.tag == "wall" || hit.collider.transform.tag == "block" || hit.collider.transform.tag == "Ground"){
				if(defaultImpactSounds.Length >= 1){
					num = Random.Range(0,defaultImpactSounds.Length);
					hitAudioObj.transform.SetParent(hit.collider.transform, true);
					hitAudioObj.transform.position = hit.point;
					impactAudioSource.maxDistance = 5f;
					impactAudioSource.minDistance = 1f;
					if(defaultImpactSounds[num] != null){impactAudioSource.PlayOneShot(defaultImpactSounds[num]); }
					hasPlayedClip = true;
				}
				return;
			}

			if(hit.collider.transform.tag == "Metal"){
				if(metalImpactSounds.Length >= 1){
					num = Random.Range(0,metalImpactSounds.Length);
					hitAudioObj.transform.SetParent(hit.collider.transform, false);
					hitAudioObj.transform.position = hit.point;
					impactAudioSource.maxDistance = 18f;
					impactAudioSource.minDistance = 1f;
					if(metalImpactSounds[num] != null){impactAudioSource.PlayOneShot(metalImpactSounds[num]);}
					hasPlayedClip = true;
				}
				return;
			}

			if(hit.collider.transform.tag == "zombie"){
				if(fleshImpactSounds.Length >= 1){
					num = Random.Range(0,fleshImpactSounds.Length);
					hitAudioObj.transform.SetParent(hit.collider.transform, false);
					hitAudioObj.transform.position = hit.point;
					impactAudioSource.maxDistance = 18f;
					impactAudioSource.minDistance = 1f;
					//if(fleshImpactSounds[num] != null){impactAudioSource.PlayOneShot(fleshImpactSounds[num]);}
					hasPlayedClip = true;
				}
				return;
			}



			if(hit.collider.transform.tag == "Wood"){
				if(woodImpactSounds.Length >= 1){
					num = Random.Range(0,woodImpactSounds.Length);
					hitAudioObj.transform.SetParent(hit.collider.transform, false);
					hitAudioObj.transform.position = hit.point;
					impactAudioSource.maxDistance = 18f;
					impactAudioSource.minDistance = 1f;
					if(woodImpactSounds[num] != null){impactAudioSource.PlayOneShot(woodImpactSounds[num]);}
					hasPlayedClip = true;
				}
				return;
			}

			if(hit.collider.transform.tag == "Water"){
				if(waterImpactSounds.Length >= 1){
					num = Random.Range(0,waterImpactSounds.Length);
					hitAudioObj.transform.SetParent(hit.collider.transform, false);
					hitAudioObj.transform.position = hit.point;
					impactAudioSource.maxDistance = 18f;
					impactAudioSource.minDistance = 1f;
					if(waterImpactSounds[num] != null){impactAudioSource.PlayOneShot(waterImpactSounds[num]);}
					hasPlayedClip = true;
				}
				return;
			}
			mResetInvoked = true;
		}
	}


	public void diableTrailEffect(){
		if(trailObject != null){
			trailObject.SetActive(false);
		}
	}

	public IEnumerator stop(){
		yield return new WaitForSeconds(5f);
		if(hitAudioObj != null){Destroy(this.hitAudioObj);}
		Destroy(this.gameObject);
	}















}
