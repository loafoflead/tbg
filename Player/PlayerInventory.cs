using System.Collections;
using System.Collections.Generic;

public class PlayerInventory {

    private Player pl;

    public List<Item> player_inventory;

    public PlayerInventory(Player p) {
        pl = p;
        player_inventory = new List<Item>();
        player_inventory.Capacity = 4;
    }

    public int add_to_inv(Item item) {
        if(player_inventory.Count > player_inventory.Capacity) {
            return 2; //inventory full
        }
        else {
            player_inventory.Add(item);
            return 1; //item added successfully
        }
    }

    public void reset_inv() {
        player_inventory = new List<Item>();
    }

}