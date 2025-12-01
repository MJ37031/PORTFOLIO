using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] customerPrefabs;
    public Transform spawnPoint;
    public float spawnInterval = 5f;
    public int maxCustomers = 5;

    private float timer = 0f;
    private int currentCustomers = 0;

    [Header("Customer Data Pool")]
    public CustomerData[] customerDataOptions;

    [Header("Scene References To Inject")]
    public SaleCounterController saleCounter;
    public Transform exitPoint;
    public Transform eatZoneWaitPoint;
    public Transform trashCanPoint;
    public Transform cashierPoint;
    public EatingZoneManager eatingZoneManager;
    public Transform EatingCashierPoint;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && currentCustomers < maxCustomers)
        {
            SpawnCustomer();
            timer = 0f;
        }
    }

    private void SpawnCustomer()
    {
        CustomerController newCustomer = CreateCustomerInstance();
        if (newCustomer != null)
        {
            newCustomer.SetSaleCounter(saleCounter);
        }
    }

    public void SpawnNextCustomer(int standIndex)
    {
        CustomerController newCustomer = CreateCustomerInstance();
        if (newCustomer != null)
        {
            newCustomer.SetSaleCounter(saleCounter);
            newCustomer.SetStandIndex(standIndex);
        }
    }

    private CustomerController CreateCustomerInstance()
    {
        if (customerPrefabs.Length == 0)
        {
            Debug.LogError("스폰할 손님 프리팹이 등록되지 않았습니다!");
            return null;
        }

        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject prefabToSpawn = customerPrefabs[randomIndex];
        GameObject newCustomerObj = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        currentCustomers++;

        CustomerController controller = newCustomerObj.GetComponent<CustomerController>();
        if (controller != null)
        {
            if (customerDataOptions.Length > 0)
            {
                controller.data = customerDataOptions[Random.Range(0, customerDataOptions.Length)];
            }
            controller.InitializeSceneReferences(exitPoint, eatZoneWaitPoint, trashCanPoint, cashierPoint, eatingZoneManager, EatingCashierPoint);
            newCustomerObj.AddComponent<CustomerTracker>().Init(this);
        }
        return controller;
    }

    public void OnCustomerLeft()
    {
        currentCustomers = Mathf.Max(0, currentCustomers - 1);
    }
}