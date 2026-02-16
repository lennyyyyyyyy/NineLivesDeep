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
                 buttonIdleSpeed = 2f,
                 baseMineChance = 0.2f,
                 mineChanceScaling = 0.06f,
                 timeTrialDuration = 60f,
                 flagSpriteDroppedScale = 0.6f;

    [System.NonSerialized]
    public int playerMaxMoveHistory = 10,
               finalFloor = 5,
               curseFreq = 2,
               mineFreq = 2,
               themeFreq = 3;

    [System.NonSerialized]
    public Color cyan = new Color(2f/3, 1, 1, 1),
                 cyanTransparent = new Color(2f/3, 1, 1, 0.7f),
                 blue = new Color(0, 0, 1, 1),
                 darkBlue = new Color(0, 0, 0.5f, 1),
                 green = new Color(0, 1, 0, 1),
                 red = new Color(1, 0, 0, 1),
                 darkRed = new Color(0.5f, 0, 0, 1),
                 black = new Color(0, 0, 0, 1),
                 gray = new Color(0.5f, 0.5f, 0.5f, 1),
                 white = new Color(1, 1, 1, 1);


    private void Awake() {
        s = this;
    }
}
