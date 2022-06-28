using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleManager : MonoBehaviour
{

    [SerializeField] GameManager GameManager;

    [SerializeField] GameObject CardPrefab;

    private int currentWithdraw = 0;

    private bool validCard(Pair<CardType, CardColor> oldC, Pair<CardType, CardColor> newC, RoundInfo roundInfo) {
        //default
        if (roundInfo.isHisTurn && !roundInfo.isBlocked) {
            if (roundInfo.hasToDraw) {
                if (oldC.second == newC.second && newC.first == CardType.Cplus2) return true;
                if (newC.first == CardType.Cplus4) return true;
            }else {
                if (oldC.first == newC.first) return true;
                if (oldC.second == newC.second) return true;

                if (newC.second == CardColor.Black) return true;
            }
        }

        return false;
    }

    private void incrementWithdraw(Pair<CardType, CardColor> newC) {
        if (newC.first == CardType.Cplus2) currentWithdraw += 2;
        else if (newC.first == CardType.Cplus4) currentWithdraw += 4;
    }

    public bool addCard(Pair<CardType, CardColor> newC, RoundInfo roundInfo) {
        Pair<CardType, CardColor> oldC = new Pair<CardType, CardColor>(CardType.C0, CardColor.Blue);

        if (transform.childCount > 0) {
            oldC = transform.GetChild(0).GetComponent<CardManager>().getInfo();
        }

        if (transform.childCount == 0 || (transform.childCount > 0 && validCard(oldC, newC, roundInfo))) {
            moveAllCardsDown();
            GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            newCard.transform.SetParent(transform);
            newCard.transform.SetSiblingIndex(0);
            newCard.GetComponent<CardManager>().changeCard(newC);

            newCard.transform.localEulerAngles = new Vector3(90.0f, Random.Range(20.0f,-20.0f), 0.0f);
            newCard.transform.localPosition = new Vector3(0.0f, 0.20f, 0.0f);

            incrementWithdraw(newC);

            return true;
        }else {
            return false;
        }
    }

    public void resetWithdraw() { currentWithdraw = 0; }

    public int getCurrentWithdraw() { return currentWithdraw; }

    public bool hasToRevers() {
        return 
    }

    void moveAllCardsDown() {
        if (transform.childCount == 5) {
            CardManager toDelete = transform.GetChild(4).GetComponent<CardManager>();
            GameManager.addCard(toDelete.CardType, toDelete.CardColor);

            toDelete.transform.SetParent(null);
            Destroy(toDelete.gameObject);
        }

        for (int i = 0; i < transform.childCount; ++i) {
            Transform child = transform.GetChild(i);
            child.localPosition = new Vector3(0.0f,child.localPosition.y - 0.01f,0.0f);
        }
    }
}
