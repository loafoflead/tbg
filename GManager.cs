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

        cutscene(cutscene_types.intro);
        box.waitf(1);
        box.clr_buffer();
        cutscene(cutscene_types.custom_txt, "start_cutscene.txt");
        box.waitf(1);
        box.clr_buffer();
        box.refresh_box();
        box.clr_text();

        if (cm.YN("Do you want to read a log file?") == true) {
            loadave(System.Console.ReadLine());
        }
        else {
            box.Print("[{Red}SIMULATING COMMAND: {end}'{Cyan,White}look{end,end}' {Red}...{end}]");
            box.nl();
            box.Print(env.current_room.desc);
            box.print_screen();
        }

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


    void loadave(string filename) {

    }


    public void Do(string action, string result) {
        switch (action) {
            case "give":
                
                Item it = env.get_item_from_tag(result);
                int res = 0;
                if (it.tag != null) res = player.inv.add_to_inv(it);
                else box.Print("{DarkRed}Item not found: " + it.name);
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

            case "take":
                switch(result) {
                    case "all":
                        player.inv.reset_inv();
                        situ_change(change_types.lose_item, "{DarkRed}INVENTORY DELETED!{end}");
                    break;

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

            case "if": //example of syntax: if:inv=headband;go:vault_lobby/say:get headband
            try {
                string compare_tag = result.Split('=')[0];

                string condition = result.Split("=")[1].Split(";")[0];
                
                string if_true = result.Split(";")[1].Split('/')[0];

                string if_false = result.Split('/')[1];


                switch(compare_tag) {
                    
                    case "inv":

                        if(player.inv.player_inventory.Contains(env.get_item_from_tag(result.Split("=")[1].Split(";")[0]))) {

                            Do(result.Split(';')[1].Split(':')[0], result.Split(';')[1].Split(':')[1].Split('/')[0]);

                        }
                        else {
                            Do(result.Split('/')[1].Split(':')[0], result.Split('/')[1].Split(':')[1]);
                        }

                    break;

                    case "name":
                        
                        if(player.name == condition) {
                            Do(if_true.Split(':')[0], if_true.Split(':')[1]);
                        }
                        else {
                            Do(if_false.Split(':')[0], if_false.Split(':')[1]);
                        }

                    break;

                    case "room":
                        if(env.current_tag == condition) {
                            Do(if_true.Split(':')[0], if_true.Split(':')[1]);
                        }
                        else {
                            Do(if_false.Split(':')[0], if_false.Split(':')[1]);
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
                            Do(if_true.Split(':')[0], if_true.Split(':')[1]);
                        }
                        else {
                            Do(if_false.Split(':')[0], if_false.Split(':')[1]);
                        }
                    break;

                    case "tag":
                        if(player.player_tags.Contains(condition)) {
                            Do(if_true.Split(':')[0], if_true.Split(':')[1]);
                        }
                        else {
                            Do(if_false.Split(':')[0], if_false.Split(':')[1]);
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
                box.Print("{Cyan}- \"" +result + "{Cyan}\"");
            break;

            default:
                box.Print("Fatal 'Do' Internal Error occurred ERR_07");
            break;
        }
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

    public enum change_types {
        gain_item,
        lose_item,
        move_to,
        missing_item,
        drop_all,

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


            default:
                box.nl();
                box.Print("{Cyan}>\t{end}<{Gray}?{end}> ({DarkRed}" + "Internal error, argument missing: 'situ_change'." + "{end})");
                box.nl();
            break;
        }
        
    }



}