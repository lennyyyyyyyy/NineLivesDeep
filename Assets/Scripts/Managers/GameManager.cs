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
}
