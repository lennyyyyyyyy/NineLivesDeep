using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal; //2019 VERSIONS
using System;
using System.Linq;
using System.Collections.Generic;

// Floor is reloaded per run, not per application
public class Floor : MonoBehaviour {
    public static Floor s;
	public static readonly Vector2Int INVALID_COORD = new Vector2Int(-100, -100);

    // saved data
    public Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
    [System.NonSerialized]
    public int floor, width, height;
    [System.NonSerialized]
    public string floorType = "none";
    public HashSet<Vector2Int> rainCoords = new HashSet<Vector2Int>();
    [System.NonSerialized]
    public float floorStartTime = -1e9f;

    // unsaved data
    [System.NonSerialized]
	public Vector3 windDirection;
    [System.NonSerialized]
    public List<GameObject> backgroundTiles = new List<GameObject>();
    [System.NonSerialized]
    public List<Tunnel> tunnels = new List<Tunnel>();
    
    private ParticleSystem ambientDust;
	private ParticleSystemForceField windZone;
    
    public float Intro(string newFloorType, int newFloor) {
        EventManager.s.OnFloorChangeBeforeIntro?.Invoke();
        GameManager.s.floorGameState = GameManager.GameState.FLOOR_UNSTABLE;
		// FLOOR TRANSITION FIRST STEP - INTRO SEQUENCE
		floorType = newFloorType;
		floor = newFloor;

        string introduction = "";
        if (floorType == "minefield") {
            introduction = "Minefield, Level " + floor.ToString() + " - " + (floor * 10 + 5).ToString() + " meters under";
        } else if (floorType == "shop") {
            introduction = "Shop, Level " + floor.ToString() + " - " + (floor * 10 + 10).ToString() + " meters under";
        } else if (floorType == "trial") {
			introduction = "Time Trial, Level " + floor.ToString() + " - " + (floor * 10 + 7).ToString() + " meters under";
		}
		
		float time = 1f;

        HelperManager.s.DelayAction(() => {
            HelperManager.s.InstantiateBubble(Vector3.zero, introduction, Color.white, 2f, 2f);
        }, time);
		time += 2.8f;

        return time;
    }
	public void IntroAndCreateFloor(string newFloorType, int newFloor) {
        float time = Intro(newFloorType, newFloor);
		if (floorType == "minefield") {
			HelperManager.s.DelayAction(() => {
				HelperManager.s.InstantiateBubble(Vector3.zero, "a mine appears...", new Color(0.5f, 0.5f, 0.5f), 2f, 2f);
			}, time);
			HelperManager.s.DelayAction(() => {
				GameObject mine = Instantiate(PrefabManager.s.minePrefab, Vector3.zero, Quaternion.identity);
                if (floor == 0) {
                    mine.AddComponent<Hydra>();
                } else {
                    mine.AddComponent(PlayerUIItemModule.s.minesUnseen[Random.Range(0, PlayerUIItemModule.s.minesUnseen.Count)]);
                }
			}, time + 1f);
			time += 2.8f;
		}

		if (floorType == "minefield" && floor % 3 == 0) {
			HelperManager.s.DelayAction(() => {
				HelperManager.s.InstantiateBubble(Vector3.zero, "CURSED", new Color(0.5f, 0, 0), 2f, 2f);
			}, time);
			HelperManager.s.DelayAction(() => {
				GameObject curse = Instantiate(PrefabManager.s.cursePrefab, Vector3.zero, Quaternion.identity);
                curse.AddComponent(PlayerUIItemModule.s.cursesUnseen[Random.Range(0, PlayerUIItemModule.s.cursesUnseen.Count)]);
			}, time + 1f);
			time += 2.8f;
		}
        PlayerUIItemModule.s.OrganizeFlags();
		// FLOOR TRANSITION SECOND STEP - DESTROY THE PREVIOUS FLOOR AND RENEW ARRAYS
        HelperManager.s.DelayAction(() => {
			EventManager.s.OnFloorChangeBeforeNewLayout?.Invoke();
			if (floorType == "minefield") {
				InitMinefield();
			} else if (floorType == "shop") {
				InitShop();
			} else if (floorType == "trial") {
				InitTrial();
			}
			
			// FLOOR TRANSITION FOURTH STEP - BUILD NEW FLOOR
			EventManager.s.OnFloorChangeAfterEntities?.Invoke();	
            Player.s.Move(0, 0, animate: false);
			PositionTilesUnbuilt();
			BuildTiles(2f, 2f);
			MainCamera.s.ZoomTo(12.5f, 1f);	
            GameManager.s.floorGameState = GameManager.GameState.FLOOR_STABLE;
            PlayerUIItemModule.s.OrganizeFlags();
            floorStartTime = Time.time;
        }, time);
		time += 3.8f;

		HelperManager.s.DelayAction(() => { MainCamera.s.locked = false; }, time);
	}	
    public void IntroAndLoadFloor(LoadData loadData) {
        float time = Intro(loadData.floorType, loadData.floor);
        PlayerUIItemModule.s.OrganizeFlags();
        HelperManager.s.DelayAction(() => {
		    foreach (TileLoadData tld in loadData.tiles) {
                PrefabData pd = CatalogManager.s.typeToData[tld.type] as PrefabData;
                GameObject tile = ReplaceTile(pd.prefab, tld.coord.x, tld.coord.y);
                if (typeof(ActionTile).IsAssignableFrom(pd.type)) {
                    ActionTile at = tile.GetComponent<ActionTile>();
                    at.Init(tld.actionCode);
                    at.amount = tld.amount;
                }
                foreach (EntityLoadData eld in tld.nonPlayerEntities) {
                    GameObject entity;
                    if (typeof(FlagSprite).IsAssignableFrom(eld.type)) {
                        entity = PlaceFlagSpriteWithoutOnPlace(eld.type, tld.coord.x, tld.coord.y);
                    } else if (typeof(MineSprite).IsAssignableFrom(eld.type)) {
                        entity = PlaceMine(eld.type, tld.coord.x, tld.coord.y);
                    } else if (typeof(PickupSprite).IsAssignableFrom(eld.type)) {
                        entity = PlacePickupSprite(eld.correspondingUIType, eld.pickupCount, eld.pickupPrice, tld.coord);
                    } else {
                        entity = PlaceMiscEntity(eld.type, tld.coord.x, tld.coord.y);
                        if (eld.type == typeof(Crank)) {
                            entity.GetComponent<Crank>().Init(eld.crankDirection);
                        }
                    }
                }
            }
            Player.s.Move(loadData.playerCoord.x, loadData.playerCoord.y, animate: false);
			PositionTilesUnbuilt();
			BuildTiles(2f, 2f);
			MainCamera.s.ZoomTo(12.5f, 1f);	
            GameManager.s.floorGameState = GameManager.GameState.FLOOR_STABLE;
            PlayerUIItemModule.s.OrganizeFlags();
            floorStartTime = Time.time - loadData.floorStartTime;
        }, time);
		time += 3.8f;

		HelperManager.s.DelayAction(() => { MainCamera.s.locked = false; }, time);
    }
    public void InitLayout(int newWidth, int newHeight) {
		// FLOOR TRANSITION SECOND STEP - DESTROY THE PREVIOUS FLOOR AND RENEW ARRAYS
		
		//destroy previous tiles
        foreach (GameObject tile in tiles.Values.ToList()) {
            DestroyTile(tile);
        } 
        foreach (GameObject bt in backgroundTiles.ToList()) {
            DestroyBackgroundTile(bt);
        }

        width = newWidth;
        height = newHeight;

		//renew tile array
        tiles.Clear();
        backgroundTiles.Clear();

        for (int i=0; i<MaxSize() * MaxSize() * 1.25f; i++) {
            backgroundTiles.Add(Instantiate(PrefabManager.s.tileBackgroundPrefab, transform));
        }

		ParticleSystem.ShapeModule sh = ambientDust.shape;
        sh.scale = new Vector3(width, height, 1) * 1.5f * (1 + Player.s.modifiers.windStrength / 3f);
		ParticleSystem.EmissionModule em = ambientDust.emission;
        em.rateOverTime = sh.scale.x * sh.scale.y * (1 + Player.s.modifiers.windStrength / 3f) / 3f;
		windZone.endRange = Mathf.Max(sh.scale.x, sh.scale.y) * 0.75f;
		
		// FLOOR TRANSITION THIRD STEP - CREATE TILES, ENTITIES, AND MINES
		EventManager.s.OnFloorChangeBeforeEntities?.Invoke();
    }
    public void InitMinefield() {
        float variation = Random.value;
        List<Vector2Int> potentialTiles = new List<Vector2Int>();
        if (variation < 0.33f) { // normal shape
            InitLayout(2 * floor + 7 + Player.s.modifiers.floorExpansion, 2 * floor + 7 + Player.s.modifiers.floorExpansion);
            for (int i=0; i<width; i++) {
                for (int j=0; j<height; j++) {
                    potentialTiles.Add(new Vector2Int(i, j));
                }
            }
        } else if (variation < 0.66f) { // donut shape
            InitLayout(2 * floor + 8 + Player.s.modifiers.floorExpansion, 2 * floor + 8 + Player.s.modifiers.floorExpansion);
            float centerX = (width - 1) / 2f;
            float centerY = (height - 1) / 2f;
            float innerRadius = (3 + Player.s.modifiers.floorExpansion) / 2f;
            for (int i=0; i<width; i++) {
                for (int j=0; j<height; j++) {
                    float distToCenter = Mathf.Sqrt((i - centerX) * (i - centerX) + (j - centerY) * (j - centerY));
                    if (distToCenter >= innerRadius) {
                        potentialTiles.Add(new Vector2Int(i, j));
                    }
                }
            }
        } else { // heart / corner shape
            InitLayout(2 * floor + 8 + Player.s.modifiers.floorExpansion, 2 * floor + 8 + Player.s.modifiers.floorExpansion);
            int xCutoff = (int) Mathf.Ceil((width - 1) / 2f);
            int yCutoff = (int) Mathf.Ceil((height - 1) / 2f);
            for (int i=0; i<width; i++) {
                for (int j=0; j<height; j++) {
                    if (i < xCutoff || j < yCutoff) {
                        potentialTiles.Add(new Vector2Int(i, j));
                    }
                }
            }
        }
        Dictionary<int, List<int>> potentialTilesByX = new Dictionary<int, List<int>>(),
                                   potentialTilesByY = new Dictionary<int, List<int>>();
        foreach (Vector2Int coord in potentialTiles) {
            if (!potentialTilesByX.ContainsKey(coord.x)) {
                potentialTilesByX[coord.x] = new List<int>();
            }
            potentialTilesByX[coord.x].Add(coord.y);
            if (!potentialTilesByY.ContainsKey(coord.y)) {
                potentialTilesByY[coord.y] = new List<int>();
            }
            potentialTilesByY[coord.y].Add(coord.x);
        }
        HashSet<Vector2Int> potentialExits = new HashSet<Vector2Int>();
        foreach (KeyValuePair<int, List<int>> p in potentialTilesByX) {
            p.Value.Sort();
            potentialExits.Add(new Vector2Int(p.Key, p.Value[p.Value.Count - 1]));
        }
        foreach (KeyValuePair<int, List<int>> p in potentialTilesByY) {
            p.Value.Sort();
            potentialExits.Add(new Vector2Int(p.Key, p.Value[p.Value.Count - 1]));
        }
        List<Vector2Int> potentialExitsList = new List<Vector2Int>(potentialExits);

        Vector2Int exitCoord = potentialExitsList[Random.Range(0, potentialExitsList.Count)];
        ActionTile.ActionCode exitCode = (floor == ConstantsManager.s.finalFloor) ? ActionTile.ActionCode.WINGAME : ActionTile.ActionCode.EXITTOSHOP;
        ReplaceTile(PrefabManager.s.tileActionPrefab, exitCoord.x, exitCoord.y).GetComponent<ActionTile>().Init(exitCode);
		ReplaceTile(PrefabManager.s.tilePrefab, 0, 0);
        foreach (Vector2Int v in rainCoords) {
            ReplaceTile(PrefabManager.s.tilePuddlePrefab, v.x, v.y);
        }
		//random chance for trial entrance
		if (Random.value < ConstantsManager.s.minefieldTrialChance) {
            Vector2Int trialCoord = potentialTiles[Random.Range(0, potentialTiles.Count)];
			if (!TileExistsAt(trialCoord.x, trialCoord.y)) {
				GameObject t = ReplaceTile(PrefabManager.s.tileActionPrefab, trialCoord.x, trialCoord.y);
				t.GetComponent<ActionTile>().Init(ActionTile.ActionCode.EXITTOTRIAL);
			}
		}
        
        foreach (Vector2Int coord in potentialTiles) {
            int i = coord.x;
            int j = coord.y;
            if (!TileExistsAt(i, j) && Random.value < 1f - Player.s.modifiers.noTileChance) {
                float tileRand = Random.value;
                if (tileRand < 0.05f) {
                    ReplaceTile(PrefabManager.s.tilePuddlePrefab, i, j);
                } else if (tileRand < 0.1f) {
                    ReplaceTile(PrefabManager.s.tileMossyPrefab, i, j);
                } else {
                    ReplaceTile(PrefabManager.s.tilePrefab, i, j);
                }

                // put misc entities
                float entityRand = Random.value;
                if (entityRand < 0.06f) {
                    Type entityChoice = new Type[] { typeof(Crank),
                                                     typeof(Pillar),
                                                     typeof(Tunnel),
                                                     typeof(Vase), }[Random.Range(0, 4)];
                    PlaceMiscEntity(entityChoice, i, j);
                }
            }
            //generate mines
            if (PrelimMineSpawnCheck(i, j)) {
                float rand = Random.value;
                float mineMult = GetTile(i, j).GetComponent<Tile>().mineMult * Player.s.modifiers.mineSpawnMult;
                float totalMineChance = mineMult * (ConstantsManager.s.baseMineChance + ConstantsManager.s.mineChanceScaling * floor);
                for (int index = 0; index < PlayerUIItemModule.s.mines.Count; index++) {
                    Mine mine = PlayerUIItemModule.s.mines[index].GetComponent<Mine>();
                    if (rand < totalMineChance * (1 - (PlayerUIItemModule.s.mines.Count - index - 1) / (1.125f + PlayerUIItemModule.s.mines.Count * 0.875f))) {
                        PlaceMine(mine.spriteType, i, j);
                        break;
                    }
                }
            }
        }

        EventManager.s.OnNewMinefield?.Invoke();
    }
    public void InitShop() {
        int[] prices;
        float priceVariation = Random.value;
        if (priceVariation < 0.5f) {
            prices = new int[] { 0,
                                 15 + Random.Range(-3, 4),
                                 15 + Random.Range(-3, 4),
                                 15 + Random.Range(-3, 4), };
        } else {
            prices = new int[] { 7 + Random.Range(-2, 3),
                                 7 + Random.Range(-2, 3),
                                 7 + Random.Range(-2, 3),
                                 7 + Random.Range(-2, 3), };
        }
        HelperManager.s.Shuffle(ref prices);

        Vector2Int[] pickupCoords;
        List<Vector2Int> tileCoords = new List<Vector2Int>();
        Vector2Int exitCoord;
        float layoutVariation = Random.value;
        if (layoutVariation < 0.33f) { // square arrangement
            InitLayout(5, 5);
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    tileCoords.Add(new Vector2Int(i, j));
                }
            }
            pickupCoords = new Vector2Int[] { new Vector2Int(1, 1),
                                              new Vector2Int(3, 1),
                                              new Vector2Int(1, 3),
                                              new Vector2Int(3, 3), };
            exitCoord = new Vector2Int(4, 4);
        } else if (layoutVariation < 0.66f) { // line arrangement
            InitLayout(9, 3);
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 3; j++) {
                    tileCoords.Add(new Vector2Int(i, j));
                }
            }
            pickupCoords = new Vector2Int[] { new Vector2Int(1, 1),
                                              new Vector2Int(3, 1),
                                              new Vector2Int(5, 1),
                                              new Vector2Int(7, 1), };
            exitCoord = new Vector2Int(8, 0);
        } else { // diagonal arrangement
            InitLayout(6, 6);
            pickupCoords = new Vector2Int[] { new Vector2Int(1, 1),
                                              new Vector2Int(2, 2),
                                              new Vector2Int(3, 3),
                                              new Vector2Int(4, 4), };
            HashSet<Vector2Int> tempSet = new HashSet<Vector2Int>();
            foreach (Vector2Int v in pickupCoords) {
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        tempSet.Add(new Vector2Int(v.x + i, v.y + j));
                    }
                }
            }
            tileCoords = new List<Vector2Int>(tempSet);
            exitCoord = new Vector2Int(5, 5);
        }

        ReplaceTile(PrefabManager.s.tileActionPrefab, exitCoord.x, exitCoord.y).GetComponent<ActionTile>().Init(ActionTile.ActionCode.EXITTOMINEFIELD);
        foreach (Vector2Int coord in tileCoords) {
            if (!TileExistsAt(coord.x, coord.y)) {
                ReplaceTile(PrefabManager.s.tilePrefab, coord.x, coord.y);
            }
        }
        for (int i=0; i<4; i++) {
            PlacePickupSprite(PlayerUIItemModule.s.flagsUnseen, PickupSprite.SpawnType.SHOP, prices[i], pickupCoords[i]);
        }

        // extra flags - ex. Car passive
        List<Vector2Int> potentialExtraFlagCoords = tileCoords.Where(c => !pickupCoords.Contains(c) &&
                                                                     !(c.x == exitCoord.x && c.y == exitCoord.y) &&
                                                                     !(c.x == 0 && c.y == 0)).ToList();
        HelperManager.s.Shuffle(ref potentialExtraFlagCoords);
        for (int i=0; i<Mathf.Min(Player.s.modifiers.extraShopFlags, potentialExtraFlagCoords.Count); i++) {
            PlacePickupSprite(PlayerUIItemModule.s.flagsUnseen, PickupSprite.SpawnType.SHOP, 0, potentialExtraFlagCoords[i]);
        }
    }
	public void InitTrial() {
		InitLayout(floor + 7 + Player.s.modifiers.floorExpansion, floor + 7 + Player.s.modifiers.floorExpansion);
		
		ReplaceTile(PrefabManager.s.tileActionPrefab, width-2, height-2).GetComponent<ActionTile>().Init(ActionTile.ActionCode.GIVETRIALREWARD);
		ReplaceTile(PrefabManager.s.tileActionPrefab, width-1, height-1).GetComponent<ActionTile>().Init(ActionTile.ActionCode.EXITTOSHOP);
		for (int i = 0; i < width-1; i++) {
			for (int j = 0; j < height-1; j++) {
				if (!TileExistsAt(i, j)) {
					ReplaceTile(PrefabManager.s.tileActionPrefab, i, j);
                    float mineMult = GetTile(i, j).GetComponent<Tile>().mineMult * Player.s.modifiers.mineSpawnMult;
                    float totalMineChance = mineMult * (ConstantsManager.s.baseMineChance + ConstantsManager.s.mineChanceScaling * floor);
					if (PrelimMineSpawnCheck(i, j) && Random.value < totalMineChance) {
						PlaceMine(typeof(MineSprite), i, j);
					}
				}
			}
		}	
	}
    public int MaxSize() {
        return Mathf.Max(width, height);
    }
    public Vector3 CoordToPos(int x, int y) {
		if (TileExistsAt(x, y)) {
			return GetTile(x, y).GetComponent<Tile>().referencePos;
		}
		return CoordToIdealPos(x, y);
    }
	public Vector3 CoordToIdealPos(int x, int y) {
		return new Vector3(-width/2f + x + 0.5f, -height/2f + y + 0.5f, 0);
	}
    public Vector2Int PosToCoord(Vector3 v) {
        Collider2D[] cs = Physics2D.OverlapPointAll(new Vector2(v.x, v.y));
        foreach (Collider2D c in cs) {
            Tile t = c.gameObject.GetComponent<Tile>();
            if (t != null) {
                return t.coord;
            }
        }
        return INVALID_COORD;
    }
    public bool TileExistsAt(int x, int y) {
        return tiles.ContainsKey(new Vector2Int(x, y));
    }
	public GameObject GetTile(int x, int y) {
		return TileExistsAt(x, y) ? tiles[new Vector2Int(x, y)] : null;
	}
    // sets a tile in Floor and Tile memory, only sets existing tile as invalid without destroying - HELPER
	private GameObject SetTile(int x, int y, GameObject tile) {
		if (TileExistsAt(x, y)) {
			tiles[new Vector2Int(x, y)].GetComponent<Tile>().coord = INVALID_COORD;
		}
		if (tile == null) {
			tiles.Remove(new Vector2Int(x, y));
		} else {
			tiles[new Vector2Int(x, y)] = tile;
			tile.GetComponent<Tile>().coord = new Vector2Int(x, y);
		}
		return tile;
	}
    // animates the replacement of a tile, destroying former tile - API
    public GameObject ReplaceTile(GameObject prefab, int x, int y) {
		if (TileExistsAt(x, y)) {
			GetTile(x, y).GetComponent<Tile>().Unbuild(0.5f);
		}
        Tile t = SetTile(x, y, Instantiate(prefab, transform)).GetComponent<Tile>();
		t.PositionUnbuilt();
		t.Build(0.5f);
		Player.s.destroyPrints();
		Player.s.updatePrints();
		Player.s.discoverTiles();
		return GetTile(x, y);
	}
	public void MoveTiles(List<Vector2Int> fromCoords, List<Vector2Int> toCoords) {
		Dictionary<Vector2Int, GameObject> relevantTiles = new Dictionary<Vector2Int, GameObject>();
		for (int i = 0; i < fromCoords.Count; i++) {
			relevantTiles[fromCoords[i]] = GetTile(fromCoords[i].x, fromCoords[i].y);
			relevantTiles[toCoords[i]] = GetTile(toCoords[i].x, toCoords[i].y);
		}
		foreach (Vector2Int coord in relevantTiles.Keys) {
			SetTile(coord.x, coord.y, null);
		}
		for (int i = 0; i < fromCoords.Count; i++) {
			SetTile(toCoords[i].x, toCoords[i].y, relevantTiles[fromCoords[i]]);
		}
		foreach (Vector2Int coord in relevantTiles.Keys) {
			if (relevantTiles[coord] != null) {
				relevantTiles[coord].GetComponent<Tile>().Build(0.5f);
			}
		}
		Player.s.destroyPrints();
		HelperManager.s.DelayAction(() => {
			Player.s.updatePrints();
			Player.s.discoverTiles();
		}, 0.5f);
	}
	public void SwapTiles(int x1, int y1, int x2, int y2) {
		MoveTiles(new List<Vector2Int>() { new Vector2Int(x1, y1), new Vector2Int(x2, y2) },
				  new List<Vector2Int>() { new Vector2Int(x2, y2), new Vector2Int(x1, y1) });
	}
    // Destroys tile (pretty, with a delay) and cleans up memory, use this instead of Destroy() - API
	public void RemoveTile(int x, int y) {
		if (TileExistsAt(x, y)) {
			GetTile(x, y).GetComponent<Tile>().Unbuild(0.5f);
            tiles.Remove(new Vector2Int(x, y));
		}
		Player.s.destroyPrints();
		Player.s.updatePrints();
		Player.s.discoverTiles();
	}
    // Destroys tile (immediate) and cleans up memory, use this instead of Destroy() - API
    public void DestroyTile(int x, int y) {
        Destroy(GetTile(x, y));
        tiles.Remove(new Vector2Int(x, y));
    }
    public void DestroyTile(GameObject t) {
        Vector2Int coord = t.GetComponent<Tile>().coord;
        Destroy(t);
        tiles.Remove(coord);
    }
    // Destroys background tile and cleans up memory, use this instead of Destroy() - API
    public void DestroyBackgroundTile(GameObject bt) {
        backgroundTiles.Remove(bt);
        Destroy(bt);
    }
    public void PositionTilesUnbuilt() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
				if (TileExistsAt(i, j)) {
	                GetTile(i, j).GetComponent<Tile>().PositionUnbuilt();
				}
            }
        }
    }
    public void BuildTiles(float totalDuration, float buildDuration) {
        List<Tile> tileList = new List<Tile>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
				if (TileExistsAt(i, j)) {
                	tileList.Add(GetTile(i, j).GetComponent<Tile>());
                } 
            }
        }
        HelperManager.s.Shuffle(ref tileList);
        for (int i=0; i<tileList.Count; i++) {
            Tile tile = tileList[i];
            HelperManager.s.DelayAction(() => { tile.Build(buildDuration); }, totalDuration * i / tileList.Count);
        }
    }
    public GameObject PlaceFlagSpriteWithoutOnPlace(Type type, int x, int y) {
        GameObject g = Instantiate(PrefabManager.s.flagSpritePrefab);
        FlagSprite fs = g.AddComponent(type) as FlagSprite;
        fs.Init(new Vector2Int(x, y));
        return g;
    }
    public GameObject PlaceMine(Type t, int x, int y) {
        GameObject g = Instantiate(PrefabManager.s.mineSpritePrefab);
        MineSprite m = g.AddComponent(t) as MineSprite;
		if (!m.Move(x, y)) {
            Destroy(g);
		}
		return g;
    }
    public GameObject PlacePickupSprite(List<Type> pool, PickupSprite.SpawnType spawnType, int price, Vector2Int spawnCoord) {
        GameObject g = Instantiate(PrefabManager.s.flagSpritePrefab);
        PickupSprite ps = g.AddComponent(typeof(PickupSprite)) as PickupSprite;
        ps.Init(pool[Random.Range(0, pool.Count)], price, spawnType, spawnCoord);
        PlayerUIItemModule.s.NoticeFlag(ps.correspondingUIType);
        return g;
    }
    public GameObject PlacePickupSprite(Type type, int count, int price, Vector2Int spawnCoord) {
        GameObject g = Instantiate(PrefabManager.s.flagSpritePrefab);
        PickupSprite ps = g.AddComponent(typeof(PickupSprite)) as PickupSprite;
        ps.Init(type, price, PickupSprite.SpawnType.RANDOM, spawnCoord, count);
        PlayerUIItemModule.s.NoticeFlag(ps.correspondingUIType);
        return g;
    }
    public GameObject PlaceMiscEntity(Type type, int x, int y) {
        PrefabData pd = CatalogManager.s.typeToData[type] as PrefabData;
        GameObject g = Instantiate(pd.prefab);
        g.GetComponent<Entity>().Move(x, y);
        return g;
    }
	public bool PrelimMineSpawnCheck(int x, int y) {
		return (x != 0 || y != 0) && GetTile(x, y) != null;
	}
	public GameObject GetUniqueMine(int x, int y) {
		if (!TileExistsAt(x, y)) return null;
        return GetTile(x, y).GetComponent<Tile>().uniqueMine;
	}
    public bool HasEntityOfType<T>(int x, int y) {
        if (!TileExistsAt(x, y)) return false;
        return GetTile(x, y).GetComponent<Tile>().HasEntityOfType<T>();
    }
	public List<GameObject> GetEntities(int x, int y) {	
		if (TileExistsAt(x, y)) {
			return GetTile(x, y).GetComponent<Tile>().entities;
		}
		return null;
	}
	// note helper - use Entity.Move() instead
	public void AddEntity(int x, int y, GameObject g) {
		if (TileExistsAt(x, y)) {
			GetTile(x, y).GetComponent<Tile>().AddEntity(g);
		} else {
			Debug.Log("Tried to add entity to invalid tile at " + x + ", " + y);
		}
	}
    // Creates the default sequence for exiting a floor into a new floor
	public void ExitFloor(string newFloorType, int newFloor) {
        Player.s.Remove(false);
        MainCamera.s.locked = true;
        MainCamera.s.ExitMotion();
        HelperManager.s.DelayAction(() => {Floor.s.IntroAndCreateFloor(newFloorType, newFloor);}, 0.5f);
	}	
    private void Awake() {
        s = this;
		ambientDust = GetComponent<ParticleSystem>();
		windZone = GetComponent<ParticleSystemForceField>();
    }
	private void Update() {
		windDirection = Quaternion.AngleAxis(Mathf.PerlinNoise(Player.s.modifiers.windFluctuation * Time.time, 0) * 1000f, Vector3.forward) * Vector3.right;
		windZone.directionX = windDirection.x * Player.s.modifiers.windStrength * 1.5f;
		windZone.directionY = windDirection.y * Player.s.modifiers.windStrength * 1.5f;
	}
}
