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
				Player.s.discover(GetCoord().x + dx, GetCoord().y + dy);
            }
        }
    }
}
