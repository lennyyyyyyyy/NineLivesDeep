using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/*
 * The GameManager manages the overall game state.
 */
public class GameManager : MonoBehaviour {
    public static GameManager s;
    [System.NonSerialized]
    public int START = 0, GAME = 1;
    [System.NonSerialized]
    public int gamestate = 0;
    public float deathReviveDuration = 3;
    public static Action OnSTARTToGAME;
    public Scene scene;
    public PhysicsScene2D physicsScene;
    [System.NonSerialized]
	public HashSet<Collider2D> collidersUnderMouse = new HashSet<Collider2D>(),
		   					   collidersUnderMouseLastFrame = new HashSet<Collider2D>();
    private void Awake() {
        s = this;
        LeanTween.init(20000);
    }
    private void Start() {      
        scene = SceneManager.GetActiveScene();
        physicsScene = PhysicsSceneExtensions2D.GetPhysicsScene2D(scene);
    }
    private void Update() {
		collidersUnderMouseLastFrame = new HashSet<Collider2D>(collidersUnderMouse);
        collidersUnderMouse = new HashSet<Collider2D>(Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

		// custom mouse events
        foreach (Collider2D c in collidersUnderMouse) {
			if (!collidersUnderMouseLastFrame.Contains(c)) {
                c.SendMessage("OnMouseEnterCustom", SendMessageOptions.DontRequireReceiver);
            }
			if (Input.GetMouseButtonDown(0)) {
				c.SendMessage("OnMouseDownCustom", SendMessageOptions.DontRequireReceiver);
			}
        }
		foreach (Collider2D c in collidersUnderMouseLastFrame) {
			if (c != null && !collidersUnderMouse.Contains(c)) {
                c.SendMessage("OnMouseExitCustom", SendMessageOptions.DontRequireReceiver);
            }
        }

    }
    public void STARTToGAME() {
        OnSTARTToGAME?.Invoke();
    }
}
