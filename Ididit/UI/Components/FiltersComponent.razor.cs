﻿using Ididit.App;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Ididit.UI.Components;

public partial class FiltersComponent
{
    [Inject]
    IRepository Repository { get; set; } = null!;

    [CascadingParameter]
    Blazorise.Size Size { get; set; }

    [Parameter]
    public Filters Filters { get; set; } = null!;

    [Parameter]
    public EventCallback<Filters> FiltersChanged { get; set; }

    string _searchFilter = string.Empty;

    async Task SearchFilterChanged(string searchFilter)
    {
        _searchFilter = searchFilter;
        Filters.SearchFilter = searchFilter;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearSearchFilter()
    {
        _searchFilter = string.Empty;
        Filters.SearchFilter = string.Empty;
        await FiltersChanged.InvokeAsync(Filters);
    }

    DateTime? _dateFilter;

    bool? IsTodayChecked => _dateFilter == DateTime.Now.Date;

    async Task TodayCheckedChanged(bool? isToday)
    {
        _dateFilter = isToday == true ? DateTime.Now.Date : null;
        Filters.DateFilter = isToday == true ? DateTime.Now.Date : null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task OnDateChanged(DateTime? dateTime)
    {
        _dateFilter = dateTime;
        Filters.DateFilter = dateTime;
        await FiltersChanged.InvokeAsync(Filters);
    }

    async Task ClearDateFilter()
    {
        _dateFilter = null;
        Filters.DateFilter = null;
        await FiltersChanged.InvokeAsync(Filters);
    }

    bool GetShowPriority(Priority priority)
    {
        Filters.ShowPriority[priority] = Repository.Settings.ShowPriority[priority];
        return Repository.Settings.ShowPriority[priority];
    }

    async Task OnShowPriorityChanged(Priority priority, bool show)
    {
        Filters.ShowPriority[priority] = show;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.ShowPriority[priority] = show;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    bool GetShowTaskKind(TaskKind taskKind)
    {
        Filters.ShowTaskKind[taskKind] = Repository.Settings.ShowTaskKind[taskKind];
        return Repository.Settings.ShowTaskKind[taskKind];
    }

    async Task OnShowTaskKindChanged(TaskKind taskKind, bool show)
    {
        Filters.ShowTaskKind[taskKind] = show;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.ShowTaskKind[taskKind] = show;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    Sort Sort
    {
        get
        {
            Filters.Sort = Repository.Settings.Sort;
            return Repository.Settings.Sort;
        }
    }

    long ElapsedToDesiredRatioMin
    {
        get
        {
            Filters.ElapsedToDesiredRatioMin = Repository.Settings.ElapsedToDesiredRatioMin;
            return Repository.Settings.ElapsedToDesiredRatioMin;
        }
    }

    bool ShowElapsedToDesiredRatioOverMin
    {
        get
        {
            Filters.ShowElapsedToDesiredRatioOverMin = Repository.Settings.ShowElapsedToDesiredRatioOverMin;
            return Repository.Settings.ShowElapsedToDesiredRatioOverMin;
        }
    }

    bool HideEmptyGoals
    {
        get
        {
            Filters.HideEmptyGoals = Repository.Settings.HideEmptyGoals;
            return Repository.Settings.HideEmptyGoals;
        }
    }

    bool ShowCategoriesInGoalList
    {
        get
        {
            Filters.ShowCategoriesInGoalList = Repository.Settings.ShowCategoriesInGoalList;
            return Repository.Settings.ShowCategoriesInGoalList;
        }
    }

    bool HideCompletedTasks
    {
        get
        {
            Filters.HideCompletedTasks = Repository.Settings.HideCompletedTasks;
            return Repository.Settings.HideCompletedTasks;
        }
    }

    async Task OnSortChanged(Sort sort)
    {
        Filters.Sort = sort;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.Sort = sort;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnHideEmptyGoalsChanged(bool? val)
    {
        Filters.HideEmptyGoals = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.HideEmptyGoals = val ?? false;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnShowCategoriesInGoalListChanged(bool? val)
    {
        Filters.ShowCategoriesInGoalList = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.ShowCategoriesInGoalList = val ?? false;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnHideCompletedTasksChanged(bool? val)
    {
        Filters.HideCompletedTasks = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.HideCompletedTasks = val ?? false;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnShowElapsedToDesiredRatioOverMinChanged(bool? val)
    {
        Filters.ShowElapsedToDesiredRatioOverMin = val ?? false;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.ShowElapsedToDesiredRatioOverMin = val ?? false;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }

    async Task OnElapsedToDesiredRatioMinChanged(long val)
    {
        Filters.ElapsedToDesiredRatioMin = val;
        await FiltersChanged.InvokeAsync(Filters);

        Repository.Settings.ElapsedToDesiredRatioMin = val;
        await Repository.UpdateSettings(Repository.Settings.Id);
    }
}
