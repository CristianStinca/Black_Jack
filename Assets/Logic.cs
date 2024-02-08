using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Logic : MonoBehaviour
{
    public Card[] cards = new Card[52];
    public Chips[] chips = new Chips[5];
    private List<int> usedCards = new List<int>();
    private List<int> currentUserCards = new List<int>();
    private List<int> currentCasinoCards = new List<int>();
    public Transform[] cardSlots;
    public int[] availableSlots;

    public Text currentScoreText;
    public Text moneyText;
    public Text tryAgain;
    public Text endScreenText;
    public InputField inputText;
    public Canvas MainScreen;
    public Canvas BettingScreen;
    public Canvas EndingScreen;
    private int currentScore = 0;
    private int casinoCurrentScore = 0;
    private int rotatedCardInd = 0;
    private string input;
    //private bool betActive = false;
    private int playerBet;

    private int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };

    // Start is called before the first frame update
    void Start()
    {
        inputText.Select();
    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = "You have: " + PlayerPrefs.GetInt("Sum", 500).ToString();
    }

    public void startDraft()
    {
        //if (!betActive) return;
        spawnCard(randomizeCardInd(), 1, false);
        spawnCard(randomizeCardInd(), 1, true);
        spawnCard(randomizeCardInd(), 2, false);
        spawnCard(randomizeCardInd(), 2, false);
    }

    public void hit()
    {
        //if (!betActive) return;
        spawnCard(randomizeCardInd(), 2, false);
    }

    public void evaluateScore()
    {
        //if (!betActive) return;
        rotateCard(cards[rotatedCardInd]);
        
        while (casinoCurrentScore < 17)
        {
            spawnCard(randomizeCardInd(), 1, false);
        }

        if (currentScore < 21)
        {
            if ((currentScore > casinoCurrentScore && casinoCurrentScore <= 21) || (casinoCurrentScore > 21))
            {
                Debug.Log("You won!");
                win();
            }
            else if (currentScore == casinoCurrentScore) 
            {
                Debug.Log("It's a draw!");
                draw();
            }
            else
            {
                Debug.Log("You lost!");
                lose();
            }
        }
        else if (currentScore == 21)
        {
            Debug.Log("You won 21!");
            win21();
        }
        else
        {
            Debug.Log("You lost!");
            lose();
        }

        MainScreen.gameObject.SetActive(false);
        EndingScreen.gameObject.SetActive(true);
    }

    private int calculateScore(List<int> l)
    {
        int sum = l.Sum();

        while (true)
        {
            if (l.Contains(1))
            {
                if (sum + 9 <= 21)
                {
                    sum += 9;
                } 
                else
                {
                    return sum;
                }
            }
            else
            {
                return sum;
            }
        }
    }

    private void spawnCard(int ind, int slot, bool isUpDown)
    {
        Vector3 myVector;
        if (slot == 1)
            myVector = new Vector3(0.04f, 0.0001f, 0f);
        else
            myVector = new Vector3(0.012f, 0.0001f, 0.02f);

        slot -= 1;
        cards[ind].transform.position = cardSlots[slot].position;
        
        if (availableSlots[slot] > 0)
        {
            for (int i = 0; i < availableSlots[slot]; i++)
            {
                cards[ind].transform.position += myVector;
            }
        }

        if (isUpDown)
        {
            rotateCard(cards[ind]);
            rotatedCardInd = ind;
        }
        
        availableSlots[slot]++;
        cards[ind].gameObject.SetActive(true);
        usedCards.Add(ind);

        if (slot == 1)
        {
            //currentScore += values[ind % 13];
            //currentScoreText.text = "Your score is: " + currentScore.ToString();

            currentUserCards.Add(values[ind % 13]);
            currentScore = calculateScore(currentUserCards);
            currentScoreText.text = "Your score is: " + calculateScore(currentUserCards).ToString();
        }
        else
        {
            //casinoCurrentScore += values[ind % 13];
            
            currentCasinoCards.Add(values[ind % 13]);
            casinoCurrentScore = calculateScore(currentCasinoCards);
        }

    }

    private void rotateCard(Card card)
    {
        card.transform.eulerAngles += new Vector3(0, 0, 180);
    }

    private int randomizeCardInd()
    {
        int i = Random.Range(0, cards.Length);
        if (usedCards.Contains(i))
        {
            i = randomizeCardInd();
        }
        return i;
    }

    public void gameEnd()
    {
        //if (!betActive) return;
        BettingScreen.gameObject.SetActive(true);
        EndingScreen.gameObject.SetActive(false);
        inputText.Select();

        for (int i = 0; i < usedCards.Count; i++)
        {
            cards[usedCards[i]].gameObject.SetActive(false);
        }

        for (int i = 0; i < availableSlots.Length; i++)
        {
            availableSlots[i] = 0;
        }

        currentScore = 0;
        casinoCurrentScore = 0;
        usedCards.Clear();
        currentUserCards.Clear();
        currentCasinoCards.Clear();

        //SceneManager.LoadScene(0);

    }

    private void win()
    {
        PlayerPrefs.SetInt("Sum", PlayerPrefs.GetInt("Sum", 500) + playerBet * 2);
        endScreenText.text = "You won!";
    }

    private void win21()
    {
        PlayerPrefs.SetInt("Sum", PlayerPrefs.GetInt("Sum", 500) + System.Convert.ToInt32(playerBet * 2.5f));
        endScreenText.text = "You won!";
    }

    private void draw()
    {
        PlayerPrefs.SetInt("Sum", PlayerPrefs.GetInt("Sum", 500) + playerBet);
        endScreenText.text = "It's  a draw!";
    }

    private void lose()
    {
        endScreenText.text = "You lost!";
    }

    public void toMainScreen()
    {
        SceneManager.LoadScene("Open");
    }

    public void ReadStringInput(string s)
    {
        input = s;
        if (!int.TryParse(input, out playerBet))
        {
            Debug.Log("Invalid input.");
            tryAgain.gameObject.SetActive(true);
            inputText.text = "";
            return;
        }
        tryAgain.gameObject.SetActive(false);
        inputText.text = "";
        //betActive = true;
        PlayerPrefs.SetInt("Sum", PlayerPrefs.GetInt("Sum", 500) - playerBet);
        BettingScreen.gameObject.SetActive(false);
        MainScreen.gameObject.SetActive(true);

        startDraft();
    }

    public void createChips()
    {
        int sum = PlayerPrefs.GetInt("Sum", 500);
        List<Chips> chipsToAdd = new List<Chips>();

        while (sum != 0)
        {
            if (sum - 100 >= 0)
            {
                sum -= 100;
                chipsToAdd.Add(chips[0]);
            } 
            else if (sum - 25 >= 0)
            {
                sum -= 25;
                chipsToAdd.Add(chips[1]);
            }
            else if (sum - 10 >= 0)
            {
                sum -= 10;
                chipsToAdd.Add(chips[2]);
            }
            else if (sum - 5 >= 0)
            {
                sum -= 5;
                chipsToAdd.Add(chips[3]);
            }
            else if (sum - 1 >= 0)
            {
                sum -= 1;
                chipsToAdd.Add(chips[4]);
            }
            else
            {
                Debug.Log("Chip calculation error.");
            }
        }


    }
}
