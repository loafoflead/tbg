using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class Environment {

    GManager gm;

    

    public List<room_short> rooms;
    private XmlNodeList room_list;
    public XmlDocument current_doc;
    public string room_file_name; //name if the xml file containing the rooms


    public List<Item> all_items;
    public List<Interactable> all_interactables;

    public string current_tag;
    public string current_name;
    public int id;
    public room_short current_room;
    public room_short previous_room;

    public string description;

    //                                                      INTERACTABLES & ITEMS 
    public XmlDocument interactable_doc;
    public XmlNodeList interactables_xml;
    public string interactable_doc_name;
    public XmlDocument item_doc;
    public XmlNodeList items_xml;
    public string item_doc_name;

    //                                                              END



    
    

    public bool forwards;
    public bool left;
    public bool right;
    public bool backwards;

    public enum direction_enum {
        left,
        right,
        forwards,
        backwards,
    }


    public Environment(GManager g) {
        gm = g;
        rooms = new List<room_short>();
    }

    public void load_env(string filenam) {
        string filename = "environments\\" + filenam + "\\" + filenam;
        rooms = new List<room_short>();
        room_file_name = filename + "_room.xml";
        current_doc = new XmlDocument();
        current_doc.Load(room_file_name);
        room_list = current_doc.GetElementsByTagName("room");
        foreach (XmlNode nod in room_list) {
            rooms.Add(new room_short{
                tag = nod.ChildNodes.Item(1).InnerText,
                id = int.Parse(nod.ChildNodes.Item(0).InnerText),
                associated_node = nod,
                front_locked = false,
                right_locked = false,
                left_locked = false,
                back_locked = false,
            });
        }

        load_rooms();


        loadRoom(rooms[0].tag);

        all_interactables = new List<Interactable>();
        load_env_interactables(filename);
        all_items = new List<Item>();
        load_env_items(filename);

        string cutscene_name = "";
        if(System.IO.File.Exists(filename + "_cutscene.xml")) {
            cutscene_name = filename + "_cutscene.xml";
            gm.cutscene(GManager.cutscene_types.custom_txt, cutscene_name);
        }
        else {
            gm.cutscene(GManager.cutscene_types.empty);
        }
        
        

    }

    public void load_rooms() {

        foreach(room_short rs in rooms) {
            loadRoom_ind(rs);
        }

    }




    //works
    void load_env_items(string filename) {
        item_doc = new XmlDocument();
        item_doc_name = filename + "_items.xml";
        item_doc.Load(item_doc_name);
        items_xml = item_doc.GetElementsByTagName("item");
        foreach (XmlNode nod in items_xml) {
            all_items.Add(new Item{
                name = nod.ChildNodes.Item(1).InnerText,
                description = nod.ChildNodes.Item(2).InnerText,
                tag = nod.ChildNodes.Item(0).InnerText,
                aliases = new List<string>(nod.ChildNodes.Item(3).InnerText.Split('/')),
            });
        }
    }

    public static string[] generic_verbs = new string[] {
        "examine", "explore", "search"
    };

    static char[] escap = { '\r', '\n' };

    //works 
    void load_env_interactables(string filename) {
        interactable_doc = new XmlDocument();
        interactable_doc_name = filename + "_objs.xml";
        interactable_doc.Load(interactable_doc_name);
        interactables_xml = interactable_doc.GetElementsByTagName("obj");
        foreach (XmlNode nod in interactables_xml) {
            Interactable new_obj = new Interactable{
                tag = nod.ChildNodes.Item(0).InnerText,
                name = nod.ChildNodes.Item(1).InnerText,
                verbs = new List<string>(nod.ChildNodes.Item(2).InnerText.Split('/')),
                full_action = nod.ChildNodes.Item(3).InnerText.Replace("\r","").Replace("\n","").Replace("\t",""),
                action_dia = nod.ChildNodes.Item(4).InnerText,
                item_req = nod.ChildNodes.Item(5).InnerText,
                item_lock_dia = nod.ChildNodes.Item(6).InnerText,
                tag_req = nod.ChildNodes.Item(7).InnerText,
                tag_lock_dia = nod.ChildNodes.Item(8).InnerText,
                tag_given = nod.ChildNodes.Item(9).InnerText,
                one_time_use = System.Convert.ToBoolean(int.Parse(nod.ChildNodes.Item(10).InnerText)),
                used_dialogue = nod.ChildNodes.Item(11).InnerText,
                aliases = new List<string>(nod.ChildNodes.Item(12).InnerText.Split('/')),
            };
            if (new_obj.verbs.Contains("gen")) {
                foreach(string s in generic_verbs) {
                    new_obj.verbs.Add(s);
                }
            }
            all_interactables.Add(new_obj);
        }
    }

    public void loadRoom_ind(room_short room_) {

        previous_room = current_room;

        current_room = room_;

        room_.front_locked = false;
        room_.back_locked = false;
        room_.left_locked = false;
        room_.right_locked = false;

        room_.id = int.Parse(room_.associated_node.ChildNodes.Item(0).InnerText);

        room_.name = room_.associated_node.ChildNodes.Item(2).InnerText;
        room_.tag = room_.associated_node.ChildNodes.Item(1).InnerText;
        
        room_.desc = room_.associated_node.ChildNodes.Item(3).InnerText;

        get_ind_dirs(room_);

        XmlNode rrom = room_.associated_node;

        try {
            room_.room_item_tags = new List<string>(rrom.ChildNodes.Item(6).InnerText.Split('/'));
        } catch {
            room_.room_item_tags = new List<string>();
            room_.room_item_tags.Add(rrom.ChildNodes.Item(6).InnerText);
        }
        try {
            room_.room_interactable_tags = new List<string>(rrom.ChildNodes.Item(5).InnerText.Split('/'));
        } catch {
            room_.room_interactable_tags = new List<string>();
            room_.room_interactable_tags.Add(rrom.ChildNodes.Item(5).InnerText);
        }

    }

    //works
    public void loadRoom(string tag) {
        current_room = rooms.Find(room_short => room_short.tag == tag);
        /*forwards = false;
        left = false;
        right = false;
        backwards = false;
        XmlNode rrom = get_room_tag(tag);
        current_tag = rrom.ChildNodes.Item(1).InnerText;
        current_name = rrom.ChildNodes.Item(2).InnerText;
        description = rrom.ChildNodes.Item(3).InnerText;
        getDirs(rrom);
        try {
            room_item_tags = new List<string>(rrom.ChildNodes.Item(6).InnerText.Split('/'));
        } catch {
            room_item_tags = new List<string>();
            room_item_tags.Add(rrom.ChildNodes.Item(6).InnerText);
        }
        try {
            room_interctable_tags = new List<string>(rrom.ChildNodes.Item(5).InnerText.Split('/'));
        } catch {
            room_interctable_tags = new List<string>();
            room_interctable_tags.Add(rrom.ChildNodes.Item(5).InnerText);
        }*/
    }


    void get_ind_dirs(room_short rs) {

        XmlNode dirs = rs.associated_node.ChildNodes.Item(4); //this gets the 'direction' tag from the file
        rs.room_directions = new List<direction>();

        foreach(XmlNode child in dirs) { //runs through and loads each direction, 'left', 'right', etc...
            if (child.ChildNodes.Item(0).InnerText != "") {
            switch (child.Name) {

                case "left":
                    left = true;
                    load_individual_dir(direction_enum.left, child, rs);
                break;

                case "right":
                    right = true;
                    load_individual_dir(direction_enum.right, child, rs);
                break;

                case "forward":
                    forwards = true;
                    load_individual_dir(direction_enum.forwards, child, rs);
                break;

                case "back":
                    backwards = true;
                    load_individual_dir(direction_enum.backwards, child, rs);
                break;

                default:
                break;

            }
            }

            /*if (child.ChildNodes.Item(0).InnerText != "") {
                switch (dir) {
                    case 0:
                        left = true;
                        load_individual_dir(direction_enum.left, child, rs);
                    break;

                    case 1:
                        right = true;
                        load_individual_dir(direction_enum.right, child, rs);
                    break;

                    case 2:
                        forwards = true;
                        load_individual_dir(direction_enum.forwards, child, rs);
                    break;

                    case 3:
                        backwards = true;
                        load_individual_dir(direction_enum.backwards, child, rs);
                    break;

                    default:
                        break;
                }
                dir ++;
            }*/
        }

    }

    void load_individual_dir(direction_enum dir, XmlNode direction_node, room_short room) {

        try{ 
            direction direc = new direction{

            corresponding_enum = dir,
            direction_str = dir.ToString(),
            direction_int = (int) dir,

            direction_leads = direction_node.ChildNodes.Item(0).InnerText,
            action_dialogue = direction_node.ChildNodes.Item(1).InnerText,
            is_locked = System.Convert.ToBoolean(int.Parse(direction_node.ChildNodes.Item(2).InnerText)),
            item_locked_dialogue = direction_node.ChildNodes.Item(3).InnerText,
            item_unlock_dialogue = direction_node.ChildNodes.Item(4).InnerText,
            tag_locked_dialogue = direction_node.ChildNodes.Item(6).InnerText,
            item_required = direction_node.ChildNodes.Item(5).InnerText,
            tag_required = direction_node.ChildNodes.Item(7).InnerText,

            };  
            room.room_directions.Add(direc);
        } catch {
            direction direc = new direction{

            corresponding_enum = dir,
            direction_str = dir.ToString(),
            direction_int = (int) dir,

            direction_leads = direction_node.ChildNodes.Item(0).InnerText,
            action_dialogue = direction_node.ChildNodes.Item(1).InnerText,
            is_locked = false,
            item_locked_dialogue = direction_node.ChildNodes.Item(3).InnerText,
            item_unlock_dialogue = direction_node.ChildNodes.Item(4).InnerText,
            tag_locked_dialogue = direction_node.ChildNodes.Item(6).InnerText,
            item_required = direction_node.ChildNodes.Item(5).InnerText,
            tag_required = direction_node.ChildNodes.Item(7).InnerText,

            }; 
            room.room_directions.Add(direc);
        }


    }

    //to be tested
    /*void getDirs(XmlNode room) {
        XmlNode dirs = room.ChildNodes.Item(4); //this gets the 'direction' tag from the file
        room_directions = new List<direction>();
        int dir = 0;
        foreach(XmlNode child in dirs) { //runs through each direction, 'left', 'right', etc...
            if (child.ChildNodes.Item(0).InnerText != "") {
                switch (dir) {
                    case 0:
                        left = true;
                        load_dir(direction_enum.left, child);
                    break;

                    case 1:
                        right = true;
                        load_dir(direction_enum.right, child);
                    break;

                    case 2:
                        forwards = true;
                        load_dir(direction_enum.forwards, child);
                    break;

                    case 3:
                        backwards = true;
                        load_dir(direction_enum.backwards, child);
                    break;

                    default:
                        break;
                }
                dir ++;
            }
        }
    }*/

    //to be tested
/*    void load_dir(direction_enum dir, XmlNode direction_node) {
        try{ 
            direction direc = new direction{

            corresponding_enum = dir,
            direction_str = dir.ToString(),
            direction_int = (int) dir,

            direction_leads = direction_node.ChildNodes.Item(0).InnerText,
            action_dialogue = direction_node.ChildNodes.Item(1).InnerText,
            is_locked = System.Convert.ToBoolean(int.Parse(direction_node.ChildNodes.Item(2).InnerText)),
            item_locked_dialogue = direction_node.ChildNodes.Item(3).InnerText,
            item_unlock_dialogue = direction_node.ChildNodes.Item(4).InnerText,
            tag_locked_dialogue = direction_node.ChildNodes.Item(6).InnerText,
            item_required = direction_node.ChildNodes.Item(5).InnerText,
            tag_required = direction_node.ChildNodes.Item(7).InnerText,

            };  
            room_directions.Add(direc);
        } catch {
            direction direc = new direction{

            corresponding_enum = dir,
            direction_str = dir.ToString(),
            direction_int = (int) dir,

            direction_leads = direction_node.ChildNodes.Item(0).InnerText,
            action_dialogue = direction_node.ChildNodes.Item(1).InnerText,
            is_locked = false,
            item_locked_dialogue = direction_node.ChildNodes.Item(3).InnerText,
            item_unlock_dialogue = direction_node.ChildNodes.Item(4).InnerText,
            tag_locked_dialogue = direction_node.ChildNodes.Item(6).InnerText,
            item_required = direction_node.ChildNodes.Item(5).InnerText,
            tag_required = direction_node.ChildNodes.Item(7).InnerText,

            }; 
            room_directions.Add(direc);
        }
    }*/

    public int Move(direction_enum direction_Enum) {
        foreach(direction direc in current_room.room_directions) {
            if (direc.direction_int == (int) direction_Enum) {
                if (direc.is_locked != true) {
                    effect_direction(direc);
                    return 1;
                }
                else {
                    gm.box.Print(direc.item_locked_dialogue);
                    if (gm.player.inv.player_inventory.Contains(get_item_from_tag(direc.item_required))) {
                        if (gm.cm.YN("Do you want unlock this passage using " + get_item_from_tag(direc.item_required).name + "?") == true) {
                            Unlock(direc);
                            effect_direction(direc);
                            return 1;
                        }
                        else {
                            return 0;
                        }
                    }
                    return 2; //if direction is locked
                }
            }
        }
        return 0;
    }

    public int Unlock(direction direct) {
        foreach(Item it in gm.player.inv.player_inventory) {
            foreach(direction dir in current_room.room_directions) {
                if (it.tag == dir.item_required) {
                    dir.is_locked = false;
                    gm.box.Print(dir.item_unlock_dialogue);
                    return 1;
                }
            }
        }
        return 0;
    }
    public int Unlock_enum(direction_enum direction_Enum) {
        foreach(direction direc in current_room.room_directions) {
            if (direc.corresponding_enum == direction_Enum) {

                if(gm.player.inv.player_inventory.Contains(get_item_from_tag(direc.item_required))) {

                    direc.is_locked = false;
                    gm.box.Print(direc.item_unlock_dialogue);
                    return 1;

                }
                else {
                    gm.box.nl();
                    gm.box.Print("{Cyan}>\t{end}<{Red}X{end}> ({Red}" + get_item_from_tag(direc.item_required).name + "{end}) {end,end}{Cyan}{end}!");
                    gm.box.nl();
                    return 2;
                }

            }
        }
        return 0; //direction not found
    }

    void effect_direction(direction dir) {
        gm.Do("go", dir.direction_leads);
        gm.box.Print(dir.action_dialogue);
    }

    //works
    public XmlNode get_room_tag(string ta) {
        foreach(room_short rs in rooms) {
            if (rs.tag == ta) { //searches all the room tags for a match with given room tag
                System.Console.WriteLine("found room tag: " + rs.associated_node.ChildNodes.Item(1).InnerText);
                return rs.associated_node;
            }
        }
        System.Console.WriteLine("failed to find room tag");
        return null;
    }
    public string get_room_name_by_tag(string tag) {
        XmlNode temp = get_room_tag(tag);
        return temp.ChildNodes.Item(2).InnerText;
    }


    public void UseAl(string alias, string verb) {
        foreach(Interactable it in all_interactables) {
            if (it.aliases.Contains(alias) && current_room.room_interactable_tags.Contains(it.tag)) {
                UseVerb(it, verb);
            }
        }
        gm.box.Print("Object not found, try checking the name for spelling errors.");
    }

    //may work
    public void UseVerbWithTag(string tag, string verb) {
        UseVerb(get_interactable_tag(tag), verb);
    }
    public int UseVerb(Interactable obj, string verb) { //to rework with locking mechanics
        if (obj.tag == "") {
            return 0;
        }

        obj.add_gm(gm);
        int try_time = obj.try_use(verb);

        switch (try_time) {
            case 0:
                return 2; //return a 2 if the object cant be used with that verb

            case 1:
                effect_obj(obj); //do the objects action if the return is 1
                return 1;

            case 2:
                gm.box.Print(obj.used_dialogue);
                return 0;

            case 3:
                gm.situ_change(GManager.change_types.missing_item, get_item_from_tag(obj.item_req).name);
                gm.box.Print(obj.item_lock_dia);
                return 3; //3 if obect is locked
            
            default:
                gm.box.Print("Fatal Internal error occurred 'Use' ERR_08");
            break;
        }
        return 0;
    }

    public void UseF(Interactable it) {
        try {
            effect_obj(it);
        } catch {
            gm.box.Print("Internal error Interactable not found ERR_06²");
        }
    }
    public void UseF(string interactable_tag) {
        try {
            UseF(get_interactable_tag(interactable_tag));
        } catch {
            gm.box.Print("Internal error Interactable not found ERR_06");
        }
    }

    //may work
    void effect_obj(Interactable obk) {
        gm.box.Print(obk.action_dia);
        //if(!gm.fm.null_or_empt(obk.tag_given)) gm.player.add_tag(obk.tag_given);
        gm.Do(obk.action_prefix, obk.action_result);
    }


    //to be tested
    public int Go(string room_tag) {
        try {

            if(room_tag.Contains(":")) {
                string env_name = room_tag.Split(":")[0];
                gm.Do("env", room_tag.Split(":")[1]);
                return 1;
            }

            room_short r = get_room_via_tag(room_tag);
            if (r != null) {
                previous_room = current_room;
                current_room = get_room_via_tag(room_tag);
                return 1;
            } 
            else return 0;
        } catch {
            return 0;
        }
    }


    room_short get_room_via_tag(string tag) {
        foreach(room_short rs in rooms) {
            if (rs.tag == tag) {
                return rs;
            }
        }
        return null;
    }

    //INTERACTABLES WORK 100%
    public Interactable get_interactable_tag(string tagname) {
        foreach(Interactable it in all_interactables) {
            if(it.tag == tagname) {
                return it;
            }
        }
        return new Interactable{name = "Interactable tag Not Found ERR_06"};
    }

    public Interactable get_interactable_name(string _name) {
        foreach(Interactable it in all_interactables) {
            if (it.name == _name) {
                return it;
            }
        }
        return new Interactable{name = "Interactable name Not Found ERR_05"};
    }

    public List<Interactable> get_room_interactables() {
        List<Interactable> return_list = new List<Interactable>();
        foreach(Interactable it in all_interactables) {
            foreach(string itr in current_room.room_interactable_tags) {
                if (it.tag == itr) {
                    return_list.Add(it);
                }
            }
        }
        return return_list;
    }

    public direction get_direction(string name) {
        foreach(direction dir in current_room.room_directions) {
            if (dir.direction_str == name) {
                return dir;
            }
        }
        return null;
    }


    //items r a wip but cool anyway :) and rooms work UPDATE: items work
    public Item get_item_from_tag(string tag_name) {
        foreach(Item it in all_items) {
            if (it.tag == tag_name) {
                return it;
            }
        }
        return new Item{name = "Item Not Found ERR_03"};
    }

    public Item get_item_from_alias(string alias) {
        foreach(Item it in all_items) {
            foreach(string f in it.aliases) {
                if (f == alias) {
                    return it;
                }
            }
        }
        return new Item{name = "{Red}ITEM NOT FOUND"};
    }

    public Item get_item_from_name(string _name) {
        foreach(Item it in all_items) {
            if (it.name.ToLower() == _name.ToLower()) {
                return it;
            }
        }
        return new Item{name = "Item Tag Not Found ERR_04"}; //try and find an item by name, if it fails, return an item with the name containing an error.
    }

    public List<Item> get_items_in_room() {
        List<Item> ret_list = new List<Item>();
        foreach (Item it in all_items) { //search through every item in add it to the list of item to return
                    ret_list.Add(it);
            foreach(string h in current_room.room_item_tags) { //then through all items in the room
                if (h == it.tag) { //if any of the tags matches any of the items, 
                }
            }
        }
        return ret_list; //return the list
    }


}

public class Item {

    public string name;
    public string description;
    public string tag;

    public List<string> aliases;

}

public class Interactable {

    GManager gm;

    public void add_gm(GManager gManager) {
        gm = gManager;
    }

    public string name;
    public string tag;
    public List<string> verbs;

    private string action;
    public string full_action {
        get {
            return action;
        }
        set{
            action = value;
        }
    }
    public string action_prefix {
        get {
            return full_action.Split(':',2)[0];
        }
    }
    public string action_result {
        get {
            return full_action.Split(':',2)[1];
        }
    }
    public string action_dia;
    public string item_req;
    public string item_lock_dia;
    public string tag_req;
    public string tag_lock_dia;
    public string tag_given;
    public bool one_time_use;
    public string used_dialogue;
    public List<string> aliases;

    public bool has_been_used = false;

    public int try_use(string verb) {
        foreach(string ver in verbs) {
            if (ver == verb) {

                if (has_been_used == true && one_time_use == true) {

                    return 2; //if the object has been used and is a one time use, return 2

                }
                else {

                    if (item_req != "" && gm.player.inv.player_inventory.Contains(gm.env.get_item_from_tag(this.item_req))) {
                        //if the item is locked and player has item required then :
                            has_been_used = true; //if its a one time use and hasnt been used, set used to true
                            return 1; //if all succeeds, return a 1 for sucess
                    }
                    else if (item_req != "" && !gm.player.inv.player_inventory.Contains(gm.env.get_item_from_tag(this.item_req))) {

                        return 3; //3 if interactable is locked and player does not have item needed.

                    }
                    else {

                        has_been_used = true;
                        return 1; // return if obj is not locked

                    }

                }

            }
        }
        return 0; //if not match is found for the verb, return a 0 for failiure
    }

}

public class direction {

    

    public Environment.direction_enum corresponding_enum;
    public string direction_str;
    public int direction_int;

    public string direction_leads;
    public string action_dialogue;
    public bool is_locked;
    public string item_locked_dialogue;
    public string item_unlock_dialogue;
    public string tag_locked_dialogue;
    public string item_required;
    public string tag_required;

}

public class room_short {

    public List<direction> room_directions;
    public List<string> room_interactable_tags;
    public List<string> room_item_tags;


    public string desc;


    public string tag;
    public string name;
    public int id;
    public XmlNode associated_node;
    public bool front_locked;
    public bool left_locked;
    public bool right_locked;
    public bool back_locked;
    
}