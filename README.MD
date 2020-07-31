# Memory.Trainer

This is a simple and currently poorly written application to easily create a trainer for several games

# Currently included sample games
- Lego City Undercover
  - Studs
  - Building Studs
  - Every Red Brick
- The Lego Movie Videogame
  - Studs
  - Studs Multiplier Red Bricks

# Add a new game
To create a new game you just need to add a json to the **MemoryFiles** directory.
The file should look like this:

``` json
{
	"Name":"THE_NAME_OF_THE_APPLICATION",
	"ProcessName":"THE_PROCESS_NAME",
	"MemoryAddresses": [
		{
			"Name": "THE_NAME_OF_THE_ADDRESS",
			"Type": "THE_TYPE_OF_THE_ADDRESS", //currently only 'int', 'string', 'float' and 'byte' is supported
			"Addresses": [
				"LEGOLCUR_DX11.exe+0x01C7E640,0x0" // A list of adresses (If you're not sure the adress you found works you can add more to list)
			]
		}
	]
}
```

# Addition
If you find any bugs or have some improvements feel free to contact me (Create an issue please)