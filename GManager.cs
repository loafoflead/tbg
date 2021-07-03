using System.Collections;
using System.Collections.Generic;


public class GManager {

    public Box box;
    public FileManager fm;
    public KeyHandler kh;
    public Environment env;
    public Player player;
    public Commands cm;

    public bool is_running = true;

    bool intro = false;


    public bool show_old_msgs = true;


    public string log_file = "";


    public GManager() {
        box = new Box();
        fm = new FileManager();
        env = new Environment(this);
        player = new Player(this);
        cm = new Commands(this);

        /*log_file = fm.newFile("log_01");
        if (fm.write_at("pee", 1, log_file) == 0) {
            box.Print("file not found");
            box.flush();
        }*/

        env.load_env("env01");
        if (intro == true) {
        cutscene(cutscene_types.intro);
        box.waitf(1);
        box.clr_buffer();
        cutscene(cutscene_types.custom_txt, "start_cutscene.txt");
        box.waitf(1);
        box.clr_buffer();
        box.refresh_box();
        box.clr_text();

        if (cm.YN("Do you want to read a log file?") == true) {
            loadsave(System.Console.ReadLine());
        }
        else {
            box.Print("[{Red}SIMULATING COMMAND: {end}'{Cyan,White}look{end,end}' {Red}...{end}]");
            box.nl();
            box.Print(env.current_room.desc);
            box.print_screen();
        }
        }

        box.clr_buffer();
        box.refresh_box();
        box.clr_text();
        box.print_screen();

        while (is_running == true) {
            cm.getInput();
            box.print_screen();
        }

        box.k.stopListener();
        
        //env.Unlock_enum(Environment.direction_enum.left);
        //env.Move(Environment.direction_enum.left);

        //box.print_screen();

        
    }

    public enum cutscene_types {
        intro,
        game_over,
        item_pickup,
        custom_txt,
        custom_xml,
        custom_string
    }


    void loadsave(string filename) {

    }

    public void Do(string full_act) {
        Do(full_act.Split(':',2)[0], full_act.Split(':',2)[1]);
    }

    public void Do(string action, string result) {
        if (result.Contains("+") ) {
            if (result.Contains("(") || result.Contains(")")) {
                goto resume;
            }
            
            string[] a = result.Split("+");
            Do(action, a[0]);
            
            for(int j = 1; j < a.Length; j ++) {
                Do(a[j]);
            }
            return;
        }
        
        resume :

        switch (action.Replace(" ", "")) {
            case "give":
                
                Item it = env.get_item_from_tag(result);
                int res = 0;
                if (it.tag != null) res = player.inv.add_to_inv(it);
                else {
                    box.Print("{DarkRed}Item not found: " + it.name);
                    return;
                }
                if (res == 2) {
                    box.Print("Inventory full!");
                }
                else {
                    situ_change(change_types.gain_item, env.get_item_from_tag(result).name);
                }
            break;
            
            case "go":
                int b = env.Go(result);
                System.Console.WriteLine(b.ToString());
                if (b == 0) {
                    box.Print("Internal error 'Go', location not found ERR_09");
                }
                else {
                    situ_change(change_types.move_to, env.get_room_name_by_tag(result));
                }
            break;

            case "addtag":
                player.player_tags.Add(result);
            break;

            case "removetag":
                switch (result) {
                    case "all":
                        player.reset_tags();
                        box.PrintD("Player tags reset.");
                    break;
                    case "rand":
                        player.remove_rand_tag();
                        box.PrintD("Random tag deleted.");
                    break;
                    default:
                        if (player.player_tags.Contains(result)) {
                            player.player_tags.Remove(result);
                            box.PrintD(result + " tag removed.");
                        }
                    break;
                }
            break;

            case "null":
                break;

            case "take":
                switch(result) {
                    case "all":
                        player.inv.reset_inv();
                        situ_change(change_types.lose_item, "{DarkRed}INVENTORY DELETED!{end}");
                    break;

                    case "rand":
                    case "random":
                    case "any":
                        player.inv.remove_rand();
                        situ_change(change_types.lose_item, "{Red}Random item{end}");
                    break;

                    default:
                        foreach(Item i in player.inv.player_inventory) {
                            if (i.tag == result) {
                                player.inv.remov(i);
                                situ_change(change_types.lose_item, i.name);
                                return;
                            }
                        }
                        box.Print("Internal error 'Do' command, 'take' tag invalid.");
                    break;
                }
            break;

            case "cutscene":
                switch(result) {

                    case "intro":
                        cutscene(cutscene_types.intro);
                    break;
                    case "game_over":
                    case "gameover":
                        cutscene(cutscene_types.game_over);
                    break;
                    default:
                        cutscene(cutscene_types.custom_txt, result);
                    break;

                }
            break;

            case "env":
                try {
                    env.load_env(result);
                    situ_change(change_types.change_env, result);
                } catch {
                    box.Print("Invalid environment file was requested to be loaded, or is missing key files in order to be loaded.");
                }
            break;

            case "clear":
                box.clr_buffer();
                box.clr();
            break;

            case "if": //example of syntax: if:inv=headband;go:vault_lobby/say:get headband
                       // new syntax: if:(inv=headband):(go:vault)?(say:get headband)
                       // if:(name=hi):(if:(op=true):(say:no)?(say:pee))?(say:bye)
                       // if:(name=no_name):(say:noname haha cringe)?(say:i like pee)
                       /*if     :    (name=no_name):(if:(inv=headband):(say:hi)?(say:bye))?(say:i like pee)*/
                       // give:headband+print:hb
                       
            try {

                string new_res = result;

                string compare_tag = new_res.Split('=')[0].Split('(',2)[1]; //the tag containing what to compare to, inv, name, etc..

                string condition = new_res.Split("=")[1].Split(")",2)[0].Replace(" ", ""); // the string that denotes the condition to be met by the compare tag


                string without_if_and_condition = new_res.Split(':',2)[1]; //just the results of the action
                
                
                string if_true =  without_if_and_condition.Split('(',2)[1];
               

                for(int i = 0; i < if_true.Length; i ++) {
                    if (if_true[i] == '(') {
                        while (if_true[i] != ')') {
                            i ++;
                        }
                        i ++;
                    }
                    if (if_true[i] == ')') {
                        if_true = split_at(if_true, i)[0];
                    }
                }
                

                string minue_true = split_at(without_if_and_condition,if_true.Length + 2)[1];
                

                string if_false = minue_true.Split('(',2)[1];

                for(int i = 0; i < if_false.Length; i ++) {
                    if (if_false[i] == '(') {
                        while (if_false[i] != ')') {
                            i ++;
                        }
                        i ++;
                    }
                    if (if_false[i] == ')') {
                        if_false = split_at(if_false, i)[0];
                    }
                }
                


                switch(compare_tag.Replace(" ", "")) {
                    
                    case "inv":

                        if(player.inv.player_inventory.Contains(env.get_item_from_tag(condition))) {

                            checkDo(if_true);

                        }
                        else {
                            checkDo(if_false);
                        }

                    break;

                    case "name":
                        
                        if(player.name == condition) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }

                    break;

                    case "room":
                        if(env.current_tag == condition) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "op":
                    bool tf = false;
                        if (condition == "true" || condition == "t" || condition == "1") {
                            tf = true;
                        }
                        else {
                            tf = false;
                        }

                        if(player.is_operator == tf) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "tag":
                        if(player.player_tags.Contains(condition)) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "resp":
                    case "player_in":
                    case "response":
                        box.Print("{Cyan}Input >");
                        box.nl();
                        box.print_screen();
                        if(System.Console.ReadLine() == condition) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                }
            } catch {
                box.Print("Internal error in xml level folder using interactable, command was: " + action + ":" + result);
            }

            break;

            case "print":
                box.Print(result);
            break;

            case "say":
                g = "";
                check_tag(result);
                box.Print("{Cyan}- \"" + g + "{Cyan}\"");
            break;

            default:
                box.Print("Fatal 'Do' Internal Error occurred ERR_07. {DarkRed}" + action + ":" + result);
            break;
        }
    }

    void check_tag(string t) {

        if (!t.Contains("#")) {
            System.Console.WriteLine("piizszs");
            System.Console.ReadKey();
            g = t;
            return;
        }

        string[] hashes = t.Split("#");

        foreach(string f in hashes) {
            System.Console.WriteLine(f);
        }
        System.Console.ReadKey();

        for(int i= 0; i < hashes.Length; i ++) {
            if (i % 2 != 0) {
                switch(hashes[i]) {
                    case "name":
                        g += player.name;
                    break;
                    case "bio":
                        g += player.bio;
                    break;
                    case "room":
                        g += env.current_name;
                    break;
                    case "fun":
                        g += player.fun;
                    break;
                    default:
                        g += "invalid tag ERR_011";
                    break;
                }
            }
            else {
                g += hashes[i];
            }
        }


        
    }

    string g = "";

    void checkDo(string action) {
        Do(action);
    }



    public void cutscene(cutscene_types ct, string file_name = "") {

        string folder_path = "cutscenes\\"; //gets the cutscene folder
        box.clr();

        switch (ct) {

            case cutscene_types.intro:
                
                box.Print(fm.readTxtFileAtLine(folder_path + "intro.txt", 1));
                box.Print(fm.readTxtFileAtLine(folder_path + "intro.txt", 2));
                box.Print(fm.readTxtFileAtLine(folder_path + "intro.txt", 3));
                box.print_screen_del();

            break;

            case cutscene_types.custom_txt:
                System.Console.WriteLine(folder_path + file_name);
                string[] str = fm.readall(folder_path + file_name);
                foreach(string g in str) {
                    box.Print(g);
                }
                box.print_screen_del();
            break;

            case cutscene_types.game_over:
                box.Print(fm.readTxtFileAtLine(folder_path + "game_over.txt", fm.getRan(1,8)));
                box.print_screen_del();
            break;

        }

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

    public enum change_types {
        gain_item,
        lose_item,
        move_to,
        missing_item,
        drop_all,
        change_env,
    }


    public void situ_change(change_types ct, string result) {

        switch (ct) {
            case change_types.gain_item:
                box.nl();
                box.Print("{Cyan}>\t{end}<{Green}+{end}> ({DarkGreen}" + result + "{end})");
                box.nl();
            break;

            case change_types.move_to:
                box.nl();
                box.Print("{Cyan}>\t{end}<{DarkYellow}→{end}> ({DarkCyan}" + result + "{end})");
                box.nl();
            break;

            case change_types.lose_item:
                box.nl();
                box.Print("{Cyan}>\t{end}<{Red}-{end}> ({DarkYellow}" + result + "{end})");
                box.nl();
            break;

            case change_types.missing_item:
                box.nl();
                box.Print("{Cyan}>\t{end}<{Red}X{end}> ({DarkRed}" + result + "{end})");
                box.nl();
            break;

            case change_types.drop_all:
                box.nl();
                box.Print("{Cyan}>\t{end}<{Red}--{end}> ({DarkRed}" + result + "{end})");
                box.nl();
            break;

            case change_types.change_env:
                box.nl();
                box.Print("{Cyan}>\t{end}<{DarkRed}-{Gray}-{DarkRed}>>{end}> ({DarkRed}" + result + "{end})");
                box.nl();
            break;

            default:
                box.nl();
                box.Print("{Cyan}>\t{end}<{Gray}?{end}> ({DarkRed}" + "Internal error, argument missing: 'situ_change'." + "{end})");
                box.nl();
            break;
        }
        
    }



}