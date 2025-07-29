using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Tower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fillIncrement;
    private Renderer renderer;
    private float fillPercent;
    [Header("Elements")]
    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    void Start()
    {
        UpdateMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
            Fill();
    }

    private void Fill()
    {
        if (fillPercent >= 1)
            return;

        fillPercent += fillIncrement;
        UpdateMaterials();

        animator.Play("Bump");
    }

    private void UpdateMaterials()
    {
        foreach (Material material in renderer.materials)
        {
            material.SetFloat("_Fill_Percent", fillPercent);
        }
    }

}
