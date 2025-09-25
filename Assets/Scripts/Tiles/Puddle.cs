using UnityEngine;

public class Puddle : Tile
{
    public GameObject puddle;
    protected override void Start() {
        base.Start();
        mineMult = 0.5f;
    }
}
