using UnityEngine;

/*
 * The PrefabManager holds references to all prefabs used in the game.
 */
public class PrefabManager : MonoBehaviour {
    public static PrefabManager s;

    [System.NonSerialized]
    public GameObject tilePrefab,
		  		      tileActionPrefab,
					  tilePuddlePrefab,
                      tileMossyPrefab,
                      tileBackgroundPrefab,
                      mossPrefab,
                      grass1Prefab,
                      grass2Prefab,
                      flagPrefab,
                      cursePrefab,
                      minePrefab,
                      mineUIItemPrefab,
                      flagSpritePrefab,
                      mineSpritePrefab,
                      pawPrefab,
                      tooltipPrefab,
                      bubblePrefab,
					  crankPrefab,
                      pillarPrefab,
                      vasePrefab,
                      tunnelPrefab,
                      psychicEyePrefab,
					  playerBitPrefab,
                      numberPrefab,
                      printPrefab,
                      bloodPrefab,
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
        flagSpritePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MainEntities/FlagSprite");
        mineSpritePrefab = HelperManager.s.LoadResourceSafe<GameObject>("Prefabs/MainEntities/MineSprite");
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
