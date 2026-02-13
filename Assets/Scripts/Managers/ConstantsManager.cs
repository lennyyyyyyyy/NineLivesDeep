using UnityEngine;

public class ConstantsManager : MonoBehaviour {
    public static ConstantsManager s;

    [System.NonSerialized]
    public float gameUITransitionDuration = 2f,
                 tileAdjacentDragSpeed = 0.50f,
                 tileAdjacentDragPower = 20.0f,
                 tileDampingPower = 0.20f,
                 playerStepImpulse = 0.20f,
                 playerReviveDuration = 3.0f,
	             minefieldTrialChance = 1f,
                 tooltipPadding = 0.01f,
                 buttonIdleStrength = 15f,
                 buttonIdleSpeed = 2f;
    [System.NonSerialized]
    public int playerMaxMoveHistory = 10,
               finalFloor = 0;

    private void Awake() {
        s = this;
    }
}
