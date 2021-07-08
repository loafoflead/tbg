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

    private bool intro = false;
    public bool fast_cutscenes = false;


    public bool show_old_msgs = true;


    public string log_file = "";

    private string cutscene_file = "";

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

        load_config_file("config.txt");
        
        if (intro == true) {
        cutscene(cutscene_types.intro);
        box.waitf(1);
        box.clr_buffer();
        cutscene(cutscene_types.custom_txt, "env01_cutscene");
        box.waitf(1);
        box.clr_buffer();
        box.refresh_box();
        box.clr_text();
        }

        /*if (cm.YN("Do you want to read a log file?") == true) {
            loadsave(System.Console.ReadLine());
        }
        else {*/
            box.Print("[{Red}SIMULATING COMMAND: {end}'{Cyan,White}look{end,end}' {Red}...{end}]");
            box.nl();
            box.Print(env.current_room.desc);
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


    void load_config_file(string filename) {
        string[] lines = fm.readall(filename);
        player.name = lines[0].Split('=')[1];
        player.bio = lines[1].Split('=')[1];

        player.player_tags.Add(lines[4].Split('=')[1]);

        player.is_operator = get_bool(lines[5].Split('=')[1]);

        cutscene_file = lines[6].Split('=')[1];
        intro = get_bool(lines[7].Split('=')[1]);

        env.load_env(lines[2].Split('=')[1]);

        player.inv.add_to_inv(env.get_item_from_tag(lines[3].Split('=')[1]));

    }

    bool get_bool(string bl) {
        if (bl == "true" || bl == "t" || bl == "1") {
            return true;
        }
        return false;
    }

    public enum cutscene_types {
        intro,
        game_over,
        item_pickup,
        custom_txt,
        custom_xml,
        custom_string,
        empty,
    }


    void loadsave(string filename) {

    }

    public void Do(string full_act) {
        Do(full_act.Split(':',2)[0], full_act.Split(':',2)[1]);
    }


    public void Do(string action, string result) {

        if (fm.null_or_empt(result)) {
            box.Print("Internal error, incomplete command requested; ERR_013");
            return;
        }

        if (result.Contains("+") ) {

            string[] a = result.Split("+",2); //the input will be: e.g. 'go' <- action, 'print:hi+if:(inv=headband):(say:bye+give:knife)?(say:hi)'
                                            //next stage: 'if' <- action, '(inv=headband):(say:bye+give:knife)?(say:hi)'
                                            //then: if_true -> say:bye+give:knife
                                            //then: if_true -> say:hi


            if (a[0].Contains("(") || a[0].Contains(")")) {
                goto resume;
            }

            Do(action, a[0]);

            Do(a[1].Split(":",2)[0], a[1].Split(":",2)[1]);
            
            return;

        }

        resume:

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

            case "unlock":
                List<string> buff_temp = box.copy_buffer();
                bool old_op = player.is_operator;
                player.is_operator = true;
                cm.arguments = new string[] {
                    "/unlock", result
                };
                cm.admin_commands();
                box.replace_buffer(buff_temp);
                player.is_operator = old_op;
            break;

            case "emu":
            case "em":
            case "emulate":
                List<string> tem = box.copy_buffer();
                bool ol = player.is_operator;
                player.is_operator = true;
                cm.emulate(result);
                box.replace_buffer(tem);
                player.is_operator = ol;
            break;

            case "end":
                is_running = false;
            break;

            case "name":
                if(!fm.null_or_empt(result)) {
                    player.name = result;
                }
                else {
                    box.Print("Internal error 'Do', name empty ERR_09 (143)");
                }
            break;

            case "bio":
                if(!fm.null_or_empt(result)) {
                    player.bio = result;
                }
                else {
                    box.Print("Internal error 'Do', bio empty ERR_09 (143)");
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

            case "use":
                if (env.all_interactables.Contains(env.get_interactable_tag(result))) {
                    env.UseF(env.get_interactable_tag(result));
                }
                else {
                    box.Print("Internal error 'Do' command, 'use' tag invalid.");
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

            case "op":
                switch (result) {
                    case "true":
                    case "1":
                    case "t":
                        player.is_operator = true;
                    break;

                    case "false":
                    case "f":
                    case "0":
                        player.is_operator = false;
                    break;

                    default:
                        box.PrintD("Internal error 'Do' command, 'op' tag invalid.");
                    break;
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

                    case "fun":

                        if(player.fun == int.Parse(condition)) {

                            checkDo(if_true);

                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "ask":
                        if(cm.YN(condition)) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;
                    
                    case "inv":

                        if(condition == "empty" || condition == "0" || condition == "null") {
                            if (player.inv.player_inventory.Count == 0) {
                                checkDo(if_true);
                            }
                            else {
                                checkDo(if_false);
                            }
                            return;
                        }

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
            g = t;
            return;
        }

        string[] hashes = t.Split("#");


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



    public void cutscene(cutscene_types ct, string filename = "") {

        string file_name = filename;

        string folder_path = "cutscenes\\"; //gets the cutscene folder
        box.clr();

        if (ct == cutscene_types.custom_txt) {
            if (!file_name.Contains(".txt")) {
                file_name += ".txt";
            }
        }

        box.clr();

        switch (ct) {

            case cutscene_types.intro:
                
                box.Print(fm.readTxtFileAtLine(folder_path + cutscene_file, 1));
                box.Print(fm.readTxtFileAtLine(folder_path + cutscene_file, 2));
                box.Print(fm.readTxtFileAtLine(folder_path + cutscene_file, 3));
                if (fast_cutscenes == false) box.print_screen_del();
                else box.print_screen();

            break;

            case cutscene_types.custom_txt:
                System.Console.WriteLine(folder_path + file_name);
                string[] str = fm.readall(folder_path + file_name);
                foreach(string g in str) {
                    box.Print(g);
                }
                if (fast_cutscenes == false) box.print_screen_del();
                else box.print_screen();
            break;

            case cutscene_types.empty:
                break;

            case cutscene_types.game_over:
                box.Print(fm.readTxtFileAtLine(folder_path + "game_over.txt", fm.getRan(1,8)));
                if (fast_cutscenes == false) box.print_screen_del();
                else box.print_screen();
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