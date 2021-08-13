using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class PlayerCollision : MonoBehaviour
    {
        bool coinTirgger = false;
        GameObject banana;
        IEnumerator DestroyCoinCoroutine;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "HardObstacle") {
                //Debug.Log("Collided");
            }
            if (other.gameObject.tag == "Banana")
            {
                banana = other.gameObject;
                {
                    coinTirgger = true;
                }
                DestroyCoinCoroutine = DestroyCoin(other.gameObject);
                StartCoroutine(DestroyCoinCoroutine);
            }
        }

        void Update()
        {
            if (banana != null && coinTirgger)
            {
                banana.transform.Translate(Time.deltaTime * 20, Time.deltaTime * 50, Time.deltaTime * 50);
            }
        }
        IEnumerator DestroyCoin(GameObject other)
        {
            yield return new WaitForSeconds(0.5f);
            StaticVariable.bananas += 15;
            Destroy(other);
        }
    }
}
