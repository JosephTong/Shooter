using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendedButtons;

public class GunController : MonoBehaviour
{
    [Header("Drag to aim")]
    [SerializeField][Range(0.1f, 2)] private float m_AimSensitivity = 0.5f;
    [SerializeField] private Button2D m_AimBtn;
    [SerializeField] private Transform m_CrossHair;
    private Vector2 m_AimDragStartPos = Vector2.zero;
    private Vector2 m_AimDragEndPos = Vector2.zero;
    private Vector3 m_CrossHairDragStartPos;


    // the area the player can shoot 
    // prevent crosshair going out of bound
    [Header("CrossHair out of bound prevention")]
    [SerializeField] private Vector2Int m_FieldSize;
    [SerializeField] private Vector3 m_FieldCenter;

    [Header("Aim effect for camera")]
    [SerializeField] private Camera m_MainCamera;
    private Vector3 m_MainCameraStartPos;
    private Vector3 m_AimDirection = Vector3.zero;
    private float m_AimDistanceNormalized = 0;
    private float m_FieldCenterToCornerDistance = 10;

    [Header("Aim effect for gun")]
    [SerializeField] private Transform m_GunModel;




    private void Start()
    {
        m_FieldCenterToCornerDistance = Mathf.Sqrt(m_FieldSize.y / 2 * m_FieldSize.y / 2 + m_FieldSize.x / 2 * m_FieldSize.x / 2);
        m_MainCameraStartPos = m_MainCamera.transform.position;
        m_AimBtn.onDown.AddListener(() =>
        {
            m_AimDragStartPos = Input.mousePosition;
            m_CrossHairDragStartPos = m_CrossHair.position;
        });
        m_AimBtn.onUp.AddListener(() =>
        {
            m_AimDragStartPos = Vector2.zero;
        });
    }


    private void Update()
    {
        if (m_AimDragStartPos != Vector2.zero)
        {
            m_AimDragEndPos = Input.mousePosition;

            Vector3 offset = m_AimSensitivity * (m_AimDragEndPos - m_AimDragStartPos) / 15;
            m_CrossHair.position = m_FieldCenter + m_CrossHairDragStartPos + offset;

            CrossHairOutOfBoundPrevention();

            m_AimDirection = (m_CrossHair.position - m_FieldCenter).normalized;
            m_AimDistanceNormalized = Vector3.Distance(m_CrossHair.position, m_FieldCenter) /
                m_FieldCenterToCornerDistance;

            m_MainCamera.transform.position = m_MainCameraStartPos + m_AimDirection * m_AimDistanceNormalized;

           // m_GunModel.localScale = new Vector3(1 - m_AimDirection.x *0.25f, 1, 1);
        }
    }




    private void CrossHairOutOfBoundPrevention()
    {
        if (m_CrossHair.position.x < m_FieldCenter.x - m_FieldSize.x / 2)
        {
            // Left out of bound
            m_CrossHair.position = new Vector3(m_FieldCenter.x - m_FieldSize.x / 2, m_CrossHair.position.y, 0);
        }

        if (m_CrossHair.position.x > m_FieldCenter.x + m_FieldSize.x / 2)
        {
            // Right out of bound
            m_CrossHair.position = new Vector3(m_FieldCenter.x + m_FieldSize.x / 2, m_CrossHair.position.y, 0);
        }


        if (m_CrossHair.position.y > m_FieldCenter.y + m_FieldSize.y / 2)
        {
            // Top out of bound
            m_CrossHair.position = new Vector3(m_CrossHair.position.x, m_FieldCenter.y + m_FieldSize.y / 2, 0);
        }


        if (m_CrossHair.position.y < m_FieldCenter.y - m_FieldSize.y / 2)
        {
            // Down out of bound
            m_CrossHair.position = new Vector3(m_CrossHair.position.x, m_FieldCenter.y - m_FieldSize.y / 2, 0);
        }
    }
}
