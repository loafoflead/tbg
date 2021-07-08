using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class Box {

    public List<string> buffer;
    public char seperator_character = '█';
    public int height;
    public int width;

    public KeyHandler k;

    public ConsoleColor default_foreground = ConsoleColor.White;
    public ConsoleColor default_background = ConsoleColor.Black;

    public ConsoleColor box_colour = ConsoleColor.White;

    public Box() {
        buffer = new List<string>();
        k = new KeyHandler();
        k.startAsyncKeyListener();
    }

    public void print_screen() {
        refresh_box();
        flush();
    }

    public void print_screen_del() {
        refresh_box();
        flush_del();
    }

    public enum format_options {
        left,
        middle,
        right,
    }

    public void PrintD(string to_print) {
        Print("{Purple}##" + to_print);
    }



    public void Print(string to_print, format_options fo = format_options.left) {
        height = Console.WindowHeight;
        width = Console.WindowWidth;

        if (to_print.Contains("\n")) {
            Print(to_print.Split("\n",2)[0], fo);
            Print(to_print.Split("\n",2)[1], fo);
            return;
        }

        if (buffer.Count > height - 8) {
            for (int i = 0; i <  buffer.Count -(height - 15); i ++) {
                buffer.Remove(buffer[i]);
            }
        }

        if (to_print == null) {
            buffer.Add("{Red}[EMPTY_LINE]{end}");
            return;
        }
        /*if (to_print.Length > width - 5) {
            string[] ne = split_at(to_print, width - 2);
            
            switch(fo) {
                case format_options.left:
                    buffer.Add(ne[0]);
                break;

                case format_options.middle:
                    string tem = "";
                    for (int i = 0; i < (width - ne[0].Length) / 2; i ++) {
                        tem += " ";
                    }
                    tem += ne[0];
                    Console.WriteLine(tem);
                    buffer.Add(tem);
                break;

                case format_options.right:
                    string temp = "";
                    for (int i = 0; i < width - ne[0].Length; i ++) {
                        temp += " ";
                    }
                    temp += ne[0];
                    buffer.Add(temp);
                break;
            }
            
            
            
            //Print(ne[1], fo);
        }
        else {*/
            switch(fo) {
                case format_options.left:
                    buffer.Add(to_print);
                break;

                case format_options.middle:
                    string tem = "";
                    for (int i = 0; i < (width - to_print.Length) / 2; i ++) {
                        tem += " ";
                    }
                    tem += to_print;
                    Console.WriteLine(tem);
                    buffer.Add(tem);
                break;

                case format_options.right:
                    string temp = "";
                    for (int i = 0; i < width - to_print.Length; i ++) {
                        temp += " ";
                    }
                    temp += to_print;
                    buffer.Add(temp);
                break;
            }
        //}
    }

    string[] split_at(string to_split, int index) {
        string[] to_return = new string[2];
        for (int i = 0; i < index; i ++) {
            to_return[0] += to_split[i];
        }
        for (int i = index; i < to_split.Length; i ++) {
            to_return[1] += to_split[i];
        }
        return to_return;
    }

    public void clr() {
        Console.Clear();
    }
    public void nl() {
        buffer.Add(" ");
    }

    public void setCursor() {
        Console.SetCursorPosition(3, height - 4);
        Console.Write(">");
    }


    public void refresh_box() {
        Console.Clear();
        height = Console.WindowHeight;
        width = Console.WindowWidth;
        Console.SetCursorPosition(0,0);
        Console.ForegroundColor = box_colour;
        Console.Write(seperator_character);
        int i = 0;
        for(i = 0; i < width - 1; i ++) {
            Console.SetCursorPosition(i, 0);
            Console.Write(seperator_character);
        }
        for(i = 0; i < height - 1; i ++) {
            Console.SetCursorPosition(0, i);
            Console.Write(seperator_character);
            Console.SetCursorPosition(width - 1, i);
            Console.Write(seperator_character);
        } 
        for(i = 0; i < width - 1; i ++) {
            Console.SetCursorPosition(i, height - 1);
            Console.Write(seperator_character);
        }
        
    }
    public void flush() {
        height = Console.WindowHeight;
        width = Console.WindowWidth;
        int line_index = 1;
        foreach(string f in buffer) {
            List<sub_string> strings = get_colours(f);
            Console.SetCursorPosition(2, line_index);
            foreach(sub_string str in strings) {
                Console.ForegroundColor = str.fg_color;
                Console.BackgroundColor = str.bg_color;
                Console.SetCursorPosition(Console.CursorLeft, line_index);
                foreach(char ch in str.content.Replace("$", "")) {
                    if (Console.CursorLeft > width - 3) {
                            line_index ++;
                            Console.SetCursorPosition(2, line_index);
                        }
                    Console.Write(ch);
                }
                default_col();
            }
            line_index ++;
        }
        
        
        setCursor();
    }

    public void flush_del(int del = 50) {
        //k.startAsyncKeyListener();
        height = Console.WindowHeight;
        width = Console.WindowWidth;
        int line_index = 1;
        foreach(string f in buffer) {
            List<sub_string> strings = get_colours(f);
            Console.SetCursorPosition(2, line_index);
            foreach(sub_string str in strings) {
                //Console.Write(str.content + ", ");
                
                Console.ForegroundColor = str.fg_color;
                Console.BackgroundColor = str.bg_color;
                Console.SetCursorPosition(Console.CursorLeft, line_index);
                foreach(char b in str.content) {
                    if (k.recently_pressed == true) {
                        //k.stopListener();
                        flush();
                        return;
                    }
                    if (b == '$') {
                        Console.Write("");
                        waitf(1); 
                        continue;
                    }
                    if (b == ' ') Console.Write(b);
                    else {
                        if (Console.CursorLeft > width - 3) {
                            line_index ++;
                            Console.SetCursorPosition(2, line_index);
                        }
                        Console.Write(b);
                        Thread.Sleep(del);
                    }
                }
                
                default_col();
            }
            line_index ++;
            //Console.ReadKey();
            //k.stopListener();
        }
        

        setCursor();
    }

    public void clr_text() {
        Console.Clear();
        refresh_box();
    }

    public void clr_buffer() {
        buffer = new List<string>();
    }

    void default_col() {
        Console.ForegroundColor = default_foreground;
        Console.BackgroundColor = default_background;
    }
    struct sub_string {
        public string content;
        public ConsoleColor fg_color;
        public ConsoleColor bg_color;
    }

    List<sub_string> get_colours(string str) {

        List<sub_string> to_return = new List<sub_string>();

        string q = str;

            if (q.Contains("{")) { //foreach string, if it contains a colour, test the parser
                string[] subs = q.Split("{");
                foreach (string s in subs) {

                string sstring = "";
                string col = "";
                col = s.Split('}')[0];
                if(!s.Contains('}')) {
                    to_return.Add(new sub_string {
                        content = s,
                        fg_color = default_foreground,
                        bg_color = default_background,
                    });
                    continue;
                }

                
                
                sstring = s.Split('}')[1]; //get the text content of the substring
                


                string fg_col = "";
                string bg_col = "";

                if (col.Contains(',')) { //if the colour contains two args, split it in two
                    fg_col = col.Split(',')[0];
                    bg_col = col.Split(',')[1];
                }
                else {
                    fg_col = col; //if not, change only the foreground color
                    bg_col = "0";
                }

                if (fg_col == "end" || fg_col == "def") {
                    fg_col = "15";
                    bg_col = "0";
                }
                


                try { //try and add the int value of the colour to the sub string
                    to_return.Add(new sub_string {
                        content = sstring,
                        fg_color = (ConsoleColor)int.Parse(fg_col),
                        bg_color = (ConsoleColor)int.Parse(bg_col),
                    });
                } catch { //if it fails, try to use the color string
                    try {
                        to_return.Add(new sub_string {
                        content = sstring,
                        fg_color = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), fg_col),
                        bg_color = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), bg_col),
                    });
                    } catch { //if that fails, set the color to the default
                        to_return.Add(new sub_string {
                            content = sstring,
                            fg_color = default_foreground,
                            bg_color = default_background,
                        });
                    }
                }

                }
            }
            else { //if not, add the string to the printing with the default colour
                to_return.Add(new sub_string {
                    content = q,
                    fg_color = default_foreground,
                    bg_color = default_background,
                });
            }

        return to_return;

    }

    public void wait(int milliseconds) {
        Thread.Sleep(milliseconds);
    }

    public void waitf(float seconds) {
        Thread.Sleep((int) seconds * 1000);
    }


    public List<string> copy_buffer() {
        return buffer;
    }

    public void replace_buffer(List<string> new_bffer) {
        buffer = new_bffer;
    }

}