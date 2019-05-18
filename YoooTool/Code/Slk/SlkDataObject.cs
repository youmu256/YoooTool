﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YoooTool.Code.Utils;

namespace YoooTool.Code.Slk
{

    public abstract class SlkDataObject : ISlkSerialize, IExport2Jass
    {
        public SlkDataObject() { }
        /// <summary>
        /// 索引ID
        /// </summary>
        [SlkProperty(-1)]
        public string Id { get; set; }

        public static string GetProperty2Csv(Type t)
        {
            StringBuilder sb = new StringBuilder();
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //无标签的无效
            properties.RemoveAll((info => info.GetCustomAttribute<SlkPropertyAttribute>() == null));

            properties.Sort((p1, p2) =>
            {
                var p1Index = p1.GetCustomAttribute<SlkPropertyAttribute>();
                var p2Index = p2.GetCustomAttribute<SlkPropertyAttribute>();
                var p1i = p1Index != null ? p1Index.Index : int.MaxValue;
                var p2i = p2Index != null ? p2Index.Index : int.MaxValue;
                return p1i - p2i;
            });

            bool isFirst = true;
            foreach (var fieldInfo in properties)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(string.Format("{0}", fieldInfo.Name));
            }
            return sb.ToString();
        }
        /// <summary>
        /// SLK Serialize主要使用的方法
        /// </summary>
        /// <returns></returns>
        public string GetProperty2Csv()
        {
            StringBuilder sb = new StringBuilder();
            var properties = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            //无标签的无效
            properties.RemoveAll((info => info.GetCustomAttribute<SlkPropertyAttribute>() == null));
            properties.Sort((p1, p2) =>
            {
                var p1Index = p1.GetCustomAttribute<SlkPropertyAttribute>();
                var p2Index = p2.GetCustomAttribute<SlkPropertyAttribute>();
                var p1i = p1Index != null ? p1Index.Index : int.MaxValue;
                var p2i = p2Index != null ? p2Index.Index : int.MaxValue;
                return p1i - p2i;
            });
            bool isFirst = true;
            foreach (var fieldInfo in properties)
            {
                if (!isFirst)
                {
                    sb.Append(",");
                }
                else
                {
                    isFirst = false;
                }
                sb.Append(string.Format("{0}", GetPropertyValueConfig(fieldInfo, this)));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 决定转换
        /// </summary>
        /// <param name="info"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetPropertyValueConfig(PropertyInfo info, object obj)
        {
            var type = info.PropertyType;
            var value = info.GetValue(obj);
            if (type.IsClass)
            {
                object infoObj = info.GetValue(obj);
                if (infoObj is IList)
                {
                    Type[] typeArguments = type.GetGenericArguments();
                    var slkType = typeArguments[0];
                    if (slkType.IsSubclassOf(typeof(SlkDataObject)))
                    {
                        IList ilist = infoObj as IList;
                        return SlkParseUtil.SlkList2IdList(ilist);
                    }
                }

                if (type == typeof(List<string>))//ID 池
                {
                    return SlkParseUtil.IdList2Config((List<string>)value);
                }
                if (type == typeof(RandomWeightPool<string>))//ID 池
                {
                    return SlkParseUtil.IdPool2Config((RandomWeightPool<string>)value);
                }
                if (type.IsSubclassOf(typeof(SlkDataObject)))
                {
                    //--引用SLKData對象就返回ID--
                    return ((SlkDataObject)value).Id;
                }
            }
            return value.ToString();
        }

        public abstract string Slk_Serialize();

        public abstract void Slk_DeSerialize(object data);
        /// <summary>
        /// 延迟反序列化-处理直接引用对象的属性反序列化
        /// </summary>
        /// <param name="data"></param>
        public virtual void Slk_LateDeSerialize(object data) { }
        /// <summary>
        /// 在Jass中对应的序列化形式
        /// </summary>
        /// <returns></returns>
        public abstract string GetJass();
    }

    public class SlkDataObjectFactory<T> where T : SlkDataObject
    {
        public static T Create()
        {
            var constructorInfo = typeof(T).GetConstructor(Type.EmptyTypes);
            return constructorInfo.Invoke(null) as T;
        }
    }

}
