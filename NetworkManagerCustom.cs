using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager {
	public NetworkMatch networkMatch;
	public 	GameObject Joinmatch;
	public 	MenuSceneManager MenuMangerScript;
	private bool showmatches = false;
	private bool hascreatedGame= false;
	private bool hasjoinGame = false;
	private bool addedPlayer; 
	private bool counter= false;
	private bool lostConnectionToHost = false;
	private float autorefreshTimer = 15f;
	public List<GameObject> matchList = new List<GameObject>();

	void Start(){
		StartCoroutine (SetupMenuScenceButtons());
	}

	public void OnError(NetworkMessage netMsg){
		Debug.Log("You Got An Error");
		hasjoinGame = false;
		hascreatedGame = false;
		if(MenuMangerScript != null){MenuMangerScript.loadanimation.SetActive(false);MenuMangerScript.OnAnimatedErrorText("unable to join");}
	}





	void Update(){
		if(networkMatch == null){
			var nm = GetComponent<NetworkMatch>();
			if(nm != null){
				networkMatch = nm as NetworkMatch;
			}
		}

		if(NetworkManager.singleton.client != null && !counter){
			NetworkManager.singleton.client.RegisterHandler(MsgType.Error, OnError);
			NetworkManager.singleton.client.RegisterHandler(MsgType.Disconnect, OnError);
		}


		if(networkMatch != null && showmatches){
			availableMatches();
		}


		if(ClientScene.ready){
			hascreatedGame = false;
			hasjoinGame = false;
		}

		if(!ClientScene.ready && networkMatch != null && MenuMangerScript != null){
			if(MenuMangerScript.currentGamesPanel.activeInHierarchy){autoRefresh();}else{autorefreshTimer = 17f;}
		}

		if(lostConnectionToHost){
			if(MenuMangerScript != null){MenuMangerScript.loadanimation.SetActive(false);MenuMangerScript.OnAnimatedErrorText("Lost Connection to Host"); lostConnectionToHost = false;}
		}


		if(networkMatch == null){NetworkManager.singleton.StartMatchMaker();} 

	}


	public override void  OnClientSceneChanged(NetworkConnection conn){
		if (!this.autoCreatePlayer)
		{
			return;
		}
		if(ClientScene.ready)
		{
			Debug.Log("There is Already a client for this connection!");
			return;
		}
		ClientScene.Ready(conn);
		bool flag = ClientScene.localPlayers.Count == 0;
		bool flag2= false;
		foreach( PlayerController current in ClientScene.localPlayers)
		{
			if (current.gameObject != null)
			{
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			flag = true;
		}
		if (flag)
		{
			ClientScene.AddPlayer (0);
		}
	}


	public void refresh (){
		showmatches = false;
		autorefreshTimer = 17f;
		clearMatches();
		MenuMangerScript.JoinAMatchButton.GetComponent<Button>().onClick.RemoveAllListeners();
		MenuMangerScript.JoinAMatchButton.GetComponent<Image>().color = Color.white;
		listMatcheMakingMatch();
		showmatches = true;
		Debug.Log("Match Refreshed!");
	}


	public void clearMatches(){
		for(int i = 0; i < matchList.Count; i++){
			Destroy(matchList[i]);
		}
		matchList.Clear();
	}


	public void  startupMatchMaking(){
		NetworkManager.singleton.StartMatchMaker();
		networkMatch = NetworkManager.singleton.matchMaker;
	}

	public void  stopMatchMaking(){
		NetworkManager.singleton.StopMatchMaker();
	}


	public void  createAMatchMakingMatch(){
		if(networkMatch != null && !hascreatedGame && !hasjoinGame && !ClientScene.ready){
			MenuMangerScript.erroranimation.SetActive(false);
			MenuMangerScript.loadanimation.SetActive(true);
			mapselect selectMap = MenuMangerScript.mapselectscript;
			NetworkManager.singleton.onlineScene = selectMap.mapnames[selectMap.mapnum];
			string mapname = selectMap.mapnames[selectMap.mapnum];
			uint maxplayers = (uint)selectMap.totalplayers;
			string gameType = selectMap.gametypes[selectMap.numgameType];
			string gameName = selectMap.gameName + ":" + gameType + ":" + mapname;

			hascreatedGame = true;
			networkMatch.CreateMatch(gameName, maxplayers,true,"","","",0,1,OnMatchCreate);

		}
	}


	public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
		if (success) {
			Debug.Log("SUCCESS MATCHCREATED!");
			StartHost(matchInfo);
		}else{
			Debug.Log("FAILD TO CREATE MATCH!");
			MenuMangerScript.loadanimation.SetActive(false);			
			MenuMangerScript.OnAnimatedErrorText("unable to create match");
			hascreatedGame = false;
		}
	}


	public void availableMatches(){
		if(NetworkManager.singleton.matches != null){
			if(NetworkManager.singleton.matches.Count > 0){
				foreach(MatchInfoSnapshot match in NetworkManager.singleton.matches){
					GameObject cmatch = Instantiate(Joinmatch);
					cmatch.transform.SetParent(MenuMangerScript.findgamescontent, false);
					string matchname = match.name;

					string[] matchDetail = matchname.Split(":"[0]);
					cmatch.transform.GetComponent<JoinInfo>().joinName.text = matchDetail[0];
					cmatch.transform.GetComponent<JoinInfo>().joinType.text = matchDetail[1]  + " (" + matchDetail[2] + ")";
					cmatch.transform.GetComponent<JoinInfo>().joinPlayers.text = match.currentSize + "/" + match.maxSize;
					cmatch.transform.GetComponent<Button>().onClick.RemoveAllListeners();
					cmatch.transform.GetComponent<Button>().onClick.AddListener(
						delegate {
							Debug.Log(match.maxSize);
							AddListenerMatch(match);
						});
					matchList.Add(cmatch);
				}
				showmatches = false;
			}
		}
	}



	public void AddListenerMatch(MatchInfoSnapshot gameMatch){
		MenuMangerScript.JoinAMatchButton.GetComponent<Button>().onClick.RemoveAllListeners();
		MenuMangerScript.JoinAMatchButton.GetComponent<Button>().onClick.AddListener(
			delegate {
				if(!hasjoinGame && !hascreatedGame &&!ClientScene.ready){
					hasjoinGame = true; 
					MenuMangerScript.loadanimation.SetActive(true);
					MenuMangerScript.erroranimation.SetActive(false);
					networkMatch.JoinMatch(gameMatch.networkId, "","","",0,1,OnMatchJoined);
				}
			});
	}


	public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo){ 
		if (success){ 
			StartClient(matchInfo);
			Debug.Log("JOIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIINNNNNNNNNNNNNNNNNNNNNNED");
		} else if (LogFilter.logError){ 
			Debug.LogError("Join Failed:" + matchInfo);
			MenuMangerScript.loadanimation.SetActive(false);
			MenuMangerScript.OnAnimatedErrorText("unable to join");
			hasjoinGame = false;
		} 
	}






	public void listMatcheMakingMatch(){
		MenuMangerScript.currentGamesPanel.SetActive(true);
		MenuMangerScript.multiStage.SetActive(false);
		if(networkMatch != null){networkMatch.ListMatches(0,20,"",true,0,1,OnMatchList);showmatches = true;}
	}




	public void autoRefresh(){
		autorefreshTimer -= Time.deltaTime;
		if(autorefreshTimer < 0){
			clearMatches();
			if(networkMatch != null){networkMatch.ListMatches(0,20,"",true,0,1,OnMatchList);showmatches = true;}
			autorefreshTimer = 17f;
			Debug.Log("autoRefresh");
		}
	}		





	public override void OnClientDisconnect(NetworkConnection conn){
		StopClient();
		Debug.Log("Lost Connection to Host");
		lostConnectionToHost = true;
	} 
		

	public void backToMultiStage(){
		clearMatches();
		showmatches = false;
		MenuMangerScript.multiStage.SetActive(true);
		MenuMangerScript.currentGamesPanel.SetActive(false);
		MenuMangerScript.JoinAMatchButton.GetComponent<Button>().onClick.RemoveAllListeners();
		MenuMangerScript.JoinAMatchButton.GetComponent<Image>().color = Color.white;
	}


	///------------------------------LOCAL HOST----------------------------------------------
	public void  startupHost(){
		setPort();
		NetworkManager.singleton.StartHost();
	}


	public void  joinGame(){
		setPort();
		NetworkManager.singleton.StartClient();
	}

	public void  setIp(){
		//var typeIpAdress = GameObject.Find("InputFieldIPAdress").transform.FindChild("Text").GetComponent(Text).text;
	}

	public void  setPort(){
		NetworkManager.singleton.networkPort = 9321;
	}


	public void OnLevelWasLoaded(int level){
		if(level == 0){
			StartCoroutine (SetupMenuScenceButtons());
			counter = false;
		}else{
			SetupOtherScenceButtons();
			counter = true;
		}
	}



	public IEnumerator SetupMenuScenceButtons(){
		yield return new WaitForSeconds(0.3f);
		if(GameObject.Find("MenuScenceManager") != null){
			
			MenuMangerScript = GameObject.Find("MenuScenceManager").GetComponent<MenuSceneManager>();
			MenuMangerScript.HostGameButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.HostGameButton.GetComponent<Button>().onClick.AddListener(startupHost);

			MenuMangerScript.JoinGameButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.JoinGameButton.GetComponent<Button>().onClick.AddListener(joinGame);

			MenuMangerScript.MultiplayerButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.MultiplayerButton.GetComponent<Button>().onClick.AddListener(startupMatchMaking);

			MenuMangerScript.CreateMatchButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.CreateMatchButton.GetComponent<Button>().onClick.AddListener(createAMatchMakingMatch);

			MenuMangerScript.FindGameButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.FindGameButton.GetComponent<Button>().onClick.AddListener(listMatcheMakingMatch);

			MenuMangerScript.MultiToManuButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.MultiToManuButton.GetComponent<Button>().onClick.AddListener(stopMatchMaking);

			MenuMangerScript.RefreshButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.RefreshButton.GetComponent<Button>().onClick.AddListener(refresh);

			MenuMangerScript.BackButton.GetComponent<Button>().onClick.RemoveAllListeners();
			MenuMangerScript.BackButton.GetComponent<Button>().onClick.AddListener(backToMultiStage);

			//GetComponent(musicManager).enabled = true;
			GetComponent<AudioSource>().enabled = true;
		}
	}




	public void SetupOtherScenceButtons(){
		canvasManager cvmanager = GameObject.Find("Canvas").GetComponent<canvasManager>();
		cvmanager.mainMenuButton.onClick.RemoveAllListeners();
		cvmanager.mainMenuButton.onClick.AddListener(NetworkManager.singleton.StopHost);
		//GetComponent(musicManager).enabled = false;
		//GetComponent(AudioSource).enabled = false;
		//GetComponent(musicManager).timer = 0f;
	}





















}
