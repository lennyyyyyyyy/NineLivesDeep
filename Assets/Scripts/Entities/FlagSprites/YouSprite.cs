using UnityEngine;

public class YouSprite : FlagSprite
{
    protected override void Start() {
        base.Start();
		removesMines = false;
    }
    public override bool CoordAllowed(int x, int y) { 
        return Floor.s.GetUniqueFlag(x, y) == null && Mathf.Abs(x - Player.s.coord.x) <= Player.s.modifiers.reviveRange && Mathf.Abs(y - Player.s.coord.y) <= Player.s.modifiers.reviveRange; 
    }
    protected override void OnPlace() {
        base.OnPlace();
		Player.s.Move(coord.x, coord.y);
        Player.s.Revive();
    }
}
