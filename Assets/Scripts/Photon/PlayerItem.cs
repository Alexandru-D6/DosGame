using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playerName;

    [SerializeField] Image backgroundImage;
    [SerializeField] Color highlightColor;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    [SerializeField] Image colorButton;

    Player player;
    private void Awake() {
        colorButton.gameObject.GetComponent<Button>().interactable = false;
    }

    public void SetPlayerInfo(Player _player) {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(_player);
    }

    public void ApplyLocalChanges() {
        backgroundImage.color = highlightColor;
        colorButton.gameObject.GetComponent<Button>().interactable = true;
    }

    public void OnClickRandomColor() {
        playerProperties["buttonColor"] = new Vector3(Random.Range(0,255), Random.Range(0, 255), Random.Range(0, 255));
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (player == targetPlayer) {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player player) {
        if (player.CustomProperties.ContainsKey("buttonColor")) {
            Vector3 color = (Vector3)player.CustomProperties["buttonColor"];
            colorButton.color = new Color(color[0] / 255.0f, color[1] / 255.0f, color[2] / 255.0f, 1.0f);
            playerProperties["buttonColor"] = player.CustomProperties["buttonColor"];
        } else {
            playerProperties["buttonColor"] = new Vector3(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255));
        }
    }
}
