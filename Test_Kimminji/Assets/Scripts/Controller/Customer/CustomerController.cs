using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum CustomerState
{
    Spawning,
    WalkingToStand,
    Waiting,
    Buying,
    WalkingToCashier,
    WaitingToPay,
    Paying,
    Leaving,

    // EatingCustomer
    WalkingToEatZone,
    WaitingInEatZone,   
    WalkingToSeat,     
    Eating,             
    WalkingToTrashCan,  
    ThrowingTrash       
}

public enum CustomerType
{
    TakeOut, 
    EatingIn      // 새로운 손님 (먹고 가기)
}


[RequireComponent(typeof(NavMeshAgent))]
public class CustomerController : MonoBehaviour
{
    #region 

    [Header("1. Data & Type")]
    public CustomerData data;
    public CustomerType customerType;

    [Header("2. Scene Points & References")]
    public Transform ExitPoint;  // 퇴장 위치
    public Transform cashierPoint; // 계산대 위치
    public Transform eatZoneWaitPoint; // Eating 손님 대기 위치
    public Transform trashCanPoint;    // Eating 손님 쓰레기통 위치
    public EatingZoneManager eatingZoneManager;
    public Transform eatingCashierPoint;

    [Header("3. Stack & Animation Settings")]
    public Transform stackPoint;
    public Transform bagHoldPoint; // 봉투를 드는 위치
    public float stackOffset = 0.25f;
    public float moveDuration = 0.25f;
    public float grabInterval = 0.3f;
    public int maxStack = 8;
    public AudioSource getbreadsound;
    public GameObject bagPrefab;
    private GameObject currentBag; // 현재 들고 있는 봉투
    public GameObject trashPrefab;
    public AudioSource trashSound;

    [Header("Animation Timings")]
    public float sitDownAnimLength = 0.6f; 
    public float standUpAnimLength = 0.6f; 

    #endregion


    #region 
    public NavMeshAgent agent { get; private set; }
    public CustomerView view { get; private set; }
    public CustomerState currentState { get; private set; }
    public SaleCounterController targetCounter { get; private set; }
    public Transform myStandPoint { get; private set; }
    public int collectedCount { get; private set; }
    #endregion


    #region
    private ICustomerBehavior behavior;

    // 계산대
    private CashierController cashier;
    private CashierQueueManager queueManager;
    private int queueIndex = -1;
    //private bool isPaying = false;
    private GameObject bag;

    // 빵
    private List<GameObject> carriedBreads = new List<GameObject>();
    private Queue<GameObject> breadQueue = new Queue<GameObject>();
    private bool isAnimating = false;

    // 판매대
    private int standIndex = -1;

    // 식당
    private Transform assignedSeat; // 배정받은 내 자리
    #endregion



    public void InitializeSceneReferences(
        Transform exit, Transform eatZone, 
        Transform trashCan, Transform cashier, 
        EatingZoneManager eatingZone, Transform EatingCashier)
    {
        this.ExitPoint = exit;
        this.eatZoneWaitPoint = eatZone;
        this.trashCanPoint = trashCan;
        this.cashierPoint = cashier;
        this.eatingZoneManager = eatingZone;
        this.eatingCashierPoint = EatingCashier;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        view = GetComponent<CustomerView>();
        agent.speed = data.moveSpeed;

        // 손님 타입에 따라 최초 행동 전략을 설정
        if (customerType == CustomerType.EatingIn)
        {
            behavior = new EatingInBehavior();
        }
        else
        {
            behavior = new TakeOutBehavior();
        }
    }

    private void Start()
    {
         cashier = FindObjectOfType<CashierController>();

         queueManager = FindObjectOfType<CashierQueueManager>();


        GameObject exitPointObj = GameObject.FindGameObjectWithTag("ExitPoint");
        if (exitPointObj != null)
            ExitPoint = exitPointObj.transform;
        else
            Debug.LogError("씬에 'ExitPoint' 태그를 가진 오브젝트가 없습니다!");


        GameObject cashierPointObj = GameObject.FindGameObjectWithTag("CashierPoint");
        if (cashierPointObj != null)
            cashierPoint = cashierPointObj.transform;
        else
            Debug.LogError("씬에 'CashierPoint' 태그를 가진 오브젝트가 없습니다!");


    }

    private void Update()
    {
        // Leaving 상태일 때 강제 제거 로직
        if (currentState == CustomerState.Leaving && agent != null)
        {
            if (!agent.pathPending && agent.remainingDistance <= 0.2f && !agent.isStopped)
            {
                Destroy(gameObject);
            }
        }

        // NavMeshAgent 도착 감지 로직
        if (agent.hasPath && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.ResetPath();
            if (behavior != null)
            {
                behavior.OnArriveAtDestination(this);
            }
        }
    }

    // -------------------------
    // 판매대 자리 배정
    // -------------------------
    public void SetSaleCounter(SaleCounterController counter)
    {
        targetCounter = counter;
        myStandPoint = targetCounter.AssignStandPoint(this);

        if (myStandPoint != null)
        {
            behavior.Enter(this); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartPayment(CashierController cashier)
    {
        SetState(CustomerState.Paying);
        StartCoroutine(PayRoutine(cashier));
    }

    // 2. PayRoutine
    private IEnumerator PayRoutine(CashierController cashier) 
    {
        EconomyModel.I.AddMoney(data.buyAmount * targetCounter.GetPrice());
        MoneyStackerManager.I.AddMoney(this.customerType, data.buyAmount);

        Debug.Log("[Customer] 계산 완료");

        view.HideDesiredItem();
        view.ShowHappyEffect();

        if (queueManager != null)
        {
            queueManager.Dequeue();
        }

        cashier.OnPaymentComplete(this);

        yield return new WaitForSeconds(0.1f);

        SetState(CustomerState.Leaving);
        agent.SetDestination(ExitPoint.position);
    }

    public void SetStandIndex(int index) { standIndex = index; }
    private SaleCounterController currentCounter;

    private void OnTriggerEnter(Collider other)
    {

        if (currentState == CustomerState.Leaving && other.CompareTag("ExitPoint"))
        {
            Debug.Log("[Customer] 출구 도착 → 제거");
            Destroy(gameObject);
        }
    }


    private void OnTriggerStay(Collider other)
    {

        if (currentState == CustomerState.Waiting)
        {
            var counter = other.GetComponentInParent<SaleCounterController>();
            if (counter != null)
            {

                if (counter == this.targetCounter)
                {
                    bool hasBread = counter.HasBread();

                    if (hasBread)
                    {
                        StopAllCoroutines();
                        StartCoroutine(BuyRoutine());
                    }
                }
            }
        }
    }

    public IEnumerator BuyRoutine()
    {
        SetState(CustomerState.Buying);
        yield return new WaitForSeconds(0.2f);

        while (collectedCount < data.buyAmount && carriedBreads.Count < maxStack)
        {
            GameObject bread = targetCounter.TakeOneBread();
            if (bread != null)
            {
                PrepareBread(bread);
                if (getbreadsound != null) getbreadsound.PlayOneShot(getbreadsound.clip);
                if (!isAnimating) StartCoroutine(ProcessQueue());
                collectedCount++;
                view.TakeOneItem();
                yield return new WaitForSeconds(grabInterval);
            }
            else
            {
                SetState(CustomerState.Waiting);
                yield break;
            }
        }

        if (collectedCount >= data.buyAmount)
        {
            behavior.DecideNextActionAfterBuying(this);
        }

        yield break;
    }

    public void GoToCashier()
    {
        if (queueManager != null)
        {
            queueIndex = queueManager.Enqueue(this);
            agent.SetDestination(queueManager.GetQueuePosition(queueIndex));
            SetState(CustomerState.WalkingToCashier);
        }
        else if (cashier != null)
        {
            agent.SetDestination(cashier.transform.position);
            SetState(CustomerState.WalkingToCashier);
        }
        else
        {
            Debug.LogError("[Customer] 계산대 없음!");
        }
    }

    public IEnumerator PackBreadsIntoBag(GameObject bag)
    {
        if (bag != null)
        {
            currentBag = bag;
            bag.transform.SetParent(bagHoldPoint);
            bag.transform.localPosition = Vector3.zero;
            bag.transform.localRotation = Quaternion.identity;
        }

        float duration = 0.2f;
        foreach (var bread in carriedBreads)
        {
            Vector3 startPos = bread.transform.position;
            Vector3 endPos = bag.transform.position;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                bread.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null; 
            }
            bread.SetActive(false);
        }

        ClearCarriedBreads();
    }


    private IEnumerator PayRoutine()
    {
        EconomyModel.I.AddMoney(data.buyAmount * targetCounter.GetPrice());
        Debug.Log("[Customer] 계산 완료");

        view.HideDesiredItem();
        view.ShowHappyEffect();

        if (queueManager != null)
        {
            queueManager.Dequeue();
        }
        cashier.OnPaymentComplete(this);

        yield return new WaitForSeconds(0.3f);

        SetState(CustomerState.Leaving);
        agent.SetDestination(ExitPoint.position);
    }


    private void PrepareBread(GameObject bread)
    {
        if (bread.TryGetComponent<Rigidbody>(out var rb)) Destroy(rb);
        if (bread.TryGetComponent<Collider>(out var col)) col.enabled = false;

        bread.transform.SetParent(stackPoint);
        bread.transform.localRotation = Quaternion.Euler(0, 90, 0);
        bread.transform.localPosition = Vector3.zero;

        breadQueue.Enqueue(bread);
    }

    private IEnumerator ProcessQueue()
    {
        isAnimating = true;

        while (breadQueue.Count > 0)
        {
            GameObject bread = breadQueue.Dequeue();

            Vector3 startPos = bread.transform.localPosition;
            Vector3 targetPos = Vector3.up * stackOffset * carriedBreads.Count;

            float elapsed = 0f;
            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveDuration);
                t = 1f - Mathf.Pow(1f - t, 3f);
                bread.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            bread.transform.localPosition = targetPos;
            carriedBreads.Add(bread);

            if (view != null)
                view.SetAnimation(currentState, true);
        }

        isAnimating = false;
    }

    public void OnSeatAssigned(Transform seat)
    {
        Debug.LogAssertion($"[CustomerController] {name}이(가) {seat.name} 자리를 배정받았습니다. 이동을 시작합니다.");
        assignedSeat = seat;
        agent.SetDestination(seat.position);
        SetState(CustomerState.WalkingToSeat);
    }

    public void SetState(CustomerState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        Debug.Log($"[Customer] 상태 변경: {newState}");

        view.SetAnimation(currentState, carriedBreads.Count > 0);

        switch (newState)
        {
            // idle
            case CustomerState.Waiting:
            case CustomerState.WaitingToPay:
            case CustomerState.Eating:
                agent.isStopped = true;
                break;

            case CustomerState.Paying:
                agent.isStopped = true; 
                StartCoroutine(PayRoutine());
                break;

            // walk
            default:
                agent.isStopped = false;
                break;
        }
    }


    // EatimgCustomer
    public IEnumerator EatRoutine()
    {

        SetState(CustomerState.Eating);
        yield return new WaitForSeconds(sitDownAnimLength);

   
        Debug.Log("[Customer] 식사 중...");
        foreach (var bread in carriedBreads) bread.SetActive(false);
        ClearCarriedBreads();
        yield return new WaitForSeconds(5.0f); 


        yield return StartCoroutine(ThrowTrashRoutine());


        if (eatingZoneManager != null && assignedSeat != null)
        {
            eatingZoneManager.ReleaseSeat(assignedSeat);
        }


        SetState(CustomerState.WalkingToCashier);
        yield return new WaitForSeconds(standUpAnimLength); 


        GoToDineInCashier();
    }

    public IEnumerator ThrowTrashRoutine()
    {
        SetState(CustomerState.ThrowingTrash);
        view.ShowTrashEffect();
        Debug.Log("[Customer] 앉은 자리에서 쓰레기 버리는 중...");

        trashSound.PlayOneShot(trashSound.clip);

        yield return new WaitForSeconds(0.5f);

        if (trashPrefab != null)
        {
            Instantiate(trashPrefab, trashCanPoint.position, Quaternion.identity);
        }
    }

    public void GoToDineInCashier()
    {
        SetState(CustomerState.WalkingToCashier);
        agent.SetDestination(eatingCashierPoint.position);
    }

    public void MoveToQueuePoint(Vector3 pos)
    {
        if (agent != null)
        {
            agent.isStopped = false;  
            agent.SetDestination(pos);
        }
    }

    public void PayImmediatelyAndLeave()
    {
        SetState(CustomerState.Paying);
        StartCoroutine(PayImmediatelyRoutine());
    }

    private IEnumerator PayImmediatelyRoutine()
    {

        EconomyModel.I.AddMoney(data.buyAmount * targetCounter.GetPrice());

        MoneyStackerManager.I.AddMoney(this.customerType, data.buyAmount);


        view.HideDesiredItem();
        view.ShowHappyEffect();

  
        yield return new WaitForSeconds(0.5f);

        // 4. 퇴장 상태로 전환하고 출구로 이동
        SetState(CustomerState.Leaving);
        agent.SetDestination(ExitPoint.position);
    }


    public int GetStandIndex() => standIndex;

    public IList<GameObject> GetCarriedBreads() => carriedBreads;
    public void ClearCarriedBreads() => carriedBreads.Clear();
}