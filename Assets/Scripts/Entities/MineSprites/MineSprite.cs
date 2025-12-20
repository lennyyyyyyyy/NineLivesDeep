using UnityEngine;

public class MineSprite : Entity {
	protected override void Start() {
		base.Start();
	}
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
	protected override void SetDefaultData() {
		SetData(sprite: UIManager.s.mineDebugSprite, obstacle: false);
	}
    public virtual void Trigger() {
        Floor.onExplosionAtCoord?.Invoke(GetCoord().x, GetCoord().y);
        Player.s.Die();
		Remove();
    }
	public override void Move(GameObject tile, bool reposition = true) {
		base.Move(tile, reposition);
		Player.s.triggerMines();
		Player.s.discoverTiles();
	}
	public override void Remove() {
		base.Remove();
	}
    public override bool CoordAllowed(int x, int y) { 
        return base.CoordAllowed(x, y); 
    }
}
