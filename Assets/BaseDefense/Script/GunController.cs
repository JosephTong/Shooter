using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GunController : MonoBehaviour
{
    [SerializeField] private Transform m_Crosshair; // sway camera 
    [SerializeField][Range(0.1f, 1.5f)] private float m_MouseSensitivity = 0.5f;

    [SerializeField] private Vector3 m_WeaponToMainCameraOffset = new Vector3(5.5f, -2.5f, 0);

    [SerializeField] private Vector2Int m_TopRight;
    [SerializeField] private Vector2Int m_BottomLeft;
    [SerializeField] private PolygonCollider2D m_BaseCameraBorder ;


    private void Start()
    {
        UpdateBaseCameraBorder();
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse X") < 1 && Input.GetAxis("Mouse Y") < 1)
        {
            m_Crosshair.position += new Vector3(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y"),
                0) * m_MouseSensitivity;

            // prevent crosshair going out of bound
            if(m_Crosshair.position.x < Mathf.Min(m_TopRight.x,m_BottomLeft.x) ){
                // Left out of bound
                m_Crosshair.position = new Vector3(Mathf.Min(m_TopRight.x,m_BottomLeft.x),m_Crosshair.position.y,0);
            }

            if(m_Crosshair.position.x > Mathf.Max(m_TopRight.x,m_BottomLeft.x) ){
                // Left out of bound
                m_Crosshair.position = new Vector3(Mathf.Max(m_TopRight.x,m_BottomLeft.x),m_Crosshair.position.y,0);
            }


            if(m_Crosshair.position.y > Mathf.Max(m_TopRight.y,m_BottomLeft.y) ){
                // Top out of bound
                m_Crosshair.position = new Vector3(m_Crosshair.position.x,Mathf.Max(m_TopRight.y,m_BottomLeft.y),0);
            }

            
            if(m_Crosshair.position.y < Mathf.Min(m_TopRight.y,m_BottomLeft.y) ){
                // Down out of bound
                m_Crosshair.position = new Vector3(m_Crosshair.position.x,Mathf.Min(m_TopRight.y,m_BottomLeft.y),0);
            }
        }

    }

    private void UpdateBaseCameraBorder(){
        Vector2 bottomLeft = new Vector2(Mathf.Min(m_TopRight.x,m_BottomLeft.x),Mathf.Min(m_TopRight.y,m_BottomLeft.y));
        Vector2 bottomRight= new Vector2(Mathf.Max(m_TopRight.x,m_BottomLeft.x),Mathf.Min(m_TopRight.y,m_BottomLeft.y));
        Vector2 topLeft = new Vector2(Mathf.Min(m_TopRight.x,m_BottomLeft.x),Mathf.Max(m_TopRight.y,m_BottomLeft.y));
        Vector2 topRight= new Vector2(Mathf.Max(m_TopRight.x,m_BottomLeft.x),Mathf.Max(m_TopRight.y,m_BottomLeft.y));
        m_BaseCameraBorder.points = new Vector2[]{
            topRight , topLeft , bottomLeft, bottomRight
        };
    }
}
