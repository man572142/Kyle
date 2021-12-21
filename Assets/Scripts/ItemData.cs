using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/new ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public Sprite icon;
    public string title;
    [TextArea(5,10)]public string info;
    public Color color;
    /// <summary>�ӹD���_�ϥ�</summary>
    public UseType useType = UseType.CAN_NOT_USE;
    public CostType costType = CostType.NO_COST;

    public float hpChangeByUse = 0f;
    public float addDamagePercent = 0f;

}
/// <summary>�O�_��ϥ�</summary>
public enum UseType
{
    CAN_USE,
    CAN_NOT_USE
}
/// <summary>�ϥΫ�O�_�|�Q����</summary>
public enum CostType
{
    COST_ONE,
    NO_COST
}
