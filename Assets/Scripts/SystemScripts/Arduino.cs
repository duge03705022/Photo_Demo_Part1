using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// For Serial
using System.IO.Ports;
// For compare array
using System.Linq;

public class Arduino : MonoBehaviour { 
	// Serial
	public HashSet<int> touchID = new HashSet<int>();
    public Stack<string> touchShowID = new Stack<string>();
    public string portName;
	public int baudRate;
	public SerialPort arduinoSerial;
	public string[] a;
	public int sensor;
    public int count;

	void Start () {
		// Open Serial port
		arduinoSerial = new SerialPort (portName, baudRate);
		// Set buffersize so read from Serial would be normal
		arduinoSerial.ReadTimeout = 50;
		arduinoSerial.ReadBufferSize = 8192;
		arduinoSerial.WriteBufferSize = 128;
		arduinoSerial.ReadBufferSize = 4096;
		arduinoSerial.Parity = Parity.None;
		arduinoSerial.StopBits = StopBits.One;
		arduinoSerial.DtrEnable = true;
		arduinoSerial.RtsEnable = true;
		arduinoSerial.Open ();
        count = 0;
    }

	void Update() {
        if (portName=="COM12")
        {
            count++;
            if (count == 10)
            {
                arduinoSerial.Write("6");
            }
            else if (count % 200 == 0)
            {
                arduinoSerial.Write("1");
            }
            else if (count % 200 == 100)
            {
                arduinoSerial.Write("2");
            }
            else if (count > 11)
            {
                count--;
            }
        }
        else
        {
            ReadFromArduino();
        }
    }

	public void ReadFromArduino () {
		string str = null;
        
        try
        {
            str = arduinoSerial.ReadLine();
            if (str.Length >= 3)
            {
                touchShowID.Push(str);
            }
        }
        catch (TimeoutException)
        {
        }
    }
}