using UnityEngine;

public class Brain : Map {
    public override void OnDiscover(int x, int y) {
        int count = 0;
        for (int dx=-1; dx<=1; dx++) {
            for (int dy=-1; dy<=1; dy++) {
                if (Floor.s.TileExistsAt(x + dx, y + dy)) {
					count += (Floor.s.GetUniqueMine(x+dx, y+dy)!=null)?1:0;
                }
            }
        }
		SetNumber(x, y, count);
    }
}
