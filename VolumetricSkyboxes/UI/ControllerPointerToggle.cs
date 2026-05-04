using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VolumetricSkyboxes.UI
{
    // A toggle with extra steps
    [RequireComponent(typeof(Button))]
    public class ControllerPointerToggle : MonoBehaviour
    {
        [SerializeField]
        public Image onImage;
        
        public ToggleEvent onValueChanged = new();
        
        [SerializeField]
        private bool m_IsOn;
        
        public bool isOn
        {
            get => m_IsOn;
            set
            {
                m_IsOn = value;
                onValueChanged.Invoke(m_IsOn);
            }
        }

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(InternalToggle);
        }

        private void Start()
        {
            SetCheckmark();
        }

        private void InternalToggle()
        {
            isOn = !isOn;
            SetCheckmark();
        }

        private void SetCheckmark()
        {
            onImage.enabled = m_IsOn;
        }

        [Serializable]
        public class ToggleEvent : UnityEvent<bool>
        {
        }
    }
}