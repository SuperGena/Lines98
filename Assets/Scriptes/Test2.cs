using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;
using System.Linq;

public class Test2 : MonoBehaviour
{
    private Pathfinding pathfinding;

    private List<Vector2> _ballPositionXY;

    private List<Vector3> pathVectorList;

    private GameObject hitedGameObject;

    private GameObject[,] _ballsArray;

    public int[,] xy;

    private int startX;
    private int startY;

    private int _clickCounter;
    private int _currentPathCount;
    private int _score;

    private bool _isMovingToPosition;
    private bool _gameOver;

    private readonly Random _rnd = new Random();

    public float ballMovementSpeed;

    public int amountBallsToSpawn;

    public GameObject cell;
    public GameObject scorePanel;
    public GameObject startGamePanel;
    public GameObject GameOverPanel;
    public GameObject fadePanel;

    public Animator fadePanelAnimator;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    public GameObject[] ballsToSpawn;

    private bool GameOver
    {
        get
        {
            return _gameOver;
        }
        set
        {
            _gameOver = value;
            if (value)
            {
                bestScoreText.text = PlayerPrefs.GetInt("RecordScore").ToString();
                //GameOverPanel.SetActive(true);
                LoseGame();
            }
        }
    }

    private int GameScore
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            scoreText.text = value.ToString();

            if (value > PlayerPrefs.GetInt("RecordScore"))
            {
                PlayerPrefs.SetInt("RecordScore", value);
            }
        }
    }

    private void Awake()
    {
        pathfinding = new Pathfinding(9, 9, false);
        fadePanelAnimator = fadePanel.GetComponent<Animator>();
    }

    private void Start()
    {
        _ballsArray = new GameObject[pathfinding.GetGrid().GetWidth(), pathfinding.GetGrid().GetHeight()];
        _clickCounter = 0;

        pathfinding.GetGrid().SpawnGridCells(cell);
        SpawnFirstBalls();
        SpawnObjectsOnField();

        GameScore = 0;
    }

    private void Update()
    {
        if (!GameOver)
        {
            MoveObjectByPath(hitedGameObject);

            if(!_gameOver)
            if(!_isMovingToPosition)
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = GetMousePos(Input.mousePosition, Camera.main);

                _clickCounter++;

                if (_clickCounter == 1)
                {
                    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
                          
                    if (x >= 0 && y >= 0 && x <= pathfinding.GetGrid().GetWidth() - 1 && y <= pathfinding.GetGrid().GetHeight() - 1)
                    {
                        if (RaycastHitInfo().transform != null && !pathfinding.GetGrid().GetGridObject(x, y).isWalkable)
                            hitedGameObject = RaycastHitInfo().collider.gameObject;
                        else
                            _clickCounter = 0;
                        
                    }
                    else
                        _clickCounter = 0;

                }
                if (_clickCounter == 2)
                {
                    pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
                    if (x >= 0 && y >= 0 && x <= pathfinding.GetGrid().GetWidth() - 1 && y <= pathfinding.GetGrid().GetHeight() - 1)
                    {
                        // Move object by path
                        if(hitedGameObject != null)
                        {
                            if (pathfinding.GetGrid().GetGridObject(x, y).isWalkable)
                            {
                                FindPathToMove(hitedGameObject.transform.position, mouseWorldPosition);
                            }
                        }
                    }
                    else
                        _clickCounter = 0;

                    _clickCounter = 0;
                }
            }
        }
    }

    public Vector3 GetMousePos(Vector3 position, Camera mainCamera)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(position);
        worldPoint.z = 0f;

        return worldPoint;
    }

    private RaycastHit RaycastHitInfo()
    {
        RaycastHit _hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out _hit))
        {
            //Debug.Log("hit  = " + _hit.transform.gameObject.name + " " + _hit.transform.position);
        } 
            return _hit;
    }

    void FindPathToMove(Vector3 startPosition, Vector3 endPosition)
    {
        pathfinding.GetGrid().GetXY(startPosition, out int a, out int b);
        startX = a;
        startY = b;

        pathVectorList = pathfinding.FindPath(hitedGameObject.transform.position, endPosition);

        _currentPathCount = 0;
    }

    private void MoveObjectByPath(GameObject ballObject)
    {
        if (pathVectorList != null)
        {
            _isMovingToPosition = true;

            Vector3 targetPosition = new Vector3(pathVectorList[_currentPathCount].x, pathVectorList[_currentPathCount].y, 0f);

            if (Vector3.Distance(ballObject.transform.position, targetPosition) > 0.025f)
            {
                Vector3 dir = (targetPosition - ballObject.transform.position).normalized;
                ballObject.transform.position += dir * ballMovementSpeed * Time.deltaTime;
            }
            else
            {
                _currentPathCount++;
                if (_currentPathCount >= pathVectorList.Count)
                {
                    pathVectorList = null;
                    hitedGameObject = null;
                    _isMovingToPosition = false;

                    pathfinding.GetGrid().GetXY(targetPosition, out int x, out int y);

                    if (_ballsArray[x, y] != null)
                    {
                        Destroy(_ballsArray[x, y].gameObject);
                        DeleteBallFromArray(x, y);
                        SetIsWalkable(x, y, true);
                    }

                    SetIsWalkable(startX, startY, true);
                    SetIsWalkable(x, y, false);

                    LeanTween.scale(ballObject, Vector3.one * pathfinding.GetGrid().GetCellSize(), 0.3f);

                    DeleteBallFromArray(startX, startY);
                    BoundBallToArray(ballObject, x, y);

                    if (!FindLines())
                    {
                        SizeUpBalls();
                        SpawnBalls();
                    }
                    else
                    {
                        //StartCoroutine(DeleteBalls(findBalls));
                    }
                }
            }
        }
    }

    private void DeleteBallFromArray(int x, int y)
    {
        _ballsArray[x, y] = null;
    }

    private void BoundBallToArray(GameObject ball, int x, int y)
    {
        _ballsArray[x, y] = ball;
    }

    private void SpawnBalls()
    {
        SpawnObjectsOnField();
    }

    private void SpawnObjectsOnField()
    {
        int x, y, a, i, j;
        List<int> xExes;
        List<int> yExes;
        xy = new int[amountBallsToSpawn, 2];

        for (int b = 0; b < amountBallsToSpawn; b++)
        {
            again:
            xExes = new List<int>();
            yExes = new List<int>();

            for (i = 0; i < pathfinding.GetGrid().GetWidth(); i++)
            {
                for (j = 0; j < pathfinding.GetGrid().GetHeight(); j++)
                {
                    if (_ballsArray[i, j] == null && pathfinding.GetGrid().GetGridObject(i, j).isWalkable)
                    {
                        xExes.Add(i);
                        yExes.Add(j);
                    }
                }
            }

            if (xExes.Count > 0 && yExes.Count > 0)
            {
                a = _rnd.Next(ballsToSpawn.Length);

                x = xExes.ElementAt(_rnd.Next(0, xExes.Count));
                y = yExes.ElementAt(_rnd.Next(0, yExes.Count));

                if (!pathfinding.GetGrid().GetGridObject(x, y).isWalkable || _ballsArray[x, y] != null)
                    goto again;

                pathfinding.GetGrid().GetWorldPositionByXY(out Vector3 pos, x, y);
                GameObject ball = Instantiate(ballsToSpawn[a], new Vector3(pos.x + pathfinding.GetGrid().GetCellSize() * 0.5f, pos.y + pathfinding.GetGrid().GetCellSize() * 0.5f, 0), Quaternion.identity);

                LeanTween.scale(ball, Vector3.one * 0.3f, 0);

                BoundBallToArray(ball, x, y);

                xy[b, 0] = x;
                xy[b, 1] = y;
            }
        }
    }

    private void SpawnFirstBalls()
    {
        int x, y, a, i, j;
        List<int> xExes;
        List<int> yExes;
        for (int b = 0; b < amountBallsToSpawn; b++)
        {
            if (amountBallsToSpawn <= 0)
                return;

            again:
            xExes = new List<int>();
            yExes = new List<int>();

            for (i = 0; i < pathfinding.GetGrid().GetWidth(); i++)
            {
                for (j = 0; j < pathfinding.GetGrid().GetHeight(); j++)
                {
                    if (_ballsArray[i, j] == null && pathfinding.GetGrid().GetGridObject(i, j).isWalkable)
                    {
                        xExes.Add(i);
                        yExes.Add(j);
                    }
                }
            }

            if (xExes.Count > 0 && yExes.Count > 0)
            {
                a = _rnd.Next(ballsToSpawn.Length);

                x = xExes.ElementAt(_rnd.Next(0, xExes.Count));
                y = yExes.ElementAt(_rnd.Next(0, yExes.Count));

                if (!pathfinding.GetGrid().GetGridObject(x, y).isWalkable || _ballsArray[x, y] != null)
                    goto again;

                SetIsWalkable(x, y, false);

                pathfinding.GetGrid().GetWorldPositionByXY(out Vector3 pos, x, y);
                GameObject ball = Instantiate(ballsToSpawn[a], new Vector3(pos.x + pathfinding.GetGrid().GetCellSize() * 0.5f, pos.y + pathfinding.GetGrid().GetCellSize() * 0.5f, 0), Quaternion.identity);

                _ballsArray[x, y] = ball;
                BoundBallToArray(ball, x, y);
            }
        }
    }

    private void SizeUpBalls()
    {
        float cellSize = pathfinding.GetGrid().GetCellSize();

        for (int i = 0; i < amountBallsToSpawn; i++)
        {
            if (_ballsArray[xy[i, 0], xy[i, 1]] != null)
            {
                if (pathfinding.GetGrid().GetGridObject(xy[i, 0], xy[i, 1]).isWalkable)
                {
                    LeanTween.scale(_ballsArray[xy[i, 0], xy[i, 1]], Vector3.one * cellSize, 0.1f);

                    SetIsWalkable(xy[i, 0], xy[i, 1], false);
                }
            }
            else
                continue;
        }
    }

    private int GetIsWalkableCount()
    {
        int count = 0;
        for (int x = 0; x < pathfinding.GetGrid().GetWidth(); x++)
        {
            for (int y = 0; y < pathfinding.GetGrid().GetHeight(); y++)
            {
                if(pathfinding.GetGrid().GetGridObject(x, y).isWalkable)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void SetIsWalkable(int x, int y, bool isWalkable)
    {
        pathfinding.GetGrid().GetGridObject(x, y).isWalkable = isWalkable;

        if (GetIsWalkableCount() <= 2)
            GameOver = true;
    }

    bool FindLines()
    {
        _ballPositionXY = new List<Vector2>();
        List<GameObject> findBalls = new List<GameObject>();
        bool getBall = true;
        int z = 0;
        int matchCount = 0;

        // Horizontal

        while (z < 1)
        {
            for (int y = 0; y < _ballsArray.GetLength(1); y++)
            {
                for (int x = 0; x < _ballsArray.GetLength(0); x++)
                {
                    z++;
                    if (x < _ballsArray.GetLength(0) - 1)
                    {
                        if (_ballsArray[x, y] && _ballsArray[x + 1, y] && _ballsArray[x, y].GetComponent<Ball>().color == _ballsArray[x + 1, y].GetComponent<Ball>().color && getBall)
                        {
                            if (matchCount == 0)
                            {
                                matchCount = 2;
                                findBalls.Add(_ballsArray[x, y]);
                                _ballPositionXY.Add(new Vector2(x, y));
                                findBalls.Add(_ballsArray[x + 1, y]);
                                _ballPositionXY.Add(new Vector2(x + 1, y));
                            }
                            else
                            {
                                matchCount++;
                                findBalls.Add(_ballsArray[x + 1, y]);
                                _ballPositionXY.Add(new Vector2(x + 1, y));
                            }
                        }
                        else if (matchCount < 5)
                        {
                            matchCount = 0;
                            findBalls = new List<GameObject>();
                            _ballPositionXY = new List<Vector2>();
                        }
                        else
                        {
                            getBall = false;
                        }
                    }

                }
            }
        }

        // Vertical

        if (matchCount == 0)
        {
            _ballPositionXY = new List<Vector2>();
            findBalls = new List<GameObject>();
            getBall = true;
            z = 0;
            while (z < 1)
            {
                for (int y = 0; y < _ballsArray.GetLength(1); y++)
                {
                    for (int x = 0; x < _ballsArray.GetLength(0); x++)
                    {
                        z++;
                        if (x < _ballsArray.GetLength(0) - 1)
                        {
                            if (_ballsArray[y, x] && _ballsArray[y, x + 1] && _ballsArray[y, x].GetComponent<Ball>().color == _ballsArray[y, x + 1].GetComponent<Ball>().color && getBall)
                            {
                                if (matchCount == 0)
                                {
                                    matchCount = 2;
                                    findBalls.Add(_ballsArray[y, x]);
                                    _ballPositionXY.Add(new Vector2(y, x));
                                    findBalls.Add(_ballsArray[y, x + 1]);
                                    _ballPositionXY.Add(new Vector2(y, x + 1));
                                }
                                else
                                {
                                    matchCount++;
                                    _ballPositionXY.Add(new Vector2(y, x + 1));
                                    findBalls.Add(_ballsArray[y, x + 1]);
                                }
                            }
                            else if (matchCount < 5)
                            {
                                matchCount = 0;
                                findBalls = new List<GameObject>();
                                _ballPositionXY = new List<Vector2>();
                            }
                            else
                            {
                                getBall = false;
                            }
                        }
                    }
                }
            }
        }
        
        // Diagonal from (0, 0) to (GetWidth(), GetHeight())
        
        if (matchCount == 0)
        {
            _ballPositionXY = new List<Vector2>();
            findBalls = new List<GameObject>();
            getBall = true;
            z = 0;
            int x, y, Ox = _ballsArray.GetLength(0) / 2 + 1;
            while (z < 1)
            {
                for (y = 0; y < _ballsArray.GetLength(1) / 2 + 1; y++)
                {
                    if (y > 0)
                        Ox = 1;
                    for (x = 0; x < Ox; x++)
                    {
                        z++;

                        if (x < _ballsArray.GetLength(0) - 1)
                        for (int i = 0; i < _ballsArray.GetLength(0) - x; i++)
                        {
                            if (y + i < _ballsArray.GetLength(1) - 1 && x + i < _ballsArray.GetLength(1) - 1)
                            {
                                if (_ballsArray[x + i, y + i] && _ballsArray[x + i + 1, y + i + 1] && _ballsArray[x + i, y + i].GetComponent<Ball>().color == _ballsArray[x + i + 1, y + i + 1].GetComponent<Ball>().color && getBall)
                                {
                                    if (matchCount == 0)
                                    {
                                        matchCount = 2;
                                        findBalls.Add(_ballsArray[x + i, y + i]);
                                        _ballPositionXY.Add(new Vector2(x + i, y + i));

                                        findBalls.Add(_ballsArray[x + i + 1, y + i + 1]);
                                        _ballPositionXY.Add(new Vector2(x + i + 1, y + i + 1));
                                    }
                                    else
                                    {
                                        matchCount++;
                                        findBalls.Add(_ballsArray[x + i + 1, y + i + 1]);
                                        _ballPositionXY.Add(new Vector2(x + i + 1, y + i + 1));
                                    }
                                }
                                else if (matchCount < 5)
                                {
                                    matchCount = 0;
                                    findBalls = new List<GameObject>();
                                    _ballPositionXY = new List<Vector2>();
                                }
                                else
                                {
                                    getBall = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        // Diagonal from (0, GetHeight()) to (GetWidth(), 0)

        if (matchCount == 0)
        {
            _ballPositionXY = new List<Vector2>();
            findBalls = new List<GameObject>();
            getBall = true;
            z = 0;
            int x, y, Ox = _ballsArray.GetLength(0) / 2 + 1;
            while (z < 1)
            {
                for (y = _ballsArray.GetLength(1) - 1; y > _ballsArray.GetLength(1) / 2 - 1; y--)
                {
                    if (y < 8)
                        Ox = 1;
                    for (x = 0; x < Ox; x++)
                    {
                        z++;

                        if (x < _ballsArray.GetLength(0) - 1)
                            for (int i = 0; i < _ballsArray.GetLength(0) - x; i++)
                            {
                                if (y - i > 0 && x + i < _ballsArray.GetLength(1) - 1)
                                {
                                    if (_ballsArray[x + i, y - i] && _ballsArray[x + i + 1, y - i - 1] && _ballsArray[x + i, y - i].GetComponent<Ball>().color == _ballsArray[x + i + 1, y - i - 1].GetComponent<Ball>().color && getBall)
                                    {
                                        if (matchCount == 0)
                                        {
                                            matchCount = 2;
                                            findBalls.Add(_ballsArray[x + i, y - i]);
                                            _ballPositionXY.Add(new Vector2(x + i, y - i));

                                            findBalls.Add(_ballsArray[x + i + 1, y - i - 1]);
                                            _ballPositionXY.Add(new Vector2(x + i + 1, y - i - 1));
                                        }
                                        else
                                        {
                                            matchCount++;
                                            findBalls.Add(_ballsArray[x + i + 1, y - i - 1]);
                                            _ballPositionXY.Add(new Vector2(x + i + 1, y - i - 1));
                                        }
                                    }
                                    else if (matchCount < 5)
                                    {
                                        matchCount = 0;
                                        findBalls = new List<GameObject>();
                                        _ballPositionXY = new List<Vector2>();
                                    }
                                    else
                                    {
                                        getBall = false;
                                    }
                                }
                            }
                    }
                }
            }
        }

        if (matchCount < 5)
        {
            //SpawnBalls();
            return false;
        }
        else
        {
            GameScore += matchCount;
            StartCoroutine(DeleteBalls(findBalls));
            return true;
        }
    }

    private IEnumerator DeleteBalls(List<GameObject> balls)
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < balls.Count; i++)
        {
            pathfinding.GetGrid().GetXY(balls[i].transform.position, out int x, out int y);
            SetIsWalkable(x, y, true);
            Destroy(balls[i]);
        }
    }

    private void ClearField()
    {
        for (int i = 0; i < pathfinding.GetGrid().GetWidth(); i++)
        {
            for (int j = 0; j < pathfinding.GetGrid().GetHeight(); j++)
            {
                if (_ballsArray[i, j] != null)
                {
                    Destroy(_ballsArray[i, j].gameObject);
                    _ballsArray[i, j] = null;
                }
                pathfinding.GetGrid().GetGridObject(i, j).isWalkable = true;
            }
        }

        _clickCounter = 0;

        GameScore = 0;
        GameOver = false;

        SpawnFirstBalls();
        SpawnObjectsOnField();
    }

    public void StartGame()
    {
        StartCoroutine(FadeAndStart());
    }

    public void LoseGame()
    {
        StartCoroutine(FadeAndEnd());
    }

    public void Replay()
    {
        StartCoroutine(FadeAndReplay());
    }

    IEnumerator FadeAndStart()
    {
        fadePanel.SetActive(true);
        fadePanelAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        startGamePanel.SetActive(false);
        scorePanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        fadePanel.SetActive(false);
    }

    IEnumerator FadeAndEnd()
    {
        fadePanel.SetActive(true);
        fadePanelAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        GameOverPanel.SetActive(true);
        scorePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        fadePanel.SetActive(false);
    }

    IEnumerator FadeAndReplay()
    {
        fadePanel.SetActive(true);
        fadePanelAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        GameOverPanel.SetActive(false);
        scorePanel.SetActive(true);

        ClearField();

        yield return new WaitForSeconds(1f);
        fadePanel.SetActive(false);
    }

}
