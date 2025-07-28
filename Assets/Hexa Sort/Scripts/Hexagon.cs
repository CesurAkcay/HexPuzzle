using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private new Renderer renderer;

    public Color color
    {
        get => renderer.material.color;
        set => renderer.material.color = value;
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
