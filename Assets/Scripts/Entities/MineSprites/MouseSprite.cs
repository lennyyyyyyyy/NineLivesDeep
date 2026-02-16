using UnityEngine;
using System.Collections.Generic;

public class MouseSprite : MineSprite {
    private float cooldown, timer = 0;
    public override bool Move(int x, int y, bool reposition = true, bool rescale = true) {
        cooldown = Random.Range(10f, 20f);
        return base.Move(x, y, reposition, rescale);
    }
    private void Move() {
		cooldown = Random.Range(10f, 20f);
        if (!PassiveActive()) return;
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i=0; i<4; i++) {
            int dx = new int[]{-1, 1, 0, 0}[i];
            int dy = new int[]{0, 0, -1, 1}[i];
			if (CoordAllowed(GetCoord().x + dx, GetCoord().y + dy)) {
                neighbors.Add(new Vector2Int(GetCoord().x + dx, GetCoord().y + dy));
            }
        }
        if (neighbors.Count > 0) {
            Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
            Move(chosen.x, chosen.y);
        }
    }
    protected void Update() {
        timer += Time.deltaTime;
        if (timer > cooldown) {
            timer = 0;
            Move();
        }
    }
}
