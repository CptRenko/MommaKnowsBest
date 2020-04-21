using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Prefabs
{
    public GameObject m_shoot;
    public GameObject m_prey;
    public GameObject m_predator;
}

[CreateAssetMenu(fileName = "PrefabDB", menuName = "Database/Prefabs", order = 2)]
public class PrefabDB : ScriptableObject
{
    public Prefabs m_prefabs;
}
