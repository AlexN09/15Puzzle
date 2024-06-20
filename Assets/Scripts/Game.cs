using UnityEngine.UI;
using UnityEngine;
using System;
using System.Linq;
using System.Collections;


public class Game : MonoBehaviour
{
    public Text text;
    public int EmptyCellX, EmptyCellY,movesCount;
    public static int fieldSize = 4;
    public GameObject cellPrefab, emptyCell,winPanel;
    GameObject[,] field;
    public GameObject[] numbers, InstantiatedNumbers;
    public float fieldX, fieldY, cellInterval;  
    System.Random rnd = new System.Random();
    IEnumerator DelayedActions(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        winPanel.SetActive(true);
        SetFieldActive(false);
    }
   
    public void ButtonContinue()
    {
        movesCount = 0;
        text.text = "Moves: " + movesCount.ToString();
        winPanel.SetActive(false);  
        CounterTimer.seconds = 0; CounterTimer.minutes = 0; CounterTimer.hours = 0;
        SetFieldActive(true);
    }
    private void SetFieldActive(bool set)
    {
        int iterator = 0;
     
        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                field[i, j].SetActive(set);
                if (iterator < (fieldSize * fieldSize) - 1)
                {
                    InstantiatedNumbers[iterator].SetActive(set);
                    iterator++;
                }

            }
        }
    }
    public void reset()
    {
        CounterTimer.seconds = 0;CounterTimer.minutes = 0; CounterTimer.hours = 0 ;    
        movesCount = 0;
        text.text = "Moves: " + movesCount.ToString();
        foreach (var obj in InstantiatedNumbers) Destroy(obj);
        int iterator = 0;
        int[] newField = CreateSolvablePuzzle();
        int fieldSize = field.GetLength(0);  
        GameObject[,] temp = new GameObject[fieldSize, fieldSize];
        Vector3[,] newPositions = new Vector3[fieldSize, fieldSize];

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                temp[i, j] = field[i, j];
            }
        }
        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                string objectName = field[i, j].name == "empty" ? "empty" : field[i, j].name;
                int flatIndex = Array.IndexOf(newField, objectName == "empty" ? 0 : int.Parse(objectName));
                int x = flatIndex % fieldSize;
                int y = flatIndex / fieldSize;
                newPositions[i, j] = field[y, x].transform.position;
            }
        }
        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                    string nameToFind = newField[iterator] == 0 ? "empty" : newField[iterator].ToString();
                    int flatIndex = FindFlatIndexByName(temp, nameToFind);
                    int x = flatIndex % fieldSize, y = flatIndex / fieldSize;
                    iterator++;

                    field[i, j] = temp[y, x];
                    temp[i, j].transform.position = newPositions[i, j];
              
            }
        }
        InstantiateNumbers();
    }
    public void InstantiateNumbers()
    {
        int iterator = 0;
        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                if (field[i, j].name != "empty")
                {
                    int nameOfTile = int.Parse(field[i, j].name);

                    InstantiatedNumbers[iterator] = Instantiate(numbers[nameOfTile - 1], field[i, j].transform.position, Quaternion.identity);
                    InstantiatedNumbers[iterator].name = numbers[nameOfTile - 1].name;
                    iterator++;
                }


            }
        }
    }
    public bool IsArrayAscending(int[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            if (array[i] != 0 && array[i + 1] != 0 && array[i + 1] - array[i] != 1)
            {
                return false;
            }
        }
        return true;
    }
    int[] CreateSolvablePuzzle() 
    {
        int[] nums = new int[(fieldSize * fieldSize)];
        int iterations = rnd.Next(100,500);
        nums = Enumerable.Range(0, (fieldSize * fieldSize)).ToArray();
        do
        {
            for (int j = 0; j < iterations; j++)
            {
                for (int i = 0; i < nums.Length - 1; i++)
                {
                    int randomIndex = rnd.Next(0, (fieldSize * fieldSize) - 1);
                    int temp = nums[randomIndex];
                    nums[randomIndex] = nums[randomIndex + 1];
                    nums[randomIndex + 1] = temp;
                }
            }
            iterations = rnd.Next(100, 500);
        } while (!IsSolvable(nums, fieldSize) || IsArrayAscending(nums));


        EmptyCellY = Array.IndexOf(nums, 0) / fieldSize;
      EmptyCellX = Array.IndexOf(nums, 0) % fieldSize;

     
        
        return nums;
    }
    public static bool IsSolvable(int[] puzzle, int size)
    {
        int inversions = 0, emptyRow = 0;

        for (int i = 0; i < puzzle.Length; i++)
        {
            if (puzzle[i] == 0)
            {
                emptyRow = size - (i / size); 
                continue;
            }

            for (int j = i + 1; j < puzzle.Length; j++)
            {
                if (puzzle[j] != 0 && puzzle[i] > puzzle[j])
                {
                    inversions++;
                }
            }
        }

      

        bool isSolvable = size % 2 == 1 ? inversions % 2 == 0 : (inversions % 2 == 0) == (emptyRow % 2 == 1);
      
        return isSolvable;
    }
    int FindFlatIndexByName(GameObject[,] objects, string name)
    {
        int rows = objects.GetLength(0);
        int columns = objects.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (objects[i, j] != null && objects[i, j].name == name)
                {
                   
                    return (i * columns + j);
                }
            }
        }

        return -1;
    }
    int FindObjectIndexByName(GameObject[] objects, string name)
    {
        return System.Array.FindIndex(objects, obj => obj.name == name);
    }
   
    bool WinCheck()
    {

        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                int debug = FindFlatIndexByName(field, field[i, j].name);
                if (field[i, j].name != "empty" && FindFlatIndexByName(field, field[i, j].name) != int.Parse(field[i,j].name) - 1)
                {
                    return false;
                }
            }
        }
        return true;
    }
  
    private void Start()
    {
        text.text = "Moves: " + movesCount.ToString();
        switch (fieldSize)
        {
            case 4:
                fieldX = -1.27f;
                break;
            case 3:
                fieldX = -0.85f;
                break;
            case 2:
                fieldX = -0.35f;
                break;
            default:
                break;
        }

     

        field = new GameObject[fieldSize, fieldSize];
        InstantiatedNumbers = new GameObject[fieldSize * fieldSize - 1];
        //int EmptyCellX = rnd.Next(0, 4), EmptyCellY = rnd.Next(0, 4);
        int name = 0;
        int[] solvableFieldNums = new int[(fieldSize * fieldSize)];
        solvableFieldNums = CreateSolvablePuzzle();
        int iterator1 = 0;
        for (int i = 0; i < fieldSize; i++)
        {
            for (int j = 0; j < fieldSize; j++)
            {
                GameObject cell = null;
                if (j == EmptyCellX && i == EmptyCellY)
                {
                    cell = Instantiate(emptyCell, new Vector2(fieldX + cellInterval * j, fieldY - cellInterval * i), Quaternion.identity);
                    cell.name = "empty";
                }
                else
                {
                    //name++;
                    //name = GetName();   
                    name = solvableFieldNums[iterator1];
                    cell = Instantiate(cellPrefab, new Vector2(fieldX + cellInterval * j, fieldY - cellInterval * i), Quaternion.identity);

                    cell.name = name.ToString();
                    
                }
                iterator1++;
                field[i, j] = cell;


            }
        }
       
        InstantiateNumbers();
    }
    private void Update()
    {
        void moveCell(int directionX,int directionY) //1:left && up || -1:right && down
        {
            string numberNameForFind = "numbers" + field[EmptyCellY + directionY, EmptyCellX + directionX].name;
            int debug = FindObjectIndexByName(InstantiatedNumbers, numberNameForFind);
            InstantiatedNumbers[FindObjectIndexByName(InstantiatedNumbers, numberNameForFind)].transform.position = field[EmptyCellY, EmptyCellX].transform.position;


            Vector2 temp = field[EmptyCellY + directionY, EmptyCellX + directionX].transform.position;
            GameObject tempCell = field[EmptyCellY + directionY, EmptyCellX + directionX];
            field[EmptyCellY + directionY, EmptyCellX + directionX].transform.position = field[EmptyCellY, EmptyCellX].transform.position;
            field[EmptyCellY, EmptyCellX].transform.position = temp;
            field[EmptyCellY+ directionY, EmptyCellX + directionX] = field[EmptyCellY, EmptyCellX];
            field[EmptyCellY, EmptyCellX] = tempCell;
          
            
            if (directionX == -1 || directionX == 1)
            {
                EmptyCellX += directionX;
            }
            else 
            {
                EmptyCellY += directionY;
            }
        }
        bool canMove(int directionX,int directionY)
        {
            if (directionX == 1 || directionX == -1)
            {
                if (EmptyCellX + directionX < fieldSize && EmptyCellX + directionX >= 0)
                {
                    return true;
                }             
            }
            else
            {
                if (EmptyCellY + directionY < fieldSize && EmptyCellY + directionY >= 0)
                {
                    return true;
                }
            }
            return false;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && canMove(1, 0))
        {
            moveCell(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && canMove(-1, 0))
        {

            moveCell(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && canMove(0, -1))
        {
            moveCell(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && canMove(0, 1))
        {
            moveCell(0, 1);
        }
      for (int i = 0; i < fieldSize; i++) 
      {
            for (int j = 0; j < fieldSize; j++)
            {
               TouchInput input = field[i, j].GetComponent<TouchInput>();
                    if (field[i,j].name != "empty" && input.wasTouch && !winPanel.activeInHierarchy)
                    {

                      
                      
                      
                      if (j + 1 < fieldSize && field[i, j + 1].name == "empty")
                      {
                        movesCount++;
                        moveCell(-1, 0);
                      }
                      if (j-1 >= 0 && field[i, j - 1].name == "empty")
                      {
                        movesCount++;
                        moveCell(1, 0);
                      }
                      if (i + 1 < fieldSize && field[i + 1, j].name == "empty")
                      {
                        movesCount++;
                        moveCell(0, -1);
                      }
                    if (i - 1 >= 0 && field[i - 1, j].name == "empty")
                    {
                        movesCount++;
                        moveCell(0, 1);
                    }
                    text.text = "Moves: " + movesCount.ToString();
                    if (WinCheck())
                    {
                        StartCoroutine(DelayedActions(0.2f));
                    }
                    input.wasTouch = false;
                    }
              
            }
      }

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    reset();
        //}
      
      
    }
    
}
