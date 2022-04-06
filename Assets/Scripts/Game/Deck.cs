using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Deck : MonoBehaviour
{
    public Card cardPrefab;
    public List<Sprite> cardsFront; // Sprites of the cards
    public List<Card> cards; // Deck cards
    public List<Card> cardsShowed; // Cards to show the player
    public List<Card> allCards; 

    public Vector3 offset;
    public Transform cardShowedPosition; 
    public AudioClip swapAudio;

    public void MakeDeck(int quantity = 1)
    {
        Vector3 offsetCard = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
        for (int q = 0; q < quantity; q++)
        {

            int i = 0;
            foreach (Card.SUIT suit in System.Enum.GetValues(typeof(Card.SUIT)))
            {
                foreach (Card.VALUE value in System.Enum.GetValues(typeof(Card.VALUE)))
                {
                    if (value == Card.VALUE.NOTHING)
                        continue;
                    Card card = Instantiate(cardPrefab.gameObject, offsetCard, Quaternion.identity).GetComponent<Card>();
                    card.suit = suit;
                    card.value = value;
                    card.frontCard = cardsFront[i];
                    card.showCard = false;
                    card.isClickable = false;
                    AddCardToDeck(card);
                    allCards.Add(card);
                    i++;
                    offsetCard.y += offset.y;
                    offsetCard.x += offset.x;
                    offsetCard.z += offset.z;
                }
            }
        }

        allCards = allCards.OrderBy(x => x.value).ToList(); // Order all cards by value => A,2,3,4,5,6,7,...,K
    }

    public void ShuffleDeck()
    {
        if (!IsValidCardOperation())
            return;

        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < cards.Count; j++)
            {
                int randomCardIndex = Random.Range(0, cards.Count);
                Card currentCard = cards[j];
                cards[j] = cards[randomCardIndex];
                cards[randomCardIndex] = currentCard;
            }
        }
        OrganizeDeck();
    }

    public void DestroyDeck()
    {
        foreach (Card card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }

    public void RemoveCardFromDeck(Card card)
    {
        if (!cards.Contains(card))
            return;

        if (!IsValidCardOperation())
            return;

        cards.Remove(card);
    }

    public void RemoveCardsBySameValueFromDeck(Card.VALUE value)
    {
        if (!IsValidCardOperation())
            return;

        List<Card> cardsToDestroy = cards.FindAll(item => item.value == value);
        cards.RemoveAll(item => item.value == value);
        foreach (Card card in cardsToDestroy)
        {
            Destroy(card.gameObject);
        }
    }

    public void AddCardToDeck(Card card)
    {
        cards.Add(card);
    }

    public void AddListOfCardToDeck(List<Card> listCards)
    {
        for (int i = 0; i < listCards.Count; i++)
        {
            cards.Add(listCards[i]);
        }
    }

    public Card GetFirstCardFromDeck()
    {
        if (!IsValidCardOperation())
            return null;

        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public Card GetLastCardFromDeck()
    {
        if (!IsValidCardOperation())
            return null;

        Card card = cards.Last();
        cards.Remove(card);
        return card;
    }

    public bool IsValidCardOperation()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("The list is empty");
            return false;
        }

        return true;
    }

    public void ResetDeck()
    {
        DestroyDeck();
    }

    public bool ContainsCardShowed(Card card)
    {
        return cardsShowed.Contains(card);
    }

    public void RemoveFromCardShowed(Card card)
    {
        cardsShowed.Remove(card);
        if (cardsShowed.Count > 0)
            cardsShowed[cardsShowed.Count - 1].isClickable = true;
    }

    //Organize cards from deck by offset
    public void OrganizeDeck()
    {
        Vector3 offsetCard = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].gameObject.SetActive(true);
            cards[i].showCard = false;
            cards[i].isClickable = false;
            cards[i].transform.DOMove(offsetCard, 0.1f);
            offsetCard.y += offset.y;
            offsetCard.x += offset.x;
            offsetCard.z += offset.z;
        }
    }

    //Rearranges the cards shown to the player after rewinding a move
    public void OrganizeShowedCard()
    {
        StartCoroutine(MoveCardToOrganizeDeck());
    }

    public IEnumerator MoveCardToOrganizeDeck()
    {
        //Rearranges the cards in Three Mode
        if (GameController.Instance.threeMode)
        {
            int index = 0;
            if (cardsShowed.Count <= 3)
                index = 0;
            else
                index = cardsShowed.Count - 3;

            for (int i = 0; i < cardsShowed.Count; i++)
                cardsShowed[i].gameObject.SetActive(false);

            OrganizeDeck();
            Vector3 pos = cardShowedPosition.position;
            int offset = 0;
            for (int i = index; i < cardsShowed.Count; i++)
            {
                cardsShowed[i].transform.DOMove(new Vector3(pos.x + (offset * 0.4f), pos.y, pos.z + (offset * -0.01f)), 0.1f);
                cardsShowed[i].gameObject.SetActive(true);
                offset++;
            }
        }
        //Rearranges the cards in One Mode
        else
        {
            if (cards.Count > 0)
            {
                Card lastCard = cards[cards.Count - 1];
                lastCard.transform.position = new Vector3(lastCard.transform.position.x, lastCard.transform.position.y, transform.position.z - 0.01f);
            }
            yield return new WaitForSeconds(0.01f);
            OrganizeDeck();
            yield return new WaitForSeconds(0.01f);

            for (int i = cardsShowed.Count - 1; i >= 0; i--)
            {
                cardsShowed[i].transform.DOMove(new Vector3(cardShowedPosition.position.x, cardShowedPosition.position.y, cardShowedPosition.position.z + (i * -0.01f)), 0.2f);
            }
        }
    }

    //Allow only the last card of the cards shown to be clicked
    public void ValidCardsClickable()
    {
        for (int i = 0; i < cardsShowed.Count; i++)
        {
            cardsShowed[i].isClickable = false;
            if (i == cardsShowed.Count - 1)
                cardsShowed[i].isClickable = true;
        }
    }

    //When the player clicks to remove cards from the deck
    public void OnMouseDown()
    {
        if (!GameController.Instance.canClick || GameController.Instance.winner)
            return;

        EffectPlayer.instance.PlayOneShotSong(swapAudio);
        GameController.Instance.StartStopwatch();
        GameController.Instance.canClick = false;
        UndoMovement.Instance.AddMove();

        //Show three cards at once [Three Mode Turn]
        if (GameController.Instance.threeMode)
        {
            //If the deck still has cards to display
            if (cards.Count > 0)
            {
                //Get cards from deck to show
                for (int i = 0; i < 3; i++)
                {
                    Card card = GetLastCardFromDeck();
                    if (card != null)
                    {
                        cardsShowed.Add(card);
                    }
                }

                //Takes the index of the first card to display the next three cards
                int index = 0;
                if (cardsShowed.Count <= 3)
                    index = 0;
                else
                    index = cardsShowed.Count - 3;

                //Disables the cards that the player will not see
                for (int i = 0; i < cardsShowed.Count; i++)
                    cardsShowed[i].gameObject.SetActive(false);


                //Enables and organizes the distance of the cards to the player
                Vector3 pos = cardShowedPosition.position;
                int offset = 0;
                for (int i = index; i < cardsShowed.Count; i++)
                {
                    cardsShowed[i].transform.DOMove(new Vector3(pos.x + (offset * 0.4f), pos.y, pos.z + (offset * -0.01f)), 0.1f);
                    cardsShowed[i].gameObject.SetActive(true);
                    cardsShowed[i].showCard = true;
                    cardsShowed[i].isClickable = false;

                    if (i == cardsShowed.Count - 1)
                        cardsShowed[i].isClickable = true;

                    offset++;
                }
                GameController.Instance.canClick = true;
            }
            //If the deck does not yet have cards to display
            else
            {
                //Return all cards to deck
                for (int i = cardsShowed.Count - 1; i >= 0; i--)
                {
                    cardsShowed[i].gameObject.SetActive(true);
                    cardsShowed[i].showCard = false;
                    cardsShowed[i].isClickable = false;
                    AddCardToDeck(cardsShowed[i]);
                }
                cardsShowed.Clear();
                OrganizeDeck();
                GameController.Instance.canClick = true;
            }
        }
        //Show one card [One Mode Turn]
        else
        {
            //If the deck still has cards to display, take the last card from the deck and show it to the player
            if (cards.Count > 0)
            {
                Card card = GetLastCardFromDeck();

                Vector3 posClicked = cardShowedPosition.position;
                if (cardsShowed.Count == 0)
                    posClicked = cardShowedPosition.position;
                else
                    posClicked = cardsShowed[cardsShowed.Count - 1].transform.position;
                posClicked.z -= 0.1f;

                Sequence sequence = DOTween.Sequence();
                sequence.Append(card.transform.DOMove(posClicked, 0.1f));
                sequence.AppendInterval(0.05f);
                sequence.OnComplete(() =>
                {
                    for (int i = cardsShowed.Count - 1; i >= 0; i--)
                    {
                        cardsShowed[i].transform.position = new Vector3(cardShowedPosition.position.x, cardShowedPosition.position.y, cardShowedPosition.position.z + (i * -0.01f));
                        cardsShowed[i].isClickable = false;
                    }
                    card.isClickable = true;
                    card.showCard = true;
                    GameController.Instance.canClick = true;
                });
                cardsShowed.Add(card);
            }
            //If the deck does not yet have cards to display
            else
            {
                //Return all cards to deck
                for (int i = cardsShowed.Count - 1; i >= 0; i--)
                {
                    cardsShowed[i].showCard = false;
                    cardsShowed[i].isClickable = false;
                    AddCardToDeck(cardsShowed[i]);
                }
                cardsShowed.Clear();
                OrganizeDeck();
                GameController.Instance.canClick = true;
            }
        }
    }


    //returns the list of card states in the deck (tapped or untapped) to be stored by UndoMovement.
    public List<bool> CardsFace()
    {
        List<bool> cardsFace = new List<bool>();
        for (int i = 0; i < cards.Count; i++)
        {
            cardsFace.Add(cards[i].showCard);
        }
        return cardsFace;
    }

    //returns the list of card states in the deck (clicklable) to be stored by UndoMovement.
    public List<bool> CardsClicklable()
    {
        List<bool> cardsClickable = new List<bool>();
        for (int i = 0; i < cards.Count; i++)
        {
            cardsClickable.Add(cards[i].isClickable);
        }
        return cardsClickable;
    }

    //returns the list of card states shown (tapped or untapped) to be stored by UndoMovement.
    public List<bool> CardsClickedFace()
    {
        List<bool> cardsFace = new List<bool>();
        for (int i = 0; i < cardsShowed.Count; i++)
        {
            cardsFace.Add(cardsShowed[i].showCard);
        }
        return cardsFace;
    }

    //returns the list of card states shown (clicklable) to be stored by UndoMovement.
    public List<bool> CardsClickedClicklable()
    {
        List<bool> cardsClickable = new List<bool>();
        for (int i = 0; i < cardsShowed.Count; i++)
        {
            cardsClickable.Add(cardsShowed[i].isClickable);
        }
        return cardsClickable;
    }
}

