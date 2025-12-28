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
    public GameObject nine, lives, deep, startbutton;
    public float startIdleStrength, startIdleSpeed;
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

        alphaEdgeBlueMat = LoadResourceSafe<Material>("Materials/AlphaEdgeBlue");
		tileNormalMat = LoadResourceSafe<Material>("Materials/TileNormal");
		tileExitMat = LoadResourceSafe<Material>("Materials/TileExit");
		tileTrialMat = LoadResourceSafe<Material>("Materials/TileTrial");
		tilePuddleMat = LoadResourceSafe<Material>("Materials/TilePuddle");
		tileMossyMat = LoadResourceSafe<Material>("Materials/TileMossy");
		mineDebugSprite = LoadResourceSafe<Sprite>("Textures/mine_debug");
        ppvp = LoadResourceSafe<VolumeProfile>("PPVoluemeProfile");
    }
    private void Start() {
        lastMousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        canvasRt = canvas.transform as RectTransform;
    }
    private void Update() {
        if (GameManager.s.gamestate == GameManager.s.START) {
            nine.transform.localPosition = new Vector3(nine.transform.localPosition.x, startIdleStrength * Mathf.Sin(startIdleSpeed * Time.time), 0);
            lives.transform.localPosition = new Vector3(lives.transform.localPosition.x, startIdleStrength * Mathf.Sin(startIdleSpeed * 0.9f * Time.time + 2), 0);
            deep.transform.localPosition = new Vector3(deep.transform.localPosition.x, startIdleStrength * Mathf.Sin(startIdleSpeed * 1.1f * Time.time + 4), 0);
            startbutton.transform.localPosition = new Vector3(startbutton.transform.localPosition.x, 90.2f + 0.5f * startIdleStrength * Mathf.Sin(startIdleSpeed * 0.8f * Time.time + 3), 0);
        }

        mouseTimer += Time.deltaTime;
        if (mouseTimer > 0.05f) {
            mouseVelocity = (Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue()) - lastMousePos) / mouseTimer;
            lastMousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            mouseTimer = 0;
        }
    }
    public void updateAlpha(SpriteRenderer sr, float a) {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
    }
    public void updateColor(SpriteRenderer sr, Color c) {
        sr.color = c;
    }
    public void updateAnchoredPosition(RectTransform rt, Vector3 v) {
        rt.anchoredPosition = new Vector2(v.x, v.y);
    }
    public void updateSizeDelta(RectTransform rt, Vector3 v) {
        rt.sizeDelta = new Vector2(v.x, v.y);
    }
    public void AddPaw() {
        GameObject g = Instantiate(GameManager.s.paw_p, pawGroup.transform);
        paws.Add(g);
        (g.transform as RectTransform).anchoredPosition = new Vector2(150, 0);
    }
    public void OrganizeFlags() {
        bool variableSpacing = Player.s.flags.Count * 100 > (UIManager.s.canvas.transform as RectTransform).rect.height;
        for (int i = 0; i < Player.s.flags.Count; i++) {
            GameObject f = Player.s.flags[i], p = paws[i];
            RectTransform prt = p.transform as RectTransform;
            Flag flag = f.GetComponent<Flag>();
            flag.UpdateUsable();

            LeanTween.cancel(f);
            LeanTween.cancel(p);

            float destinationY;
            if (variableSpacing) {
                destinationY = -(UIManager.s.canvas.transform as RectTransform).rect.height * (i+1) / (Player.s.flags.Count + 1);
            } else {
                destinationY = -60 - i * 100;
            }

            if (flag.usable && !(Vector2.Distance(flag.rt.anchoredPosition, new Vector2(-60, destinationY)) < 0.1f && Vector2.Distance(prt.anchoredPosition, new Vector2(0, destinationY)) < 0.1f)) {
                LeanTween.value(f, (Vector3 v) => updateAnchoredPosition(flag.rt, v), flag.rt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                LeanTween.value(p, (Vector3 v) => updateAnchoredPosition(prt, v), prt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(()=>{
                    LeanTween.value(p, (Vector3 v) => updateAnchoredPosition(prt, v), prt.anchoredPosition, new Vector2(0, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                });
            } else if (!flag.usable && !(Vector2.Distance(flag.rt.anchoredPosition, new Vector2(-30, destinationY)) < 0.1f && Vector2.Distance(prt.anchoredPosition, new Vector2(-30, destinationY)) < 0.1f)) {
                LeanTween.value(f, (Vector3 v) => updateAnchoredPosition(flag.rt, v), flag.rt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(()=>{
                    LeanTween.value(f, (Vector3 v) => updateAnchoredPosition(flag.rt, v), flag.rt.anchoredPosition, new Vector2(-30, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
                });
                LeanTween.value(p, (Vector3 v) => updateAnchoredPosition(prt, v), prt.anchoredPosition, new Vector2(-60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic).setOnComplete(()=>{
                    LeanTween.value(p, (Vector3 v) => updateAnchoredPosition(prt, v), prt.anchoredPosition, new Vector2(-30, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
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
		foreach (GameObject notFlag in Player.s.notFlags) {
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
		Player.s.notFlags.Clear();
		for (int i = 0; i < sortedNotFlags.Length; i++) {
			foreach (GameObject g in sortedNotFlags[i]) {
				Player.s.notFlags.Add(g);
			}
		}
		//then move items in their places
        bool variableSpacing = Player.s.notFlags.Count * 100 > (UIManager.s.canvas.transform as RectTransform).rect.height;
        for (int i = 0; i < Player.s.notFlags.Count; i++) {
            GameObject g = Player.s.notFlags[i];
            LeanTween.cancel(g);
			UIItem item = g.GetComponent<UIItem>();

            float destinationY;
            if (variableSpacing) {
                destinationY = -(UIManager.s.canvas.transform as RectTransform).rect.height * (i+1) / (Player.s.notFlags.Count + 1);
            } else {
                destinationY = -60 - i * 100;
            }
            if (!(Vector2.Distance(item.rt.anchoredPosition, new Vector2(60, destinationY)) < 0.1f)) {
                LeanTween.value(g, (Vector3 v) => updateAnchoredPosition(item.rt, v), item.rt.anchoredPosition, new Vector2(60, destinationY), 0.5f).setEase(LeanTweenType.easeInOutCubic);
            } 
		}
		Player.s.RecalculateModifiers();
	}
    public void InstantiateBubble(Vector3 pos, string t, Color c, float time = 1f, float scale = 1f) {
        GameObject b = Instantiate(GameManager.s.bubble_p, pos, Quaternion.identity, UIManager.s.bubbleGroup.transform);
        b.GetComponent<Bubble>().Init(c, t, time, scale);
    }
    public void InstantiateBubble(GameObject g, string t, Color c, float radius = 0.5f, float time = 1f, float scale = 1f) {
        InstantiateBubble(g.transform.position + new Vector3(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius), 0), t, c, time);
    }
    public Vector2 WorldSizeFromRT(RectTransform rt) {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Vector2(corners[2].x - corners[0].x, corners[1].y - corners[0].y);
    }
    public void floatingHover(Transform t, float hoveredScale, float offset, Vector3 defaultRotation, float stretch=0.1f, float angle=10f, float period=0.65f, float power=0.65f) {
        Vector3 newScale = Vector3.one;

        newScale[0] = Mathf.Lerp(t.localScale[0], hoveredScale * Mathf.Pow(1+stretch, Mathf.Sin((period*Time.time)*(2*Mathf.PI))), 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f));
        newScale[1] = Mathf.Lerp(t.localScale[1], Mathf.Pow(hoveredScale, 2) / newScale[0], 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f));

        t.localScale = newScale;
        t.localEulerAngles = new Vector3(t.localEulerAngles.x, 
                                                t.localEulerAngles.y, 
                                                Mathf.LerpAngle(t.localEulerAngles.z, defaultRotation.z + angle * Mathf.Sin((period*Time.time + offset)*(2*Mathf.PI)), 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f)));
    }
	public T LoadResourceSafe<T>(string path) where T : UnityEngine.Object {
		T resource = Resources.Load<T>(path);
		if (typeof(T) == typeof(Texture2D)) {
			return resource ?? Resources.Load<T>("Textures/nulltex");
		}
		return resource;
	}
	public void SetupUIEventTriggers(GameObject g, EventTriggerType[] eventIDs, Action<PointerEventData>[] actions) { 
		EventTrigger trigger;
		EventTrigger.Entry entry;
		trigger = g.GetComponent<EventTrigger>() == null ? g.AddComponent<EventTrigger>() : g.GetComponent<EventTrigger>();
		for (int i = 0; i < eventIDs.Length; i++) {
			entry = new EventTrigger.Entry{eventID = eventIDs[i]};
			int iCopy = i; //capture variable for closure
			entry.callback.AddListener((data) => { actions[iCopy]((PointerEventData)data); });
			trigger.triggers.Add(entry);
		}
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
