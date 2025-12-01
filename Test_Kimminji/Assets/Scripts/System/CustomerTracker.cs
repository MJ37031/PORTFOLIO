using UnityEngine;

public class CustomerTracker : MonoBehaviour
{
    private CustomerSpawner spawner;

    public void Init(CustomerSpawner spawner)
    {
        this.spawner = spawner;
    }

    private void OnDestroy()
    {
        if (spawner != null)
            spawner.OnCustomerLeft();
    }
}
