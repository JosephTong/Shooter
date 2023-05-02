using System.Collections;
using System.Collections.Generic;
using GunReloadScriptableNameSpace;
using UnityEngine;

namespace GunReloadScriptableNameSpace
{
    [System.Serializable]
    public class GunReloadDragFunction
    {
        [Header ("Start Drag")]
        public GameObject StartDragPrefab;
        public Vector2 StartDragPosition;
        public Sprite MainGunSpriteOnStart;

        [Header ("Draging")]
        public GameObject DragCursorPrefab;

        [Header ("End Drag")]
        public GameObject EndDragPrefab;
        public Vector2 EndDragPosition;
        public Sprite MainGunSpriteOnEnd;



        public GunReloadActionResult ResultAction;
    }
    [System.Serializable]
    public class GunReloadTapFunction
    {
        public Vector2 TapPosition;
        public GameObject TapPrefab;
        public Sprite MainGunSpriteOnEnd;
        public GunReloadActionResult ResultAction;

    }

    [System.Serializable]
    public class GunReloadExtraImage
    {
        public Vector2 Position;
        public GameObject ImagePrefab;

    }

    [System.Serializable]
    public class GunReloadPhase
    {
        public List<GunReloadDragFunction> m_DragFunction = new List<GunReloadDragFunction>();
        public List<GunReloadTapFunction> m_TapFunction = new List<GunReloadTapFunction>();
        public List<GunReloadExtraImage> m_ExtraImages = new List<GunReloadExtraImage>();
        
    }


    [System.Flags]
    public enum GunReloadActionResult
    {   
        None ,
        CancelReload = 1 << 1 ,
        GainOneAmmo = 1 << 2 , // do nothing if ammo already reach maximum 
        FullAmmoReload = 1 << 3 ,
        ToNextPhase = 1 << 4 ,
        RefreshThisPhase = 1 << 5 , // for shotgun


    }
}

[CreateAssetMenu(fileName = "Gun", menuName = "Scriptable/Reload", order = 2)]
public class GunReloadScriptable : ScriptableObject
{
    public Sprite StartMainGunImage; // image tobe use at the start of reloading
    public Vector2 MainGunSize;
    public List<GunReloadPhase> ReloadPhases = new List<GunReloadPhase>();
}
