using System;
using static System.Console;
using System.Threading;
using System.Threading.Tasks;

public class KeyHandler {

    public bool is_listening = false;

    public ConsoleKeyInfo latest_key;
    public string last_key_str;
    public string before_last_key_str;
    public ConsoleKeyInfo before_last_key;
    public bool recently_pressed = false;


    private Thread key_listener;

    public KeyHandler() {
        key_listener = new Thread(get_key_info);
        t = new Thread(hollup);
    }




    public ConsoleKeyInfo GetKey() {
        return Console.ReadKey();
    }
    public void waitAnyKey() {
        Console.ReadKey();
    }


    public void toggleListener() {
        if (is_listening == true) {
            is_listening = false;
        }
        else {
            is_listening = true;
        }
    }
    public void startAsyncKeyListener() {
        key_listener.Start();
        is_listening = true;
    }

    public void stopListener() {
        is_listening = false;
    }
    public void startListener() {
        is_listening = true;
    }
    public void forceStop() {
        Thread t = key_listener;
        throw new Exception();
    }

    private Thread t;

    void get_key_info() {
        while (is_listening == true) {
            ConsoleKeyInfo k_info = Console.ReadKey();
            before_last_key = latest_key;
            before_last_key_str = last_key_str;
            latest_key = k_info;
            last_key_str = k_info.KeyChar.ToString();
            if (last_key_str == "/") {
                Console.WriteLine("t: " + t.IsAlive + " listener: " + key_listener.IsAlive);
            }
            if (!t.IsAlive) {
                t = new Thread(hollup);
                t.Start();
            } 
        }
        
    }

    void hollup() {
        recently_pressed = true;
        Thread.Sleep(1000);
        recently_pressed = false;
    }


}