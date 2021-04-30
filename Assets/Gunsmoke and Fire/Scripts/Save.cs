using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
    private Player player;

    public Player getPlayer() {
        return player;
    }

    public void setPlayer(Player set) {
        player = set;
    }

}
