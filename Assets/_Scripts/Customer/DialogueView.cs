using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueView : MonoBehaviour //ini nanti bakal banyak yg mau kurevisi
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueObject;       
    [SerializeField] private TMP_Text customerNameText;
    [SerializeField] private TMP_Text customerDialogueText;

    [Header("Lines")]
    [SerializeField] private string[] emptyOrderLines = { "kosongggg?" };
    [SerializeField] private string[] thanksLines = { "Terima kasih." };
    [SerializeField] private string wrongItemLine = "Loh, aku gak pesan ini.";

    [Header("Timing")]
    [SerializeField] private float noticeDuration = 1.5f;

    private bool advanceRequested;
    private string orderLine = "";
    private Coroutine noticeRoutine;

    void Awake()
    {
        dialogueObject.SetActive(false);
    }

    void Update()
    {
        if (!dialogueObject.activeInHierarchy) return;
        if (Mouse.current.leftButton.wasPressedThisFrame) advanceRequested = true;
    }

    public IEnumerator PlayGreeting(SO_Customer customer)
    {
        StopNotice();
        customerNameText.text = customer.customerName;
        dialogueObject.SetActive(true);

        if (customer.customerDialogue != null) yield return PlayLines(customer.customerDialogue.dialogueLines);
    }

    public IEnumerator PlayEmptyOrder()
    {
        yield return PlayLines(emptyOrderLines);
    }

    public IEnumerator PlayOrder(List<SO_Item> items)
    {
        orderLine = "Aku beli " + JoinNames(items) + ".";
        yield return PlayLines(new[] { orderLine });
    }

    public IEnumerator PlayThanks()
    {
        yield return PlayLines(thanksLines);
    }

    public void ShowWrongItem()
    {
        StopNotice();
        noticeRoutine = StartCoroutine(NoticeThenRestoreOrder());
    }

    public void ShowPayment(int paid, int change)
    {
        StopNotice();
        customerDialogueText.text = "Nih duit " + paid + ". kasi aku kembalian " + change + ".";
    }

    public void Close()
    {
        StopNotice();
        dialogueObject.SetActive(false);
    }

    private IEnumerator PlayLines(string[] lines)
    {
        if (lines == null) yield break;

        foreach (string line in lines)
        {
            customerDialogueText.text = line;
            advanceRequested = false;
            yield return new WaitUntil(() => advanceRequested);
        }
    }

    private IEnumerator NoticeThenRestoreOrder()
    {
        customerDialogueText.text = wrongItemLine;
        yield return new WaitForSeconds(noticeDuration);

        customerDialogueText.text = orderLine;
        noticeRoutine = null;
    }

    private void StopNotice()
    {
        if (noticeRoutine == null) return;

        StopCoroutine(noticeRoutine);
        noticeRoutine = null;
    }

    private static string JoinNames(List<SO_Item> items)
    {
        return string.Join(", ", items.ConvertAll(i => i.itemName));
    }
}