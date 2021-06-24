using System;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager {

    public List<Button> button_list;

    public ButtonManager() {
        button_list = new List<Button>();
    }

    public void print_buttons() {
        foreach(Button b in button_list) {
            switch(b.format) {
                case (Box.format_options.left) :
                break;



            }
        }
    }

}