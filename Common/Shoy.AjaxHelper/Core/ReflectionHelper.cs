using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shoy.AjaxHelper.Core
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    internal class ReflectionHelper
    {
        /// <summary>
        /// 获取该方法的特性列表 将会转换为指定的泛型列表
        /// </summary>
        /// <param name="methodInfo">方法的类型</param>
        /// <returns></returns>
        public static List<T> GetAttributes<T>(MethodInfo methodInfo) where T : class
        {
            //获取该方法全部的自定义属性
            object[] attrs = GetAttributes(methodInfo);
            var attrList = new List<T>();
            if (attrs != null && attrs.Length > 0)
            {
                //判断得到的特性是否可用
                attrList.AddRange(attrs.OfType<T>());
            }
            attrList.Sort();
            return attrList;
        }

        /// <summary>
        /// 获取该方法的特性列表 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static object[] GetAttributes(MethodInfo methodInfo)
        {
            object[] attrs = methodInfo.GetCustomAttributes(false);
            return attrs;
        }

        /// <summary>
        /// 得到类的实例类型
        /// </summary>
        /// <param name="assemlyString">程序集路径/命名空间</param>
        /// <param name="className">实例名称 不需要含程序集</param>
        /// <returns></returns>
        public static object GetClassType(string assemlyString, string className)
        {
            Type t = null;
            //加载指定程序集
            Assembly assembly = Assembly.Load(assemlyString);
            //得到该类的类型
            t = assembly.GetType(className);

            return t;
        }

        /// <summary>
        /// 得到方法实例
        /// </summary>
        /// <param name="assemlyString">程序集路径/命名空间</param>
        /// <param name="className">方法的实例</param>
        /// <param name="methodName">方法的名称</param>
        /// <param name="bindingAttr">该方法的条件</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(string assemlyString, string className, string methodName, BindingFlags bindingAttr)
        {
            MethodInfo methodInfo = null;
            //加载指定程序集
            Assembly assembly = Assembly.Load(assemlyString);
            //得到该类的类型
            Type t = assembly.GetType(className);
            //得到指定的方法
            methodInfo = t.GetMethod(methodName, bindingAttr);

            return methodInfo;
        }

        /// <summary>
        /// 得到方法的实例
        /// </summary>
        /// <param name="t">方法所在类的类型</param>
        /// <param name="methodName">方法的名称</param>
        /// <param name="bindingAttr">获取方法的过滤条件</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type t, string methodName, BindingFlags bindingAttr)
        {
            MethodInfo methodInfo = null;
            //得到指定的方法
            methodInfo = t.GetMethod(methodName, bindingAttr);

            return methodInfo;
        }

        public static object ChangeType(object obj, Type type)
        {
            object result;
            try
            {
                result = Convert.ChangeType(obj, type, null);
            }
            catch
            {
                result = (type.IsValueType ? Activator.CreateInstance(type) : null);
            }
            return result;
        }
    }
}
