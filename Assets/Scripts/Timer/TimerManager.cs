using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 时间单位枚举
/// </summary>
public enum TimeUnit
{
    Seconds,    // 秒
    Ticks,      // 游戏刻
    RealSeconds // 真实时间（不受游戏暂停影响）
}

/// <summary>
/// 定时任务数据类
/// </summary>
public class TimerTask
{
    public string Id { get; private set; }
    public List<Action> Callback { get; private set; }
    public float Interval { get; private set; }
    public TimeUnit Unit { get; private set; }
    public bool IsLoop { get; private set; }
    public object Owner { get; private set; }

    // 内部状态
    private float _elapsed;
    private int _ticksElapsed;
    private bool _isPaused;

    public TimerTask(string id, List<Action> callback, float interval, TimeUnit unit,
                    bool isLoop = false, object owner = null)
    {
        Id = id;
        Callback = callback;
        Interval = interval;
        Unit = unit;
        IsLoop = isLoop;
        Owner = owner;
        _elapsed = 0f;
        _ticksElapsed = 0;
        _isPaused = false;
    }

    /// <summary>
    /// 更新任务
    /// </summary>
    /// <returns>如果任务已完成且不需要循环，返回true</returns>
    public bool Update(float deltaTime, int ticksPerSecond)
    {
        if (_isPaused) return false;

        switch (Unit)
        {
            case TimeUnit.Seconds:
            case TimeUnit.RealSeconds:
                _elapsed += deltaTime;
                if (_elapsed >= Interval)
                {
                    foreach (Action callback in Callback)
                    {
                        callback?.Invoke();
                    }
                    if (IsLoop)
                    {
                        _elapsed = 0f;
                        return false;
                    }
                    return true;
                }
                break;

            case TimeUnit.Ticks:
                // 游戏刻需要根据ticksPerSecond转换为时间
                _elapsed += deltaTime;
                float tickDuration = 1f / ticksPerSecond;
                int newTicks = Mathf.FloorToInt(_elapsed / tickDuration);
                if (newTicks > _ticksElapsed)
                {
                    int ticksPassed = newTicks - _ticksElapsed;
                    _ticksElapsed = newTicks;

                    // 检查是否达到间隔
                    if (_ticksElapsed >= Interval)
                    {
                        foreach (Action callback in Callback)
                        {
                            callback?.Invoke();
                        }
                        if (IsLoop)
                        {
                            _ticksElapsed = 0;
                            _elapsed = 0f;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                break;
        }

        return false;
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }

    public void Reset()
    {
        _elapsed = 0f;
        _ticksElapsed = 0;
        _isPaused = false;
    }
}

public class TimerManager : BaseManager 
{
    private GameTimer timer;

    private Dictionary<string, TimerTask> _tasks = new Dictionary<string, TimerTask>();
    private List<string> _tasksToRemove = new List<string>();

    // 游戏刻速率（每秒多少刻）
    private int _ticksPerSecond = 6;

    // 游戏是否暂停
    private bool _isGamePaused = false;

    public int TicksPerSecond
    {
        get => _ticksPerSecond;
        set => _ticksPerSecond = Mathf.Max(1, value);
    }

    public bool IsGamePaused
    {
        get => _isGamePaused;
        set => _isGamePaused = value;
    }

    public TimerManager()
    {
        timer = new GameTimer();
    }

    public void Register(float time, System.Action callback)
    {
        timer.Register(time, callback);
    }

    /// <summary>
    /// 注册定时任务
    /// </summary>
    public string RegisterTask(string taskId, List<Action> callback, float interval,
                             TimeUnit unit = TimeUnit.Seconds, bool isLoop = false, object owner = null)
    {
        if (string.IsNullOrEmpty(taskId))
        {
            taskId = Guid.NewGuid().ToString();
        }

        if (_tasks.ContainsKey(taskId))
        {
            Debug.LogWarning($"任务ID已存在: {taskId}");
            return taskId;
        }

        var task = new TimerTask(taskId, callback, interval, unit, isLoop, owner);
        _tasks.Add(taskId, task);
        return taskId;
    }

    /// <summary>
    /// 移除定时任务
    /// </summary>
    public void RemoveTask(string taskId)
    {
        if (_tasks.ContainsKey(taskId))
        {
            _tasks.Remove(taskId);
        }
    }

    /// <summary>
    /// 移除所有属于指定对象的任务
    /// </summary>
    public void RemoveTasksByOwner(object owner)
    {
        foreach (var task in _tasks.Values)
        {
            if (task.Owner == owner)
            {
                _tasksToRemove.Add(task.Id);
            }
        }

        foreach (var taskId in _tasksToRemove)
        {
            _tasks.Remove(taskId);
        }
        _tasksToRemove.Clear();
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    public void PauseTask(string taskId)
    {
        if (_tasks.TryGetValue(taskId, out TimerTask task))
        {
            task.Pause();
        }
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    public void ResumeTask(string taskId)
    {
        if (_tasks.TryGetValue(taskId, out TimerTask task))
        {
            task.Resume();
        }
    }

    /// <summary>
    /// 重置任务
    /// </summary>
    public void ResetTask(string taskId)
    {
        if (_tasks.TryGetValue(taskId, out TimerTask task))
        {
            task.Reset();
        }
    }

    /// <summary>
    /// 检查任务是否存在
    /// </summary>
    public bool HasTask(string taskId)
    {
        return _tasks.ContainsKey(taskId);
    }

    public override void Update(float dt)
    {
        timer.OnUpdate(dt);
        if (_isGamePaused == true)
        {
            // 游戏暂停时，只更新RealSeconds单位的任务
            foreach (var task in _tasks.Values)
            {
                if (task.Unit == TimeUnit.RealSeconds)
                {
                    if (task.Update(dt, _ticksPerSecond))
                    {
                        _tasksToRemove.Add(task.Id);
                    }
                }
            }
        }
        else
        {
            // 正常更新所有任务
            foreach (var task in _tasks.Values)
            {
                if (task.Update(dt, _ticksPerSecond))
                {
                    _tasksToRemove.Add(task.Id);
                }
            }
        }

        // 移除已完成的任务
        foreach (var taskId in _tasksToRemove)
        {
            _tasks.Remove(taskId);
        }
        _tasksToRemove.Clear();
    }

    /// <summary>
    /// 清空所有任务
    /// </summary>
    public void ClearAllTasks()
    {
        _tasks.Clear();
    }
}
