using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtendedButtons;

public class GunController : MonoBehaviour
{
    [SerializeField] private GunScriptable m_SelectedGun;

    [Header("Drag to aim")]
    [SerializeField][Range(0.1f, 2)] private float m_AimSensitivity = 0.5f;
    [SerializeField] private Button2D m_AimBtn;
    [SerializeField] private Transform m_CrossHair;
    private Vector2 m_AimDragMouseStartPos = Vector2.zero;
    private Vector2 m_AimDragMouseEndPos = Vector2.zero;
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


    [Header("Accracy")]
    [SerializeField] private float m_CurrentAccruacy = 0;
    private Vector3 m_MousePreviousPos = Vector3.zero;


    private void Start()
    {
        this.transform.position = new Vector3(7, -4.5f, 0) + m_FieldCenter;
        m_MainCamera.transform.position = new Vector3(m_FieldCenter.x, m_FieldCenter.y, -10);
        m_FieldCenterToCornerDistance = Mathf.Sqrt(m_FieldSize.y / 2 * m_FieldSize.y / 2 + m_FieldSize.x / 2 * m_FieldSize.x / 2);
        m_MainCameraStartPos = m_MainCamera.transform.position;
        m_AimBtn.onDown.AddListener(() =>
        {
            m_MousePreviousPos = Input.mousePosition;
            m_AimDragMouseStartPos = Input.mousePosition;
            m_CrossHairDragStartPos = m_CrossHair.position;
        });
        m_AimBtn.onUp.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
        });
        
        m_AimBtn.onExit.AddListener(() =>
        {
            m_AimDragMouseStartPos = Vector2.zero;
            m_MousePreviousPos = Vector3.zero;
        });
    }


    private void Update()
    {


        if (m_AimDragMouseStartPos != Vector2.zero)
        {
            m_AimDragMouseEndPos = Input.mousePosition;

            Vector3 offset = m_AimSensitivity * (m_AimDragMouseEndPos - m_AimDragMouseStartPos) / 15;
            m_CrossHair.position = m_CrossHairDragStartPos + offset;

            // acc lose for moving
            float mouseMoveAmound = Vector3.Distance(m_MousePreviousPos, Input.mousePosition) /  
                ( Mathf.Sqrt( Screen.height * Screen.height + Screen.width * Screen.width ) /2 ) * 1000 ;

            if(mouseMoveAmound==0){
                // draging but not moving , gain accruacy over time
                AccruacyGainOvertime();
            }else{
                m_CurrentAccruacy -= Time.deltaTime * mouseMoveAmound * (200 - m_SelectedGun.Stability);
                m_MousePreviousPos = Input.mousePosition;
            }


            CrossHairOutOfBoundPrevention();

            // aim to camera effect
            m_AimDirection = (m_CrossHair.position - m_FieldCenter).normalized;
            m_AimDistanceNormalized = Vector3.Distance(m_CrossHair.position, m_FieldCenter) /
                m_FieldCenterToCornerDistance;

            // aim to weapon effect
            m_MainCamera.transform.position = m_MainCameraStartPos + m_AimDirection * m_AimDistanceNormalized;
            float gunScaleX = 0.1f * ((m_CrossHair.position.x - m_FieldCenter.x) / (m_FieldSize.x / 2));
            m_GunModel.localScale = new Vector3(1 - gunScaleX, 1, 1);

            float gunRotationZ = -5 * (m_CrossHair.position.y - m_FieldCenter.y) / (m_FieldSize.y / 2);
            m_GunModel.localEulerAngles = new Vector3(0, 0, gunRotationZ);
        }else{
            AccruacyGainOvertime();
        }
        if (m_CurrentAccruacy < 0)
            m_CurrentAccruacy = 0;
        m_CrossHair.localScale = new Vector3((100 - m_CurrentAccruacy) / 100, (100 - m_CurrentAccruacy) / 100, (100 - m_CurrentAccruacy) / 100);
    }

    private void AccruacyGainOvertime(){
        if (m_CurrentAccruacy < m_SelectedGun.Accuracy)
        {
            m_CurrentAccruacy += Time.deltaTime * m_SelectedGun.Handling;
        }
        else
        {
            m_CurrentAccruacy = m_SelectedGun.Accuracy;
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
