using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parkour2 : MonoBehaviour {

	public Transform mbody;
	public GameObject weaponHolder;
	public float distanceToClimb = 0.5f; 
	public float raycastDistance = 5f;
	public LayerMask layerMask;
	private RaycastHit hit;
	private float heightToBe;
	public bool climb;
	public bool canClimb;
	private Vector3 positionToBe;
	public float climbRate= 12.1f;
	private bool climbingladder = false;
	private bool inside;
	public float speed = 2f;
	private Vector3 positionToBeTopLadder;
	private Vector3 positionToBeBottomLadder;


	void Update(){
		if(climb){Climb();}
		climbOrNot();


		if(inside && Input.GetKey("w") && transform.position.y < positionToBeTopLadder.y && !climb){
			climbingladder = true;
			GetComponent<CharacterController>().Move(Vector3.up * Time.deltaTime * speed);
		}else{
			if(!inside){climbingladder = false;}
		}


		if(inside && Input.GetKey("s") && !climb){   
			if( GetComponent<CharacterController>().isGrounded){
				climbingladder = false;
			}else{
				GetComponent<CharacterController>().Move(Vector3.down * Time.deltaTime * speed);
				climbingladder = true;
			}
		}
	}


	public bool isClimbingladder(){
		return climbingladder;
	}
	public void Climb(){
		if(weaponHolder != null){weaponHolder.SetActive(false);}
		canClimb = false;

		if(Vector3.Distance(transform.position,positionToBe) > 0.1f){
			transform.position = Vector3.MoveTowards(transform.position, positionToBe, climbRate * Time.deltaTime);
		}else{
			if(climb){
				climb = false;
				if (weaponHolder != null) {
					weaponHolder.SetActive (true);
					if (weaponHolder.activeInHierarchy) {
						weaponHolder.GetComponent<Animator> ().Play ("weaponReady");
					}
				}
				GetComponent<PlayerShootManager>().cancelAll();
			}
		}
	}


	public void climbOrNot(){
		if(climb || climbingladder){return;}
		if(Physics.Raycast(mbody.position,mbody.forward,out hit,raycastDistance,layerMask)){
			if(hit.transform.gameObject.GetComponent<Renderer>() == null){return;}
			//Debug.Log("Center Position: " + hit.transform.gameObject.GetComponent(Renderer).bounds.center);
			var centerY = hit.transform.gameObject.GetComponent<Renderer>().bounds.center.y;
			var sizeY = hit.transform.gameObject.GetComponent<Renderer>().bounds.size.y/2f;
			//Debug.Log("Centery = " + centerY);
			//Debug.Log("sizey div 2 = " + sizeY);
			var realPos = centerY + sizeY;
			var realBot = centerY - sizeY;
			var hitbottom = Mathf.Round(realBot - hit.point.y);
			//Debug.Log("REAL POS Y = " + realPos);
			//Debug.Log("Point y of Hit: " + hit.point.y.ToString());
			//Debug.Log("RealBot Pos y: " + realBot);
			//Debug.Log("hitbottom: is  " + hitbottom);
			if((realPos - hit.point.y) < distanceToClimb && (realPos - hit.point.y) != 0 && hitbottom != 0){
				//Debug.Log("CanClimb");
				positionToBe = new Vector3(hit.point.x,0f,hit.point.z) + new Vector3(0f,realPos + 1f, 0f);
				positionToBe = positionToBe + (hit.normal * -0.5f);
				//Debug.Log("Position To Be: " + positionToBe);
				canClimb = true;
			}else{
				canClimb = false;
			}
		}else{
			canClimb = false;
		}
	}





	public void hasEnergy(bool energy){
		if(energy){
			climbRate =  11.95f;
		}else{
			climbRate = 11.95f;
		}
	}



	public void OnTriggerEnter(Collider col) {
		if(col.gameObject.tag == "Ladder"){
			inside = !inside;
			var centerY = col.bounds.center.y;
			var sizeY = col.bounds.size.y/2;
			var realPos = centerY + sizeY;
			var realBot = centerY - sizeY;
			var hitbottom = Mathf.Round(realBot);
			positionToBeBottomLadder = new Vector3(hit.point.x,0f,hit.point.z) + new Vector3(0f,realBot, 0f);
			positionToBeTopLadder = new Vector3(hit.point.x,0f,hit.point.z) + new Vector3(0f,realPos + 1f, 0f);

		}
	}


	public void OnTriggerExit(Collider col) {
		if(col.gameObject.tag == "Ladder"){
			inside = !inside;
		}
	}









}