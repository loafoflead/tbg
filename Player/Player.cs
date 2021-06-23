

public class Player {

    public PlayerInventory inv;
    GManager gM;


    public string[] player_tags; //hard limit on player tags for safety
    public int tag_index;

    public Player(GManager gManager) {
        player_tags = new string[50];
        tag_index = 0;
        gM = gManager;
        inv = new PlayerInventory();
    }

    public void add_tag(string tag) {
        player_tags[tag_index] = tag;
        tag_index ++;
    }

    public bool tags_contains(string ind_tag) {
        foreach(string g in player_tags) {
            if (g == ind_tag) {
                return true;
            }
        }
        return false;
    }

}