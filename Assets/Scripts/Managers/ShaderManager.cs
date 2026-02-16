using UnityEngine;

/*
 * The ShaderManager manages global shader properties.
 */
public class ShaderManager : MonoBehaviour {
    public static ShaderManager s;

    private GameObject underDarkenTarget;
    private Matrix4x4 disturbancePositions1 = Matrix4x4.zero, disturbancePositions2 = Matrix4x4.zero;
    private Vector4[] disturbancePositions = new Vector4[8];
    private Vector4 disturbanceTimes1 = -1 * Vector4.one, disturbanceTimes2 = -1 * Vector4.one;
    private float[] disturbanceTimes = new float[8];
    public readonly int LayerID = Shader.PropertyToID("_Layer"),
                        UnderDarkenID = Shader.PropertyToID("_UnderDarken"),
                        DPos1ID = Shader.PropertyToID("_DisturbancePositions1"),
                        DPos2ID = Shader.PropertyToID("_DisturbancePositions2"),
                        DTimes1ID = Shader.PropertyToID("_DisturbanceTimes1"),
                        DTimes2ID = Shader.PropertyToID("_DisturbanceTimes2"),
                        WorldPosID = Shader.PropertyToID("_WorldPosition"), 
                        WorldSizeID = Shader.PropertyToID("_WorldSize"),
                        SeedID = Shader.PropertyToID("_Seed"),
                        ThemeID = Shader.PropertyToID("_Theme"),
                        EdgeColorID = Shader.PropertyToID("_EdgeColor");
    private void Awake() {
        s = this;
        for (int i=0; i<4; i++) {
            disturbancePositions1.SetRow(i, 10000 * Vector4.one);
            disturbancePositions2.SetRow(i, 10000 * Vector4.one);
        }
    }
    private void Start() {
        Shader.SetGlobalFloat(LayerID, 0);
        Shader.SetGlobalFloat(UnderDarkenID, 1);
        underDarkenTarget = new GameObject("UnderDarkenTarget");
        underDarkenTarget.transform.parent = this.transform;
    }
    public void DisturbShaders(float x, float y) {
        if (Time.time > Mathf.Max(disturbanceTimes[7] + 1, disturbanceTimes[0] + 0.125f)) {
            for (int i=7; i>0; i--) {
                disturbancePositions[i] = disturbancePositions[i-1];
                disturbanceTimes[i] = disturbanceTimes[i-1];
            }
            disturbancePositions[0] = new Vector4(x, y, 0, 0);
            disturbanceTimes[0] = Time.time;
            for (int i=0; i<4; i++) {
                disturbancePositions1.SetRow(i, disturbancePositions[i]);
                disturbanceTimes1[i] = disturbanceTimes[i];
                disturbancePositions2.SetRow(i, disturbancePositions[i+4]);
                disturbanceTimes2[i] = disturbanceTimes[i+4];
            }
            Shader.SetGlobalMatrix(DPos1ID, disturbancePositions1);
            Shader.SetGlobalVector(DTimes1ID, disturbanceTimes1);
            Shader.SetGlobalMatrix(DPos2ID, disturbancePositions2);
            Shader.SetGlobalVector(DTimes2ID, disturbanceTimes2);
        }
    }
    public void TweenUnderDarken(float brightness, float duration) {
        LeanTween.cancel(underDarkenTarget);
        LeanTween.value(underDarkenTarget, (float f) => { Shader.SetGlobalFloat(UnderDarkenID, f); }, Shader.GetGlobalFloat(UnderDarkenID), brightness, duration).setEase(LeanTweenType.easeInOutCubic);
    }
    private void SetGlobalTheme() {
        Shader.SetGlobalFloat(ThemeID, Floor.s.floor / 3);
    }
    private void OnEnable() {
        EventManager.s.OnFloorIntroEnd += SetGlobalTheme;
    }
    private void OnDisable() {
        EventManager.s.OnFloorIntroEnd -= SetGlobalTheme;
    }
}
