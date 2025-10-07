using UnityEngine;

public class BackgroundTile : Parallax {
    private Vector4 seed;
    private float speed = 0.065f;
	public SpriteRenderer sr;

	protected static readonly int ThemeID = Shader.PropertyToID("_Theme");

    protected override void Start() {
        base.Start();
        seed = new Vector4(Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f));

		//put the correct theme for the floor
		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		sr.GetPropertyBlock(mpb);
		mpb.SetFloat(ThemeID, Floor.s.floor/3 + 1);
		sr.SetPropertyBlock(mpb);

        transform.eulerAngles = Random.Range(0,4) * Vector3.forward * 90;
    }
    protected override void Update() {
        base.Update();
        referencePos = 5.5f * Floor.s.MaxSize() * new Vector3(Mathf.PerlinNoise(seed.x, Time.time * speed) - 0.5f, Mathf.PerlinNoise(seed.y, Time.time * speed) - 0.5f, 0);
        depth = 1.55f + 0.5f * Mathf.PerlinNoise(seed.z, Time.time * speed);
        transform.localEulerAngles = new Vector3(0, 0, 360*3*Mathf.PerlinNoise(seed.z, Time.time*speed/3));
    }
}

    
