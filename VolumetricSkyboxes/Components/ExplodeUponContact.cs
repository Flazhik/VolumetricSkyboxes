using UnityEngine;

namespace VolumetricSkyboxes.Components
{
    public class ExplodeUponContact : MonoBehaviour
    {
        public int throwsLeft;
        public GameObject thatExplosionTemplate;
        
        private GameObject _thatExplosion;
        private bool _collided;

        public void Throw()
        {
            throwsLeft--;
        }

        private void OnCollisionEnter(Collision cols)
        {
            var collision = cols.contacts[0];
            if (_collided || !(throwsLeft < 0 && LayerMaskDefaults.IsMatchingLayer(collision.otherCollider.gameObject.layer, LMD.Environment)))
                return;
            
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
            
            _collided = true;
            var velocity = GetComponent<Rigidbody>().velocity;
            var explosionRotation = Quaternion.LookRotation(collision.normal);
            
            _thatExplosion = Instantiate(thatExplosionTemplate, gameObject.transform.position - velocity * Time.fixedDeltaTime * 2, explosionRotation);
            Invoke(nameof(DestroyPlushie), 1f);
        }

        private void DestroyPlushie()
        {
            Destroy(_thatExplosion);
            Destroy(gameObject);
        }
    }
}