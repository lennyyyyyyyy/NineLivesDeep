using UnityEngine;

public class HydraSprite : MineSprite {
    public override void Trigger() {
        int left = 2;
        while (left != 0) {
            int dx = Random.Range(-1, 2);
            int dy = Random.Range(-1, 2);
            if (dx != 0 || dy != 0) {
                left--;
                Floor.s.PlaceMine(typeof(Mine), GetCoord().x + dx, GetCoord().y + dy); 
            }
        }
        base.Trigger();
    }
}
