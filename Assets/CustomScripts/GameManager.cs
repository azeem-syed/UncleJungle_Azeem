using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class GameManager : MonoBehaviour
    {
        public const int windowSize = 50;
        public const float multiplier = 20;
        public const float alpha = 0.4f;
        public const float trackLength = 100f;
        public const float maxVelocity = 10f;
        public const float jumpPower = 150f;
        public const float spawnDistanceMin = 100;
        public const float spawnDistanceMax = 200f;
        public const float safeDistance = 10f;

        public GameObject player = null;
        public TextMesh gameText = null;

        public GameObject[] tracks;
        public List<GameObject> obstacles;
        public List<GameObject> instiatedObstacles;

        GameObject obst;
        Rigidbody playerRigidBody;

        int frameCount = 0;
        int arrayCount = 0;
        int trackIndex = 0;

        float trackDistance = trackLength;
        float spawnDistance = 0;

        float facePositionY = 0;
        float min = 0;
        float max = 0;
        float prevVelocity = 0;
        float currVelocity = 0;
        float average = 0;
        List<float> facePositions;

        float startTime = 0;
        float interval = 0;
        IEnumerator InstantiateObstacleCoroutine;

        // Start is called before the first frame update
        void Start()
        {
            facePositions = new List<float>();
            playerRigidBody = player.GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
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

            //currVelocity = 10f; //////////////////////////////////////////// - For Unity Play Mode - //////////////////////////////////////////////////////

            currVelocity = currVelocity > maxVelocity ? maxVelocity : currVelocity;

            player.transform.localPosition += Vector3.forward * Time.deltaTime * currVelocity;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerRigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }

            if (player.transform.localPosition.z >= trackDistance)
            {
                trackDistance += trackLength;
                Vector3 trackPosition = tracks[trackIndex].transform.localPosition;
                tracks[trackIndex].transform.localPosition = new Vector3(trackPosition.x, trackPosition.y, trackPosition.z + trackLength * (tracks.Length));

                trackIndex++;
                trackIndex %= tracks.Length;
            }

            if (player.transform.localPosition.z > spawnDistance)
            {
                spawnDistance = player.transform.localPosition.z + Random.Range(spawnDistanceMin, spawnDistanceMax);
                InstantiateObstacleCoroutine = InstantiateObstacle(spawnDistance);
                StartCoroutine(InstantiateObstacleCoroutine);
            }

            prevVelocity = currVelocity;

            frameCount++;
        }
        IEnumerator InstantiateObstacle(float spawnDistance)
        {
            foreach (GameObject obj in instiatedObstacles)
            {
                Destroy(obj);
            }
            instiatedObstacles.Clear();
            obst = Instantiate(obstacles[Random.Range(0, obstacles.Count)], new Vector3(0f, 0.01f, spawnDistance), Quaternion.identity) as GameObject;
            //obst = Instantiate(obstacles[14], new Vector3(0f, 0.01f, spawnDistance), Quaternion.identity) as GameObject;
            instiatedObstacles.Add(obst);

            yield return new WaitForEndOfFrame();
        }
    }
}