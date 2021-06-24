using System;
using System.Collections;
using System.Collections.Generic;

public class Button : Box {

    public string text;
    public ConsoleColor fg_col;
    public ConsoleColor bg_col;

    public ConsoleColor select_fg_col;
    public ConsoleColor select_bg_col;
    public format_options format;
    public int line;
    public ConsoleKey to_use;
    public bool is_in_list;
    public List<Button> button_buddies;

    public Button(string textt, int line, format_options fo = format_options.middle, ConsoleColor foreground = ConsoleColor.Red, ConsoleColor background = ConsoleColor.Yellow, ConsoleColor select_foreground = ConsoleColor.DarkRed, ConsoleColor select_background= ConsoleColor.White, ConsoleKey key_to_use = ConsoleKey.A) {
        text = textt;
        fg_col = foreground;
        bg_col = background;
        select_fg_col = select_foreground;
        select_bg_col = select_background;
        format = fo;
        to_use = key_to_use;
    }

    public Button(string textt, int line, format_options fo = format_options.middle, ConsoleKey key_to_use = ConsoleKey.A) {
        text = textt;
        fg_col = ConsoleColor.Red;
        bg_col = ConsoleColor.Yellow;
        select_fg_col = ConsoleColor.DarkRed;
        select_bg_col = ConsoleColor.White;
        format = fo;
        to_use = key_to_use;
    }

    public Button(string textt, int line, bool is_part_of_list, List<Button> friends,  format_options fo = format_options.middle) {
        text = textt;
        fg_col = ConsoleColor.Red;
        bg_col = ConsoleColor.Yellow;
        select_fg_col = ConsoleColor.DarkRed;
        select_bg_col = ConsoleColor.White;
        format = fo;
        is_in_list = is_part_of_list;
        button_buddies = friends;
    }

    public Button(string textt, int line, bool is_part_of_list, List<Button> friends, format_options fo = format_options.middle, ConsoleColor foreground = ConsoleColor.Red, ConsoleColor background = ConsoleColor.Yellow, ConsoleColor select_foreground = ConsoleColor.DarkRed, ConsoleColor select_background= ConsoleColor.White) {
        text = textt;
        fg_col = foreground;
        bg_col = background;
        select_fg_col = select_foreground;
        select_bg_col = select_background;
        format = fo;
        is_in_list = is_part_of_list;
        button_buddies = friends;
    }


}