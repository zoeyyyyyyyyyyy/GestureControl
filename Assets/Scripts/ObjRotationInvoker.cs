using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum RotationDirection
{
    Left,
    Right,
    Up,
    Down,
}

public class ObjRotation
{
    private static ObjRotation _instacne = new ObjRotation();

    //移动距离
    private const float MoveDistance = 100f;

    public static ObjRotation Instacne => _instacne;

    private ObjRotation()
    {
    }

    //移动
    public void Rotation(Transform _target, RotationDirection direction)
    {
        var eulerAngles = _target.eulerAngles;
        eulerAngles = direction switch
        {
            RotationDirection.Left => new Vector3(eulerAngles.x, eulerAngles.y - 90, eulerAngles.z),
            RotationDirection.Right => new Vector3(eulerAngles.x, eulerAngles.y + 90, eulerAngles.z),
            RotationDirection.Up => new Vector3(eulerAngles.x - 90, eulerAngles.y, eulerAngles.z),
            RotationDirection.Down => new Vector3(eulerAngles.x - 90, eulerAngles.y, eulerAngles.z),
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };
        _target.DORotate(eulerAngles, 0.3f).OnComplete(() => { _target.eulerAngles = eulerAngles; });
    }
}

public interface ICommand
{
    // 执行
    void Execute();

    // 撤销
    void Undo();

    // 命令转换字符串
    string ToString();
}

public class RotationCommand : ICommand
{
    //保持唯一的一份
    private readonly ObjRotation _receiver;
    private readonly Transform _transform;
    private readonly RotationDirection _direction;

    public RotationCommand(RotationDirection direction, Transform transform)
    {
        _direction = direction;
        _transform = transform;
        _receiver = ObjRotation.Instacne; //单例实例
    }

    public void Execute() => _receiver.Rotation(_transform, _direction);

    public void Undo()
    {
        switch (_direction)
        {
            case RotationDirection.Left:
                _receiver.Rotation(_transform, RotationDirection.Right);
                break;
            case RotationDirection.Right:
                _receiver.Rotation(_transform, RotationDirection.Left);
                break;
            case RotationDirection.Up:
                _receiver.Rotation(_transform, RotationDirection.Down);
                break;
            case RotationDirection.Down:
                _receiver.Rotation(_transform, RotationDirection.Up);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_direction));
        }
    }

    public override string ToString()
    {
        return _direction.ToString();
    }
}

public class ObjRotationInvoker
{
    private List<ICommand> _commands;
    private int _index = -1;

    public int Index => _index;
    public List<ICommand> CommandList => _commands;

    public ObjRotationInvoker()
    {
        _commands = new List<ICommand>();
    }

    //执行命令
    public void Rotation(ICommand command)
    {
        command.Execute();

        //如果命令不是最后一个，那就删除后面所有命令
        if (_index < _commands.Count - 1)
        {
            _commands.RemoveRange(_index + 1, _commands.Count - _index - 1);
        }

        _commands.Add(command);
        _index++;
    }

    public void Clear()
    {
        if (_commands != null && _commands.Count > 0) _commands.Clear();
    }

    //恢复操作
    public void ReDo()
    {
        if (_index >= _commands.Count - 1) return;

        _index++;
        _commands[_index].Execute();
    }

    //撤销操作 
    public void UnDo()
    {
        if (_index < 0) return;

        _commands[_index].Undo();
        _index--;
    }
}