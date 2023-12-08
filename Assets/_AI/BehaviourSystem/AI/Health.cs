using System;
using UnityEngine;

public interface IDamagable
{
    public event Action<float> OnHealthDecreased;
    public void Damage(float value);
}

public interface IHealable
{
    public event Action<float> OnHealthIncreased;
    public void Heal(float value);
}

public class Health : MonoBehaviour, IDamagable, IHealable
{
    public event Action<float> OnHealthIncreased;
    public event Action<float> OnHealthDecreased;
    public event Action OnHealthDepleted;
    public float HealthPoints => _health;

    [SerializeField] private float _maxHealth = 100.0f;
    [SerializeField] private float _health = 100.0f;


    private void Start()
    {
        _health = _maxHealth;
    }

    public void Damage(float value)
    {
        float newHealth = Mathf.Clamp(_health - value, 0, _maxHealth);

        if (newHealth != _health) {
            _health = newHealth;
            OnHealthDecreased?.Invoke(_health);

            if (HealthPoints <= 0)
                OnHealthDepleted?.Invoke();
        }

    }

    public void Heal(float value)
    {
        float newHealth = Mathf.Clamp(_health + value, 0, _maxHealth);

        if (newHealth != _health) {
            _health = newHealth;
            OnHealthIncreased?.Invoke(_health);
        }
    }
}