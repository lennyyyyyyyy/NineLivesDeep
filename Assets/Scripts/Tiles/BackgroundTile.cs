using UnityEngine;

public class BackgroundTile : Parallax {
    private Vector4 seed;
    private float speed = 0.065f;
    protected override void Start() {
        base.Start();
        seed = new Vector4(Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f));
        Sprite s;
        float r = Random.Range(0f, 1f);
        if (r < .8f) {
            s = UIManager.s.tile_s;
        } else if (r < .9f) {
            s = UIManager.s.tile_wood_s;
        } else {
            s = UIManager.s.tile_brick_s;
        }
        GetComponent<SpriteRenderer>().sprite = s;
        transform.eulerAngles = Random.Range(0,4) * Vector3.forward * 90;
    }
    protected override void Update() {
        base.Update();
        referencePos = 5.5f * Floor.s.MaxSize() * new Vector3(Mathf.PerlinNoise(seed.x, Time.time * speed) - 0.5f, Mathf.PerlinNoise(seed.y, Time.time * speed) - 0.5f, 0);
        depth = 1.55f + 0.5f * Mathf.PerlinNoise(seed.z, Time.time * speed);
        transform.localEulerAngles = new Vector3(0, 0, 360*3*Mathf.PerlinNoise(seed.z, Time.time*speed/3));
    }
}

    