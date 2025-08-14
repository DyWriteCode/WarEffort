using System;
using UnityEngine;

/// <summary>
/// 污染管理系统，负责管理游戏中的污染值
/// </summary>
public class PollutionManager : BaseManager
{
    [SerializeField] private float _globalPollution = 0;
    [SerializeField] private float _maxPollution = 0;
    [SerializeField] private float _pollutionWarningThreshold = 0;

    public PollutionManager(float _globalPollution, float _maxPollution, float _pollutionMagnification) 
    {
        this._globalPollution = _globalPollution;
        this._maxPollution = _maxPollution;
        _pollutionWarningThreshold = _maxPollution * _pollutionMagnification;
    }

    /// <summary>
    /// 当前污染值
    /// </summary>
    public float GlobalPollution => _globalPollution;

    /// <summary>
    /// 最大污染值
    /// </summary>
    public float MaxPollution => _maxPollution;

    /// <summary>
    /// 污染百分比（0-1）
    /// </summary>
    public float PollutionPercentage => _globalPollution / _maxPollution;

    /// <summary>
    /// 是否达到污染警告阈值
    /// </summary>
    public bool IsPollutionWarning => _globalPollution >= _pollutionWarningThreshold;

    /// <summary>
    /// 污染度变化事件
    /// </summary>
    public event Action<float> OnPollutionChanged;

    /// <summary>
    /// 增加污染值
    /// </summary>
    /// <param name="amount">增加的污染量</param>
    public void AddPollution(float amount)
    {
        // 确保污染值不超过上限
        _globalPollution = Mathf.Min(_maxPollution, _globalPollution + amount);

        // 触发污染变化事件
        OnPollutionChanged?.Invoke(_globalPollution);

        // 达到警告阈值时触发警告
        if (_globalPollution >= _pollutionWarningThreshold)
        {
            Debug.LogWarning($"污染度警告: {_globalPollution}/{_maxPollution}");
        }
    }

    /// <summary>
    /// 减少污染值
    /// </summary>
    /// <param name="amount">减少的污染量</param>
    public void ReducePollution(float amount)
    {
        // 确保污染值不低于0
        _globalPollution = Mathf.Max(0, _globalPollution - amount);

        // 触发污染变化事件
        OnPollutionChanged?.Invoke(_globalPollution);
    }

    /// <summary>
    /// 设置污染警告阈值
    /// </summary>
    /// <param name="threshold">新的警告阈值（0-1之间的百分比）</param>
    public void SetWarningThreshold(float threshold)
    {
        // 确保阈值在合理范围内
        _pollutionWarningThreshold = Mathf.Clamp(threshold, 0f, 1f) * _maxPollution;
    }

    /// <summary>
    /// 重置污染值
    /// </summary>
    public void ResetPollution()
    {
        _globalPollution = 0f;
        OnPollutionChanged?.Invoke(0f);
    }

    /// <summary>
    /// 设置最大污染值
    /// </summary>
    /// <param name="maxPollution">新的最大污染值</param>
    public void SetMaxPollution(float maxPollution)
    {
        _maxPollution = Mathf.Max(1, maxPollution);

        // 确保当前污染值不超过新的上限
        _globalPollution = Mathf.Min(_globalPollution, _maxPollution);

        // 更新警告阈值（保持原有百分比）
        float warningPercentage = _pollutionWarningThreshold / _maxPollution;
        _pollutionWarningThreshold = warningPercentage * _maxPollution;

        OnPollutionChanged?.Invoke(_globalPollution);
    }
}