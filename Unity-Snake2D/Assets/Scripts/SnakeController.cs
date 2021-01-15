using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
	#region Private Properties

	private List<GameObject> _snakeObjectList;                                  // List of snake part gameobject
	private Vector2 _direction;                                                 // Direction of movement

	private float _speed = 0.5f;                                                // Speed value
	private float _angle;                                                       // Angle, use during movement

    #endregion

    #region Inspector Properties

    [Header("Snake Setting")]
    [SerializeField] private GameObject _SnakeBodyGameObject;                   // Snake body prefab.

    [Header("Snake Spirte Setting")]
    [SerializeField] private List<Sprite> _SnakeHeadSpriteList;                 // Snake head sprite list.
    [SerializeField] private List<Sprite> _SnakeBodySpriteList;                 // Snake body sprite list.
    [SerializeField] private List<Sprite> _SnakeTailSpriteList;                 // Snake tail sprite list.

    #endregion

    #region Events

    public static Action OnEatenFood;                                           // Snake eat food event.

    #endregion

    #region Methods

    #region Unity Callback Methods

    /// <summary>
    /// Initialize data in this awake method.
    /// </summary>
    private void Awake()
    {
        _snakeObjectList = new List<GameObject>();

        for(int i = 0; i < transform.childCount; i++)
        {
            _snakeObjectList.Add(transform.GetChild(i).gameObject);
        }

        _direction = Vector3.right;
    }

    /// <summary>
    /// Use this start method to looping the snake movement.
    /// </summary>
    /// <returns></returns>
	private IEnumerator Start()
    {
        // While the game is not over, keep continue the movement, else break this loop.
        while (true)
        {
            yield return new WaitUntil(() => GameManager.Instance.IsStart == true);

            if (GameManager.Instance.IsOver)
            {
                StopAllCoroutines();
                break;
            }
            yield return StartCoroutine(MoveHead());
        }
    }

    /// <summary>
    /// Use update method to check turn movement.
    /// </summary>
    private void Update()
    {
        // Check turn movment in this update method.
        Turn();
    }

    /// <summary>
    /// Subscribe event.
    /// </summary>
    private void OnEnable()
    {
        GameManager.OnSpeedRateChange += UpdateGlobalSpeed;
    }

    /// <summary>
    /// Unsubscribe event.
    /// </summary>
    private void OnDisable()
    {
        GameManager.OnSpeedRateChange -= UpdateGlobalSpeed;
    }

    #endregion

    #region Control Methods

    /// <summary>
    /// Call this method to move the snake's head.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveHead()
    {
        if (GameManager.Instance.IsOver) yield break;

        yield return new WaitForSeconds(_speed);

        if(transform.childCount > 0)
        {
            Transform headSnake = transform.GetChild(0);
            Vector2 oldPosition = headSnake.position;

            headSnake.Translate(_direction);

            SpriteRenderer spriteRenderer = headSnake.GetComponent<SpriteRenderer>();
            if(spriteRenderer != null)
            {
                spriteRenderer.sprite = GetHeadSprite(headSnake.position, oldPosition);
            }

            // If snake has other than its head, then call move body method.
            if (_snakeObjectList.Count > 1 && _snakeObjectList != null)
            {
                if (_snakeObjectList.Count == 2)
                    MoveTail(oldPosition);
                else
                    MoveBody(1, oldPosition);
            }
        }
    }

    /// <summary>
    /// Call this method to move the snake's body.
    /// </summary>
    /// <param name="index">Current body index</param>
    /// <param name="newPosition">New body position</param>
    private void MoveBody(int index, Vector2 newPosition)
    {
        if (GameManager.Instance.IsOver) return;

        Transform previousBody = transform.GetChild(index - 1);
        Transform currentBody = transform.GetChild(index);
        Vector2 oldPosition = currentBody.position;
        currentBody.position = newPosition;

        SpriteRenderer spriteRenderer = currentBody.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GetBodySprite(previousBody.position, currentBody.position, oldPosition);
        }

        if (!currentBody.gameObject.activeSelf)
            currentBody.gameObject.SetActive(true);

        index++;

        if (index < transform.childCount && index != transform.childCount- 1)
        {
            MoveBody(index, oldPosition);
        }
        else if(index < transform.childCount && index == transform.childCount - 1)
        {
            MoveTail(oldPosition);
        }
    }

    /// <summary>
    /// Call this method to move the snake's tail.
    /// </summary>
    /// <param name="newPosition">New snake's tail position.</param>
    private void MoveTail(Vector2 newPosition)
    {
        if (GameManager.Instance.IsOver) return;

        Transform previousBody = transform.GetChild(transform.childCount - 2);
        Transform tailTransform = transform.GetChild(transform.childCount - 1);
        tailTransform.position = newPosition;

        SpriteRenderer spriteRenderer = tailTransform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = GetTailSprite(previousBody.position, tailTransform.position);
        }

        if (!tailTransform.gameObject.activeSelf)
            tailTransform.gameObject.SetActive(true);
    }

    /// <summary>
    /// Call this method to turn the snake head.
    /// </summary>
    private void Turn()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            if(_direction + Vector2.up != Vector2.zero)
            {
                _direction = Vector2.up;
            }
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            if (_direction + Vector2.left != Vector2.zero)
            {
                _direction = Vector2.left;
            }
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            if (_direction + Vector2.down != Vector2.zero)
            {
                _direction = Vector2.down;
            }
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            if (_direction + Vector2.right != Vector2.zero)
            {
                _direction = Vector2.right;
            }
        }
    }

    #endregion

    #region Update Snake Methods

    /// <summary>
    /// Call this method to add body game object into snake.
    /// </summary>
    /// <param name="position">body's position.</param>
    public void AddBodies(Vector3 position)
    {
        GameObject body = Instantiate(_SnakeBodyGameObject, position, Quaternion.identity, transform);
        _snakeObjectList.Add(body);
        OnEatenFood?.Invoke();
    }

    /// <summary>
    /// Call this method to update global speed of snake.
    /// </summary>
    private void UpdateGlobalSpeed()
    {
        _speed *= GameManager.Instance.SpeedRate;
    }

    /// <summary>
    /// Call this method to get the new head sprite.
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="currentPos"></param>
    /// <returns></returns>
    private Sprite GetHeadSprite(Vector2 newPos, Vector2 currentPos)
    {
        Vector2 vector = newPos - currentPos;

        if(vector == Vector2.up)
        {
            return _SnakeHeadSpriteList[(int)HeadAndTailSprite.Top];
        }
        else if (vector == Vector2.right)
        {
            return _SnakeHeadSpriteList[(int)HeadAndTailSprite.Right];
        }
        else if (vector == Vector2.down)
        {
            return _SnakeHeadSpriteList[(int)HeadAndTailSprite.Bottom];
        }
        else if (vector == Vector2.left)
        {
            return _SnakeHeadSpriteList[(int)HeadAndTailSprite.Left];
        }

        return null;
    }

    /// <summary>
    /// Call this method to get the new body sprite.
    /// </summary>
    /// <param name="otherCurrentPos"></param>
    /// <param name="yourNewPos"></param>
    /// <param name="yourCurrentPos"></param>
    /// <returns></returns>
    private Sprite GetBodySprite(Vector2 otherCurrentPos, Vector2 yourNewPos, Vector2 yourCurrentPos)
    {
        Vector2 firstVector = otherCurrentPos - yourNewPos;
        Vector2 secondVector = yourNewPos - yourCurrentPos;

        if ((firstVector == Vector2.up && secondVector == Vector2.up) || firstVector == Vector2.down && secondVector == Vector2.down)
        {
            return _SnakeBodySpriteList[(int)BodySprite.Verticle];
        }
        else if ((firstVector == Vector2.left && secondVector == Vector2.left) || firstVector == Vector2.right && secondVector == Vector2.right)
        {
            return _SnakeBodySpriteList[(int)BodySprite.Horizontal];
        }
        else if ((firstVector == Vector2.up && secondVector == Vector2.right) || (firstVector == Vector2.left && secondVector == Vector2.down))
        {
            return _SnakeBodySpriteList[(int)BodySprite.BottomLeft];
        }
        else if ((firstVector == Vector2.right && secondVector == Vector2.up) || (firstVector == Vector2.down && secondVector == Vector2.left))
        {
            return _SnakeBodySpriteList[(int)BodySprite.TopRight];
        }
        else if ((firstVector == Vector2.left && secondVector == Vector2.up) || (firstVector == Vector2.down && secondVector == Vector2.right))
        {
            return _SnakeBodySpriteList[(int)BodySprite.TopLeft];
        }
        else if ((firstVector == Vector2.right && secondVector == Vector2.down) || (firstVector == Vector2.up && secondVector == Vector2.left))
        {
            return _SnakeBodySpriteList[(int)BodySprite.BottomRight];
        }

        return null;
    }

    /// <summary>
    /// Call this method to get the new tail sprite.
    /// </summary>
    /// <param name="newPos"></param>
    /// <param name="currentPos"></param>
    /// <returns></returns>
    private Sprite GetTailSprite(Vector2 newPos, Vector2 currentPos)
    {
        Vector2 vector = newPos - currentPos;

        if (vector == Vector2.up)
        {
            return _SnakeTailSpriteList[(int)HeadAndTailSprite.Top];
        }
        else if (vector == Vector2.right)
        {
            return _SnakeTailSpriteList[(int)HeadAndTailSprite.Right];
        }
        else if (vector == Vector2.down)
        {
            return _SnakeTailSpriteList[(int)HeadAndTailSprite.Bottom];
        }
        else if (vector == Vector2.left)
        {
            return _SnakeTailSpriteList[(int)HeadAndTailSprite.Left];
        }

        return null;
    }

    #endregion

    #endregion
}
