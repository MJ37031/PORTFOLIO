using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    // State에 Ready(준비) 단계를 추가하여 동기화를 확실하게 합니다.
    enum State { Start = 0, Ready, Game, End }
    enum Turn { MyTurn = 0, YourTurn }
    enum CardState { FaceDown = 0, FaceUp, Matched }

    [Header("네트워크 UI")]
    public TMP_InputField ip;
    public GameObject networkPanel;

    [Header("게임 UI 요소")]
    public Image[] cards;
    public TMP_Text myScoreText;
    public TMP_Text yourScoreText;
    public TMP_Text resultText;
    public TMP_Text turnIndicatorText;
    public GameObject whoseTurn;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject samePanel;

    [Header("카드 이미지 (스프라이트)")]
    public Sprite spriteCardBack;
    public Sprite[] spriteCardFaces;

    private Tcp tcp;
    private State state;
    private Turn currentTurn;
    private int[] cardValues = new int[8];
    private CardState[] cardStates = new CardState[8];
    private List<int> openedCards = new List<int>();
    private bool isChecking = false;
    private int myScore = 0;
    private int yourScore = 0;
    private int matchedPairsCount = 0;


    void Start()
    {
        tcp = GetComponent<Tcp>();
        state = State.Start;
        resultText.gameObject.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].gameObject.SetActive(false);
        }
    }

    public void ServerStart() { tcp.StartServer(10000, 10); }
    public void ClientStart() { tcp.Connect(ip.text, 10000); }

    void Update()
    {
        if (state == State.Start && tcp.IsConnect())
        {
            state = State.Ready;
            networkPanel.SetActive(false);
            turnIndicatorText.text = "Wait...";
        }

        // Ready 상태에서 동기화 처리
        if (state == State.Ready)
        {
            UpdateReady();
        }

        // Game 상태에서 상대방 입력 처리
        if (state == State.Game)
        {
            UpdateGame();
        }
    }

    // [수정] 동기화를 위한 Ready 상태 로직
    void UpdateReady()
    {
        if (tcp.IsServer())
        {
            // 서버는 클라이언트가 접속했는지 IsConnect()로 확인합니다.
            // GetClientCount() > 0 대신 IsConnect()를 사용합니다.
            if (tcp.IsConnect())
            {
                // 클라이언트가 접속하면 카드 덱을 생성하고 딱 한 번만 전송
                int[] deck = new int[] { 0, 0, 1, 1, 2, 2, 3, 3 };
                System.Random random = new System.Random();
                cardValues = deck.OrderBy(x => random.Next()).ToArray();
                byte[] data = cardValues.Select(val => (byte)val).ToArray();
                tcp.Send(data, data.Length);

                // 서버는 바로 게임 시작
                StartGame(Turn.MyTurn);
            }
        }
        else // 클라이언트
        {
            // 클라이언트는 서버로부터 카드 덱 정보를 받을 때까지 계속 대기
            byte[] data = new byte[8];
            if (tcp.Receive(ref data, data.Length) > 0)
            {
                cardValues = data.Select(val => (int)val).ToArray();

                // 데이터를 받으면 게임 시작
                StartGame(Turn.YourTurn);
            }
        }
    }

    // [추가] 게임 시작 공통 함수
    void StartGame(Turn startTurn)
    {
        state = State.Game; // 상태를 Game으로 변경
        currentTurn = startTurn;

        // 모든 카드를 보이게 하고 UI 초기화
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].gameObject.SetActive(true);
            cardStates[i] = CardState.FaceDown;
        }

        UpdateAllCardsUI();
        UpdateScoreUI();
        UpdateTurnIndicator();
    }

    // (UpdateGame, OnCardClicked, FlipCard, CheckForMatch 등 나머지 함수는 이전과 동일합니다)

    void UpdateGame()
    {
        if (currentTurn == Turn.YourTurn)
        {
            byte[] data = new byte[1];
            if (tcp.Receive(ref data, data.Length) > 0)
            {
                int cardIndex = (int)data[0];
                FlipCard(cardIndex);
            }
        }
    }

    public void OnCardClicked(int cardIndex)
    {
        if (currentTurn != Turn.MyTurn || cardStates[cardIndex] != CardState.FaceDown || isChecking)
        {
            return;
        }

        byte[] data = new byte[1] { (byte)cardIndex };
        tcp.Send(data, data.Length);
        FlipCard(cardIndex);
    }

    void FlipCard(int cardIndex)
    {
        if (cardStates[cardIndex] != CardState.FaceDown) return;

        cardStates[cardIndex] = CardState.FaceUp;
        openedCards.Add(cardIndex);
        UpdateAllCardsUI();

        if (openedCards.Count == 2)
        {
            StartCoroutine(CheckForMatch());
        }
    }

    IEnumerator CheckForMatch()
    {
        isChecking = true;
        int card1_idx = openedCards[0];
        int card2_idx = openedCards[1];
        yield return new WaitForSeconds(1.0f);

        if (cardValues[card1_idx] == cardValues[card2_idx]) // 짝이 맞으면
        {
            cardStates[card1_idx] = CardState.Matched;
            cardStates[card2_idx] = CardState.Matched;

            if (currentTurn == Turn.MyTurn) myScore++;
            else yourScore++;

            matchedPairsCount++;
            UpdateScoreUI();
        }
        else // 짝이 다르면
        {
            cardStates[card1_idx] = CardState.FaceDown;
            cardStates[card2_idx] = CardState.FaceDown;
        }

        currentTurn = (currentTurn == Turn.MyTurn) ? Turn.YourTurn : Turn.MyTurn;
        UpdateTurnIndicator();

        openedCards.Clear();
        UpdateAllCardsUI();
        isChecking = false;

        if (matchedPairsCount == 4)
        {
            EndGame();
        }
    }

    void UpdateScoreUI()
    {
        myScoreText.text = "Score " + myScore;
        yourScoreText.text = "You: " + yourScore;
    }

    void UpdateTurnIndicator()
    {
        if (state != State.Game) return;
        turnIndicatorText.text = (currentTurn == Turn.MyTurn) ? "My Turn" : "Wait...";
    }

    void EndGame()
    {
        state = State.End;
        // resultText.gameObject.SetActive(true);
        whoseTurn.SetActive(false);
        if (myScore > yourScore)
        {
            //resultText.text = "Win!";
            winPanel.SetActive(true);
        }
        else if (yourScore > myScore)
        {
            //resultText.text = "Lose...";
            losePanel.SetActive(true);
        }
        else
        {
            //resultText.text = "Same";
            samePanel.SetActive(true);
        }
    }

    void UpdateAllCardsUI()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cardStates[i] == CardState.FaceDown)
            {
                cards[i].sprite = spriteCardBack;
                cards[i].color = Color.white;
                cards[i].GetComponent<Button>().interactable = true;
            }
            else if (cardStates[i] == CardState.FaceUp)
            {
                cards[i].sprite = spriteCardFaces[cardValues[i]];
                cards[i].color = Color.white;
            }
            else // Matched
            {
                cards[i].color = new Color(0, 0, 0, 0);
                cards[i].GetComponent<Button>().interactable = false;
            }
        }
    }
}