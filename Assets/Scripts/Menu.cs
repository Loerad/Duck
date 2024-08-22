using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using System.Linq;

public class Menu : MonoBehaviour
{

    private Button playButton;
    private Button highscoreButton;
    private Button tutorialButton;
    private Button quitButton;
    private Label versionNumber;

    private Dictionary<string, VisualElement> sceneRootElements = new Dictionary<string, VisualElement>();

    void Awake()
    {
        VisualElement document = GetComponent<UIDocument>().rootVisualElement;
        playButton = document.Q<Button>("Play");
        playButton.RegisterCallback<ClickEvent>(Play);

        highscoreButton = document.Q<Button>("Highscores");
        tutorialButton = document.Q<Button>("Tutorial");

        quitButton = document.Q<Button>("Quit");
        quitButton.RegisterCallback<ClickEvent>(Quit);

        versionNumber = document.Q<Label>("VersionNumber");
        versionNumber.text = "Alpha V0.9.0";

    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        SFXManager.Instance.PlayBackgroundMusic(SFXManager.Instance.TitleScreen);
        StartCoroutine(LoadBackgroundScene("Highscores", highscoreButton));
        StartCoroutine(LoadBackgroundScene("Tutorial", tutorialButton));
    }

    IEnumerator LoadBackgroundScene(string sceneName, Button button)
    {
        Color originalColour = button.resolvedStyle.backgroundColor; //Record original colour
        button.style.backgroundColor = new StyleColor(new Color(0.5f, 0.5f, 0.5f)); //Turn gray while loading
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        //When the scene is loaded add the click event
        button.RegisterCallback<ClickEvent>(SubMenuButton);
        button.style.backgroundColor = new StyleColor(originalColour); //Return to original colour

        //Following code was based off of a request to copilot on how to access the UI doc of the scene loaded
        //Code was non functional to start and heavily modified but suggestion of using linq queries is the same
        Scene loadedScene = SceneManager.GetSceneByName(sceneName); //Getting the scene I just loaded
        if (loadedScene.IsValid())
        {
            GameObject[] rootObjects = loadedScene.GetRootGameObjects(); // Gets an array of all the objects in the scene that aren't inside other objects
            UIDocument uiDocument = rootObjects
                .Select(obj => obj.GetComponent<UIDocument>())
                .FirstOrDefault(doc => doc != null); // Checking each object to see if it has a UIDocument component, and if it does, it returns it
            
            if (uiDocument != null)
            {
                VisualElement rootElement = uiDocument.rootVisualElement; // Getting the root visual element of the UI document
                rootElement.style.display = DisplayStyle.None;
                sceneRootElements[sceneName] = rootElement;
                //Store the root element in the dictionary. I need to do this because I can't pass through references or do returns in coroutines
                //and there is no other way I can think of to have this method not hard code the variables or scene names
                //This way it dynamically creates entries in the dictionary based on whatever name it was passed
            }
        }

    }

    public void Play(ClickEvent click)
    {
        GameSettings.gameState = GameState.InGame;
        //REMINDER: Change this back to main scene before merging
        StartCoroutine(LoadScene("Palin-MainScene"));
    }

    IEnumerator LoadScene(string sceneName)
    {
        //TODO: Add loading screen if the load times start to get longer
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        playButton.text = "Loading";
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void Quit(ClickEvent click)
    {
        Application.Quit();
    }

    //Two button click methods turned into one. The button that was clicked is turned into a visual element and the name is used to find the correct dictionary entry
    public void SubMenuButton(ClickEvent click)
    {
        VisualElement targetElement = click.target as VisualElement;
        if (targetElement != null)
        {
            sceneRootElements.TryGetValue(targetElement.name, out VisualElement rootElement);
            rootElement.style.display = DisplayStyle.Flex; // Set to visible
        }
    }
}


