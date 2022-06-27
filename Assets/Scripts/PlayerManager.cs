using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] GameManager GameManager;
    [SerializeField] DeckManager DeckManager;

    [SerializeField] GameObject Deck;


    [SerializeField] GameObject CardPrefab;

    void calculateRayCast() {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.transform.parent.gameObject;

            GameObject _cardToRise = null;

            if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                _cardToRise = objectHit;
            }

            DeckManager.riseCardFromDeck(_cardToRise);

            if (Input.GetMouseButtonDown(0)) {
                if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                    DeckManager.removeCardFromDeck(objectHit);
                    //TODO: enviar carta al centro de la mesa
                } else if (objectHit.layer == LayerMask.NameToLayer(Layers.CenterDeck.getString())) {
                    var card = GameManager.getCard();
                    GameObject newCard = Instantiate(CardPrefab, new Vector3(0,0,0), Quaternion.identity);

                    DeckManager.addNewCardToDeck(newCard, card);
                }
            }else if (Input.GetMouseButtonDown(1)) {
                DeckManager.removeAllCards();
            }

        }
    }

    // Update is called once per frame
    private void Update()
    {
        calculateRayCast();
    }
}
