using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnPile : Pile
{
    //Checks if it is possible to move a card to this pile [Column]
    public override bool ValidToAddCardToPile(Card card, int pileLength = -1)
    {
        //If the pile has no cards and the current card is a KING, then it is valid
        if (pileLength == 0 && card.value == Card.VALUE.KING)
            return true;
        else if (cards.Count == 0)
            return false;


        Card lastCard = cards[cards.Count - 1];

        //Check if they are of different colors and suits
        if (card.suit == Card.SUIT.CLUBS && (lastCard.suit == Card.SUIT.CLUBS || lastCard.suit == Card.SUIT.SPADES))
            return false;
        else if (card.suit == Card.SUIT.SPADES && (lastCard.suit == Card.SUIT.CLUBS || lastCard.suit == Card.SUIT.SPADES))
            return false;
        else if (card.suit == Card.SUIT.HEART && (lastCard.suit == Card.SUIT.HEART || lastCard.suit == Card.SUIT.DIAMONDS))
            return false;
        else if (card.suit == Card.SUIT.DIAMONDS && (lastCard.suit == Card.SUIT.HEART || lastCard.suit == Card.SUIT.DIAMONDS))
            return false;

        //Checks if the value is less than the last card in the pile by 1
        if (((int)card.value < (int)lastCard.value) && ((int)lastCard.value - (int)card.value == 1))
            return true;

        return false;
    }
}
