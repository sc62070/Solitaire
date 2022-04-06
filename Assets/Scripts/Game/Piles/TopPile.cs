using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopPile : Pile
{
    //Checks if it is possible to move a card to this pile [Top]
    public override bool ValidToAddCardToPile(Card card, int pileLength = -1)
    {
        //If the pile is empty and the card is an ACE, then it is valid
        if (pileLength == 0 && card.value == Card.VALUE.ACE)
            return true;
        else if (cards.Count == 0)
            return false;


        Card lastCard = cards[cards.Count - 1];


        //Check if they are of the same suit and color
        if (card.suit != lastCard.suit)
            return false;

        //Checks if the value is greater than the last card in the pile by 1
        if (((int)card.value > (int)lastCard.value) && ((int)card.value - (int)lastCard.value == 1))
            return true;

        return false;
    }
}
