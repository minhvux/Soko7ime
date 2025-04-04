using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FutureIndicator : MonoBehaviour
{   
    public int paradox = 14;
    private int paradoxIndex = 0;
    public bool isActive = true;
    private int stepBeforeParadox = 0;
    // Manually assign this field in the Inspector.
    public TMP_Text paradoxText;
    

    // Start is called before the first frame update
    

    public void CheckActive()
    {   
        
        if (DataHub.Instance.canFuture)
        {
            gameObject.SetActive(false);
            return;
        }


        stepBeforeParadox = paradox - DataHub.Instance.futureIndex + 1;

        if (isActive) 
        {
            paradoxIndex = stepBeforeParadox;
        } else
        {   
            CheckParadox();
            
            return;
        }

        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Player"));
        if (hitCollider != null)
        {
            if (!DataHub.Instance.futurePastPlayer.gameObject.activeSelf)
            {   
                DeativeFutureIndicator();
                return;
            } 
            else if(hitCollider.CompareTag("FuturePast"))
            {   
                DeativeFutureIndicator();
                return;
            }
            
        }
        
        CheckParadox();
      
    }

    private void DeativeFutureIndicator()
    {
        isActive = false;
        paradoxIndex = stepBeforeParadox;
        gameObject.SetActive(false);
        DataHub.Instance.DeactivateFuturePastPlayer();
        //Debug.Log("wtf");
    }

    private void CheckParadox()
    {   
        
        // Update the text if the component is assigned.
        if (paradoxText != null)
        {
            paradoxText.text = paradoxIndex.ToString();
        }

        if (paradoxIndex == 0)
        {
            Debug.Log("Paradox!");
            
            GameManager.Instance.Paradox();
        }
        
    }

    public void revertFutureIndicator()
    {   
        if (DataHub.Instance.canFuture) return;
        if (paradoxIndex <= stepBeforeParadox)
        {   
            paradoxIndex = stepBeforeParadox;
            isActive = true;
            gameObject.SetActive(true);
        } 
        CheckParadox();
        CheckActive();
        
    }
}
