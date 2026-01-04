using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
public class UIManager : MonoBehaviour
{
    public static UIManager s;
    public Sprite player, player_trapped;
    public GameObject canvas, STARTUI, GAMEUI, flagGroup, pawGroup, tooltipGroup, bubbleGroup;
    [System.NonSerialized]
    public RectTransform canvasRt;
    [System.NonSerialized]
    public Vector3 lastMousePos, mouseVelocity = Vector2.zero;
    private float mouseTimer = 0;

    [System.NonSerialized]
    public List<GameObject> paws = new List<GameObject>();
    [System.NonSerialized]
    public Material alphaEdgeBlueMat, tileNormalMat, tileExitMat, tileTrialMat, tilePuddleMat, tileMossyMat;
	[System.NonSerialized]
	public Sprite mineDebugSprite;
    public Volume ppv;
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
    private void MatchPawsToFlags() {
        while (paws.Count < PlayerUIItemModule.s.flags.Count) {
            GameObject g = Instantiate(PrefabManager.s.pawPrefab, pawGroup.transform);
            paws.Add(g);
            (g.transform as RectTransform).anchoredPosition = new Vector2(150, 0);
        }
        while (paws.Count > PlayerUIItemModule.s.flags.Count) {
            GameObject p = paws[paws.Count - 1];
            paws.RemoveAt(paws.Count - 1);
            RectTransform prt = p.transform as RectTransform;
            LeanTween.value(p, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(prt, v), prt.anchoredPosition, new Vector2(-60, prt.anchoredPosition.y), 0.5f)
                .setEase(LeanTweenType.easeInOutCubic)
                .setOnComplete(() => {
                Destroy(p);
            });
        } 
    }
    public void OrganizeFlags() {
        MatchPawsToFlags();
        bool variableSpacing = PlayerUIItemModule.s.flags.Count * 100 > (UIManager.s.canvas.transform as RectTransform).rect.height;
        for (int i = 0; i < PlayerUIItemModule.s.flags.Count; i++) {
            GameObject f = PlayerUIItemModule.s.flags[i], p = paws[i];
            RectTransform prt = p.transform as RectTransform;
            Flag flag = f.GetComponent<Flag>();
            flag.UpdateUsable();

            LeanTween.cancel(f);
            LeanTween.cancel(p);

            float destinationY;
            if (variableSpacing) {
                destinationY = -(UIManager.s.canvas.transform as RectTransform).rect.height * (i+1) / (PlayerUIItemModule.s.flags.Count + 1);
            } else {
                destinationY = -60 - i * 100;
            }

            if (flag.usable && !(Vector2.Distance(flag.rt.anchoredPosition, new Vector2(-60, destinationY)) < 0.1f && Vector2.Distance(prt.anchoredPosition, new Vector2(0, destinationY)) < 0.1f)) {
                LeanTween.value(f, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(flag.rt, v), 
                    flag.rt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                LeanTween.value(p, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(prt, v),
                    prt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(()=>{
                    LeanTween.value(p, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(prt, v), 
                        prt.anchoredPosition, new Vector2(0, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                });
            } else if (!flag.usable && !(Vector2.Distance(flag.rt.anchoredPosition, new Vector2(-30, destinationY)) < 0.1f && Vector2.Distance(prt.anchoredPosition, new Vector2(-30, destinationY)) < 0.1f)) {
                LeanTween.value(f, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(flag.rt, v),
                    flag.rt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(()=>{
                    LeanTween.value(f, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(flag.rt, v),
                        flag.rt.anchoredPosition, new Vector2(-30, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                });
                LeanTween.value(p, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(prt, v),
                    prt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(()=>{
                    LeanTween.value(p, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(prt, v),
                        prt.anchoredPosition, new Vector2(-30, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                });
            }
        }
		Player.s.RecalculateModifiers();
    }
	public void OrganizeNotFlags() {
		//counting sort in the right order
		List<GameObject>[] sortedNotFlags = new List<GameObject>[4];
		for (int i = 0; i < 4; i++) {
			sortedNotFlags[i] = new List<GameObject>();
		}
		foreach (GameObject notFlag in PlayerUIItemModule.s.notFlags) {
			UIItem uiItem = notFlag.GetComponent<UIItem>();
			if (uiItem is MineUIItem) {
				sortedNotFlags[0].Add(notFlag);
			} else if (uiItem is Intensify){
				sortedNotFlags[1].Add(notFlag);
			} else if (uiItem is Curse) {
				sortedNotFlags[2].Add(notFlag);
			} else if (uiItem is Mine) {
				sortedNotFlags[3].Add(notFlag);
			}
		}
		PlayerUIItemModule.s.notFlags.Clear();
		for (int i = 0; i < sortedNotFlags.Length; i++) {
			foreach (GameObject g in sortedNotFlags[i]) {
				PlayerUIItemModule.s.notFlags.Add(g);
			}
		}
		//then move items in their places
        bool variableSpacing = PlayerUIItemModule.s.notFlags.Count * 100 > (UIManager.s.canvas.transform as RectTransform).rect.height;
        for (int i = 0; i < PlayerUIItemModule.s.notFlags.Count; i++) {
            GameObject g = PlayerUIItemModule.s.notFlags[i];
            LeanTween.cancel(g);
			UIItem item = g.GetComponent<UIItem>();

            float destinationY;
            if (variableSpacing) {
                destinationY = -(UIManager.s.canvas.transform as RectTransform).rect.height * (i+1) / (PlayerUIItemModule.s.notFlags.Count + 1);
            } else {
                destinationY = -60 - i * 100;
            }
            if (!(Vector2.Distance(item.rt.anchoredPosition, new Vector2(60, destinationY)) < 0.1f)) {
                LeanTween.value(g, (Vector3 v) => HelperManager.s.UpdateAnchoredPosition(item.rt, v),
                    item.rt.anchoredPosition, new Vector2(60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
            } 
		}
		Player.s.RecalculateModifiers();
	}
    public void STARTToGAME() {
        OrganizeFlags();
        LeanTween.value(gameObject, (float f) => {
            STARTUI.GetComponent<CanvasGroup>().alpha = f;
            GAMEUI.GetComponent<CanvasGroup>().alpha = 1 - f;
        }, 1, 0, 2f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(() => {
            STARTUI.active = false;
        });
    }
    private void OnEnable() {
        GameManager.OnSTARTToGAME += STARTToGAME;
        Player.OnAliveChange += OrganizeFlags;
    }
    private void OnDisable() {
        GameManager.OnSTARTToGAME -= STARTToGAME;
        Player.OnAliveChange -= OrganizeFlags;
    }
}
