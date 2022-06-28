using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool newCard = false;
    [SerializeField] int NrDecks;
    [SerializeField] int RemainingCards;

    public List<Pair<CardType, CardColor>> AvailablesCards = new List<Pair<CardType, CardColor>>();

    [SerializeField] MiddleManager MiddleManager;

    private List<GameObject> _players = null;
    private int _currentTurn = -1;
    private bool _senseGame = false;

    void initAvailablesCards() {
        RemainingCards = 108 * NrDecks;

        for (int j = 0; j < 4; ++j) { addCard(Enumerations.getTypeByIndex(0), Enumerations.getColorByIndex(j)); }

        for (int i = 1; i < 10; ++i) {
            for (int j = 0; j < 4; ++j) { for(int t = 0; t < 2; ++t) addCard(Enumerations.getTypeByIndex(i), Enumerations.getColorByIndex(j));  }
        }

        for (int i = 10; i < 13; ++i) {
            for (int j = 0; j < 4; ++j) { for (int t = 0; t < 2; ++t) addCard(Enumerations.getTypeByIndex(i), Enumerations.getColorByIndex(j)); }
        }

        for (int i = 13; i < 15; ++i) {
            for (int t = 0; t < 4; ++t) addCard(Enumerations.getTypeByIndex(i), Enumerations.getColorByIndex(4));
        }
    }

    public List<Pair<CardType, CardColor>> drawCards(RoundInfo roundInfo) {
        if (roundInfo.isHisTurn) {
            List<Pair<CardType, CardColor>> res = new List<Pair<CardType, CardColor>>();
            
            if (roundInfo.hasToDraw) {
                for (int i = 0; i < MiddleManager.getCurrentWithdraw(); ++i) {
                    res.Add(AvailablesCards[Random.Range(0, AvailablesCards.Count)]);
                    AvailablesCards.Remove(res[res.Count-1]);
                }
                MiddleManager.resetWithdraw();
            }else { //there will be only a card
                res.Add(AvailablesCards[Random.Range(0, AvailablesCards.Count)]);
                AvailablesCards.Remove(res[0]);
            }
            RemainingCards = AvailablesCards.Count;
            return res;
        }
        return null;
    }

    public void addCard(CardType a, CardColor b) {
        AvailablesCards.Add(new Pair<CardType, CardColor>(a, b));
        RemainingCards = AvailablesCards.Count;
    }

    void Start()
    {
        initAvailablesCards();

        MiddleManager.addCard(this.drawCards(new RoundInfo(true, false, false))[0], new RoundInfo(true,false,false));

        _players = new List<GameObject>();
        //refactor to add multi
        _players.Add(GameObject.FindGameObjectWithTag("Player"));
        nextTurn();
    }

    void printCard() {

        if (RemainingCards > 0) {
            var temp = drawCards(new RoundInfo(true, false, false))[0];
            newCard = false;
        }
    }

    void nextTurn() {
        if (_senseGame) {
            _currentTurn++;
            if (_currentTurn == _players.Count) _currentTurn = 0;
        }


        _players[_currentTurn].GetComponent<PlayerManager>().giveTurn(new RoundInfo(true, MiddleManager.getCurrentWithdraw() > 0, false));
    }

    public void finishedTurn() {
        nextTurn();
    }

    void Update()
    {
        if (newCard) printCard();
    }
}
