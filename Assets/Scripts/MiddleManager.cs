using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleManager : MonoBehaviour
{

    [SerializeField] GameManager GameManager;

    [SerializeField] GameObject CardPrefab;

    public bool addCard(Pair<CardType, CardColor> card) {
        if (transform.childCount == 0 || (transform.childCount > 0 && (transform.GetChild(0).GetComponent<CardManager>().CardColor == card.second || transform.GetChild(0).GetComponent<CardManager>().CardType == card.first))) {
            moveAllCardsDown();
            GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            newCard.transform.SetParent(transform);
            newCard.transform.SetSiblingIndex(0);
            newCard.GetComponent<CardManager>().changeCard(card.first, card.second);

            newCard.transform.localEulerAngles = new Vector3(90.0f, Random.Range(20.0f,-20.0f), 0.0f);
            newCard.transform.localPosition = new Vector3(0.0f, 0.20f, 0.0f);

            return true;
        }else {
            return false;
        }
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
