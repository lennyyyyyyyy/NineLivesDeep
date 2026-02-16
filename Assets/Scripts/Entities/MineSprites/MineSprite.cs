using UnityEngine;

public class MineSprite : Entity {
	protected override void Update() {
		base.Update();
		// watched mine jumping on player
		Vector2Int playerCoord = Player.s.GetCoord();
		Vector2Int mineCoord = GetCoord();
		if (Player.s.alive &&
			Player.s.modifiers.watched &&
			Player.s.watchedMineJumpTimer > Player.s.modifiers.watchedMineJumpTime &&
			Mathf.Abs(playerCoord.x - mineCoord.x) <= 1 &&
			Mathf.Abs(playerCoord.y - mineCoord.y) <= 1 &&
			Random.value < 1 - Mathf.Pow(1 - Player.s.modifiers.watchedMineJumpChancePerSecond, Time.deltaTime)) {
			Move(Player.s.GetTile());
		}
			
	}	
	public override void Init() {
		Init(sprite: UIManager.s.mineDebugSprite, obstacle: false);
	}
    public virtual void Trigger() {
        EventManager.s.OnExplosionAtCoord?.Invoke(GetCoord().x, GetCoord().y, gameObject);
		Remove();
    }
	public override bool Move(GameObject tile, bool reposition = true, bool rescale = true) {
		if (base.Move(tile, reposition, rescale)) {
            HelperManager.s.DelayActionFrames(() => {
                Player.s.TriggerMines();
                Player.s.discoverTiles();
            }, 1);
            return true;
        }
        return false;
	}
    public override bool CoordAllowed(int x, int y) {
        return base.CoordAllowed(x, y) && 
               Floor.s.GetTile(x, y).GetComponent<Tile>().uniqueMine != gameObject;
    }
    public virtual bool PassiveActive() {
        Vector2Int coord = GetCoord();
        for (int x = coord.x - 1; x <= coord.x + 1; x++) {
            for (int y = coord.y - 1; y <= coord.y + 1; y++) {
                if (Floor.s.TileExistsAt(x, y) && Floor.s.GetTile(x, y).GetComponent<Tile>().HasEntityOfType<StopSprite>()) {
                    return false;
                }
            }
        }
        return true;
    }
    protected virtual void OnExplosionAtCoord(int x, int y, GameObject source) {
        Vector2Int coord = GetCoord();
        if (coord.x == x && coord.y == y && source != gameObject) {
            Trigger();
        }
    }
    protected virtual void OnEnable() {
        EventManager.s.OnExplosionAtCoord += OnExplosionAtCoord;
    }
    protected virtual void OnDisable() {
        EventManager.s.OnExplosionAtCoord -= OnExplosionAtCoord;
    }
}
