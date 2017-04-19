<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	bool updateIndex = false;
	
	var folderToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "glock", "Glock" },
		{ "ruger", "Ruger" },
		{ "sig", "Sig Sauer" },
		{ "sw", "Smith & Wesson" },
	};

	var gunToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "lcpii", "LCP II" },
		{ "lc9s", "LC9s" },
	};

	TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
	var guns = new Dictionary<string, List<Gun>>();
	
	foreach (string dir in Directory.EnumerateDirectories(@"C:\Users\Malvin\Documents\GitHub\nivlam.github.io\guns"))
	{
		string group = new DirectoryInfo(dir).Name;
		
		if (!guns.ContainsKey(group))
			guns.Add(group, new List<Gun>());
			
		foreach (string image in Directory.EnumerateFiles(dir))
		{
			string name = new FileInfo(image).Name;
			int index = name.IndexOf('_');
			
			int length = Convert.ToInt32(name.Substring(0, index));
			string gunName = Path.GetFileNameWithoutExtension(name.Substring(index + 1));
					
			string displayName = textInfo.ToTitleCase(gunName.Replace('_', ' '));
			
			if (gunToName.ContainsKey(displayName))
				displayName = gunToName[displayName];
			
			guns[group].Add(new Gun(group, length, image, displayName));
		}
	}

	StringBuilder list = new StringBuilder();
	StringBuilder images = new StringBuilder();
	
	foreach (var group in guns.OrderBy(x => x.Key))
	{
		list.Append($"\t\t<p><strong>{folderToName[group.Key]}</strong></p>\r\n\r\n\t\t<ul>\r\n");
		
		foreach (Gun gun in group.Value.OrderByDescending(x => x.Length))
		{
			string id = $"{group.Key.ToLower()}_{Path.GetFileNameWithoutExtension(gun.Path).Replace("&", "").Replace(".", "")}";

			list.Append($"\t\t\t<li>\r\n\t\t\t\t<input type=\"checkbox\" id=\"{id}\" class=\"chkGun\" data-show=\"img_{id}\" />\r\n\t\t\t\t<label for=\"{id}\">{gun.Name}</label>\r\n\t\t\t</li>\r\n");
		}
		
		list.Append("\t\t</ul>\r\n\r\n");
	}
	
	List<Gun> allGuns = guns.SelectMany(x => x.Value).ToList();
	
	foreach (Gun gun in allGuns.OrderByDescending(x => x.Length))
	{
		string id = $"{gun.Group.ToLower()}_{Path.GetFileNameWithoutExtension(gun.Path).Replace("&", "").Replace(".", "")}";
		images.Append($"\t\t<img id=\"img_{id}\" class=\"gunImage\" src=\"./images/loading.gif\" data-src=\"{gun.Path.Replace("&", "&amp;")}\" style=\"z-index: {1000 - gun.Length}\" />\r\n");
	}
	
	string beta = File.ReadAllText(@"C:\Users\Malvin\Documents\GitHub\nivlam.github.io\template.html")
		.Replace("{{LIST}}", list.ToString())
		.Replace("{{IMAGES}}", images.ToString());
		
	File.WriteAllText(@"C:\Users\Malvin\Documents\GitHub\nivlam.github.io\beta.html", beta);
	
	if (updateIndex)
		File.WriteAllText(@"C:\Users\Malvin\Documents\GitHub\nivlam.github.io\index.html", beta);
	
	Console.WriteLine("Done");
}

public class Gun
{
	public string Group { get; set; }
	public int Length { get; set; }
	
	private string _path;
	public string Path
	{
		get
		{ 
			return _path.Replace(@"C:\Users\Malvin\Documents\GitHub\nivlam.github.io\", "./").Replace("\\", "/");
		}
		set 
		{
			_path = value;
		}
	}

	public string Name { get; set; }

	public Gun(string group, int length, string path, string name)
	{
		this.Group = group;
		this.Length = length;
		this.Path = path;
		this.Name = name;
	}
}
