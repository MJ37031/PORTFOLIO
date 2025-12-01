using UnityEngine;

public class Trash : MonoBehaviour
{
    public ParticleSystem cleanEffect;
    public AudioSource cleanSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어가 쓰레기를 주웠습니다.");

            ShowCleanEffect();
            cleanSound.PlayOneShot(cleanSound.clip);

            Destroy(gameObject);
        }
    }



    public void ShowCleanEffect(float duration = 1.5f)
    {
        if (cleanEffect != null)
        {
            cleanEffect.Play();

        }
    }
}