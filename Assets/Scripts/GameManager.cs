using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private GameObject[] gameingredients;
    public static GameManager instance = null;

    public Text ordertext;
    public Text requesttext;
    public Button playbutton;

	public Canvas gameCanvas;
	public Canvas gameOverCanvas;
	public Canvas BurnCanvas;
	public Canvas JokerCanvas;

    private List<Ingredient> ingredients;
    private List<Ingredient> requestedOrder;

	public List<Customer> customers;
	private List<Customer> roundCustomers = new List<Customer>();

	//   private Customer[] customers;
	//private Customer[] origCustomers;

	public Ingredient bread;
	public Ingredient meat;
	public Ingredient fish;
	public Ingredient cheese;
	public Ingredient lettuce;

	public int rounds = 5;
	private int currentRound = 0;

    // Use this for initialization
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

		gameCanvas.enabled = false;
        //playbutton  = GameObject.Find("NotInGame/PlayButton").GetComponent<Button>();
        //requesttext = GameObject.Find("InGame/Request").GetComponent<Text>();
        //ordertext   = GameObject.Find("InGame/Order").GetComponent<Text>();


        gameingredients = GameObject.FindGameObjectsWithTag("Item");

		foreach (GameObject go in gameingredients)
		{
			go.SetActive(false);
		}
		playbutton.gameObject.SetActive(true);
		//createNewGame();
	}

	private void gameOver()
    {
		foreach (GameObject go in gameingredients)
        {
            go.SetActive(false);
        }

		requesttext.text = "";
		ordertext.text = "You ruined my rutial!\n\nGAME OVER! Press button to try again.\nBtw, you just died a horrible slow death by the angry villagers";
		ordertext.alignment = TextAnchor.UpperCenter;
		//playbutton.GetComponent<CanvasGroup>().alpha = 1;
		//playbutton.interactable = true;
		playbutton.gameObject.SetActive(true);
    }

    public void createNewGame()
    {
		gameCanvas.enabled = true;
		Debug.Log("Creating a game");
		foreach (GameObject go in gameingredients)
		{
			go.SetActive(true);
		}
		playbutton.gameObject.SetActive(false);

		resetCustomers();

        play();
    }

	public void playRound()
	{
		Debug.Log("Playing a new round, so adding all the peoples again");
		roundCustomers = new List<Customer>(customers.ToArray()); // origCustomers;
	}


	void resetCustomers()
    {
		foreach (Customer c in customers)
		{
			c.reset();
		}
    }
    
    public void play()
    {
		//foreach (GameObject go in gameingredients)
		//{
		//	go.SetActive(true);
		//}
		Debug.Log("Hiding play button and plays the game");
		//playbutton.gameObject.SetActive(false);

		if (roundCustomers.Count <= 0)
		{	
			playRound();
		}

		ordertext.text = "";
		ordertext.alignment = TextAnchor.UpperLeft;
		requesttext.text = "";

		int customerIndex = Random.Range(0, roundCustomers.Count);
        Customer customer = roundCustomers[customerIndex];
		roundCustomers.RemoveAt(customerIndex);
		Debug.Log("Now there are " + roundCustomers.Count + " peoples left");
        newCustomer(customer);
    }

    public void newCustomer(Customer customer)
    {
		Debug.Log(customer.getName());
        ingredients = new List<Ingredient>();
        requestedOrder = customer.order;

        requesttext.text = customer.createOrderText();

    }

    public void addIngredient(Ingredient item)
    {
        int total = ingredients.Count;
        if (item.code.Equals(requestedOrder[total].code))
        {
            ingredients.Add(item);

			ordertext.text += " - " + item.title + "\n";

			if (ingredients.Count == requestedOrder.Count)
			{
				Debug.Log("HURRAY! You completed the order for the first player");
				play();
			}
        }
        else
        {
			gameOver();
        }
    }
}

public static class IListExtensions
{
    public static void Shuffle<T>(this List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

