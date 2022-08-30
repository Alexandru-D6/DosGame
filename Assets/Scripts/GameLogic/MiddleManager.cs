using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MiddleManager : MonoBehaviourPunCallbacks
{

    [SerializeField] GameManager gameManager;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject reversPrefab;
    [SerializeField] GameObject colorSelectorPrefab;
    [SerializeField] GameObject withdrawCounter;

    private int currentWithdraw = 0;
    public bool usedMiddleCard = false;

    public int initElements = 2;
    public int middleCardStack = 5;

    public void RotateMiddle(int rot) {
        transform.localEulerAngles = new Vector3(withdrawCounter.transform.localEulerAngles.x, rot, withdrawCounter.transform.localEulerAngles.z);
    }

    private void incrementWithdraw(Pair<CardType, CardColor> _newC) {
        if (_newC.first == CardType.Cplus2) currentWithdraw += 2;
        else if (_newC.first == CardType.Cplus4) currentWithdraw += 4;

        if (currentWithdraw > 0) {
            withdrawCounter.SetActive(true);
            withdrawCounter.GetComponent<TextMeshPro>().text = currentWithdraw.ToString();
        }
    }

    private void senseChanged(Pair<CardType, CardColor> _newC) {
        if (_newC.first == CardType.CRevers) {
            GameObject revers = Instantiate(reversPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            revers.transform.SetParent(transform);
            revers.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
            revers.transform.localEulerAngles = new Vector3(55.0f, 0.0f, 0.0f);
        }
    }

    [PunRPC]
    private void colorChangeNeeded(CardType _cardType, CardColor _cardColor, RoundInfo _roundInfo) {
        if (_cardType == CardType.Cplus4 || _cardType == CardType.CChangeColor) { // why not using .second (color)???
            GameObject colorSelector = Instantiate(colorSelectorPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            colorSelector.GetComponent<ColorSelectorManager>().assignPlayerID(_roundInfo.playerID);
        }
    }

    [PunRPC]
    public void destroyColorSelector() {
        Destroy(GameObject.FindGameObjectWithTag("ColorSelector"));
    }

    public bool validCard(Pair<CardType, CardColor> _newC, RoundInfo _roundInfo) {
        Pair<CardType, CardColor> oldC = new Pair<CardType, CardColor>(CardType.C0, CardColor.Blue);

        if (transform.childCount > initElements) {
            oldC = transform.GetChild(0).GetComponent<CardManager>().getInfo();
        }
        //Debug.Log("Validation -->");
        //Debug.Log(oldC.first + " -- " + oldC.second);
        //Debug.Log(newC.first + " -- " + newC.second);

        //Debug.Log(roundInfo.isHisTurn + " -- " + roundInfo.isBlocked + " -- " + roundInfo.hasToDraw);

        //default
        if (_roundInfo.isHisTurn && !_roundInfo.isBlocked) {
            //Debug.Log(roundInfo.isHisTurn + " -- " + roundInfo.isBlocked + " -- " + roundInfo.hasToDraw);
            if (_roundInfo.hasToDraw) {
                if ((_newC.first == CardType.Cplus2 && _newC.second == oldC.second) || (_newC.first == oldC.first) || _newC.first == CardType.Cplus4) return true;
            } else {
                if (oldC.first == _newC.first) return true;
                if (oldC.second == _newC.second) return true;
                if (_newC.second == CardColor.Black) return true;
            }
        }

        return false;
    }

    [PunRPC]
    public void addCardMiddle(CardType _newCa, CardColor _newCb, RoundInfo _roundInfo) {
        //Debug.Log("intentando añadir carta!");
        Pair<CardType, CardColor> newC = new Pair<CardType, CardColor>(_newCa, _newCb);

        //Debug.Log(validCard(newC, roundInfo));
        if (transform.childCount == initElements || (transform.childCount > initElements)) {
            if (PhotonNetwork.IsMasterClient) {
                PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, false);
            }
          
            //_usedMiddleCard = false;
            moveAllCardsDown();
            GameObject newCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            newCard.transform.SetParent(transform);
            newCard.transform.SetSiblingIndex(0);
            newCard.GetComponent<CardManager>().changeCard(newC);

            newCard.transform.localEulerAngles = new Vector3(90.0f, Random.Range(20.0f,-20.0f), 0.0f);
            newCard.transform.localPosition = new Vector3(0.0f, 0.20f, 0.0f);

            incrementWithdraw(newC);
            senseChanged(newC);
            if (!_roundInfo.automaticPlay && PhotonNetwork.IsMasterClient) PhotonView.Get(this).RPC("colorChangeNeeded", RpcTarget.AllViaServer, newC.first, newC.second, _roundInfo);
            //Debug.Log("conseguido");
        }
    }

    [PunRPC]
    public void resetWithdraw() { 
        currentWithdraw = 0;
        withdrawCounter.SetActive(false);
        withdrawCounter.GetComponent<TextMeshPro>().text = currentWithdraw.ToString();
    }

    public int getCurrentWithdraw() { return currentWithdraw; }

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
        if (transform.childCount > initElements && !usedMiddleCard) {
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
        usedMiddleCard = state;
    }

    public bool hasBlock() {
        if (transform.childCount > initElements && !usedMiddleCard) {
            if (transform.GetChild(0).GetComponent<CardManager>().CardType == CardType.CBlock) {
                PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, true);
                //_usedMiddleCard = true;
                return true;
            }
        }
        PhotonView.Get(this).RPC("useMiddleCard", RpcTarget.AllViaServer, false);
        return false;
    }

    public int sizeMiddleCards() {
        return (transform.childCount - initElements);
    }

    [PunRPC]
    public void changeColorMiddleCard(CardColor _color) {
        if (transform.childCount > initElements && getMiddleColor() == CardColor.Black) {
            transform.GetChild(0).GetComponent<CardManager>().changeCard(new Pair<CardType, CardColor>(getMiddleType(), _color));
        }
    }

    void moveAllCardsDown() {
        if (transform.childCount > middleCardStack + initElements) {
            CardManager toDelete = transform.GetChild(middleCardStack - 1).GetComponent<CardManager>();
            gameManager.GetComponent<PhotonView>().RPC("addCard", RpcTarget.AllViaServer, toDelete.CardType, (toDelete.CardType == CardType.Cplus4 || toDelete.CardType == CardType.CChangeColor) ? CardColor.Black : toDelete.CardColor);
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
