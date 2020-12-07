using UnityEngine;

public class ScrollUVWater : MonoBehaviour
{
    public  Vector2 AnimatRate = new Vector2(0.0f, 0.0f);
    private Vector2 UVOffs     = Vector2.zero;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        UVOffs += AnimatRate * Time.deltaTime;
        GetComponent<Renderer>().materials[0].SetTextureOffset("_MainTex", UVOffs);
    }
}