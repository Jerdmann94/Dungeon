using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Core_Scripts.EnemyScripts
{
    public class EnemyMovementController : MonoBehaviour
    {
        private float speed;
        public float buffSpeed;
        public SpriteRenderer spriteObj;
        
        public float Speed
        {
            get => speed + buffSpeed;
            set => speed = value;
        }


        
        private Vector3 targetPosition;
        private bool isMoving = false;
        private float movementSpeed = 5f;

        public void DoEnemyMovement(Vector3 position)
        {
            var time = SpeedCalculator.GetSpeedTimer(speed+ buffSpeed) -0.1f;
            if (isMoving)
            {
                // If the enemy is already moving, stop the current movement
                StopMovement();
                return;
            }

            targetPosition = GetClosestCardinalPosition(position);
            StartCoroutine(MoveToTargetPosition(targetPosition, time));
        }

        private IEnumerator MoveToTargetPosition(Vector3 position, float time)
        {
            isMoving = true;
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                float t = elapsedTime / time;
                transform.position = Vector3.Lerp(startPosition, position, t);
                var dir = (position - startPosition).normalized;
                spriteObj.transform.rotation = GetCardinalRotation(dir);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = position;
            SnapToWholeNumberGrid();
            var dirEnd = (position - startPosition).normalized;
            spriteObj.transform.rotation = GetCardinalRotation(dirEnd);
            isMoving = false;
        }

        private void StopMovement()
        {
            StopAllCoroutines();
            //isMoving = false;
        }

        private Vector3 GetClosestCardinalPosition(Vector3 position)
        {
            // Round the position to the nearest whole number
            Vector3 roundedPosition = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));

            // Restrict movement to cardinal directions (x or y)
            if (Mathf.Abs(roundedPosition.x - transform.position.x) > Mathf.Abs(roundedPosition.y - transform.position.y))
            {
                return new Vector3(roundedPosition.x, transform.position.y, transform.position.z);
            }
            else
            {
                return new Vector3(transform.position.x, roundedPosition.y, transform.position.z);
            }
        }
        public Quaternion GetCardinalRotation(Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, angle);
        }
        private void SnapToWholeNumberGrid()
        {
            Vector3 snappedPosition = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            transform.position = snappedPosition;
            isMoving = false;
        }
    }
    
    
}