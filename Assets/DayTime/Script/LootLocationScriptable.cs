using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootLocation", menuName = "Scriptable/LootLocation", order = 4)]
public class LootLocationScriptable : ScriptableObject
{
    public string DisplayName;
    public Sprite Image;
    public Vector2 Size = new Vector2(150f,150f);
    public Vector2 Position;
    [Range(0f,1000f)]public float MinRawMaterial;
    [Range(0f,1000f)]public float MaxRawMaterial;
    [Range(0f,1000f)]public float MinScrapMaterial;
    [Range(0f,1000f)]public float MaxScrapMaterial;
    [Range(0f,1000f)]public float MinChemMaterial;
    [Range(0f,1000f)]public float MaxChemMaterial;
    [Range(0f,1000f)]public float MinElectricMaterial;
    [Range(0f,1000f)]public float MaxElectricMaterial;


}
