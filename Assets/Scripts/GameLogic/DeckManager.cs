using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeckManager : MonoBehaviourPunCallbacks
{
    //List<Pair<Pair<CardType, CardColor>, GameObject>> PlayerCards = new List<Pair<Pair<CardType, CardColor>, GameObject>>();
    private GameObject _risedCard;
    private GameManager GameManager;
    [SerializeField] MiddleManager MiddleManager;

    [SerializeField] float MaxAngle;
    [SerializeField] float MaxSeparation;

    private void Start() {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        MiddleManager = GameObject.FindGameObjectWithTag("MiddleCard").GetComponent<MiddleManager>();
    }

    //TODO: implement Binary Search to improve performance
    private void newCard(GameObject newCard, Pair<CardType, CardColor> card) {
        for (int i = transform.childCount-2; i >= 0; --i) {
            CardManager childCard = transform.GetChild(i).GetComponent<CardManager>();
            if ((childCard.CardColor.getIndex() >= card.second.getIndex() && childCard.CardType.getIndex() >= card.first.getIndex()) || childCard.CardColor.getIndex() > card.second.getIndex()) {
                newCard.transform.SetSiblingIndex(i);
            }
        }
    }

    private void reorganizeCards() {
        int nr = transform.childCount;
        if (nr == 0) return;

        float angleDispersion = Mathf.Min(MaxAngle*2.0f / nr, MaxSeparation);
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

    public void addNewCardToDeck(GameObject newCard, Pair<CardType, CardColor> card) {
        newCard.transform.SetParent(transform);
        newCard.GetComponent<CardManager>().initCard(card);

        this.newCard(newCard, card);
        this.reorganizeCards();
    }

    GameObject findChildCard(Pair<CardType, CardColor> Card) {
        foreach(var card in transform.GetComponentsInChildren<CardManager>()) {
            if (card.getInfo() == Card) {
                return card.gameObject;
            }
        }
        return null;
    }

    [PunRPC]
    public void deleteCardFromDeck(CardType a, CardColor b, RoundInfo roundInfo) {
        Pair<CardType, CardColor> Card = new Pair<CardType, CardColor>(a, b);

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
    public bool removeCardFromDeck(GameObject Card, RoundInfo roundInfo) {
        Pair<CardType, CardColor> card = Card.GetComponent<CardManager>().getInfo();
        if (MiddleManager.validCard(card, roundInfo)) {
            //Debug.Log(roundInfo);
            //Debug.Log(roundInfo.isHisTurn + " -- " + roundInfo.isBlocked + " -- " + roundInfo.hasToDraw);
            MiddleManager.GetComponent<PhotonView>().RPC("addCardMiddle", RpcTarget.AllViaServer, card.first, card.second, roundInfo);
            PhotonView.Get(this).RPC("deleteCardFromDeck", RpcTarget.AllViaServer, card.first, card.second, roundInfo);

            if (card.second == CardColor.Black) return false;
            return true;
        }else {
            //TODO: some wiggle to notify that move is incorrect
            return false;
        }
    }

    public void removeAllCards() {
        foreach(var child in transform.GetComponentsInChildren<CardManager>()) {
            GameManager.addCard(child.CardType, child.CardColor);
            Destroy(child.gameObject);
        }
    }

    //TODO: implement Binary Search to improve performance
    private int findChildIndex(GameObject obj) {
        for (int i = 0; i < transform.childCount; ++i) {
            if (transform.GetChild(i).gameObject == obj) return i;
        }
        return -1;
    }

    private void expandColor(int index) {
        float compact = -1.0f;
        for (int i = index - 1; i >= 0; --i) {
            CardManager child = transform.GetChild(i).GetComponent<CardManager>();
            float prevZ = transform.GetChild(i + 1).GetComponent<CardManager>().getAngle();

            if (child.CardColor == _risedCard.GetComponent<CardManager>().CardColor) {
                child.setAngle(prevZ + MaxSeparation);
            }else {
                if (compact == -1.0f) {
                    compact = (Mathf.Abs(MaxAngle - prevZ) / (i + 1));
                    compact = Mathf.Min(compact, MaxSeparation);
                    //Debug.Log(compact);
                }
                child.setAngle( prevZ + compact );
            }
        }

        compact = -1.0f;

        for (int i = index + 1; i < transform.childCount; ++i) {
            CardManager child = transform.GetChild(i).GetComponent<CardManager>();
            float prevZ = transform.GetChild(i - 1).GetComponent<CardManager>().getAngle();

            if (child.CardColor == _risedCard.GetComponent<CardManager>().CardColor || transform.GetChild(i - 1).GetComponent<CardManager>().CardColor == _risedCard.GetComponent<CardManager>().CardColor) {
                child.setAngle(prevZ - MaxSeparation);
            } else {
                if (compact == -1.0f) {
                    compact = (Mathf.Abs(prevZ - (-1.0f*MaxAngle)) / (transform.childCount - i));
                    compact = Mathf.Min(compact, MaxSeparation);
                    //Debug.Log(compact);
                }
                child.setAngle(prevZ - compact);
            }
        }
    }

    public void riseCardFromDeck(GameObject risedCard) {
        if (_risedCard != risedCard ) {

            if (_risedCard != null) {
                _risedCard.GetComponent<CardManager>().sitCard();
                this.reorganizeCards();
            }

            _risedCard = risedCard;

            if (_risedCard != null) {
                _risedCard.GetComponent<CardManager>().riseCard();
                this.expandColor(findChildIndex(_risedCard));
            }
        }
    }
}
