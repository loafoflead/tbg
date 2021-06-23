using System;

public class Commands {

    GManager gm;

    public string[] arguments;

    public Commands(GManager g) {
        gm = g;
        arguments = new string[10];
    }

    string h = "";
    public void getInput() {
        gm.box.k.stopListener();
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
        gm.box.Print("{Gray}" + get_string(arguments) + " <", Box.format_options.right);

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

            case "examine":
            case "exa":

                try {

                    gm.box.Print(gm.env.get_item_from_alias(arguments[1]).description);

                } catch {
                    gm.box.Print("Incorrect usage of '{Cyan}examine{end}', usage: 'examine [item]', where item is an item in your inventory. You can type 'list' to list these items.");
                }

                break;

            case "explore":
            case "check":
            case "look":
                gm.box.Print(gm.env.description);
            break;

            case "q":
            case "quit":
                gm.is_running = false;
                break;

            default:

                foreach(string obj_tag in gm.env.room_interctable_tags) {
                    Interactable temp_obj = gm.env.get_interactable_tag(obj_tag);

                    foreach(string verb in temp_obj.verbs) {
                        if (verb == arguments[0]) {
                            foreach(string alias in temp_obj.aliases) {
                                if (alias == arguments[1]) {
                                    gm.env.UseVerb(temp_obj, arguments[0]);
                                    return;
                                }
                            }
                        }
                    }
                    

                }

                gm.box.Print("{DarkGray}>\tUnknown Command!'" + get_string(arguments) + "'");

                //do thing to sift through every interctable to check if any of the args correspond to the verb or the object k cool bye
                //thanks!
            break;

        }
    }

    void unlock_menu() {
        
    }


    void admin_commands() {
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
                    gm.box.Print("{Gray}Location not found, ({Blue}" + arguments[1] + "{end})");
                }
                break;

            case "status":
                try {
                    switch (arguments[1]) {

                        case "obj":
                            Interactable temp = gm.env.get_interactable_tag(arguments[2]);
                            gm.box.Print("{Magenta}" + temp.name + ", has_been_used: " + temp.has_been_used + ", one_time_use: " + temp.one_time_use);
                        break;

                        case "dir":
                            direction temp_dir = gm.env.get_direction(arguments[2]);
                            gm.box.Print("{Magenta}" + temp_dir.direction_str + ", is_locked: " + temp_dir.is_locked + ", item_required: " + temp_dir.item_required + ", leads: " + temp_dir.direction_leads);
                        break;

                        case "dirs":
                            foreach(direction dirt in gm.env.room_directions) {
                                gm.box.Print("{Magenta}" + dirt.direction_str + ", is_locked: " + dirt.is_locked + ", item_required: " + dirt.item_required + ", leads: " + dirt.direction_leads);
                            }
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
                            temp.item_req = "";
                        break;

                        case "dir":
                            direction tem = gm.env.get_direction(arguments[2]);
                            tem.is_locked = false;
                        break;

                    }
                } catch {
                    gm.box.Print("Incorrect syntax: usage: '{Cyan}unlock [obj/direction] [object name/direction]'.");
                }
            break;
            
            case "l":
            case "ls":
            case "list":
                try { 
                    switch (arguments[1]) {
            
                    case "rooms":
                        foreach(room_short rom in gm.env.room_tags) {
                            gm.box.Print(rom.tag);
                        }
                    break;

                    case "objs":
                        if (arguments[2] != null) {
                            if (arguments[2] == "all" || arguments[2] == "a") {
                                foreach(Interactable it in gm.env.all_interactables) {
                                    gm.box.Print(it.name);
                                }
                            }
                            else {
                                foreach(string it in gm.env.room_interctable_tags) {
                                    gm.box.Print(it);
                                }
                            }
                        } 
                    break;

                    case "items":
                        if (arguments[2] != null) {
                            if (arguments[2] == "all" || arguments[2] == "a") {
                                foreach(Item it in gm.env.all_items) {
                                    gm.box.Print(it.name);
                                }
                            }
                            else {
                                foreach(string it in gm.env.room_item_tags) {
                                    gm.box.Print(it);
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