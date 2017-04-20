<Query Kind="Program">
  <Namespace>System.Globalization</Namespace>
</Query>

void Main()
{
	bool updateIndex = true;

	var folderToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "sig", "Sig Sauer" },
		{ "sw", "Smith & Wesson" },
		{ "cz", "CZ" },
		{ "hk", "Heckler & Koch" },
	};

	var gunToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "lcpii", "LCP II" },
		{ "lc9s", "LC9s" },
		{ "sr9c", "SR9c" },
		{ "sr40c", "SR40c" },
		{ "9e", "9E" },
		{ "sr9", "SR9" },
		{ "sr40", "SR40" },
		{ "sr45", "SR45" },
		
		{ "cz 2075 rami", "CZ 2075 RAMI" },
		{ "cz75 compact", "CZ 75 Compact" },
		{ "cz75b", "CZ 75 B" },
		{ "cz75 sp01", "CZ 75 SP-01" },
		{ "cz97 b", "CZ 97 B" },

		{ "xd 3 9", "XD 3\" 9mm" },
		{ "xd 4 9", "XD 4\" 9mm" },
		{ "xd 5 9", "XD 5\" 9mm" },
		{ "xds 3 9", "XD-S 3.3\" 9mm" },
		{ "xds 4 9", "XD-S 4\" 9mm" },
		{ "xd mod2 3 9", "XD MOD.2 3\" 9mm" },
		{ "xd mod2 4 9", "XD MOD.2 4\" 9mm" },
		{ "xd mod2 5 9", "XD MOD.2 5\" 9mm" },
		{ "xdm 4 9", "XD(M) 4.5\" 9mm" },
		{ "xdm 3 9", "XD(M) 3.8\" Compact 9mm" },
		{ "xdm 3 full 9", "XD(M) 3.8\" 9mm" },

		{ "cw380", "CW380" },
		{ "pm9", "PM9" },
		{ "cw9", "CW9" },

		{ "p2000sk", "P2000SK" },
		{ "p30sk", "P30SK" },
		{ "vp9", "VP9" },
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
	bool first = true;
	
	foreach (var group in guns.OrderBy(x => x.Key))
	{
		string arrow = "&#9656;";
		string display = "";
		
		if (first)
		{
			arrow = "&#9662;";
			display = "style=\"display: block\"";
			first = false;
		}

		string headerName = textInfo.ToTitleCase(group.Key);
		
		if (folderToName.ContainsKey(group.Key))
			headerName = folderToName[group.Key];
		
		list.Append($"\t\t<p class=\"list-header\"><span>{arrow}</span><strong>{headerName}</strong></p>\r\n\r\n\t\t<ul class=\"list-guns\" {display} >\r\n");
		
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
		images.Append($"\t\t<img id=\"img_{id}\" class=\"gun-image\" src=\"./images/loading.gif\" data-src=\"{gun.Path.Replace("&", "&amp;")}\" style=\"z-index: {1000 - gun.Length}\" />\r\n");
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
