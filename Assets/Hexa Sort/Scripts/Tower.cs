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
        Fill(fillIncrement);
    }
    public void Fill(float increment)
    {
        if (fillPercent >= 1)
            return;

        fillPercent += increment;
        fillPercent = Mathf.Clamp01(fillPercent); // Keep between 0 and 1
        
        UpdateMaterials();
        
        if (animator != null)
            animator.Play("Bump");
    }

    public float GetFillPercent()
    {
        return fillPercent;
    }

    private void UpdateMaterials()
    {
        foreach (Material material in renderer.materials)
        {
            material.SetFloat("_Fill_Percent", fillPercent);
        }
    }

}
