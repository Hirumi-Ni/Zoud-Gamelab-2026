using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/test item")]
public class SO_Item : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;                     
    public int baseSellPrice;
}
