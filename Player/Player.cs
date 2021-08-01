using System.Collections;
using System.Collections.Generic;

public class Player {

    public PlayerInventory inv;
    public GManager gM;

    public bool is_operator = false;
    public string name = "no_name";
    public string bio ="bio_empty";
    public int fun = 0;

    public struct player_value {
        public string value;
        public string name;
    }


    public List<string> player_tags; //hard limit on player tags for safety haha jk not
    public List<player_value> player_Values;
    public int tag_index;

    public Player(GManager gManager) {
        System.Random r = new System.Random();
        player_tags = new List<string>();
        player_Values = new List<player_value>();
        tag_index = 0;
        fun = r.Next(1,11);
        gM = gManager;
        inv = new PlayerInventory(this);
    }


    public void add_value(string new_name, string new_value) {
        player_Values.Add(new player_value {
            value = new_value,
            name= new_name,
        });
    }

    public player_value get_value(string tagname) {
        return player_Values.Find(player_value => player_value.name == tagname);
    }

    public void remove_value_by_tag(string tag) {
        player_Values.Remove(player_Values.Find(player_value => player_value.name == tag));
    }


    public void add_tag(string tag) {
        player_tags.Add(tag);
    }

    public bool tags_contains(string ind_tag) {
        foreach(string g in player_tags) {
            if (g == ind_tag) {
                return true;
            }
        }
        return false;
    }

    public void reset_tags() {
        player_tags = new List<string>();
    }

    public void remove_rand_tag() {
        System.Random r = new System.Random();
        int g = r.Next(1,player_tags.Count);
        for (int i = 0; i < g; i ++) {
            player_tags.RemoveAt(i);
        }
    }

}