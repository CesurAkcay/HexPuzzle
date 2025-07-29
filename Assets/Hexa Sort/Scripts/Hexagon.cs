using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private new Renderer renderer;
     [SerializeField] private new Collider collider ;
    public HexStack HexStack { get; private set; }

    public Color color
    {
        get => renderer.material.color;
        set => renderer.material.color = value;
    }

    // our hexStack is a property so when we configure it, we store the value inside of the property so that we can grab it whenever we want to control our stack

    public void Configure(HexStack hexStack)
    {
        HexStack = hexStack;
    }
    public void DisableCollider() => collider.enabled = false;

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
