using System.IO;
using System.Net;

namespace ISpyGame;

public class ISpyGame
{
	private List<string> words;
	private List<string> inputOptionWords;
	private string selectedWord;
	private char selectedWordInitialLetter;
	private bool hasPlayerQuit;
	private bool hasPlayerWon;

	public static void Main(string[] args)
	{
		ISpyGame game = new ISpyGame();
	}

	public ISpyGame()
	{
		string input;

		Init();

		ShowGameStartScreen();

		do
		{
			ShowBoard();

			do
			{
				ShowInputOptions();
				input = GetInput();
			}
			while (!IsValidInput(input));

			ProcessInput(input);

			UpdateGameState();
		}
		while (!IsGameOver());

		ShowGameOverScreen();
	}

	public void Init()
	{
		words = new List<string>();
		inputOptionWords = new List<string>();

		string filename = "words.txt";

		if (!File.Exists(filename))
		{
			Console.Write("Downloading word list...");

			string url = "https://raw.githubusercontent.com/dwyl/english-words/master/words.txt";

			using HttpClient client = new HttpClient();
			using FileStream fs = new FileStream(filename, FileMode.Create);

			client.GetStreamAsync(url).Result.CopyTo(fs);

			Console.WriteLine("done!");
		}

		words.AddRange(File.ReadLines(filename));

		Random rnd = new Random();

		int r = rnd.Next(words.Count);

		selectedWord = words[r];

		selectedWordInitialLetter = char.ToLower(selectedWord[0]);

		int s = words.FindIndex(x => x.StartsWith(selectedWordInitialLetter));
		int e = words.FindLastIndex(x => x.StartsWith(selectedWordInitialLetter)) + 1;
		int m = Math.Min(10, e - s);

		for (int i = 0; i < m; i++)
		{
			r = rnd.Next(s, e);

			inputOptionWords.Add(words[r].ToLower());
		}

		r = rnd.Next(inputOptionWords.Count);

		selectedWord = inputOptionWords[r];

		hasPlayerQuit = false;
		hasPlayerWon = false;
	}

	public void ShowGameStartScreen()
	{
		Console.WriteLine("Welcome to I Spy!");
	}

	public void ShowBoard()
	{
		Console.WriteLine();
		Console.WriteLine($"I spy with my little eye, something that starts with... {selectedWordInitialLetter}");
	}

	public void ShowInputOptions()
	{
		Console.WriteLine();

		foreach(string word in inputOptionWords)
		{
			Console.WriteLine(word);
		}

		Console.WriteLine();
		Console.Write("Guess a word: ");
	}

	public string GetInput()
	{
		return Console.ReadLine().Trim().ToLower();
	}

	public bool IsValidInput(string input)
	{
		if (input.Length == 0)
		{
			return true;
		}
		else if (input[0] != selectedWordInitialLetter)
		{
			Console.WriteLine($"Invalid input: word must start with the letter {selectedWordInitialLetter}.");

			return false;
		}
		else
		{
			return true;		
		}
	}

	public void ProcessInput(string input)
	{
		if (input.Length == 0)
		{
			hasPlayerQuit = true;
		}
		else if (input == selectedWord)
		{
			hasPlayerWon = true;
		}
		else
		{
			Console.WriteLine("Wrong guess. Try again!");
		}
	}

	public void UpdateGameState()
	{

	}

	public bool IsGameOver()
	{
		return hasPlayerWon || hasPlayerQuit;
	}

	public void ShowGameOverScreen()
	{
		if (hasPlayerWon)
		{
			Console.WriteLine("You won!");
		}
		else // if(hasPlayerQuit)
		{
			Console.WriteLine($"You lost. The secret word was {selectedWord}.");
		}
	}
}
