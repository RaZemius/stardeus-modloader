
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
using System;

namespace Game.Console
{
    public class ConsoleCommandCreate : BaseInGameConsoleCommand
    {
        public static GameState s;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void load() => ConsoleCommand.Register((ConsoleCommand)new ConsoleCommandCreate());
        protected override Dictionary<string, string> subCommands => new Dictionary<string, string>()
        {
            {"list", "list"},
            {"entity", "spawn"}
        };

        public override string Autocomplete(ConsoleCommandArguments args)
        {
            if (args.HasArgument(2))
            {
                List<string> items = new List<string>() { "" };
                foreach (var i in The.Defs.Defs)
                {
                    items.Add(i.Key);
                }
                return args.GetString(1) + " " + Strings.Autocomplete(args.GetString(2), items);
            }
            if (args.HasArgument(1))
            {
                List<string> items = new List<string>() { "list", "entity" };
                return Strings.Autocomplete(args.GetString(1), items);
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
            Vector2 pos;
            int length = 1;
            string def = args.Arguments[2];

            if (args.Arguments.Length < 3)
                return this.Error("create had not enought args!");

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
                    new CmdSpawnBeing(pos, The.Defs.Defs[def], true).Execute(The.R.State);
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
