HI!

Welcome to TBG, or The Big Game! A text based... a d v e n t u r e game with fun and engaging gameplay!

Here's a quick rundown of the commands;
    'look' 
        this command provides the user with feedback on the room they are in, by printing the room's description to the screen.
    'get [item]'
        this command permits the user to pick up and item in the room, one that they know exists.
    'inspect [item]'
        this command tells the player about a specific item in their inventory. To find the items in their inventory, the player can use the 'list' command.
    'list [inventory]'
        this commands shows the player every item in their inventory.
    'drop [item]'
        this command allows the player to drop a specific item from their inventory. dropped items are irretrievable.
    'go [direction]'
        this command uses a directional moving system to let the player move around in the world. the player uses commands such as 'go south', or 'go north', to move around.
    'back'
        this command takes the player back to the previous room they were in.
    'clear'
        clears the screen.
    'where' or 'room'
        tells the player the name of the room they are in.
    'quit' or 'q'
        quits the game.
    
    If the player doesn't input a command, but inputs a verb such as 'examine', or 'use' and then the name of an interactable in the room, the interactable will be used. 
    E.G:
        >look    
        You are in a room, there is a cupboard on the wall.
        >open cupboard  
        You open the cupboard, and get a screw.
    'help'
        displays a help page.
    
    finally, if the player inputs an invalid command, the console outputs the message, 'Unknown command!'.

Admin commands: 
    the admin commands include:

    'give [item tag]':
        Gives the player the item tag they requested.
    'goto [room tag]':
        changes the player's room to be the room tag specified.
    'border [character]'
        changes the screen border to the specified character. It is by default set to ascii 219, or █. (press alt and then the number 219)
    'display [display element]'
        toggles or edits the display element specified. Elements are: 'show_previous_message', to toggle on and off displaying the most recent message in grey on the screen.
    'colour [foreground/highlight/border] [colour]'
        chnages the foreground, background, or border colour to the specified colour. (note, colour names must start with a capital letter.)
    'status [obj/objs/dir/dirs] [optionally specify the object or the direction you're searching for]'
        get the status of a specific object or direction, (obj and dir) or the status of every object and direction in the room [objs/dirs]. to find the objects use '/list'
    'unlock [dir/obj] [north,east,etc/interactable tag]'
        unlocks the direction of object specific, does nothing if it is already unlocked or doesn't exist. You can find interactables using the '/list' command.
    'do [tag] [result]':
        interfaces directly with the 'Do' function. Tags include, go/say/if/print/take/give, and result can be any corresponding information. 
        E.G: 
            >/do go vault_hallway
            or
            >/do say this message is being printed to the screen in double quotes!
            or 
            >/do take [all/random/item tag] headband (note, if the player does not posess a headband, nothing happens)
    'env [env folder name]'
        loads an environment folder with the specified name. More information on environments can be found further down in the readme.
    'tag [add/remove/reset/list]'
        resets, adds, removes, or lists tags, where tags are pieces of information attached to the player to enrich gameplay.
    'use [interactable tag]'
        forces the use of the specified interactable, does not affect the 'has_been_used' tag.
    'list [tags/rooms/objs/items] [all/room]'
        lists every one of the specified thing either in the scope of the room or the scope of the environment. 
    'help'
        displays a help page.
    
    finally, if no match is found for the command, the message 'Unknown command!" is displayed.

Environments:
    Environments are made up of three xml files, one containing the interactables in the room, one containing the items, and one detailing the rooms. in the folder env_01 there exists a template for each of these
    commented out at the bottom of each file.
    room files contain a name, a description, a tag, an id, directions, the items in the room, and the objects in the room.
    the name and description are self explanatory, the id is meaningless, but the tag is important.
    the tag is how other rooms or interactables can interface with the room. For example, if you called your room {Red}DANGER ZONE{end}, it would be inconvenient to have to write the excape sequences
    each time you wanted to reference it, so make the tag dange_zone, for example.
    The directions node contains four directions, leave the 'leads' tag blank if you dont want the direction to exist. other than that, the tags are relatively self explanatory.
    for the objs and items tags, put a '/' separated list of all the object and item tags you can in your room, and if they exist in your environment_items.xml file, they will be loaded into the room.
    be aware that mispelling information or adding in items or interactables that do not exist may cause a crash or other unexpected errors!