using UnityEngine;

public static class Tracer
{
    public static void Trace(Vector2 lastPosition, Vector2 currentPosition, Color color, float duration)
    {
        if (lastPosition != Vector2.zero)
        {
            Debug.DrawLine(currentPosition, lastPosition, color, duration);
        }
    }
    public static void Trace(Vector2 lastPosition, Vector2 currentPosition, Color color)
    {
        Trace(lastPosition, currentPosition, color, 5f);
    }
    public static void Trace(Vector2 lastPosition, Vector2 currentPosition)
    {
        Trace(lastPosition, currentPosition, Color.red, 5f);
    }
    public static void Trace(Vector2 lastPosition, Vector2 currentPosition, float duration)
    {
        Trace(lastPosition, currentPosition, Color.red, duration);
    }

    public static void DrawTangental(Vector3 start,Vector3 direction, Color color)
    {
        Debug.DrawLine(start, start + direction, color);
    }
    public static void DrawCentripetal(Vector3 start, Vector3 direction, Color color)
    {
        Debug.DrawLine(start, start + direction, color);
    }

    public static void DrawTangentalandCentripetal(Vector3 start, Vector3 tangentalDirection, Vector3 centripetalDirection, Color tangentialColor, Color CentripetalColor)
    {
        DrawTangental(start, tangentalDirection, tangentialColor);
        DrawCentripetal(start, centripetalDirection, CentripetalColor);
    }

    public static void DrawCircle(Vector3 position, float radius, int segments, Color color, float duration)
    {
        // If either radius or number of segments are less or equal to 0, skip drawing
        if (radius <= 0.0f || segments <= 0)
        {
            return;
        }

        // Single segment of the circle covers (360 / number of segments) degrees
        float angleStep = (360.0f / segments);

        // Result is multiplied by Mathf.Deg2Rad constant which transforms degrees to radians
        // which are required by Unity's Mathf class trigonometry methods

        angleStep *= Mathf.Deg2Rad;

        // lineStart and lineEnd variables are declared outside of the following for loop
        Vector3 lineStart = Vector3.zero;
        Vector3 lineEnd = Vector3.zero;

        for (int i = 0; i < segments; i++)
        {
            // Line start is defined as starting angle of the current segment (i)
            lineStart.x = Mathf.Cos(angleStep * i);
            lineStart.y = Mathf.Sin(angleStep * i);

            // Line end is defined by the angle of the next segment (i+1)
            lineEnd.x = Mathf.Cos(angleStep * (i + 1));
            lineEnd.y = Mathf.Sin(angleStep * (i + 1));

            // Results are multiplied so they match the desired radius
            lineStart *= radius;
            lineEnd *= radius;

            // Results are offset by the desired position/origin 
            lineStart += position;
            lineEnd += position;

            // Points are connected using DrawLine method and using the passed color
            if (duration != 0)
            {
                Debug.DrawLine(lineStart, lineEnd, color, duration);
            }
            else
            {
                Debug.DrawLine(lineStart, lineEnd, color);
            }
        }
    }
    public static void DrawCircle(Vector3 position, float radius, int segments, Color color)
    {
        DrawCircle(position, radius, segments, color, 0);
    }

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if (color == null)
        {
            color = Color.white;
        }
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        textMesh.transform.localScale = 0.1f*Vector3.one;
        return textMesh;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector2 vec = Input.mousePosition;
        Vector3 worldPostion = Camera.main.ScreenToWorldPoint(vec);
        
        return worldPostion;
    }
}
