using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/*
 * The GameManager manages the overall game state.
 */
public class GameManager : MonoBehaviour {
    public enum GameState {
        START,
        GAME
    }

    public static GameManager s;
    [System.NonSerialized]
    public GameState gameState = GameState.START;
    public Scene scene;
    public PhysicsScene2D physicsScene;

    private void Awake() {
        s = this;
        LeanTween.init(20000);
    }
    private void Start() {      
        scene = SceneManager.GetActiveScene();
        physicsScene = PhysicsSceneExtensions2D.GetPhysicsScene2D(scene);
    }
    private void OnGameStart() {
        gameState = GameState.GAME;
        Instantiate(PrefabManager.s.runPrefab);
        Instantiate(PrefabManager.s.mineUIItemPrefab);
        GameObject brain = Instantiate(PrefabManager.s.flagPrefab);
        brain.AddComponent<Brain>();
        GameObject you = Instantiate(PrefabManager.s.flagPrefab); 
        You youComponent = you.AddComponent<You>();
        youComponent.count = 8;
        GameObject baseFlag = Instantiate(PrefabManager.s.flagPrefab);
        Base baseComponent = baseFlag.AddComponent<Base>();
        baseComponent.count = 10;
        HelperManager.s.DelayAction(() => { Floor.s.IntroAndCreateFloor("minefield", 0); }, 1f);
    }
    private void OnGameLoad() {
        gameState = GameState.GAME;
        Instantiate(PrefabManager.s.runPrefab);
        Instantiate(PrefabManager.s.mineUIItemPrefab);
        LoadData loadData = SaveManager.s.GetLoadData();
        Player.s.Load(loadData);
        Floor.s.InitLayout(loadData.width, loadData.height);
        PlayerUIItemModule.s.LoadUIItems(loadData);
        HelperManager.s.DelayAction(() => { Floor.s.IntroAndLoadFloor(loadData); }, 1f);
    }
    private void OnEnable() {
        EventManager.s.OnGameStart += OnGameStart;
        EventManager.s.OnGameLoad += OnGameLoad;
    }
    private void OnDisable() {
        EventManager.s.OnGameStart -= OnGameStart;
        EventManager.s.OnGameLoad -= OnGameLoad;
    }

}
