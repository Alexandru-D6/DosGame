using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] PhotonView gameManager;

    private void Start() {
        int rot = 0;
        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
            if (player.Value == PhotonNetwork.LocalPlayer) {
                rot = player.Key-1;
            }
        }

        //TODO: adapting the rotation and table to be able to place the exact count of players (if they are 3 create a triangular table??) 
        rot *= -90;
        GameObject newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0,0,0), Quaternion.Euler(0,rot,0));

        gameManager.RPC("PlayerCreated", RpcTarget.AllViaServer, newPlayer.GetComponent<PhotonView>().ViewID, rot);
    }

}
