using UnityEngine;
using System;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {
    public static EventManager s;

    public Action OnFloorChangeBeforeIntro,
                  OnFloorChangeBeforeNewLayout,
                  OnFloorChangeBeforeEntities,
                  OnFloorChangeAfterEntities,
                  OnNewMinefield,
                  OnPlayerDie,
                  OnPlayerRevive,
                  OnPlayerAliveChange,
                  OnUpdateSecondaryMapActive,
                  OnGameStart,
                  OnGameLoad,
                  OnGameExit;
    public Action<int, int, GameObject> OnExplosionAtCoord;
    public Action<int, int> OnPlayerMoveToCoord;

	private HashSet<Collider2D> collidersUnderMouse = new HashSet<Collider2D>(),
                                collidersUnderMouseLastFrame = new HashSet<Collider2D>();
    private void Awake() {
        s = this;
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
			if (Input.GetMouseButtonUp(0)) {
				c.SendMessage("OnMouseUpCustom", SendMessageOptions.DontRequireReceiver);
			}
        }
		foreach (Collider2D c in collidersUnderMouseLastFrame) {
			if (c != null && !collidersUnderMouse.Contains(c)) {
                c.SendMessage("OnMouseExitCustom", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
