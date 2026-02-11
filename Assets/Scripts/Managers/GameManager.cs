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
        GAME,
        FLOOR_STABLE,
        FLOOR_UNSTABLE,
    }

    public static GameManager s;
    [System.NonSerialized]
    public GameState gameState = GameState.START, 
                     floorGameState = GameState.FLOOR_UNSTABLE;
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
        Instantiate(PrefabManager.s.flagPrefab).AddComponent<Brain>();
        Instantiate(PrefabManager.s.flagPrefab).AddComponent<You>().Init(initialCount: 8);
        Instantiate(PrefabManager.s.flagPrefab).AddComponent<Base>().Init(initialCount: 10);
        Instantiate(PrefabManager.s.flagPrefab).AddComponent<Exit>();
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
    private void OnGameExit() {
        gameState = GameState.START;
        SaveManager.s.Save();
        PlayerUIItemModule.s.DestroyAllUIItemsWithoutProcessing();
        Destroy(Run.s.gameObject);
    }
    private void OnEnable() {
        EventManager.s.OnGameStart += OnGameStart;
        EventManager.s.OnGameLoad += OnGameLoad;
        EventManager.s.OnGameExit += OnGameExit;
    }
    private void OnDisable() {
        EventManager.s.OnGameStart -= OnGameStart;
        EventManager.s.OnGameLoad -= OnGameLoad;
        EventManager.s.OnGameExit -= OnGameExit;
    }

}
