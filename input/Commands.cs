using System;

public class Commands {

    GManager gm;

    public string[] arguments;

    public Commands(GManager g) {
        gm = g;
        arguments = new string[10];
    }

    public string[] custom_commands;
    public string[] custom_admin_commands;

    public string h = "";
    public bool succeeded = false;
    public void getInput() {
        gm.box.k.stopListener();

        succeeded = true; //set the printing out of the most recent to true;

        h = Console.ReadLine(); //read a line from the console as input

        arguments = new string[1]; //reset the arguments array

        if (h.Contains(" ")) arguments = h.Split(" "); //if the command is longer than one word split it into spaces
        else arguments[0] = h; // if not set the first item to the arguments string

        if (arguments[0].Length > 1) {  // if the first word is longer than one character check if it's an admin command or a regular one

            if(arguments[0][0] == '/') { //if the first character is a / check for admin commands
                admin_commands();
            }
            else { //if not check for a regular command
                regular_commands();
            }

        }
        else { //idk why i added this but im scared to remove it

            if(arguments[0] == "/") {
                admin_commands();
            }
            else {
                regular_commands();
            }
            
        }

        //whahahahaa what was i gonna do cummy wummy hole man >:))))))))]]]]]]]]]] help

        if (succeeded == true && gm.show_old_msgs == true) { /* If the command was found, print it to the old command reel */

            gm.box.PrintLn(" ", 100); //empty the line
            if (get_string(arguments).Remove(get_string(arguments).Length - 1, 1) == "quit"){
                arguments[0] = "{DarkRed}quit{DarkGray}";
            }
            if (get_string(arguments).Remove(get_string(arguments).Length - 1, 1) == "q"){
                arguments[0] = "{DarkRed}q{DarkGray}";
            }
            if (get_string(arguments).Remove(get_string(arguments).Length - 1, 1) == "_<"){
                arguments[0] = "{Yellow}>_<{DarkGray}";
            }
            gm.box.PrintLn(
                "{Gray}" + get_string(arguments).Remove(get_string(arguments).Length - 1, 1) /* Add the most recent command in Grey, */ + 
                "{DarkGray}, " + get_back_str(previous_msgs.ToArray()) /* then the previous ones in DarkGray */, 100
            );
            previous_msgs.Add(get_string(arguments).Remove(get_string(arguments).Length - 1, 1) + ", "); /* add the most recent message to the previous message list */

        }

        //gm.box.k.startListener();

    }

    public System.Collections.Generic.List<string> previous_msgs = new System.Collections.Generic.List<string>(); /* list of previous commands */


    /*
        get_string 
        returns: a string composed of the elements of an array
    */
    public string get_string(string[] str, char seperator_character = ' ') {
        string to_ret = "";
        foreach(string h in str) {
            if (gm.fm.null_or_empt(h)) to_ret += "";
            else to_ret += h + seperator_character.ToString();
        }
        return to_ret;
    }

    string get_back_str(string[] str) {
        string to_return = "";
        for (int i = str.Length - 1; i > -1; i --) {
            if (gm.fm.null_or_empt(str[i])) to_return += "";
            else to_return += str[i] + " ";
        }
        return to_return;
    }

    public void emulate(string command) {
        arguments = new string[10];
        if (command.Contains(" ")) arguments = command.Split(" ");
        else arguments[0] = command;
        

        if (arguments[0].Length > 1) { 
            if(arguments[0].Contains("/")) {
                admin_commands();
            }
            else {
                regular_commands();
            }
        }
        else {
            if(arguments[0].Contains("/")) {
                admin_commands();
            }
            else {
                regular_commands();
            }
            
            //gm.box.Print("{Gray}>\tUnkown Command!\t'" + h + "'");
        }
    }

    public void regular_commands() {

        switch (arguments[0]) {

            case "back":
                gm.env.loadRoom_ind(gm.env.previous_room);
                gm.situ_change(GManager.change_types.move_to, gm.env.current_room.name);
            break;

            case "clear":
            case "clr":
            case "cls":
                gm.box.clr_buffer();
                previous_msgs = new System.Collections.Generic.List<string>();
            break;

            case "save":
            case "s":
                gm.save_game();
            break;

            case "take":
            case "get":
            case "pickup":
                try {
                    foreach(string it in gm.env.current_room.room_item_tags) {
                        if (gm.env.get_item_from_tag(it).aliases.Contains(arguments[1])) {
                            if(gm.player.inv.add_to_inv(gm.env.get_item_from_tag(it)) == 1) {
                                gm.situ_change(GManager.change_types.gain_item, gm.env.get_item_from_tag(it).name);
                                return;
                            }
                            else {
                                gm.box.Print("Item not found!");
                                return;
                            }
                        }
                    }
                    gm.box.Print("Item not found!");
                } catch {
                    gm.box.Print("Item not found!");
                }
            break;

            case "drop":
            case "dr":

                if (arguments[1] != null) {
                    if (arguments[1] == "all") {
                        gm.player.inv.reset_inv();
                        gm.situ_change(GManager.change_types.drop_all, "{Red}all{end}");
                        return;
                    }
                }

                try {

                    foreach(Item it in gm.player.inv.player_inventory) {
                        if (it.aliases.Contains(arguments[1])) {
                            gm.situ_change(GManager.change_types.lose_item, it.name);
                            gm.player.inv.remov(it);
                            return;
                        }
                    }

                    gm.box.Print("Item not found: " + arguments[1]);

                }catch {
                    gm.box.Print("Unknown item!");
                }
            break;

            case "list":
                try {

                    switch (arguments[1]) {

                        case "items":
                        case "inv":
                        case "inventory":
                        case "item":
                        case "i":
                            int index = 1;
                            foreach(Item it in gm.player.inv.player_inventory) {
                                gm.box.Print(index + ": " + it.name);
                                index ++;
                            }
                            if (gm.player.inv.player_inventory.Count < 1) {
                                gm.box.Print("Inventory empty!");
                                return;
                            }
                        break;

                        default:
                            gm.box.Print("Unknown list element: " + arguments[1]);
                        break;

                    }

                } catch {
                    gm.box.Print("Incorrect syntax, usage is '{Cyan}list 'items'{end}'.");
                }
            break;

            case "go":
            case "move":
                try {
                    switch (arguments[1]) {

                    case "l":
                    case "left":
                        int h = gm.env.Move(Environment.direction_enum.left);
                        if (h == 0) {
                            gm.box.Print("There isn't a passage there!");
                        }
                        break;

                    case "r":
                    case "right":
                        if (gm.env.Move(Environment.direction_enum.right) == 0) {
                            gm.box.Print("There isn't a passage there!");
                        }
                        break;

                    case "f":
                    case "forward":
                    case "forwards":
                        if (gm.env.Move(Environment.direction_enum.forwards) == 0) {
                            gm.box.Print("There isn't a passage there!");
                        }
                        break;

                    case "b":
                    case "back":
                    case "backward":
                    case "backwards":
                        if (gm.env.Move(Environment.direction_enum.backwards) == 0) {
                            gm.box.Print("There isn't a passage there!");
                        }
                        break;

                    default:
                        gm.box.Print("Unknown direction, or too many arguments.");
                        break;
                }
                } catch {
                    gm.box.Print("Incorrect syntax, usage is: 'go [direction]', where [direction] can be: left, right, forwards, backwards, etc. They can be abbreviatted to f, l, r, b.");
                }
                break;

            case "inspect":
            case "insp":

                Item prt = gm.env.get_item_from_alias(arguments[1]);
                if(!gm.player.inv.player_inventory.Contains(prt)) {
                    gm.box.Print("Item not found.");
                }
                if (prt == null) {
                    gm.box.Print("Incorrect usage of '{Cyan}examine{end}', usage: 'examine [item]', where item is an item in your inventory. You can type 'list' to list these items.");
                }else {
                    gm.box.Print(prt.description);
                }
                

            break;

            case "explore":
            case "check":
            case "look":
                gm.box.Print(gm.env.current_room.desc);
            break;

            case "where":
            case "room":
                gm.box.Print(gm.env.current_room.name);
            break;

            case "q":
            case "quit":
                gm.is_running = false;
                if (arguments.Length > 1) {
                    if (arguments[1] == "s" || arguments[1] == "save") {
                        gm.save_game();
                    }
                    else if (arguments[1] == "f" || arguments[1] == "unsaved") {
                        return;
                    }
                    else {
                        return;
                    }
                }
                else {
                    gm.save_game();
                }
                break;

            default:

                foreach(string comma in custom_commands) {
                    if (arguments[0] == comma.Split('=')[0]) {
                        gm.Do(comma.Split('=')[1]);
                        return;
                    } 
                }

                try {

                    foreach(string obj_tag in gm.env.current_room.room_interactable_tags) { //runs through each obj in current room
                        Interactable temp_obj = gm.env.get_interactable_tag(obj_tag); //gets the object

                        if(temp_obj.verbs.Contains(arguments[0])) {
                            if (temp_obj.aliases.Contains(arguments[1])) {
                                gm.env.UseVerb(temp_obj, arguments[0]);
                                return;
                            }
                        }

                        if(temp_obj.verbs.Contains(arguments[0] +  " " + arguments[1])) {
                            if (temp_obj.aliases.Contains(arguments[2])) {
                                gm.env.UseVerb(temp_obj, arguments[0] +  " " + arguments[1]);
                                return;
                            }
                        }

                        if(temp_obj.verbs.Contains(arguments[0] +  " " + arguments[1])) {
                            if (temp_obj.aliases.Contains(arguments[2] +  " " + arguments[3])) {
                                gm.env.UseVerb(temp_obj, arguments[0] +  " " + arguments[1]);
                                return;
                            }
                        }

                        if(temp_obj.verbs.Contains(arguments[0])) {
                            if (temp_obj.aliases.Contains(arguments[1] + " " + arguments[2])) {
                                gm.env.UseVerb(temp_obj, arguments[0]);
                                return;
                            }
                        }

                        
        
                    }

                    succeeded = false;
                    gm.box.Print("{DarkGray}<{Yellow}@{DarkGray}> Unknown Command!'" + get_string(arguments) + "'");
                } catch {
                    gm.box.Print("Unexpected error searching for interactable.");
                }

                //do thing to sift through every interctable to check if any of the args correspond to the verb or the object k cool bye
                //thanks!
            break;

            case "_<":
                
                if (gm.player.is_operator == true) {
                    gm.box.Print(">{Gray}Cheats {Red}disabled{end}, '{DarkCyan}/{Gray}' commands will no longer have any effect.");
                    gm.player.is_operator = false;
                } else {
                    gm.box.Print(">{Gray}Cheats {Red}activated{end}, use '{DarkCyan}/{Gray}' to begin a command.");
                    gm.player.is_operator = true;
                }
                
            break;

        }
    }

    void unlock_menu() {
        
    }


    public void admin_commands() {

        if(gm.player.is_operator == false) {
            gm.box.Print("{Red}>[WARNING]: You do not have permission to execute {DarkRed}operator{Red} commands.");
            return;
        }

        arguments[0] = arguments[0].Replace("/", "");

        switch (arguments[0]) {

            case "get":
            case "obtain":
            case "give":
                if (arguments.Length > 1) {
                    gm.Do("give", arguments[1]);
                }
                else {
                    gm.box.Print("Incorrect syntax, '{Cyan}give [item_tag]{end}'. type '{Cyan}/list items all{end}' for the items in the room.");
                }
            break;

            case "reload":
                if (arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax, usage is '{Cyan}/reload [env/macros]'.");
                    return;
                }
                switch(arguments[1]) {
                    case "env":
                        gm.env.load_env(gm.env.current_env_name);
                        gm.box.PrintD("Environment reloaded.");
                    break;
                    case "macros":
                        gm.load_custom_commands();
                        gm.box.PrintD("Macros reloaded.");
                    break;
                }
            break;

            case "goto":
            case "g":
            case "go":
                if (arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax, usage is '{Cyan}/go [room tag]{end}'.");
                    return;
                }
                int g = gm.env.Go(arguments[1]);
                if (g == 1) {
                    gm.situ_change(GManager.change_types.move_to, gm.env.current_room.name);
                }
                else {
                    gm.box.Print("{Gray}Location not found, ({Red}" + arguments[1] + "{end})");
                }
            break;

            case "log":
                gm.box.Print("Log file name: {Gray}" + gm.log_file);
            break;
            case "loadsave":
                if (arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax, usage is '{Cyan}/loadsave [save file name].");
                    return;
                }
                
                int y = gm.loadsave(arguments[1]);
                if (y == 1) {
                    gm.box.Print("Game successfully loaded!");
                }
                else if (y == 3) {
                    gm.box.Print("Save file not found!");
                }
                else {
                    gm.box.Print("Save file corrupt!");
                }
            break;
            case "listsaves":
                string[] save_files = System.IO.Directory.GetFiles("logs\\");
                int number = 1;
                foreach(string jk in save_files) {
                    gm.box.Print("{Magenta}> {Black,Magenta}" + number.ToString() + ": " + jk.Replace("logs\\", "") + " -> {Red,Magenta}" + gm.fm.readTxtFileAtLine(jk, 1));
                    number ++;
                }
            break;

            case "border":
                if (arguments[1] == null || arguments[1].Length > 1){
                    gm.box.Print("Incorrect syntax, usage is '{Cyan}border [single character]'.");
                    return;
                }
                else {

                    gm.box.seperator_character = char.Parse(arguments[1]);
                    return;

                } 

            case "disp":
            case "display":
                if (arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax, usage is: '{Cyan}display [msg/display element]{end}'");
                    return;
                }

                switch (arguments[1]) {

                    case "show_previous_message":
                    case "prev_msg":
                    case "msg":
                        if (gm.show_old_msgs == true) {
                            gm.show_old_msgs = false;
                        }
                        else {
                            gm.show_old_msgs = true;
                        }
                    break;

                    default:
                        gm.box.Print("Unknown display element.");
                    break;

                }

            break;

            case "colour":
            case "color":
            case "col":
                if (arguments[1] == null || arguments[2] == null) {
                    gm.box.Print("Incorrect syntax, usage is: '{Cyan}colour [foreground/background/border] [consolecolour]'");
                    return;
                }
                switch (arguments[1]) {

                    case "border":
                        try {
                            gm.box.box_colour = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), arguments[2]);
                            gm.box.Print("Border colour successfully changed!");
                        } catch {
                            gm.box.Print("Unknown colour, perhaps you mispelled the colour name.");
                        }
                    break;

                    case "foreground":
                    case "fg":
                    case "text":
                        try {
                            gm.box.default_foreground = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), arguments[2]);
                            gm.box.Print("Foreground colour successfully changed!");
                        } catch {
                            gm.box.Print("Unknown colour, perhaps you mispelled the colour name.");
                        }
                    break;

                    case "background":
                    case "bg":
                    case "highlight":
                    case "hl":
                        try {
                            gm.box.default_background = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), arguments[2]);
                            gm.box.Print("Highlight colour successfully changed!");
                        } catch {
                            gm.box.Print("Unknown colour, perhaps you mispelled the colour name.");
                        }
                    break;

                    default:
                        gm.box.Print("Incorrect syntax, usage is: '{Cyan}colour [foreground/background/border] [consolecolour]'");
                        break;

                }
            break;

            case "status":

                if (arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax: usage: '{Cyan}status [obj/direction] [object name/direction]'.");
                    return;
                }

                try {
                    switch (arguments[1]) {

                        case "obj":
                            Interactable temp = gm.env.get_interactable_tag(arguments[2]);
                            gm.box.Print("{Magenta}" + temp.name + "{Magenta}, lock: " + temp.item_req + ", has_been_used: " + temp.has_been_used + ", one_time_use: " + temp.one_time_use + ".");
                        break;

                        case "objs":
                            foreach(string it in gm.env.current_room.room_interactable_tags) {
                                Interactable tempo = gm.env.get_interactable_tag(it);
                                gm.box.Print("{Magenta}" + tempo.name + "{Magenta}, lock: " + tempo.item_req + ", has_been_used: " + tempo.has_been_used + ", one_time_use: " + tempo.one_time_use + ".");
                            }
                        break;

                        case "dir":
                            direction temp_dir = gm.env.get_direction(arguments[2]);
                            gm.box.Print("{Magenta}" + temp_dir.direction_str + ", is_locked: " + temp_dir.is_locked + ", item_required: " + temp_dir.item_required + ", leads: " + temp_dir.direction_leads);
                        break;

                        case "dirs":
                            foreach(direction dirt in gm.env.current_room.room_directions) {
                                gm.box.Print("{Magenta}" + dirt.direction_str + ", is_locked: " + dirt.is_locked + ", item_required: " + dirt.item_required + ", leads: " + dirt.direction_leads);
                            }
                        break;

                        default:
                            gm.box.Print("Incorrect syntax: usage: '{Cyan}status [obj/direction] [object name/direction]'.");
                        break;

                    }
                } catch {
                    gm.box.Print("Incorrect syntax: usage: '{Cyan}status [obj/direction] [object name/direction]'.");
                }
            break;

            case "unlock":
                try {
                    switch (arguments[1]) {

                        case "obj":
                            Interactable temp = gm.env.get_interactable_tag(arguments[2]);
                            if (temp.tag == null) {
                                gm.box.Print("Interactable not found.");
                                return;
                            }
                            temp.item_req = "";
                            gm.box.Print("{Magenta}" + arguments[2] + " unlocked.");
                        break;

                        case "dir":
                            direction tem = gm.env.get_direction(arguments[2]);
                            if (tem.direction_leads == null) {
                                gm.box.Print("Direction not found.");
                                return;
                            }
                            tem.is_locked = false;
                            gm.box.Print("{Magenta}" + arguments[2] + " unlocked.");
                        break;

                        case "dirs":
                            foreach(direction dir in gm.env.current_room.room_directions) {
                                dir.is_locked = false;
                            }
                            gm.box.PrintD("Directions unlocked.");
                        break;

                        case "objs":
                            foreach(Interactable it in gm.env.get_room_interactables()) {
                                it.item_req= "";
                            }
                            gm.box.PrintD("Objects unlocked.");
                        break;

                        case "objs_all":
                            foreach(Interactable itr in gm.env.all_interactables) {
                                itr.item_req = "";
                            }
                            gm.box.PrintD("Objects unlocked.");
                        break;

                        default:
                            gm.box.Print("Incorrect syntax: usage: '{Cyan}unlock [obj/direction] [object name/direction]'.");
                        break;

                    }
                } catch {
                    gm.box.Print("Incorrect syntax: usage: '{Cyan}unlock [obj/direction] [object name/direction]'.");
                }
            break;

            case "do":
                if (arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax, usage is '{Cyan}/do [command tag] [result tag]{end}'");
                    return;
                }
                gm.Do(arguments[1], arguments[2]);
                break;

            case "env":
                try {
                    if(arguments[1] == null) {
                        gm.box.Print("Incorrect syntax, usage is: '{Cyan}env [name of env file]{end}.");
                    }
                    gm.env.load_env(arguments[1]);
                    gm.box.Print("environment succesfully loaded: " + arguments[1] + ", please be aware any errors such as {Red}missing rooms or objects{end} are not caught by the parser.");
                } catch {
                    gm.box.Print("Error: either the environment file you asked for doesn't exist, or is missing key files in order to be loaded.");
                }
            break;

            case "tag":
            case "tags":
                if(arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax, usage is: '{Cyan}tag [add/remove/list] [optional tag to remove or add]{end}.");
                    return;
                }
                switch (arguments[1]) {

                    case "add":
                        gm.player.player_tags.Add(arguments[2]);
                        gm.box.Print(arguments[2] + " successfully added.");
                    break;

                    case "remove":
                        if(gm.player.player_tags.Contains(arguments[2])) {
                            gm.player.player_tags.Remove(arguments[2]);
                            gm.box.Print(arguments[2] + " successfully removed.");
                        }
                        else {
                            gm.box.PrintD("Tag not found.");
                        }
                    break;

                    case "reset":
                        gm.player.reset_tags();
                    break;

                    case "list":
                        string h = "";
                        foreach(string ta in gm.player.player_tags) {
                            h += ta + " ";
                        }
                        gm.box.PrintD(h);
                    break;

                    default:
                        gm.box.Print("Incorrect syntax, usage is: '{Cyan}tag [add/remove/list] [optional tag to remove or add]{end}.");
                    return;

                }
            break;

            case "use":
                if (arguments.Length < 2) {
                    gm.box.Print("Incorrect syntax, usage is '{Cyan}/use [interactable tag]{end}'");
                    return;
                }
                if (gm.env.all_interactables.Contains(gm.env.get_interactable_tag(arguments[1]))) {
                    gm.env.UseF(gm.env.get_interactable_tag(arguments[1]));
                }
                else {
                    gm.box.PrintD("Interactable not found.");
                }
            break;

            case "name":
                if (arguments.Length < 2) {
                    gm.box.Print("{Magenta}Player name: " + gm.player.name);
                    return;
                }
                else gm.Do("name", arguments[1]);
            break;

            case "bio":
                if (arguments.Length < 2) {
                    gm.box.Print("{Magenta}Player bio: " + gm.player.bio);
                    return;
                }
                gm.Do("bio", arguments[1]);
            break;
            
            case "l":
            case "ls":
            case "list":
                try { 
                    switch (arguments[1]) {
            
                    case "rooms":
                        foreach(room_short rom in gm.env.rooms) {
                            gm.box.Print(rom.tag);
                        }
                    break;

                    case "tags":
                        string h = "";
                        foreach(string ta in gm.player.player_tags) {
                            h += ta + " ";
                        }
                        gm.box.PrintD(h);
                    break;

                    case "objs":
                        if (arguments[2] != null) {
                            if (arguments[2] == "all" || arguments[2] == "a") {
                                foreach(Interactable it in gm.env.all_interactables) {
                                    gm.box.Print(it.name + " - " + it.tag);
                                }
                            }
                            else if(arguments[2] == "room" || arguments[2] == "r") {
                                foreach(string it in gm.env.current_room.room_interactable_tags) {
                                    gm.box.Print(it);
                                }
                            }
                            else {
                                foreach(Interactable it in gm.env.all_interactables) {
                                    gm.box.Print(it.name + " - " + it.tag);
                                }
                            }
                        } 
                    break;

                    case "items":
                        if (arguments[2] != null) {
                            if (arguments[2] == "all" || arguments[2] == "a") {
                                foreach(Item it in gm.env.all_items) {
                                    gm.box.Print(it.name + " - " + it.tag);
                                }
                            }
                            else if(arguments[2] == "room" || arguments[2] == "r") {
                                foreach(string it in gm.env.current_room.room_item_tags) {
                                    gm.box.Print(it);
                                }
                            }
                            else {
                                foreach(Item it in gm.env.all_items) {
                                    gm.box.Print(it.name + " - " + it.tag);
                                }
                            }
                        } 
                    break;

                    case "saves":
                    case "save":
                        string[] sflies = System.IO.Directory.GetFiles("logs\\");
                        int nul = 1;
                        foreach(string jk in sflies) {
                            gm.box.Print("{Magenta}> {Black,Magenta}" + nul.ToString() + ": " + jk.Replace("logs\\", "") + " -> {Red,Magenta}" + gm.fm.readTxtFileAtLine(jk, 1));
                            nul ++;
                        }
                    break;

                    default:
                        gm.box.Print("Incorrect syntax, usage is: '{Cyan}list [element]{end}', where element is rooms, interactables, or items.");
                    break;
                    }
                } catch {
                    gm.box.Print("Incorrect syntax, usage is: '{Cyan}list [element]{end}', where element is rooms, interactables, or items.");
                }
            break;

            case "clr":
                gm.box.clr_text();
                gm.box.clr_buffer();
            break;

            default:

                foreach(string comma in custom_admin_commands) {
                    if (arguments[0] == comma.Replace("/", "").Split('=')[0]) {
                        gm.Do(comma.Split('=')[1].Split(":")[0], comma.Split("=")[1].Split(":")[1]);
                        return;
                    } 
                }

                gm.box.Print("Unknown command '{Cyan}" + get_string(arguments) + "{end}', type '{Yellow}/help{end}' or '{Yellow}/?{end}' for help.");
                break;

        }

    }


    public bool YN(string question) {
        gm.box.Print(question);
        gm.box.print_screen();
        string g = System.Console.ReadLine();
        g = g.ToLower();
        if (g == "y" || g == "yes") {
            return true;
        }
        if (g == "n" || g == "no") {
            return false;
        }
        return YN(question);
    }

}