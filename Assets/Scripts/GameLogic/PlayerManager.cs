using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] new Camera camera;
    [SerializeField] GameManager gameManager;
    [SerializeField] DeckManager deckManager;

    [SerializeField] GameObject deck;

    [SerializeField] int initCards;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject blockPrefab;

    [SerializeField] RoundInfo roundInfo = new RoundInfo(false, false, false, false, -1, -1);
    [SerializeField] bool gamePaused = false;

    private void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void CalculateRayCast() {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.transform.parent.gameObject;

            GameObject _cardToRise = null;

            if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString()) && roundInfo.isHisTurn && !gameManager.selectingColor()) {
                _cardToRise = objectHit;
            }

            deckManager.riseCardFromDeck(_cardToRise);

            if (Input.GetMouseButtonDown(0) && PhotonView.Get(this).IsMine && !gameManager.selectingColor()) {
                if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                    if (deckManager.removeCardFromDeck(objectHit, roundInfo)) {
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

    public int deckSize() {
        return deck.GetComponent<DeckManager>().deckSize();
    }

    public void DrawCards(int playerID) {
        var cards = gameManager.drawCards(roundInfo);

        foreach(var card in cards) {
            PhotonView.Get(this).RPC("SpawnCard", RpcTarget.AllViaServer, card.first, card.second, playerID);
        }

        if (roundInfo.hasToDraw && PhotonView.Get(this).IsMine) {
            LocalTurnFinished();
        }
    }

    [PunRPC]
    public void SpawnCard(CardType a, CardColor b, int playerID) {
        if (PhotonView.Get(this).ViewID == playerID) {
            GameObject newCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            deckManager.addNewCardToDeck(newCard, new Pair<CardType, CardColor>(a,b));

            if (!PhotonView.Get(this).IsMine) {
                newCard.layer = (int)Layers.IgnoreRayCast;
            }
        }
    }

    private void SpawnBlock() {
        GameObject block = Instantiate(blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        block.transform.SetParent(transform);
        block.transform.localPosition = new Vector3(0.0f,2.0f,-6.2f);
        //Change color material and program autodelete
    }

    [PunRPC]
    public void turnFinished(bool state, int playerID) {
        Debug.Log(playerID + " --> " + PhotonView.Get(this).ViewID);
        if (PhotonView.Get(this).ViewID == playerID) {
            roundInfo.isHisTurn = state;
        }
    }

    [PunRPC]
    public void turnBlocked(bool state, int playerID) {
        if (PhotonView.Get(this).ViewID == playerID) {
            roundInfo.isHisTurn = state;
        }
    }

    private void LocalTurnFinished() {
        PhotonView.Get(this).RPC("turnFinished", RpcTarget.AllViaServer, false, Convert.ToInt32(roundInfo.playerID));
        gameManager.GetComponent<PhotonView>().RPC("finishedTurn", RpcTarget.AllViaServer);
        //_roundInfo.isHisTurn = false;
        //GameManager.finishedTurn();
    }

    [PunRPC]
    public void giveTurn(RoundInfo _roundInfo) {
        //Debug.Log("hello there");
        //Debug.Log("-->" + PhotonView.Get(this).ViewID + " - "+ roundInfo.playerID + " - " + roundInfo.isHisTurn);
        if (PhotonView.Get(this).ViewID == _roundInfo.playerID) {
            roundInfo = _roundInfo;

            if (_roundInfo.isBlocked) {
                //show block 
                SpawnBlock();

                if (PhotonView.Get(this).IsMine) {
                    PhotonView.Get(this).RPC("turnBlocked", RpcTarget.AllViaServer, false, Convert.ToInt32(roundInfo.playerID));
                    LocalTurnFinished();
                }
            }
        }
    }

    [PunRPC]
    public void initPlayer(int playerID, int rot) {
        if (PhotonView.Get(this).ViewID == playerID && PhotonView.Get(this).IsMine) {
            gameManager.RotateMiddle(rot);
            roundInfo.playerRotation = Convert.ToInt16(rot);
            roundInfo.playerID = Convert.ToInt16(playerID);
            for (int i = 0; i < initCards; ++i) {
                DrawCards(playerID);
            }
        }
    }

    public void pauseGame(bool state) {
        gamePaused = state;
    }

    // Update is called once per frame
    private void Update()
    {
        if (roundInfo.isHisTurn && PhotonView.Get(this).IsMine && !gamePaused) {
            CalculateRayCast();
        }
    }
}
