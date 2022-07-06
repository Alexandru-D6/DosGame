using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_Text buttonText;

    public void OnClickConnect() {
        if (usernameInput.text.Length >= 3) {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting..";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();
        SceneManager.LoadScene("Lobby");
    }

}
