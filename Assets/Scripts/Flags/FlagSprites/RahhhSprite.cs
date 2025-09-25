using UnityEngine;

public class RahhhSprite : FlagSprite
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnPlace() {
        base.OnPlace();
        for (int dx=-1; dx<=1; dx++) {
            for (int dy=-1; dy<=1; dy++) {
                if (Floor.s.within(coord.x + dx, coord.y + dy)) {
                    Player.s.discover(coord.x + dx, coord.y + dy);
                }
            }
        }
    }
}
