using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPanels : MonoBehaviour
{
    /*Displays Panel by reseting position, turning object visible
    making object interactable 
    */
     public void ShowPanel(GameObject yourObject){
        var getCanvasGroup  = yourObject.GetComponent<CanvasGroup>();
        getCanvasGroup.alpha = 1;
        getCanvasGroup.interactable = true;
        var rect = yourObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, 0);

         
             
     }
    /*Displays Panel by shifting position, turning object invisible
    making object not interactable 
    */
     public void HidePanel(GameObject yourObject) {
        var getCanvasGroup  = yourObject.GetComponent<CanvasGroup>();
        getCanvasGroup.alpha = 0;
        getCanvasGroup.interactable = false;

        var rect = yourObject.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(4000, 0);

     }
     //Makes object uninteractable
     public void DeactivatePanel(GameObject yourObject){
        var getCanvasGroup  = yourObject.GetComponent<CanvasGroup>();

        getCanvasGroup.interactable = false;
     }
     //Makes object ineractable
     public void ActivatePanel(GameObject yourObject){
        var getCanvasGroup  = yourObject.GetComponent<CanvasGroup>();

        getCanvasGroup.interactable = true;
     }
}

