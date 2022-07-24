using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviourPunCallbacks
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameManager GameManager;
    // Start is called before the first frame update
    private void Start() {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardLogger();
    }

    private void KeyboardLogger() {
        if(Input.GetKeyDown("escape")) {
            setMenuState(!pauseMenu.activeSelf);
        }
    }

    public void setMenuState(bool _state) {
        pauseMenu.SetActive(_state);
        PhotonView.Find(GameManager.getLocalPlayerID()).gameObject.GetComponent<PlayerManager>().pauseGame(_state);
    }

    public void ExitRoomGame() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();
        SceneManager.LoadScene("Lobby");
    }
}
