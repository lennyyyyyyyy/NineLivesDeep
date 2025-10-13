using UnityEngine;

public class MineSprite : Entity {
    public Vector2Int coord;
    public bool detectable = true;
	protected override void Start() {
		base.Start();
		obstacle = false;
	}
    public virtual void Trigger() {
        Player.s.Die();
		Remove();
    }
	public override void Move(GameObject tile, bool reposition = true) {
		base.Move(tile, reposition);
	}
	public override void Remove() {
		base.Remove();
	}
    public override bool CoordAllowed(int x, int y) { 
        return base.CoordAllowed(x, y); 
    }
}
