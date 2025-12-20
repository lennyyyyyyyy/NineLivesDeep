using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Tunnel : Entity {
    private static List<Tunnel> tunnels = new List<Tunnel>();
    private static bool playerTeleportedLast = false;

    private void Awake() {
        tunnels.Add(this);
    }
    private void OnPlayerMove(int x, int y) {
        if (GetCoord().x == x && GetCoord().y == y && tunnels.Count > 1) {
            if (playerTeleportedLast) {
                playerTeleportedLast = false;
            } else {
                List<Tunnel> possibleTunnels = tunnels.Where(t => t != this).ToList();
                Vector2Int newCoord = possibleTunnels[Random.Range(0, possibleTunnels.Count)].GetCoord();
                playerTeleportedLast = true;
                Player.s.Move(newCoord.x, newCoord.y, animate: false);
            }
        }
    }
    private void OnEnable() {
        Player.OnMove += OnPlayerMove;
    }
    private void OnDisable() {
        Player.OnMove -= OnPlayerMove;
    }
    private void OnDestroy() {
        tunnels.Remove(this);
    }
}
