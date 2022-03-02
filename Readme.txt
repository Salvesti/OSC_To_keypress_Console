This is a simple console based application that allows you to take an OSC output and translate it to a keypress, 
or take a keypress and translate it to an OSC input.

To send a key combination from a OSCMessage:
Have a parameter on your avatar that matches one of the lines in 'ReceiverHotkeys.txt', and a keybind added in the program you want to activate that matches the key combination.


To send a OSCMessage from a key combination
A parameter on your model that you want to trigger from a keybind and an associated line in the 'SenderHotkeys.txt' file.


Then you just run the program OSC_To_keypress_Console.exe, and activate OSC from the radial menu in VrChat. 
Once it is running toggling any parameter will send an OSCMessage to activate the associated keybind, 
and pressing any defined keybind will send the associated values to the OSCAddress.


The folder the OSC_To_keypress_Console.exe is in must inclued a file named 'ReceiverHotkeys.txt', and 'SenderHotkeys.txt'.
These files contain the hotkeys binds.

The 'ReceiverHotkeys.txt', and 'SenderHotkeys.txt' files use VirtualKeyCodes.
Reference the included InfoForKeys.txt for specific key names.
Some oddities of note:
MENU is the keyname for ALT.
Letter keys are VK_{Letter} so 'VK_A' for 'A'

----------------
'ReceiverHotkeys.txt' defines OSC Outputs that should trigger system key presses.
The file can be empty if you do not want any OSC Outputs to trigger system key presses.

Each line of the file must be in the format:

Key_Name{tab}Key_Name{tab}OSC_Address{tab}data_type{tab}data_type_value

example line:
NUMPAD1	LCONTROL	/avatar/parameters/keybind1	bool	true

OSC addresses must have no spaces in them.
If you want only a single key to be pressed just set it for both key names.
If the given OSC_Address is recieved the associated key combination will be triggered.

----------------
'SenderHotkeys.txt' defines system key presses that should trigger OSC Inputs.
The file can be empty if you do not want any key presses to trigger OSC inputs.

Each line of the file must be in the format:

Key_Name{tab}Key_Name{tab}OSC_Address{tab}data_type{tab}data_type_value

example lines:
VK_A	MENU	/avatar/parameters/keypress1	bool	true
VK_B	MENU	/avatar/parameters/keypress2	int	1
VK_C	MENU	/avatar/parameters/keypress3	float	1.2

The data_type names are case sensitive.
OSC addresses must have no spaces in them.

If you want only a single key to need to be pressed just set it for both key names.
If the given key combination is triggered the associated dataType and value will be sent to the given OSC_Address.