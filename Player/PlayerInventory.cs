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

    public void remove_rand() {
        System.Random r = new System.Random();
        int g = r.Next(1,player_inventory.Count);
        for (int i = 0; i < g; i ++) {
            player_inventory.RemoveAt(i);
        }
    }

    public void remov(Item i) {
        player_inventory.Remove(i);
    }

    public void reset_inv() {
        player_inventory = new List<Item>();
    }

}