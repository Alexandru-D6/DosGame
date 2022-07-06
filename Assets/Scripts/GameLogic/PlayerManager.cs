using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Camera Camera;
    [SerializeField] GameManager GameManager;
    [SerializeField] DeckManager DeckManager;

    [SerializeField] GameObject Deck;

    [SerializeField] int InitCards;

    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject BlockPrefab;

    private RoundInfo _roundInfo = new RoundInfo(false, false, false, false, -1);

    private void Start() {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void calculateRayCast() {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.transform.parent.gameObject;

            GameObject _cardToRise = null;

            if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString()) && _roundInfo.isHisTurn) {
                _cardToRise = objectHit;
            }

            DeckManager.riseCardFromDeck(_cardToRise);

            if (Input.GetMouseButtonDown(0)) {
                if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                    if (DeckManager.removeCardFromDeck(objectHit, _roundInfo) && !GameManager.selectingColor()) {
                        GameManager.GetComponent<PhotonView>().RPC("finishedTurn", RpcTarget.AllViaServer);//GameManager.finishedTurn();
                    } else {
                        //TODO: some wiggle to notify that move is incorrect
                    }
                } else if (objectHit.layer == LayerMask.NameToLayer(Layers.CenterDeck.getString())) {
                    drawCards(PhotonView.Get(this).ViewID);
                }
            }else if (Input.GetMouseButtonDown(1)) {
                DeckManager.removeAllCards();
            }
        }
    }

    public void drawCards(int playerID) {
        var cards = GameManager.drawCards(_roundInfo);

        foreach(var card in cards) {
            PhotonView.Get(this).RPC("SpawnCard", RpcTarget.AllViaServer, card.first, card.second, playerID);
        }

        if (_roundInfo.hasToDraw && PhotonView.Get(this).IsMine) GameManager.GetComponent<PhotonView>().RPC("finishedTurn", RpcTarget.AllViaServer);//GameManager.finishedTurn();
    }

    [PunRPC]
    public void SpawnCard(CardType a, CardColor b, int playerID) {
        if (PhotonView.Get(this).ViewID == playerID) {
            GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DeckManager.addNewCardToDeck(newCard, new Pair<CardType, CardColor>(a,b));
        }
    }

    private void spawnBlock() {
        GameObject block = Instantiate(BlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        block.transform.SetParent(transform);
        block.transform.localPosition = new Vector3(0.0f,2.0f,-6.2f);
        //Change color material and program autodelete
    }

    [PunRPC]
    public void giveTurn(RoundInfo roundInfo) {
        _roundInfo = roundInfo;

        if (roundInfo.isBlocked) {
            //show block 
            spawnBlock();
            GameManager.GetComponent<PhotonView>().RPC("finishedTurn", RpcTarget.AllViaServer);
            //GameManager.finishedTurn();
        }
    }

    [PunRPC]
    public void initPlayer(int playerID) {
        if (PhotonView.Get(this).ViewID == playerID && PhotonView.Get(this).IsMine) {
            _roundInfo.playerID = playerID;
            for (int i = 0; i < InitCards; ++i) {
                drawCards(playerID);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_roundInfo.isHisTurn && PhotonView.Get(this).IsMine) {
            calculateRayCast();
        }
    }
}
