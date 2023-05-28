using System.Threading.Tasks;
using UnityEngine;

namespace Core_Scripts.EnemyScripts
{
    public class EnemyMovementController : MonoBehaviour
    {

        public float speed;


        private bool isMoving = false;

        public async Task DoEnemyMovement(Vector3 pos)
        {
            Debug.Log("INSIDE ENEMY MOVER");
            if (isMoving)
            {
                return; // Already moving, ignore the request
            }

            isMoving = true;

            float t = 0f;
            Vector3 startPos = transform.position;
            Vector3 endPos = pos;

           
            while (t < 1f)
            {
              
                t += speed * Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                await Task.Yield();
            }

            // Enemy has reached the destination
            // You can add any logic or behavior here

            isMoving = false;
        }
    }
}