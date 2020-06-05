using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaningScript : MonoBehaviour
{
	public Transform leanTransform;
	public bool slerp = true;
	[Range(0f,1f)]
	public float curve = 0f;
	public float leantime = 0.28f;
	public Vector3 left_leanPosition = new Vector3(-0.6f,-0.2f,0f);
	public Vector3 right_leanPosition= new Vector3(0.6f,-0.2f,0f);
	public Vector3 left_leanRotation= new Vector3(0f,0f,15f);
	public Vector3 right_leanRotation= new Vector3(0f,0f,-15f);
	private Vector3 initial_Position=Vector3.zero;
	private Vector3 initial_Rotation=Vector3.zero;
	private string leftleankey = "q";
	private string rightleankey = "e";
	private float f;
	private Vector3 newPos;
	private Vector3 newRot;
	private bool isleanLeft;
	private bool isleanRight;
	private bool canlean;

	private void OnEnable()
	{
		if (!leanTransform)
		{
			leanTransform = base.transform;
		}
	}

	private void Update()
	{
		if ((Input.GetKey (leftleankey) && !Input.GetKey (rightleankey))  && (isleanLeft || f == 0f)) {
			newPos = left_leanPosition;
			newRot = left_leanRotation;
			isleanLeft = true;
			isleanRight = false;
			canlean = true;
		} else if ((Input.GetKey (rightleankey) && !Input.GetKey (leftleankey)) && (isleanRight || f == 0f)) {
			newPos = right_leanPosition;
			newRot = right_leanRotation;
			isleanLeft = false;
			isleanRight = true;
			canlean = true;
		} else {
			canlean = false;
		}

		if (canlean) {
			f += Time.deltaTime / leantime;
			f = Mathf.Clamp01 (f);
		} else {
			f -= Time.deltaTime / leantime;
			f = Mathf.Clamp01 (f);
		}

		leanTransform.localPosition = Vector3.Lerp(Vector3.zero, Vector3.zero + this.newPos * (1f - Mathf.Abs(1f - this.f) * this.curve), this.f);
		leanTransform.localEulerAngles = Vector3.Slerp (Vector3.Slerp (Vector3.zero, newRot, f), newRot, f);

	}

	public bool IsLeaning()
	{
		return (Input.GetKey (leftleankey) || Input.GetKey (rightleankey));
	}

	public int LeaningRight()
	{
		int result = 0;
		if (Input.GetKey (leftleankey) == Input.GetKey (rightleankey)) {
			result = 0;
		} else if (Input.GetKey (rightleankey) && !Input.GetKey (leftleankey)) {
			result = 1;
		} else if (Input.GetKey (leftleankey) && !Input.GetKey (rightleankey)) {
			result = -1;
		} else {
			result = 0;
		}
		return result;
	}

	public Vector3 EulerDelta()
	{
		return (leanTransform.localEulerAngles * -1f);
	}

}
