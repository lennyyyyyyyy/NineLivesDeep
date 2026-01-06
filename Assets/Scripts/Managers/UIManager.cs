using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class UIManager : MonoBehaviour {
    public static UIManager s;

    public GameObject canvas; 
    public Volume ppv;

    [System.NonSerialized]
    public RectTransform canvasRt;
    [System.NonSerialized]
    public Vector3 lastMousePos, mouseVelocity = Vector2.zero;
    private float mouseTimer = 0;
    [System.NonSerialized]
    public Material alphaEdgeBlueMat,
                    tileNormalMat,
                    tileExitMat,
                    tileTrialMat,
                    tilePuddleMat,
                    tileMossyMat;
	[System.NonSerialized]
	public Sprite playerSprite,
                  playerTrappedSprite,
                  mineDebugSprite;
    [System.NonSerialized]
    public VolumeProfile ppvp;

    private void Awake() {
        s = this;

        alphaEdgeBlueMat = HelperManager.s.LoadResourceSafe<Material>("Materials/AlphaEdgeBlue");
		tileNormalMat = HelperManager.s.LoadResourceSafe<Material>("Materials/TileNormal");
		tileExitMat = HelperManager.s.LoadResourceSafe<Material>("Materials/TileExit");
		tileTrialMat = HelperManager.s.LoadResourceSafe<Material>("Materials/TileTrial");
		tilePuddleMat = HelperManager.s.LoadResourceSafe<Material>("Materials/TilePuddle");
		tileMossyMat = HelperManager.s.LoadResourceSafe<Material>("Materials/TileMossy");
        playerSprite = HelperManager.s.LoadResourceSafe<Sprite>("Textures/player");
        playerTrappedSprite = HelperManager.s.LoadResourceSafe<Sprite>("Textures/player_trapped");
		mineDebugSprite = HelperManager.s.LoadResourceSafe<Sprite>("Textures/mine_debug");
        ppvp = HelperManager.s.LoadResourceSafe<VolumeProfile>("PPVolumeProfile");
    }
    private void Start() {
        lastMousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        canvasRt = canvas.transform as RectTransform;
    }
    private void Update() {
        mouseTimer += Time.deltaTime;
        if (mouseTimer > 0.05f) {
            mouseVelocity = (Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue()) - lastMousePos) / mouseTimer;
            lastMousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            mouseTimer = 0;
        }
    }
}
