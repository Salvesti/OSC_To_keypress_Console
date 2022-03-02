extern alias sxzd;

using sxzd::Rug.Osc;
using WindowsInput.Native;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Forms;

namespace OSC_To_keypress_Console
{
    class OscToKeypress
    {
        static OscReceiver receiver;
        static OscSender sender;
        static Thread recieveThread;
        static Thread sendThread;

        private static IntPtr _hookID = IntPtr.Zero;
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, IntPtr hHotKey);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        static void Main(string[] args)
        {


            // This is the port we are going to listen on 
            int port = 9001;
            int sendPort = 9000;
            IPAddress address = IPAddress.Parse("127.0.0.1");

            // Create the receiver
            Console.WriteLine("Receiving Port: " + port);
            receiver = new OscReceiver(port);

            Console.WriteLine("Sending Port: " + sendPort);
            sender = new OscSender(address,port,sendPort);


            // Create a thread to do the listening
            recieveThread = new Thread(new ThreadStart(ListenLoop));
            sendThread = new Thread(new ThreadStart(SenderListenLoop));
            
            // Connect the receiver
             receiver.Connect();
            
            sender.Connect();

            // Start the listen thread
            recieveThread.Start();

            // Start the send thread
            sendThread.Start();

            // wait for a key press to exit
            Console.Title = "OSC To Keybind";
            Console.WriteLine("Please reference the Readme for info on setting Parameters and associated Keybinds");
            //Console.WriteLine("Press any key to exit");
            //Console.ReadKey(true);

            // close the Reciver 
            //receiver.Close();

            // Wait for the listen thread to exit
            //recieveThread.Join();
            //sendThread.Join();
        }

        static void SenderListenLoop()
        {
            HotKeyFileReader senderFileReader = new HotKeyFileReader();
            HotKeyInfo senderHotkeys = senderFileReader.translalteToHotkeys("SenderHotkeys.txt");
            KeybindToOSC[] senderKeybinds = new KeybindToOSC[senderHotkeys.getHotkeyCount()];
            if (senderHotkeys.getHotkeyCount() == 0)
            {
                Console.WriteLine("--");
                Console.Write("SenderHotkeys.txt contains no defined hotkeys.");
            }
            //Generate listeners for keypresses
            for (int i = 0; i <  senderHotkeys.getHotkeyCount(); i++)
            {
                senderKeybinds[i] = new KeybindToOSC(senderHotkeys.getHotkey(i), senderHotkeys.getModifier(i), senderHotkeys.getAddress(i), senderHotkeys.getDataType(i), senderHotkeys.getDataTypeValue(i));
            }           

            try
            {
                while (true)
                {
                    for (int i = 0; i < senderHotkeys.getHotkeyCount(); i++)
                    {
                        //check each listener to see if its keybind has been pressed.
                        if(senderKeybinds[i].listenForKeybind())
                        {
                            //Connect the sender
                            sender.Send(senderKeybinds[i].generateOSCMessage());
                            sender.WaitForAllMessagesToComplete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if the socket was connected when this happens
                // then tell the user
                if (sender.State == OscSocketState.Connected)
                {
                    Console.WriteLine("Exception in sender listen loop");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void ListenLoop()
        {
            HotKeyFileReader receiverFileReader = new HotKeyFileReader();
            HotKeyInfo receiverHotkeys = receiverFileReader.translalteToHotkeys("ReceiverHotkeys.txt");
            OSCToKeybind[] receiverKeybinds = new OSCToKeybind[receiverHotkeys.getHotkeyCount()];
            if(receiverHotkeys.getHotkeyCount() == 0)
            {
                Console.WriteLine("--");
                Console.Write("ReceiverHotkeys.txt contains no defined hotkeys.");
            }
            //Generate listeners for ocs packets.
            for (int i = 0; i < receiverHotkeys.getHotkeyCount(); i++)
            {
                receiverKeybinds[i] = new OSCToKeybind(receiverHotkeys.getHotkey(i), receiverHotkeys.getModifier(i), receiverHotkeys.getAddress(i),receiverHotkeys.getDataType(i),receiverHotkeys.getDataTypeValue(i));
            }
            
            try
            {
                while (receiver.State != OscSocketState.Closed)
                {
                    // if we are in a state to recieve
                    if (receiver.State == OscSocketState.Connected)
                    {
                        // get the next message 
                        // this will block until one arrives or the socket is closed
                        OscPacket packet = receiver.Receive();
                        for (int i = 0; i < receiverHotkeys.getHotkeyCount(); i++)
                        {
                            //let each receiver see the package and determine if it is one it handles.
                            receiverKeybinds[i].receiveOSCRequest(packet);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // if the socket was connected when this happens
                // then tell the user
                if (receiver.State == OscSocketState.Connected)
                {
                    Console.WriteLine("Exception in listen loop");
                    Console.WriteLine(ex.Message);
                }
            }
        }


    }
    class OSCToKeybind{
        VirtualKeyCode key;
        VirtualKeyCode keyMod;
        String address;
        String dataType;
        String dataTypeValue;

        public OSCToKeybind(VirtualKeyCode key, VirtualKeyCode keyMod, String address, String dataType, String dataTypeValue)
        {
            this.key = key;
            this.keyMod = keyMod;
            this.address = address;
            this.dataType = dataType;
            this.dataTypeValue = dataTypeValue;
        }

        public void receiveOSCRequest(OscPacket packet)
        {
            String[] splitPacket = packet.ToString().Split();
            String addressIncoming = splitPacket[0];
            string dataTypeValueIncoming = splitPacket[1];
            if (addressIncoming == address+"," && dataTypeValueIncoming.ToLower() == dataTypeValue.ToLower())
            {
                var simulator = new WindowsInput.InputSimulator();
                //Run a keystroke command using given modifier.
                simulator.Keyboard.ModifiedKeyStroke(keyMod, key);
                Console.WriteLine("--");
                Console.WriteLine("OSC Packet Received:" + packet.ToString());
                Console.WriteLine("Running associated Hotkey: "+key+" + "+keyMod);
            }
        }
    }


    class KeybindToOSC
    {
        VirtualKeyCode key;
        VirtualKeyCode keyMod;
        String address;
        String dataType;
        String dataTypeValue;
        public KeybindToOSC(VirtualKeyCode key, VirtualKeyCode keyMod, String address, String dataType, String dataTypeValue)
        {
            this.key = key;
            this.keyMod = keyMod;
            this.address = address;
            this.dataType = dataType;
            this.dataTypeValue = dataTypeValue;
        }

        public bool listenForKeybind()
        {
            var simulator = new WindowsInput.InputSimulator();
            if(simulator.InputDeviceState.IsKeyDown(key) && simulator.InputDeviceState.IsKeyDown(keyMod)){
                Console.WriteLine("--");
                Console.WriteLine("Keybind detected: " + key + " + " + keyMod);
                return true;
            }
            return false;
        }

        public OscMessage generateOSCMessage()
        {
            OscMessage message = null;
            switch (dataType)
            {
                case "int":
                    message = new OscMessage(address, int.Parse(dataTypeValue));
                    break;
                case "float":
                    message = new OscMessage(address, float.Parse(dataTypeValue));
                    break;
                case "bool":
                    message = new OscMessage(address, bool.Parse(dataTypeValue));
                    break;
            }
            Console.WriteLine("Sending associated message: " + message.ToString());
            return message;
        }
    }
     

        
}