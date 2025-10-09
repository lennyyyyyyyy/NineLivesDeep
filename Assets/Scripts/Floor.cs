using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal; //2019 VERSIONS
using System;
using System.Collections.Generic;
public class Floor : MonoBehaviour
{
    public static Floor s;
    public GameObject[,] mines, tiles, flags;
    [System.NonSerialized]
    public List<GameObject> backgroundTiles = new List<GameObject>();
    private ParticleSystem ambientDust;
	private ParticleSystemForceField windZone;
    [System.NonSerialized]
    public int floor, width, height;
    [System.NonSerialized]
    public string floorType = "none";
    public static Action onFloorChange, onNewMinefield;
    public float tileExternalPower = 0.65f, tileDampingPower = 0.65f, tileAdjacentDragPower = 0.5f;
	[System.NonSerialized]
	public int floorDeathCount = 0;
	public float trialChance = 0.5f;
	[System.NonSerialized]
	public Vector3 windDirection;
	public void IntroAndCreateFloor(string newFloorType, int newFloor) {
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

		if (floorType == "minefield" && floor % 3 == 0) {
			GameManager.s.DelayAction(() => {
				UIManager.s.InstantiateBubble(Vector3.zero, "CURSED", new Color(0.5f, 0, 0), 2f, 2f);
			}, time);
			GameManager.s.DelayAction(() => {
				GameObject curse = Instantiate(GameManager.s.curse_p, Vector3.zero, Quaternion.identity, UIManager.s.GAMEUI.transform);
				curse.AddComponent(Player.s.cursesUnseen[Random.Range(0, Player.s.cursesUnseen.Count)]);
				//curse.AddComponent(typeof(Taken));
			}, time + 1f);
			time += 2.8f;
		}

        GameManager.s.DelayAction(() => {
			if (floorType == "minefield") {
				InitMinefield();
			} else if (floorType == "shop") {
				InitShop();
			} else if (floorType == "trial") {
				InitTrial();
			}
			onFloorChange?.Invoke();
        }, time);
	}	
    public void FloorChange() {
		floorDeathCount = 0;
        PositionTilesUnbuilt();

		float time = 0f;
		GameManager.s.DelayAction(() => {
			BuildTiles(2f, 2f);
			MainCamera.s.ZoomTo(12.5f, 1f);	
		}, time);
		time += 3.8f;
		GameManager.s.DelayAction(() => { MainCamera.s.locked = false; }, time);

		Player.s.takenDisabledFlag = Player.s.flags[Random.Range(0, Player.s.flags.Count)];
		UIManager.s.OrganizeFlags();
    }
    public void InitLayout(int newWidth, int newHeight, string newFloorType) {
        //destroy previous floor
        for (int i=0; i<width; i++) {
            for (int j=0; j<height; j++) {
                Destroy(tiles[i, j]);
                if (flags[i, j] != null) {
                    Destroy(flags[i, j]);
                }
                Destroy(mines[i, j]);
            }
        }
        foreach (GameObject bt in backgroundTiles) {
            Destroy(bt);
        }

        width = newWidth;
        height = newHeight;

        tiles = new GameObject[width, height];
        flags = new GameObject[width, height];
        mines = new GameObject[width, height];
        backgroundTiles.Clear();

        for (int i=0; i<MaxSize() * MaxSize() * 1.25f; i++) {
            backgroundTiles.Add(Instantiate(GameManager.s.tile_background_p, transform));
        }

		ParticleSystem.ShapeModule sh = ambientDust.shape;
        sh.scale = new Vector3(width*1.5f, height*1.5f, 1);
		ParticleSystem.EmissionModule em = ambientDust.emission;
        em.rateOverTime = width * height / 3f;
		windZone.endRange = MaxSize() * 0.75f;
    }
    public void InitMinefield() {
        InitLayout(2 * floor + 7 + Player.s.modifiers.floorExpansion, 2 * floor + 7 + Player.s.modifiers.floorExpansion, "minefield");
		
		int exitX, exitY;
		if (Random.value < 0.5) {
			exitX = Random.Range(0, width);
			exitY = height-1;
		} else {
			exitX = width-1;
			exitY = Random.Range(0, height);
		}
        PlaceTile(GameManager.s.tile_exit_p, exitX, exitY).GetComponent<ActionTile>().Init(ActionTile.EXITTOSHOP);
		PlaceTile(GameManager.s.tile_p, 0, 0);
        foreach (Vector2Int v in Raincloud.rainCoords) {
            PlaceTile(GameManager.s.tile_puddle_p, v.x, v.y);
        }
		//random chance for trial entrance
		if (Random.value < trialChance) {
			int x = Random.Range(0, width), y = Random.Range(0, height);
			if (tiles[x, y] == null) {
				GameObject t = PlaceTile(GameManager.s.tile_exit_p, x, y);
				t.GetComponent<ActionTile>().Init(ActionTile.EXITTOTRIAL);
			}
		}
        
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (tiles[i, j] == null) {
					if (Random.value < 1f - Player.s.modifiers.noTileChance) {
                    	float tileRand = Random.value;
                    	if (tileRand < 0.05f) {
                    	    PlaceTile(GameManager.s.tile_puddle_p, i, j);
                    	} else if (tileRand < 0.1f) {
                    	    PlaceTile(GameManager.s.tile_mossy_p, i, j);
                    	} else {
                    	    PlaceTile(GameManager.s.tile_p, i, j);
                    	}
					}
                }
				//generate mines
                if (MineAvailableStart(i, j)) {
                    float rand = Random.value;
                    float mineMult = tiles[i, j].GetComponent<Tile>().mineMult * Player.s.modifiers.mineSpawnMult;
                    //if (rand < 0.02f*mineMult) {
                    //    PlaceMine(typeof(Mine), i, j);
                    //} else if (rand == 0.04f*mineMult) {
                    //    PlaceMine(typeof(Mini), i, j);
                    //} else if (rand == 0.06f*mineMult) {
                    //    PlaceMine(typeof(Mouse), i, j);
                    //} else if (rand == 0.08f*mineMult) {
                    //    PlaceMine(typeof(Trap), i, j);
                    //} else if (rand == 0.1f*mineMult) {
                    //    PlaceMine(typeof(Hydra), i, j);
					//}
					if (rand < 1f * mineMult) {
						PlaceMine(typeof(Telemine), i, j);
					}
				}
            }
        }

        onNewMinefield?.Invoke();
    }
    public void InitShop() {
        InitLayout(6, 2, "shop");
        PlaceTile(GameManager.s.tile_exit_p, 5, 0).GetComponent<ActionTile>().Init(ActionTile.EXITTOMINEFIELD);
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (tiles[i, j] == null) {
                    PlaceTile(GameManager.s.tile_p, i, j);
                }
            }
        }

        PlacePickupSprite(Player.s.flagsUnseen, PickupSprite.SHOP, 0, 1, 1);
        PlacePickupSprite(Player.s.flagsUnseen, PickupSprite.SHOP, 15, 2, 1);
        PlacePickupSprite(Player.s.flagsUnseen, PickupSprite.SHOP, 15, 3, 1);
        PlacePickupSprite(Player.s.flagsUnseen, PickupSprite.SHOP, 15, 4, 1);
    }
	public void InitTrial() {
		InitLayout(floor + 7 + Player.s.modifiers.floorExpansion, floor + 7 + Player.s.modifiers.floorExpansion, "trial");
		
		PlaceTile(GameManager.s.tile_exit_p, width-2, height-2).GetComponent<ActionTile>().Init(ActionTile.GIVETRIALREWARD);
		PlaceTile(GameManager.s.tile_exit_p, width-1, height-1).GetComponent<ActionTile>().Init(ActionTile.EXITTOSHOP);
		for (int i = 0; i < width-1; i++) {
			for (int j = 0; j < height-1; j++) {
				if (tiles[i, j] == null) {
					PlaceTile(GameManager.s.tile_p, i, j);
					if (MineAvailableStart(i, j) && Random.value < 0.2 * Player.s.modifiers.mineSpawnMult) {
						PlaceMine(typeof(Mine), i, j);
					}
				}
			}
		}	
	}
    public int MaxSize() {
        return Mathf.Max(width, height);
    }
    public Vector3 CoordToPos(int x, int y) {
        return tiles[x, y].GetComponent<Tile>().referencePos;
    }
    public Vector2Int PosToCoord(Vector3 v) {
        Collider2D[] cs = Physics2D.OverlapPointAll(new Vector2(v.x, v.y));
        foreach (Collider2D c in cs) {
            Tile t = c.gameObject.GetComponent<Tile>();
            if (t != null) {
                return t.coord;
            }
        }
        return new Vector2Int(-1, -1);
    }
    public GameObject PlaceTile(GameObject prefab, int i, int j) {
        tiles[i, j] = Instantiate(prefab, transform);
        tiles[i, j].GetComponent<Tile>().coord = new Vector2Int(i, j);
		return tiles[i, j];
    }
    public void PositionTilesUnbuilt() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
				if (tiles[i, j] != null) {
	                tiles[i, j].GetComponent<Tile>().PositionUnbuilt();
				}
            }
        }
    }
    public void BuildTiles(float totalDuration, float buildDuration) {
        List<Tile> tileList = new List<Tile>();
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
				if (tiles[i, j] != null) {
                	tileList.Add(tiles[i, j].GetComponent<Tile>());
                } 
            }
        }
        GameManager.s.Shuffle(ref tileList);
        for (int i=0; i<tileList.Count; i++) {
            Tile tile = tileList[i];
            GameManager.s.DelayAction(() => { tile.Build(buildDuration); }, totalDuration * i / tileList.Count);
        }
    }
    public void PlaceMine(Type t, int x, int y) {
        GameObject g = Instantiate(GameManager.s.mine_p, tiles[x, y].transform);
		g.transform.localPosition = Vector3.zero;
        Mine m = g.AddComponent(t) as Mine;
        m.init(x, y);
        mines[m.coord.x, m.coord.y] = g;
    }
    public void PlacePickupSprite(List<Type> pool, int spawnType, int p, int x, int y) {
        GameObject g = Instantiate(GameManager.s.flagSprite_p);
        PickupSprite ps = g.AddComponent(typeof(PickupSprite)) as PickupSprite;
        ps.Init(pool[Random.Range(0, pool.Count)], spawnType, p, x, y);
        Player.s.NoticeFlag(ps.parentType);
    }
    public bool within(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    public bool mineAvailable(int x, int y) {
        return tiles[x, y] != null && mines[x, y] == null && (x != Player.s.coord.x || y != Player.s.coord.y) && tiles[x, y].GetComponent<ActionTile>() == null;
    }
	public bool MineAvailableStart(int x, int y) {
		return mineAvailable(x, y) && !(x == 0 && y == 0);
	}
    private void Awake() {
        s = this;
        onFloorChange += FloorChange;
    }
	private void Start() {
		ambientDust = GetComponent<ParticleSystem>();
		windZone = GetComponent<ParticleSystemForceField>();
	}
	private void Update() {
		windDirection = Quaternion.AngleAxis(Mathf.PerlinNoise(Player.s.modifiers.windFluctuation * Time.time, 0) * 1000f, Vector3.forward) * Vector3.right;
		windZone.directionX = windDirection.x * Player.s.modifiers.windStrength;
		windZone.directionY = windDirection.y * Player.s.modifiers.windStrength;
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
