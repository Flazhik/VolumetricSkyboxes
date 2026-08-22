using UnityEngine;
using UnityEngine.Events;

namespace VolumetricSkyboxes
{
    public class OnNextWave : MonoBehaviour
    {
        public UnityEvent onNextWave;
        
        public void Execute() => onNextWave?.Invoke();
    }
}