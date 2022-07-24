using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] bool newCard = false;
    [SerializeField] int NrDecks;
    [SerializeField] int RemainingCards;

    public List<Pair<CardType, CardColor>> AvailablesCards = new List<Pair<CardType, CardColor>>();

    [SerializeField] MiddleManager MiddleManager;

    public List<PhotonView> _players = null;
    [SerializeField] int _currentTurn = -1;
    private bool _senseGame = false;

    void initAvailablesCards() {
        RemainingCards = 108 * NrDecks;

        for (int j = 0; j < 4; ++j) { addCard(Enumerations.getTypeByIndex(0), Enumerations.getColorByIndex(j)); }

        for (int i = 1; i < 10; ++i) {
            for (int j = 0; j < 4; ++j) { for(int t = 0; t < 2; ++t) addCard(Enumerations.getTypeByIndex(i), Enumerations.getColorByIndex(j));  }
        }

        for (int i = 10; i < 13; ++i) {
            for (int j = 0; j < 4; ++j) { for (int t = 0; t < 2; ++t) addCard(Enumerations.getTypeByIndex(i), Enumerations.getColorByIndex(j)); }
        }

        for (int i = 13; i < 15; ++i) {
            for (int t = 0; t < 4; ++t) addCard(Enumerations.getTypeByIndex(i), Enumerations.getColorByIndex(4));
        }
    }

    public List<Pair<CardType, CardColor>> drawCards(RoundInfo roundInfo) {
        if (roundInfo.isHisTurn || _currentTurn == -1) {
            List<Pair<CardType, CardColor>> res = new List<Pair<CardType, CardColor>>();
            
            if (roundInfo.hasToDraw) {
                for (int i = 0; i < MiddleManager.getCurrentWithdraw(); ++i) {
                    res.Add(AvailablesCards[Random.Range(0, AvailablesCards.Count)]);
                    PhotonView.Get(this).RPC("removeCard", RpcTarget.AllViaServer, res[res.Count - 1].first, res[res.Count - 1].second);
                }
                MiddleManager.GetComponent<PhotonView>().RPC("resetWithdraw", RpcTarget.AllViaServer);
                //MiddleManager.resetWithdraw();
            }else { //there will be only a card
                res.Add(AvailablesCards[Random.Range(0, AvailablesCards.Count)]);
                PhotonView.Get(this).RPC("removeCard", RpcTarget.AllViaServer, res[0].first, res[0].second);
            }
            RemainingCards = AvailablesCards.Count;
            return res;
        }
        return null;
    }

    [PunRPC]
    public void addCard(CardType a, CardColor b) {
        AvailablesCards.Add(new Pair<CardType, CardColor>(a, b));
        RemainingCards = AvailablesCards.Count;
    }

    [PunRPC]
    public void removeCard(CardType a, CardColor b) {
        bool found = false;

        for (int i = 0; i < RemainingCards && !found; ++i) {
            if (AvailablesCards[i].first == a && AvailablesCards[i].second == b) {
                AvailablesCards.RemoveAt(i);
                found = true;
            }
        }

        RemainingCards = AvailablesCards.Count;
    }

    public void RotateMiddle(int rot) {
        MiddleManager.RotateMiddle(rot);
    }

    public List<int> getPlayersID() {
        List<int> ids = new List<int>();
        foreach (PhotonView p in _players) {
            ids.Add(p.ViewID);
        }
        return ids;
    }

    public int getLocalPlayerID() {
        foreach (PhotonView p in _players) {
            if (p.IsMine) return p.ViewID;
        }
        return -1;
    }

    void Start()
    {
        initAvailablesCards();

        Pair<CardType, CardColor> card = drawCards(new RoundInfo(true, false, false, true, -1, -1))[0];
        MiddleManager.GetComponent<PhotonView>().RPC("addCardMiddle", RpcTarget.AllViaServer, card.first, card.second, new RoundInfo(true, false, false, true, -1, -1));

        if (MiddleManager.getMiddleColor() == CardColor.Black) MiddleManager.GetComponent<PhotonView>().RPC("changeColorMiddleCard", RpcTarget.AllViaServer, Enumerations.getRandomColor(0, 3));

        _players = new List<PhotonView>();
    }

    void printCard() {

        if (RemainingCards > 0) {
            var temp = drawCards(new RoundInfo(true, false, false, true, -1, -1))[0];
            newCard = false;
        }
    }

    public bool currentPlayer() {
        return true;
    }

    void nextTurn() {
        _senseGame = MiddleManager.hasRevers();

        if (_senseGame) {
            _currentTurn++;
            if (_currentTurn == _players.Count) _currentTurn = 0;
        } else {
            _currentTurn--;
            if (_currentTurn < 0) _currentTurn = _players.Count - 1;
        }
        //Debug.Log(_players[_currentTurn].ViewID);
        _players[_currentTurn].RPC("giveTurn", RpcTarget.AllViaServer, new RoundInfo(true, MiddleManager.getCurrentWithdraw() > 0, MiddleManager.hasBlock(), false, System.Convert.ToInt16(_players[_currentTurn].ViewID), -1/*it's not used to updated the rotation*/));
        //_players[_currentTurn].GetComponent<PlayerManager>().giveTurn(new RoundInfo(true, MiddleManager.getCurrentWithdraw() > 0, MiddleManager.hasBlock(), false, _players[_currentTurn].GetComponent<PhotonView>().ViewID));
    }

    public bool selectingColor() {
        return MiddleManager.transform.Find("ColorSelector(Clone)") != null;
    }

    [PunRPC]
    public void finishedTurn() {
        if (PhotonNetwork.IsMasterClient) {
            nextTurn();
        }
    }

    void Update()
    {
        if (newCard) printCard();
    }

    [PunRPC]
    void PlayerCreated(int _player, int rot) {
        _players.Add(PhotonView.Find(_player));

        if (!PhotonView.Find(_player).IsMine) {
            PhotonView.Find(_player).GetComponentInChildren<Camera>().enabled = false;
        }

        if (PhotonNetwork.IsMasterClient) {
            PhotonView.Find(_player).RPC("initPlayer", RpcTarget.AllViaServer, _player, rot);
            //_player.gameObject.GetComponent<PlayerManager>().initPlayer(_player.ViewID);

            if (_players.Count == PhotonNetwork.CurrentRoom.PlayerCount) {
                PhotonView.Get(this).RPC("finishedTurn", RpcTarget.AllViaServer);
                //nextTurn();
            }
        }
        
    }
}
