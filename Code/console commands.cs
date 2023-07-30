
using Internal;
using KL.Console;
using KL.Utils;
using Game.Console;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Game.Data;
using Game;
using Game.Components;
using Game.Commands;
using System.Text.RegularExpressions;
using System;

namespace Game.Console
{
    public class ConsoleCommandCreate : BaseInGameConsoleCommand
    {
        public static GameState s;
        private int suggestion = 0;
        List<string> items = new List<string>();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void load() => ConsoleCommand.Register((ConsoleCommand)new ConsoleCommandCreate());
        protected override Dictionary<string, string> subCommands => new Dictionary<string, string>()
        {
            {"list", "list"},
            {"entity", "spawn"}
        };

        private void recalc(string input)
        {
            D.Warn("recalc triggered");
            items = new List<string>();
            Regex rule = new Regex(@input);
            foreach (var i in The.Defs.Defs)
            {
                if (!rule.IsMatch(i.Key)) continue;
                items.Add(i.Key);
            }
        }
        private string End(string text, IEnumerable<string> items){
            if(text != null || text != string.Empty){

            }
            return null;
        }
        public override string Autocomplete(ConsoleCommandArguments args)
        {
            if (args.HasArgument(2))
            {
                try{
                //error not going to next suggestion after creation list with matches by tabing 
                if (items.Count == 0)
                {
                    recalc(args.Arguments[2]);
                }
                D.Warn(items[suggestion]);
                if (Strings.Autocomplete(args.Arguments[2], items) == items[0] && !Regex .IsMatch(items[suggestion],args.Arguments[2] ))
                {
                    D.Warn("no match found");
                    recalc(args.Arguments[2]);
                    return args.Arguments[1] + " " + args.Arguments[2];
                }
                D.Warn(suggestion.ToString() + " "+ items.Count.ToString());
                if (suggestion >= items.Count-1) suggestion = 0;
                if (items[suggestion] == args.Arguments[2])
                {
                    suggestion++;
                    return args.Arguments[1] + " " + items[suggestion];
                }
                else
                {
                    recalc(args.Arguments[2]);
                    suggestion = 0;
                    return args.Arguments[1] + " " + items[suggestion];
                }
                } catch{
                    suggestion = 0;
                    recalc(args.Arguments[2]);
                }


            }
            if (args.HasArgument(1))
            {
                List<string> commands = new List<string>() { "list", "entity" };
                return Strings.Autocomplete(args.GetString(1), commands);
            }
            return "";

        }
        public override void Initialize()
        {
            this.Name = "create";
            this.HelpLine = "summon any creature or item in game by id\n list \n * return all entity that can be summoned in world\n entity {<Def> id, <int> stack, <vector2> {<int> x, <int> y} OR pos = mouse_pos }\n * used to summon any creature or object... whatever you creating in mod";
        }

        protected ConsoleCommandResult spawn(ConsoleCommandArguments args)
        {

            List<string> items = new List<string>();
            Vector2 pos;
            int length = 1;

            if (args.Arguments.Length < 3)
                return this.Error("create had not enought args!");
            string def = args.Arguments[2];

            if (The.Defs.Defs[def] == null)
                return this.Error("no such entity or not entity " + args.Arguments[2]);

            if (args.HasArgument(3))
                length = Convert.ToInt16(args.Arguments[3]);

            if (args.HasArgument(4) && args.HasArgument(5))
                pos = new Vector2((float)Convert.ToDouble(args.Arguments[4] + ".0"), (float)Convert.ToDouble(args.Arguments[5] + ".0"));
            else
                pos = The.Bindings.Mouse.WorldPosition;

            if (!def.Contains("Beings/"))
            {
                for (int i = 0; i < length; i++)
                {

                    The.R.State.Objs.Create(pos, The.Defs.Defs[def]);
                }
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    CmdSpawnBeing cmd =
                    new CmdSpawnBeing(pos, The.Defs.Defs[def], true);
                    cmd.Execute(The.R.State);
                    cmd = null;
                }

            }

            return this.OK("spawn command done");
        }
        protected ConsoleCommandResult list(ConsoleCommandArguments args)
        {
            string str = "";
            foreach (var item in The.Defs.Defs)
            {
                if (item.Value.Id.StartsWith("Obj/"))
                    str += item.Value.ParentId + " : " + item.Key.Replace("Obj/", "") + "\n";
            }
            D.Warn(str);
            return this.OK();
        }
    }
}
