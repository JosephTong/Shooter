using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionSlot : MonoBehaviour
{
    public Image m_Border;
    public Image m_WeaponImage;
    public Button m_Btn;

    public void BorderColorToggle(bool isActive){
        m_Border.color = isActive?Color.green:Color.white;   
    }
}
