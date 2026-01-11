using UnityEngine;

public class Run : MonoBehaviour {
    public static Run s;

    private void Awake() {
        s = this;
    }
    private void OnDestroy() {
        s = null;
    }
}

