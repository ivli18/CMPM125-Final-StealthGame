using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public int value = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PlaySFX(AudioManager.SFXType.Collected);
            ScoreManager.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
}
