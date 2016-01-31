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
	public Canvas burnCanvas;
	public Canvas jokerCanvas;
	public Canvas introCanvas;
	public Canvas uiCanvas;
    private List<Ingredient> ingredients;
    private List<Ingredient> requestedOrder;

	public AudioSource introSound;
	public AudioSource gameSound;
	public AudioSource fireSound;
	public AudioSource fireCrySound;
	public AudioSource fireAndCrySound;

	private Countdown countdown;

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
	private int origRounds;
	private int currentRound = 0;

    // Use this for initialization
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

		origRounds = rounds;
		uiCanvas.enabled = false;
		gameCanvas.enabled = false;
		introCanvas.enabled = true;
		burnCanvas.enabled = false;
		jokerCanvas.enabled = false;

		countdown = gameCanvas.GetComponent<Countdown>();

		gameingredients = GameObject.FindGameObjectsWithTag("Item");

		foreach (GameObject go in gameingredients)
		{
			go.SetActive(false);
		}
		playbutton.gameObject.SetActive(true);
		//createNewGame();

		gameoverSound(false);
		introplaySound(true);
		gameplaySound(false);
	}

	public void hideIntro()
	{
		gameoverSound(false);
		introplaySound(true);
		gameplaySound(false);

		burnCanvas.enabled = false;
		jokerCanvas.enabled = false;
		introCanvas.enabled = false;
		uiCanvas.enabled = true;
		gameCanvas.enabled = true;
	}

	public void gameOver()
    {
		foreach (GameObject go in gameingredients)
        {
            go.SetActive(false);
        }

		requesttext.text = "";
		//ordertext.text = "You ruined my rutial!\n\nGAME OVER! Press button to try again.\nBtw, you just died a horrible slow death by the angry villagers";
		//ordertext.alignment = TextAnchor.UpperCenter;
		ordertext.text = "";
		//playbutton.GetComponent<CanvasGroup>().alpha = 1;
		//playbutton.interactable = true;
		playbutton.gameObject.SetActive(true);

		gameCanvas.enabled = false;
		uiCanvas.enabled = false;
		burnCanvas.enabled = true;

		if (gameSound != null)
		{
			gameSound.Stop();
		}
		gameoverSound(true);


	}

	private void gameoverSound(bool on)
	{
		if (on)
		{
			if (fireCrySound != null)
			{
				if (!fireCrySound.isPlaying)
					fireCrySound.Play();
			}
			if (fireSound != null)
			{
				if (!fireSound.isPlaying)
					fireSound.Play();
			}
		}
		else
		{
			if (fireCrySound != null)
			{
				fireCrySound.Stop();
			}
			if (fireSound != null)
			{
				fireSound.Stop();
			}
		}
	}

	private void gameplaySound(bool on)
	{
		if (on)
		{
			if (gameSound != null)
			{
				if (!gameSound.isPlaying)
					gameSound.Play();
			}
		} else
		{
			if (gameSound != null)
			{
				gameSound.Stop();
			}
		}
	}
	
	private void introplaySound(bool on)
	{
		if (on)
		{
			if (introSound != null)
			{
				if (!introSound.isPlaying)
					introSound.Play();
			}
		} else
		{
			if (introSound != null)
			{
				introSound.Stop();
			}
		}
	}

    public void createNewGame()
    {

		gameoverSound(false);
		introplaySound(false);
		gameplaySound(true);
		countdown.startTimer();
		if (gameSound != null)
		{
			gameSound.Play();
		}
		rounds = origRounds;
		currentRound = 0;
		
		gameCanvas.enabled = true;
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
		currentRound++;
		if (currentRound > rounds)
		{
			// WinnerBurn
			winnerBurn();
		}
		else
		{
			Debug.Log("Playing a new round, so adding all the peoples again");
			roundCustomers = new List<Customer>(customers.ToArray()); // origCustomers;
		}
	}

	private void winnerBurn()
	{
		Debug.Log("YOU WON BUT ALSO LOST THE GAME.\nTHEY ARE NOW BURNING YOUR BODY BECAUSE YOU'RE DOING WHICHCRAFT!");
		gameCanvas.enabled = false;
		burnCanvas.enabled = true;
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
		if (roundCustomers.Count > 0)
		{
			ordertext.text = "";
			ordertext.alignment = TextAnchor.UpperLeft;
			requesttext.text = "";

			int customerIndex = Random.Range(0, roundCustomers.Count);
			Customer customer = roundCustomers[customerIndex];
			roundCustomers.RemoveAt(customerIndex);
			Debug.Log("Now there are " + roundCustomers.Count + " peoples left");
			newCustomer(customer);
		}
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

