﻿using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Ididit.Data.Models;

public class GoalModel
{
    static readonly MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak().Build();

    [JsonIgnore]
    internal long Id { get; init; }
    [JsonIgnore]
    internal long CategoryId { get; set; }

    public long? PreviousId { get; set; }

    public string Name { get; set; } = string.Empty;

    private string _details = string.Empty;
    public string Details
    {
        get => _details;
        set
        {
            _details = value;

            if (!CreateTaskFromEachLine)
                DetailsMarkdownHtml = Markdown.ToHtml(_details, _markdownPipeline).Replace("<p>", "<div>").Replace("</p>", "</div>");
        }
    }

    [JsonIgnore]
    internal string DetailsMarkdownHtml { get; set; } = string.Empty;

    [JsonIgnore]
    internal int Rows => Details.Count(c => c == '\n') + 1;

    public bool CreateTaskFromEachLine { get; set; }

    public List<TaskModel> TaskList = new();

    public TaskModel CreateTask(long id, string name)
    {
        return CreateTask(id, name, TimeSpan.Zero, Priority.Medium, TaskKind.Task);
    }

    public TaskModel CreateTask(long id, string name, TimeSpan desiredInterval, Priority priority, TaskKind taskKind)
    {
        TaskModel task = new()
        {
            Id = id,
            GoalId = Id,
            PreviousId = TaskList.Any() ? TaskList.Last().Id : null,
            Name = name,
            CreatedAt = DateTime.Now,
            DesiredInterval = desiredInterval,
            Priority = priority,
            TaskKind = taskKind
        };

        TaskList.Add(task);

        return task;
    }

    public (TaskModel newTask, TaskModel? changedTask) CreateTaskAt(long id, int index)
    {
        TaskModel? changedTask = null;

        TaskModel task = new()
        {
            Id = id,
            GoalId = Id,
            PreviousId = TaskList.Any() ? TaskList.Last().Id : null,
            CreatedAt = DateTime.Now,
            DesiredInterval = TimeSpan.Zero,
            Priority = Priority.Medium,
            TaskKind = TaskKind.Task
        };

        if (index > 0)
            task.PreviousId = TaskList[index - 1].Id;
        else
            task.PreviousId = null;

        if (index < TaskList.Count)
        {
            changedTask = TaskList[index];
            changedTask.PreviousId = task.Id;
        }

        TaskList.Insert(index, task);

        return (task, changedTask);
    }

    public TaskModel? RemoveTask(TaskModel task)
    {
        TaskModel? changedTask = null;

        int index = TaskList.IndexOf(task);

        if (index < TaskList.Count - 1)
        {
            changedTask = TaskList[index + 1];

            if (index > 0)
                changedTask.PreviousId = TaskList[index - 1].Id;
            else
                changedTask.PreviousId = null;
        }

        TaskList.Remove(task);

        return changedTask;
    }

    public void OrderTasks()
    {
        List<TaskModel> taskList = new();
        long? previousId = null;

        while (TaskList.Any())
        {
            TaskModel task = TaskList.Single(t => t.PreviousId == previousId);
            previousId = task.Id;
            taskList.Add(task);
            TaskList.Remove(task);
        }

        TaskList = taskList;
    }

    public IEnumerable<TaskModel> GetFilteredTasks(Filters filters)
    {
        IEnumerable<TaskModel> tasks = TaskList.Where(task =>
        {
            bool isRatioOk = task.ElapsedToDesiredRatio >= filters.ElapsedToDesiredRatioMin;

            bool isNameOk = string.IsNullOrEmpty(filters.SearchFilter) || task.Name.Contains(filters.SearchFilter, StringComparison.OrdinalIgnoreCase);

            bool isDateOk = filters.DateFilter == null || task.TimeList.Any(time => time.Date == filters.DateFilter?.Date);

            bool isPriorityOk = filters.ShowPriority[task.Priority];

            bool isTaskKindOk = filters.ShowTaskKind[task.TaskKind];

            return isNameOk && isDateOk && isPriorityOk && isTaskKindOk &&
                (isRatioOk || !filters.ShowElapsedToDesiredRatioOverMin) &&
                (!task.IsCompleted || !filters.HideCompletedTasks);
        });

        return GetSortedTasks(tasks, filters);
    }

    private static IEnumerable<TaskModel> GetSortedTasks(IEnumerable<TaskModel> tasks, Filters filters)
    {
        return filters.Sort switch
        {
            Sort.None => tasks,
            Sort.Name => tasks.OrderBy(task => task.Name),
            Sort.Priority => tasks.OrderByDescending(task => task.Priority),
            Sort.ElapsedTime => tasks.OrderByDescending(task => task.ElapsedTime),
            Sort.ElapsedToAverageRatio => tasks.OrderByDescending(task => task.ElapsedToAverageRatio),
            Sort.ElapsedToDesiredRatio => tasks.OrderByDescending(task => task.ElapsedToDesiredRatio),
            Sort.AverageToDesiredRatio => tasks.OrderByDescending(task => task.AverageToDesiredRatio),
            _ => throw new ArgumentException("Invalid argument: " + nameof(filters.Sort))
        };
    }
}
