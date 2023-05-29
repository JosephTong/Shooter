using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EclecticController : MonoBehaviour
{
    [SerializeField] private Transform m_StartPos;
    [SerializeField] private Transform m_PosTwo;
    [SerializeField] private Transform m_PosThree;
    [SerializeField] private Transform m_EndPos;

    [SerializeField] private Vector2 m_StartCurve;
    [SerializeField] private Vector2 m_EndCurve;

    private void Start() {
        
    }

    private void Update() {
        m_StartPos.right = m_EndPos.position - m_StartPos.position;
        m_EndPos.right = m_StartPos.position - m_EndPos.position;

        float distance = Vector3.Distance(m_EndPos.position,m_StartPos.position);

        m_PosTwo.localPosition = m_StartCurve * distance/2;
        m_PosThree.localPosition = m_EndCurve * distance/2;

    }
}
