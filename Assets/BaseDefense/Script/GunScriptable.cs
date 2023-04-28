using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun", order = 1)]
public class GunScriptable : ScriptableObject
{
    public string DisplayName;
    public float Damage;
    public float ClipSize;
    [Range (0,100)]public float Accuracy; // max accuracy , 100 = always hit center
    [Range (0,100)]public float Recoil; // accuracy lose on fire
    [Range (0,100)]public float Stability; // accuracy lose on moving , the higher , the less accruacy lose while moving
    [Range (0,100)]public float Handling; // accuracy gain overtime 

}
