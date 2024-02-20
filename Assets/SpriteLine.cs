using UnityEngine;

public class SpriteLine : MonoBehaviour
{
    /*public Transform ConnectTo;
    public bool UpdateChain = true;
    public float spriteScale = 1f;

    void Update()
    {
        LineRenderer renderer = GetComponent<LineRenderer>();
        if (renderer != null)
        {
            if (ConnectTo != null)
            {
                if (UpdateChain)
                {
                    int spriteCount = Mathf.FloorToInt(Vector3.Distance(ConnectTo.position, transform.position) / spriteScale);

                    Vector3[] positions = new Vector3[] {
                    transform.position,
                    (ConnectTo.position - transform.position).normalized * spriteScale * spriteCount
                };

                    renderer.SetVertexCount(positions.Length);
                    renderer.SetPositions(positions);

                    if (renderer.material != null)
                        renderer.material.mainTextureScale = new Vector2(spriteScale * spriteCount, 1);
                    else
                        Debug.LogError(name + "'s Line Renderer has no material!");
                }
            }
            else
            {
                renderer.SetVertexCount(0);
            }
        }
        else
        {
            Debug.Log(name + " has no LineRenderer component!");
        }
    }*/
}
