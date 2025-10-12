using UnityEngine;

public class YouSprite : FlagSprite
{
    protected override void Start() {
        base.Start();
		removesMines = false;
    }
    public override bool CoordAllowed(int x, int y) { 
        return Mathf.Abs(x - Player.s.coord.x) <= Player.s.modifiers.reviveRange && Mathf.Abs(y - Player.s.coord.y) <= Player.s.modifiers.reviveRange; 
    }
    protected override void OnPlace() {
        base.OnPlace();
		Player.s.setCoord(coord.x, coord.y);
        Player.s.Revive();
    }
}
