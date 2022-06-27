using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    //List<Pair<Pair<CardType, CardColor>, GameObject>> PlayerCards = new List<Pair<Pair<CardType, CardColor>, GameObject>>();
    private GameObject _risedCard;
    private GameManager GameManager;

    private void Start() {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

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

        float angleDispersion = 40.0f / nr;
        float zDispersion = 0.01f;
        float initZ = zDispersion * (nr / 2.0f);
        float initAngle = 20.0f;

        for (int i = 0; i < transform.childCount; ++i) {
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

    public void removeCardFromDeck(GameObject Card) {
        GameManager.addCard(Card.GetComponent<CardManager>().CardType, Card.GetComponent<CardManager>().CardColor);
        Destroy(Card.gameObject);

        this.reorganizeCards();
    }

    public void removeAllCards() {
        foreach(var child in transform.GetComponentsInChildren<CardManager>()) {
            GameManager.addCard(child.GetComponent<CardManager>().CardType, child.GetComponent<CardManager>().CardColor);
            Destroy(child.gameObject);
        }
    }

    public void riseCardFromDeck(GameObject risedCard) {
        if (_risedCard != risedCard ) {
            if (_risedCard != null) {
                _risedCard.GetComponent<CardManager>().sitCard();
            }
            _risedCard = risedCard;
            if (_risedCard != null) {
                _risedCard.GetComponent<CardManager>().riseCard();
            }
        }
    }
}
