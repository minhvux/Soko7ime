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
    public List<GameObject> boxObjects;

    // Switches are maintained separately.
    public List<SwitchController> switchControllers;

    // Dictionary for tracking position history
    private Dictionary<GameObject, Stack<Vector2>> eventPositionHistory = new Dictionary<GameObject, Stack<Vector2>>();

    public LayerMask blockLayer;
    public LayerMask wallLayer;
    public LayerMask goalLayer;
    public LayerMask lavaLayer;

    public bool isMoving = false;

    
    //private bool rewinded = false;

    public bool isAlive = true;


    public bool canRewind = true;
    public int rewindSteps = 7;
    private int rewindIndex = 0;

    public bool futureMode = false;
    public bool canFuture = true;
    private int futureIndex = 0;
    

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
            isMoving = true;
            player.GetComponent<PlayerController>().TryToMove(moveDirection);
            isMoving = false;
        }
        
        if (futurePastPlayer != null && futurePastPlayer.activeSelf)
        {   
            isMoving = true;
            futurePastPlayer.GetComponent<FuturePastPlayerController>().TryToMove(moveDirection);
            isMoving = false;
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
        UpdateAll();
    }

    public void DataHubRewind()
    {   
        if (GetHistoricalPosition(player, rewindSteps) == Vector2.zero)
        {
            Debug.Log("No history available for rewind.");
            return;
        }

        player.transform.position = pastIndicator.transform.position;
        canRewind = false;
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
            Debug.LogWarning("Provided object is null.");
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
        if(futurePastPlayer.activeSelf) CheckLava(futurePastPlayer.transform.position);
        CheckGoal(player.transform.position);
        CheckSwitches();
        Debug.Log(canFuture);
        if(canFuture){
            
            RewindIndexUpdate();
            PastIndicatorUpdate();
        }
    }

    private void CheckLava(Vector2 position)
    {
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
        
        if (rewindIndex > 0)
        {
            canRewind = false;
            //pastIndicator.SetActive(false);
            //Debug.Log("Rewind index is 0, cannot rewind anymore.");
        }
        else if (rewindIndex == 0)
        {
            canRewind = true;
            //rewinded = false;
            //pastIndicator.SetActive(true);
        } 
        
        

        Debug.Log("Rewind index: " + rewindIndex);
    }

    private void IncrementRewindIndex()
    {
        if (!canRewind) 
        {
            rewindIndex++;
            Debug.Log("dcm");
        }
    }

    private void DecrementRewindIndex()
    {
        if (rewindIndex > 0)
        {
            rewindIndex--;
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
        canFuture = false;
        AfterMovingUpdate();
        IncrementRewindIndex();
    
        
    }
}
