using UnityEngine;
using System.Collections;

public enum GameState {
	None=-1,
	Starting,
	Connecting
}

public enum EngineVarType {
}
	

public class GameEngine : MonoBehaviour 
{
	static GameEngine _this;
	
	public GameState gameState;

	public static GameState State {
		get { if (_this == null) return GameState.None; 
			return _this.gameState; }
		set { 
			_this.gameState = value;
			TouchMonitor.SetActiveHandlers();
		}
	}
	
	float gameplayTime;
	
	public static float PlayTime { get { return _this.gameplayTime; } }
	public static void Save() {
		if (_this == null) return;
		_this.SaveEngineVars();
	}
	
	void Awake () {
		_this = this;
	}
	
	void Start() {
//		State = GameState.Starting;	
//		ResetEngineVars();
//		TestingEngineVars();
		LoadEngineVars();
		Invoke("FinishedLoading",0.25f);
	}
	
	void FinishedLoading() {
//		Loader.FinishedLoading();	
	}
	
	void OnApplicationQuit() {
		SaveEngineVars();	
	}
	
	void LoadEngineVars() {
//		float hp;
//		int exp, money;
//		
//		if (PlayerPrefs.HasKey("gameplayTime")) {
//			gameplayTime = PlayerPrefs.GetFloat("gameplayTime");
//			hp = PlayerPrefs.GetFloat("playerCurrentHp");
//			exp = PlayerPrefs.GetInt("playerCurrentXp");
//			money = PlayerPrefs.GetInt("playerCurrentMoney");
//			PlayerStats.SetDirectly(hp, exp, money);
//			PlayerStats.Lvl = ExpToLevel.GetLevelFromExperience(PlayerStats.Exp);
//			PlayerStats.MaxHp = PlayerStats.SetMaxHpFromLevel(PlayerStats.Lvl);
//			SkillTree.LoadSkillTree(PlayerPrefs.GetString("playerSkillTree"));
//		} else {  
//			Debug.Log (Time.realtimeSinceStartup + " gameplayTime not found - new game?");
//			gameplayTime = 0;
//			hp = 30;
//			exp = 0;
//			money = 0;
//			PlayerStats.SetDirectly(hp, exp, money);
//			PlayerStats.Lvl = ExpToLevel.GetLevelFromExperience(PlayerStats.Exp);
//			PlayerStats.MaxHp = PlayerStats.SetMaxHpFromLevel(PlayerStats.Lvl);
//			SkillTree.LoadSkillTree("0;0;0;0;0;0;0;0;0;0;0");
//		}
//
//		if (PlayerPrefs.HasKey("playerCurrentXp")) {
//			PlayerStats.ExpNoLvlup = PlayerPrefs.GetInt("playerCurrentXp");
//		} else {
//			Debug.Log (Time.realtimeSinceStartup + " not found xp");
//			PlayerStats.ExpNoLvlup = 0;	
//		}
//		
//		if (PlayerPrefs.HasKey("playerCurrentMoney")) {
//			PlayerStats.Money = PlayerPrefs.GetInt("playerCurrentMoney");
//		} else {
//			Debug.Log (Time.realtimeSinceStartup + " not found money");
//			PlayerStats.Money = 0;	
//		}
//		
//		
//		if (PlayerPrefs.HasKey("playerCurrentHp")) {
//			PlayerStats.Hp = PlayerPrefs.GetFloat("playerCurrentHp");
//		} else { 
//			PlayerStats.Hp = PlayerStats.MaxHp;	
//		}
//		
//		if (PlayerPrefs.HasKey("playerSkillTree")) {
//		} 
	}
	
//	void TestingEngineVars() {
//		PlayerPrefs.SetFloat("gameplayTime", 0);
//		PlayerPrefs.SetFloat("playerCurrentHp", 30);
//		PlayerPrefs.SetInt("playerCurrentXp", 0);
//		PlayerPrefs.SetInt("playerCurrentMoney", 0);
//		PlayerPrefs.SetString("playerSkillTree", "0;0;0;0;0;0;0;0;0;0;0");
//	}
//	
//	void ResetEngineVars() {
//		PlayerPrefs.SetFloat("gameplayTime", 0);
//		PlayerPrefs.SetFloat("playerCurrentHp", 30);
//		PlayerPrefs.SetInt("playerCurrentXp", 0);
//		PlayerPrefs.SetInt("playerCurrentMoney", 0);
//		PlayerPrefs.SetString("playerSkillTree", "0;0;0;0;0;0;0;0;0;0;0");
//	}
	
	void SaveEngineVars() {
//		PlayerPrefs.SetFloat("gameplayTime", gameplayTime + Time.realtimeSinceStartup);
//		PlayerPrefs.SetFloat("playerCurrentHp", PlayerStats.Hp);
//		PlayerPrefs.SetInt("playerCurrentXp", PlayerStats.Exp);
//		PlayerPrefs.SetInt("playerCurrentMoney", PlayerStats.Money);
//		PlayerPrefs.SetString("playerSkillTree", SkillTree.SaveSkillTree());
	}
	
	public static void SaveVar(EngineVarType engineVar) {
//		_this.saveEngineVar(engineVar);	
	}
	
	void saveEngineVar(EngineVarType engineVar) {
//		switch (engineVar) {
//		case EngineVarType.Hp:
//			PlayerPrefs.SetFloat("playerCurrentHp", PlayerStats.Hp);
//			break;
//		case EngineVarType.Xp:
//			PlayerPrefs.SetInt("playerCurrentXp", PlayerStats.Exp);
//			break;
//		case EngineVarType.Money:
//			PlayerPrefs.SetInt("playerCurrentMoney", PlayerStats.Money);
//			break;
//		case EngineVarType.SkillTree:
//			PlayerPrefs.SetString("playerSkillTree", SkillTree.SaveSkillTree());
//			break;
//		}
//		PlayerPrefs.SetFloat("gameplayTime", gameplayTime + Time.realtimeSinceStartup);
	}
}
