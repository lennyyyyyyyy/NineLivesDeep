using UnityEngine;

class NormalTile : Tile {
    public GameObject tileTexture;
    protected virtual void Awake() {
        Sprite s;
        float r = Random.Range(0f, 1f);
        if (r < .8f) {
            s = UIManager.s.tile_s;
        } else if (r < .9f) {
            s = UIManager.s.tile_wood_s;
        } else {
            s = UIManager.s.tile_brick_s;
        }
        tileTexture.GetComponent<SpriteRenderer>().sprite = s;
        tileTexture.transform.eulerAngles = Random.Range(0,4) * Vector3.forward * 90;
        tileTexture.transform.localScale = Vector3.one - (Random.Range(0, 2) * 2 * Vector3.right);
    }

}