using UnityEngine;

public class ShaderGraph : MonoBehaviour
{
    protected MaterialPropertyBlock mpb;
    protected SpriteRenderer sr;
    protected static readonly int WorldPosID = Shader.PropertyToID("_WorldPosition"), 
                                WorldSizeID = Shader.PropertyToID("_WorldSize"),
                                SeedID = Shader.PropertyToID("_Seed");

    protected virtual void Awake()
    {
        mpb = new MaterialPropertyBlock();
        sr = GetComponent<SpriteRenderer>();
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(SeedID, Random.value);
        sr.SetPropertyBlock(mpb);
    }

    protected virtual void LateUpdate()
    {
        sr.GetPropertyBlock(mpb);
        mpb.SetVector(WorldPosID, transform.position);
        mpb.SetVector(WorldSizeID, sr.bounds.size);
        sr.SetPropertyBlock(mpb);
    }
}
