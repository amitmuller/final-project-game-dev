using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [Header("Line Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.1f;
    
    private LineRenderer _currentLineRenderer;
    private List<Vector3> _pointsList;

    void Start()
    {
        _pointsList = new List<Vector3>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            CreateNewLine();
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            
            if (_currentLineRenderer != null)
            {
                if (_pointsList.Count == 0 || Vector3.Distance(_pointsList[_pointsList.Count - 1], mousePos) > 0.1f)
                {
                    AddPoint(mousePos);
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Finished drawing stroke with " + _pointsList.Count + " points.");
        }
    }
    
    void CreateNewLine()
    {
        GameObject lineObj = new GameObject("Line");
        _currentLineRenderer = lineObj.AddComponent<LineRenderer>();
        _currentLineRenderer.material = lineMaterial;
        _currentLineRenderer.startWidth = lineWidth;
        _currentLineRenderer.endWidth = lineWidth;
        _currentLineRenderer.positionCount = 0;
        _currentLineRenderer.numCapVertices = 5; // Smooth round caps
        
        _pointsList.Clear();
    }
    void AddPoint(Vector3 point)
    {
        _pointsList.Add(point);
        _currentLineRenderer.positionCount = _pointsList.Count;
        _currentLineRenderer.SetPositions(_pointsList.ToArray());
    }
}
