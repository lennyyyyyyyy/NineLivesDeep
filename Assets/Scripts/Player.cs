using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using System.Linq;

public class Modifiers {
	public float mineSpawnMult, mineDefuseMult, noTileChance, windStrength, windFluctuation, cameraShakeStrength, cameraShakePeriod;
	public int floorExpansion, tempChangesUntilDeath;
	public bool wobbly, amnesia, watched, taken;
	public void Reset() {
		mineSpawnMult = 1;
		mineDefuseMult = 1;
		floorExpansion = 0;
		noTileChance = 0.1f;
		windStrength = 0;
		windFluctuation = 0.15f;
		cameraShakeStrength = 0;
		cameraShakePeriod = 0.4f;
		wobbly = false;
		amnesia = false;
		watched = false;
		taken = false;
		tempChangesUntilDeath = (int) 2e9;
	}
	public Modifiers() {
		Reset();
	}
}
public class Player : VerticalObject
{
    public static Player s;
    [System.NonSerialized]
    public List<GameObject> prints = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> flags = new List<GameObject>(), notFlags = new List<GameObject>(), mines = new List<GameObject>(), tilesUnvisited = new List<GameObject>();
    public HashSet<Type> flagsSeen = new HashSet<Type>(), cursesSeen = new HashSet<Type>(), minesSeen = new HashSet<Type>(); // not in use but holding on to these
	public List<Type> flagsUnseen, consumableFlagsUnseen, cursesUnseen, minesUnseen;
    public HashSet<GameObject> tilesVisited = new HashSet<GameObject>();
    public List<Vector2Int> printLocs;
    public Vector2Int coord;
    public GameObject light, blood, feet;
    public GameObject[,] playerBits;
    [System.NonSerialized]
    public int discoverRange = 0;
    public bool trapped = false, alive = true;
    [System.NonSerialized]
    public Animator animator;
    [System.NonSerialized]
    public int texWidth, texHeight;
    public static Action OnDie, OnRevive, OnAliveChange;
    [System.NonSerialized]
    public float money = 0;
    private float stepImpulse = 0.2f;
    [System.NonSerialized]
	public float vision = 2f;
	public Modifiers modifiers = new Modifiers();
	[System.NonSerialized]
	public Vector2 lastMovement = Vector2.zero;
	private int tempChanges = 0;
	[System.NonSerialized]
	public GameObject takenDisabledFlag;

    public bool hasFlag(Type type) {
        return UIManager.s.flagUIVars[type].instances.Count > 0;
    }
    public void setTrapped(bool b) {
        // if (b) {
        //     if (trapped) {
        //         //die instantly
        //     } else {
        //         sr.sprite = UIManager.s.player_trapped;
        //     }

        // } else {
        //     sr.sprite = UIManager.s.player;
        // }
        // trapped = b;
    }
	public void RecalculateModifiers() {
		modifiers.Reset();
		foreach (GameObject flag in flags) {
			flag.GetComponent<UIItem>().Modify(ref modifiers);
		}
		foreach (GameObject notFlag in notFlags) {
			notFlag.GetComponent<UIItem>().Modify(ref modifiers);
		}
	}
	public void NoticeFlag(Type type) {
		flagsSeen.Add(type);
		flagsUnseen.Remove(type);
		consumableFlagsUnseen.Remove(type);
	}			
	public void NoticeCurse(Type type) {
		cursesSeen.Add(type);
		cursesUnseen.Remove(type);
	}
	public void NoticeMine(Type type) {
		minesSeen.Add(type);
		minesUnseen.Remove(type);
	}
    public void Die() {
		Floor.s.floorDeathCount++;
        alive = false;
        OnAliveChange?.Invoke();
        UpdateActiveMapLayers();
        UpdateActivePrints();
        OnDie?.Invoke();
        sr.enabled = false;
    }
    public void Revive() {
        OnRevive?.Invoke();
        GameManager.s.DelayAction(() => {
            sr.enabled = true; 
            alive = true;
            OnAliveChange?.Invoke();
            UpdateActiveMapLayers();
            UpdateActivePrints();
			tempChanges = 0;
			triggerMines();
        }, GameManager.s.deathReviveDuration);
    }
    public void UpdateActiveMapLayers() {
        bool active = alive;
        foreach (GameObject g in flags) {
            Flag flag = g.GetComponent<Flag>();
            if (flag is Placeable) {
                active &= (flag as Placeable).sprite == null || (flag as Placeable).sprite.GetComponent<FlagSprite>().state == "dropped";
            }
        }
        foreach (GameObject g in flags) {
            Flag flag = g.GetComponent<Flag>();
            if (flag is Map) {
                Map m = flag as Map;
                active &= m.active;
                foreach (GameObject n in m.numbers) {
					if (n != null) {
                    	n.active = active;
					}
                }
            }
        }
    }
    public void UpdateActivePrints() {
        bool active = alive;
        foreach (GameObject g in flags) {
            Flag flag = g.GetComponent<Flag>();
            if (flag is Placeable) {
                active &= (flag as Placeable).sprite == null || (flag as Placeable).sprite.GetComponent<FlagSprite>().state == "dropped";
            }
        }
        foreach (GameObject g in prints) {
            g.active = active;
        }
    }
    public void UpdateMoney(float newCount) {
        UIManager.s.InstantiateBubble(MineUIItem.s.gameObject, (newCount - money >= 0 ? "+" : "-") + (Mathf.Round(Mathf.Abs(newCount - money)*100f)/100f).ToString(), Color.white);
        money = newCount;
        
        MineUIItem.s.count.text = money.ToString();
    }
    public void destroyPrints() {
        for (int i = 0; i < prints.Count; i++) {
            Destroy(prints[i]);
        }
        prints.Clear();
    }
    public void updatePrints() {
        List<Vector2Int> filteredPrintLocs = new List<Vector2Int>(printLocs);
        //extend radius if catnip
        if (hasFlag(typeof(Catnip))) {
            for (int x = -2; x <= 2; x++) {
                for (int y = -2; y <= 2; y++) {
                    if (Math.Abs(x) == 2 || Math.Abs(y) == 2) {
                        filteredPrintLocs.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        //restrict if trapped
        if (trapped) {
            for (int i = filteredPrintLocs.Count - 1; i >= 0; i--) {
                if (filteredPrintLocs[i].x != 0 && filteredPrintLocs[i].y != 0) {
                    filteredPrintLocs.RemoveAt(i);
                }
            }
        }
        //bounce off rubber
        for (int i = 0; i < filteredPrintLocs.Count; i++) {

            Vector2Int bouncedPrintLoc = filteredPrintLocs[i];
            if (!Floor.s.within(coord.x + bouncedPrintLoc.x, coord.y + bouncedPrintLoc.y)) continue;
            GameObject g = Floor.s.flags[coord.x + bouncedPrintLoc.x, coord.y + bouncedPrintLoc.y];
            while (g != null && g.GetComponent<FlagSprite>() is RubberSprite) {
                bouncedPrintLoc += filteredPrintLocs[i];
                if (!Floor.s.within(coord.x + bouncedPrintLoc.x, coord.y + bouncedPrintLoc.y)) break;
                g = Floor.s.flags[coord.x + bouncedPrintLoc.x, coord.y + bouncedPrintLoc.y];
            }
            filteredPrintLocs[i] = bouncedPrintLoc;

        }
		//filter out same direction as last if wobbly curse is active
		if (modifiers.wobbly) {
			for (int i = filteredPrintLocs.Count - 1; i >= 0; i--) {
				if (Vector2.Dot(lastMovement.normalized, ((Vector2) filteredPrintLocs[i]).normalized) > 0.99f) {
					filteredPrintLocs.RemoveAt(i);
				}
			}
		}
        //filter out of bounds or on a flag
        for (int i = filteredPrintLocs.Count - 1; i >= 0; i--) {
			int printX = coord.x + filteredPrintLocs[i].x, printY = coord.y + filteredPrintLocs[i].y;
            if (!Floor.s.within(printX, printY) ||
				Floor.s.tiles[printX, printY] == null ||
                Floor.s.flags[printX, printY] != null) {
                filteredPrintLocs.RemoveAt(i);
            }
        }
        //remove dupes
        filteredPrintLocs = new List<Vector2Int>(new HashSet<Vector2Int>(filteredPrintLocs));

        foreach (Vector2Int v in filteredPrintLocs) {
            GameObject p = Instantiate(GameManager.s.print_p);
            p.GetComponent<Print>().init(v.x, v.y);
            prints.Add(p);
        }

        UpdateActivePrints();
    }
    public void discover(int x, int y) {
        foreach (GameObject g in flags) {
            Flag flag = g.GetComponent<Flag>();
            if (flag is Map) {
                Map map = (flag as Map);
                if (map.usable) {
                    map.OnDiscover(x, y);
                }
            }
        }
    }
    public void triggerMines() {
		if (Player.s.alive && Floor.s.mines[coord.x, coord.y] != null) {
			Floor.s.mines[coord.x, coord.y].GetComponent<MineSprite>().Trigger();
		}
    }
    public void discoverTiles() {
        for (int dx=-discoverRange; dx<=discoverRange; dx++) {
            for (int dy=-discoverRange; dy<=discoverRange; dy++) {
                if (Floor.s.within(coord.x + dx, coord.y + dy)) {
                    discover(coord.x + dx, coord.y + dy);
                }
            }
        }
    } 
    public void setCoord(int x, int y, bool animate = true) {
        if (animate) {
            if (x - coord.x > 0) {
                transform.localScale = Vector3.one;
            } else if (x - coord.x < 0) {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (y - coord.y >= 0) {
                animator.SetTrigger("jumpUpStart");
            } else if (y - coord.y < 0) {
                animator.SetTrigger("jumpDownStart");
            } else {
                animator.SetTrigger("jumpNeutralStart");
            }
            coord = new Vector2Int(x, y);
            destroyPrints();
            LeanTween.move(gameObject, Floor.s.CoordToPos(x, y), 0.5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() => {
                animator.SetTrigger("jumpEnd");
                updatePrints();
                triggerMines();
                discoverTiles();
                if (Floor.s.tiles[x, y].GetComponent<ActionTile>() != null) {
					Floor.s.tiles[x, y].GetComponent<ActionTile>().PerformAction();
                } else {
                    transform.parent = Floor.s.tiles[x, y].transform;
                }
                Floor.s.tiles[x, y].GetComponent<Tile>().externalDepthImpulse += stepImpulse;
            }).setOnUpdate((float f) => {
                GameManager.s.disturbShaders(feet.transform.position.x, feet.transform.position.y);
            });
        } else {
            coord = new Vector2Int(x, y);
            transform.parent = Floor.s.tiles[0, 0].transform;
            transform.localPosition = Vector3.zero;
            destroyPrints();
            updatePrints();
            triggerMines();
            discoverTiles();
        }
        //wildcat passive
        if (hasFlag(typeof(Wildcat)) && !tilesVisited.Contains(Floor.s.tiles[x, y]) && Floor.s.tiles[x, y].GetComponent<MossyTile>() != null) {
            Flag w = UIManager.s.flagUIVars[typeof(Wildcat)].instances[0].GetComponent<Flag>();
            w.UpdateCount(w.count-1);
        }
		//fragile curse passive
		if (Floor.s.tiles[x, y].GetComponent<MossyTile>() != null || Floor.s.tiles[x, y].GetComponent<Puddle>() != null) {
			tempChanges++;
			if (tempChanges >= modifiers.tempChangesUntilDeath) {
				Die();
			}
		}
        tilesVisited.Add(Floor.s.tiles[x, y]);
		tilesUnvisited.Remove(Floor.s.tiles[x, y]);
    }
	public void setCoord(GameObject tile) {
		setCoord(tile.GetComponent<Tile>().coord.x, tile.GetComponent<Tile>().coord.y);
	}
    private void onFloorChange() {
        tilesVisited.Clear();
		tilesUnvisited.Clear();
		foreach (GameObject tile in Floor.s.tiles) {
			if (tile != null) {
				tilesUnvisited.Add(tile);
			}
		}
        setCoord(0, 0, false);
        setTrapped(false);
		tempChanges = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        s = this;
    }
    protected override void Start() {
        base.Start(); // get sprite renderer
        animator = GetComponent<Animator>();

        // initialize player bits
        texWidth = (int) sr.sprite.rect.width;
        texHeight = (int) sr.sprite.rect.height;
        playerBits = new GameObject[texWidth, texHeight];
        for (int i=0; i<texWidth; i++) {
            for (int j=0; j<texHeight; j++) {
                playerBits[i, j] = Instantiate(GameManager.s.playerBit_p);
                playerBits[i, j].GetComponent<PlayerBit>().Init(i, j);
            }
        }
    }
    protected override void Update() {
        base.Update(); // vertical object order
    }
    void OnEnable() {
        Floor.onFloorChange += onFloorChange;
    }
    void OnDisable() {
        Floor.onFloorChange -= onFloorChange;
    }
}
