using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricSkyboxes.Utils;

namespace VolumetricSkyboxes.Components
{
    public class VolumetricSkyboxesCredits: MonoBehaviour
    {
        private readonly List<GameObject> _currentPlushies = new();
        
        [SerializeField]
        public GameObject flazhikPlushie;
        
        [SerializeField]
        public GameObject frenchinatorPlushie;
        
        [SerializeField]
        public GameObject clofPlushie;
        
        [SerializeField]
        public GameObject squeakySound;
        
        private NewMovement _nm;
        private int _nightmareEventCounter;
        private bool _nightmareEvent;

        private void Awake()
        {
            _nm = NewMovement.Instance;
        }

        public void SpawnPlushies()
        {
            if (_nightmareEvent)
                return;
            
            if (_nightmareEventCounter > 10)
            {
                _nightmareEvent = true;
                StartCoroutine(NightmareEvent());
                StartCoroutine(NightmareSqueaking());
                StartCoroutine(DeathScreen());
                StartCoroutine(QuitOnMusicStop());
                return;
            }
            
            var rotation = Quaternion.Euler(0, 180, 0);
            var nmTransformPos = _nm.transform.position;
            Instantiate(squeakySound, nmTransformPos, Quaternion.identity, transform);

            _currentPlushies.Add(Instantiate(frenchinatorPlushie, nmTransformPos + new Vector3(-1f, 1.5f, 0.5f), rotation));
            _currentPlushies.Add(Instantiate(flazhikPlushie, nmTransformPos + new Vector3(0f, 1.5f, 0.5f), rotation));
            _currentPlushies.Add(Instantiate(clofPlushie, nmTransformPos + new Vector3(1f, 1.5f, 0.5f), rotation));
            
            _nightmareEventCounter++;
        }

        private IEnumerator NightmareEvent()
        {
            GetComponent<AudioSource>().Play();
            for (;;)
            {
                var plushie = Random.Range(0, 3) switch
                {
                    0 => flazhikPlushie,
                    1 => frenchinatorPlushie,
                    _ => clofPlushie
                };
                
                CameraController.Instance!.transform.GetPositionAndRotation(out _, out var rotation);
                Instantiate(plushie, _nm.transform.position + rotation * Vector3.forward * 2f, Quaternion.Euler(0, 180, 0), transform);
                yield return new WaitForSeconds(0.02f);
            }
        }        
        
        private IEnumerator NightmareSqueaking()
        {
            GetComponent<AudioSource>().Play();
            for (;;)
            {
                Instantiate(squeakySound, _nm.transform.position, Quaternion.Euler(0, 180, 0), transform);
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private IEnumerator DeathScreen()
        {
            var deathSequence = NewMovement.Instance!.deathSequence;
            var deathSequenceClip = deathSequence.GetComponent<AudioSource>().clip;
            
            var music = GetComponent<AudioSource>();
            var musicClipLength = music.clip.length;
            
            yield return new WaitUntil(() => music.isPlaying && musicClipLength - music.time <= deathSequenceClip.length - 0.1f);
            deathSequence.gameObject.SetActive(true);
        }

        private IEnumerator QuitOnMusicStop()
        {
            yield return new WaitUntil(() => !GetComponent<AudioSource>().isPlaying && Application.isFocused);
            Application.Quit();
        }
    }
}