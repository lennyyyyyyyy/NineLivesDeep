using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using System.Linq;

public class Modifiers {
	public float mineSpawnMult, 
		         mineDefuseMult,
				 noTileChance,
				 windStrength,
				 windFluctuation,
				 cameraShakeStrength,
				 cameraShakePeriod,
				 vision,
				 mapNumberDisappearChancePerSecond,
				 watchedMineJumpTime,
				 watchedMineJumpChancePerSecond,
				 cataractConfuseChance;
	public int floorExpansion,
		       tempChangesUntilDeath,
			   interactRange,
			   discoverRange,
			   reviveRange,
			   moveDirectionDisableDuration,
               reflectionPassiveCount;
	public bool watched;
	public HashSet<Type> amnesiaUITypes;	
    public List<Vector2Int> moveOptions;
    public HashSet<GameObject> takenFlags;
	public void Reset() {
		mineSpawnMult = 1;
		mineDefuseMult = 1;
		floorExpansion = 0;
		noTileChance = 0.1f;
		windStrength = 0;
		windFluctuation = 0.4f;
		cameraShakeStrength = 0;
		cameraShakePeriod = 1f;
		vision = 2f;
		interactRange = 1;
		discoverRange = 0;
		reviveRange = 0;
		moveDirectionDisableDuration = 0;
		amnesiaUITypes = new HashSet<Type>();
		cataractConfuseChance = 0f;
		mapNumberDisappearChancePerSecond = 0;
		watched = false;
		watchedMineJumpTime = 0f;
		watchedMineJumpChancePerSecond = 0f;
		tempChangesUntilDeath = (int) 2e9;
        reflectionPassiveCount = 0;
        moveOptions = new List<Vector2Int>() {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),
            new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1)
        };
        takenFlags = new HashSet<GameObject>();
	}
	public Modifiers() {
		Reset();
	}
}

// Player is reloaded per run, not per application
public class Player : Entity {
    public static Player s;

    // saved data
    [System.NonSerialized]
    public bool trapped = false, alive = true;
    [System.NonSerialized]
    public float money = 0;
    [System.NonSerialized]
	public int tempChanges = 0;
	[System.NonSerialized]
	public int floorDeathCount = 0;
    public HashSet<GameObject> tilesVisited = new HashSet<GameObject>();
	[System.NonSerialized]
	public List<Vector2Int> moveHistory = new List<Vector2Int>();
    [System.NonSerialized]
    public bool tunneledLastMove = false;

    // unsaved data
    [System.NonSerialized]
    public List<GameObject> prints = new List<GameObject>();
    [System.NonSerialized]
    public List<GameObject> tilesUnvisited = new List<GameObject>();
    public GameObject light, blood, feet;
    public GameObject[,] playerBits;
    [System.NonSerialized]
    public Animator animator;
    [System.NonSerialized]
    public int texWidth, texHeight;
	public Modifiers modifiers = new Modifiers();
	[System.NonSerialized]
	public float watchedMineJumpTimer = 0f;
    [System.NonSerialized]
    public bool dogFlagActive = false,
                mapNumberActive = true;

    protected override void BeforeInit() {
        base.BeforeInit();
        s = this;
		marker = feet; // for VerticalObject
        animator = GetComponent<Animator>();
        // initialize player bits
        texWidth = (int) sr.sprite.rect.width;
        texHeight = (int) sr.sprite.rect.height;
        playerBits = new GameObject[texWidth, texHeight];
        for (int i=0; i<texWidth; i++) {
            for (int j=0; j<texHeight; j++) {
                playerBits[i, j] = Instantiate(PrefabManager.s.playerBitPrefab);
                playerBits[i, j].GetComponent<PlayerBit>().Init(i, j);
            }
        }
    }
    public override void Init() {
        Init(obstacle: false);
    }
    protected override void Update() {
        base.Update(); // vertical object order
		if (modifiers.watched) {
			watchedMineJumpTimer += Time.deltaTime;
		}
    }
    public void Load(LoadData loadData) {
        money = loadData.money;
        if (!loadData.playerAlive) {
            Die();
        }
    }
	public void RecalculateModifiers() {
		modifiers.Reset();
		foreach (GameObject flag in PlayerUIItemModule.s.flags) {
			flag.GetComponent<UIItem>().Modify(ref modifiers);
		}
		foreach (GameObject notFlag in PlayerUIItemModule.s.notFlags) {
			notFlag.GetComponent<UIItem>().Modify(ref modifiers);
		}
	}
    public void Die() {
        if (!alive) return;
		floorDeathCount++;
        alive = false;
        sr.enabled = false;
        EventManager.s.OnPlayerAliveChange?.Invoke();
        EventManager.s.OnPlayerDie?.Invoke();
        UpdateActivePrints();
    }
    public void Revive() {
        EventManager.s.OnPlayerRevive?.Invoke();
        HelperManager.s.DelayAction(() => {
            sr.enabled = true; 
            alive = true;
            EventManager.s.OnPlayerAliveChange?.Invoke();
            UpdateActivePrints();
			tempChanges = 0;
			TriggerMines();
        }, ConstantsManager.s.playerReviveDuration);
    }
    public void UpdateActivePrints() {
        bool active = alive;
        foreach (GameObject g in PlayerUIItemModule.s.flags) {
            Flag flag = g.GetComponent<Flag>();
            if (flag is Placeable) {
                active &= (flag as Placeable).sprite == null || (flag as Placeable).sprite.GetComponent<FlagSprite>().state == "dropped";
            }
        }
        foreach (GameObject print in prints) {
            print.SetActive(active);
        }
    }
    public void UpdateMoney(float newCount) {
        HelperManager.s.InstantiateBubble(MineUIItem.s.gameObject, (newCount - money >= 0 ? "+" : "-") + (Mathf.Round(Mathf.Abs(newCount - money)*100f)/100f).ToString(), Color.white);
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
        List<Vector2Int> filteredPrintLocs = new List<Vector2Int>(modifiers.moveOptions);
        //restrict if trapped
        if (trapped) {
            for (int i = filteredPrintLocs.Count - 1; i >= 0; i--) {
                if (filteredPrintLocs[i].x != 0 && filteredPrintLocs[i].y != 0) {
                    filteredPrintLocs.RemoveAt(i);
                }
            }
        }
        //bounce off rubber
        for (int i=0; i<filteredPrintLocs.Count; i++) {
            int bounces = 1;
            while (Floor.s.HasEntityOfType<RubberSprite>(GetCoord().x + filteredPrintLocs[i].x * bounces,
                                                         GetCoord().y + filteredPrintLocs[i].y * bounces)) {
                bounces++;
            }
            filteredPrintLocs[i] *= bounces;
        }
		//filter out disabled moves from wobbly
		for (int i = moveHistory.Count - 1; i >= Mathf.Max(0, moveHistory.Count - modifiers.moveDirectionDisableDuration); i--) {
			for (int j = filteredPrintLocs.Count - 1; j >= 0; j--) {
				if (Vector2.Dot(((Vector2) moveHistory[i]).normalized, ((Vector2) filteredPrintLocs[j]).normalized) > 0.99f) {
					filteredPrintLocs.RemoveAt(j);
				}
			}
		}
        //filter out of bounds or on obstacle entity
        for (int i = filteredPrintLocs.Count - 1; i >= 0; i--) {
			int printX = GetCoord().x + filteredPrintLocs[i].x, printY = GetCoord().y + filteredPrintLocs[i].y;
			bool remove = !Floor.s.TileExistsAt(printX, printY);
			if (!remove) {
				foreach (GameObject entity in Floor.s.GetEntities(printX, printY)) {
					remove |= entity.GetComponent<Entity>().obstacle;
				}
			}
            if (remove) {
                filteredPrintLocs.RemoveAt(i);
            }
        }
        //remove dupes
        filteredPrintLocs = new List<Vector2Int>(new HashSet<Vector2Int>(filteredPrintLocs));

        foreach (Vector2Int v in filteredPrintLocs) {
            GameObject p = Instantiate(PrefabManager.s.printPrefab);
            p.GetComponent<Print>().init(v.x, v.y);
            prints.Add(p);
        }

        UpdateActivePrints();
    }
    public void discover(int x, int y) {
        if (new Vector2Int(x, y) == Floor.INVALID_COORD) return;
        foreach (GameObject g in PlayerUIItemModule.s.flags) {
            Flag flag = g.GetComponent<Flag>();
            if (flag is Map) {
                Map map = (flag as Map);
                if (map.usable) {
                    map.OnDiscover(x, y);
                }
            }
        }
    }
    public void TriggerMines() {
		if (alive && !dogFlagActive && Floor.s.GetUniqueMine(GetCoord().x, GetCoord().y) != null) {
			Floor.s.GetUniqueMine(GetCoord().x, GetCoord().y).GetComponent<MineSprite>().Trigger();
		}
    }
    public void TriggerDogEffect() {
        if (alive && dogFlagActive) {
            GameObject mine = Floor.s.GetUniqueMine(GetCoord().x, GetCoord().y);
            if (mine == null) {
                Die();
            }
        }
        dogFlagActive = false;
    }
    public void discoverTiles() {
        for (int dx=-modifiers.discoverRange; dx<=modifiers.discoverRange; dx++) {
            for (int dy=-modifiers.discoverRange; dy<=modifiers.discoverRange; dy++) {
                discover(GetCoord().x + dx, GetCoord().y + dy);
            }
        }
    } 
	public override bool Move(int x, int y, bool reposition = true) {
		return Move(x, y, reposition, true);
	}
	public virtual bool Move(int x, int y, bool reposition = true, bool animate = true) {
        if (!CoordAllowed(x, y)) {
			Debug.Log("Tried to move player to invalid coord " + x + ", " + y);
            return false;
        }
        if (animate) {
            if (x - GetCoord().x > 0) {
                transform.localScale = Vector3.one;
            } else if (x - GetCoord().x < 0) {
                transform.localScale = new Vector3(-1, 1, 1);
            } else if (transform.localScale.x > 0) {
                transform.localScale = Vector3.one;
            } else {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (y - GetCoord().y >= 0) {
                animator.SetTrigger("jumpUpStart");
            } else if (y - GetCoord().y < 0) {
                animator.SetTrigger("jumpDownStart");
            } else {
                animator.SetTrigger("jumpNeutralStart");
            }
            base.Move(x, y, false);
            destroyPrints();
            LeanTween.moveLocal(gameObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() => {
                animator.SetTrigger("jumpEnd");
                destroyPrints();
                updatePrints();
                TriggerMines();
                TriggerDogEffect();
                discoverTiles();
                if (Floor.s.GetTile(x, y).GetComponent<ActionTile>() != null) {
                    Floor.s.GetTile(x, y).GetComponent<ActionTile>().PerformAction();
                } 
                Floor.s.GetTile(x, y).GetComponent<Tile>().externalDepthImpulse += ConstantsManager.s.playerStepImpulse;
                EventManager.s.OnPlayerMoveToCoord?.Invoke(x, y);
            }).setOnUpdate((float f) => {
                ShaderManager.s.DisturbShaders(feet.transform.position.x, feet.transform.position.y);
            });
        } else {
            base.Move(x, y, reposition);
            TriggerMines();
            TriggerDogEffect();
            discoverTiles();
            EventManager.s.OnPlayerMoveToCoord?.Invoke(x, y);
        }
        //fragile curse passive
        if (Floor.s.GetTile(x, y).GetComponent<MossyTile>() != null || Floor.s.GetTile(x, y).GetComponent<Puddle>() != null) {
            tempChanges++;
            if (tempChanges >= modifiers.tempChangesUntilDeath) {
                Die();
            }
        }
        tilesVisited.Add(Floor.s.GetTile(x, y));
        tilesUnvisited.Remove(Floor.s.GetTile(x, y));
        //watched curse reset
        if (modifiers.watched) {
            watchedMineJumpTimer = 0f;
        }
        return true;
	}
	public virtual void Move(GameObject tile, bool reposition = true, bool animate = true) {
		Move(tile.GetComponent<Tile>().coord.x, tile.GetComponent<Tile>().coord.y, reposition, animate);
	}
    private void OnFloorChangeAfterEntities() {
        floorDeathCount = 0;
		tempChanges = 0;
        tilesVisited.Clear();
		tilesUnvisited.Clear();
		foreach (GameObject tile in Floor.s.tiles.Values) {
			if (tile != null) {
				tilesUnvisited.Add(tile);
			}
		}
    }
    private void OnFloorChangeBeforeNewLayout() {
        Remove(false);
    }
    private void OnExplosionAtCoord(int x, int y, GameObject source) {
        Vector2Int coord = GetCoord();
        if (coord.x == x && coord.y == y) {
            Die();
        }
    }
    private void OnForceMapNumberActive(bool active) {
        mapNumberActive = active;
    }
    private void OnEnable() {
        EventManager.s.OnFloorChangeBeforeNewLayout += OnFloorChangeBeforeNewLayout;
        EventManager.s.OnFloorChangeAfterEntities += OnFloorChangeAfterEntities;
        EventManager.s.OnExplosionAtCoord += OnExplosionAtCoord;
        EventManager.s.OnForceMapNumberActive += OnForceMapNumberActive;
    }
    private void OnDisable() {
        EventManager.s.OnFloorChangeBeforeNewLayout -= OnFloorChangeBeforeNewLayout;
        EventManager.s.OnFloorChangeAfterEntities -= OnFloorChangeAfterEntities;
        EventManager.s.OnExplosionAtCoord -= OnExplosionAtCoord;
        EventManager.s.OnForceMapNumberActive -= OnForceMapNumberActive;
    }
}
