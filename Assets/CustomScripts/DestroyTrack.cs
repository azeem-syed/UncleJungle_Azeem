using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class DestroyTrack : MonoBehaviour
    {
        public GameObject player;

        // Update is called once per frame
        void Update()
        {
            if (player.transform.localPosition.z > transform.localPosition.z + GameManager.trackLength) {
                Destroy(gameObject);
            }
        }
    }
}