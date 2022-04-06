using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public enum SUIT 
    {
        DIAMONDS = 1,
        SPADES,
        HEART,
        CLUBS
    }

    public enum VALUE
    {
        ACE=1, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, NOTHING = 0
    }

    public SUIT suit;
    public VALUE value;
    public VALUE auxValue = VALUE.NOTHING;

    private SpriteRenderer spriteRenderer;
    public Sprite frontCard;
    public Sprite backCard;

    public bool showCard;
    public bool hideCard;
    public bool isClickable;

    private void Start()
    {
        gameObject.name = $"{value}_{suit}";
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //Show card face or card back
        CardShowState(showCard);
        if (!showCard)
            isClickable = false;
    }

    public void CardShowState(bool state)
    {
        if (hideCard)
        {
            spriteRenderer.sprite = backCard;
            return;
        }

        if (state)
            spriteRenderer.sprite = frontCard;
        else
            spriteRenderer.sprite = backCard;
    }

    //Do animation shake feedback
    public void ShakeCard()
    {
        GameController.Instance.canClick = false;
        transform.DOShakePosition(0.3f, new Vector3(0.2f,0f,0f), randomness:1).OnComplete(()=> {
            GameController.Instance.canClick = true;
        });
    }

    //Do animation rotate feedback
    public void RotateAndShowCard()
    {
        transform.DORotate(new Vector3(0,90,0), 0.1f, RotateMode.FastBeyond360).SetLoops(2, LoopType.Yoyo);
        showCard = true;
    }


    public void OnMouseDown()
    {
        if (!isClickable || GameController.Instance.winner || !GameController.Instance.canClick || !showCard )
            return;

        //Check if this card can move to another pile
        GameController.Instance.ValidMoveCard(this);
    }

}
