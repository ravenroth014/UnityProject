using UnityEngine;

public class SnakeSensor : MonoBehaviour
{
    #region Inspector Properties

    [SerializeField] private SnakeController _SnakeController;                  // Snake controller script.

    #endregion

    #region Methods
    /// <summary>
    /// Collision checking method. 
    /// </summary>
    /// <param name="other">Other collider object</param>
    private void OnTriggerEnter2D(Collider2D collidedObject)
    {
        // if the other is food, increase the length of the tail
        if (collidedObject.CompareTag("Food"))
        {
            _SnakeController.AddBodies(collidedObject.transform.position);
            GameManager.Instance.UpdateScore(this);
            FoodPool.SharedInstance.ReturnFoodToPool(collidedObject.gameObject);
        }
        // if the other is either wall or body of the snake, the game is over.
        else if (collidedObject.CompareTag("Wall") || collidedObject.CompareTag("Body"))
        {
            GameManager.Instance.GameOver(this);
            Destroy(gameObject);
        }
    }
    #endregion
}
