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

    Room files contain a name, a description, a tag, an id, directions, the items in the room, and the objects in the room.
    The name and description are self explanatory, the id is meaningless, but the tag is important.
    The tag is how other rooms or interactables can interface with the room. For example, if you called your room {Red}DANGER ZONE{end}, it would be inconvenient to have to write the excape sequences
    each time you wanted to reference it, so make the tag danger_zone, for example.
    The directions node contains four directions, leave the 'leads' tag blank if you dont want the direction to exist. other than that, the tags are relatively self explanatory.
    for the objs and items tags, put a '/' separated list of all the object and item tags you can in your room, and if they exist in your environment_items.xml file, they will be loaded into the room.
    be aware that mispelling information or adding in items or interactables that do not exist may cause a crash or other unexpected errors!

    Item files contain item nodes made up of a tag, a name, and a description. Since the items can't actually be used individually, there's no point in assigning functions to them, instead
    letting the rooms and interactables use them as keys and other things.
    The tag and name system works the same as with the rooms, but there's also the alias list, which is a '/' seperated list of the different names to call the item by when the player is
    referencing it. Its very important not to leave this field blank, as it can cause a crash or other unexpected issues.

    Interactable files are the most complex of the bunch, with a whole bunch of tags and weird syntaxes.
    To get the basic things out of the way, interactables use the same alias system as items to be used by the player, and the same tag and name system as rooms and items.
    Now, onto the new stuff;
        -Verbs 
            Each interactable has it's own '/' seperated list of verbs, which can be used to interface with it. Avoid naming these verbs the same as any of the commands listed above, as it it can
            cause unexpected issues.
        -Action Dia (Action dialogue)
            The dialogue the interactable displays when it is used by the player.
        -Item required & Item dialogue
            The item the obj needs to be used by the player, and the dialogue it will display if the player does not posess the item.
        -Tag required & tag dialogue
            These tags are obsolete :/
        -Tag given
            The tag added to the player when they use the interactable
        -one time use & used dialogue
            Whether the interactable can only be used once and what dialogue it should display when it has already been used.
        
        Finally, there's a tag called the 'action' tag, this is the most stupidly overengineered one of all of them, so buckle in. This tag allows you to perform simple actions such as,
            go: [room tag]
            say: [words to be printed on the screen in cyan double quotes]
            give: [item tag to be given to the player]
            take: [all/random/ or the item tag to take from the player] (note: if the player doesn't posess the item, nothing happens.)
            print: [any text to be printed raw to the screen]
            null: [do nothing]

        To chain multiple tags together, simply place a '+' sign in between them, as follows:
            say:hello, how are you?+give:hundred_pounds

        But there is also a tag called 'if'. The general syntax of the 'if' tag is as follows:
            
            <action>if:(name=loafoflead):(say:hello bread)?(take:all)</action>

        So, let's break this down. 
            Firstly, we can see that the 'if' tag is place right at the start, followed by a colon, what follows in rounded brackets is the condition to be met.
            Conditions include,
                -name (the player's name)
                -inv (searches the player's inventory for the specified item)
                -room (the current room's tag)
                -op (whether or not the player has operator status)
                -tag (searches the player's tags for a specific tag)
            After the condition is an '=' sign, and what is written behind the '=' symbol is the condition to be met,
                E.G:
                    if:(name=Derek) 
                          ^     ^
                checks player the name you're
                    name        looking for

            After the condition in brackets, there is another colon signifying what happens if the condition is met.
                if:(name=Derek):(say:hi Derek)?[...]
                                    ^   ^
                               action to execute if 
                                condition is met    
            Finally, after the first condition is a '?', and everything after that is to be executed if the condition is not met.
                if:(name=Derek):(say:hi Derek)?(say:You smell)
                                                      ^
                                              execute if condition is
                                                    NOT met
        
        With all that explained, there's only one more thing that i should probably clarify. Nested ifs are supported, but not to a massive extent. For example,
            if:(name=no_name):(if:(inv=headband):(say:hi)?(say:bye))?(say:i like france)
            This, is a valid tag to be put in the action node.
        This statement can be broken down into it's component parts:
            if:(name=no_name):          ([...])     ?      (say:i like france)
                    ^                      ^                        ^
                the condition     execute if condition  execute if condition is not
                                        is met                     met
        Then, we can easily take the segment inside sqare brackets, '[...]', or '(if:(inv=headband):(say:hi)?(say:bye))', and apply the exact same logic to it that we applied to the 
        earlier tags.

    So there, a step by step breakdown of everything you need to know about environment files and how to configure them. And keep in mind the action tag system can be interacted with directly using 
    commands for testing, the only thing to note is that a command like this one, 'if:(name=hihi):(say:haha)?(say:bye)', will be entered like this, '/do if (name=hihi):(say:haha)?(say:bye)'

END OF README (yeah, theres actually nothing more. oh actually, there's this '_<') ((now it's done))