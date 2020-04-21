using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct PreyData
{
    public string m_name;
    public Sprite m_sprite;
}

[CreateAssetMenu(fileName = "SpritesDatabase", menuName ="Database/Sprites", order = 1)]
public class SpritesDB : ScriptableObject
{
    public Sprite m_spriteMother; //Player
    public Sprite m_spriteChildren;
    public Sprite m_spriteShoot;
    public Sprite[] m_spritePredator;
    public PreyData[] m_spritePrey;
}
