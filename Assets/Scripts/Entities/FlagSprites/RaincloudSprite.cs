using UnityEngine;

class RaincloudSprite : FlagSprite {
    protected override void OnPlace() {
        base.OnPlace();
        Raincloud.rainCoords.Add(coord);
    }
}