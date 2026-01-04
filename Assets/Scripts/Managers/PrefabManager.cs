using UnityEngine;

/*
 * The PrefabManager holds references to all prefabs used in the game.
 */
public class PrefabManager : MonoBehaviour {
    public static PrefabManager s;

    // prefabs
    [System.NonSerialized]
    public GameObject tilePrefab,
		  		      tileActionPrefab,
					  tilePuddlePrefab,
                      tileMossyPrefab,
                      tileBackgroundPrefab,
                      mossPrefab,
                      grass1Prefab,
                      grass2Prefab,
                      minePrefab,
                      mineUIItemPrefab,
                      mineSpritePrefab,
                      numberPrefab,
                      printPrefab,
                      bloodPrefab,
                      flagPrefab,
                      flagSpritePrefab,
                      cursePrefab,
                      pawPrefab,
                      tooltipPrefab,
                      bubblePrefab,
                      psychicEyePrefab,
					  playerBitPrefab,
					  crankPrefab,
                      pillarPrefab,
                      vasePrefab,
                      tunnelPrefab,
                      runPrefab;

    private void Awake() {
        s = this;

        tilePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/Tile");
        tileActionPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/ActionTile");
        tilePuddlePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/Puddle");
        tileMossyPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/MossyTile");
        tileBackgroundPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/BackgroundTile");
        mossPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/MossyTileComponents/Moss");
        grass1Prefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/MossyTileComponents/Grass1");
        grass2Prefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Tiles/MossyTileComponents/Grass2");
        flagPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/UIItems/Flag");
        cursePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/UIItems/Curse");
        minePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/UIItems/Mine");
        mineUIItemPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/UIItems/MineUIItem");
        mineSpritePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MainEntities/MineSprite");
        flagSpritePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MainEntities/FlagSprite");
        pawPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/UIComponents/Paw");
        tooltipPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/UIComponents/Tooltip");
        bubblePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/UIComponents/Bubble");
        crankPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MiscEntities/Crank");
        pillarPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MiscEntities/Pillar");
        vasePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MiscEntities/Vase");
        tunnelPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MiscEntities/Tunnel");
        psychicEyePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/PsychicEye");
        playerBitPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/PlayerBit");
        numberPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Number");
        printPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Print");
        bloodPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Blood");
        runPrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/Run");
    }
}
