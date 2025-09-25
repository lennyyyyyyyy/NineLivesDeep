using UnityEngine;

public class Aromatic : Passive
{
    protected override void Start()
    {
        Player.s.discoverRange++;
        base.Start();
    }
}
