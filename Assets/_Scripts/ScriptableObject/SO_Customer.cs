using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game Data/Customer")]
public class SO_Customer : ScriptableObject
{
    public Sprite customerSprite;
    public string customerName;
    public List<string> customerPurchaseItem = new();//sementara ku set string dulu, soalnya SO itemnya/enumnya blom ada
}
