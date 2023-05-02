using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using GunReloadScriptableNameSpace;
using UnityEngine;
using UnityEngine.UI;

public class GunReloadController : MonoBehaviour
{
    public GunReloadScriptable m_ReloadScriptable;


    [SerializeField] private Image m_MainGunImage;
    [SerializeField] private Transform m_GrayWhileDragPanel; // gray out while draging
    [SerializeField] private Transform m_NotGrayWhileDragPanel; // NOT gray out while draging , for EndDragPrefab in ReloadScriptable
    [SerializeField] private RectTransform m_DragImagePos; // current mouse position while dragging

    private List<GameObject> m_AllSpawnedImage = new List<GameObject>();
    private int m_CurReloadPhase = 0;

    private void Start() {
        InIt();
    }

    public void InIt(){
        m_CurReloadPhase = 0;
        m_MainGunImage.sprite = m_ReloadScriptable.StartMainGunImage;
        m_MainGunImage.rectTransform.sizeDelta = m_ReloadScriptable.MainGunSize;

        GunReloadPhase currentGunReloadPhase = m_ReloadScriptable.ReloadPhases[m_CurReloadPhase];
        SpawnDragItems(currentGunReloadPhase.m_DragFunction[0]);
    }

    private void SpawnDragItems(GunReloadDragFunction dragFunction){
        var spawnedStartDrag = Instantiate(dragFunction.StartDragPrefab);
        spawnedStartDrag.transform.SetParent(m_GrayWhileDragPanel);
        spawnedStartDrag.GetComponent<RectTransform>().anchoredPosition = dragFunction.StartDragPosition;
        m_AllSpawnedImage.Add(spawnedStartDrag);
    }
}
