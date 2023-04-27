using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendedButtons;

public class GunController : MonoBehaviour
{
    [Header("Drag to aim")]
    [SerializeField][Range(0.1f,2)] private float m_AimSensitivity = 0.5f;
    [SerializeField] private Button2D m_AimBtn;
    [SerializeField] private Transform m_CrossHair;
    private Vector2 m_AimDragStartPos = Vector2.zero;
    private Vector2 m_AimDragEndPos = Vector2.zero;
    private Vector3 m_CrossHairDragStartPos;



    private void Start() {
        m_AimBtn.onDown.AddListener(()=>{
            m_AimDragStartPos = Input.mousePosition;
            m_CrossHairDragStartPos = m_CrossHair.position;
        });
        m_AimBtn.onUp.AddListener(()=>{
            m_AimDragStartPos = Vector2.zero;
        });
    }


    private void Update() {
        if(m_AimDragStartPos != Vector2.zero){
            m_AimDragEndPos = Input.mousePosition;

            Vector3 offset = m_AimSensitivity * ( m_AimDragEndPos - m_AimDragStartPos) / 15;
            m_CrossHair.position = m_CrossHairDragStartPos + offset;

        }
    }
}
