using UnityEngine;

public class YouSprite : FlagSprite
{
    public override bool CoordAllowed(int x, int y) { 
        return Floor.s.GetUniqueFlag(x, y) == null && Mathf.Abs(x - Player.s.GetCoord().x) <= Player.s.modifiers.reviveRange && Mathf.Abs(y - Player.s.GetCoord().y) <= Player.s.modifiers.reviveRange; 
    }
    protected override void OnPlace() {
        base.OnPlace();
		Player.s.Move(GetCoord().x, GetCoord().y);
        Player.s.Revive();
    }
}
