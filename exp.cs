using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class exp : MonoBehaviour {
	public UnityEngine.UI.Image expBar; 
	public UnityEngine.UI.Text lvlText;
	private float ex;
	public float speed;
	public float currentmaxExp = 0;
	public int level;
	public float Xp; 
	public int totalscore;
	public bool reset = false;
	public int nextlvl;

	void Start(){
		expBar = GameObject.Find("expBar").GetComponent<UnityEngine.UI.Image>();
		lvlText = GameObject.Find("lvlText").GetComponent<UnityEngine.UI.Text>();

		level = PlayerPrefs.GetInt("Mylevel");
		currentmaxExp = PlayerPrefs.GetFloat("Current maxExp");
		Xp = PlayerPrefs.GetFloat("Current Xp");
		ex = map( Xp , 0 , currentmaxExp , 0 , 1);
		expBar.fillAmount = ex;

		totalscore = PlayerPrefs.GetInt("MyScore");
	}
		
	void Update () {
		nextlvl = level + 1;
		lvlText.text = "lvl" + nextlvl.ToString();

		leveledUp();
		ex = map(Xp,0,currentmaxExp,0,1);
		expBar.fillAmount = Mathf.Lerp(expBar.fillAmount, ex, Time.deltaTime * speed);

		if(reset == true){
			resetXp();
			reset = false;
		}
	}

	public float map(float x,float minExp, float maxExp, float min, float max){
		return (x - minExp) * (max - min) / (maxExp - minExp) + min;
	}
		

	public void leveledUp(){
		if(Xp >= currentmaxExp && expBar.fillAmount >= 0.95){
			float leftover;
			leftover = Xp - currentmaxExp;
			Xp = leftover;
			currentmaxExp += 100f;
			level++;
			UpdateRecord();
			expBar.fillAmount = 0;
		}	
	}	


	public void resetXp(){
		PlayerPrefs.DeleteKey("Mylevel");
		PlayerPrefs.DeleteKey("Current maxExp");
		PlayerPrefs.DeleteKey("Current Xp");
		level = 0;
		Xp = 0;
		currentmaxExp = 100;
	}	

	public void addXp(int xp){
		Xp += xp;
		PlayerPrefs.SetFloat("Current Xp", Xp);
		totalscore += xp;
		PlayerPrefs.SetInt("MyScore", totalscore);
	}		

	public void UpdateRecord(){
		PlayerPrefs.SetInt("Mylevel",level);
		PlayerPrefs.SetFloat("Current maxExp", currentmaxExp);
		PlayerPrefs.SetFloat("Current Xp", Xp);
		GetComponent<pointSystem>().myMedals.Enqueue("LEVEL " + level);
		GetComponent<pointSystem>().spawnfeedlog(new Vector3(0.7f,1f,1f), 15f, false,"level up " + level.ToString() , 45,"");
	}





}
