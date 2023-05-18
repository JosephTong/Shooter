using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private Transform m_Start;
    [SerializeField] private Transform m_End;
    [SerializeField] private LineRenderer m_Line;

    private void FixedUpdate() {
        m_Line.SetPosition(0,m_Start.position);
        m_Line.SetPosition(1,m_End.position);
    }

}
