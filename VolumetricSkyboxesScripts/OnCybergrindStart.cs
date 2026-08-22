using UnityEngine;
using UnityEngine.Events;

namespace VolumetricSkyboxes
{
    public class OnCybergrindStart : MonoBehaviour
    {
        public UnityEvent onCybergrindStart;

        public void Execute() => onCybergrindStart?.Invoke();
    }
}