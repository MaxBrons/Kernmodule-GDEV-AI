using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class StateHeaderUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private Image _background;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_camera != null) {
            Vector3 targetDirection = transform.position - _camera.transform.position;
            float singleStep = 3 * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    public void SetHeader(string text, Color textColor, Color backgroundColor)
    {
        _header.text = text;
        _header.color = textColor;
        _background.color = backgroundColor;
    }
}