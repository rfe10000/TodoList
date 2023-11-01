using System.Data;
using System.Configuration;
using TodoList;

public partial class Program
{
    private static string sumary = @"You have {0} task todo and {1} are done";

    private static string optionMenue = string.Empty;
    private static string menueAlt1 = "Pick an option:\n(1) Show Task List (by date or project)\n(2) Add New Task\n" +
        "(3) Edit Task (update, mark as done, remove)\n(4) Save and Quit";

    private static string menueAlt2 = "Pick an option:\n(1) Add New Task\n(2) Save and Quit";  

    private static string dataSourceDir = @"C:\rf\lexicon\test-data\";
    private static string dataSource = string.Empty; 
    private static List<ToDoProject> list = null;


    private static bool HandleNewTask(bool exist = true)
    {
        string choice = string.Empty;
        if (exist)
        {
            PrintColoredMessage("Add to existing project (1), or a new project (2)", ConsoleColor.DarkYellow);
            choice = (Console.ReadLine() ?? string.Empty).Trim();
        }
        else
            choice = "2";

        if (choice.Equals("1"))
        {
            PrintColoredMessage("Select project number:", ConsoleColor.DarkYellow);

            for (int i = 0; i < list.Count; i++)
            {
                PrintColoredMessage((i + 1) + ". " + list[i].Project + "    ", onSingleLine: true);
            }
            Console.WriteLine();
            string prj = (Console.ReadLine() ?? string.Empty).Trim();
            bool ok = int.TryParse(prj, out int value);
            if (ok && (value - 1) < list.Count)
            {
                ToDoProject pr = list[value - 1];
                if (ManageTaskInput(pr))
                    PrintColoredMessage($"New task was added to [{pr.Project}]", ConsoleColor.Green);
            }
            else
                PrintColoredMessage("Not a valid choice", ConsoleColor.Red);
        }
        else if (choice.Equals("2"))
        {
            PrintColoredMessage("Enter a Project name: ", ConsoleColor.DarkYellow, onSingleLine: true);
            string projName = (Console.ReadLine() ?? string.Empty).Trim();
            if (!list.Where(p => p.Project == projName).Any())
            {
                do
                {                   
                    PrintColoredMessage($"Enter a Project name or quit (\"q\"): ", ConsoleColor.DarkYellow, true);
                    projName = (Console.ReadLine() ?? string.Empty).Trim();
                    if (projName.ToLower().Equals("q"))
                        return false;
                }
                while (projName.Equals(string.Empty));

                ToDoProject pr = new ToDoProject(projName, new List<ToDoItem>());
                if (ManageTaskInput(pr))
                {
                    list.Add(pr);
                    PrintColoredMessage($"New task was added to new project [{pr.Project}]", ConsoleColor.Green);
                }
                else
                    return false;
            }
            else
            {
                PrintColoredMessage("Project already exist", ConsoleColor.Red);
                PrintColoredMessage($"Add to existing project: {projName} (y/n): ", ConsoleColor.DarkYellow, onSingleLine: true);
                string opt = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
                if (opt.Equals("y"))
                {
                    ToDoProject pr = list.Where(p => p.Project == projName).First();
                    if (ManageTaskInput(pr))
                        PrintColoredMessage($"New task was added to [{pr.Project}]", ConsoleColor.Green);

                }
                //else if (opt.Equals("n"))
                //    PrintColoredMessage("Quiting", ConsoleColor.Red);
                else
                    PrintColoredMessage("Quiting", ConsoleColor.Red);
            }
        }
        else
            PrintColoredMessage("Not a valid choice", ConsoleColor.Red);
        return true;
    }
    private static bool ManageTaskInput(ToDoProject pr)
    {
        string quitOption = string.Empty;
        var stTpl = (Title: string.Empty, Description: string.Empty, DueDate: default(DateTime));

        while (true)
        {
            //måste ange en benämning
            if (stTpl.Title == string.Empty)
            {
                PrintColoredMessage($"Enter a task{quitOption}: ", ConsoleColor.DarkYellow, onSingleLine: true);
                string title = (Console.ReadLine() ?? string.Empty).Trim();
                if (title.ToLower() == "q")
                    return false;

                if (title == string.Empty)
                {
                    quitOption = ", or quit \"q\"";
                    continue;
                }
                stTpl.Title = title;
                quitOption = string.Empty;
            }

            PrintColoredMessage($"Enter a Description{quitOption}: ", ConsoleColor.DarkYellow, onSingleLine: true);
            string desc = (Console.ReadLine() ?? string.Empty).Trim();
            stTpl.Description = desc;

            PrintColoredMessage($"Enter a due date in the format yyyy-MM-dd (if empty todays date): ", ConsoleColor.DarkYellow, onSingleLine: true);
            string doDate = (Console.ReadLine() ?? string.Empty).Trim();

            if (DateTime.TryParse(doDate, out DateTime due))
                stTpl.DueDate = due;
            else
                stTpl.DueDate = DateTime.Today;

            pr.AddToDo(new ToDoItem(stTpl.Title, stTpl.Description, stTpl.DueDate));
            break;
        }
        return true;
    }

    private static(bool status, bool update, bool removed) HandleTaskUpdate()
    {
        (bool status, bool update, bool removed) tpl = (false, false, false);
        PrintColoredMessage("Select project [Number]:", ConsoleColor.DarkYellow);
        for (int i = 0; i < list.Count; i++)
        {
            PrintColoredMessage((i + 1) + ". " + list[i].Project + "    ", onSingleLine: true);
        }
        Console.WriteLine();
        string prj = (Console.ReadLine() ?? string.Empty).Trim();
        bool ok = int.TryParse(prj, out int value);
        if (ok && (value - 1) < list.Count)
        {
            ToDoProject pr = list[value - 1];
            tpl = ManageTaskUpdate(pr);
            if (tpl.status || tpl.update || tpl.removed)
                PrintColoredMessage($"Task for [{pr.Project}] was changed", ConsoleColor.Green);
        }
        else
            PrintColoredMessage("Not a valid choice", ConsoleColor.Red);

        return tpl;
    }

    private static(bool status, bool update, bool removed) ManageTaskUpdate(ToDoProject pr)
    {
        (bool status, bool update, bool removed) tpl = (false, false, false);
        PrintColoredMessage("Select todo to alter [Number]:", ConsoleColor.DarkYellow);
        for (int i = 0; i < pr.ToDo.Count; i++)
        {
            PrintColoredMessage((i + 1) + ". " + pr.ToDo[i].Title + "    ");
        }
        string prj = (Console.ReadLine() ?? string.Empty).Trim();
        bool ok = int.TryParse(prj, out int value);
        if (ok && (value - 1) < pr.ToDo.Count)
        {
            ToDoItem tdItm = pr.ToDo[value - 1];

            PrintColoredMessage($"Todo to change [{pr.ToDo[value - 1].Title}] for project [{pr.Project}]", ConsoleColor.Cyan);
            PrintColoredMessage("1. Mark as done   2. Update   3. Remove", ConsoleColor.DarkYellow);
            string operation = (Console.ReadLine() ?? string.Empty).Trim();
            if (operation.Equals("1"))
            {
                tdItm.Status = Status.Closed;
                tpl.status = true;
            }
            else if (operation.Equals("2"))
            {
                PrintColoredMessage("Change description: ", ConsoleColor.DarkYellow, onSingleLine: true);
                string description = tdItm.Description ?? string.Empty;

                if (!String.IsNullOrEmpty(description))
                    tdItm.Description = ToDoUtils.EditString(description);
                else
                    tdItm.Description = (Console.ReadLine() ?? string.Empty).Trim();

                if (!description.Equals(tdItm.Description))
                    tpl.update = true;

                PrintColoredMessage("Change due date (yyyy-MM-dd), if empty no update: ", ConsoleColor.DarkYellow, onSingleLine: true);
                string doDate = (Console.ReadLine() ?? string.Empty).Trim();

                if (DateTime.TryParse(doDate, out DateTime due))
                {
                    tdItm.DueDate = due;
                    tpl.update = true;
                }
            }
            else if (operation.Equals("3"))
            {
                pr.RemoveToDo(tdItm);
                tpl.removed = true;
            }
        }
        return tpl;
    }

    private static(int openCount, int doneCount) GetCounts(List<ToDoProject> list)
    {
        int antalOpen = 0;
        int antalClosed = 0;

        try
        {
            var todoCounting =
                from todoItems in list
                where todoItems.ToDo != null
                select todoItems.ToDo into internalToDo
                from item in internalToDo
                group item by item.Status into countStatus
                select new { stat = countStatus.Key, num = countStatus.Count() };


            foreach (var cnt in todoCounting)
            {
                if (cnt.stat == Status.Open)
                    antalOpen = cnt.num;
                else if (cnt.stat == Status.Closed)
                    antalClosed = cnt.num;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return (antalOpen, antalClosed);
    }

    private static void PrintColoredMessage(string msg, ConsoleColor color = ConsoleColor.White, bool onSingleLine = false)
    {
        if (color != ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            if (onSingleLine)
                Console.Write(msg);
            else
                Console.WriteLine(msg);
            Console.ResetColor();
        }
        else
        {
            if (onSingleLine)
                Console.Write(msg);
            else
                Console.WriteLine(msg);
        }
    }

    private static void ToDoByDate(List<ToDoProject> list)
    {
        //Todo efter datum med projectnamn
        var todoByDate =
            from todoItems in list
            where todoItems.ToDo != null
            from todos in todoItems.ToDo
            orderby todos.DueDate descending
            select new
            {
                Project = todoItems.Project,
                todos
            };
        if (todoByDate.Count() > 0)
        {
            PrintColoredMessage($"{"Date".PadRight(10)}{" ".PadRight(3)}{"Project".PadRight(12)}{"Title".PadRight(30)}{"Status"}{" ".PadRight(6)}Descrition", ConsoleColor.Green);
            foreach (var pr in todoByDate)
            {
                PrintColoredMessage($"{pr.todos.DueDate:yyyy-MM-dd}{" ".PadRight(3)}{pr.Project.PadRight(12)}{pr.todos.Title.PadRight(30)}{pr.todos.Status}{" ".PadRight(8)}{pr.todos.Description}");
            }
        }
        else
            PrintColoredMessage("Nothing to show", ConsoleColor.Cyan);
    }

    private static void ToDoByProject(List<ToDoProject> list)
    {
        var todoByProject =
            from todoItems in list
            where todoItems.ToDo != null
            from todos in todoItems.ToDo
            orderby todoItems.Project, todos.DueDate descending
            select new
            {
                Project = todoItems.Project,
                todos
            };

        if (todoByProject.Count() > 0)
        {
            PrintColoredMessage($"{"Project".PadRight(12)}{"Title".PadRight(30)}{"Date".PadRight(10)}{" ".PadRight(3)}{"Status"}{" ".PadRight(6)}Descrition", ConsoleColor.Green);
            foreach (var tdP in todoByProject)
            {
                PrintColoredMessage($"{tdP.Project.PadRight(12)}{tdP.todos.Title.PadRight(30)}{tdP.todos.DueDate:yyyy-MM-dd}{" ".PadRight(3)}{tdP.todos.Status}{" ".PadRight(8)}{tdP.todos.Description}");
            }
        }
        else
            PrintColoredMessage("Nothing to show", ConsoleColor.Cyan);
    }

    private static void SaveAndExit(string dataSourceDir, List<ToDoProject> list, (bool status, bool update, bool removed) tplChanged, bool taskAdded)
    {
        if (!taskAdded && !tplChanged.status && !tplChanged.update && !tplChanged.removed)
            PrintColoredMessage("Exiting", ConsoleColor.Cyan);
        else
        {
            

            if (dataSource.Equals(string.Empty))
            {
                bool nameNotGiven = true;
                do
                {
                    PrintColoredMessage($"Give a datasource name{(!nameNotGiven? ", or quit \"q\"" : string.Empty)}: ", ConsoleColor.DarkYellow, true);                
                    dataSource = (Console.ReadLine() ?? string.Empty).Trim();
                    nameNotGiven = false;
                    if (dataSource.ToLower().Equals("q"))
                    {
                        PrintColoredMessage("Exiting", ConsoleColor.Cyan);
                        return;
                    }
                }
                while (dataSource.Equals(string.Empty));
                //Todo kolla filändelsen
                PrintColoredMessage("Saving and exiting", ConsoleColor.Cyan);
            }
            else
                PrintColoredMessage("Saving and exiting", ConsoleColor.Cyan);

            ToDoUtils.SerializeListToFile(list, dataSourceDir + dataSource);
        }
    }
}