using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new Collider collider;
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

    public void MoveToLocal(Vector3 targerLocalPos)
    {
        LeanTween.cancel(gameObject);

        float delay = transform.GetSiblingIndex() * .01f;

        LeanTween.moveLocal(gameObject, targerLocalPos, .2f)
            .setEase(LeanTweenType.easeInOutSine)
            .setDelay(delay);

        Vector3 direction = (targerLocalPos - transform.localPosition).With(y: 0).normalized;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        LeanTween.rotateAround(gameObject, rotationAxis, 180, .2f)
            .setEase(LeanTweenType.easeInOutSine)
            .setDelay(delay);
    }

    public void Vanish(float delay)
    {
        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, Vector3.zero, .2f)
        .setEase(LeanTweenType.easeInOutSine)
        .setDelay(delay)
        .setOnComplete(() => Destroy(gameObject));
    }
}
