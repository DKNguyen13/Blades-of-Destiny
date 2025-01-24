using UnityEngine;

[RequireComponent(typeof(Renderer))]//Đảm bảo gameObject luôn chứa Renderer
public class SortingLayerSetter : MonoBehaviour
{
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sortingLayerName = sortingLayerName;
        renderer.sortingOrder = sortingOrder;
    }
}
