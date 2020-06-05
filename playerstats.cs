using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerstats : MonoBehaviour{

	public UnityEngine.UI.Image expbar;
	public UnityEngine.UI.Text currentlvltext;
	public UnityEngine.UI.Text nextlvltext;
	public UnityEngine.UI.Text scoretext;
	public UnityEngine.UI.Text wintext;
	public UnityEngine.UI.Text tiestext;
	public UnityEngine.UI.Text losstext;
	public UnityEngine.UI.Text winstreak;
	public UnityEngine.UI.Text killstext;
	public UnityEngine.UI.Text deathtext;
	public UnityEngine.UI.Text kdrtext;
	public UnityEngine.UI.Text headshottext;
	public UnityEngine.UI.Text assisttext;
	public UnityEngine.UI.Text killstreaktext;



	void Start () {
		int level  = PlayerPrefs.GetInt("Mylevel");
		currentlvltext.text = "CURRENT LVL " + level.ToString();
		nextlvltext.text = (level + 1).ToString();
		float currentmaxExp = PlayerPrefs.GetFloat("Current maxExp");
		float Xp = PlayerPrefs.GetFloat("Current Xp");
		float ex = map( Xp , 0f , currentmaxExp , 0f , 1f);
		expbar.fillAmount = ex;
		scoretext.text = PlayerPrefs.GetInt("MyScore").ToString();
		wintext.text = PlayerPrefs.GetInt("MyWin").ToString();
		tiestext.text =  PlayerPrefs.GetInt("MyTies").ToString();
		losstext.text = PlayerPrefs.GetInt("MyLoss").ToString();
		winstreak.text = PlayerPrefs.GetInt("MyWinStreak").ToString();
		killstext.text = PlayerPrefs.GetInt("MyKills").ToString();
		deathtext.text = PlayerPrefs.GetInt("MyDeaths").ToString();
		headshottext.text = PlayerPrefs.GetInt("MyHeadShots").ToString();
		assisttext.text = PlayerPrefs.GetInt("MyAssists").ToString();
		killstreaktext.text = PlayerPrefs.GetInt("MyKillStreak").ToString();
		kdrtext.text = calculateKDR(PlayerPrefs.GetInt("MyKills"),PlayerPrefs.GetInt("MyDeaths")).ToString();
	}



	public float map(float x, float minExp, float maxExp, float min = 0f, float max = 1f){
		return (x - minExp) * (max - min) / (maxExp - minExp) + min;
	}



	public float calculateKDR(float numkill, float numdead){
		if(numkill == 0 && numdead == 0){
			return 0;
		}
		if(numkill != 0 && numdead !=0){
			if(numkill >= numdead){
				return numkill/numdead;
			}
			if(numkill < numdead){
				return -numkill/numdead;
			}
		}

		if(numkill != 0 && numdead == 0){
			return numkill;
		}
		if(numkill == 0 && numdead !=0){
			return -numdead;
		}	
		return 0;
	}


}
