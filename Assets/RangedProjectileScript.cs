using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RangedProjectileScript : MonoBehaviour
{
    public float speed = 5f;
    public void GoToAndDie(Vector3 targetPosition)
    {
        GetComponent<NetworkObject>().Spawn();
        StartCoroutine(MoveAndDestroy(targetPosition));
    }
    private IEnumerator MoveAndDestroy(Vector2 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Calculate the direction to the target position
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Calculate the angle in degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a rotation quaternion
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Apply the rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.5f);

            yield return null;
        }

       // GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
