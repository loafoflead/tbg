using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class FileManager {

    Random rand = new Random();
    public string most_recent_file;

    public FileManager() {

    }


    public int write_at(string content, int line, string file) {
        try {

            StreamWriter str_wr = new StreamWriter(file);

            for (int i = 0; i < line; i ++) {
                str_wr.WriteLine(content);
            }

            str_wr.Close();
            return 1;

        } catch {
            return 0;
        }
    }

    public int write_next(string content, string file) {

        try {

            StreamReader str_r = new StreamReader(file);

            int index = 0;

            while(str_r.Peek() != -1) {
                index ++;
            }

            str_r.Close();

            return write_at(content, index, file);

        } catch {
            return 2;
        }

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

            r.Close();

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

    public string newFile(string filename, string path = @"logs\") {
        string file = path + filename; //the @ means you dont have to use escape sequences when making \n newline or \\ slashes 
        

        if (File.Exists(file)) {
            string f = filename + "_" + DateTime.Now.Minute.ToString().Replace(@"\", "/");
            newFile(f, path);
        }
        else {

            StreamWriter write = File.CreateText(file);
            write.Close();
            return file;

        }

        return "Error";

    }


}