using System;
using System.Collections;
using UnityEngine;
using easyar;

public class GetColorFromCamera : MonoBehaviour
{
    public CameraImageRenderer cameraRenderer;
    private Material _material; 
    public Color MaterialColor { get { return _material.color; } private set { _material.color = value; } }
    private ImageTargetController _imageTarget;
    private RenderTexture _renderTexture;
    private Texture2D _cameraTexture;

    [Range(1,5)] [SerializeField] float _scaleCameraCoefficient = 1f;
    public Vector2[] relativeTargetAnglePoints;
    private Vector3[] _targets;
    [SerializeField] int _calculatingAreaSize = 1;

    [SerializeField] int _textFont = 1;

    private void Awake()
    {
        _imageTarget = GetComponentInParent<ImageTargetController>();
        _material = GetComponent<MeshRenderer>().material;
        cameraRenderer.RequestTargetTexture((camera, texture) => { _renderTexture = texture; });    
        _cameraTexture = new Texture2D(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        // set target points
        _targets = SetTargetPoints();

        // copy texture from RenderTexture to Texture2D
        if (_cameraTexture.width == 0 && _cameraTexture.height == 0) 
            _cameraTexture.Resize(_renderTexture.width, _renderTexture.height); 

        StartCoroutine(GetTextureFromRender());

        // check if the object is within the bounds and assign new color
        if (IsWithinBounds())
        {
            AssignNewColorToObject();
        }

    }

    private Vector3[] SetTargetPoints()
    {
        Vector3[] targets = new Vector3[relativeTargetAnglePoints.Length];

        // local coordinates of image points (see below)
        var halfWidth = 0.5f/_scaleCameraCoefficient;
        var halfHeight = halfWidth / _imageTarget.Target.aspectRatio();

        for (int i = 0; i < relativeTargetAnglePoints.Length; i++)
        {
            float x = relativeTargetAnglePoints[i].x;
            float y = relativeTargetAnglePoints[i].y;
            Vector3 targetAnglePoint = _imageTarget.transform.TransformPoint(new Vector3( x * halfWidth,  y * halfHeight, 0));
            targets[i] = Camera.main.WorldToScreenPoint(targetAnglePoint);
        }

        return targets;
    }

    private bool IsWithinBounds()
    {
        int numberOfItemInBounds = 0;

        if (_targets != null)
        {
            foreach (Vector3 item in _targets)
            {
                numberOfItemInBounds += Convert.ToInt32(IsTargetInBounds(item, _calculatingAreaSize, _cameraTexture));
            }
        }

        return numberOfItemInBounds == _targets?.Length;        
    }

    private void AssignNewColorToObject()
    {
        Color[][] quads = new Color[_targets.Length][];
        Color quadAvg = new Color(0f,0f,0f,0f);

        // get pixels from image areas and calculate average color
        for (int i = 0; i < _targets.Length; i++)
        {
            quads[i] = _cameraTexture.GetPixels((int)_targets[i].x - _calculatingAreaSize, (int)_targets[i].y - _calculatingAreaSize, _calculatingAreaSize, _calculatingAreaSize);
            quadAvg += GetAverageColor(quads[i]);
        }
        quadAvg /= quads.Length;
        
        // if recent object material color is "greater" than calculating average, then assign new material color
        if (isColorGreaterThan(MaterialColor, quadAvg))
            MaterialColor = quadAvg;
    }

    private IEnumerator GetTextureFromRender()
    {
        yield return new WaitForEndOfFrame();
        RenderTexture.active = _renderTexture;
        _cameraTexture.ReadPixels(new Rect(0, 0, _cameraTexture.width, _cameraTexture.height), 0, 0);
        _cameraTexture.Apply();
    }

    private bool isColorGreaterThan(Color lhs, Color rhs)
    {
        return lhs.a > rhs.a || lhs.r > rhs.r ||
               lhs.g > rhs.g || lhs.b > rhs.b;
    }
    
    private bool IsTargetInBounds(Vector3 point, int calculatingAreaSize, Texture2D texture)
    {
        return point.x - calculatingAreaSize > 0 && point.x + calculatingAreaSize < texture.width  &&
               point.y - calculatingAreaSize > 0 && point.y + calculatingAreaSize < texture.height;
    }

    private Color GetAverageColor(Color[] colors)
    {
        Color avg = new Color(0,0,0,0);
        foreach (var color in colors) {
            avg += color;
        }
        avg /= colors.Length;
        return avg;
    }

    void OnValidate()
    {
        foreach (Vector2 item in relativeTargetAnglePoints)
        {
            if (item.x > 1 || item.y > 1 || item.x < -1 || item.y < -1)
            {   
                throw new Exception ("Relative point vector items must be in range [-1;1]!");
            }   
                
        }
    }
    
    void OnGUI()
    {
        if (!IsWithinBounds()) {
            GUIStyle style = new GUIStyle ();
            style.fontSize = (int)(_textFont * _cameraTexture.width / 1000);
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(_cameraTexture.width/2-_cameraTexture.width/8, _cameraTexture.height/2-_cameraTexture.width/8, 
                               _cameraTexture.width/4, _cameraTexture.width/4), 
                               "QR marker is out of bounds\nColor may be incorrect", style);
        }

    }

}
