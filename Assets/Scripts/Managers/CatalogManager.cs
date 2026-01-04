using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
public class TypeData {
    public Type type;
	public int id;
	protected static int nextID = 0;
    public TypeData(Type type) {
        this.type = type;
        this.id = nextID;
        nextID++;
        CatalogManager.s.idToType.Add(this.type);
		CatalogManager.s.typeToData[this.type] = this;
    }
}
public class UIItemData : TypeData {
	public Texture2D tex2d;
    public Sprite sprite;
	public TooltipData tooltipData;

	public UIItemData(Type type, string category, string resourceName, TooltipData tooltipData) : base(type) {
		this.tex2d = HelperManager.s.LoadResourceSafe<Texture2D>("Textures/" + category + "_" + resourceName);
		this.sprite = HelperManager.s.LoadResourceSafe<Sprite>("Textures/" + category + "_" + resourceName);
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
	}
}
public class FlagData : UIItemData {
    public Type placeableSpriteType;
	public bool placeableRemovesMines = true;
	public int consumableDefaultCount = 0; 
	public bool showCount = false;
	public List<string> allowedFloorTypes = new List<string>() {"minefield"};
	public FlagData(Type type,
				      string resourceName, 
					  TooltipData tooltipData, 
					  Type placeableSpriteType = null,
					  bool? placeableRemovesMines = null,
					  int? consumableDefaultCount = null,
					  bool? showCount = null,
					  List<string> allowedFloorTypes = null) : base(type, "flag", resourceName, tooltipData) {
		this.placeableSpriteType = placeableSpriteType ?? this.placeableSpriteType;
		this.placeableRemovesMines = placeableRemovesMines ?? this.placeableRemovesMines;
		this.consumableDefaultCount = consumableDefaultCount ?? this.consumableDefaultCount;
		this.showCount = showCount ?? (placeableSpriteType != null || consumableDefaultCount != null);
		this.allowedFloorTypes = allowedFloorTypes ?? this.allowedFloorTypes;	

		if (this.placeableSpriteType != null) {
			CatalogManager.s.spriteTypeToUIType[this.placeableSpriteType] = this.type;
            TypeData td = new TypeData(this.placeableSpriteType);
		}
		CatalogManager.s.allFlagTypes.Add(this.type);
        if (typeof(Consumable).IsAssignableFrom(this.type)) {
            CatalogManager.s.allConsumableFlagTypes.Add(this.type);
        }
	}
}
public class CurseData : UIItemData {
	public CurseData(Type type, string resourceName, TooltipData tooltipData) : base(type, "curse", resourceName, tooltipData) {
		CatalogManager.s.allCurseTypes.Add(this.type);
	}
}
public class MineData : UIItemData {
	public Type spriteType;
	public MineData(Type type, string resourceName, TooltipData tooltipData, Type spriteType) : base(type, "mine", resourceName, tooltipData) {
		this.spriteType = spriteType;
		CatalogManager.s.spriteTypeToUIType[this.spriteType] = this.type;
		CatalogManager.s.allMineTypes.Add(this.type);
        TypeData td = new TypeData(this.spriteType);
	}
}
/* 
 * The catalog manager maps all types to various static data
 * Used for save files, tooltips, sprites, textures, etc.
 * Created once at application start and never modified again
 */
public class CatalogManager : MonoBehaviour {
    public static CatalogManager s;

	public List<TypeData> allTypeData = new List<TypeData>();
	public Dictionary<Type, TypeData> typeToData = new Dictionary<Type, TypeData>();
	public Dictionary<Type, Type> spriteTypeToUIType = new Dictionary<Type, Type>();
    public List<Type> idToType = new List<Type>();
	public List<Type> allFlagTypes = new List<Type>();
    public List<Type> allConsumableFlagTypes = new List<Type>();
	public List<Type> allCurseTypes = new List<Type>();
	public List<Type> allMineTypes = new List<Type>();
    private void Awake() {
        s = this;
		allTypeData = new List<TypeData>() {
            //entities
            new TypeData(typeof(Crank)),
            new TypeData(typeof(Pillar)),
            new TypeData(typeof(Vase)),
            new TypeData(typeof(Tunnel)),
            //tiles
            new TypeData(typeof(NormalTile)),
            new TypeData(typeof(ActionTile)),
            new TypeData(typeof(MossyTile)),
            new TypeData(typeof(Puddle)),
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
			new FlagData(typeof(Shovel), "shovel", new TooltipData("Shovel Flag", "An upgrade for your tiny claws.", "ONE TIME USE - Immediately skip to the start of the next floor."),
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
			new MineData(typeof(Mine), "base", new TooltipData("Mine", "Boom.", "Standard mine. Explodes when stepped on."), typeof(MineSprite)),
			new MineData(typeof(Hydra), "hydra", new TooltipData("Hydra Mine", "The mine, the myth, the legend.", "When stepped on, explodes and spawns two standard mines in adjacent tiles."), typeof(HydraSprite)),
			new MineData(typeof(Mouse), "mouse", new TooltipData("Mouse Mine", "Slippery vermin.", "Moves to an adjacent tile periodically."), typeof(MouseSprite)),
			new MineData(typeof(Chief), "chief", new TooltipData("Chief Mine", "It calls to its cronies.", "When triggered, moves mines closer to you."), typeof(ChiefSprite)),
			new MineData(typeof(Telemine), "telemine", new TooltipData("Telemine", "Explosion so powerful it makes a wormhole.", "Teleports you to a nearby undiscovered tile."), typeof(TelemineSprite))
		};

		PlayerUIItemModule.s.InitializeUnseenFlags();
    }
}
