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
            Transform objectHit = hit.transform;

            if (objectHit.gameObject.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                //TODO: levantar y alejar las demas cartas
            }

            if (Input.GetMouseButtonDown(0)) {
                if (objectHit.gameObject.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                    //enviar carta al centro de la mesa
                } else if (objectHit.gameObject.layer == LayerMask.NameToLayer(Layers.CenterDeck.getString())) {
                    var card = GameManager.getCard();
                    Debug.Log(card.first.getString() + "_" + card.second.getString());
                    GameObject newCard = Instantiate(CardPrefab, new Vector3(0,0,0), Quaternion.identity);

                    DeckManager.addNewCardToDeck(newCard, card);
                }
            }else if (Input.GetMouseButtonDown(1)) {
                DeckManager.removeAllCards();
            }

        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        calculateRayCast();
    }
}
