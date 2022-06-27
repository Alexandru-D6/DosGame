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

    public Pair<CardType, CardColor> getCard() {
        var res = AvailablesCards[Random.Range(0, AvailablesCards.Count)];
        AvailablesCards.Remove(res);
        RemainingCards = AvailablesCards.Count;
        return res;
    }

    public void addCard(CardType a, CardColor b) {
        AvailablesCards.Add(new Pair<CardType, CardColor>(a, b));
        RemainingCards = AvailablesCards.Count;
    }

    void Start()
    {
        initAvailablesCards();

        MiddleManager.addCard(this.getCard());
    }

    void printCard() {

        if (RemainingCards > 0) {
            var temp = getCard();
            newCard = false;
        }
    }

    void Update()
    {
        if (newCard) printCard();
    }
}
