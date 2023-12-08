using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float Damage => _damage;

    [SerializeField, Min(0)] private float _damage;
}