using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWithEndController : MonoBehaviour
{
    [SerializeField] private Transform m_Start;
    [SerializeField] private Transform m_End;
    [SerializeField] private LineRenderer m_Line;
    [SerializeField][Range(0f,1f)] private float m_Normalized = 1f;
    [SerializeField][Range(0.1f,1f)] private float m_LineWidth = 0.2f;
    [SerializeField] private ParticleSystem m_StartSmallSpark;
    [SerializeField] private ParticleSystem m_StartBigSpark;
    [SerializeField] private ParticleSystem m_StartRoundBigSpark;
    [SerializeField] private ParticleSystem m_EndBigSpark;
    [SerializeField] private ParticleSystem m_EndRoundBigSpark;

    [Header("Color Handling")] 
    [SerializeField][Range(1f,10f)] private float m_LineExtraEmission = 6f;
    
    [SerializeField][ColorUsage(true, true)] private Color m_Color;

    private void Start() {
    }

    private void FixedUpdate() {
        SetColor(m_Color);
        SetLineWidth(m_LineWidth);
        m_Start.LookAt(m_End);
        m_End.LookAt(m_Start);
        m_Line.SetPosition(0,m_Start.position);
        m_Line.SetPosition(1,m_End.position);
    }

    private void SetLineWidth(float width){
        
        m_Line.sharedMaterial.SetFloat("_Normalized",m_Normalized);
        m_Line.SetWidth(width,width);
        
        m_StartSmallSpark.startLifetime = width * 0.3f;
        m_StartRoundBigSpark.startLifetime = width*0.5f;
        m_StartBigSpark.startSize = width*2f;

        m_EndRoundBigSpark.startLifetime = width*0.5f;
        m_EndBigSpark.startSize = width*2f;
    }

    private void SetColor(Color color){
        m_StartSmallSpark.startColor = color;
        m_StartBigSpark.startColor = color;
        m_StartRoundBigSpark.startColor = color;
        m_EndBigSpark.startColor = color;
        m_EndRoundBigSpark.startColor = color;

        var particleSystemRenderer = m_StartSmallSpark.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.sharedMaterial.color = color;

        particleSystemRenderer = m_StartBigSpark.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.sharedMaterial.color = color;

        particleSystemRenderer = m_StartRoundBigSpark.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.sharedMaterial.color = color;

        particleSystemRenderer = m_EndBigSpark.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.sharedMaterial.color = color;

        particleSystemRenderer = m_EndRoundBigSpark.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.sharedMaterial.color = color;

        m_Line.sharedMaterial.SetColor("_Color",color * m_LineExtraEmission);

    }

}
