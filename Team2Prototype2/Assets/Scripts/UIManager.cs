/*****************************************************************************
// File Name :         UIManager.cs
// Author :            Cade Naylor
// Creation Date :     February 26, 2024
//
// Brief Description : Handles buttons + UI Interactability on all screens

*****************************************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{

    [SerializeField] private float overwriteFlashTime = .5f;

    [SerializeField]
    private Canvases[] canvases;

    [SerializeField]private Image[] states;
    [Tooltip("Unselected Empty, Unselected Full, Selected Empty, Selected Full")]
    [SerializeField]
    private Sprite[] stateSprites;

    private string lastCanvas;


    private void Start()
    {
        if(SceneManager.GetActiveScene().name.Contains("Puzzle"))
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Sets the active canvas based on a provided name
    /// </summary>
    /// <param name="newCanvas">The canvas to set active</param>
    private void SwitchCanvas(string newCanvas)
    {
        //Finds the associated canvas and sets all canvases to inactive
        Canvases canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == newCanvas);
        foreach (Canvases c in canvases)
        {
            if(c.Canvas.activeInHierarchy)
            {
                lastCanvas = c.canvasName;
            }
            c.Canvas.SetActive(false);
        }

        //Sets the current canvas and its first selected
        if (canvas != null)
        {
            canvas.Canvas.SetActive(true);
            if(canvas.firstSelected!=null)
            {
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(canvas.firstSelected);
            }    

        }
    }

    /// <summary>
    /// Opens the credit canvas
    /// </summary>
    public void OpenCredits()
    {
        SwitchCanvas("Credits");
    }

    /// <summary>
    /// Opens the quit canvas
    /// </summary>
    public void QuitMenu()
    {
        SwitchCanvas("Quit");
    }

    /// <summary>
    /// Closes any non-menu canvases
    /// </summary>
    public void Back()
    {
        SwitchCanvas(lastCanvas);
    }

    /// <summary>
    /// Opens the how to play canvas
    /// </summary>
    public void HowToPlay()
    {
        SwitchCanvas("HowToPlay");
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Loads the game scene
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("PuzzleRoom1");
    }

    public void SwitchHighlightButton(int saveState)
    {

    }

    /// <summary>
    /// Changes the sprite of a save state to indicate it has been saved to
    /// </summary>
    /// <param name="saveState">the slot saved to</param>
    public void FillSaveState(int saveState)
    {
        Canvases canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == "SaveStateUI");
        if (canvas.Canvas.activeInHierarchy)
        {
            states[saveState].sprite = stateSprites[((saveState + 1) * 4) - 1];
        }
    }

    /// <summary>
    /// Changes the sprite of a save state to indicate it has been loaded from
    /// </summary>
    /// <param name="saveState">the slot loaded from</param>
    public void EmptySaveState(int saveState)
    {
        Canvases canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == "SaveStateUI");
        if (canvas.Canvas.activeInHierarchy)
        {
            states[saveState].sprite = stateSprites[((saveState + 1) * 4) - 2];
        }
    }


    /// <summary>
    /// Changes the sprite of a save state with a brief flash to indicate it has been overwriten to
    /// </summary>
    /// <param name="saveState">the slot overwritten to</param>
    /// <returns>the time flashed for</returns>
    //This function may be buggy
    public IEnumerator OverwriteSaveState(int saveState)
    {
        Canvases canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == "SaveStateUI");
        if (canvas.Canvas.activeInHierarchy)
        {
            states[saveState].sprite = stateSprites[(saveState + 1) * 4 - 2];
        }
        yield return new WaitForSeconds(overwriteFlashTime);
        if (canvas.Canvas.activeInHierarchy)
        {
            SaveStateBehvaior ssb = FindObjectOfType<SaveStateBehvaior>();
            if(ssb.CurrentStateSelected == saveState)
            {
                states[saveState].sprite = stateSprites[((saveState + 1) * 4 - 1)];
            }
            else
            {
                states[saveState].sprite = stateSprites[(saveState + 1) * 4 - 3];
            }
        }
    }

    public void SelectSaveState(int saveState)
    {
        Canvases canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == "SaveStateUI");
        if (canvas.Canvas.activeInHierarchy)
        {
            //If the current save state is full, change it to the full selected state
            if (states[saveState].sprite == stateSprites[(saveState + 1) * 4 - 3])
            {
                states[saveState].sprite = stateSprites[(saveState + 1) * 4 - 1];
            }
            //if the current save state is empty, change it to the empty selected state
            else
            {
                states[saveState].sprite = stateSprites[(saveState + 1) * 4 -2];
            }
        }
    }

    public void DeselectSaveState(int saveState)
    {
        Canvases canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == "SaveStateUI");
        if (canvas.Canvas.activeInHierarchy)
        {
            //If the previously selected save state was full, change it to the full deselected state
            if (states[saveState].sprite == stateSprites[((saveState+1) * 4 - 1)])
            {
                states[saveState].sprite = stateSprites[((saveState + 1) * 4 -3)];
            }
            //if the previously selected save state was empty, change it to the empty deselected state
            else
            {
                states[saveState].sprite = stateSprites[((saveState) * 4)];
            }
        }
    }
}
