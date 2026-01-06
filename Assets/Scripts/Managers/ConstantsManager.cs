using UnityEngine;

public class ConstantsManager : MonoBehaviour {
    public static ConstantsManager s;

    [System.NonSerialized]
    public float gameStartTransitionDuration = 2f;
    private void Awake() {
        s = this;
    }
}
