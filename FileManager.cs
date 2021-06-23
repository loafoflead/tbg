using System;
using System.Collections;
using System.Collections.Generic;

public class FileManager {

    Random rand = new Random();

    public FileManager() {

    }

    public string readTxtFileAtLine(string file,int line) {
        string ToBeReturned = "pipi";
        //creates a return string
        try {
            System.IO.StreamReader r = new System.IO.StreamReader(file);
            //creates a streamreader(?)
            try {
                for(int i = 0; i < line; i ++) {
                    ToBeReturned = r.ReadLine();
                }
            } catch {
                ToBeReturned = "Index was too large or too small. ERR_02";
                return ToBeReturned;
            }

            r.Close();
        } catch {
            ToBeReturned = "File not found. ERR_01";
            return ToBeReturned;
        }
        
        
        return ToBeReturned;
    }

    public string[] readall(string file_name) {
        List<string> ToBeReturned = new List<string>();
        //creates a return string
        
        try {

            System.IO.StreamReader r = new System.IO.StreamReader(file_name);

            while (r.Peek() > 0) {
                ToBeReturned.Add(r.ReadLine());
            }

            string[] st = ToBeReturned.ToArray();

            return st;


        } catch {
            string[] n = new string[1];
            n[0] = "Failed to open file. ERR_01";
            return n;
        }
        
        
    }

    public int getRan(int min, int max) {
        return rand.Next(min, max + 1);
    }
    public int getRan1_0() {
        return rand.Next(0,2);
    }


}