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
	public override bool Move(GameObject tile, bool reposition = true) {
        bool success = base.Move(tile, reposition);
		if (success) {
            Player.s.TriggerMines();
            Player.s.discoverTiles();
        }
        return success;
	}
    public override bool CoordAllowed(int x, int y) {
        return base.CoordAllowed(x, y) && 
               Floor.s.GetTile(x, y).GetComponent<Tile>().uniqueMine != gameObject;
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
