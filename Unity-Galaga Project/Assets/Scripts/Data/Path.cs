//  Path.cs
//  By Atid Puwatnuttasit

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    #region Inspector Properties

    [Header("DEBUG")]
    [SerializeField] protected Color _PathColor;                                            // Color for visualize on debug mode.
    [SerializeField] [Range(1, 30)] protected int _LineDensity = 1;                         // Bezier line precision node.
    [SerializeField] protected List<Transform> _PathList;                                   // Way-point path list.
    [SerializeField] protected List<Vector3> _BezierPathList = new List<Vector3>();         // Bezier way-point path list.
    [SerializeField] protected bool _ShowFormation;                                         // Debug enable state.

    #endregion

    #region Public Properties

    public List<Transform> PathList => _PathList;
    public List<Vector3> BezierPathList => _BezierPathList;

    #endregion

    #region Protected Properties
    protected int _overload;
    #endregion

    #region Methods

    #region Built-in Methods

    private void Start()
    {
        GeneratePath();
    }

    // Debug mode method.
    protected void OnDrawGizmos()
    {
        // If the debug mode is disable, return this method.
        if (!_ShowFormation) return;

        Gizmos.color = _PathColor;
        _PathList = gameObject.GetComponentsInChildren<Transform>().ToList();
        _PathList.Remove(this.transform);

        // Draw the way-point.
        for (int i = 0; i < _PathList.Count; i++)
        {
            Vector3 pos = _PathList[i].position;
            Gizmos.DrawWireSphere(pos, 0.5f);
            if (i > 0)
            {
                Vector3 previousPos = _PathList[i - 1].position;
                Gizmos.DrawLine(previousPos, pos);
            }
        }

        // Draw curved path

        // Check overload
        if (_PathList.Count % 2 == 0)
        {
            _PathList.Add(_PathList[_PathList.Count - 1]);
            _overload = 2;
        }
        else
        {
            _PathList.Add(_PathList[_PathList.Count - 1]);
            _PathList.Add(_PathList[_PathList.Count - 1]);
            _overload = 3;
        }

        // Bezier curved creating
        _BezierPathList.Clear();

        Vector3 lineStart = _PathList[0].position;

        // Visualize way-point.
        for (int i = 0; i < _PathList.Count - _overload; i+=2)
        {
            for (int j = 0; j <= _LineDensity ; j++)
            {
                Vector3 lineEnd = GetQuadraticBezierPoint(
                    _PathList[i].position
                    , _PathList[i + 1].position
                    , _PathList[i + 2].position
                    , j/(float)_LineDensity
                );

                Gizmos.color = Color.red;
                Gizmos.DrawLine(lineStart, lineEnd);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(lineStart, 0.1f);

                lineStart = lineEnd;

                _BezierPathList.Add(lineStart);
            }
        }
    }

    #endregion

    #region Path Generate Methods

    /// <summary>
    /// Call this method to get the Quadratic Bezier curve point.
    /// </summary>
    /// <param name="p0">point 0.</param>
    /// <param name="p1">point 1.</param>
    /// <param name="p2">point 2.</param>
    /// <param name="t"></param>
    /// <returns></returns>
    protected Vector3 GetQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        // B(t) = (1-t)2P0 + 2(1-t)tP1 + t2P2
        // Reference: http://www.theappguruz.com/blog/bezier-curve-in-games
        return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }

    /// <summary>
    /// Call this method to generate paths.
    /// </summary>
    protected void GeneratePath()
    {
        _PathList = gameObject.GetComponentsInChildren<Transform>().ToList();
        _PathList.Remove(this.transform);

        // Check overload
        if (_PathList.Count % 2 == 0)
        {
            _PathList.Add(_PathList[_PathList.Count - 1]);
            _overload = 2;
        }
        else
        {
            _PathList.Add(_PathList[_PathList.Count - 1]);
            _PathList.Add(_PathList[_PathList.Count - 1]);
            _overload = 3;
        }

        // Curved creating
        _BezierPathList.Clear();

        Vector3 lineStart = _PathList[0].position;

        for (int i = 0; i < _PathList.Count - _overload; i+=2)
        {
            for (int j = 0; j <= _LineDensity ; j++)
            {
                Vector3 lineEnd = GetQuadraticBezierPoint(
                    _PathList[i].position
                    , _PathList[i + 1].position
                    , _PathList[i + 2].position
                    , j/(float)_LineDensity
                );

                lineStart = lineEnd;

                _BezierPathList.Add(lineStart);
            }
        }
    }

    #endregion

    #endregion
}
