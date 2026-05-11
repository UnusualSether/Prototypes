using System;
using UnityEngine;

[Serializable]
public abstract class BulletData : ScriptableObject
{
    [SerializeField]
    public string bulletName;

    [SerializeField]
    public string bulletDescription;

    [SerializeField]
    public int bulletDamage;


    public abstract void BulletEffect();


}
