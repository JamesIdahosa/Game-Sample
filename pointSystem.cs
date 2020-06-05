using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class pointSystem : NetworkBehaviour {
	[SyncVar(hook = "OnGetRevengeNetID")]
	public string revengeNetID;

	[SyncVar(hook = "OnGetAssistNetID")]
	public string assistNetID;

	[SyncVar(hook = "OnGotAssitFromShot")]
	public int assistNotification;

	[SyncVar(hook = "OnGotAssitSuicide")]
	public int assistSuicide;


	[SyncVar(hook = "OnTeamStreakAssist")]
	public int streakAssist;


	public int deathLimit;
	public int pointAmount;
	public string personthatkilledmeid;
	public float countdown;
	public float assistCountDown;

	public streakcontrol streakScript;
	public logManager logmanager;

	Animator AcheivementAnim;
	Animator DouTriQuadAnim;
	UnityEngine.UI.Text DouTriQuadText;
	Animator streakPushAnim;


	public UnityEngine.UI.Image stkImage;
	public UnityEngine.UI.Text  stkName;
	public UnityEngine.UI.Text  stkDescription;
	public UnityEngine.UI.Image stkBckground;


	public Animator medalAnimation;
	public UnityEngine.UI.Image medalImage;
	public UnityEngine.UI.Text medalName;
	public UnityEngine.UI.Text medalDescription;
	public UnityEngine.UI.Image medalBackground;


	public UnityEngine.Sprite[] steakImages = new UnityEngine.Sprite[25];
	public UnityEngine.Sprite[] backgrounds; 
	public UnityEngine.Sprite[] medalImages; 

	private bool display = false;
	private float start = 1f;


	private float medalTimer;
	private float medals;
	private float timeM;
	private float killtextTimer;
	private float killtexts;
	private float timeK;
	private float myMedalTime;
	private float medalTime;
	private float timeMP;

	public Queue<string> Alltext= new Queue<string>();
	public Queue<string> Allstreak = new Queue<string>();
	public Queue<string> myMedals = new Queue<string>();
	public Stack<string> assistStack = new Stack<string>();
	public List<string> paybackList = new List<string>();



	void Start(){
		if(isLocalPlayer){
			DouTriQuadAnim = GameObject.Find("killTest").GetComponent<Animator>();
			DouTriQuadText = GameObject.Find("killTest").GetComponent<UnityEngine.UI.Text>();
			streakPushAnim = GameObject.Find("streakPush").GetComponent<Animator>();
			//--------------------------------------------------------------------------------------------	
			AcheivementAnim = GameObject.Find("killStreakAchievements").GetComponent<Animator>();
			stkImage = GameObject.Find("streakImage").GetComponent<UnityEngine.UI.Image>();
			stkName = GameObject.Find("streakName").GetComponent<UnityEngine.UI.Text>();
			stkDescription = GameObject.Find("streakDescription").GetComponent<UnityEngine.UI.Text>();
			stkBckground =  GameObject.Find("streakBackground").GetComponent<UnityEngine.UI.Image>();
			//--------------------------------------------------------------------------------------------
			medalAnimation = GameObject.Find("medalAnim").GetComponent<Animator>();
			medalImage = GameObject.Find("medImage").GetComponent<UnityEngine.UI.Image>();
			medalName = GameObject.Find("medName").GetComponent<UnityEngine.UI.Text>();
			medalDescription = GameObject.Find("medDescription").GetComponent<UnityEngine.UI.Text>();
			medalBackground = GameObject.Find("medBackground").GetComponent<UnityEngine.UI.Image>();
			//---------------------------------------------------------------------------------------------

			logmanager= GameObject.Find("Canvas").transform.GetComponent<logManager>();

		}
	}



	void Update() {
		if(isLocalPlayer){
			AcheivementAnim.SetInteger("reward",0);
			DouTriQuadAnim.SetInteger("kill",0);
			streakPushAnim.SetInteger("push",0);
			medalAnimation.SetInteger("meds",0);



			if(Input.GetKeyDown("p")){addtext();}
			if(Input.GetKeyDown("k")){addStreak();}

			medalTimer -= Time.deltaTime;
			myMedalTime -= Time.deltaTime;
			killtextTimer -= Time.deltaTime;
			countdown -= Time.deltaTime;
			assistCountDown -= Time.deltaTime;

			priorityAnim();
			myStreaks();
			myMedalplay();
			medalText();
			assistTimeFinished();		

		}
	}


	public void assistTimeFinished(){
		if(assistCountDown < 0){
			if(assistStack.Count > 0){
				assistStack.Clear();
			}
		}
	}


	public void addtext(){
		Allstreak.Enqueue("jet");
		//Alltext.Enqueue("YOU UNLOCKED ACR-6");
	}

	public void addStreak(){
		myMedals.Enqueue("Testing");
	}


	public void myStreaks(){
		if(killtextTimer < 0 && myMedalTime < 0){
			if(medalTimer < 0){
				timeM= 0;
			}
			medals += Time.deltaTime * 1;
			if(medals > timeM){
				timeM = 0.25f;//timeM = 1.2;
				if(Allstreak.Count > 0){
					if(Allstreak.Peek() != null){
						streakPushAnim.Play("pushStreak",-1,1);
						AcheivementAnim.Play("kill",-1,1);
						Allstreak.Dequeue();
						medals=0;
						medalTimer = 2.14f;
					}
				}
			}
		}
	}


	public void medalText(){
		if(medalTimer < 0 && myMedalTime < 0){
			if(killtextTimer < 0){
				timeK = 0;
			}
			killtexts += Time.deltaTime;
			if(killtexts > timeK){
				timeK = 0.7f;
				if(Alltext.Count > 0){
					if(Alltext.Peek() != null){
						DouTriQuadText.text = Alltext.Peek();
						DouTriQuadAnim.Play("AwardanimText",-1,1);
						Alltext.Dequeue();
						killtexts = 0;
						killtextTimer = 1.83f;
					}
				}
			}
		}
	}


	public void myMedalplay(){
		if(medalTimer < 0 && killtextTimer < 0){
			if(myMedalTime < 0){
				timeMP = 0;
			}
			medalTime += Time.deltaTime;
			if(medalTime > timeMP){
				timeMP = 0.85f;
				if(myMedals.Count > 0){
					if(myMedals.Peek() != null){
						showMedal(myMedals.Peek());
						medalAnimation.Play("medals", -1, 1);
						DouTriQuadAnim.Play("none",-1,1);
						myMedals.Dequeue();
						medalTime = 0;
						myMedalTime = 2.40f;
					}
				}
			}
		}
	}







	public void priorityAnim(){
		if((Allstreak.Count > 0) && (killtextTimer > 0 || myMedalTime > 0)){
			medalTimer = 2.14f;
			killtextTimer = 0;
			myMedalTime = 0;
			streakPushAnim.Play("pushStreak",-1,1);
			DouTriQuadAnim.Play("none",-1,1);
			medalAnimation.Play("none",-1,1);
		}
	}


	public void awards(Vector3 enemyposition, string killinfo, string enemyNetID, string bodyhit_type){
		if(countdown > 0){
			pointAmount += 50;
			countdown = 5f;
		}else{
			pointAmount = 50;
			countdown = 5f;
		}

		int xp = 100;	
		if(killinfo == "melee"){
			myMedals.Enqueue("Crush!");
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "CRUSH", 50,"");
			xp += 50;
		}
		if(deathLimit >= 3){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "COMEBACK", 25,"");
			xp += 25;
		}
		deathLimit = 0;
		if(killinfo == "Sniper"){
			if(this.transform.GetComponent<myPlayer>().Health == this.transform.GetComponent<myPlayer>().MaxHealth){
				spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "ONE SHOT, ONE KILL", 100,"");
				xp+=100;
			}
		}
			
		if(this.transform.GetComponent<myPlayer>().Health <= 30){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "ALMOST DEAD KILL", 75,"");
			xp+=75;
		}
		if (pointAmount == 100){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "DOULBLE    KILL", 100,"");
			xp+=100;
		}
		if (pointAmount == 150){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "TRIPPLE     KILL", 150,"");
			xp+=150;
		}
		if (pointAmount >= 200){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "MONSTER     KILL", 200,"");
			xp+=200;
		}

		if(Vector3.Distance(transform.position, enemyposition) >= 50 && killinfo != "missile" && killinfo != "Streak" && killinfo != "Equipment"){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "LONGSHOT", 50,"");
			xp+=50;
		}


		foreach(string IDs in paybackList){
			if(enemyNetID == IDs){
				spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "PAYBACK", 25,"");
				paybackList.Remove(enemyNetID);
				xp+=25;
				break;
			}
		}
			
		if(this.transform.GetComponent<myPlayer>().Health <= 0){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "UNDERTAKER", 35,"");
			xp+=35;
		}

		if(bodyhit_type == "head"){
			PlayerPrefs.SetInt("MyHeadShots" , PlayerPrefs.GetInt("MyHeadShots") + 1);
			spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "HEADSHOT", 100,"");
			xp+=100;
		}	
			
		showpoints(xp, killinfo);
		GetComponent<exp>().addXp(xp);
		giveTeamStreakAssist();
		if (GetComponent<PlayerShootManager> ().momentum) {
			int currentHealth = GetComponent<myPlayer> ().Health;
			if (currentHealth < 100) {
				spawnfeedlog (new Vector3 (0.7f, 1f, 1f), 21f, false, "MOMENTUM", 100, "");
				int plusHealth = (int)(currentHealth + (currentHealth * 0.25f));
				if (plusHealth > 100) {plusHealth = 100;}
				GetComponent<myPlayer> ().Cmdreviveme (plusHealth);
			}
		}

	}



	public void showMedal(string medalname){
		int ros = Random.Range( 0, medalImages.Length);
		int pos = Random.Range( 0, backgrounds.Length);
		medalName.text = medalname;
		//medalBackground.sprite = backgrounds[pos];
		//medalImage.sprite = medalImages[ros];

		if(medalname == "Crush!"){
			medalDescription.text = "Get a melee kill[" + "<color=#00ff46ff>" + "+100" + "</color>"+"]";
		}
		medalDescription.text = "Get a melee kill[" + "<color=#00ff46ff>" + "+100" + "</color>"+"]";
	}	



	public void semtexstuck(){
		spawnfeedlog(new Vector3(0.7f,1f,1f), 21f, false, "Stuck", 25,"");
		GetComponent<exp>().addXp(78);
	}



	public void OnGetRevengeNetID(string rev){
		revengeNetID = rev;
		if(revengeNetID != ""){personthatkilledmeid = revengeNetID;}
		if(isLocalPlayer &&  revengeNetID != ""){
			if(paybackList.Contains(rev) == false){ paybackList.Add(rev);}
			GetComponent<playerName>().findNameWithNetID(personthatkilledmeid);
			GiveAssist(transform.GetComponent<playerName>().players, revengeNetID);
		}
	}



	public void OnGetAssistNetID(string assitID){
		assistNetID = assitID;
		if(isLocalPlayer){
			if(assistStack.Contains(assitID) == false){
				assistCountDown = 15f;
				assistStack.Push(assitID);
			}
		}
	}


	[Command]
	public void CmdTellServerGiveAssitToPlayer(GameObject player){
		player.transform.GetComponent<pointSystem>().assistNotification += 1;
	}


	[Command]
	public void CmdTellServerAssitSuicideToPlayer(GameObject player){
		player.transform.GetComponent<pointSystem>().assistSuicide += 1;
	}



	public void GiveAssist(GameObject[] playerList, string guywhokillmeID){
		bool teamBaseMatch = isTeamBaseMatch();
		string myNetID = GetComponent<GamemodeManager>().myNetID;

		if(teamBaseMatch && (guywhokillmeID != myNetID)){
			for( int i = 0; i < playerList.Length; i++){
				if(playerList[i] != null){
					foreach(string IDs in assistStack){
						if(playerList[i].transform.GetComponent<GamemodeManager>().myNetID != guywhokillmeID){
							if(playerList[i].transform.GetComponent<GamemodeManager>().myNetID == IDs){
								CmdTellServerGiveAssitToPlayer(playerList[i]);
							}
						}
					}
				}
			}
		}

		if(teamBaseMatch && (guywhokillmeID == myNetID)){
			for(int j = 0; j < playerList.Length; j++){
				if(playerList[j] != null){
					foreach(string IDs in assistStack){
						if(playerList[j].transform.GetComponent<GamemodeManager>().myNetID != guywhokillmeID){
							if(playerList[j].transform.GetComponent<GamemodeManager>().myNetID == IDs){
								CmdTellServerAssitSuicideToPlayer(playerList[j]);
							}
						}
					}
				}
			}
		}



		if(!teamBaseMatch && (guywhokillmeID == myNetID)){
			for(int k = 0; k < playerList.Length; k++){
				if(playerList[k] != null){
					foreach(string IDs in assistStack){
						if(playerList[k].transform.GetComponent<GamemodeManager>().myNetID != guywhokillmeID){
							if(playerList[k].transform.GetComponent<GamemodeManager>().myNetID == IDs){
								CmdTellServerAssitSuicideToPlayer(playerList[k]);
							}
						}
					}
				}
			}
		}	

		CmdTellServerToSetNormal(this.transform.gameObject);
		assistStack.Clear();
	}


	public void OnGotAssitFromShot(int assInt){
		assistNotification = assInt;
		if(isLocalPlayer){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 15f, false, "ASSIST", 25,"");
			PlayerPrefs.SetInt("MyAssists" , PlayerPrefs.GetInt("MyAssists") + 1);
			GetComponent<exp>().addXp(55);
		}	
	}

	public void OnGotAssitSuicide(int s){
		assistSuicide = s;
		if(isLocalPlayer){
			spawnfeedlog(new Vector3(0.7f,1f,1f), 15f, false, "ASSIST    SUICIDE", 75,"");
			countdown = 15f;
			GetComponent<exp>().addXp(125);
		}	
	}


	[Command]
	public void CmdTellServerToSetNormal(GameObject player){
		player.transform.GetComponent<pointSystem>().revengeNetID = "";
		player.transform.GetComponent<pointSystem>().assistNetID = "";
	}


	public bool isTeamBaseMatch(){
		bool isTeamGame = GetComponent<GamemodeManager>().isTeamGame;
		bool tb = false;
		if(isTeamGame){tb = true;}
		return tb;
	}

	public bool isTeamMate(GameObject player){
		bool isTeamGame  = GetComponent<GamemodeManager>().isTeamGame;
		if(isTeamGame){
			if(player.transform.GetComponent<GamemodeManager>().team == GetComponent<GamemodeManager>().team){
				return true;
			}
		}
		return false;
	}


	public void giveTeamStreakAssist(){
		if(!isTeamBaseMatch() || !isLocalPlayer){return;}
		GameObject[] playerList = GetComponent<playerName>().players;
		for( int i = 0; i < playerList.Length; i++){
			if(playerList[i] != null){
				if(playerList[i] != this.gameObject){
					if(isTeamMate(playerList[i])){
						CmdTellServerToGiveStreakAssist(playerList[i]);
					}
				}   
			}
		}    
	}

	public void  OnTeamStreakAssist(int x){
		streakAssist = x;
		if(isLocalPlayer){
			int xp = 0;
			if(streakScript.hasUAV){
				spawnfeedlog(new Vector3(0.7f,1f,1f), 15f, false, "UAV    BONUS", 25 ,"");
				xp+=25;
			}
			if(streakScript.hasSuperUAV){
				spawnfeedlog(new Vector3(0.7f,1f,1f), 15f, false, "SUPER    BONUS", 25 ,"");
				xp+=25;
			}

			if(streakScript.hasEMP){
				spawnfeedlog(new Vector3(0.7f,1f,1f), 15f, false, "EMPASSIST    BONUS", 75 ,"");
				xp+=75;
			}

			if(streakScript.hasCounterUAV){
				spawnfeedlog(new Vector3(0.7f,1f,1f), 15f, false, "COUNTERUAV    BONUS", 25 ,"");
				xp+=25;
			}

			GetComponent<exp>().addXp(xp);
		}
	}



	[Command]
	public void CmdTellServerToGiveStreakAssist(GameObject player){
		player.transform.GetComponent<pointSystem>().streakAssist ++;
	}
		

	public void  showpoints(int points, string headshot){

	}

	public void spawnfeedlog(Vector3 scale, float sizeHeight, bool isKill, string text, int points, string headshot){
		if(isKill){
			logmanager.logtext.Enqueue(text + ":" + "+" + points.ToString() + ":kill");
			Debug.Log (text + ":" + "+" + points.ToString () + ":kill");
		}else{
			logmanager.logtext.Enqueue(text + ":" + "+" + points.ToString() + ":");
		} 
	}
}
