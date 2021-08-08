using System.Collections;
using System.Collections.Generic;


public class GManager {

    public Box box;
    public FileManager fm;
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


        try {

            load_config_file("config.txt");

        } catch {

            box.Print("Config file missing or incomplete, please restore it for the game to function correctly.");

            if (cm.YN("Do you want to restore the config file automatically or quit the game and do it yourself? \n{DarkRed}[WARNING]{Gray}: Make sure to write a space before your answer, ' yes'.")) {
                string config_file = fm.newFile("config.txt");
                int i = fm.write_at("name=no_name", 1, "config.txt");
                fm.write_at("bio=no_bio", 2, "config.txt");
                fm.write_at("env=env01", 3, "config.txt");
                fm.write_at("inv=", 4, "config.txt");
                fm.write_at("tags=", 5, "config.txt");
                fm.write_at("op=false", 6, "config.txt");
                fm.write_at("intro=intro.txt", 7, "config.txt");
                fm.write_at("play_intro=true", 8, "config.txt");
                fm.write_at("fast cutscenes=false", 9, "config.txt");
                fm.write_at("debug_text_is_printed=false", 10, "config.txt");
                fm.write_at("to_run_on_startup=null();", 11, "config.txt");
                System.Console.WriteLine(i);
                System.Console.ReadKey();
                load_config_file("config.txt");
            }
            else {
                return;
            }

        }
        box.clr_buffer();

        load_custom_commands("macros.txt");
        
        if (intro == true) {
        cutscene(cutscene_types.intro);
        box.waitf(1);
        box.k.waitAnyKey();
        box.clr_buffer();
        cutscene(cutscene_types.custom_txt, "env01_cutscene");
        box.waitf(1);
        box.k.waitAnyKey();
        box.clr_buffer();
        box.refresh_box();
        box.clr_text();
        }

        ask_for_save();

        while (is_running == true) {
            cm.getInput();
            //box.nl();
            box.print_screen();
        }


        
    }

    public void save_game() {

        fm.clear_file(log_file);

        int exists_question_mark = fm.write_at(player.name,1,log_file);
        if (exists_question_mark == 0) {
            box.Print("{DarkRed}The game failed to save, this could be an error concerning the save file, make sure the file in the 'log' folder. If that isn't the problem make note of the error and contact the developer.");
            box.flush();
            return;
        }
        fm.write_at(player.bio, 2, log_file);
        fm.write_at(env.current_env_name, 3, log_file);

        //write the player's inventory
        string to_write_to_save = "";
        foreach(Item it in player.inv.player_inventory) {
            to_write_to_save += it.environment_owned;
            to_write_to_save += ':';
            to_write_to_save += it.tag;
            to_write_to_save += '/';
        }
        if (to_write_to_save.Length > 2) to_write_to_save = to_write_to_save.Remove(to_write_to_save.Length - 1);
        fm.write_at(to_write_to_save, 4, log_file);

        fm.write_at(cm.get_string(player.player_tags.ToArray(), '/'), 5, log_file);

        fm.write_at(player.is_operator.ToString().ToLower(), 6, log_file);

        fm.write_at(env.current_room.tag, 7, log_file);

        string to_add = "";

        short pl = 0;

        foreach(player_value pv in player.player_Values) {
            if (pl != 0) to_add += '/';
            to_add += pv.name;
            to_add += ':';
            to_add += pv.value;
            pl = 1;
        }

        fm.write_at(to_add, 8, log_file);

        box.Print("{Magenta}Game saved!");

    }

    void ask_for_save() {

        box.Print("SAVE FILES:");
        string[] save_files = System.IO.Directory.GetFiles("logs\\");
        int number = 1;
        foreach(string jk in save_files) {
            box.Print("{Yellow}> {Black,White}" + number.ToString() + ": " + jk.Replace("logs\\", "") + " -> {Red,White}" + fm.readTxtFileAtLine(jk, 1));
            number ++;
        }
        box.flush();

        //if (cm.YN("Do you want to read a log file?") == true) { /* If the player wants to load a save file, */
            file_is_asked:
            box.clr();
            box.Print("Input save file name or number: ");
            box.flush();
            string inp = System.Console.ReadLine();
            if (inp == "q" || inp == "quit") {
                is_running = false;
                return;
            }
            int g = loadsave(inp);
            if (g == 0) { /* if the save file is missing important elements; */

                box.Print("Save file is either corrupted or missing some elements, try repairing it using the template specified in the README file, or create a new one.");
                box.flush();
                is_running = false;
                
            }
            else if (g == 3) { /* If the file doesn't exist or can't be found; */
                try {
                    int save_number = int.Parse(inp);
                    if (save_number > save_files.Length || save_number < 1) {
                        box.Print("Out of range number, files are in range: 1-" + save_files.Length);
                        goto file_is_asked;
                    }
                    else {
                        
                        if (fm.null_or_empt(fm.readTxtFileAtLine(save_files[save_number - 1], 1))) {

                            log_file = save_files[save_number - 1];
                            save_game();
                            loadsave(save_files[save_number - 1].Replace("logs\\", ""));
                            box.clr_buffer();
                            box.Print("Save file successfully loaded!");
                            box.flush();
                            box.waitf(0.5f);
                            box.clr_buffer();
                            box.Print(env.current_room.desc);
                            box.print_screen();
                        }
                        else {
                            loadsave(save_files[save_number - 1].Replace("logs\\", ""));
                            box.clr_buffer();
                            box.Print("Save file successfully loaded!");
                            box.flush();
                            box.waitf(0.5f);
                            box.clr_buffer();
                            box.Print(env.current_room.desc);
                            box.print_screen();
                        }

                    }

                } catch {   
                    box.Print("Save file not found: " + inp + ", try inputting the name again.");
                    box.flush();
                    goto file_is_asked;
                }
                
            }
            else { /* If the save can be loaded; */
                box.clr_buffer();
                box.Print("Save file successfully loaded!");
                box.flush();
                box.waitf(0.5f);
                box.clr_buffer();
                box.Print(env.current_room.desc);
                box.print_screen();
            }
        

    }

    public void load_custom_commands(string fl = "macros.txt") {

        string[] lines = fm.readall(fl);

        List<string> adm_com = new List<string>();
        List<string> reg_com = new List<string>();

        foreach(string gh in lines) {
            if(gh[0] == '/') {
                adm_com.Add(gh);
            }
            else {
                reg_com.Add(gh);
            }
        }

        cm.custom_admin_commands = adm_com.ToArray();
        cm.custom_commands = reg_com.ToArray();

    }


    void load_config_file(string filename) {
        string[] lines = fm.readall(filename);
        player.name = lines[0].Split('=')[1];
        player.bio = lines[1].Split('=')[1];

        player.player_tags.Add(lines[4].Split('=')[1]);

        player.is_operator = get_bool(lines[5].Split('=')[1]);

        cutscene_file = lines[6].Split('=')[1];
        fast_cutscenes = get_bool(lines[8].Split('=')[1]);
        intro = get_bool(lines[7].Split('=')[1]);

        try {
            env.load_env(lines[2].Split('=')[1]);
        }
        catch {
            box.Print("{Red}**WARNING**", Box.format_options.middle);
            box.Print("{Grey}It seems that the environment specified in the config file is causing issues and the game is unable to load it. Check the file name 'config.txt'", Box.format_options.middle);
            box.Print("{Grey}to attempt to rectify the issue.", Box.format_options.middle);
        }

        box.debug_print = get_bool(lines[9].Split('=')[1]);

        if(!fm.null_or_empt(lines[10]) && lines.Length > 10) {
            try {Do(lines[10].Split('=',2)[1]);}
            catch {box.Print("Non-fatal error loading the config file, startup action failed to load.");}
        }

        if(!fm.null_or_empt(lines[3].Split('=')[1])) player.inv.add_to_inv(env.get_item_from_tag(lines[3].Split('=')[1]));

    }

    bool get_bool(string bl) {
        if (bl == "True" || bl == "true" || bl == "t" || bl == "1") {
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


    public int loadsave(string filename) {

        player.inv.player_inventory = new List<Item>();
        player.player_Values = new List<player_value>();
        player.player_tags = new List<string>();

        if (!filename.Contains(".txt")) {
            return loadsave(filename + ".txt");
        }

        if (!System.IO.File.Exists("logs\\" + filename)) {
            return 3;
        }

        string[] save_file = fm.readall("logs\\" + filename);
        
        if (save_file.Length < 1) {
            return 0;
        }

        player.name = save_file[0];
        player.bio = save_file[1];

        try {
            env.load_env(save_file[2]);
        } catch {
            return 0;
        }

        if (save_file[3].Contains('/')) {

            foreach(string hi in save_file[3].Split('/')) {

                List<Item> temporary_items = env.load_items_from_env(hi.Split(':')[0]);
                player.inv.add_to_inv(temporary_items.Find(Item => Item.tag == hi.Split(':')[1]));

            }

        }
        else {


            if (!fm.null_or_empt(save_file[3])) {
                List<Item> temporary_items = env.load_items_from_env(save_file[3].Split(':')[0]);
                player.inv.add_to_inv(temporary_items.Find(Item => Item.tag == save_file[3].Split(':')[1]));
            }


        }

        if (!fm.null_or_empt(save_file[4])) {
            if (save_file[4].Contains('/')) {
                foreach(string h in save_file[4].Split('/')) {
                    player.player_tags.Add(h);
                }
            }
            else {
                player.player_tags.Add(save_file[4]);
            }
        }

        player.is_operator = get_bool(save_file[5]);
        env.current_room = env.rooms.Find(room_short => room_short.tag == save_file[6]);

        log_file = "logs\\" + filename;

        if(fm.null_or_empt(save_file[7])) {
            return 1;
        }

        if (save_file[7].Contains('/')) {
            foreach(string vals in save_file[7].Split('/')) {
                player.add_value(vals.Split(':')[0], vals.Split(':')[1]);
            }
        } 
        else {
            player.add_value(save_file[7].Split(':')[0], save_file[7].Split(':')[1]);
        }

        

        return 1;

    }

    public void Do(string full_act) {
        if (!full_act.Contains('(') || !full_act.Contains(')') || !full_act.Contains(';')) {
            box.Print("Internal error, incomplete command requested; " + full_act + ", ERR_013");
            return;
        }
        Do(full_act.Split('(',2)[0].Replace(" ", ""), '(' + full_act.Split('(',2)[1]);
        
        if (rest_maybe != "" || !fm.is_spaces(rest_maybe)) {
            Do(rest_maybe.Split('(',2)[0].Replace(" ", ""), '(' + rest_maybe.Split('(',2)[1]);
            rest_maybe = "";
        }
        
    }

    List<string> buffer_copy = new List<string>();

    public int count_char(string str, char character) {
        int to_return = 0;
        foreach(char j in str) {
            if (j == character) {
                to_return ++;
            }
        }
        return to_return;
    }

    string rest_maybe = "";

    private static string[] null_action_commands = new string[] {
        "null", "nl", "wait", "wt", "flush", "flsh", "clr", "clear"
    };

    private static string[] exceptional_commands = new string[] {
        "creat_subroutine",
        "create_sub", "createsub",
        "subroutine",
        "routine",
        "new_sub",
        "newsub",
        "if",
    };

    public void Do(string action, string resultt) {

        

        string result = resultt;
        string to_run_at_end = "";
        int num_of_lines = 0;
        box.PrintD(result + "," + action);

        if (action.Replace(" ", "") != "if") {
            foreach(string g in exceptional_commands) {
                if (action.Replace(" ", "") == g) {
                    goto resume;
                }
            }
            num_of_lines = count_char(resultt, ';');

            if (num_of_lines > 1) to_run_at_end = resultt.Split(';',2)[1];
            else to_run_at_end = null;

            /*if (resultt.Contains("if")) { // print(hi); if(tag=name):(say(hi);)?(say(bye);)
                                                //                                           ^ this is where it gets cut
                result = resultt.Split(';',2)[1];

                result = result.Replace("(", "").Replace(")", "");

                goto resume;

            }*/

            box.PrintD("num of lines: " + num_of_lines.ToString() + ", to run: " + to_run_at_end);
            box.PrintD("running: {Red}" + resultt.Split(';',2)[0]);

            result = resultt.Split(';',2)[0];

            result = result.Replace("(", "").Replace(")", "");
        }

        resume:

        /*
            SYNTAX:

                <action>print(Hi!);</action>

                <action>
                    if(tag=hurt_knee):
                        (
                            print(bye);
                            give(money);
                        )
                    ?
                        (
                            print(no money!!);
                        )
                </action>

        */

        if (fm.null_or_empt(result)) {
            foreach(string g in null_action_commands) {
                if (g == action) {
                    goto resume_null_action;
                }
            }
            box.Print("Internal error, incomplete command requested; ERR_013");
            return;
        }

        resume_null_action:

        //box.Print("action: " + action + ", result: \n" + result.Replace(":", "{White}:{DarkYellow}").Replace("(", "{White}({Cyan}").Replace(")", "{White})").Replace(" ", "").Replace("+", "{Red}+\n{White}").Replace("?", "{Blue}\n?\n{White}").Replace("if:", "{Red}if{White}:\n"));
        /*box.clr_buffer();
        box.clr();
        box.flush();
        box.Print("action: " + action);
        string print = "";
        char previous_character = ' ';
        foreach(char ch in result) {
            switch(ch) {
                case '?':
                    print += "\n{Blue}?\n";
                    continue;

                case ':':
                    print += "{White}:{DarkYellow}";
                    continue;

                case '+':
                    print += "{Red}+\n{White}";
                    continue;
                
                case '(':
                    print += "{White}({Cyan}";
                    continue;

                case ')':
                    print += "{White})";
                    continue;

                case ' ':
                    if (previous_character == ' ') {
                        continue;
                    }
                    else {
                        print += ch.ToString();
                    }
                break;

                default:
                    print += ch.ToString();
                break;
            }
            previous_character = ch;
        }
        box.Print(print);
        box.flush();
        box.k.waitAnyKey();
        */

        switch (action.Replace(" ", "")) {
            case "give":
                
                Item it = env.get_item_from_tag(result.Replace(" ", ""));
                int res = 0;
                if (it.tag != null) res = player.inv.add_to_inv(it);
                else {
                    box.Print("{DarkRed}Item not found: '" + result.Replace(" ", "") + "'");
                    break;
                }
                if (res == 2) {
                    box.Print("Inventory full!");
                }
                else {
                    situ_change(change_types.gain_item, env.get_item_from_tag(result.Replace(" ", "")).name);
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

            case "replace":
            case "replace_item":
            case "replaceitem":
                if (!result.Contains(':')) {
                    break;
                }
                Item temp_to_change = player.inv.player_inventory.Find(Item => Item.tag == result.Split(':')[0]);
                if (temp_to_change == null) break;
                else {
                    string temp_name = temp_to_change.name;
                    player.inv.player_inventory.Remove(temp_to_change);
                    player.inv.player_inventory.Add(env.get_item_from_tag(result.Split(':')[1]));
                    situ_change(change_types.item_shift, temp_name + "/" + temp_to_change.name);
                }
            break;

            case "gosub":
            case "go_sub":
            case "go_subroutine":
            case "run_subroutine":
            case "call":
            case "run":
            case "runsub":
            case "run_sub":
            case "execute":
            case "exec":
                foreach(subroutine sbt in env.subroutines) {
                    if (sbt.name == result) {
                        try {
                            Do(sbt.value);
                        } catch {
                            box.Print("Internal error, 'call_subroutine' function.");
                        }
                    }
                }
            break;

            case "creat_subroutine":
            case "create_sub":
            case "createsub":
            case "subroutine":
            case "routine":
            case "new_sub":
            case "newsub": //e.x. new_sub{say_hi=say(hello);}

                if (!result.Contains('=')) break;

                box.Print("{Green}" + result);

                string subroutine = result.Split('(',2)[1];

                subroutine = subroutine.Split('=',2)[1];

                string subroutine_name = result.Split('(',2)[1].Split('=',2)[0];

                for(int i = 0; i < subroutine.Length; i ++) {

                    if (subroutine[i] == '(') {
                        while(subroutine[i] != ')') {
                            i ++;
                        }
                        i ++;
                    }

                    if (subroutine[i] == ')') {
                        to_run_at_end = split_at(subroutine, i + 2)[1];
                        if (!fm.null_or_empt(to_run_at_end)) {
                            num_of_lines = 2;
                        }
                        else num_of_lines = 1;
                        subroutine = split_at(subroutine, i)[0];
                    }


                }

                env.subroutines.Add(new subroutine{
                    name = subroutine_name,
                    value = subroutine,
                });
                
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

            case "wait":
            case "wt":
                try {
                    box.waitf(int.Parse(result.Replace(" ", "")));
                }
                catch {
                    box.waitf(1f);
                }
            break;

            case "name":
                if(!fm.null_or_empt(result)) {
                    player.name = result;
                }
                else {
                    box.Print("Internal error 'Do', name empty ERR_09 (143)");
                }
            break;

            case "flush":
            case "flsh":
            case "fl":
                box.flush();
            break;

            case "cpy":
            case "copy":
            case "copy_buffer":
                buffer_copy = box.copy_buffer();
            break;

            case "pst":
            case "paste":
            case "paste_buffer":
                box.replace_buffer(buffer_copy);
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
                int b = env.Go(result.Replace(" ", ""));
                System.Console.WriteLine(b.ToString());
                if (b == 0) {
                    box.Print("Internal error 'Go', location not found ERR_09: '" + result + "'");
                }
                else {
                    situ_change(change_types.move_to, env.get_room_name_by_tag(result.Replace(" ", "")));
                }
            break;

            case "setroom":
            case "room":
            case "st_rm":
            case "set_room":
                room_short temp = env.get_room_short_by_tag(result.Replace(" ", ""));
                if (temp == null) {
                    box.Print("Internal error 'Set Room', location not found ERR_??: '" + result + "'");
                }
                else {
                    env.current_room = temp;
                }
            break;

            case "add_val":
            case "add_value":
            case "addvalue":
            case "value":
            case "new_val":
            case "newval":
            case "give_value":
            case "give_val":
                player.add_value(result.Split(':')[0], result.Split(':')[1].Replace(" ", ""));
            break;

            case "remove_value":
                player.remove_value_by_tag(result.Replace(" ", ""));
            break;

            case "change_value":
            case "change_val":
            case "edit_val":
            case "editvalue":
                player_value temp_val = player.get_value(result.Split(';')[0]);
                temp_val.value = result.Split(':')[1].Replace(" ", "");
            break;

            case "addtag":
            case "givetag":
            case "gt":
            case "givet":
                player.player_tags.Add(result);
            break;

            case "taketag":
            case "rt":
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
                switch(result.Replace(" ", "")) {
                    case "all":
                        player.inv.reset_inv();
                        situ_change(change_types.drop_all, "");
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
                                goto skip;
                            }
                        }
                        box.Print("Internal error 'Do' command, 'take' tag invalid.");
                        skip:
                    break;
                }
            break;

            case "cutscene":
                switch(result.Replace(" ", "")) {

                    case "intro":
                        cutscene(cutscene_types.intro);
                    break;
                    case "game_over":
                    case "gameover":
                        cutscene(cutscene_types.game_over);
                    break;
                    default:
                        cutscene(cutscene_types.custom_txt, result.Replace(" ", ""));
                    break;

                }
            break;

            case "use":
                if (env.all_interactables.Contains(env.get_interactable_tag(result.Replace(" ", "")))) {
                    env.UseF(env.get_interactable_tag(result.Replace(" ", "")));
                }
                else {
                    box.Print("Internal error 'Do' command, 'use' tag invalid.");
                }
            break;

            case "env":
                try {
                    env.load_env(result.Replace(" ", ""));
                    situ_change(change_types.change_env, result.Replace(" ", ""));
                } catch {
                    box.Print("Invalid environment file was requested to be loaded, or is missing key files in order to be loaded.");
                }
            break;

            case "op":
                switch (result.Replace(" ", "")) {
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

            case "clr":
            case "cls":
            case "clear":
                box.clr_buffer();
                box.clr();
            break;

            case "print":
                g = "";
                check_tag(result);
                box.Print(g);
            break;

            case "say":
                g = "";
                check_tag(result);
                box.Print("{Cyan}- \"" + g + "{Cyan}\"");
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
                box.PrintD("Compare tag: {Yellow}" + compare_tag);

                string condition = new_res.Split("=")[1].Split(")",2)[0].Replace(" ", ""); // the string that denotes the condition to be met by the compare tag
                box.PrintD("Condition: {Yellow}" + condition);

                string without_if_and_condition = new_res.Split(')',2)[1]; //just the results of the action
                box.PrintD("command without condition: {Yellow}" + without_if_and_condition);
                
                string if_true =  without_if_and_condition.Split('[',2)[1];

                int i = 0;

                for(i = 0; i < if_true.Length; i ++) {

                    if (if_true[i] == '[') {
                        while(if_true[i] != ']') {
                            i ++;
                        }
                        i ++;
                    }

                    if (if_true[i] == ']') {
                        if_true = split_at(if_true, i)[0];
                    }

                }

                box.PrintD("execute if true: {Yellow}" + if_true);
                

                string minue_true = without_if_and_condition.Replace("[" + if_true + "]", "");
                minue_true = minue_true.Split('?',2)[1];
                box.PrintD("command without true: {Yellow}" + minue_true + ", true length: " + if_true.Length.ToString());

                string if_false = minue_true.Split('[',2)[1];

                for(i = 0; i < if_false.Length; i ++) {

                    if (if_false[i] == '[') {
                        while(if_false[i] != ']') {
                            i ++;
                        }
                        i ++;
                    }

                    if (if_false[i] == ']') {
                        try {
                            if (!fm.is_spaces(split_at(if_false, i + 1)[1])) rest_maybe = split_at(if_false, i + 1)[1].Replace("\t", "").Replace("\n", "");
                        } catch {}
                        if_false = split_at(if_false, i)[0];
                    }

                }
                
                box.PrintD("execute if false: {Yellow}" + if_false);

                if (rest_maybe != "" || fm.is_spaces(rest_maybe)) {
                    box.PrintD("rest of command: {Yellow}" + rest_maybe);
                }

                if (compare_tag.Contains('!')) {
                    string temp_true = if_true;
                    if_true = if_false;
                    if_false = temp_true;
                    compare_tag.Replace("!", "");
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

                    case "rand5050":
                    case "5050":
                    case "1|2":
                    case "rand_5050":
                    case "random5050":
                    case "random_5050":
                    case "random_one_in_two":
                        if (fm.getRan(1,101) < 50) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "rand2080":
                    case "1|4":
                    case "rand14":
                    case "random_one_in_four":
                        if (fm.getRan(1,101) < 50) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "rand100":
                    case "random100":
                    case "rand":
                    case "100":
                        if (fm.getRan(1,101) < int.Parse(condition)) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "value":
                    case "val":
                    case "value_is":
                        if (player.get_value(condition.Split(':')[0]).value == condition.Split(':')[1]) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "value_isnt":
                    case "!value":
                    case "!val":
                        if (player.get_value(condition.Split(':')[0]).value == condition.Split(':')[1]) {
                            checkDo(if_false);
                        }
                        else {
                            checkDo(if_true);
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
                    case "inventory":
                    case "inv_contains":

                        if(condition == "empty" || condition == "0" || condition == "null") {
                            if (player.inv.player_inventory.Count == 0) {
                                checkDo(if_true);
                            }
                            else {
                                checkDo(if_false);
                            }
                            break;
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
                    case "current_room":
                        if(env.current_room.tag == condition) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    case "op":
                    case "operator":
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
                    case "tags":
                    case "t":
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

                    case "YN":
                    case "yes_or_no":
                    case "yes_no":
                    case "yn":
                        if(cm.YN(condition) == true) {
                            checkDo(if_true);
                        }
                        else {
                            checkDo(if_false);
                        }
                    break;

                    default:
                        foreach(player_value pv in player.player_Values) {/* foreach player value */
                            if (pv.name == compare_tag) { 
                                if (pv.value == condition) {
                                    checkDo(if_true);
                                }
                                else {
                                    checkDo(if_false);
                                }
                                goto break_label;
                            }
                        }
                        box.Print("Internal error, invalid comparison tag requested; " + compare_tag + "=" + condition);
                        break_label:
                    break;

                }
            } catch {
                box.Print("Internal error in xml level folder using interactable, command was: {Blue}" + action + "{White}:{Red}" + result);
            }
            break;

            case "store":
                if (!result.Contains(':')) {
                    box.Print("Syntax error in 'store' command: " + result);
                    break;
                }
                switch(result.Split(':')[0]) {

                    case "room":
                        player.add_value(result.Split(':')[1], env.get_room_name_by_tag(env.current_room.tag));
                    break;

                    case "fun":
                        player.add_value(result.Split(':')[1], player.fun.ToString());
                    break;

                    case "name":
                        player.add_value(result.Split(':')[1], player.name);
                    break;

                    case "bio":
                        player.add_value(result.Split(':')[1], player.bio);
                    break;

                    case "environment":
                        player.add_value(result.Split(':')[1], env.current_env_name.Replace("_", " "));
                    break;

                    case "items":
                    case "num_of_items":
                    case "numofitems":
                        player.add_value(result.Split(':')[1], player.inv.player_inventory.Count.ToString());
                    break;

                    case "num_of_values":
                    case "numofvalues":
                    case "values":
                        player.add_value(result.Split(':')[1], player.player_Values.Count.ToString());
                    break;

                    case "num_of_tags":
                    case "numoftags":
                    case "tags":
                    case "tag_count":
                        player.add_value(result.Split(':')[1], player.player_tags.Count.ToString());
                    break;

                    default:
                        foreach(player_value pv in player.player_Values) {
                            if (pv.name == result.Split(':')[0]) {
                                player.add_value(result.Split(':')[1], player.get_value(result.Split(':')[0]).value);
                                goto break_label_store;
                            }
                        }
                        box.Print("'Do()' error with 'store' command, invalid tag to store the data of requested.");
                        break_label_store:
                    break;

                }
            break;

            
            default:
                box.Print("Fatal 'Do' Internal Error occurred ERR_07. {DarkRed}" + action + "//{Red}" + result);
            break;
        }

        if (num_of_lines > 1) Do(to_run_at_end);

        
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
                        g += env.current_room.name;
                    break;
                    case "fun":
                        g += player.fun;
                    break;
                    case "room_desc":
                    case "desc":
                        g += env.current_room.desc;
                    break;
                    default:
                        foreach(player_value pv in player.player_Values) {
                            if (pv.name == hashes[i]) {
                                g += pv.value;
                                goto break_label_store;
                            }
                        }
                        g += "invalid_tag_error";
                        break_label_store:
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

        //kh.startListener();

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

        //kh.stopListener();

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
        item_shift,
    }


    public void situ_change(change_types ct, string result) {

        switch (ct) {
            case change_types.gain_item:

                box.Print("{White}<{Green}Gained Item{end} {DarkYellow}-->{end} {Gray}" + result + "{end}>");
                
            break;

            case change_types.move_to:

                box.Print("{end}<{DarkYellow}Moved to{end} {DarkYellow}-->{end} {Gray}" + result + "{end}>");
                
            break;

            case change_types.lose_item:

                box.Print("{end}<{Red}Item Dropped{end} {DarkYellow}-->{end} {Gray}" + result + "{end}>");
                
            break;

            case change_types.missing_item:

                box.Print("{end}<{DarkRed}Item Required{end} {DarkYellow}-->{end} {Gray}" + result + "{end}>");
                
            break;     

            case change_types.drop_all:

                box.Print("{end}<{DarkRed}Inventory Dropped{end}>");
                
            break;

            case change_types.change_env:

                box.Print("{end}<{DarkGray}Moved to {DarkYellow}Environment{end} {DarkYellow}-->{end}  {DarkGray}" + result + "{end}>");
                
            break;

            case change_types.item_shift:

                box.Print("{White}<{DarkYellow}Item {White}<{DarkGray}" + result.Split('/')[0] + "{White}> {DarkYellow}replaced by --> {DarkGray}" + result.Split('/')[1] + "{White}>");

            break;

            default:

                box.Print("{end}<{Gray}?{end} {DarkRed}" + "Internal error, argument missing: 'situ_change'." + "{end}>");
                
            break;
        }
        
    }



}