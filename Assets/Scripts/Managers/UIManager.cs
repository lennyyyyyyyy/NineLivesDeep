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
    [System.NonSerialized]
    public HashSet<GameObject> tooltips = new HashSet<GameObject>();

    private float mouseTimer = 0;

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
        SetTooltipTargetPositions();
    }
    private void SetTooltipTargetPositions() {
        List<GameObject> activeTooltips = tooltips.Where(t => t.activeSelf).ToList();
        activeTooltips.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y)); 
        float averageY = 0, totalY = 0;
        foreach (GameObject g in activeTooltips) {
            averageY += g.GetComponent<Tooltip>().idealTargetPosition.y;
            totalY += HelperManager.s.WorldSizeFromRT(g.transform as RectTransform).y;
        }
        averageY /= activeTooltips.Count;
        totalY += ConstantsManager.s.tooltipPadding * (activeTooltips.Count - 1) * 4;
        float currentY = averageY - totalY / 2;
        foreach (GameObject g in activeTooltips) {
            Vector2 size = HelperManager.s.WorldSizeFromRT(g.transform as RectTransform);
            float targetY = currentY + size.y / 2;
            g.GetComponent<Tooltip>().targetPosition = g.GetComponent<Tooltip>().idealTargetPosition;
            g.GetComponent<Tooltip>().targetPosition.y = targetY;
            currentY += size.y + ConstantsManager.s.tooltipPadding * 4;
        }
    }
}
