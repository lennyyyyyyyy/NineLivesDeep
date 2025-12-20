using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
public class UIItemData {
	public Type uiType;
	public Texture2D tex2d;
    public Sprite sprite;
	public TooltipData tooltipData;
	public int id;
	public List<GameObject> instances = new List<GameObject>();

	protected static int nextID = 0;

	public UIItemData(Type uiType, string category, string resourceName, TooltipData tooltipData) {
		this.uiType = uiType;
		this.tex2d = UIManager.s.LoadResourceSafe<Texture2D>("Textures/" + category + "_" + resourceName);
		this.sprite = UIManager.s.LoadResourceSafe<Sprite>("Textures/" + category + "_" + resourceName);
        Color[] colors = tex2d.GetPixels();
        float r = 0, g = 0, b = 0;
        foreach (Color c in colors) {
            if (c.a > 0) {
                r += c.r;
                g += c.g;
                b += c.b;
            }
        }
		this.tooltipData = tooltipData;
        this.tooltipData.color = new Color(r / Mathf.Max(r, g, b), g / Mathf.Max(r, g, b), b / Mathf.Max(r, g, b));
        this.id = nextID;
        nextID++;
	
		UIManager.s.allUIItemData.Add(this);
		UIManager.s.uiTypeToData[this.uiType] = this;
	}
}
public class FlagData : UIItemData {
    public Type placeableSpriteType;
	public bool placeableRemovesMines = true;
	public int consumableDefaultCount = 0; 
	public bool showCount = false;
	public List<string> allowedFloorTypes = new List<string>() {"minefield"};
	public FlagData(Type uiType,
				      string resourceName, 
					  TooltipData tooltipData, 
					  Type placeableSpriteType = null,
					  bool? placeableRemovesMines = null,
					  int? consumableDefaultCount = null,
					  bool? showCount = null,
					  List<string> allowedFloorTypes = null) : base(uiType, "flag", resourceName, tooltipData) {
		this.placeableSpriteType = placeableSpriteType ?? this.placeableSpriteType;
		this.placeableRemovesMines = placeableRemovesMines ?? this.placeableRemovesMines;
		this.consumableDefaultCount = consumableDefaultCount ?? this.consumableDefaultCount;
		this.showCount = showCount ?? (placeableSpriteType != null || consumableDefaultCount != null);
		this.allowedFloorTypes = allowedFloorTypes ?? this.allowedFloorTypes;	

		if (this.placeableSpriteType != null) {
			UIManager.s.spriteTypeToUIType[this.placeableSpriteType] = this.uiType;
		}
		UIManager.s.allFlagTypes.Add(this.uiType);
        if (typeof(Consumable).IsAssignableFrom(this.uiType)) {
            UIManager.s.allConsumableFlagTypes.Add(this.uiType);
        }
	}
}
public class CurseData : UIItemData {
	public CurseData(Type uiType, string resourceName, TooltipData tooltipData) : base(uiType, "curse", resourceName, tooltipData) {
		UIManager.s.allCurseTypes.Add(this.uiType);
	}
}
public class MineData : UIItemData {
	public Type spriteType;
	public MineData(Type uiType, string resourceName, TooltipData tooltipData, Type spriteType) : base(uiType, "mine", resourceName, tooltipData) {
		this.spriteType = spriteType;
		UIManager.s.spriteTypeToUIType[this.spriteType] = this.uiType;
		UIManager.s.allMineTypes.Add(this.uiType);
	}
}
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

	public List<UIItemData> allUIItemData = new List<UIItemData>();
	public Dictionary<Type, UIItemData> uiTypeToData = new Dictionary<Type, UIItemData>();
	public Dictionary<Type, Type> spriteTypeToUIType = new Dictionary<Type, Type>();
	public List<Type> allFlagTypes = new List<Type>();
    public List<Type> allConsumableFlagTypes = new List<Type>();
	public List<Type> allCurseTypes = new List<Type>();
	public List<Type> allMineTypes = new List<Type>();

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
		allUIItemData = new List<UIItemData>() {
			//placeable flags
			new FlagData(typeof(Base), "base", new TooltipData("Flag", "Nostalgic, isn't it?", "Drag and drop these wherever you think mines are."),
					     placeableSpriteType: typeof(BaseSprite),
						 allowedFloorTypes: new List<string>{"minefield", "trial"}),
			new FlagData(typeof(Anti), "anti", new TooltipData("Anti-Flag", "Anti-matter without the explosion.", "Remove and reuse your placed flags by dropping this flag on them."),
					     placeableSpriteType: typeof(AntiSprite)),
			new FlagData(typeof(Psychic), "psychic", new TooltipData("Psychic Flag", "I can see what you can't, stupid cat.", "Points its eye to the nearest mine."),
					     placeableSpriteType: typeof(PsychicSprite)),	
			new FlagData(typeof(Rahhh), "rahhh", new TooltipData("RAHHH", "Coloni—spread freedom from afar!", "Discovers a 3x3 area, applying to all map flags."),
					     placeableSpriteType: typeof(RahhhSprite)),
			new FlagData(typeof(Rubber), "rubber", new TooltipData("Rubber Flag", "Instead of blowing in the wind, it just kind of vibrates.", "Place these and bounce on them to travel further."),
					     placeableSpriteType: typeof(RubberSprite)),
			new FlagData(typeof(You), "you", new TooltipData("You Flag", "For use in the worst case scenario", "Revive yourself whenever you die"),
					     placeableSpriteType: typeof(YouSprite),
						 placeableRemovesMines: false,
						 allowedFloorTypes: new List<string>{"minefield", "trial"}),
			new FlagData(typeof(Raincloud), "raincloud", new TooltipData("Raincloud Flag", "The tiles are actually pretty porous.", "Guarantees a puddle at the same coordinates on all following floors."),
					     placeableSpriteType: typeof(RaincloudSprite)),
			//passive flags
			new FlagData(typeof(Aromatic), "aromatic", new TooltipData("Aromatic Flag", "Your sinuses feel clearer already!", "You can now sniff out mines an extra 1 tile away. Detect mice.")),
			new FlagData(typeof(Catnip), "catnip", new TooltipData("Catnip Flag", "", "You can now jump to tiles two units away.")),
			new FlagData(typeof(Gambling), "gambling", new TooltipData("Gambling Addiction", "It's only a crippling addiction if you lose.", "Remove all health. Mines have an 80% chance to do nothing and a 20% chance to kill you instantly.")),
			new FlagData(typeof(OneUp), "1up", new TooltipData("One-Up Flag", "Nintendo please don't sue me.", "One random square on the floor gives you an additional heart.")),
			new FlagData(typeof(Daytrader), "daytrader", new TooltipData("Daytrader Flag", "", "Every floor, gain either 50% more mines or 25% less.")),
			new FlagData(typeof(Bank), "bank", new TooltipData("Bank Flag", "", "Every floor, gain 10% more mines.")),
            new FlagData(typeof(Collector),"collector", new TooltipData("Collector Flag", "Unlike Pokemon cards, high-tech mines are genuinely expensive to produce.", "The more advanced the mine, the more it's worth.")),
            new FlagData(typeof(Reflection), "reflection", new TooltipData("Reflection Flag", "Use one get one free!", "Placing a non-base flag in a puddle gives you a base flag back.")),
            new FlagData(typeof(Wildcat), "wildcat", new TooltipData("Wildcat Flag", "Born to be wild.", "Every five new grassy tiles you step in gives you an extra life."),
						 showCount: true),
			new FlagData(typeof(Curious), "curious", new TooltipData("Curious Flag", "Very wide-eyed.", "Increases vision distance.")),
			new FlagData(typeof(Astral), "astral", new TooltipData("Astral Flag", "Your body held you back.", "Respawn in a 3x3 area around where you died.")),
			new FlagData(typeof(Milk), "milk", new TooltipData("Milk Flag", "Gives you bigger bones.", "Increases interaction radius")),
			//map flags
			new FlagData(typeof(Brain), "brain", new TooltipData("Brain Flag", "Turns out your puny little brain is also a flag.", "Senses the number of mines around you."),
					     allowedFloorTypes: new List<string>{"minefield", "trial"}),
			new FlagData(typeof(Vertical), "vertical", new TooltipData("Vertical Flag", "", "Gives the number of mines in your column.")),
			new FlagData(typeof(Horizontal), "horizontal", new TooltipData("Horizontal Flag", "", "Gives the number of mines in your row.")),
			new FlagData(typeof(Manhattan), "manhattan", new TooltipData("Manhattan Flag", "The nearest mine's just around the block.", "Gives the manhattan distance to the nearest mine.")),
			new FlagData(typeof(Knight), "knight", new TooltipData("Knight Flag", "Yee haw", "Gives the number of mines a knight's move away.")),
			//consumable flags
			new FlagData(typeof(Chocolate), "chocolate", new TooltipData("Chocolate Flag", "Curiosity made the cat stronger.", "ONE TIME USE - Sacrifice half your health rounded up for two random flags."),
						 consumableDefaultCount: 1),
			new FlagData(typeof(Dog), "dog", new TooltipData("Dog Flag", "Oh, how the tables have turned.", "For your next movement, you are immune to mines, but take damage from safe tiles."),
						 consumableDefaultCount: 10),
			new FlagData(typeof(Shovel), "shovel", new TooltipData("Shovel Flag", "An upgrade for your tiny claws.", "ONE TIME USE - Immeditaely skip to the start of the next floor."),
						 consumableDefaultCount: 1),
			//curses
			new CurseData(typeof(Watched), "watched", new TooltipData("Watched", "They lie in wait...", "Mines jump on you, if you're too still.")),
			new CurseData(typeof(Windy), "windy", new TooltipData("Windy", "The ventilation is surprisingly good here.", "Flags may blow and land somewhere else when let go.")),
			new CurseData(typeof(Taxed), "taxed", new TooltipData("Taxed", "Can't hide from the IRS.", "More mines, less value")),
			new CurseData(typeof(Expansion), "expansion", new TooltipData("Expansion", "The unknowable will of the tiles.", "All floors become larger.")),
			new CurseData(typeof(Decrepit), "decrepit", new TooltipData("Decrepit", "This place is falling apart. Probably the explosions.", "More holes.")),
			new CurseData(typeof(Fragile), "fragile", new TooltipData("Fragile", "You've got sensitive skin.", "Walking into grass or water enough kills you, eventually. Resets on new floors.")),
			new CurseData(typeof(Taken), "taken", new TooltipData("Taken", "Currently disabling nothing.", "Takes flags away on every minefield.")),
			new CurseData(typeof(Intensify), "intensify", new TooltipData("Intensify", "Currently intensifying nothing.", "One random curse is heightened every floor.")),
			new CurseData(typeof(Amnesia), "amnesia", new TooltipData("Amnesia", "Why are you here again?", "Tooltips disappear from items. Map numbers have a chance to disappear.")),
			new CurseData(typeof(Wobbly), "wobbly", new TooltipData("Wobbly", "They fried your cerebellum lil bro", "You can't walk straight.")),
			new CurseData(typeof(Cataracts), "cataracts", new TooltipData("Cataracts", "Staring longer isn't going to help.", "Flags are sometimes confused for other flags, when on the ground.")),
			new CurseData(typeof(Shaky), "shaky", new TooltipData("Shaky", "Something's on your nerves.", "The camera shakes and moves unpredictably.")),
			//mines
			new MineData(typeof(Mine), "mine", new TooltipData("Mine", "Boom.", "Standard mine. Explodes when stepped on."), typeof(MineSprite)),
			new MineData(typeof(Hydra), "hydra", new TooltipData("Hydra Mine", "The mine, the myth, the legend.", "When stepped on, explodes and spawns two standard mines in adjacent tiles."), typeof(HydraSprite)),
			new MineData(typeof(Mouse), "mouse", new TooltipData("Mouse Mine", "Slippery vermin.", "Moves to an adjacent tile periodically."), typeof(MouseSprite)),
			new MineData(typeof(Chief), "chief", new TooltipData("Chief Mine", "It calls to its cronies.", "When triggered, moves mines closer to you."), typeof(ChiefSprite)),
			new MineData(typeof(Telemine), "telemine", new TooltipData("Telemine", "Explosion so powerful it makes a wormhole.", "Teleports you to a nearby undiscovered tile."), typeof(TelemineSprite))
		};

		Player.s.InitializeUnseenFlags();

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
