using UnityEngine;

public class HydraSprite : MineSprite {
    public override void trigger() {
        int left = 2;
        while (left != 0) {
            int dx = Random.Range(-1, 2);
            int dy = Random.Range(-1, 2);
            if ((dx != 0 || dy != 0) && Floor.s.within(coord.x + dx, coord.y + dy) && Floor.s.mineAvailable(coord.x + dx, coord.y + dy)) {
                left--;
                Floor.s.PlaceMine(typeof(Mine), coord.x + dx, coord.y + dy); 
            }
        }
        base.trigger();
    }
}
