using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiddleManager : MonoBehaviour
{

    [SerializeField] GameManager GameManager;

    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject ReversPrefab;
    [SerializeField] GameObject ColorSelectorPrefab;
    [SerializeField] GameObject WithdrawCounter;

    private int _currentWithdraw = 0;
    private bool _usedMiddleCard = false;

    private bool validCard(Pair<CardType, CardColor> oldC, Pair<CardType, CardColor> newC, RoundInfo roundInfo) {
        //default
        if (roundInfo.isHisTurn && !roundInfo.isBlocked) {
            if (roundInfo.hasToDraw) {
                if (newC.first == CardType.Cplus2 || newC.first == CardType.Cplus4) return true;
            }else {
                if (oldC.first == newC.first) return true;
                if (oldC.second == newC.second) return true;

                if (newC.second == CardColor.Black) return true;
            }
        }

        return false;
    }

    private void incrementWithdraw(Pair<CardType, CardColor> newC) {
        if (newC.first == CardType.Cplus2) _currentWithdraw += 2;
        else if (newC.first == CardType.Cplus4) _currentWithdraw += 4;

        if (_currentWithdraw > 0) {
            WithdrawCounter.SetActive(true);
            WithdrawCounter.GetComponent<TextMeshPro>().text = _currentWithdraw.ToString();
        }
    }

    private void senseChanged(Pair<CardType, CardColor> newC) {
        if (newC.first == CardType.CRevers) {
            GameObject revers = Instantiate(ReversPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            revers.transform.SetParent(transform);
            revers.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
            revers.transform.localEulerAngles = new Vector3(55.0f, 0.0f, 0.0f);
        }
    }

    private void colorChangeNeeded(Pair<CardType, CardColor> newC) {
        if (newC.first == CardType.Cplus4 || newC.first == CardType.CChangeColor) {
            GameObject colorSelector = Instantiate(ColorSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            colorSelector.transform.SetParent(transform);
            colorSelector.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
            colorSelector.transform.localEulerAngles = new Vector3(-115.0f, 0.0f, 0.0f);
        }
    }

    public bool addCard(Pair<CardType, CardColor> newC, RoundInfo roundInfo) {
        Pair<CardType, CardColor> oldC = new Pair<CardType, CardColor>(CardType.C0, CardColor.Blue);

        if (transform.childCount > 1) {
            oldC = transform.GetChild(0).GetComponent<CardManager>().getInfo();
        }

        if (transform.childCount == 1 || (transform.childCount > 1 && validCard(oldC, newC, roundInfo))) {
            _usedMiddleCard = false;
            moveAllCardsDown();
            GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            newCard.transform.SetParent(transform);
            newCard.transform.SetSiblingIndex(0);
            newCard.GetComponent<CardManager>().changeCard(newC);

            newCard.transform.localEulerAngles = new Vector3(90.0f, Random.Range(20.0f,-20.0f), 0.0f);
            newCard.transform.localPosition = new Vector3(0.0f, 0.20f, 0.0f);

            incrementWithdraw(newC);
            senseChanged(newC);
            if (!roundInfo.automaticPlay) colorChangeNeeded(newC);

            return true;
        }else {
            return false;
        }
    }

    public void resetWithdraw() { 
        _currentWithdraw = 0;
        WithdrawCounter.SetActive(false);
        WithdrawCounter.GetComponent<TextMeshPro>().text = _currentWithdraw.ToString();
    }

    public int getCurrentWithdraw() { return _currentWithdraw; }

    public CardColor getMiddleColor() {
        if (transform.childCount > 1) {
            return transform.GetChild(0).GetComponent<CardManager>().CardColor;
        }
        return CardColor.Black;
    }

    public CardType getMiddleType() {
        if (transform.childCount > 1) {
            return transform.GetChild(0).GetComponent<CardManager>().CardType;
        }
        return CardType.C0;
    }

    public bool hasRevers() {
        if (transform.childCount > 1 && !_usedMiddleCard) {
            if (transform.GetChild(0).GetComponent<CardManager>().CardType == CardType.CRevers) {
                _usedMiddleCard = true;
                return true;
            }
        }
        return false;
    }

    public bool hasBlock() {
        if (transform.childCount > 1 && !_usedMiddleCard) {
            if (transform.GetChild(0).GetComponent<CardManager>().CardType == CardType.CBlock) {
                _usedMiddleCard = true;
                return true;
            }
        }
        return false;
    }

    public void changeColorMiddleCard(CardColor color) {
        if (transform.childCount > 1 && getMiddleColor() == CardColor.Black) {
            transform.GetChild(0).GetComponent<CardManager>().changeCard(new Pair<CardType, CardColor>(getMiddleType(), color));
        }
    }

    void moveAllCardsDown() {
        if (transform.childCount > 5) {
            CardManager toDelete = transform.GetChild(4).GetComponent<CardManager>();
            GameManager.addCard(toDelete.CardType, (toDelete.CardType == CardType.Cplus4 || toDelete.CardType == CardType.CChangeColor) ? CardColor.Black : toDelete.CardColor);

            toDelete.transform.SetParent(null);
            Destroy(toDelete.gameObject);
        }

        for (int i = 0; i < transform.childCount-1; ++i) {
            Transform child = transform.GetChild(i);
            child.localPosition = new Vector3(0.0f,child.localPosition.y - 0.01f,0.0f);
        }
    }
}
