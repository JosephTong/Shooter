using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun", order = 1)]
public class GunScriptable : ScriptableObject
{
    public string DisplayName;
    public float Damage;
    public float ClipSize;
    [Header("True if holding down shoot btn will shoot continuously")]
    public bool IsSemiAuto = true;

    [Header("How many ammo could be shot out every 1 second")]
    [Range (1,25)]public float FireRate = 2;

    [Header("Max accuracy , 100 = always hit center")]
    [Range (0,100)]public float Accuracy = 50; 

    [Header("Accuracy lose on shoot ( 15 recoil = 15 accruacy lose per shot )")]
    [Range (0,100)]public float Recoil = 50; 

    [Header("Accuracy lose on moving , the higher the stability , the less accruacy lose while moving")]
    [Range (0,100)]public float Stability = 50;  
    
    [Header("Acuracy gain overtime")]
    [Range (0,100)]public float Handling = 50; 

}
