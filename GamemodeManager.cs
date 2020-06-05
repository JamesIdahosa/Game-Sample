using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GamemodeManager : NetworkBehaviour {

	public int gameScoreLimit;
	public GameObject host;
	public playerName playerNameScript;
	public canvasManager cvmanager;	
	public string lobbygameType;
	[SyncVar(hook = "OnGameStart")]
	public string gamemode;
	[SyncVar(hook = "OnChooseTeam")]
	public string team;
	[SyncVar(hook = "OnMinutesChange")]
	public int minutes = 10;
	[SyncVar(hook = "OnSecondsChange")]
	public float seconds = 0;
	[SyncVar(hook = "OnGetNetID")]
	public string myNetID;
	[SyncVar(hook = "OnGetRank")]
	public int myRank;
	[SyncVar(hook = "OnMapA")]
	public int mapAnumber;
	[SyncVar(hook = "OnMapB")]
	public int mapBnumber;
	[SyncVar(hook = "OnServerStart")]
	public bool isHost;
	[SyncVar(hook = "OnMatchOver")]
	public bool matchDone;
	public bool isTeamGame;
	[SyncVar(hook = "OnTeamAScore")]
	public int TeamBaseteamAscore;
	[SyncVar(hook = "OnTeamBScore")]
	public int TeamBaseteamBscore;



	private bool matchisDone;
	private float sendTimer;
	private bool hasChosenNextMap;
	private float voteTimer = 40f;
	private bool hasSwitchMap;
	private string mapA;
	private string mapB;
	private bool playedEndingAnimation;
	private bool gameOver = false;

	public UnityEngine.UI.Text gameTypeText;
	public UnityEngine.UI.Text timerText;
	public UnityEngine.UI.Text myTeamText;
	public UnityEngine.UI.Text myTeamScoreText;
	public UnityEngine.UI.Text enemyTeamText;
	public UnityEngine.UI.Text enemyTeamScoreText;
	public UnityEngine.UI.Text myDeathMatchScoreText;
	public UnityEngine.UI.Text enemyDeathMatchScoreText;
	public UnityEngine.UI.Text voteMapHeader;
	public GameObject voteAMap;
	public GameObject voteBMap;
	public UnityEngine.UI.Text voteAname;
	public UnityEngine.UI.Text voteBname;
	public Transform mapAContent;
	public Transform mapBContent;
	public GameObject matchInformationUI;
	public NetworkManagerCustom netmanager;
	public GameObject voterPrefab;
	public GameObject voterVoted = null;
	public GameObject topPlayerPlanel;
	public Sprite[] mapSprites;
	public string[] availableMaps;
	private string myColHash =  "<color=#00aaffff>";
	private string enemyColHash  = "<color=#ff5454ff>";
	private int winsave;
	private int losssave;
	private int tiessave;
	private int highestwinstreak;
	private int currentgamewinstreak;




	private int matchminutes;
	private float matchseconds;
	private float matchtime;
	private int matchteamascore;
	private int matchteambscore;
	private bool hasPickedteam = false;
	private float waitTime = 0f;
	private bool isFindingHost = false;

	void Start (){

		mapAContent = GameObject.Find("votecontentA").transform;
		mapBContent = GameObject.Find("votecontentB").transform;
		voterVoted = null;

		if(isLocalPlayer){
			netmanager = GameObject.Find ("NetworkManagerCustom").transform.GetComponent<NetworkManagerCustom> ();
			cvmanager = GameObject.Find("Canvas").transform.GetComponent<canvasManager>();
			currentgamewinstreak = PlayerPrefs.GetInt("CurrentWinStreak");
			winsave = PlayerPrefs.GetInt("MyWin");
			losssave = PlayerPrefs.GetInt("MyLoss");
			tiessave = PlayerPrefs.GetInt("MyTies");
			highestwinstreak = PlayerPrefs.GetInt("MyWinStreak");
			voteMapHeader = GameObject.Find("voteHeader").transform.GetComponent<UnityEngine.UI.Text>();
			voteAMap = GameObject.Find("mapA");
			voteBMap = GameObject.Find("mapB");
			voteAname = GameObject.Find("mapAname").transform.GetComponent<UnityEngine.UI.Text>();
			voteBname = GameObject.Find("mapBname").transform.GetComponent<UnityEngine.UI.Text>();
			matchInformationUI = GameObject.Find("matchInfoHolder");
			voteMapHeader.enabled = false;
			voteAMap.transform.GetComponent<UnityEngine.UI.Image>().enabled = false;
			voteBMap.transform.GetComponent<UnityEngine.UI.Image>().enabled = false;
			voteAMap.transform.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			voteAMap.transform.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { PlaceVote("mapA");});
			voteBMap.transform.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
			voteBMap.transform.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { PlaceVote("mapB");});
			voteAname.enabled = false;
			voteBname.enabled = false;

			timerText = GameObject.Find("time").transform.GetComponent<UnityEngine.UI.Text>();
			myTeamText = GameObject.Find("meTeam").transform.GetComponent<UnityEngine.UI.Text>();
			enemyTeamText = GameObject.Find("notmeTeam").transform.GetComponent<UnityEngine.UI.Text>();
			myTeamScoreText = GameObject.Find("scoreMyTeam").transform.GetComponent<UnityEngine.UI.Text>();
			enemyTeamScoreText = GameObject.Find("scoreEnemyTeam").transform.GetComponent<UnityEngine.UI.Text>();
			gameTypeText = GameObject.Find("gameTypename").transform.GetComponent<UnityEngine.UI.Text>();
			myDeathMatchScoreText = myTeamScoreText;
			enemyDeathMatchScoreText = enemyTeamScoreText;
			topPlayerPlanel = GameObject.Find("TopPlayerPanel");
			gameScoreLimit = PlayerPrefs.GetInt("ScoreLimit");
			waitTime = 0f;
			CmdTellMyNetID(this.transform.gameObject,GetComponent<NetworkIdentity>().netId.ToString());
			CmdTellSetmyRank(this.gameObject,PlayerPrefs.GetInt("Mylevel"));

			if (isClient && !isServer) {
				if (host == null && !isFindingHost) { isFindingHost = true; StartCoroutine (findHostAndPickTeam ());}
			}

			if(isServer){
				lobbygameType = PlayerPrefs.GetString("GameMode");
				minutes = PlayerPrefs.GetInt("MatchTime");
				matchminutes = minutes;
				CmdtellSeverToUpdateGameTime(this.gameObject,minutes, seconds);
				CmdTellServerToActivateGameMode(this.transform.gameObject, lobbygameType);
				host = ClientScene.localPlayers[0].gameObject;
				if(Random.Range(0,2) == 0){CmdTellServeryourTeam(this.gameObject,"A");}else{CmdTellServeryourTeam(this.gameObject,"B"); }
				hasPickedteam = true;
			}

		}
	}






	void LateUpdate(){
		if(isLocalPlayer){
			//------------------------------------------------------------Server-----------------------------------------------------------------------------
			if(isServer && isClient){
				matchisDone = matchDone;
				if(!matchDone){seconds -= Time.deltaTime;}

				if(((minutes <= 0 && seconds <= 0) || gameOver) && !hasChosenNextMap){
					int mapAnum = Random.Range(0, availableMaps.Length);
					int mapBnum = 0;
					for(int m = 0; m < 10 ; m++){
						mapBnum=Random.Range(0,availableMaps.Length);
						if(mapBnum != mapAnum){
							break;
						}
					}
					CmdTellServerToTellClientsGameDoneAndVoteMap(this.transform.gameObject,mapAnum,mapBnum);
					hasChosenNextMap = true;
				}


				if(seconds <= 0 && !matchDone){
					minutes -= 1;
					seconds = 60f;
				}

				sendTimer -= Time.deltaTime;
				if(sendTimer <= 0 && !matchDone){
					CmdtellSeverToUpdateGameTime(this.gameObject,minutes, seconds);
					sendTimer = 0.5f;
				}
				timerText.text = minutes.ToString() + ":" + seconds.ToString("f0");
				gameTypeText.text = gamemode;
				showVoteMaps();
				showMapOptions(mapAnumber,mapBnumber);
				switchMap();




				//---------------------------------------------ServerTeamDeathMatch/KillComfirm-------------------------------------------------------------------------------------
				if(gamemode == "TeamDeathMatch" || gamemode == "KillComfirm"){
					myTeamText.enabled = true;
					enemyTeamText.enabled = true;
					if(team == "A"){
						myTeamText.text = myColHash + "A" + "</color>";
						enemyTeamText.text = enemyColHash + "B" + "</color>";
						myTeamScoreText.text = myColHash + TeamBaseteamAscore.ToString() + "</color>";
						enemyTeamScoreText.text = enemyColHash + TeamBaseteamBscore.ToString() + "</color>";
					}
					if(team == "B"){
						myTeamText.text = myColHash + "B" + "</color>";
						enemyTeamText.text = enemyColHash + "A" + "</color>"; 
						myTeamScoreText.text = myColHash + TeamBaseteamBscore.ToString() + "</color>";
						enemyTeamScoreText.text = enemyColHash + TeamBaseteamAscore.ToString() + "</color>";
					}
					gameOver = teamDMScores(TeamBaseteamAscore,TeamBaseteamBscore);
				}

				//---------------------------------------------ServerDeathMatch-------------------------------------------------------------------------------------
				if(gamemode == "DeathMatch"){
					myTeamText.enabled = false;
					enemyTeamText.enabled = false;
					enemyDeathMatchScoreText.text = enemyColHash + deathmatchenemyScore().ToString() + "</color>";
					myDeathMatchScoreText.text = myColHash + GetComponent<stats>().kills.ToString() + "</color>";	
					gameOver = deathmatchScore();
				}
			}











			//------------------------------------------------------------CLIENT-----------------------------------------------------------------------------

			if(isClient && !isServer){
				if(host == null && !isFindingHost){isFindingHost = true; StartCoroutine(findHostAndPickTeam());}
				if(host != null){
					showVoteMaps();
					showMapOptions(host.transform.GetComponent<GamemodeManager>().mapAnumber,host.transform.GetComponent<GamemodeManager>().mapBnumber);
					matchisDone = host.transform.GetComponent<GamemodeManager>().matchDone;
					matchminutes =  host.transform.GetComponent<GamemodeManager>().minutes;
					matchseconds =  host.transform.GetComponent<GamemodeManager>().seconds;
					timerText.text = matchminutes.ToString() + ":" + matchseconds.ToString("f0");
					gameTypeText.text = gamemode;

					//---------------------------------------------ClientTeamDeathMatch-------------------------------------------------------------------------------------
					if(gamemode == "TeamDeathMatch" || gamemode == "KillComfirm"){
						matchteamascore = host.GetComponent<GamemodeManager>().TeamBaseteamAscore;
						matchteambscore = host.GetComponent<GamemodeManager>().TeamBaseteamBscore;
						myTeamText.enabled = true;
						enemyTeamText.enabled = true;

						if(team == "A"){
							myTeamText.text = myColHash + "A" + "</color>";
							enemyTeamText.text = enemyColHash + "B" + "</color>";
							myTeamScoreText.text = myColHash + matchteamascore + "</color>";
							enemyTeamScoreText.text = enemyColHash + matchteambscore + "</color>";
						}
						if(team == "B"){
							myTeamText.text = myColHash + "B" + "</color>";
							enemyTeamText.text =  enemyColHash + "A" + "</color>"; 
							myTeamScoreText.text =  myColHash + matchteambscore + "</color>";
							enemyTeamScoreText.text = enemyColHash + matchteamascore + "</color>";
						}
					}

					//--------------------------------------ClientDeathMatch------------------------------------------------------------------------------------------------
					if(gamemode == "DeathMatch"){
						myTeamText.enabled = false;
						enemyTeamText.enabled = false;
						enemyDeathMatchScoreText.text = enemyColHash + deathmatchenemyScore().ToString() + "</color>";
						myDeathMatchScoreText.text = myColHash + GetComponent<stats>().kills.ToString() + "</color>";
					}
				}
			}							
		}
	}








	public IEnumerator findHostAndPickTeam(){
		yield return new WaitForSeconds (Random.Range (0.5f, 1.5f));
		GameObject[] playerlist = GameObject.FindGameObjectsWithTag ("Player");
		int numteamA = 0;
		int numteamB = 0;
		string finalteam = "";

		for(int i = 0 ; i < playerlist.Length; i++){
			if(playerlist[i] != null){
				if(playerlist[i] != this.gameObject){
					if(playerlist[i].transform.GetComponent<GamemodeManager>().isHost){host = playerlist[i];}
					if(playerlist[i].transform.GetComponent<GamemodeManager>().team == "A"){numteamA ++;}
					if(playerlist[i].transform.GetComponent<GamemodeManager>().team == "B"){numteamB ++;}
				}
			}
		}

		if(numteamA > numteamB){ finalteam = "B";}
		if(numteamA < numteamB){ finalteam = "A";}

		if(numteamA == numteamB){if(Random.Range(0,2) == 1){ finalteam = "A";}else{finalteam = "B";}}

		if (host != null) {
			CmdTellServerToSetClientGameMode (this.transform.gameObject, host.transform.GetComponent<GamemodeManager> ().gamemode);
			if (!hasPickedteam) {
				CmdTellServeryourTeam (this.gameObject, finalteam);
				hasPickedteam = true;
			}
		}
		isFindingHost = false;
	}



	[Command]
	public void CmdTellServerToSetClientGameMode(GameObject clint, string gamename){
		clint.GetComponent<GamemodeManager>().gamemode = gamename;
		clint.GetComponent<GamemodeManager>().isHost = false;
	}
		

	[Command]
	public void CmdTellServerToActivateGameMode(GameObject host, string gamename){
		host.GetComponent<GamemodeManager>().gamemode = gamename;
		host.GetComponent<GamemodeManager>().isHost = true;
	}

	[Command]
	public void CmdtellSeverToUpdateGameTime(GameObject host, int min, float sec){
		host.transform.GetComponent<GamemodeManager>().minutes = min;
		host.transform.GetComponent<GamemodeManager>().seconds = sec;
	}

	[Command]
	public void CmdTellServeryourTeam(GameObject myPlayer,string t){
		myPlayer.GetComponent<GamemodeManager>().team = t;
	}

	[Command]
	public void CmdTellMyNetID(GameObject myPlayer, string nID){
		myPlayer.GetComponent<GamemodeManager>().myNetID = nID;
	}

	[Command]
	public void CmdTellServerToTellClientsGameDoneAndVoteMap(GameObject hoster, int mpa, int mpb){
		hoster.transform.GetComponent<GamemodeManager>().mapAnumber = mpa;
		hoster.transform.GetComponent<GamemodeManager>().mapBnumber = mpb;
		hoster.transform.GetComponent<GamemodeManager>().matchDone = true;
	}


	public void PlaceVote(string v){
		if(isLocalPlayer && matchisDone){
			CmdTellServerYourVotePick(v);
		}
	}

	[Command]
	public void CmdTellServerYourVotePick(string vote){
		RpcplaceMyVoteChoice(vote);
	}

	[ClientRpc]
	public void RpcplaceMyVoteChoice(string vp){
		if(voterVoted == null){
			GameObject myvote = Instantiate(voterPrefab);
			if(vp == "mapA"){
				myvote.transform.SetParent(mapAContent,true);	
			}else{
				myvote.transform.SetParent(mapBContent,true);
			}
			voterVoted = myvote;
		}else{
			if(vp == "mapA"){
				voterVoted.transform.SetParent(mapAContent,true);
			}else{
				voterVoted.transform.SetParent(mapBContent,true);
			}	
		}
	}




	public void showVoteMaps(){
		if(matchisDone == true){
			/*
			GetComponent(enabler).disableAllmovements();
			GetComponent(enabler).disableAllWeapons();
			GetComponent(enabler).disableStreakscallIn();
			GetComponent(enabler).disableminiMap();
			GetComponent(enabler).disablemouseScript();
			GetComponent(enabler).disableWeaponAmmoText();
			GetComponent(enabler).disablematchInformationUI();
			GetComponent(PlayerSyncMovement).enabled = false;
			GetComponent(PlayerSyncRotation).enabled = false;
			GetComponent(PlayerHealthNetwork).enabled = false;
			*/
			if(!playedEndingAnimation){
				StartCoroutine (playTop3PlayersAnimation());
				playedEndingAnimation = true;
			}
		}
	}


	public IEnumerator playTop3PlayersAnimation(){
		waitTime = Time.time + 10f; 
		string firstname = "";
		string firstkillsNDeath= "";
		int firstRank = 0;
		string secondname= "";
		string secondkillsNDeath= "";
		int secondRank = 0;
		string thirdname= "";
		string thirdkillsNDeath= "";
		int thirdRank = 0;
		int numPlayers = 0;

		int topkill = 0;
		for(int i = 0; i<GetComponent<playerName>().players.Length; i++){
			if(GetComponent<playerName>().players[i] != null){
				numPlayers ++;
				if(GetComponent<playerName>().players[i].transform.GetComponent<stats>().kills >= topkill){
					topkill = GetComponent<playerName>().players[i].transform.GetComponent<stats>().kills;
					firstname = GetComponent<playerName>().players[i].transform.GetComponent<playerName>().pname;
					firstkillsNDeath = "KILL: " + GetComponent<playerName>().players[i].transform.GetComponent<stats>().kills.ToString() + " Death: " + GetComponent<playerName>().players[i].transform.GetComponent<stats>().death.ToString();
					firstRank = GetComponent<playerName>().players[i].transform.GetComponent<GamemodeManager>().myRank;
				}
			}
		}

		int secondkill = 0;
		for(int j = 0; j<GetComponent<playerName>().players.Length; j++){
			if(GetComponent<playerName>().players[j] != null){
				if(GetComponent<playerName>().players[j].transform.GetComponent<stats>().kills >= secondkill && GetComponent<playerName>().players[j].transform.GetComponent<stats>().kills < topkill ){
					secondkill = GetComponent<playerName>().players[j].transform.GetComponent<stats>().kills;
					secondname = GetComponent<playerName>().players[j].transform.GetComponent<playerName>().pname;
					secondkillsNDeath = "KILL: " +GetComponent<playerName>().players[j].transform.GetComponent<stats>().kills.ToString() + " Death: " + GetComponent<playerName>().players[j].transform.GetComponent<stats>().death.ToString();
					secondRank = GetComponent<playerName>().players[j].transform.GetComponent<GamemodeManager>().myRank;
				}
			}
		}



		int thirdkill = 0;
		for(int k = 0; k<GetComponent<playerName>().players.Length; k++){
			if(GetComponent<playerName>().players[k] != null){
				if(GetComponent<playerName>().players[k].transform.GetComponent<stats>().kills >= thirdkill && GetComponent<playerName>().players[k].transform.GetComponent<stats>().kills < secondkill){
					thirdkill = GetComponent<playerName>().players[k].transform.GetComponent<stats>().kills;
					thirdname = GetComponent<playerName>().players[k].transform.GetComponent<playerName>().pname;
					thirdkillsNDeath = "KILL: " + GetComponent<playerName>().players[k].transform.GetComponent<stats>().kills.ToString() + " Death: " + GetComponent<playerName>().players[k].transform.GetComponent<stats>().death.ToString();
					thirdRank = GetComponent<playerName>().players[k].transform.GetComponent<GamemodeManager>().myRank;
				}
			}
		}



		cvmanager.firstplaceNameText.text = firstname;
		cvmanager.firstplaceKillNDeathText.text = firstkillsNDeath;

		cvmanager.secondplaceNameText.text = secondname;
		cvmanager.secondplaceKillNDeathText.text = secondkillsNDeath;


		cvmanager.thirdplaceNameText.text = thirdname;
		cvmanager.thirdplaceKillNDeathText.text = thirdkillsNDeath;



		if(numPlayers > 0){
			cvmanager.firstplaceNameText.enabled = true;
			cvmanager.firstplaceKillNDeathText.enabled = true;
		}

		if(numPlayers > 1){
			cvmanager.secondplaceNameText.enabled = true;
			cvmanager.secondplaceKillNDeathText.enabled = true;
		}

		if(numPlayers > 2){
			cvmanager.thirdplaceNameText.enabled = true;
			cvmanager.thirdplaceKillNDeathText.enabled = true;
		}



		int scorOfAscore= host.GetComponent<GamemodeManager>().TeamBaseteamAscore;
		int scorOfBscore = host.GetComponent<GamemodeManager>().TeamBaseteamBscore;
		int myDMscore = GetComponent<stats>().kills;
		int enemyDmscore = deathmatchenemyScore();
		cvmanager.gameWinnerName.enabled = true;
		if(isTeamGame ){
			if(team == "A"){
				if(scorOfAscore > scorOfBscore){
					cvmanager.gameWinnerName.color = Color.green;
					cvmanager.gameWinnerName.text = "Your Team Won";
					winsave += 1;
					PlayerPrefs.SetInt("MyWin", winsave);
					currentgamewinstreak += 1;	
					PlayerPrefs.SetInt("CurrentWinStreak" , currentgamewinstreak);
					if(currentgamewinstreak >= highestwinstreak){
						highestwinstreak += 1;
						PlayerPrefs.SetInt("MyWinStreak", highestwinstreak);
					}

				}
				if(scorOfAscore < scorOfBscore){
					cvmanager.gameWinnerName.color = Color.red;
					cvmanager.gameWinnerName.text = "Your Team Lost";
					losssave += 1;
					PlayerPrefs.SetInt("MyLoss", losssave);
					PlayerPrefs.SetInt("CurrentWinStreak" , 0);
				}
				if(scorOfAscore == scorOfBscore){
					cvmanager.gameWinnerName.color = Color.grey;
					cvmanager.gameWinnerName.text = "Tie Game";
					tiessave += 1;
					PlayerPrefs.SetInt("MyTies", tiessave);
					PlayerPrefs.SetInt("CurrentWinStreak" , 0);
				}
			}

			if(team == "B"){
				if(scorOfBscore > scorOfAscore){
					cvmanager.gameWinnerName.color = Color.green;
					cvmanager.gameWinnerName.text = "Your Team Won";
					winsave += 1;
					PlayerPrefs.SetInt("MyWin", winsave);
					currentgamewinstreak += 1;	
					PlayerPrefs.SetInt("CurrentWinStreak" , currentgamewinstreak);
					if(currentgamewinstreak >= highestwinstreak){
						highestwinstreak += 1;
						PlayerPrefs.SetInt("MyWinStreak", highestwinstreak);
					}
				}
				if(scorOfBscore < scorOfAscore){
					cvmanager.gameWinnerName.color = Color.red;
					cvmanager.gameWinnerName.text = "Your Team Lost";
					losssave += 1;
					PlayerPrefs.SetInt("MyLoss", losssave);
					PlayerPrefs.SetInt("CurrentWinStreak" , 0);
				}
				if(scorOfBscore == scorOfAscore){
					cvmanager.gameWinnerName.color = Color.grey;
					cvmanager.gameWinnerName.text = "Tie Game";
					tiessave += 1;
					PlayerPrefs.SetInt("MyTies", tiessave);
					PlayerPrefs.SetInt("CurrentWinStreak" , 0);
				}
			}
		}


		if(!isTeamGame){
			if(myDMscore > enemyDmscore){
				cvmanager.gameWinnerName.color = Color.green;
				cvmanager.gameWinnerName.text = "You Won";
				winsave += 1;
				PlayerPrefs.SetInt("MyWin", winsave);
				currentgamewinstreak += 1;	
				PlayerPrefs.SetInt("CurrentWinStreak" , currentgamewinstreak);
				if(currentgamewinstreak >= highestwinstreak){
					highestwinstreak += 1;
					PlayerPrefs.SetInt("MyWinStreak", highestwinstreak);
				}
			}

			if(myDMscore < enemyDmscore){
				cvmanager.gameWinnerName.color = Color.red;
				cvmanager.gameWinnerName.text = "You Lose";
				losssave += 1;
				PlayerPrefs.SetInt("MyLoss", losssave);
				PlayerPrefs.SetInt("CurrentWinStreak" , 0);
			}

			if(myDMscore == enemyDmscore){
				cvmanager.gameWinnerName.color = Color.grey;
				cvmanager.gameWinnerName.text = "Tie Game";
				tiessave += 1;
				PlayerPrefs.SetInt("MyTies", tiessave);
				PlayerPrefs.SetInt("CurrentWinStreak" , 0);
			}

		}
		topPlayerPlanel.transform.GetComponent<Animation>().Play("TOP");
		yield return new WaitForSeconds(10f);
		topPlayerPlanel.SetActive (false);
		voteMapHeader.enabled = true;
		voteAMap.transform.GetComponent<UnityEngine.UI.Image>().enabled = true;
		voteBMap.transform.GetComponent<UnityEngine.UI.Image>().enabled = true;
		voteAname.enabled = true;
		voteBname.enabled = true;
	}



	public void showMapOptions(int op1, int op2){
		if(matchisDone){
			if(op1 >= 0 && op1 < availableMaps.Length && op2 >= 0 && op2 < availableMaps.Length){
				voteAname.text = availableMaps[op1];
				voteBname.text = availableMaps[op2];
				mapA = availableMaps[op1];
				mapB = availableMaps[op2];
			}
			if(op1 >= 0 && op1 < mapSprites.Length && op2 >= 0 && op2 < mapSprites.Length){
				voteAMap.transform.GetComponent<UnityEngine.UI.Image>().sprite = mapSprites[op1];
				voteBMap.transform.GetComponent<UnityEngine.UI.Image>().sprite = mapSprites[op2];
			}
		}
	}


	public void switchMap(){
		if(matchDone){
			voteTimer -= Time.deltaTime * 1;
			if(voteTimer <= 0 && !hasSwitchMap){
				if(mapAContent.childCount > mapBContent.childCount){
					if(netmanager != null){netmanager.ServerChangeScene(mapA);}
				}else{
					if (netmanager != null) {netmanager.ServerChangeScene (mapB);}
				}
				hasSwitchMap = true;
			}
		}
	}


	[Command]
	public void CmdTellSetmyRank(GameObject myPlayer, int lvl){
		myPlayer.GetComponent<GamemodeManager>().myRank = lvl;
	}


	public void OnMinutesChange(int min){
		minutes = min;
	}
	public void OnSecondsChange(float sec){
		seconds = sec;
	}
	public void OnServerStart(bool hos){
		isHost = hos;
	}

	public void OnGameStart(string mode){
		gamemode = mode;
		if(isLocalPlayer){
			if(mode == "TeamDeathMatch" || mode == "KillComfirm"){ isTeamGame = true;}else{isTeamGame = false;}
			GameObject.Find("Canvas").GetComponent<killfeedManager>().isTeamgame = isTeamGame;
			GameObject.Find("Canvas").GetComponent<hudManager>().isTeamGame = isTeamGame;
		}
	}

	public void OnChooseTeam(string onteam){
		team = onteam;
		if(isLocalPlayer){
			GameObject.Find("Canvas").GetComponent<killfeedManager>().myTeam = onteam;
			GameObject.Find("Canvas").GetComponent<hudManager>().mTeam = onteam;
		}
	}

	public void OnGetNetID(string netiD){
		myNetID = netiD;
		if(isLocalPlayer){
			GameObject.Find("Canvas").GetComponent<killfeedManager>().myNetID = netiD;
		}
	}

	public void OnMatchOver(bool matchover){
		matchDone = matchover;
	}

	public void OnGetRank(int rank){
		myRank = rank;
	}

	public void OnMapA(int x){
		mapAnumber = x;
	}

	public void OnMapB(int x){
		mapBnumber = x;
	}


	public void OnTeamAScore(int x){
		TeamBaseteamAscore = x;
	}


	public void OnTeamBScore(int x){
		TeamBaseteamBscore = x;
	}


	[Command]
	public void CmdTellIncreaseScoreTeamA(int scrA, int scrB, GameObject h){
		h.GetComponent<GamemodeManager>().TeamBaseteamAscore += scrA;
		h.GetComponent<GamemodeManager>().TeamBaseteamBscore += scrB;
	}


	public void TDMKill(){
		if(gamemode == "TeamDeathMatch"){
			if(team == "A" && host != null){
				CmdTellIncreaseScoreTeamA(1, 0, host);
			}
			if(team == "B" && host != null){
				CmdTellIncreaseScoreTeamA(0, 1, host);
			}
		}
	}

	public void KCTAG(){
		if(gamemode == "KillComfirm"){
			if(team == "A" && host != null){
				CmdTellIncreaseScoreTeamA(1, 0, host);
			}
			if(team == "B" && host != null){
				CmdTellIncreaseScoreTeamA(0, 1, host);
			}
		}
	}

	public bool teamDMScores(int scoreA, int scoreB){
		bool matchisover = false;
		if(scoreA >= gameScoreLimit || scoreB >= gameScoreLimit){
			matchisover = true;
		}
		return matchisover;
	}

	public int deathmatchenemyScore(){
		int emyScore0 = 0;
		for(int j = 0; j<GetComponent<playerName>().players.Length; j++){
			if(GetComponent<playerName>().players[j] != null){
				if(GetComponent<playerName>().players[j] != this.gameObject && GetComponent<playerName>().players[j].transform.GetComponent<stats>().kills >= emyScore0){
					emyScore0 = GetComponent<playerName>().players[j].transform.GetComponent<stats>().kills;
				}
			}
		}
		return emyScore0;
	}


	public bool deathmatchScore(){
		bool foundwinner = false;
		for(int i = 0; i < playerNameScript.players.Length; i++){
			if(playerNameScript.players[i] != null){
				if(playerNameScript.players[i].GetComponent<stats>().kills >= gameScoreLimit){
					foundwinner = true;
				}
			}
		}
		return foundwinner;
	}













}
