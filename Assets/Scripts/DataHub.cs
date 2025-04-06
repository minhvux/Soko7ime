using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHub : MonoBehaviour
{
    public static DataHub Instance { get; private set; }

    // Separate event objects
    public GameObject player;
    public GameObject futurePastPlayer;
    public GameObject pastIndicator;
    public GameObject goalObject;
    public GameObject futureIndicator;
    public List<GameObject> boxObjects;

    // Switches are maintained separately.
    public List<SwitchController> switchControllers;

    // Dictionary for tracking position history
    private Dictionary<GameObject, Stack<Vector2>> eventPositionHistory = new Dictionary<GameObject, Stack<Vector2>>();

    public LayerMask blockLayer;
    public LayerMask wallLayer;
    public LayerMask goalLayer;
    public LayerMask lavaLayer;
    public LayerMask playerLayer;


    
    //private bool rewinded = false;

    public bool isAlive = true;
    public bool isMoving = false;
    private int pendingMoves = 0;


    public bool canRewind = true;
    public int rewindSteps = 7;
    private int rewindIndex = 0;

    public bool futureMode = false;
    public bool canFuture = true;
    public int futureIndex = 0;
    

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Combine objects that need history tracking.
        List<GameObject> eventObjects = new List<GameObject>();
        if (player != null) eventObjects.Add(player);
        if (futurePastPlayer != null) eventObjects.Add(futurePastPlayer);
        if (pastIndicator != null) eventObjects.Add(pastIndicator);
        if (goalObject != null) eventObjects.Add(goalObject);
        if (boxObjects != null)
        {
            foreach (var box in boxObjects)
            {
                if (box != null) eventObjects.Add(box);
            }
        }
        

        // Initialize the history stacks
        foreach (var obj in eventObjects)
        {
            eventPositionHistory[obj] = new Stack<Vector2>();
            // Push the initial position
            eventPositionHistory[obj].Push(new Vector2(obj.transform.position.x, obj.transform.position.y));
        }
    }

    public void Move(Vector2 moveDirection)
    {   
        
        if (player != null && player.activeSelf)
        {   
            player.GetComponent<PlayerController>().TryToMove(moveDirection);
        }
        
        if (futurePastPlayer != null && futurePastPlayer.activeSelf)
        {   
            futurePastPlayer.GetComponent<FuturePastPlayerController>().TryToMove(moveDirection);
        }
        
        
        
    }

    public void ReportMoveStarted()
    {
        pendingMoves++;
        isMoving = true;
    }

    public void ReportMoveComplete()
    {
        pendingMoves--;
        // When all moves are complete, update history once.
        if (pendingMoves <= 0)
        {   
            isMoving = false;
            if (!futureMode)
            {
                AfterMovingUpdate();
            }
        }
        
    }


    public void AfterMovingUpdate()
    {
        // Update the position history for all tracked objects.
        foreach (var kv in eventPositionHistory)
        {
            GameObject obj = kv.Key;
            if (obj != null)
            {
                Vector2 currentPosition = new Vector2(obj.transform.position.x, obj.transform.position.y);
                eventPositionHistory[obj].Push(currentPosition); 
            }
        }
        
        IncrementRewindIndex();
        //if (!canFuture) futureIndex++;
        UpdateAll();
    }

    public void DataHubRewind()
    {   
        if (GetHistoricalPosition(player, rewindSteps) == Vector2.zero && canFuture)
        {
            Debug.Log("No history available for rewind.");
            return;
        }

       
        player.transform.position = pastIndicator.transform.position;
        
        canRewind = false;

        if (!canFuture){
            futurePastPlayer.SetActive(false);
        }
        //rewinded = true;
        AfterMovingUpdate();
    
    }

    public void RevertAllToPreviousPositions()
    {
        foreach (var obj in eventPositionHistory.Keys)
        {
            RevertToPreviousPosition(obj);
        }
        //Debug.Log("Reverted all event objects to their previous positions.");
        DecrementRewindIndex();
        futureIndicator.GetComponent<FutureIndicator>().revertFutureIndicator();
        UpdateAll();
        
        
       
    }

    public void RevertToPreviousPosition(GameObject obj)
    {
        if (eventPositionHistory.ContainsKey(obj) && eventPositionHistory[obj].Count > 1)
        {
            eventPositionHistory[obj].Pop();
            Vector2 previousPosition = eventPositionHistory[obj].Peek();
            obj.transform.position = new Vector3(previousPosition.x, previousPosition.y, obj.transform.position.z);
        }
    }

    private Vector2 GetHistoricalPosition(GameObject obj, int stepsAgo)
    {
        if (obj == null)
        {
            //Debug.LogWarning("Provided object is null.");
            return Vector2.zero;
        }
        
        if (!eventPositionHistory.ContainsKey(obj))
        {
            //Debug.LogWarning("No history for object: " + obj.name);
            return new Vector2(obj.transform.position.x, obj.transform.position.y);
        }
        
        var historyStack = eventPositionHistory[obj];
        if (historyStack.Count < stepsAgo)
        {   
            //Debug.Log(historyStack.Count + " history for " + obj.name);
            //Debug.LogWarning("Not enough history for " + obj.name);
            return Vector2.zero;
        }
        
        // The most recent position is at index 0.
        Vector2[] historyArray = historyStack.ToArray();
        return historyArray[stepsAgo - 1];
    }

    private void UpdateAll(){
        CheckLava(player.transform.position);
        if(futurePastPlayer.activeSelf && isAlive) CheckLava(futurePastPlayer.transform.position);
        CheckGoal(player.transform.position);
        CheckParadox(player.transform.position);
        if(futurePastPlayer.activeSelf && isAlive) CheckParadox(futurePastPlayer.transform.position);
        if(!canFuture) futureIndicator.GetComponent<FutureIndicator>().CheckActive();
        CheckSwitches();        
        RewindIndexUpdate();

        if(!futureMode) PastIndicatorUpdate();
        //Debug.Log("Future index: " + futureIndex);
        
    }

    private void CheckLava(Vector2 position)
    {   
        Physics2D.SyncTransforms();
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, lavaLayer);
        if (hit != null)
        {
            Debug.Log("Player is in lava at " + position);
            isAlive = false;
        }
        else 
        {
            isAlive = true;
        }
    }

    private void CheckGoal(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, goalLayer);
        if (hit != null)
        {
            Debug.Log("win");
            GameManager.Instance.LevelComplete();
        }
    }

    private void CheckParadox(Vector2 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 0.1f, playerLayer | blockLayer | wallLayer);
        if (hits.Length > 1)
        {
            Debug.Log("Paradox detected at " + position);
            GameManager.Instance.Paradox();
        }
        else 
        {
            GameManager.Instance.NoParadox();
        }
    }

    

    private void CheckSwitches()
    {
        foreach (var switchController in switchControllers)
        {
            switchController.CheckActive();
        }
    }

    private void PastIndicatorUpdate()
    {   
        if (pastIndicator != null)
        {
            if (canRewind)
            {   
                //Debug.Log("Can rewind: " + canRewind);
                if (!canFuture && futureIndicator.GetComponent<FutureIndicator>().isActive)
                {   
                    pastIndicator.SetActive(true);
                    pastIndicator.transform.position = futurePastPlayer.transform.position;
                    return;
                }
                Vector2 pastPosition = GetHistoricalPosition(player, rewindSteps);
                if (pastPosition == Vector2.zero)
                {
                    pastIndicator.SetActive(false);
                    return;
                }
                pastIndicator.SetActive(true);
                pastIndicator.transform.position = new Vector3(pastPosition.x, pastPosition.y, pastIndicator.transform.position.z);
            } 
            else 
            {
                pastIndicator.SetActive(false);
            }
        }
    }

    private void RewindIndexUpdate()
    {   
        //Debug.Log(canRewind);
        //Debug.Log(rewindIndex);
        if (rewindIndex > 0)
        {
            canRewind = false;
            
        }
        else if (rewindIndex == 0)
        {   
            if (!canFuture)
            {   

                if (futureIndicator.GetComponent<FutureIndicator>().isActive) futurePastPlayer.SetActive(true);
                
            }
            canRewind = true;
            
            //rewinded = false;
            //pastIndicator.SetActive(true);
        } 

        if (futureIndex > 0)
        {
            canFuture = false;
            
        }
        else if (futureIndex == 0)
        {   
            if (!canFuture)
            {
                futurePastPlayer.SetActive(false);
            }
            canFuture = true;
            futureIndicator.SetActive(false);
            
        }
        
        

        
    }

    private void IncrementRewindIndex()
    {
        if (!canRewind) 
        {
            rewindIndex++;
            
        }

        if (!canFuture)
        {
            futureIndex++;
        }
    }

    private void DecrementRewindIndex()
    {
        if (rewindIndex > 0)
        {
            rewindIndex--;
        }

        if (futureIndex > 0)
        {
            futureIndex--;
        }
    }

    public void DataHubToFuture()
    {
        if (futurePastPlayer != null)
        {
            futurePastPlayer.transform.position = player.transform.position;
            futurePastPlayer.SetActive(true);
            futureMode = true;
        }
        if (pastIndicator != null)
        {
            pastIndicator.SetActive(false);
        }
    }
    public void SettleFuture()
    {   
        futureMode = false;
        //Debug.Log(canFuture);
        canFuture = false;
        futureIndicator.transform.position = player.transform.position;
        futureIndicator.SetActive(true);
        AfterMovingUpdate();  

    }

    public void DeactivateFuturePastPlayer()
    {
        futurePastPlayer.SetActive(false);
        
    }
}
