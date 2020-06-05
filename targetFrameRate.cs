using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class targetFrameRate : MonoBehaviour {

	public UnityEngine.UI.Text fpsuitext;
	public int target = 90;
	public float nextupdateTime = 1f;
	private int frames = 0;
	private float timer = 0f;
	public float currentfps;

	void Awake () {
		//QualitySettings.vSyncCount = 0;
		//target = PlayerPrefs.GetInt("TargetFps");
		Application.targetFrameRate = target;
		timer = 0;
	}

	void Start(){
		fpsuitext = GameObject.Find("Canvas").GetComponent<canvasManager>().fpsText;
	}

	void Update () {
		if(target != Application.targetFrameRate){ Application.targetFrameRate = target; }
		if(fpsuitext != null){fpsuitext.text = "fps: " + currentfps.ToString("F2");}
		++frames;
		timer += Time.deltaTime;

		if(timer > nextupdateTime){
			currentfps = frames/nextupdateTime;
			frames = 0;
			timer = 0;
		}

	}

}
