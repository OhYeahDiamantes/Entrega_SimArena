using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;                  // Ancho de la cuadrícula
    public int height = 30;                 // Alto de la cuadrícula
    public float updateTime = 0.1f;
    public GameObject cellPrefab;

    [Header("Sand Simulation Settings")]
    [Range(0f, 1f)] public float initialSandProbability = 0.5f;
    [Range(0f, 1f)] public float topSandSpawnProbability = 0.2f;
    public Color sandColor = new Color(0.96f, 0.82f, 0.44f);
    public Color emptyColor = Color.white;

    private bool[,] grid;                   // Cuadrícula actual: true si hay arena, false si está vacío
    private bool[,] nextGrid;               // Cuadrícula para la siguiente generación
    private GameObject[,] cellObjects;
    private float timer;
    private bool isPaused = false;

    void Start()
    {
        grid = new bool[width, height];
        nextGrid = new bool[width, height];
        cellObjects = new GameObject[width, height];

        GenerateGrid();
        RandomizeGrid();
    }
    //Biza sacó a Daddy Yankee del retiro akjskajsk tremendo
    void Update()
    {
        if (isPaused) return;

        timer += Time.deltaTime;
        if (timer >= updateTime)
        {
            Step();                         // Calcular el siguiente estado del tablero
            UpdateVisuals();
            timer = 0f;
        }

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            isPaused = !isPaused;
            Debug.Log($"Simulación {(isPaused ? "PAUSADA" : "REANUDADA")}");
        }
    }

    // Genera el tablero en la escena instanciando cada célula
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Instanciar una célula en la posición (x, y)
                GameObject cell = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                cell.transform.parent = transform;
                cellObjects[x, y] = cell;
            }
        }
    }
    //SI ME PREGUNTAN A NADIE YO LE DEBOOOO
    void RandomizeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = Random.value < initialSandProbability;
            }
        }
        UpdateVisuals();
    }

    // Aplicar arena cayendo
    void Step()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nextGrid[x, y] = false;
            }
        }

        //Generar nueva arena en la fila superior
        for (int x = 0; x < width; x++)
        {
            if (Random.value < topSandSpawnProbability)
            {
                nextGrid[x, height - 1] = true;
            }
        }

        //Súbale mambo pa que mi gata prenda lo' motore'
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y]) // Si hay arena en la celda actual
                {
                    if (y == 0)
                    {
                        continue;
                    }
                    if (!nextGrid[x, y - 1] && y > 0)
                    {
                        nextGrid[x, y - 1] = true; // La arena cae una posición hacia abajo
                    }
                    else // Si no puede caer directamente, intenta moverse en diagonal
                    {
                        bool movedDiagonal = false;
                        int direction = Random.Range(0, 2); // 0 para izquierda, 1 para derecha

                        if (direction == 0)
                        {
                            if (x > 0 && y > 0 && !nextGrid[x - 1, y - 1])
                            {
                                nextGrid[x - 1, y - 1] = true;
                                movedDiagonal = true;
                            }
                            else if (x < width - 1 && y > 0 && !nextGrid[x + 1, y - 1])
                            {
                                nextGrid[x + 1, y - 1] = true;
                                movedDiagonal = true;
                            }
                        }
                        else
                        {
                            if (x < width - 1 && y > 0 && !nextGrid[x + 1, y - 1])
                            {
                                nextGrid[x + 1, y - 1] = true;
                                movedDiagonal = true;
                            }
                            else if (x > 0 && y > 0 && !nextGrid[x - 1, y - 1])
                            {
                                nextGrid[x - 1, y - 1] = true;
                                movedDiagonal = true;
                            }
                        }
                        if (!movedDiagonal)
                        {
                            if (!nextGrid[x, y])
                            {
                                nextGrid[x, y] = true;
                            }
                        }
                    }
                }
            }
        }
        var temp = grid;
        grid = nextGrid;
        nextGrid = temp;
    }
    //CUANDO ME VAYA DE AQUÍ NADA ME LLEVOOOO
    //SOLO ME VOY CON UN AMOR VERDADEROOOOOO

    // Cambia el color de las células según su estado (arena o vacío)
    void UpdateVisuals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var rend = cellObjects[x, y].GetComponent<SpriteRenderer>();
                rend.color = grid[x, y] ? sandColor : emptyColor;
            }
        }
    }
}
//Na q temazo se sacaron