using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Shoy.Utility.Extend
{
    ///<summary>
    /// 扩展接口类
    ///</summary>
    ///<typeparam name="TV"></typeparam>
    public interface IExtension<TV>
    {
        ///<summary>
        /// 获取值
        ///</summary>
        ///<returns></returns>
        TV GetValue();
    }

    ///<summary>
    /// 字符扩展组
    ///</summary>
    public static class StringExtensionGroup
    {
        private static readonly IDictionary<Type, Type> Cache = new Dictionary<Type, Type>();
        private static readonly object LockObj = new object();

        /// <summary>
        /// 扩展转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T As<T>(this string s) where T:IExtension<string>
        {
            return As<T, string>(s);
        }

        /// <summary>
        /// 扩展转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        public static T As<T, TV>(this TV v) where T : IExtension<TV>
        {
            Type t;
            Type valueType = typeof (T);
            if (Cache.ContainsKey(valueType))
            {
                t = Cache[valueType];
            }
            else
            {
                lock (LockObj)
                {
                    if (!Cache.ContainsKey(valueType))
                    {
                        t = CreateType<T, TV>();
                        Cache.Add(valueType, t);
                    }
                    else
                    {
                        t = Cache[valueType];
                    }
                }
            }
            object result = Activator.CreateInstance(t, v);
            return (T) result;
        }

        private static Type CreateType<T, TV>() where T : IExtension<TV>
        {
            Type targetInterfaceType = typeof (T);
            string generatedClassName = targetInterfaceType.Name.Remove(0, 1);
            //
            var aName = new AssemblyName("ExtensionDynamicAssembly");
            AssemblyBuilder ab =
                AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            ModuleBuilder mb = ab.DefineDynamicModule(aName.Name);
            TypeBuilder tb = mb.DefineType(generatedClassName, TypeAttributes.Public);
            //实现接口
            tb.AddInterfaceImplementation(typeof (T));
            //value字段
            FieldBuilder valueFiled = tb.DefineField("value", typeof (TV), FieldAttributes.Private);
            //构造函数
            ConstructorBuilder ctor = tb.DefineConstructor(MethodAttributes.Public,
                                                           CallingConventions.Standard, new Type[] {typeof (TV)});
            ILGenerator ctor1Il = ctor.GetILGenerator();
            ctor1Il.Emit(OpCodes.Ldarg_0);
            ctor1Il.Emit(OpCodes.Call, typeof (object).GetConstructor(Type.EmptyTypes));
            ctor1Il.Emit(OpCodes.Ldarg_0);
            ctor1Il.Emit(OpCodes.Ldarg_1);
            ctor1Il.Emit(OpCodes.Stfld, valueFiled);
            ctor1Il.Emit(OpCodes.Ret);
            //GetValue方法
            MethodBuilder getValueMethod = tb.DefineMethod("GetValue",
                                                           MethodAttributes.Public | MethodAttributes.Virtual,
                                                           typeof (TV), Type.EmptyTypes);
            ILGenerator numberGetIl = getValueMethod.GetILGenerator();
            numberGetIl.Emit(OpCodes.Ldarg_0);
            numberGetIl.Emit(OpCodes.Ldfld, valueFiled);
            numberGetIl.Emit(OpCodes.Ret);
            //接口实现
            MethodInfo getValueInfo = targetInterfaceType.GetInterfaces()[0].GetMethod("GetValue");
            tb.DefineMethodOverride(getValueMethod, getValueInfo);
            //
            Type t = tb.CreateType();
            return t;
        }
    }
}
