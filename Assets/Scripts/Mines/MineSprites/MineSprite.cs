using UnityEngine;

public class MineSprite : MonoBehaviour
{
    public Vector2Int coord;
    public bool detectable = true;
    public void move(int x, int y) {
        remove();
        Floor.s.mines[x, y] = gameObject;
        coord = new Vector2Int(x, y);
        transform.position = Floor.s.CoordToPos(x, y);
    }
    public void remove() {
        Floor.s.mines[coord.x, coord.y] = null;
    }
    public virtual void init(int x, int y) {
        coord = new Vector2Int(x, y);
    }
    public virtual void trigger() {
        Player.s.Die();
        remove();
        Destroy(gameObject);
    }
}
