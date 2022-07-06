using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    private void Start() {
        int rot = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
            if (player.Value == PhotonNetwork.LocalPlayer) {
                rot = player.Key-1;
            }
        }
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,0,0), Quaternion.Euler(0,-90*rot,0));
    }

}
