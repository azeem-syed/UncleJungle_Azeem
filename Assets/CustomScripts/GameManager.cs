using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation
{
    public class GameManager : MonoBehaviour
    {
        public const int windowSize = 50;
        public const int spawnPositionsPerTrack = 7;
        public const int bananasPerTrack = 15;
        public const int minLaneNumber = -1;
        public const int maxLaneNumber = 1;
        public const float editorVelocityFixed = 8f;
        public const float multiplier = 30;
        //public const float alpha = 0.4f;
        public const float trackLength = 100f;
        public const float trackSpawnDistance = 200f; // Thress times of track distance
        public const float maxVelocity = 15f;
        public const float minVelocity = 1f;
        public const float jumpPower = 150f;
        public const float fadeLength = 0.2f;
        public const float destroyDistance = 15f;
        public const float lanePosition = 1.7f;
        public const float moveSpeed = 30.0f;
        public const float leadDistance = 5.0f;


        public GameObject player = null;
        public GameObject banana = null;

        public GameObject playerObject = null;

        public TextMesh gameText = null;

        //[Range(1.0f, 15.0f)]
        //public 
        float editorVelocity = editorVelocityFixed;



        [Range(0.0f, 10.0f)]
        public float narrowTrackProbability = 0.0f;

        public GameObject[] tracks;
        public List<GameObject> obstacles;


        Vector3 leadVector = Vector3.zero;
        List<GameObject> instiatedObstacles;
        List<GameObject> instantiatedBananas;

        List<GameObject> instiatedTracks;

        Rigidbody playerRigidBody;
        Animator playerAnimator;

        int frameCount = 0;
        int arrayCount = 0;
        int trackIndex = 0;
        int velocityHash;

        float trackDistance = trackLength;
        float spawnDistance = 0;

        float facePositionY = 0;
        float facePositionYPrev = 0;
        float min = 0;
        float max = 0;
        float deviation = 0;
        float currVelocity = 0;
        float average = 0;

        int laneNumber = 0;
        float calculatedLane = 0.0f;
        Vector3 lookAtRotation = Vector3.zero;

        List<float> facePositions;

        float startTime = 0;
        float interval = 0;
        IEnumerator InstantiateObstacleCoroutine;
        IEnumerator InstantiateTrackCoroutine;
        IEnumerator InstantiateCoinsCoroutine;




        bool jumping = false;
        bool sprinting = false;

        int temp = 0;
        // Start is called before the first frame update
        void Start()
        {
            instiatedObstacles = new List<GameObject>();
            instiatedTracks = new List<GameObject>();
            instantiatedBananas = new List<GameObject>();
            facePositions = new List<float>();
            facePositions.Add(facePositionY);
            playerRigidBody = player.GetComponent<Rigidbody>();
            playerAnimator = playerObject.GetComponent<Animator>();
            velocityHash = Animator.StringToHash("Velocity");
            leadVector = player.transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            /* Face detection based run
             *
            facePositionY = StaticVariable.PositionY;
            if (facePositions.Count > 0 && facePositionY != facePositions[facePositions.Count - 1])
            {
                facePositions.Add(facePositionY);
                frameCount++;
                //temp++;
            }

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

                currVelocity = minVelocity + Mathf.Abs(max - min) * intervalMultiplier;

                currVelocity = currVelocity > maxVelocity ? maxVelocity : currVelocity;

                #region Jumping Code
                //deviation = GetVariation(facePositions, true);
                #endregion
            }

            arrayCount = facePositions.Count;

            if (facePositionYPrev > average && facePositionY <= average)
            {
                interval = startTime > 0 ? Time.time - startTime : 0;
                startTime = Time.time;
            }*/
            int coinsCount = StaticVariable.bananas;
            gameText.text = String.Format("Coins: {0}", coinsCount.ToString());

#if UNITY_EDITOR
            currVelocity = editorVelocity; //////////////////////////////////////////// - For Unity Play Mode - //////////////////////////////////////////////////////
#endif

            if (!jumping)
            {
                playerAnimator.SetFloat(velocityHash, currVelocity / maxVelocity);
            }

            #region Jumping Code
            if (!jumping && Input.GetKeyDown(KeyCode.Space))
            {
                jumping = true;
                playerAnimator.SetBool("Jumping", jumping);
                playerRigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                StartCoroutine("DisableJump");
            }
            playerAnimator.SetBool("Jumping", jumping);

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (laneNumber < maxLaneNumber)
                    laneNumber++;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (laneNumber > minLaneNumber)
                    laneNumber--;
            }

            if (sprinting == false && Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (coinsCount > 90)
                {
                    sprinting = true;
                    editorVelocity = 15.0f;
                    StartCoroutine("Sprint");
                }
            }

           //if (jumping && facePositionY > max + deviation) {
            //    jumping = false;
            //    playerRigidBody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            //    StartCoroutine("DisableJump");
            //}
            #endregion

            if (player.transform.localPosition.z >= trackDistance)
            {
                float currentTrackLocation = trackDistance;
                trackDistance += trackLength;
                InstantiateTrackCoroutine = InstantiateTrack(currentTrackLocation + trackSpawnDistance);
                StartCoroutine(InstantiateTrackCoroutine);
            }

            leadVector += Vector3.forward * Time.deltaTime * currVelocity;
            leadVector.z = player.transform.localPosition.z + leadDistance;

            leadVector.x = Mathf.Lerp(leadVector.x, lanePosition * laneNumber, moveSpeed * Time.deltaTime);

            player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, leadVector, currVelocity * Time.deltaTime);
            player.transform.LookAt(leadVector);

            facePositionYPrev = facePositionY;
        }
        IEnumerator InstantiateObstacle(float spawnPosition)
        {
            GameObject obst = Instantiate(obstacles[Random.Range(0, obstacles.Count)], new Vector3(Random.Range(-3f, 3f), 5f, spawnPosition), Quaternion.identity) as GameObject;

            instiatedObstacles.Add(obst);

            for (int i = 0; i < instiatedObstacles.Count; i++)
            {
                if (instiatedObstacles[i].transform.position.z < player.transform.position.z - trackLength / 2f)
                {
                    Destroy(instiatedObstacles[i].gameObject);
                    instiatedObstacles.RemoveAt(i);
                }
            }

            yield return new WaitForEndOfFrame();
        }

        IEnumerator InstantiateCoins(float spawnPosition, float xPosition)
        {
            GameObject banana_inst = Instantiate(banana, new Vector3(2.5f * xPosition, 10f, spawnPosition), Quaternion.identity) as GameObject;

            instantiatedBananas.Add(banana_inst);

            for (int i = 0; i < instantiatedBananas.Count; i++)
            {
                if (instantiatedBananas[i] != null && instantiatedBananas[i].transform.position.z < player.transform.position.z - trackLength / 2f)
                {
                    Destroy(instantiatedBananas[i].gameObject);
                    instantiatedBananas.RemoveAt(i);
                }
            }

            yield return new WaitForEndOfFrame();
        }


        IEnumerator InstantiateTrack(float spawnLocation)
        {

            float chooseTrack = Random.Range(0.0f, 10.0f);

            if (chooseTrack >= narrowTrackProbability)
            {
                GameObject trac = Instantiate(tracks[0], new Vector3(0f, 0f, spawnLocation), Quaternion.identity) as GameObject;
                instiatedTracks.Add(trac);
            }
            else
            {
                GameObject trac = Instantiate(tracks[1], new Vector3(0f, 0f, spawnLocation), Quaternion.identity) as GameObject;
                instiatedTracks.Add(trac);
            }

            float difference = trackLength / spawnPositionsPerTrack;
            float spawnPositionDifference = spawnLocation - Mathf.Floor(spawnPositionsPerTrack / 2) * difference;

            for (int i = 1; i < spawnPositionsPerTrack; i++)
            {
                InstantiateObstacleCoroutine = InstantiateObstacle(spawnPositionDifference + difference * i);
                StartCoroutine(InstantiateObstacleCoroutine);
            }

            float coinsDistance = trackLength / bananasPerTrack;
            float coinPositionDifference = spawnLocation - Mathf.Floor(bananasPerTrack / 2) * coinsDistance;
            float theta = -Mathf.PI;
            float thetaDifference = 2.0f * Mathf.PI / bananasPerTrack;
            for (int i = 1; i < bananasPerTrack; i++)
            {
                float xPosition = Mathf.Sin(theta);
                InstantiateCoinsCoroutine = InstantiateCoins(coinPositionDifference + coinsDistance * i, xPosition);
                StartCoroutine(InstantiateCoinsCoroutine);
                theta = theta + thetaDifference;
            }

            for (int i = 0; i < instiatedTracks.Count; i++)
            {
                if (instiatedTracks[i].transform.position.z < player.transform.position.z - trackLength)
                {
                    Destroy(instiatedTracks[i].gameObject);
                    instiatedTracks.RemoveAt(i);
                }
            }

            yield return new WaitForEndOfFrame();
        }

        IEnumerator Sprint()
        {
            StaticVariable.bananas -= 90;
            yield return new WaitForSeconds(3.0f);
            editorVelocity = editorVelocityFixed;
            sprinting = false;
        }

        #region Jumping Code
        IEnumerator DisableJump()
        {
            yield return new WaitForSeconds(1.3f);
            jumping = false;
        }
        #endregion

        /// <summary>
        /// Function to calculate Variance or Standard Deviation
        /// </summary>
        /// <param name="values">List of floating values</param>
        /// <param name="variance">Boolean if true returns Variance, if not provided or false, returns Standard Deviation</param>
        /// <returns>Variance or Standard Deviation from a list of floating points</returns>
        public static float GetVariation(List<float> values, bool variance = false)
        {
            if (values.Count > 0)
            {
                float avg = values.Average();
                float sum = values.Sum(v => (v - avg) * (v - avg));
                float denominator = values.Count - 1;
                if (variance)
                    return denominator > 0.0 ? (sum / denominator) : 0.0f;
                else
                    return denominator > 0.0 ? Mathf.Sqrt(sum / denominator) : 0.0f;
            }
            return 0;
        }
    }
}