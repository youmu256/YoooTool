﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoooTool.Code;
using YoooTool.Code.Slk;
using YoooTool.Code.Utils;

namespace YoooTool
{
    class Program
    {
        public class Parent
        {
            public string Name;
        }

        public class Child:Parent
        {
            
        }
        public class Room<T> where  T: Parent
        {
            public List<T> List = new List<T>();
        }

        public class PC_Test
        {
            public static void Test()
            {
                List<Room<Parent>> rooms = new List<Room<Parent>>();
                var room = new Room<Child>();
                //rooms.Add(room); how to make this
            }
        }

        public class Item
        {
            public string Name;
        }

        static void RandomPoolTest()
        {

            RandomWeightPool<Item> rp = new RandomWeightPool<Item>();
            rp.SetItemWeight(new Item() { Name = "5" }, 5);
            rp.SetItemWeight(new Item() { Name = "10" }, 10);
            Dictionary<Item, int> counter = new Dictionary<Item, int>();
            for (int i = 0; i < 100000; i++)
            {
                var it = rp.GetItem();
                if (!counter.ContainsKey(it))
                {
                    counter.Add(it, 0);
                }
                counter[it]++;
            }

            foreach (var p in counter)
            {
                Console.WriteLine(p.Key.Name + "__" + p.Value);
            }
            Console.ReadKey();
        }

        static void TypeCheck(object obj)
        {
            var memberType = obj.GetType();
            Type[] typeArguments = memberType.GetGenericArguments();
            if (obj is IList)
            {
                IList ilist = obj as IList;
                var ie = ilist.GetEnumerator();
                while (ie.MoveNext())
                {
                    Console.WriteLine(ie.Current);
                }
                /*
                foreach (var de in (tempvalue as ArrayList))
                {
                    var method = memberType.GetMethod("Add");
                    method.Invoke(obj, new object[] { de });
                }
                */
            }else if (obj is IDictionary)
            {
                IDictionary idct = obj as IDictionary;
                var ie = idct.GetEnumerator();
                while (ie.MoveNext())
                {
                    Console.WriteLine(ie.Entry.Key + "," + ie.Entry.Value);
                }
            }
        }

        static void TTT()
        {
            List<int> intList = new List<int>() {1,2,3,4,5};
            TypeCheck(intList);
            Dictionary<int,int> intDict = new Dictionary<int, int>()
            {
                {1,1 }, {2,2 }
            };
            TypeCheck(intDict);
            int[] irr = new[] {1, 2, 3};
            TypeCheck(irr);
            Console.ReadKey();
        }

        [STAThread]
        static void Main(string[] args)
        {
            SlkManager.CreateInstance();
            var export = new ExportHelper();
            //export.ExportLevel2Jass(Slk_Level.TestLevel);
            export.ExportAllLevel2Jass();
            //SlkManager.Instance.GetSlkData<SlkDataObject>("Room_1");
            /*
            var l = new LevelManager();
            l.TestInit();
            l.Export();
            Console.ReadKey();
            */
            //OpenLevelForm();
            Console.ReadKey();
            return;
            new SlkManager().OutPutTest();
            Console.ReadKey();
            return;
            var game = new Game();
            game.Init();
            game.StartRun();
            Console.ReadKey();
            return;

        }

        static void OpenLevelForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LevelEdit());
        }

    }
}
