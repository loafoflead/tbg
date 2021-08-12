using System.Collections;
using System.Collections.Generic;

public class Action {

    private GManager gm;

    public Action(GManager g) {
        gm = g;
    }


    public int execute(string full_actionn) {

        string full_action = full_actionn.Replace("\r", "").Replace("\n", "").Replace("\t", "");

        if (!full_action.Contains('(')) return 0;
        if (gm.fm.null_or_empt(full_action)) return 0;

        evaluate_action(full_action);

        return 0;

    }

    struct action {
        public int length;
        public action_type action_Type;
        public string action_tag;
        public string content;
        public string condition;
        public string operation {
            get {
                return get_op(condition);
            }
        }
        public string comparison_tag {
            get {
                return condition.Split(operation,2)[0];
            }
        }
        public string comparison_made {
            get {
                return condition.Split(operation, 2)[1];
            }
        }
        public string if_true;
        public string if_false;
        public string content_to_repeat;

        public string routine_name;
        public string sub_routine;

        string get_op(string st) {
            foreach(char t in st) {
                if (t == '=' || t == '<' || t == '>' || t == '!') {
                    return t.ToString();
                }
            }
            return "null";
        }
    }

    

    private enum action_type {
        conditional = 0,
        repeat = 1,
        regular = 2,
        null_action = 3,
        subroutine =4,
        cancel = 5,
    };

    private string[] exceptional_actions = new string[] {
        "if", "repeat"
    };

    void pr(string stringg) {
        gm.box.Print(stringg);
    }

    void prd(string str) {
        gm.box.PrintD(str);
    }

    private static string[] null_action_commands = new string[] {
        "null", "nl", "wait", "wt", "flush", "flsh", "clr", "clear", "nl", "newline", "new_line", "waitk", "wait_key", "wait_k", "wait_any_key"
    };

    private static string[] exceptional_commands = new string[] {
        "creat_subroutine",
        "create_sub", "createsub",
        "subroutine",
        "routine",
        "new_sub",
        "newsub",
        "new_subroutine",
        "sub",
    };

    private static string[] return_commands = new string[] {
        "return", "end", "stop", "cancel", "quit", "stop_action", "end_action", "halt"
    };

    private bool array_contains(string str, string[] array) {
        foreach(string h in array) {
            if (h == str) {
                return true;
            }
        }   
        return false;
    }

    List<action> actions = new List<action>();

    public void cease() {
        //actions = new List<action>();
    }

    int evaluate_action(string full_act) {

        actions = new List<action>();
    
        string act = "";

        for(int i = 0; i < full_act.Length; i ++) {

            string current = full_act[i].ToString();

            if(current != "(" && current != "[") {
                if (current != " ") act += current;
            }   else {
            
                if ("if" == act) {

                    action to_add = parse_if(gm.split_at(full_act, i)[1]);
                    i += to_add.length - act.Length;
                    actions.Add(to_add);

                }
                else if ("repeat" == act) {

                    action to_add = parse_repeat(gm.split_at(full_act, i)[1]);
                    i += to_add.length  - act.Length;
                    actions.Add(to_add);

                }
                else if (array_contains(act, return_commands)) { /* RETURN COMMANDS */

                    action to_add = new action();
                    i += 3;
                    to_add.action_Type = action_type.cancel;
                    actions.Add(to_add);

                }
                else if (array_contains(act, exceptional_commands)) { /* EXCEPTIONAL COMMANDS: CREATE_SUB */

                    action to_add = parse_subroutine(gm.split_at(full_act, i)[1]);
                    i += to_add.length - act.Length;
                    actions.Add(to_add);

                }
                else if (array_contains(act, null_action_commands)) { /* NULL ACTION COMMANDs */

                    action to_add = parse_null(act);
                    i += to_add.length - act.Length;
                    actions.Add(to_add);

                }
                else {
                    
                    action _to_add = parse_regular(gm.split_at(full_act, i)[1], act);
                    i += _to_add.length - act.Length - 1;
                    actions.Add(_to_add);

                }

                prd("latest: " + act + ", index: " + i.ToString() + ", length: " + full_act.Length.ToString());
                act = "";
                

            }
            

        }

        foreach(action ac in actions) {
            action_status(ac);
            switch(ac.action_Type) {

                case action_type.regular:
                    try {
                        gm.Dod(ac.action_tag, ac.content);
                    } catch {
                        prd("failed to run a regular action;");
                    }
                break;

                case action_type.conditional:
                    bool success = gm.check_conditions(ac.comparison_tag, ac.operation, ac.comparison_made);
                    if (success == true) {
                        evaluate_action(ac.if_true);
                    }
                    else if (!gm.fm.null_or_empt(ac.if_false)) {
                        evaluate_action(ac.if_false);
                    }
                break;

                case action_type.subroutine:
                    gm.env.subroutines.Add(new subroutine{
                        name = ac.routine_name,
                        value = ac.sub_routine,
                    });
                break;

                case action_type.null_action:
                    gm.Dod(ac.action_tag, "();");
                break;

                case action_type.cancel:    
                    return 1;

                default:
                    break;

            }
            if (gm.step_through_actions == true) {
                gm.box.k.waitAnyKey();
            }
        }

        return 0;

    }

    action parse_null(string tag) {

        prd("found null");

        action ac  = new action();
        ac.action_Type = action_type.null_action;
        ac.length = tag.Length + 2;
        ac.action_tag = tag;

        action_status(ac);
        
        return ac;
    }

    action parse_subroutine(string routine) {

        if (!routine.Contains('[') || !routine.Contains(']')) {
            gm.box.PrintD("repeat action was recognized with invalid syntax.");
            return new action{
                action_tag = "null",
                content = "();"
            };
        }

        prd("found subroutine declaration");

        action to_return = new action();

        to_return.action_tag = "new_subroutine";

        to_return.action_Type = action_type.subroutine;

        string routine_name = routine.Split('[',2)[1];
        routine_name = routine_name.Split('=',2)[0];

        string routine_proper = routine.Split("=",2)[1];

        routine_proper = gm.split_at(routine_proper, gm.count_to_end(routine_proper))[0];

        to_return.routine_name = routine_name;
        to_return.sub_routine = routine_proper;

        to_return.length = gm.count_to_end(routine.Split('[',2)[1]) + to_return.action_tag.Length + 1;

        action_status(to_return);
        
        return to_return;

    }

    action parse_regular(string action_script, string action_t) {

        action_t = action_t.Replace("[", "").Replace("]", "");
       if (action_t.Contains(';')) action_t = action_t.Split(';',2)[1];

        prd("found regular");

        action to_return = new action();

        to_return.action_tag = action_t;

        string rest = action_script.Split('(',2)[1];

        to_return.action_Type = action_type.regular;

        string content = gm.split_at(rest, gm.count_to_end(rest, 0, '(', ')'))[0];

        if (gm.fm.null_or_empt(content)) {
            to_return.action_Type = action_type.null_action;
            to_return.length = to_return.action_tag.Length + 3;
            action_status(to_return);
            
            return to_return;
        }

        to_return.content = content;

        to_return.length = to_return.action_tag.Length + content.Length + 3;

        action_status(to_return);
        
        return to_return;

    }


    action parse_repeat(string repeat_string) {

        if (!repeat_string.Contains('[') || !repeat_string.Contains(']')) {
            gm.box.PrintD("repeat action was recognized with invalid syntax.");
            return new action{
                action_tag = "null",
                content = "();"
            };
        }

        prd("found repeat");

        action to_return = new action();

        to_return.action_tag = "repeat";

        to_return.action_Type = action_type.repeat;

        string condition = repeat_string.Split('(',2)[1];
        condition = condition.Split(')',2)[0];

        to_return.condition = condition;

        string command_without_condition = repeat_string.Split('[',2)[1];

        string to_repeat = gm.split_at(command_without_condition, gm.count_to_end(command_without_condition))[0];

        to_return.content_to_repeat = to_repeat;

        to_return.length = gm.count_to_end(repeat_string.Replace(to_repeat, to_repeat + "§"), 0, '¨', '§');

        action_status(to_return);
        

        return to_return;

    }

    action parse_if(string if_string) {

        if (!if_string.Contains('[') || !if_string.Contains(']')) {
            gm.box.PrintD("if action was recognized with invalid syntax.");
            return new action{
                action_tag = "null",
                content = "();"
            };
        }

        prd("found if");

        action to_return = new action();

        to_return.action_tag = "if";

        string condition = if_string.Split('(',2)[1];
        condition = condition.Split(")",2)[0];

        to_return.condition = condition;

        string if_true_and_if_false = if_string.Split('[',2)[1];

        string if_true = gm.split_at(if_true_and_if_false, gm.count_to_end(if_true_and_if_false))[0];

        to_return.if_true = if_true;

        if_true_and_if_false = gm.split_at(if_true_and_if_false, gm.count_to_end(if_true_and_if_false))[1];

        if (!if_true_and_if_false.Split('[',2)[0].Contains("?") && !if_true_and_if_false.Split('[',2)[0].Contains("else")) {

            prd("if statement without false");

            to_return.length = gm.count_to_end(if_string.Replace(if_true, if_true + "§"), 0, '¨', '§');

            action_status(to_return);
            
            to_return.action_Type = action_type.conditional;

            return to_return;
        }

        string if_false = if_true_and_if_false.Split("[",2)[1];

        if_false = gm.split_at(if_false, gm.count_to_end(if_false))[0];

        to_return.if_false = if_false;

        to_return.length = if_false.Length + if_true.Length + condition.Length;

        to_return.length = gm.count_to_end(if_string.Replace(if_false, if_false + "§"), 0, '¨', '§');
        
        action_status(to_return);
        
        to_return.action_Type = action_type.conditional;

        return to_return;
    }

    private void action_status(action ac) {

        if (gm.box.debug_print ==false) return;

        switch(ac.action_Type) {

            case action_type.regular:
                pr("action: " + ac.action_tag);
                gm.box.Print("content: " + ac.content);
                pr(ac.length.ToString());
            break;

            case action_type.null_action:
                gm.box.Print("action: " + ac.action_tag);
                pr(ac.length.ToString());
            break;

            case action_type.conditional:
                gm.box.Print("action: " + ac.action_tag);
                gm.box.Print("condition: " + ac.condition);
                gm.box.Print("if_true: " + ac.if_true);
                gm.box.Print("if_false: " + ac.if_false);
                gm.box.Print("length: " + ac.length.ToString());
            break;

            case action_type.repeat:
                pr("action" + ac.action_tag);
                pr("condition: " + ac.condition);
                pr("content to repeat: " + ac.content_to_repeat);
                pr("lenght: " + ac.length.ToString());
            break;

            case action_type.subroutine:
                prd("subrotine name: " + ac.routine_name);
                prd("subroutine: " + ac.sub_routine);
                prd("Lenght: " + ac.length);
            break;

            default:
                prd("Error in 'action_status(action ac)', invalid or missing action_type.");
            break;

        }

        gm.box.flush();
        gm.box.k.waitAnyKey();

    }

    

}