using UnityEngine;
using UnityEngine.Serialization;

namespace VolumetricSkyboxes
{
    [CreateAssetMenu(menuName = "Volumetric Skyboxes/Skybox")]
    public class VolumetricSkyboxData : ScriptableObject
    {
        [Tooltip("Skybox prefab")]
        public GameObject prefab;
        [Tooltip("Skybox name shown in the terminal")]
        public string title;
        [Tooltip("Skybox author")]
        public string author;
        [Tooltip("Skybox icon shown in the terminal")]
        public Sprite skyboxIcon;
        [Tooltip("A regular skybox used in the scene")]
        public Material skyboxMaterial;
        
        [Space(10)]
        [Header("Arena lighting settings")]
        [Tooltip("Enable custom arena lighting")]
        public bool customLighting;
        [Tooltip("Arena lighting color")]
        public Color lightingColor;
        [Tooltip("Arena lights intensity")]
        [Range(0.0f, 1.0f)]
        public float lightingIntensity;
        
        [Space(10)]
        [Header("Fog settings")]
        [Tooltip("Enable custom arena fog")]
        public bool customFog;
        [Tooltip("Arena fog color")]
        public Color fogColor;
        public float fogMinimum;
        public float fogMaximum;
        
        [FormerlySerializedAs("fogColor")]
        [Space(10)]
        [Header("Grid Textures")]

        public Texture2D baseGridTexture;
        public Texture2D topRowGridTexture;
        public Texture2D topGridTexture;
        
        [Space(10)]
        [Header("Camera Settings")]
        [Tooltip("Defines how fast background moves relative to the player; 0 = background is static; 1 = background moves normally; 0.5 = background moves twice as slow; 2 = background moves twice as fast")]
        public float ParallaxCoefficient = 1f;
        public float NearClipPlane = 0.1f;
        public float FarClipPlane = 4000f;
    }
}