<Query Kind="Statements" />

foreach (string file in Directory.EnumerateFiles(@"C:\Users\Malvin\Documents\GitHub\nivlam.github.io\guns\springfield"))
{
	var i = Path.GetFileNameWithoutExtension(file).Substring(4).Replace("_", " ");
	Console.WriteLine($"{{ \"{i}\", \"\" }},");
}