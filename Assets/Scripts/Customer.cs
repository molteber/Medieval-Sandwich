using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Customer : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum;             //Minimum value for our Count class.
		public int maximum;             //Maximum value for our Count class.


		//Assignment constructor.
		public Count(int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}


	private Count ingredientRange = new Count(1, 3);
	private List<IngredientAmount> amountIngredients;

	class IngredientAmount
	{
		public Ingredient ingredient;
		public int amount;

		public IngredientAmount(Ingredient t, Count range)
		{
			ingredient = t;
			amount = Random.Range(range.minimum, range.maximum + 1);
		}
	}

	public List<Ingredient> order = new List<Ingredient>();
	public string title;
	public int appeared = 0;
	private int origAppeared;

	public string getName()
	{
		return this.title;
	}

	void init()
	{
		origAppeared = appeared;
		reset();
	}

	public void reset()
	{
		appeared = origAppeared;
        amountIngredients = new List<IngredientAmount>();
		order = new List<Ingredient>();

		amountIngredients.Add(new IngredientAmount(GameManager.instance.meat, ingredientRange));
		amountIngredients.Add(new IngredientAmount(GameManager.instance.fish, ingredientRange));
		amountIngredients.Add(new IngredientAmount(GameManager.instance.cheese, ingredientRange));
		amountIngredients.Add(new IngredientAmount(GameManager.instance.lettuce, ingredientRange));

		// Sort them
		amountIngredients.Sort(delegate (IngredientAmount a, IngredientAmount b)
		{
			return a.amount.CompareTo(b.amount);
		});

		foreach (IngredientAmount ia in amountIngredients)
		{
			addToOrder(ia.ingredient, ia.amount);
		}
		IListExtensions.Shuffle<Ingredient>(order);

		// Adding a peice of bread in the beginning and the end of the order, this way we complete our "Sandwich"
		order.Insert(0, GameManager.instance.bread);
		order.Add(GameManager.instance.bread);
	}

	public static class Greetings
	{
		private static string[] greetings = {
			"Hi, can you please make me a sandwich?",
			"Hello, kind Sir. I request a sandwich from thee",
			"Hi, i want something to eat",
			"Make me a sandwich just the way i like it"
		};

		private static string[] hungry =
		{
			"Just make me my sandwich!",
			"Haven't you prepared my sandwich??",
			"FOOD! NOW!",
			"Hurry, I have to give it to the king!\nI'm so getting sacrificed today because you're so slow.."
		};

		public static string getGreeting()
		{
			return greetings[Random.Range(0, greetings.Length)];
		}
		public static string makeMySandwich()
		{
			return hungry[Random.Range(0, hungry.Length)];
		}
	}

	public string createOrderText()
	{
		string output = ""; //Greetings.getGreeting() + "\n";
		if (appeared < 2)
		{
			Debug.Log(order[0]);
			foreach (Ingredient o in order)
			{
				output += " - " + o.title + "\n";
			}
			//char[] trim = { ' ', ',' };

			// Remove the last space and comma
		}
		else if ((appeared - 2) >= amountIngredients.Count)
		{
			output = "The usual";
			output = Greetings.makeMySandwich();
		}
		else
		{
			// How many ingredients should we short down?
			int shortdown = appeared - 2;

			IDictionary<string, Ingredient> ignorelist = new Dictionary<string, Ingredient>();

			Ingredient ignorelistTryGet = null;

			output += " - " + order[0].title + "\n";

			for (var i = 0; i <= shortdown && i < amountIngredients.Count; i++)
			{
				//Debug.Log(ignorelist.Count + " Dictionary "+amountIngredients[i].ingredient.code);
				try {
					ignorelist.Add(amountIngredients[i].ingredient.code, amountIngredients[i].ingredient);
				} catch(ArgumentException e)
				{
					Debug.Log("Erroooor!");
					string log = "";
					foreach (IngredientAmount ia in amountIngredients)
					{
						log += " " + ia.ingredient.code;
					}
					Debug.Log(log);
				}
				//if (amountIngredients[i].amount > 1)
				//{
				//	output += " - " + amountIngredients[i].ingredient.plural + "\n";
				//}
				//else
				//{
				//	output += " - " + amountIngredients[i].ingredient.title + "\n";
				//}

			}

			for (var i = 1; i < order.Count; i++)
			{
				if (ignorelist.TryGetValue(order[i].code, out ignorelistTryGet))
				{
					if (ignorelistTryGet.code == order[i].code)
					{
						continue;
					}
				}
				output += " - " + order[i].title + "\n";
			}

			if (ignorelist.Count > 0)
			{
				output += "\nyeah, and\n";

				foreach (string key in ignorelist.Keys)
				{
					Ingredient i;
					if (ignorelist.TryGetValue(key, out i))
					{
						output += " - " + i.plural + "\n";
					}
				}
			}
		}
		appear();
		return output;
	}

	public void appear()
	{
		this.appeared++;
	}

	private void addToOrder(Ingredient ingredient, int amount)
	{
		for (var i = 0; i < amount; i++)
		{
			this.order.Add(ingredient);
		}
	}
}
