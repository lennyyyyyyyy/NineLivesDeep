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
	public override void Move(int x, int y, bool reposition = true) {
		base.Move(x, y, reposition);
		if (coord.x != -1) {	
			Floor.s.mines[coord.x, coord.y] = null;
		}
		Floor.s.mines[x, y] = gameObject;
	}
	public override void Remove() {
		base.Remove();
		if (coord.x != -1) {
			Floor.s.mines[coord.x, coord.y] = null;
		}
	}
    public override bool CoordAllowed(int x, int y) { 
        return base.CoordAllowed(x, y); 
    }
}
