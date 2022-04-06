using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Pile : MonoBehaviour
{
    public List<Card> cards;

    public Vector3 offset;

    public virtual bool ValidToAddCardToPile(Card card, int pileLength = -1)
    {
        return false;
    }

    public void AddCardToPile(Card card, bool organizePile = false)
    {
        card.transform.SetParent(transform);
        cards.Add(card);
        if (organizePile)
            OrganizePile();
    }

    public void RemoveCardFromPile(Card card, bool organizePile = false)
    {
        cards.Remove(card);
        if (organizePile)
            OrganizePile();
    }

    public bool ContainsCard(Card card)
    {
        return cards.Contains(card);
    }

    //Checks if the pile still has upturned cards
    public bool IsAllCardsShowed()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (!cards[i].showCard)
                return false;
        }
        return true;
    }


    //Get all the child cards of the reference card
    public List<Card> GetCardAndChilds(Card card)
    {
        List<Card> cardAndChilds = new List<Card>();
        int parentIndex = cards.IndexOf(card);
        for (int i = parentIndex; i < cards.Count; i++)
        {
            cardAndChilds.Add(cards[i]);
        }
        return cardAndChilds;
    }


    //Turn over the last card in the pile 
    public void UntapLastCard()
    {
        if(cards.Count > 0 && !cards[cards.Count-1].showCard)
        {
            cards[cards.Count - 1].isClickable = true;
            cards[cards.Count - 1].RotateAndShowCard();
        }
    }


    //Returns all upturned cards
    public List<bool> CardsFace()
    {
        List<bool> cardsFace = new List<bool>();
        for (int i = 0; i < cards.Count; i++)
        {
            cardsFace.Add(cards[i].showCard);
        }
        return cardsFace;
    }


    //Returns all cards that can be clicked
    public List<bool> CardsClicklable()
    {
        List<bool> cardsClickable = new List<bool>();
        for (int i = 0; i < cards.Count; i++)
        {
            cardsClickable.Add(cards[i].isClickable);
        }
        return cardsClickable;
    }

    public void OrganizePile()
    {
        GameController.Instance.canClick = false;
        Sequence sequence = DOTween.Sequence();
        //Arrange the distance of the cards in the pile
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetParent(transform);
            if (i == 0)
                sequence.Append(cards[i].transform.DOMove(transform.position, 0.2f));

            else
            {
                Vector3 newPos = new Vector3(
                    transform.position.x + (i * offset.x),
                    transform.position.y + (i * offset.y),
                    transform.position.z + (i * offset.z));

                sequence.Join(cards[i].transform.DOMove(newPos, 0.2f));
            }
            sequence.OnComplete(() =>
            {
                GameController.Instance.canClick = true;
            });
        }
    }
}

