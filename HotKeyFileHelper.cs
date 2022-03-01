using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using WindowsInput.Native;

namespace OSC_To_keypress_Console
{
    class HotKeyFileReader
    {
        public string[] readFromFile(string filePath)
        {
            return System.IO.File.ReadAllLines(filePath);
        }

        public HotKeyInfo translalteToHotkeys(string filePath)
        {
            HotKeyInfo hotkeyInfo = new HotKeyInfo();

            string[] fileContents = readFromFile(filePath);
            int i = 0;
            foreach (string line in fileContents)
            {
                String[] splitLine = line.Split("\t");
                VirtualKeyCode key = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), splitLine[0], true);
                VirtualKeyCode modifier = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), splitLine[1], true);
                string address = splitLine[2];
                
                //if has datatypevalue line
                if(splitLine.Length == 5)
                {
                    string dataType = splitLine[3];
                    string dataTypeValue = splitLine[4];
                    hotkeyInfo.addHotKey(i, key, modifier, address, dataType, dataTypeValue);
                }
                else if(splitLine.Length == 3)
                {
                    hotkeyInfo.addHotKey(i, key, modifier, address);
                }
                i++;
            }
            return hotkeyInfo;
        }
    }

    class HotKeyInfo
    {
        private List<VirtualKeyCode> hotkeys;
        private List<VirtualKeyCode> modifiers;
        private List<String> addresses;
        private List<String> dataTypes;
        private List<String> dataTypeValues;
        public HotKeyInfo() 
        {
            hotkeys = new List<VirtualKeyCode>();
            modifiers = new List<VirtualKeyCode>();
            addresses = new List<string>();
            dataTypes = new List<string>();
            dataTypeValues = new List<string>();
        }

        public void addHotKey(int arrayPosition, VirtualKeyCode hotkey, VirtualKeyCode modifier, string address)
        {
            hotkeys.Add(hotkey);
            modifiers.Add(modifier);
            addresses.Add(address);
        }
        public void addHotKey(int arrayPosition, VirtualKeyCode hotkey, VirtualKeyCode modifier, string address, string dataType, string dataTypeValue)
        {
            hotkeys.Add(hotkey);
            modifiers.Add(modifier);
            addresses.Add(address);
            dataTypes.Add(dataType);
            dataTypeValues.Add(dataTypeValue);
        }

        public VirtualKeyCode getHotkey(int position)
        {
            return hotkeys[position];
        }
        public VirtualKeyCode getModifier(int position)
        {
            return modifiers[position];
        }

        public string getAddress(int position)
        {
            return addresses[position];
        }

        public string getDataType(int position)
        {
            return dataTypes[position];
        }

        public string getDataTypeValue(int position)
        {
            return dataTypeValues[position];
        }

        public int getHotkeyCount()
        {
            return hotkeys.Count;
        }

    }   
}
