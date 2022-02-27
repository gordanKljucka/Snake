using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    int[,] grid;
    public Material colorSpace;
    public Material materialGreen;
    public GameObject[,] cubeArr;
    int sizeX = 15, sizeZ = 15;

    // pozicija
    public int plX, plZ;
    public int smjerX = 1, smjerZ = 0;
    public int speed;

    public List<int[]> tail;
    int tailLength = 7;

    public float timer;
    public float timerMax;
    bool isMoving = true;

    GameObject world;


    private void Start()
    {
        world = new GameObject("World");
        speed = 1;
        timerMax = 1.0f;
        timer = timerMax;
        grid = new int[sizeX, sizeZ];
        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeZ; j++)
            {
                grid[i, j] = 0;
                if (i == 0 || i == sizeX - 1 || j == 0 || j == sizeZ - 1)
                {
                    grid[i, j] = 1; //  radi rub
                }
            }
        }

        cubeArr = new GameObject[sizeX, sizeZ];
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                SpawnCubes(i, 0, j);
            }
        }

        // poicioniranje playera
        plX = Random.Range(3, sizeX - 3);
        plZ = Random.Range(3, sizeZ - 3);
        ChangeCubeColor(plX, plZ, materialGreen);

        timer = 1.0f;
        tail = new List<int[]>();
        tail.Add(new int[] { plX, plZ });

        PowerUp();
    }

    void PowerUp()
    {
        int x = Random.Range(1, sizeX - 1);
        int z = Random.Range(1, sizeZ - 1);
        for(int i = 0; i < tail.Count; i++)
        {
            if(x == tail[i][0] && z == tail[i][1])
            {
                PowerUp();
            }
        }
        grid[x, z] = 2;
        ChangeCubeColor(x, z, materialGreen);
    }

    void Movement() //  problem je u invoke repeatingu kada zovemo metodu sa parametrima
    {
        plX += smjerX;
        plZ += smjerZ;
        print("vrijednost polja " + grid[plX, plZ]);
        if(grid[plX, plZ] == 1)
        {
            speed = 0;
            ChangeCubeColor(0, 0, materialGreen);
            isMoving = false;
            print("gameOver");
        }
        else
        {
            ChangeCubeColor(plX, plZ, materialGreen);
            print("AAAAA " + grid[plX, plZ]);

            tail.Insert(0, new int[] { plX, plZ });     //  nulti
            if(tail.Count >= 2 && tail[1] != null)
            {
                grid[tail[1][0], tail[1][1]] = 55;           // prvi, sprijecava da se vratimo u istom smjeru

            }
            if(tail.Count >= 3 && tail[2] != null)
            {
                grid[tail[2][0], tail[2][1]] = 1;            // drugi, daje vrijednost zida //  tu ga nesta jebe
            }
            if (tail.Count >= tailLength)
            {
                grid[tail[tail.Count - 1][0], tail[tail.Count - 1][1]] = 0;
                ChangeCubeColor(tail[tail.Count - 1][0], tail[tail.Count - 1][1], colorSpace);
                tail.RemoveAt(tail.Count - 1);
            }
        }

        if(grid[plX, plZ] == 2)
        {
            tailLength++;
            // dodaj bod;
            grid[plX, plZ] = 0;
            timerMax -= 0.05f;
            PowerUp();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0 && isMoving == true)
        {
            Movement();
            timer = timerMax;
        }
        if(Input.GetKeyDown(KeyCode.UpArrow) && grid[plX, plZ + 1] != 55)   //   radi
        {
            smjerX = 0;
            smjerZ = speed;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && grid[plX, plZ - 1] != 55)
        {
            smjerX = 0;
            smjerZ = -speed;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && grid[plX - 1, plZ] != 55)
        {
            smjerX = -speed;
            smjerZ = 0;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && grid[plX + 1, plZ] != 55)
        {
            smjerX = speed;
            smjerZ = 0;
        }
    }

    public void SpawnCubes(int x, int y, int z) //  MOZDA da dodam transform
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.transform.SetParent(world.transform);
        g.name = "x : " + x + ", z : " + z;
        g.transform.position = new Vector3(x, y, z);

        Renderer rend = g.GetComponent<Renderer>();
        rend.material = colorSpace;
        if (grid[x, z] == 1)
        {
            g.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //rend.material.color = new Color(0.02006942f, 0.3867925f, 0.2353655f);
            //rend.material.color = new Color(0, 1, 0);
            rend.material = materialGreen;
        }
        cubeArr[x, z] = g;
    }

    void ChangeCubeColor(int x, int z, Material mat)  //  dodaj parametat "materijal"
    {
        Renderer rend = cubeArr[x, z].GetComponent<Renderer>();
        rend.material = mat;
    }
}
