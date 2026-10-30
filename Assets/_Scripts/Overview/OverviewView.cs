using System;
using TMPro;
using UnityEngine;

public class OverviewView : MonoBehaviour
{
    [SerializeField] private GameObject overviewObject;    
    [SerializeField] private TMP_Text dayText;

    public event Action NextDayRequested;

    void Awake()
    {
        overviewObject.SetActive(false);
    }

    public void Show(int dayNum)
    {
        dayText.text = "Day " + dayNum;
        overviewObject.SetActive(true);
    }

    public void Hide()
    {
        overviewObject.SetActive(false);
    }

    public void OnContinueClicked()
    {
        Hide();
        NextDayRequested?.Invoke();
    }
}