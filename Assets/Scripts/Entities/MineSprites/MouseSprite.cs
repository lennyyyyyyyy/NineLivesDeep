using UnityEngine;
using System.Collections.Generic;

public class MouseSprite : MineSprite {
    private float cooldown, timer = 0;
    public override void Move(int x, int y, bool reposition = true) {
        base.Move(x, y, reposition);
        cooldown = Random.Range(10f, 20f);
    }
    public virtual void Move() {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i=0; i<4; i++) {
            int dx = new int[]{-1, 1, 0, 0}[i];
            int dy = new int[]{0, 0, -1, 1}[i];
            if (Floor.s.within(coord.x + dx, coord.y + dy) && Floor.s.mineAvailable(coord.x + dx, coord.y + dy)) {
                neighbors.Add(new Vector2Int(coord.x + dx, coord.y + dy));
            }
        }
        if (neighbors.Count > 0) {
            Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
            Move(chosen.x, chosen.y);
            Player.s.triggerMines();
            Player.s.discover(Player.s.coord.x, Player.s.coord.y);
        }
		cooldown = Random.Range(10f, 20f);
    }
    protected void Update() {
        timer += Time.deltaTime;
        if (timer > cooldown) {
            timer = 0;
            Move();
        }
    }
}
