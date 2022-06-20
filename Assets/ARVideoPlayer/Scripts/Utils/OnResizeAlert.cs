using UnityEngine;

public class OnResizeAlert : MonoBehaviour
{
    public Vector2 InitSize;
    public Vector2 CurrentSize;
    private RectTransform rt;

    // Start is called before the first frame update
    private void Start()
    {
        rt = (RectTransform)transform;
        InitSize = rt.sizeDelta;
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentSize = rt.sizeDelta;
        if (CurrentSize!=InitSize)
        {
            Debug.Log("Resized!!!!");
        }
    }
}