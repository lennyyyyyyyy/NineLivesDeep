using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal; //2019 VERSIONS
using System;
using System.Collections.Generic;
public class Floor : MonoBehaviour
{
    public static Floor s;
	public static readonly Vector2Int INVALID_COORD = new Vector2Int(-100, -100);
    public Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
    [System.NonSerialized]
    public List<GameObject> backgroundTiles = new List<GameObject>();
    private ParticleSystem ambientDust;
	private ParticleSystemForceField windZone;
    [System.NonSerialized]
    public int floor, width, height;
    [System.NonSerialized]
    public string floorType = "none";
    public static Action onFloorChangeBeforeNewLayout,
                         onFloorChangeBeforeEntities,
                         onFloorChangeAfterEntities,
                         onNewMinefield;
    public static Action<int, int> onExplosionAtCoord;
    public float tileExternalPower = 0.65f, tileDampingPower = 0.65f, tileAdjacentDragPower = 0.5f;
	[System.NonSerialized]
	public int floorDeathCount = 0;
	public float trialChance = 0.5f;
	[System.NonSerialized]
	public Vector3 windDirection;
	
	public void IntroAndCreateFloor(string newFloorType, int newFloor) {
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
		MainCamera.s.SetupFloorIntro();
		
		float time = 1f;

        GameManager.s.DelayAction(() => {
            UIManager.s.InstantiateBubble(Vector3.zero, introduction, Color.white, 2f, 2f);
        }, time);
		time += 2.8f;

		if (floorType == "minefield") {
			GameManager.s.DelayAction(() => {
				UIManager.s.InstantiateBubble(Vector3.zero, "a mine appears...", new Color(0.5f, 0.5f, 0.5f), 2f, 2f);
			}, time);
			GameManager.s.DelayAction(() => {
				GameObject mine = Instantiate(GameManager.s.mine_p, Vector3.zero, Quaternion.identity, UIManager.s.GAMEUI.transform);
                if (floor == 0) {
                    mine.AddComponent<Mine>();
                } else {
                    mine.AddComponent(Player.s.minesUnseen[Random.Range(0, Player.s.minesUnseen.Count)]);
                }
			}, time + 1f);
			time += 2.8f;
		}

		if (floorType == "minefield" && floor % 3 == 0) {
			GameManager.s.DelayAction(() => {
				UIManager.s.InstantiateBubble(Vector3.zero, "CURSED", new Color(0.5f, 0, 0), 2f, 2f);
			}, time);
			GameManager.s.DelayAction(() => {
				GameObject curse = Instantiate(GameManager.s.curse_p, Vector3.zero, Quaternion.identity, UIManager.s.GAMEUI.transform);
				curse.AddComponent(Player.s.cursesUnseen[Random.Range(0, Player.s.cursesUnseen.Count)]);
			}, time + 1f);
			time += 2.8f;
		}

		// FLOOR TRANSITION SECOND STEP - DESTROY THE PREVIOUS FLOOR AND RENEW ARRAYS
        GameManager.s.DelayAction(() => {
			onFloorChangeBeforeNewLayout?.Invoke();
			if (floorType == "minefield") {
				InitMinefield();
			} else if (floorType == "shop") {
				InitShop();
			} else if (floorType == "trial") {
				InitTrial();
			}
			
			// FLOOR TRANSITION FOURTH STEP - BUILD NEW FLOOR
			onFloorChangeAfterEntities?.Invoke();	
			
			floorDeathCount = 0;
			PositionTilesUnbuilt();
			BuildTiles(2f, 2f);
			MainCamera.s.ZoomTo(12.5f, 1f);	
			UIManager.s.OrganizeFlags();
        }, time);
		time += 3.8f;

		GameManager.s.DelayAction(() => { MainCamera.s.locked = false; }, time);
	}	
    public void InitLayout(int newWidth, int newHeight, string newFloorType) {
		// FLOOR TRANSITION SECOND STEP - DESTROY THE PREVIOUS FLOOR AND RENEW ARRAYS
		
		//destroy previous tiles
        foreach (GameObject tile in tiles.Values) {
            Destroy(tile);
        } 
        foreach (GameObject bt in backgroundTiles) {
            Destroy(bt);
        }

        width = newWidth;
        height = newHeight;

		//renew tile array
        tiles.Clear();
        backgroundTiles.Clear();

        for (int i=0; i<MaxSize() * MaxSize() * 1.25f; i++) {
            backgroundTiles.Add(Instantiate(GameManager.s.tile_background_p, transform));
        }

		ParticleSystem.ShapeModule sh = ambientDust.shape;
        sh.scale = new Vector3(width, height, 1) * 1.5f * (1 + Player.s.modifiers.windStrength / 3f);
		ParticleSystem.EmissionModule em = ambientDust.emission;
        em.rateOverTime = sh.scale.x * sh.scale.y * (1 + Player.s.modifiers.windStrength / 3f) / 3f;
		windZone.endRange = Mathf.Max(sh.scale.x, sh.scale.y) * 0.75f;
		
		// FLOOR TRANSITION THIRD STEP - CREATE TILES, ENTITIES, AND MINES
		onFloorChangeBeforeEntities?.Invoke();
    }
    public void InitMinefield() {
        float variation = Random.value;
        List<Vector2Int> potentialTiles = new List<Vector2Int>();
        if (variation < 0.33f) { // normal shape
            InitLayout(2 * floor + 7 + Player.s.modifiers.floorExpansion, 2 * floor + 7 + Player.s.modifiers.floorExpansion, "minefield");
            for (int i=0; i<width; i++) {
                for (int j=0; j<height; j++) {
                    potentialTiles.Add(new Vector2Int(i, j));
                }
            }
        } else if (variation < 0.66f) { // donut shape
            InitLayout(2 * floor + 8 + Player.s.modifiers.floorExpansion, 2 * floor + 8 + Player.s.modifiers.floorExpansion, "minefield");
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
            InitLayout(2 * floor + 8 + Player.s.modifiers.floorExpansion, 2 * floor + 8 + Player.s.modifiers.floorExpansion, "minefield");
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
        ReplaceTile(GameManager.s.tile_exit_p, exitCoord.x, exitCoord.y).GetComponent<ActionTile>().Init(ActionTile.EXITTOSHOP);
		ReplaceTile(GameManager.s.tile_p, 0, 0);
        foreach (Vector2Int v in Raincloud.rainCoords) {
            ReplaceTile(GameManager.s.tile_puddle_p, v.x, v.y);
        }
		//random chance for trial entrance
		if (Random.value < trialChance) {
            Vector2Int trialCoord = potentialTiles[Random.Range(0, potentialTiles.Count)];
			if (!TileExistsAt(trialCoord.x, trialCoord.y)) {
				GameObject t = ReplaceTile(GameManager.s.tile_exit_p, trialCoord.x, trialCoord.y);
				t.GetComponent<ActionTile>().Init(ActionTile.EXITTOTRIAL);
			}
		}
        
        foreach (Vector2Int coord in potentialTiles) {
            int i = coord.x;
            int j = coord.y;
            if (!TileExistsAt(i, j) && Random.value < 1f - Player.s.modifiers.noTileChance) {
                float tileRand = Random.value;
                if (tileRand < 0.05f) {
                    ReplaceTile(GameManager.s.tile_puddle_p, i, j);
                } else if (tileRand < 0.1f) {
                    ReplaceTile(GameManager.s.tile_mossy_p, i, j);
                } else {
                    ReplaceTile(GameManager.s.tile_p, i, j);
                }

                // put misc entities
                float entityRand = Random.value;
                if (entityRand < 0.10f) {
                    GameObject g = Instantiate(GameManager.s.crank_p);
                    g.GetComponent<Entity>().Move(i, j);
                }
            }
            //generate mines
            if (PrelimMineSpawnCheck(i, j)) {
                float rand = Random.value;
                float mineMult = GetTile(i, j).GetComponent<Tile>().mineMult * Player.s.modifiers.mineSpawnMult;
                float totalMineChance = mineMult * (0.1f + 0.05f * floor);
                for (int index = 0; index < Player.s.mines.Count; index++) {
                    Mine mine = Player.s.mines[index].GetComponent<Mine>();
                    if (rand < totalMineChance * (1 - (Player.s.mines.Count - index - 1) / (1.125f + Player.s.mines.Count * 0.875f))) {
                        PlaceMine(mine.spriteType, i, j);
                        break;
                    }
                }
            }
        }

        onNewMinefield?.Invoke();
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
        GameManager.s.Shuffle(ref prices);

        Vector2Int[] pickupCoords;
        List<Vector2Int> tileCoords = new List<Vector2Int>();
        Vector2Int exitCoord;
        float layoutVariation = Random.value;
        if (layoutVariation < 0.33f) { // square arrangement
            InitLayout(5, 5, "shop");
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
            InitLayout(9, 3, "shop");
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
            InitLayout(6, 6, "shop");
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

        ReplaceTile(GameManager.s.tile_exit_p, exitCoord.x, exitCoord.y).GetComponent<ActionTile>().Init(ActionTile.EXITTOMINEFIELD);
        foreach (Vector2Int coord in tileCoords) {
            if (!TileExistsAt(coord.x, coord.y)) {
                ReplaceTile(GameManager.s.tile_p, coord.x, coord.y);
            }
        }
        for (int i=0; i<4; i++) {
            PlacePickupSprite(Player.s.flagsUnseen, PickupSprite.SpawnType.SHOP, prices[i], pickupCoords[i]);
        }
    }
	public void InitTrial() {
		InitLayout(floor + 7 + Player.s.modifiers.floorExpansion, floor + 7 + Player.s.modifiers.floorExpansion, "trial");
		
		ReplaceTile(GameManager.s.tile_exit_p, width-2, height-2).GetComponent<ActionTile>().Init(ActionTile.GIVETRIALREWARD);
		ReplaceTile(GameManager.s.tile_exit_p, width-1, height-1).GetComponent<ActionTile>().Init(ActionTile.EXITTOSHOP);
		for (int i = 0; i < width-1; i++) {
			for (int j = 0; j < height-1; j++) {
				if (!TileExistsAt(i, j)) {
					ReplaceTile(GameManager.s.tile_p, i, j);
					if (PrelimMineSpawnCheck(i, j) && Random.value < 0.2 * Player.s.modifiers.mineSpawnMult) {
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
		GameManager.s.DelayAction(() => {
			Player.s.updatePrints();
			Player.s.discoverTiles();
		}, 0.5f);
	}
	public void SwapTiles(int x1, int y1, int x2, int y2) {
		MoveTiles(new List<Vector2Int>() { new Vector2Int(x1, y1), new Vector2Int(x2, y2) },
				  new List<Vector2Int>() { new Vector2Int(x2, y2), new Vector2Int(x1, y1) });
	}
	public void RemoveTile(int x, int y) {
		if (TileExistsAt(x, y)) {
			GetTile(x, y).GetComponent<Tile>().Unbuild(0.5f);
			SetTile(x, y, null);
		}
		Player.s.destroyPrints();
		Player.s.updatePrints();
		Player.s.discoverTiles();
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
        GameManager.s.Shuffle(ref tileList);
        for (int i=0; i<tileList.Count; i++) {
            Tile tile = tileList[i];
            GameManager.s.DelayAction(() => { tile.Build(buildDuration); }, totalDuration * i / tileList.Count);
        }
    }
    public GameObject PlaceMine(Type t, int x, int y) {
        GameObject g = Instantiate(GameManager.s.mineSprite_p);
        MineSprite m = g.AddComponent(t) as MineSprite;
		if (m.Move(x, y)) {
			return g;
		}
		Destroy(g);
		return null;
    }
    public void PlacePickupSprite(List<Type> pool, PickupSprite.SpawnType spawnType, int price, Vector2Int spawnCoord) {
        GameObject g = Instantiate(GameManager.s.flagSprite_p);
        PickupSprite ps = g.AddComponent(typeof(PickupSprite)) as PickupSprite;
        ps.SetInitialData(pool[Random.Range(0, pool.Count)], price, spawnType, spawnCoord);
        Player.s.NoticeFlag(ps.correspondingUIType);
    }
    public bool TileExistsAt(int x, int y) {
        return tiles.ContainsKey(new Vector2Int(x, y));
    }
	public GameObject GetTile(int x, int y) {
		return TileExistsAt(x, y) ? tiles[new Vector2Int(x, y)] : null;
	}
	public GameObject SetTile(int x, int y, GameObject tile) {
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
	public bool PrelimMineSpawnCheck(int x, int y) {
		return (x != 0 || y != 0) && GetTile(x, y) != null;
	}
	public GameObject GetUniqueFlag(int x, int y) {
		if (TileExistsAt(x, y)) {
			return GetTile(x, y).GetComponent<Tile>().GetUniqueFlag();
		}
		return null;
	}
	public GameObject GetUniqueMine(int x, int y) {
		if (TileExistsAt(x, y)) {
			return GetTile(x, y).GetComponent<Tile>().GetUniqueMine();
		}
		return null;
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
	// note helper - use Entity.Remove() instead
	public void RemoveEntity(int x, int y, GameObject g) {
		if (TileExistsAt(x, y)) {
			GetTile(x, y).GetComponent<Tile>().entities.Remove(g);
			g.transform.parent = null;
		} else {
			Debug.Log("Tried to remove entity from invalid tile at " + x + ", " + y);
		}
	}
    private void Awake() {
        s = this;
    }
	private void Start() {
		ambientDust = GetComponent<ParticleSystem>();
		windZone = GetComponent<ParticleSystemForceField>();
	}
	private void Update() {
		windDirection = Quaternion.AngleAxis(Mathf.PerlinNoise(Player.s.modifiers.windFluctuation * Time.time, 0) * 1000f, Vector3.forward) * Vector3.right;
		windZone.directionX = windDirection.x * Player.s.modifiers.windStrength * 1.5f;
		windZone.directionY = windDirection.y * Player.s.modifiers.windStrength * 1.5f;
	}
    private void STARTToGAME() {
        GameManager.s.DelayAction(() => {IntroAndCreateFloor("minefield", 0);}, 1f);
    }
    private void OnEnable() {
        GameManager.OnSTARTToGAME += STARTToGAME;
    }
    private void OnDisable() {
        GameManager.OnSTARTToGAME -= STARTToGAME;
    }
}
