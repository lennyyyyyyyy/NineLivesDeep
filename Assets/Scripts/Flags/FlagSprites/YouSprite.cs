using UnityEngine;

public class YouSprite : FlagSprite
{
    protected override void Start()
    {
        base.Start();
    }
    protected override bool CoordAllowed(int x, int y) { 
        return x == Player.s.coord.x && y == Player.s.coord.y; 
    }
    protected override void OnPlace() {
        base.OnPlace();
        Player.s.Revive();
    }
}