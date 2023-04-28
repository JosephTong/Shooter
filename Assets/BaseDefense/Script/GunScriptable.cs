using System.Collections;
using System.Collections.Generic;
using GunReloadScriptableNameSpace;
using UnityEngine;


namespace GunReloadScriptableNameSpace
{
    [System.Serializable]
    public class GunReloadDragFunction
    {
        public Vector2 StartDragPosition;
        public Vector2 EndDragPosition;

        public Sprite DragCursorSprite;
        public Vector2 DragCursorSize;

        public GunReloadActionResult Result;
    }
    [System.Serializable]
    public class GunReloadTapFunction
    {
        public Vector2 TapPosition;
        public GunReloadActionResult Result;

    }

    [System.Serializable]
    public class GunReloadExtraImage
    {
        public Vector2 Position;
        public Vector2 Size;
        public Sprite ImageSprite;

    }

    [System.Serializable]
    public class GunReloadPhras
    {
        public List<GunReloadDragFunction> m_DragFunction = new List<GunReloadDragFunction>();
        public List<GunReloadTapFunction> m_TapFunction = new List<GunReloadTapFunction>();
        public List<GunReloadExtraImage> m_ExtraImages = new List<GunReloadExtraImage>();
        
        public Sprite m_MainGunSprite;
    }


    [System.Flags]
    public enum GunReloadActionResult
    {   
        None ,
        CancelReload = 1 << 1 ,
        GainOneAmmo = 1 << 2 ,
        FullAmmoReload = 1 << 3 ,
        ToNextPhase = 1 << 4  

    }
}

[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable/Weapon", order = 1)]
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

    [Header("Accuracy lose on shoot")]
    [Range (0,100)]public float Recoil = 50; 

    [Header("Accuracy lose on moving , the higher the stability , the less accruacy lose while moving")]
    [Range (0,100)]public float Stability = 50;  
    
    [Header("Acuracy gain overtime")]
    [Range (0,100)]public float Handling = 50; 


    
    [Space(20)]
    [Header("Reload")]
    public List<GunReloadPhras> Phases = new List<GunReloadPhras>();
    public Vector2 MainGunSize;

}
