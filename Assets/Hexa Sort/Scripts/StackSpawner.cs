using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StackSpawner : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform stackPositionParent;
    [SerializeField] private Hexagon hexagonPrefab;
    [SerializeField] private HexStack hexagonStackPrefab;

    [Header("Settings")]  
    [NaughtyAttributes.MinMaxSlider(2, 8)]
    [SerializeField] private Vector2Int minMaxHexCount;
    [SerializeField] private Color[] colors;
    private int stackCounter;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        StackContoller.onStackPlaced += StackPlacedCallback;
    }

    private void OnDestroy()
    {
        StackContoller.onStackPlaced -= StackPlacedCallback;

    }

    private void StackPlacedCallback(GridCell cell)
    {
        stackCounter++;
        if (stackCounter >= 3)
        {
            stackCounter = 0;
            GenerateStacks();
        }
    }

    //First we are going to create the stack and then drop the hexagons inside of that stack
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateStacks();
    }

    private void GenerateStacks()
    {
        for (int i = 0; i < stackPositionParent.childCount; i++)
        {
            GenerateStack(stackPositionParent.GetChild(i));
        }
    }

    private void GenerateStack(Transform parent)
    {
        HexStack hexStack = Instantiate(hexagonStackPrefab, parent.position, Quaternion.identity, parent);
        hexStack.name = $"Stack {parent.GetSiblingIndex()}";

        int amount = Random.Range(minMaxHexCount.x, minMaxHexCount.y);

        int firstColorHexagonCount = Random.Range(0, amount);

        Color[] colorArray = GetRandomColors();

        for (int i = amount - 1; i >= 0; i--)
        {
            // if the index is less than the first color hexagon count we gonna grap first color otherwise second one
            Vector3 hexagonLocalPosition = Vector3.up * i * .2f; //Because i know that one hexagon is .2 deep
            Vector3 spawnPosition = hexStack.transform.TransformPoint(hexagonLocalPosition);
            Hexagon hexagonInstance = Instantiate(hexagonPrefab, spawnPosition, Quaternion.identity, hexStack.transform);

            hexagonInstance.color = i < firstColorHexagonCount ? colorArray[0] : colorArray[1];

            hexagonInstance.Configure(hexStack);

            //Adding to the stack
            hexStack.Add(hexagonInstance);
        }
    }


    private Color[] GetRandomColors() // to make sure always grab two random colors from private Color[] colors array!
    {
        List<Color> colorList = new List<Color>();
        colorList.AddRange(colors);
        if (colorList.Count <= 0)
        {
            Debug.LogError("No Color Found!");
            return null;
        }

        Color firstColor = colorList.OrderBy(x => Random.value).First(); //Shuffle the colors and grab the first element 
        colorList.Remove(firstColor);

        if (colorList.Count <= 1)
        {
            Debug.LogError("Only one color found");
            return null;
        }

        Color secondColor = colorList.OrderBy(x => Random.value).First();

        return new Color[] { firstColor, secondColor };
    }
}
