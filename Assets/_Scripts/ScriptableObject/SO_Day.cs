using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game Data/Day")]
public class SO_Day : ScriptableObject
{
    public List<SO_Customer> dayCustomer = new();
}
