using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoMovement : MonoBehaviour
{
    public static UndoMovement Instance;
    public List<Move> undoMovements;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //Adds every move that was valid during the game, storing the state of the lists (Deck, Column Piles, Top Piles)
    public void AddMove()
    {
        if (GameController.Instance.winner)
            return;

        Move move = new Move();

        //Add card deck movement
        DeckData deckData = new DeckData();
        deckData.deckCards = new List<Card>(GameController.Instance.deck.cards);
        deckData.deckCardsFaceState = new List<bool>(GameController.Instance.deck.CardsFace());
        deckData.deckCardsClicklabeState = new List<bool>(GameController.Instance.deck.CardsClicklable());

        deckData.deckCardsClicked = new List<Card>(GameController.Instance.deck.cardsShowed);
        deckData.deckCardsClickedFaceState = new List<bool>(GameController.Instance.deck.CardsClickedFace());
        deckData.deckCardsClickedClicklabeState = new List<bool>(GameController.Instance.deck.CardsClickedClicklable());
        move.deckData = deckData;


        //Add columns/piles card movement
        for (int i = 0; i < GameController.Instance.piles.Count; i++)
        {
            PileData pileData = new PileData();
            pileData.pileCards = new List<Card>(GameController.Instance.piles[i].cards);
            pileData.pileCardsFaceState = new List<bool>(GameController.Instance.piles[i].CardsFace());
            pileData.pileCardsClicklabeState = new List<bool>(GameController.Instance.piles[i].CardsClicklable());
            move.pilesData.Add(pileData);
        }

        //Add top piles card movement
        for (int i = 0; i < GameController.Instance.topPiles.Count; i++)
        {
            PileData pileData = new PileData();
            pileData.pileCards = new List<Card>(GameController.Instance.topPiles[i].cards);
            pileData.pileCardsFaceState = new List<bool>(GameController.Instance.topPiles[i].CardsFace());
            pileData.pileCardsClicklabeState = new List<bool>(GameController.Instance.topPiles[i].CardsClicklable());
            move.topPilesData.Add(pileData);
        }
        undoMovements.Add(move);

        GameController.Instance.SetMove(undoMovements.Count);
    }


    public void BackMove()
    {
        if (undoMovements.Count > 0)
        {
            Move move = undoMovements[undoMovements.Count - 1];

            //Remake deck cards
            GameController.Instance.deck.cards = move.deckData.deckCards;
            for (int i = 0; i < GameController.Instance.deck.cards.Count; i++)
            {
                GameController.Instance.deck.cards[i].showCard = move.deckData.deckCardsFaceState[i];
                GameController.Instance.deck.cards[i].isClickable = move.deckData.deckCardsClicklabeState[i];
            }

            //Remake showed cards from deck
            GameController.Instance.deck.cardsShowed = move.deckData.deckCardsClicked;
            for (int i = 0; i < GameController.Instance.deck.cardsShowed.Count; i++)
            {
                GameController.Instance.deck.cardsShowed[i].showCard = move.deckData.deckCardsClickedFaceState[i];
                GameController.Instance.deck.cardsShowed[i].isClickable = move.deckData.deckCardsClickedClicklabeState[i];
            }

            //Remake columns/piles
            for (int i = 0; i < GameController.Instance.piles.Count; i++)
            {
                GameController.Instance.piles[i].cards = move.pilesData[i].pileCards;
                for (int j = 0; j < GameController.Instance.piles[i].cards.Count; j++)
                {
                    GameController.Instance.piles[i].cards[j].showCard = move.pilesData[i].pileCardsFaceState[j];
                    GameController.Instance.piles[i].cards[j].isClickable = move.pilesData[i].pileCardsClicklabeState[j];
                }
            }

            //Remake top piles
            for (int i = 0; i < GameController.Instance.topPiles.Count; i++)
            {
                GameController.Instance.topPiles[i].cards = move.topPilesData[i].pileCards;
                for (int j = 0; j < GameController.Instance.topPiles[i].cards.Count; j++)
                {
                    GameController.Instance.topPiles[i].cards[j].showCard = move.topPilesData[i].pileCardsFaceState[j];
                    GameController.Instance.topPiles[i].cards[j].isClickable = move.topPilesData[i].pileCardsClicklabeState[j];
                }
            }

            //Movement cards to original position
            GameController.Instance.deck.OrganizeShowedCard();
            for (int i = 0; i < GameController.Instance.piles.Count; i++)
                GameController.Instance.piles[i].OrganizePile();

            for (int i = 0; i < GameController.Instance.topPiles.Count; i++)
                GameController.Instance.topPiles[i].OrganizePile();

            undoMovements.Remove(move);
            GameController.Instance.CanDoAutoCompleteGame();
            GameController.Instance.SetMove(undoMovements.Count);
        }
    }

    //Back to the first movement, reset the game
    public void BackToFirstMove()
    {
        if (undoMovements.Count > 1)
            undoMovements.RemoveRange(1, undoMovements.Count - 1);

        BackMove();
    }
}



[System.Serializable]
public class Move
{
    public DeckData deckData;
    public List<PileData> pilesData = new List<PileData>();
    public List<PileData> topPilesData = new List<PileData>();
}


[System.Serializable]
public class DeckData
{
    public List<Card> deckCards = new List<Card>();
    public List<bool> deckCardsFaceState = new List<bool>();
    public List<bool> deckCardsClicklabeState = new List<bool>();

    public List<Card> deckCardsClicked = new List<Card>();
    public List<bool> deckCardsClickedFaceState = new List<bool>();
    public List<bool> deckCardsClickedClicklabeState = new List<bool>();
}



[System.Serializable]
public class PileData
{
    public List<Card> pileCards = new List<Card>();
    public List<bool> pileCardsFaceState = new List<bool>();
    public List<bool> pileCardsClicklabeState = new List<bool>();
}