using System;
using UnityEngine;
using UnityEngine.Events;

namespace VolumetricSkyboxes
{
    [Serializable]
    public class OnDoorsOpen : MonoBehaviour
    {
        [Tooltip("Lower priority events are invoked first")]
        [Range(0, 1000)]
        public int priority;
        public UnityEvent onSceneLoad;
        
        public void Execute()
        {
            onSceneLoad?.Invoke();
        }
    }
}