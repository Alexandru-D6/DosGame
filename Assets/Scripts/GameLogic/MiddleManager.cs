using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MiddleManager : MonoBehaviourPunCallbacks
{

    [SerializeField] GameManager GameManager;

    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject ReversPrefab;
    [SerializeField] GameObject ColorSelectorPrefab;
    [SerializeField] GameObject WithdrawCounter;

    private int _currentWithdraw = 0;
    public bool _usedMiddleCard = false;

    public int initElements = 2;
    public int middleCardStack = 5;

    public void RotateMiddle(int rot) {
        transform.localEulerAngles = new Vector3(WithdrawCounter.transform.localEulerAngles.x, rot, WithdrawCounter.transform.localEulerAngles.z);
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

    [PunRPC]
    private void colorChangeNeeded(CardType cardType, CardColor cardColor, RoundInfo roundInfo) {
        if (cardType == CardType.Cplus4 || cardType == CardType.CChangeColor) { // why not using .second (color)???
            GameObject colorSelector = Instantiate(ColorSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            colorSelector.GetComponent<ColorSelectorManager>().assignPlayerID(roundInfo.playerID);
        }
    }

    [PunRPC]
    public void destroyColorSelector() {
        Destroy(GameObject.FindGameObjectWithTag("ColorSelector"));
    }

    public bool validCard(Pair<CardType, CardColor> newC, RoundInfo roundInfo) {
        Pair<CardType, CardColor> oldC = new Pair<CardType, CardColor>(CardType.C0, CardColor.Blue);

        if (transform.childCount > initElements) {
            oldC = transform.GetChild(0).GetComponent<CardManager>().getInfo();
        }
        //Debug.Log("Validation -->");
        //Debug.Log(oldC.first + " -- " + oldC.second);
        //Debug.Log(newC.first + " -- " + newC.second);

        //Debug.Log(roundInfo.isHisTurn + " -- " + roundInfo.isBlocked + " -- " + roundInfo.hasToDraw);

        //default
        if (roundInfo.isHisTurn && !roundInfo.isBlocked) {
            //Debug.Log(roundInfo.isHisTurn + " -- " + roundInfo.isBlocked + " -- " + roundInfo.hasToDraw);
            if (roundInfo.hasToDraw) {
                if ((newC.first == CardType.Cplus2 && newC.second == oldC.second) || newC.first == CardType.Cplus4) return true;
            } else {
                if (oldC.first == newC.first) return true;
                if (oldC.second == newC.second) return true;
                if (newC.second == CardColor.Black) return true;
            }
        }

        return false;
    }

    [PunRPC]
    public void addCardMiddle(CardType newCa, CardColor newCb, RoundInfo roundInfo) {
        //Debug.Log("intentando añadir carta!");
        Pair<CardType, CardColor> newC = new Pair<CardType, CardColor>(newCa, newCb);

        //Debug.Log(validCard(newC, roundInfo));
        if (transform.childCount == initElements || (transform.childCount > initElements)) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, false);
            }
          
            //_usedMiddleCard = false;
            moveAllCardsDown();
            GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            newCard.transform.SetParent(transform);
            newCard.transform.SetSiblingIndex(0);
            newCard.GetComponent<CardManager>().changeCard(newC);

            newCard.transform.localEulerAngles = new Vector3(90.0f, Random.Range(20.0f,-20.0f), 0.0f);
            newCard.transform.localPosition = new Vector3(0.0f, 0.20f, 0.0f);

            incrementWithdraw(newC);
            senseChanged(newC);
            if (!roundInfo.automaticPlay && PhotonNetwork.IsMasterClient) PhotonView.Get(this).RPC("colorChangeNeeded", RpcTarget.AllViaServer, newC.first, newC.second, roundInfo);
            //Debug.Log("conseguido");
        }
    }

    [PunRPC]
    public void resetWithdraw() { 
        _currentWithdraw = 0;
        WithdrawCounter.SetActive(false);
        WithdrawCounter.GetComponent<TextMeshPro>().text = _currentWithdraw.ToString();
    }

    public int getCurrentWithdraw() { return _currentWithdraw; }

    public CardColor getMiddleColor() {
        if (transform.childCount > initElements) {
            return transform.GetChild(0).GetComponent<CardManager>().CardColor;
        }
        return CardColor.Black;
    }

    public CardType getMiddleType() {
        if (transform.childCount > initElements) {
            return transform.GetChild(0).GetComponent<CardManager>().CardType;
        }
        return CardType.C0;
    }

    public bool hasRevers() {
        if (transform.childCount > initElements && !_usedMiddleCard) {
            if (transform.GetChild(0).GetComponent<CardManager>().CardType == CardType.CRevers) {
                PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, true);
                //_usedMiddleCard = true;
                return true;
            }
        }
        PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, false);
        return false;
    }

    [PunRPC]
    public void useMiddleCard(bool state) {
        _usedMiddleCard = state;
    }

    public bool hasBlock() {
        if (transform.childCount > initElements && !_usedMiddleCard) {
            if (transform.GetChild(0).GetComponent<CardManager>().CardType == CardType.CBlock) {
                PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, true);
                //_usedMiddleCard = true;
                return true;
            }
        }
        PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, false);
        return false;
    }

    [PunRPC]
    public void changeColorMiddleCard(CardColor color) {
        if (transform.childCount > initElements && getMiddleColor() == CardColor.Black) {
            transform.GetChild(0).GetComponent<CardManager>().changeCard(new Pair<CardType, CardColor>(getMiddleType(), color));
        }
    }

    void moveAllCardsDown() {
        if (transform.childCount > middleCardStack + initElements) {
            CardManager toDelete = transform.GetChild(middleCardStack - 1).GetComponent<CardManager>();
            GameManager.GetComponent<PhotonView>().RPC("addCard", RpcTarget.AllViaServer, toDelete.CardType, (toDelete.CardType == CardType.Cplus4 || toDelete.CardType == CardType.CChangeColor) ? CardColor.Black : toDelete.CardColor);
            //GameManager.addCard(toDelete.CardType, (toDelete.CardType == CardType.Cplus4 || toDelete.CardType == CardType.CChangeColor) ? CardColor.Black : toDelete.CardColor);

            toDelete.transform.SetParent(null);
            Destroy(toDelete.gameObject);
        }

        for (int i = 0; i < transform.childCount-initElements; ++i) {
            Transform child = transform.GetChild(i);
            child.localPosition = new Vector3(0.0f,child.localPosition.y - 0.01f,0.0f);
        }
    }
}
