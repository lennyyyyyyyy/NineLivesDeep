using UnityEngine;

/*
 * Assign this component to any object that uses dynamic, non-global shader graph properties.
 */
public class ShaderGraph : MonoBehaviour {
    public SpriteRenderer sr;
    protected MaterialPropertyBlock mpb;

    protected virtual void Awake() {
        mpb = new MaterialPropertyBlock();
        sr = GetComponent<SpriteRenderer>();
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(ShaderManager.s.SeedID, Random.value);
        sr.SetPropertyBlock(mpb);
    }
    protected virtual void LateUpdate() {
        sr.GetPropertyBlock(mpb);
        mpb.SetVector(ShaderManager.s.WorldPosID, transform.position);
        mpb.SetVector(ShaderManager.s.WorldSizeID, sr.bounds.size);
        sr.SetPropertyBlock(mpb);
    }
}
