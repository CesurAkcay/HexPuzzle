using System.Collections.Generic;
using UnityEngine;

public class HexStack : MonoBehaviour
{

    public void Initialize()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Add(transform.GetChild(i).GetComponent<Hexagon>());
        }

        Place();
    }
    public List<Hexagon> Hexagons { get; private set; }
    public int Count { get; internal set; }

    public void Add(Hexagon hexagon)
    {
        if (Hexagons == null)
        {
            Hexagons = new List<Hexagon>();
        }

        Hexagons.Add(hexagon);
        hexagon.SetParent(transform);
    }
    public void Remove(Hexagon hexagon)
    {
        Hexagons.Remove(hexagon);

        if (Hexagons.Count <= 0)
            DestroyImmediate(gameObject);
    }

    public Color GetTopHexagonColor()
    {
        return Hexagons[^1].color; // this is how you return the last index of a list
        //return Hexagons[Hexagons.ChildCount - 1].color;
    }

    public void Place()
    {
        foreach (Hexagon hexagon in Hexagons)
        {
            hexagon.DisableCollider();
        }
    }

    public bool Contains(Hexagon hexagon)
    {
        return Hexagons.Contains(hexagon);
    }
}
