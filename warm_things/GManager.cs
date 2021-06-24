public class GManager {

    public Box box;
    public FileManager fm;
    public KeyHandler kh;
    public Environment env;
    public Player player;
    public Commands cm;

    public bool is_running = true;


    public GManager() {
        box = new Box();
        fm = new FileManager();
        env = new Environment(this);
        player = new Player(this);
        cm = new Commands(this);

        env.load_env("env01");

        cutscene(cutscene_types.intro);
        box.waitf(1);
        box.clr_buffer();
        cutscene(cutscene_types.custom_txt, "start_cutscene.txt");
        box.waitf(1);
        box.clr_buffer();
        box.refresh_box();
        box.clr_text();

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

    public enum cutscene_types {
        intro,
        game_over,
        item_pickup,
        custom_txt,
        custom_xml,
        custom_string
    }


    public void Do(string action, string result) {
        switch (action) {
            case "give":
                int res = player.inv.add_to_inv(env.get_item_from_tag(result));
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

            default:
                box.Print("Fatal 'Do' Internal Error occurred ERR_07");
            break;
        }
    }





    public void cutscene(cutscene_types ct, string file_name = "") {

        string folder_path = "C:\\Users\\benja\\Documents\\warm_things\\cutscenes\\"; //gets the cutscene folder
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

            default:
                box.nl();
                box.Print("{Cyan}>\t{end}<{Gray}?{end}> ({DarkRed}" + "Internal error, argument missing: 'situ_change'." + "{end})");
                box.nl();
            break;
        }
        
    }



}