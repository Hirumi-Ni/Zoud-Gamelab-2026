using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private Image customerSprite;
    [SerializeField] private float delay = 1.5f;   

    private Coroutine leaveRoutine;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(SO_Customer data)
    {
        if (leaveRoutine != null)
        {
            StopCoroutine(leaveRoutine);
            leaveRoutine = null;
        }

        customerSprite.sprite = data.customerSprite;
        gameObject.SetActive(true);
    }


    public IEnumerator Leave()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(delay);
    }
}