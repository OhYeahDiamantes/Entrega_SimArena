using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 30;
    public float updateTime = 0.1f;
    public GameObject cellPrefab;

    [Header("Sand Simulation Settings")]
    public Color sandColor = new Color(0.96f, 0.82f, 0.44f);
    public Color emptyColor = Color.white;

    [Header("Click & Spread Settings")]
    public int spawnRadius = 2;
    [Range(0f, 1f)] public float spreadProbability = 0.5f; // Probabilidad de que la arena se mueva lateralmente en cada paso
    public float lifetimeOfSand = 10f; // Tiempo en segundos antes de que una partícula de arena desaparezca


    private float[,] sandGrid;
    private float[,] nextSandGrid;
    private GameObject[,] cellObjects;
    private float timer;
    private bool isPaused = false;

    //Bizarrap sacó a Daddy Yankee del retiro kjsncknskjn tremendo
    void Start()
    {
        sandGrid = new float[width, height];
        nextSandGrid = new float[width, height];
        cellObjects = new GameObject[width, height];
        GenerateGrid();
    }

    void Update()
    {
        // Detectar clic del ratón para generar arena
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            GenerateSandOnClick();
        }

        if (isPaused) return;

        timer += Time.deltaTime;
        if (timer >= updateTime)
        {
            Step();
            UpdateVisuals();
            timer = 0f;
        }

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            isPaused = !isPaused;
            Debug.Log($"Simulación {(isPaused ? "PAUSADA" : "REANUDADA")}");
        }
    }

    //SI ME PREGUNTAN A NADIE YO LE DEBOOOOOO
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                cell.transform.parent = transform;
                cellObjects[x, y] = cell;
            }
        }
        UpdateVisuals();
    }

    // Genera arena en la posición del clic del ratón
    void GenerateSandOnClick()
    {
        // Obtener la posición del ratón en coordenadas de pantalla
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Convertir a coordenadas de la cuadrícula
        int gridX = Mathf.RoundToInt(mouseWorldPos.x);
        int gridY = Mathf.RoundToInt(mouseWorldPos.y);

        // Generar arena en un radio alrededor del clic
        for (int x = gridX - spawnRadius; x <= gridX + spawnRadius; x++)
        {
            for (int y = gridY - spawnRadius; y <= gridY + spawnRadius; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height &&
                    Vector2.Distance(new Vector2(gridX, gridY), new Vector2(x, y)) <= spawnRadius)
                {
                    sandGrid[x, y] = lifetimeOfSand;
                }
            }
        }
        UpdateVisuals();
    }

    //CUANDO ME VAYA DE AQUÍ NADA ME LLEVOOOOO
    //SÓLO ME VOY CON UN AMOR VERDADEROOOOOO

    // Aplica las reglas de la arena que se esparce
    void Step()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Reducir el tiempo de vida. Si llega a 0 o menos, la arena desaparece.
                nextSandGrid[x, y] = sandGrid[x, y] - updateTime;
                if (nextSandGrid[x, y] < 0) nextSandGrid[x, y] = 0;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (sandGrid[x, y] > 0)
                {
                    if (nextSandGrid[x, y] <= 0) continue;

                    // Intentar moverse lateralmente
                    if (Random.value < spreadProbability)
                    {
                        int direction = Random.Range(0, 2); // 0 para izquierda, 1 para derecha
                        int targetX = x;

                        if (direction == 0) // Intentar izquierda
                        {
                            targetX = x - 1;
                        }
                        else // Intentar derecha
                        {
                            targetX = x + 1;
                        }

                        // Verificar límites y colisiones
                        if (targetX >= 0 && targetX < width && nextSandGrid[targetX, y] <= 0)
                        {
                            nextSandGrid[targetX, y] = sandGrid[x, y]; // Mover la arena
                            nextSandGrid[x, y] = 0;
                        }
                    }
                }
            }
        }
        var temp = sandGrid;
        sandGrid = nextSandGrid;
        nextSandGrid = temp;
    }

    //Súbale mambo pa' que mi gata prenda lo' motore'
    void UpdateVisuals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var rend = cellObjects[x, y].GetComponent<SpriteRenderer>();
                if (sandGrid[x, y] > 0)
                {
                    rend.color = sandColor;
                }
                else
                {
                    rend.color = emptyColor;
                }
            }
        }
    }
}

//Tremendo temazo se aventaron naa