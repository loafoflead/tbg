using System;

public class Commands {

    GManager gm;

    public string[] arguments;

    public Commands(GManager g) {
        gm = g;
        arguments = new string[10];
    }

    public string h = "";
    public bool succeeded = false;
    public void getInput() {
        gm.box.k.stopListener();
        succeeded = true;
        h = Console.ReadLine();
        arguments = new string[10];
        if (h.Contains(" ")) arguments = h.Split(" ");
        else arguments[0] = h;
        //gm.box.Print("'" + get_string(arguments) + "' '" + h + "'");

        if (arguments[0].Length > 1) { 
            if(arguments[0][0] == '/') {
                admin_commands();
            }
            else {
                regular_commands();
            }
        }
        else {
            if(arguments[0] == "/") {
                admin_commands();
            }
            else {
                regular_commands();
            }
            
            //gm.box.Print("{Gray}>\tUnkown Command!\t'" + h + "'");
        }

        //whahahahaa what was i gonna do cummy wummy hole man >:))))))))]]]]]]]]]] help
        if (succeeded == true && gm.show_old_msgs == true) {
            //gm.fm.write_next(get_string(arguments), gm.log_file); BIG BROKEN !!!!!!!
            gm.box.Print("{DarkGray}" + get_string(arguments));
        }

        gm.box.k.startListener();

    }

    string get_string(string[] str) {
        string to_ret = "";
        foreach(string h in str) {
            to_ret += h + " ";
        }
        return to_ret;
    }

    void regular_commands() {

        switch (arguments[0]) {

            case "back":
                gm.env.loadRoom_ind(gm.env.previous_room);
            break;

            case "clear":
            case "clr":
            case "cls":
                gm.box.clr_buffer();
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
                            gm.player.inv.player_inventory.Remove(it);
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
                break;

            default:

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


    void admin_commands() {

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

            case "goto":
            case "g":
            case "go":
                int g = gm.env.Go(arguments[1]);
                if (g == 1) {
                    gm.situ_change(GManager.change_types.move_to, gm.env.current_name);
                }
                else {
                    gm.box.Print("{Gray}Location not found, ({Red}" + arguments[1] + "{end})");
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
                if (arguments[1] == null) {
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

                        default:
                            gm.box.Print("Incorrect syntax: usage: '{Cyan}unlock [obj/direction] [object name/direction]'.");
                        break;

                    }
                } catch {
                    gm.box.Print("Incorrect syntax: usage: '{Cyan}unlock [obj/direction] [object name/direction]'.");
                }
            break;

            case "do":
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
                if (gm.env.all_interactables.Contains(gm.env.get_interactable_tag(arguments[1]))) {
                    gm.env.UseF(gm.env.get_interactable_tag(arguments[1]));
                }
                else {
                    gm.box.PrintD("Interactable not found.");
                }
            break;

            case "name":
                gm.Do("name", arguments[1]);
            break;

            case "bio":
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

                    case "clr":
                        gm.box.clr_text();
                        gm.box.clr_buffer();
                    break;

                    default:
                        gm.box.Print("Incorrect syntax, usage is: '{Cyan}list [element]{end}', where element is rooms, interactables, or items.");
                    break;
                    }
                } catch {
                    gm.box.Print("Incorrect syntax, usage is: '{Cyan}list [element]{end}', where element is rooms, interactables, or items.");
                }
            break;

            default:
                gm.box.Print("Unkown command, type '{Yellow}/help{end}' or '{Yellow}/?{end}' for help.");
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