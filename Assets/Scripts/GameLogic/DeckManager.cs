using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeckManager : MonoBehaviourPunCallbacks
{
    //List<Pair<Pair<CardType, CardColor>, GameObject>> PlayerCards = new List<Pair<Pair<CardType, CardColor>, GameObject>>();
    private GameObject risedCard;
    [SerializeField] GameManager gameManager;
    [SerializeField] MiddleManager middleManager;

    [SerializeField] float maxAngle;
    [SerializeField] float maxSeparation;

    private void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        middleManager = GameObject.FindGameObjectWithTag("MiddleCard").GetComponent<MiddleManager>();
    }

    //TODO: implement Binary Search to improve performance
    private void newCard(GameObject _newCard, Pair<CardType, CardColor> _card) {
        for (int i = transform.childCount-2; i >= 0; --i) {
            CardManager childCard = transform.GetChild(i).GetComponent<CardManager>();
            if ((childCard.CardColor.getIndex() >= _card.second.getIndex() && childCard.CardType.getIndex() >= _card.first.getIndex()) || childCard.CardColor.getIndex() > _card.second.getIndex()) {
                _newCard.transform.SetSiblingIndex(i);
            }
        }
    }

    private void reorganizeCards() {
        int nr = transform.childCount;
        if (nr == 0) return;

        float angleDispersion = Mathf.Min(maxAngle*2.0f / nr, maxSeparation);
        float zDispersion = 0.01f;
        float initZ = 0.0f;
        float initAngle = 0.0f;

        if (nr % 2 == 0) initAngle = angleDispersion / 2.0f;

        for (int i = (int)((nr-1)/2); i >= 0; i--) {
            CardManager childCard = transform.GetChild(i).GetComponent<CardManager>();

            childCard.setZ(initZ);
            childCard.setAngle(initAngle);

            initAngle += angleDispersion;
            initZ += zDispersion;
        }

        initZ = -1.0f*zDispersion;

        if (nr % 2 == 0) initAngle = angleDispersion / -2.0f;
        else initAngle = -1.0f*angleDispersion;

        for (int i = (int)((nr - 1) / 2)+1; i < transform.childCount; ++i) {
            CardManager childCard = transform.GetChild(i).GetComponent<CardManager>();

            childCard.setZ(initZ);
            childCard.setAngle(initAngle);

            initAngle -= angleDispersion;
            initZ -= zDispersion;
        }
    }

    public void addNewCardToDeck(GameObject _newCard, Pair<CardType, CardColor> _card) {
        _newCard.transform.SetParent(transform);
        _newCard.GetComponent<CardManager>().initCard(_card);

        this.newCard(_newCard, _card);
        this.reorganizeCards();
    }

    GameObject findChildCard(Pair<CardType, CardColor> _card) {
        foreach(var card in transform.GetComponentsInChildren<CardManager>()) {
            if (card.getInfo() == _card) {
                return card.gameObject;
            }
        }
        return null;
    }

    [PunRPC]
    public void deleteCardFromDeck(CardType _a, CardColor _b, RoundInfo _roundInfo) {
        Pair<CardType, CardColor> Card = new Pair<CardType, CardColor>(_a, _b);

        GameObject deckCard = findChildCard(Card);

        if (deckCard != null) {
            deckCard.transform.SetParent(null);
            Destroy(deckCard);
        } else {
            Debug.Log("error");
        }

        this.reorganizeCards();
    }

    //TODO: separarlo en dos para hacer directamente la llamada punRPC desde player manager
    public bool removeCardFromDeck(GameObject _card, RoundInfo _roundInfo) {
        Pair<CardType, CardColor> card = _card.GetComponent<CardManager>().getInfo();
        if (middleManager.validCard(card, _roundInfo)) {
            //Debug.Log(roundInfo);
            //Debug.Log(roundInfo.isHisTurn + " -- " + roundInfo.isBlocked + " -- " + roundInfo.hasToDraw);
            middleManager.GetComponent<PhotonView>().RPC("addCardMiddle", RpcTarget.AllViaServer, card.first, card.second, _roundInfo);
            PhotonView.Get(this).RPC("deleteCardFromDeck", RpcTarget.AllViaServer, card.first, card.second, _roundInfo);

            if (card.second == CardColor.Black) return false;
            return true;
        }else {
            //TODO: some wiggle to notify that move is incorrect
            return false;
        }
    }

    public void removeAllCards() {
        foreach(var child in transform.GetComponentsInChildren<CardManager>()) {
            gameManager.addCard(child.CardType, child.CardColor);
            Destroy(child.gameObject);
        }
    }

    //TODO: implement Binary Search to improve performance
    private int findChildIndex(GameObject _obj) {
        for (int i = 0; i < transform.childCount; ++i) {
            if (transform.GetChild(i).gameObject == _obj) return i;
        }
        return -1;
    }

    private void expandColor(int _index) {
        float compact = -1.0f;
        for (int i = _index - 1; i >= 0; --i) {
            CardManager child = transform.GetChild(i).GetComponent<CardManager>();
            float prevZ = transform.GetChild(i + 1).GetComponent<CardManager>().getAngle();

            if (child.CardColor == risedCard.GetComponent<CardManager>().CardColor) {
                child.setAngle(prevZ + maxSeparation);
            }else {
                if (compact == -1.0f) {
                    compact = (Mathf.Abs(maxAngle - prevZ) / (i + 1));
                    compact = Mathf.Min(compact, maxSeparation);
                    //Debug.Log(compact);
                }
                child.setAngle( prevZ + compact );
            }
        }

        compact = -1.0f;

        for (int i = _index + 1; i < transform.childCount; ++i) {
            CardManager child = transform.GetChild(i).GetComponent<CardManager>();
            float prevZ = transform.GetChild(i - 1).GetComponent<CardManager>().getAngle();

            if (child.CardColor == risedCard.GetComponent<CardManager>().CardColor || transform.GetChild(i - 1).GetComponent<CardManager>().CardColor == risedCard.GetComponent<CardManager>().CardColor) {
                child.setAngle(prevZ - maxSeparation);
            } else {
                if (compact == -1.0f) {
                    compact = (Mathf.Abs(prevZ - (-1.0f*maxAngle)) / (transform.childCount - i));
                    compact = Mathf.Min(compact, maxSeparation);
                    //Debug.Log(compact);
                }
                child.setAngle(prevZ - compact);
            }
        }
    }

    public void riseCardFromDeck(GameObject _risedCard) {
        if (risedCard != _risedCard ) {

            if (risedCard != null) {
                risedCard.GetComponent<CardManager>().sitCard();
                this.reorganizeCards();
            }

            risedCard = _risedCard;

            if (risedCard != null) {
                risedCard.GetComponent<CardManager>().riseCard();
                this.expandColor(findChildIndex(risedCard));
            }
        }
    }
}
