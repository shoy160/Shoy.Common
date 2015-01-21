using System;
using Shoy.Utility.Extend;
using Shoy.AjaxHelper.Model;
using Shoy.AjaxHelper.Core;

namespace Shoy.AjaxHelper
{
    /// <summary>
    /// Ajax请求参数特征
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AjaxParameter : AttrBase
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Type ParaType { get; set; }
        public string RegexText { get; set; }
        private double _minValue = -1;
        public double MinValue { get { return _minValue; } set { _minValue = value; } }
        private double _maxValue = -1;
        public double MaxValue { get { return _maxValue; } set { _maxValue = value; } }
        private int _maxLength = -1;
        public int MaxLength { get { return _maxLength; } set { _maxLength = value; } }
        private int _minLength = -1;
        public int MinLength { get { return _minLength; } set { _minLength = value; } }
        public string ErrMsg { get; set; }
        public bool CanBeNull { get; set; }

        public AjaxParameter(string name, Type paraType)
        {
            Name = name;
            ParaType = paraType;
            Level = 200;
        }

        public override bool IsValidate()
        {
            if (base.IsValidate())
            {
                if (Name.IsNullOrEmpty() || ParaType == null)
                    throw new AjaxException("参数名/参数类型不能为空！");
                Value = CurrentHttpRequest.WebParameters[Name] ?? string.Empty;
                if (CheckData())
                    return true;
                if (ErrMsg.IsNotNullOrEmpty())
                    throw new AjaxException(ErrMsg);
                return false;
            }
            return false;
        }

        private bool CheckData()
        {
            ErrMsg = string.Empty;
            try
            {
                var result = ReflectionHelper.ChangeType(Value, ParaType);
                if (Value.IsNullOrEmpty() || result == null)
                {
                    if (!CanBeNull)
                        ErrMsg += "参数{0}不可为空！".FormatWith(Name);
                }
            }
            catch
            {
                ErrMsg += "参数{0}类型错误！".FormatWith(Name);
            }
            if (ErrMsg.IsNullOrEmpty())
            {
                if (RegexText.IsNotNullOrEmpty() && !Value.As<IRegex>().IsMatch(RegexText))
                {
                    ErrMsg += "参数{0}值不匹配！[{1}]".FormatWith(Name, RegexText);
                }
                if (_minLength >= 0 && Value.Length < _minLength)
                {
                    ErrMsg += "参数{0}小于最小长度{1}".FormatWith(Name, _minLength);
                }
                if (_maxLength >= 0 && Value.Length > _maxLength)
                {
                    ErrMsg += "参数{0}大于最大长度{1}".FormatWith(Name, _minLength);
                }
                if (_minValue > 0 && Value.As<IConvert>().ToFloat(0) < _minValue)
                {
                    ErrMsg += "参数{0}小于最小值{1}".FormatWith(Name, _minValue);
                }
                if (_maxValue > 0 && Value.As<IConvert>().ToFloat(-1) > _maxValue)
                {
                    ErrMsg += "参数{0}大于最大值{1}".FormatWith(Name, _maxValue);
                }
            }
            return ErrMsg.IsNullOrEmpty();
        }
    }
}
