using System.Collections;
using System.Collections.Generic;
using ExtendedButtons;
using UnityEngine;
using UnityEngine.UI;

public class GunReloadController : MonoBehaviour
{

    [SerializeField] private Image m_MainGunImage;

    [SerializeField] private Button m_FirstTapImage;
    [SerializeField] private Button m_SecondTapImage;


    
    [SerializeField] private RectTransform m_DragImage; // current mouse position while dragging
    [Header("First Drag")]
    [SerializeField] private Button m_FirstStartDragImage;
    [SerializeField] private Button m_FirstEndDragImage; // destination

    [Header("Second Drag")]
    [SerializeField] private Button m_SecondStartDragImage;
    [SerializeField] private Button m_SecondEndDragImage; // destination
    
    public void InIt(){

    }
}
