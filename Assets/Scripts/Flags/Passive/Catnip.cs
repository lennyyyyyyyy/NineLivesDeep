using UnityEngine;
using System.Collections.Generic;

public class Catnip : Passive
{
    public override void Modify(ref Modifiers modifiers) {
        List<Vector2Int> addedMoveOptions = new List<Vector2Int>() {
            new Vector2Int(-2, -2),
            new Vector2Int(-2, -1),
            new Vector2Int(-2, 0),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, 2),
            new Vector2Int(-1, 2),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2),
            new Vector2Int(2, 2),
            new Vector2Int(2, 1),
            new Vector2Int(2, 0),
            new Vector2Int(2, -1),
            new Vector2Int(2, -2),
            new Vector2Int(1, -2),
            new Vector2Int(0, -2),
            new Vector2Int(-1, -2)
        };
        modifiers.moveOptions.AddRange(addedMoveOptions);
    }
}
