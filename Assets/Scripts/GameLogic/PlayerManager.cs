using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Camera Camera;
    [SerializeField] GameManager GameManager;
    [SerializeField] DeckManager DeckManager;

    [SerializeField] GameObject Deck;

    [SerializeField] int InitCards;

    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject BlockPrefab;

    [SerializeField] RoundInfo _roundInfo = new RoundInfo(false, false, false, false, -1, -1);

    private void Start() {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void CalculateRayCast() {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.transform.parent.gameObject;

            GameObject _cardToRise = null;

            if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString()) && _roundInfo.isHisTurn && !GameManager.selectingColor()) {
                _cardToRise = objectHit;
            }

            DeckManager.riseCardFromDeck(_cardToRise);

            if (Input.GetMouseButtonDown(0) && PhotonView.Get(this).IsMine && !GameManager.selectingColor()) {
                if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                    if (DeckManager.removeCardFromDeck(objectHit, _roundInfo)) {
                        LocalTurnFinished();
                    } else {
                        //TODO: some wiggle to notify that move is incorrect
                    }
                } else if (objectHit.layer == LayerMask.NameToLayer(Layers.CenterDeck.getString())) {
                    DrawCards(PhotonView.Get(this).ViewID);
                }
            }/*else if (Input.GetMouseButtonDown(1)) {
                DeckManager.removeAllCards();
            }*/
        }
    }

    public void DrawCards(int playerID) {
        var cards = GameManager.drawCards(_roundInfo);

        foreach(var card in cards) {
            PhotonView.Get(this).RPC("SpawnCard", RpcTarget.AllViaServer, card.first, card.second, playerID);
        }

        if (_roundInfo.hasToDraw && PhotonView.Get(this).IsMine) {
            LocalTurnFinished();
        }
    }

    [PunRPC]
    public void SpawnCard(CardType a, CardColor b, int playerID) {
        if (PhotonView.Get(this).ViewID == playerID) {
            GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DeckManager.addNewCardToDeck(newCard, new Pair<CardType, CardColor>(a,b));

            if (!PhotonView.Get(this).IsMine) {
                newCard.layer = (int)Layers.IgnoreRayCast;
            }
        }
    }

    private void SpawnBlock() {
        GameObject block = Instantiate(BlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        block.transform.SetParent(transform);
        block.transform.localPosition = new Vector3(0.0f,2.0f,-6.2f);
        //Change color material and program autodelete
    }

    [PunRPC]
    public void turnFinished(bool state, int playerID) {
        Debug.Log(playerID + " --> " + PhotonView.Get(this).ViewID);
        if (PhotonView.Get(this).ViewID == playerID) {
            _roundInfo.isHisTurn = state;
        }
    }

    [PunRPC]
    public void turnBlocked(bool state, int playerID) {
        if (PhotonView.Get(this).ViewID == playerID) {
            _roundInfo.isHisTurn = state;
        }
    }

    private void LocalTurnFinished() {
        PhotonView.Get(this).RPC("turnFinished", RpcTarget.AllViaServer, false, Convert.ToInt32(_roundInfo.playerID));
        GameManager.GetComponent<PhotonView>().RPC("finishedTurn", RpcTarget.AllViaServer);
        //_roundInfo.isHisTurn = false;
        //GameManager.finishedTurn();
    }

    [PunRPC]
    public void giveTurn(RoundInfo roundInfo) {
        //Debug.Log("hello there");
        //Debug.Log("-->" + PhotonView.Get(this).ViewID + " - "+ roundInfo.playerID + " - " + roundInfo.isHisTurn);
        if (PhotonView.Get(this).ViewID == roundInfo.playerID) {
            _roundInfo = roundInfo;

            if (roundInfo.isBlocked) {
                //show block 
                SpawnBlock();

                if (PhotonView.Get(this).IsMine) {
                    PhotonView.Get(this).RPC("turnBlocked", RpcTarget.AllViaServer, false, Convert.ToInt32(_roundInfo.playerID));
                    LocalTurnFinished();
                }
            }
        }
    }

    [PunRPC]
    public void initPlayer(int playerID, int rot) {
        if (PhotonView.Get(this).ViewID == playerID && PhotonView.Get(this).IsMine) {
            GameManager.RotateWithdrawCounter(rot);
            _roundInfo.playerRotation = Convert.ToInt16(rot);
            _roundInfo.playerID = Convert.ToInt16(playerID);
            for (int i = 0; i < InitCards; ++i) {
                DrawCards(playerID);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_roundInfo.isHisTurn && PhotonView.Get(this).IsMine) {
            CalculateRayCast();
        }
    }
}
