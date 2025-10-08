using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
public class UIVars {
	public Texture2D tex2d;
    public Sprite sprite;
	public string name, flavor, info;
	public Color color;
	public int id;
	public static int nextID = 0;
	public List<GameObject> instances = new List<GameObject>();
	public UIVars(string category, string resourceName, string n, string f, string i) {
		tex2d = Resources.Load<Texture2D>("Textures/" + category + "_" + resourceName);
		sprite = Resources.Load<Sprite>("Textures/" + category + "_" + resourceName);
        Color[] colors = tex2d.GetPixels();
        float r = 0, g = 0, b = 0;
        foreach (Color c in colors) {
            if (c.a > 0) {
                r += c.r;
                g += c.g;
                b += c.b;
            }
        }
        color = new Color(r / Mathf.Max(r, g, b), g / Mathf.Max(r, g, b), b / Mathf.Max(r, g, b));

        name = n;
        flavor = f;
        info = i;

        id = nextID;
        nextID++;
	}
}
public class FlagUIVars : UIVars {
    public Type spriteType;
	public int consumableDefaultCount = 0; 
	public bool showCount = false;
    public FlagUIVars(string resourceName, string n, string f, string i, bool showC, Type type = null) : base("flag", resourceName, n, f, i) {
		showCount = showC;
        spriteType = type;
    }
    public FlagUIVars(string resourceName, string n, string f, string i, bool showC, int defaultCount) : base("flag", resourceName, n, f, i) {
		showCount = showC;
		consumableDefaultCount = defaultCount;
    }
}
public class CurseUIVars : UIVars {
	public CurseUIVars(string resourceName, string n, string f, string i) : base("curse", resourceName, n, f, i) {
	}
}
public class UIManager : MonoBehaviour
{
    public static UIManager s;
    public Sprite player, player_trapped;
    public GameObject canvas, STARTUI, GAMEUI, flagGroup, pawGroup, tooltipGroup, bubbleGroup;
    [System.NonSerialized]
    public RectTransform canvasRt;
    public GameObject nine, lives, deep, startbutton, minecount;
    public float startIdleStrength, startIdleSpeed;
    [System.NonSerialized]
    public Vector3 lastMousePos, mouseVelocity = Vector2.zero;
    private float mouseTimer = 0;
    public Dictionary<Type, FlagUIVars> flagUIVars;
    public Dictionary<Type, Type> spriteToFlag = new Dictionary<Type, Type>();
	public Dictionary<Type, CurseUIVars> curseUIVars;
    [System.NonSerialized]
    public List<GameObject> paws = new List<GameObject>();
    [System.NonSerialized]
    public Material alphaEdgeBlueMat, tileNormalMat, tileExitMat, tileTrialMat, tilePuddleMat, tileMossyMat;
    public Volume ppv;
    [System.NonSerialized]
    public VolumeProfile ppvp;

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
    private void Awake() {
        s = this;
        flagUIVars = new Dictionary<Type, FlagUIVars>{
            //placeable flags
            {typeof(Base), new FlagUIVars("base", "Flag", "Nostalgic, isn't it?", "Drag and drop these wherever you think mines are.", true, typeof(BaseSprite))},
            {typeof(Anti), new FlagUIVars("anti", "Anti-Flag", "Anti-matter without the explosion.", "Remove and reuse your placed flags by dropping this flag on them.", true, typeof(AntiSprite))},
            {typeof(Psychic), new FlagUIVars("psychic", "Psychic Flag", "I can see what you can't, stupid cat.", "Points its eye to the nearest mine.", true, typeof(PsychicSprite))},
            {typeof(Rahhh), new FlagUIVars("rahhh", "RAHHH", "Coloni—spread freedom from afar!", "Discovers a 3x3 area, applying to all map flags.", true, typeof(RahhhSprite))},
            {typeof(Rubber), new FlagUIVars("rubber", "Rubber Flag", "Instead of blowing in the wind, it just kind of vibrates.", "Place these and bounce on them to travel further.", true, typeof(RubberSprite))},
            {typeof(You), new FlagUIVars("you", "You Flag", "For use in the worst case scenario", "Revive yourself whenever you die", true, typeof(YouSprite))},
            {typeof(Raincloud), new FlagUIVars("raincloud", "Raincloud Flag", "The tiles are actually pretty porous.", "Guarantees a puddle at the same coordinates on all following floors.", true, typeof(RaincloudSprite))},
            //passive flags
            {typeof(Aromatic), new FlagUIVars("aromatic", "Aromatic Flag", "Your sinuses feel clearer already!", "You can now sniff out mines an extra 1 tile away. Detect mice.", false)},
            {typeof(Catnip), new FlagUIVars("catnip", "Catnip Flag", "", "You can now jump to tiles two units away.", false)},
            {typeof(Gambling), new FlagUIVars("gambling", "Gambling Addiction", "It's only a crippling addiction if you lose.", "Remove all health. Mines have an 80% chance to do nothing and a 20% chance to kill you instantly.", false)},
            {typeof(OneUp), new FlagUIVars("1up", "One-Up Flag", "Nintendo please don't sue me.", "One random square on the floor gives you an additional heart.", false)},
            {typeof(Daytrader), new FlagUIVars("daytrader", "Daytrader Flag", "", "Every floor, gain either 50% more mines or 25% less.", false)},
            {typeof(Bank), new FlagUIVars("bank", "Bank Flag", "", "Every floor, gain 10% more mines.", false)},
            {typeof(Collector), new FlagUIVars("collector", "Collector Flag", "Unlike Pokemon cards, high-tech mines are genuinely expensive to produce.", "The more advanced the mine, the more it's worth.", false)},
            {typeof(Reflection), new FlagUIVars("reflection", "Reflection Flag", "Use one get one free!", "Placing a non-base flag in a puddle gives you a base flag back.", false)},
            {typeof(Wildcat), new FlagUIVars("wildcat", "Wildcat Flag", "Born to be wild.", "Every five new grassy tiles you step in gives you an extra life.", true)},
            //map flags
            {typeof(Brain), new FlagUIVars("brain", "Brain Flag", "Turns out your puny little brain is also a flag.", "Senses the number of mines around you.", false)},
            {typeof(Vertical), new FlagUIVars("vertical", "Vertical Flag", "", "Gives the number of mines in your column.", false)},
            {typeof(Horizontal), new FlagUIVars("horizontal", "Horizontal Flag", "", "Gives the number of mines in your row.", false)},
            {typeof(Manhattan), new FlagUIVars("manhattan", "Manhattan Flag", "The nearest mine's just around the block.", "Gives the manhattan distance to the nearest mine.", false)},
            {typeof(Knight), new FlagUIVars("knight", "Knight Flag", "Yee haw", "Gives the number of mines a knight's move away.", false)},
            //consumable flags
            {typeof(Chocolate), new FlagUIVars("chocolate", "Chocolate Flag", "Curiosity made the cat stronger.", "ONE TIME USE - Sacrifice half your health rounded up for two random flags.", true, 1)},
            {typeof(Dog), new FlagUIVars("dog", "Dog Flag", "Oh, how the tables have turned.", "For your next movement, you are immune to mines, but take damage from safe tiles.", true, 10)},
            {typeof(Shovel), new FlagUIVars("shovel", "Shovel Flag", "An upgrade for your tiny claws.", "ONE TIME USE - Immeditaely skip to the start of the next floor.", true, 1)},
        };
		Player.s.flagsUnseen = new List<Type>(flagUIVars.Keys);
		Player.s.consumableFlagsUnseen = Player.s.flagsUnseen.Where(flag => typeof(Consumable).IsAssignableFrom(flag)).ToList();
        foreach (KeyValuePair<Type, FlagUIVars> entry in flagUIVars) {
            if (entry.Value.spriteType != null) {
                spriteToFlag[entry.Value.spriteType] = entry.Key;
            }
        }

		curseUIVars = new Dictionary<Type, CurseUIVars>{
			{typeof(Watched), new CurseUIVars("watched", "Watched", "They lie in wait...", "Staying still for too long may cause a nearby mine to jump on you and explode")},
			{typeof(Windy), new CurseUIVars("windy", "Windy", "The ventilation is surprisingly good here.", "Flags may blow and land somewhere else when placed.")},
			{typeof(Taxed), new CurseUIVars("taxed", "Taxed", "Can't hide from the IRS.", "More mines, less value")},
			{typeof(Expansion), new CurseUIVars("expansion", "Expansion", "The unknowable will of the tiles.", "All floors become larger.")},
			{typeof(Decrepit), new CurseUIVars("decrepit", "Decrepit", "This place is falling apart. Probably the explosions.", "More holes.")},
			{typeof(Fragile), new CurseUIVars("fragile", "Fragile", "You've got sensitive skin.", "Walking into grass or water five times within a single life kills you. Resets on new floors.")},
			{typeof(Taken), new CurseUIVars("taken", "Taken", "It must be those thieving rats.", "One random flag is disabled on every minefield.")}, 
			{typeof(Intensify), new CurseUIVars("intensify", "Intensify", "Its a curse curse.", "One random curse is heightened every floor.")},
			{typeof(Amnesia), new CurseUIVars("amnesia", "Amnesia", "Hope you remember the way out.", "Tooltips disappear from held flags. Map numbers have a slight chance to disappear.")},
			{typeof(Wobbly), new CurseUIVars("wobbly", "Wobbly", "They fried your cerebellum lil bro", "You can no longer perform the same direction movement twice in a row.")},
			{typeof(Cataracts), new CurseUIVars("cataracts", "Cataracts", "50% blind, 100% annoyed.", "All dropped flags will be confused for other flags; equipped flags are fine.")},
			{typeof(Shaky), new CurseUIVars("shaky", "Shaky", "Something's on your nerves.", "The camera shakes and moves unpredictably.")},
		};
		Player.s.cursesUnseen = new List<Type>(curseUIVars.Keys);

        alphaEdgeBlueMat = Resources.Load<Material>("Materials/AlphaEdgeBlue");
		tileNormalMat = Resources.Load<Material>("Materials/TileNormal");
		tileExitMat = Resources.Load<Material>("Materials/TileExit");
		tileTrialMat = Resources.Load<Material>("Materials/TileTrial");
		tilePuddleMat = Resources.Load<Material>("Materials/TilePuddle");
		tileMossyMat = Resources.Load<Material>("Materials/TileMossy");
        ppvp = Resources.Load<VolumeProfile>("PPVoluemeProfile");
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
