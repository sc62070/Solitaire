using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public enum GameState
    {
        GAME,
        PAUSE
    }

    [Header("Settings")]
    [Space(10)]
    public Deck deck;
    public List<ColumnPile> piles; // columns
    public List<TopPile> topPiles;

    public Text moveText;
    public Text timeText;

    public int moves;
    public float time;

    [Header("Game UI Buttons")]
    [Space(10)]
    public Button autoCompleteButton;
    public Button undoMovementButton;

    [Header("States")]
    [Space(10)]
    public bool canClick;
    public bool winner;
    public bool canCountTimer;
    public bool threeMode;

    [Header("Audios")]
    [Space(10)]
    public AudioClip shuffleAudio;
    public AudioClip swapAudio;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }


    private void Start()
    {
        autoCompleteButton.onClick.AddListener(AutoCompleteGame);
        undoMovementButton.onClick.AddListener(Undo);
        StartCoroutine(WaitToOrganizeBoard());
    }


    //Creates the deck and distributes the cards to piles
    private IEnumerator WaitToOrganizeBoard()
    {
        canClick = false;
        deck.MakeDeck();
        deck.ShuffleDeck();
        yield return new WaitForSeconds(0.3f);
        threeMode = Settings.Instance.intToBool(Settings.Instance.threeModeTurnActive); // Check game mode
        EffectPlayer.instance.PlayOneShotSong(shuffleAudio);

        for (int i = 0; i < piles.Count; i++)
        {
            for (int j = 0; j < (i + 1); j++)
            {
                Card card = deck.GetLastCardFromDeck();
                card.showCard = false;
                card.isClickable = false;
                piles[i].AddCardToPile(card);

                //Just turn the last card
                if (i == j)
                {
                    card.showCard = true;
                    card.isClickable = true;
                }
            }
            piles[i].OrganizePile();
        }
        canClick = true;
    }

    //Move to top pile
    public bool MoveCardToTopPile(Card card)
    {
        Pile pile = null;
        //Checks if it is possible to move the card to the top pile
        for (int i = 0; i < topPiles.Count; i++)
        {
            if (topPiles[i].ValidToAddCardToPile(card, topPiles[i].cards.Count))
            {
                UndoMovement.Instance.AddMove();
                pile = topPiles[i];
                topPiles[i].AddCardToPile(card);
                pile.OrganizePile();
                IsWonTheGame();
                return true;
            }
        }
        return false;
    }

    //Move to column pile
    public bool MoveOneCardToColumnPile(Card card)
    {
        Pile pile = null;
        //Checks if you have any other piles that can receive the card's movement
        for (int i = 0; i < piles.Count; i++)
        {
            if (piles[i].ValidToAddCardToPile(card, piles[i].cards.Count))
            {
                UndoMovement.Instance.AddMove();
                pile = piles[i];
                piles[i].AddCardToPile(card);
                pile.OrganizePile();
                return true;
            }
        }
        return false;
    }


    public (List<Card>, Pile) CardsToMove(Card card)
    {
        Pile pile = null;
        List<Card> cardsToMove = new List<Card>();
        //Checks which pile has the clicked card, takes the current card and its relatives
        for (int i = 0; i < piles.Count; i++)
        {
            if (piles[i].ContainsCard(card))
            {
                pile = piles[i];
                cardsToMove = piles[i].GetCardAndChilds(card);
                break;
            }
        }

        return (cardsToMove, pile);
    }

    public void MovePileCardToAnotherColumnPile(Card card, Pile actualPile, List<Card> cardsToMove)
    {
        Pile newPile = null;
        //Checks if you have any other piles that can receive the card's movement
        bool canMoveCard = false;
        for (int i = 0; i < piles.Count; i++)
        {
            if (piles[i] != actualPile)
            {
                if (piles[i].ValidToAddCardToPile(card, piles[i].cards.Count))
                {
                    UndoMovement.Instance.AddMove();
                    newPile = piles[i];
                    canMoveCard = true;
                    //If valid, pass all the cards to this pile
                    for (int j = 0; j < cardsToMove.Count; j++)
                    {
                        piles[i].AddCardToPile(cardsToMove[j]);
                    }
                    break;
                }
            }
        }

        //If the move was validated, remove the cards from the current pile
        if (canMoveCard)
        {
            EffectPlayer.instance.PlayOneShotSong(swapAudio);
            for (int i = 0; i < cardsToMove.Count; i++)
            {
                actualPile.RemoveCardFromPile(cardsToMove[i]);
            }

            //Rearranges the current pile and the pile that received the cards
            if (actualPile != null)
                actualPile.OrganizePile();
            if (newPile != null)
                newPile.OrganizePile();

            //Checks if it is possible to untap the last card in the pile
            actualPile.UntapLastCard();
        }
        else
            card.ShakeCard();
    }


    //Checks if the player's move is possible
    public void ValidMoveCard(Card card)
    {
        //Returns the type of move to be made
        int operation = CardClickedOperation(card);

        //If it's the player's first move, the timer starts
        if (operation != 0)
        {
            StartStopwatch();
        }


        //[Click/Touch] came from a deck card
        if (operation == 1)
        {
            //Checks if it is possible to place the card on top of the pile
            if (MoveCardToTopPile(card))
            {
                deck.RemoveFromCardShowed(card);
                deck.ValidCardsClickable();
                EffectPlayer.instance.PlayOneShotSong(swapAudio);
            }
            //Checks if it is possible to place the card in one of the columns/piles
            else if (MoveOneCardToColumnPile(card))
            {
                deck.RemoveFromCardShowed(card);
                deck.ValidCardsClickable();
                EffectPlayer.instance.PlayOneShotSong(swapAudio);
            }
            else
                card.ShakeCard();
        }
        //[Click/Touch] came from a top pile card
        else if (operation == 2)
        {
            //Checks if it is possible to place the card in one of the columns/piles
            if (MoveOneCardToColumnPile(card))
            {
                EffectPlayer.instance.PlayOneShotSong(swapAudio);
                //If it was valid, remove the card that was on the top pile
                for (int i = 0; i < topPiles.Count; i++)
                {
                    if (topPiles[i].ContainsCard(card))
                    {
                        topPiles[i].RemoveCardFromPile(card);
                        break;
                    }
                }
            }
            else
            {
                card.ShakeCard();
            }
        }
        //[Click/Touch] came from a column/pile
        else if (operation == 3)
        {
            var tupleCardMove = CardsToMove(card); //Method with two returns (List of cards AND actual pile)
            List<Card> cardsToMove = tupleCardMove.Item1;
            Pile pile = tupleCardMove.Item2;

            bool moveToTopPile = false;
            //Checks if the clicked card has no children
            if (pile.GetCardAndChilds(card).Count == 1)
            {
                moveToTopPile = MoveCardToTopPile(card); //Return if it is possible to move to the top of the stack
                if (moveToTopPile)
                {
                    EffectPlayer.instance.PlayOneShotSong(swapAudio);
                    pile.RemoveCardFromPile(card);
                    pile.UntapLastCard();
                }
            }

            //If it was not possible to move the card to the top pile, check the other columns/piles
            if (cardsToMove.Count > 0 && !moveToTopPile)
                MovePileCardToAnotherColumnPile(card, pile, cardsToMove);
        }
        else
        {
            Debug.LogWarning("Card does not belong in any pile");
        }

        //Check if it is possible to autocomplete the game
        autoCompleteButton.gameObject.SetActive(CanDoAutoCompleteGame());
    }

    //Returns the type of move the player is making
    public int CardClickedOperation(Card card)
    {
        //[Click/Touch] came from a deck card
        if (deck.ContainsCardShowed(card))
            return 1;

        //[Click/Touch] came from the top piles
        for (int i = 0; i < topPiles.Count; i++)
            if (topPiles[i].ContainsCard(card))
                return 2;

        //[Click/Touch] came from the columns/piles
        for (int i = 0; i < piles.Count; i++)
            if (piles[i].ContainsCard(card))
                return 3;

        return 0;
    }


    //Check if the game is finished
    public void IsWonTheGame()
    {
        //If all top piles are filled, then the game is over.
        bool value = true;
        for (int i = 0; i < topPiles.Count; i++)
        {
            if (topPiles[i].cards.Count != 13)
            {
                value = false;
                break;
            }
        }

        if (value)
        {
            CancelInvoke("AddTime");
            winner = true;
            canClick = false;
        }
    }

    //Check if it is possible to autocomplete the game
    public bool CanDoAutoCompleteGame()
    {
        autoCompleteButton.gameObject.SetActive(false);
        //If all the cards in the piles are turned over, it is possible to autocomplete the game
        for (int i = 0; i < piles.Count; i++)
        {
            if (!piles[i].IsAllCardsShowed())
                return false;
        }
        return true;
    }

    public void AutoCompleteGame()
    {
        StartCoroutine(MakeAutoCompleteMovements());
    }

    //Animation to auto-complete the game
    public IEnumerator MakeAutoCompleteMovements()
    {
        GameController.Instance.canClick = false;
        autoCompleteButton.gameObject.SetActive(false);
        winner = true;
        CancelInvoke("AddTime");
        //Show all cards face
        for (int i = 0; i < deck.allCards.Count; i++)
        {
            deck.allCards[i].showCard = true;
            deck.allCards[i].isClickable = false;
        }

        //Remove cards that are already on top of the pile
        deck.allCards = deck.allCards.Except(topPiles[0].cards).ToList(); 
        deck.allCards = deck.allCards.Except(topPiles[1].cards).ToList();
        deck.allCards = deck.allCards.Except(topPiles[2].cards).ToList();
        deck.allCards = deck.allCards.Except(topPiles[3].cards).ToList();

        //Make cards move to top piles
        while(deck.allCards.Count > 0)
        {
            for (int j = 0; j < deck.allCards.Count; j++)
            {
                Card card = deck.allCards[j];
                if (MoveCardToTopPile(card))
                {
                    deck.allCards.Remove(card);
                    deck.RemoveCardFromDeck(card);
                    for (int z = 0; z < piles.Count; z++)
                    {
                        if (piles[z].ContainsCard(card))
                        {
                            piles[z].RemoveCardFromPile(card);
                            break;
                        }
                    }
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }


    //Undo last movement
    public void Undo()
    {
        UndoMovement.Instance.BackMove();
    }

    public void SetMove(int move)
    {
        this.moves = move;
        moveText.text = $"MOVES: {this.moves}";
    }

    public void StartStopwatch()
    {
        if(!canCountTimer)
        {
            InvokeRepeating("AddTime", 1f, 1f);
            canCountTimer = true;
        }
    }


    //Format the time to display to the player
    public void AddTime()
    {
        time += 1;
        var ss = ((int)(time % 60)).ToString("00");
        var mm = (Mathf.Floor(time / 60) % 60).ToString("00");
        var hh = Mathf.Floor(time / 60 / 60).ToString("00");
        timeText.text = $"TIME {mm}:{ss}";
    }

    public void ResetGame()
    {
        threeMode = Settings.Instance.intToBool(Settings.Instance.threeModeTurnActive);
        winner = false;
        canCountTimer = false;
        time = 0;
        timeText.text = "TIME 00:00";
        CancelInvoke("AddTime");
        SetMove(0);
    }
}
