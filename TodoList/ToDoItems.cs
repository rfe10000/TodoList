using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoList
{
    enum Status
    {
        Open = 0,
        Closed = 1
    }

    class ToDoItem
    {
        public ToDoItem()
        {
            ID = DateTime.Now.Ticks;
        }

        public ToDoItem(string title, string description, DateTime dueDate)
        {
            ID = DateTime.Now.Ticks;
            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = Status.Open;
        }

        public ToDoItem(string title, string description, DateTime dueDate, Status status)
        {
            ID = DateTime.Now.Ticks;
            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = status;
        }

        public long ID { get; init; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; }
    }

    class ToDoProject
    {
        public ToDoProject()
        {
            ID = DateTime.Now.Ticks;
            ToDo = new List<ToDoItem>();
        }

        //?
        public ToDoProject(string project, List<ToDoItem> toDo)
        {
            ID = DateTime.Now.Ticks;
            Project = project;
            ToDo = toDo;
        }

        public long ID { get; init; }
        public string Project { get; set; }
        public List<ToDoItem> ToDo { get; set; }

        public void AddToDo(ToDoItem todo)
        {
            ToDo.Add(todo);
        }

        public void AddToDo(string title, DateTime dueDate, Status status)
        {
            ToDo.Add(new ToDoItem(title, Project, dueDate, status));
        }

        public bool RemoveToDo(ToDoItem todo)
        {
            return ToDo.Remove(todo);
        }

        //?
        public bool UpdateToDo(ToDoItem todo)
        {
            return false;
        }
    }
}
