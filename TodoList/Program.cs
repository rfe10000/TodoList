using System.Text.Json;
using System;
using TodoList;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics.SymbolStore;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using Microsoft.VisualBasic.FileIO;
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic;
using System.IO.Enumeration;
using System.Reflection;


(int open, int closed) tplCounts = (0, 0);
string dataSourceOption = string.Empty;
bool menuIsFirst = true;
try
{
    PrintColoredMessage("Welcom to ToDo", ConsoleColor.Green);

    Dictionary<string, string> fileNames = ToDoUtils.GetDataSource(dataSourceDir);
    if (fileNames.Count() > 0)
    {
        PrintColoredMessage("Select data source [Number]: 1.Existing  2.New: ", ConsoleColor.DarkYellow, true);        

        do
        {
            dataSourceOption = (Console.ReadLine() ?? string.Empty).Trim();
            if (dataSourceOption.Equals("1"))
            {
                optionMenue = menueAlt1;
                foreach (var result in fileNames)
                {
                    PrintColoredMessage($"{result.Key}. {result.Value}");
                }
                string sourceSelected = (Console.ReadLine() ?? string.Empty).Trim();
                dataSource = fileNames[sourceSelected];

                list = ToDoUtils.ParseToDoInfo(dataSourceDir + dataSource);

                tplCounts = GetCounts(list);
                if (tplCounts.open + tplCounts.closed == 0)
                {
                    optionMenue = menueAlt2;
                    menuIsFirst = false;
                    PrintColoredMessage(String.Format(sumary, tplCounts.open, tplCounts.closed), ConsoleColor.Cyan);
                }
                else
                    PrintColoredMessage(String.Format(sumary, tplCounts.open, tplCounts.closed), ConsoleColor.Cyan);
                break;
            }
            else if (dataSourceOption.Equals("2"))
            {
                menuIsFirst = false;
                optionMenue = menueAlt2;
                list = new List<ToDoProject>();
                break;
                //skapa ny fil 
            }
            else
            {
                if (dataSourceOption.ToLower().Equals("q"))
                    throw new Exception("Quiting");                
                
                PrintColoredMessage("Not a valid choice (q) to quit", ConsoleColor.Red);
            }
        }
        while (true);
    }
    else
    {
        menuIsFirst = false;
        optionMenue = menueAlt2;
        list = new List<ToDoProject>();
    }

    PrintColoredMessage(optionMenue, ConsoleColor.Blue);

    (bool status, bool update, bool removed) tplChanged = (false, false, false);
    bool taskAdded = false;
    while (true)
    {
        bool noRecount = false;
        string choice = (Console.ReadLine() ?? string.Empty).Trim();
        if (menuIsFirst)
        {
            if (choice.Equals("1"))
            {
                PrintColoredMessage("1. Show by project   2. Show by date", ConsoleColor.DarkYellow);
                string opt = (Console.ReadLine() ?? string.Empty).Trim();
                if (opt.Equals("1"))
                {
                    ToDoByProject(list);
                }
                else if (opt.Equals("2"))
                {
                    ToDoByDate(list);
                }
                else
                    PrintColoredMessage("Not a valid choice", ConsoleColor.Red);
                noRecount = true;
            }
            else if (choice.Equals("2"))
            {
                if (tplCounts.open + tplCounts.closed == 0)
                    taskAdded = HandleNewTask(false);
                else
                    taskAdded = HandleNewTask();
            }
            else if (choice.Equals("3"))
            {
                tplChanged = HandleTaskUpdate();
                noRecount = tplChanged.update == true; //om description eller datum ändrats behövs ingen "recount"
            }
            else if (choice.Equals("4"))
            {
                SaveAndExit(dataSourceDir, list, tplChanged, taskAdded);
                break;
            }
            else
            {
                PrintColoredMessage("Not a valid choice", ConsoleColor.Red);
            }
            Console.WriteLine();
        }
        else
        {
            if (choice.Equals("1"))
            {
                if (HandleNewTask(false))
                {
                    optionMenue = menueAlt1;
                    menuIsFirst = true;
                }
            }
            else if (choice.Equals("2"))
            {
                //spara tomt
                SaveAndExit(dataSourceDir, list, tplChanged, taskAdded);
                break;
            }
            Console.WriteLine();
        }
        if (!noRecount)
            tplCounts = GetCounts(list);
        PrintColoredMessage(String.Format(sumary, tplCounts.open, tplCounts.closed), ConsoleColor.Cyan);
        PrintColoredMessage(optionMenue, ConsoleColor.Blue);
    }
}
catch (ToDoUtils.ToDoDataIsCorupptedException tdex)
{
    PrintColoredMessage(tdex.Message, ConsoleColor.DarkRed);
}
catch (Exception ex)
{
    PrintColoredMessage(ex.Message, ConsoleColor.DarkRed);
}

