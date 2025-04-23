using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Sprite[] cardSprites;
    public GameObject cardPrefab; // 카드 프리팹 연결할 슬롯
    public Transform playerArea;  // 카드 나올 위치
    public Transform dealerArea;

    private bool isPlayerTurn = true;
    private bool gameEnded = false;

    public int playerScore;

    public void DealCardToPlayer()
    {
        if (!isPlayerTurn || gameEnded) return; // ✅ 이미 스탠드 했으면 못 뽑게

        int randIndex = Random.Range(0, cardSprites.Length);
        Sprite selectedCard = cardSprites[randIndex];

        GameObject card = Instantiate(cardPrefab, playerArea);
        card.GetComponent<RectTransform>().localScale = Vector3.one;
        card.GetComponent<Image>().sprite = selectedCard;

        playerScore += GetCardValue(selectedCard.name);
        Debug.Log($"플레이어 카드 뽑음, 현재 점수: {playerScore}");

        if (playerScore > 21)
        {
            gameEnded = true;
            Debug.Log("플레이어 Bust!");
            CheckWinner();
        }
    }
    public void OnStandButton()
    {
        if (!isPlayerTurn || gameEnded) return; // ✅ 이미 끝났으면 못 누르게

        isPlayerTurn = false; // 턴 넘김
        Debug.Log("Stand 눌림! 딜러 턴 시작");
        StartCoroutine(DealerTurn());
    }
    // 딜러 영역

    int GetCardValue(string cardName)
    {
        // 예: "card_2", "card_10", "card_K", "card_A"
        if (cardName.Contains("A"))
            return 11;
        else if (cardName.Contains("K") || cardName.Contains("Q") || cardName.Contains("J"))
            return 10;
        else
        {
            string num = System.Text.RegularExpressions.Regex.Match(cardName, @"\d+").Value;
            return int.Parse(num);
        }
    }



    private List<GameObject> dealerCards = new List<GameObject>();
    private int dealerScore = 0;



    IEnumerator DealerTurn()
    {
        Debug.Log("DealerTurn() 실행됨");

        while (dealerScore < 17)
        {
            yield return new WaitForSeconds(1f);

            int rand = Random.Range(0, cardSprites.Length);
            GameObject card = Instantiate(cardPrefab, dealerArea);
            card.GetComponent<Image>().sprite = cardSprites[rand];

            dealerCards.Add(card);
            int cardValue = GetCardValue(cardSprites[rand].name);
            dealerScore += cardValue;

            Debug.Log($"딜러가 뽑은 카드: {cardSprites[rand].name}, 값: {cardValue}, 현재 점수: {dealerScore}");
        }

        yield return new WaitForSeconds(1f);
        gameEnded = true;
        CheckWinner();
    }


    void CheckWinner()
    {
        string result = "";

        if (playerScore > 21)
            result = "You Bust! Dealer Wins!";
        else if (dealerScore > 21)
            result = "Dealer Busts! You Win!";
        else if (playerScore > dealerScore)
            result = "You Win!";
        else if (playerScore < dealerScore)
            result = "Dealer Wins!";
        else
            result = "It's a Draw!";

        Debug.Log(result);
    }

}

