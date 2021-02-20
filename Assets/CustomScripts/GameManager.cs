using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class GameManager : MonoBehaviour
    {
        public const int windowSize = 50;
        public const float multiplier = 20;
        public const float alpha = 0.2f;

        public GameObject player = null;
        public TextMesh gameText = null;

        int frameCount = 0;
        int arrayCount = 0;

        float facePositionY = 0;
        float min = 0;
        float max = 0;
        float prevVelocity = 0;
        float currVelocity = 0;
        float average = 0;
        float stepCount = 0;
        List<float> facePositions;

        float startTime = 0;
        float interval = 0;

        // Start is called before the first frame update
        void Start()
        {
            facePositions = new List<float>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            facePositionY = StaticVariable.positionY;
            facePositions.Add(facePositionY);

            if (facePositions.Count > windowSize)
            {
                facePositions.RemoveAt(0);
            }

            if (frameCount >= windowSize)
            {
                frameCount = 0;
                min = Mathf.Min(facePositions.ToArray());
                max = Mathf.Max(facePositions.ToArray());

                average = (max - min) / 2.0f;

                float intervalMultiplier = interval > 0 ? multiplier / interval : multiplier;
                currVelocity = Mathf.Abs(max - min) * intervalMultiplier;

            }

            arrayCount = facePositions.Count;

            if (arrayCount >= windowSize - 1 && facePositions[arrayCount - 1] > average && facePositions[arrayCount - 2] <= average)
            {
                interval = startTime > 0 ? Time.time - startTime : 0;
                startTime = Time.time;
            }

            currVelocity = Mathf.Lerp(prevVelocity, currVelocity, alpha);
            gameText.text = currVelocity.ToString();

            player.transform.localPosition += Vector3.forward * Time.deltaTime * currVelocity;
            prevVelocity = currVelocity;

            frameCount++;
        }
    }
}