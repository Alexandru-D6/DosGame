using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] GameManager GameManager;
    [SerializeField] DeckManager DeckManager;

    [SerializeField] GameObject Deck;

    [SerializeField] int InitCards;

    [SerializeField] GameObject CardPrefab;
    [SerializeField] GameObject BlockPrefab;

    private RoundInfo _roundInfo = new RoundInfo(false, false, false, false);

    private void calculateRayCast() {
        RaycastHit hit;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) {
            GameObject objectHit = hit.transform.parent.gameObject;

            GameObject _cardToRise = null;

            if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString()) && _roundInfo.isHisTurn) {
                _cardToRise = objectHit;
            }

            DeckManager.riseCardFromDeck(_cardToRise);

            if (Input.GetMouseButtonDown(0)) {
                if (objectHit.layer == LayerMask.NameToLayer(Layers.PlayerCard.getString())) {
                    if (DeckManager.removeCardFromDeck(objectHit, _roundInfo) && !GameManager.selectingColor()) {
                        GameManager.finishedTurn();
                    }else {
                        //TODO: some wiggle to notify that move is incorrect
                    }
                } else if (objectHit.layer == LayerMask.NameToLayer(Layers.CenterDeck.getString())) {
                    drawCards();
                }
            }else if (Input.GetMouseButtonDown(1)) {
                DeckManager.removeAllCards();
            }
        }
    }

    public void drawCards() {
        var cards = GameManager.drawCards(_roundInfo);

        foreach(var card in cards) {
            GameObject newCard = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DeckManager.addNewCardToDeck(newCard, card);
        }

        if (_roundInfo.hasToDraw) GameManager.finishedTurn();
    }

    private void spawnBlock() {
        GameObject block = Instantiate(BlockPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        block.transform.SetParent(transform);
        block.transform.localPosition = new Vector3(0.0f,2.0f,-6.2f);
        //Change color material and program autodelete
    }

    public void giveTurn(RoundInfo roundInfo) {
        _roundInfo = roundInfo;

        if (roundInfo.isBlocked) {
            //show block 
            spawnBlock();
            GameManager.finishedTurn();
        }
    }

    public void initPlayer() {
        for(int i = 0; i < InitCards; ++i) {
            drawCards();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (_roundInfo.isHisTurn) {
            calculateRayCast();
        }
    }
}
