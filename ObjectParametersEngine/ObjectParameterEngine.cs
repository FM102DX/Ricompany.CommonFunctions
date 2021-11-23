﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Ricompany.CommonFunctions;

namespace Ricompany.ObjectParametersEngine
{
    //пакет инструментов для присвоения значений полям/свойствам ообъекта, или их чтения

    public static class ObjectParameters
    {
        //дело в том, что у объекта есть field и property, и все его поля данных - они либо такие, либо такие
        //часть данных объекта храняться как филд, часть - как проперти
        //чтобы не перебирать филд и проепрти каждый раз в коде, делается objectParameter внутри которого и перебирается филд и проперти

        public static ObjectParameterOperationResult setObjectParameter(object x, string name, object value)
        {

            FieldInfo[] newObjectFields = x.GetType().GetFields();
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            Fn.CommonOperationResult convRez;

            foreach (FieldInfo f0 in newObjectFields)
            {
                if (f0.Name.ToLower() == name.ToLower())
                {
                    try
                    {
                        convRez = Fn.convertedObject(f0.FieldType.ToString(), value); //чтобы внутри object было значение нужно типа
                        if (convRez.success)
                        {
                            f0.SetValue(x, convRez.returningValue);
                            return ObjectParameterOperationResult.sayOk();
                        }
                        else
                        {
                            return ObjectParameterOperationResult.sayNo(convRez.msg);
                        }
                    }
                    catch
                    {
                        return ObjectParameterOperationResult.sayNo("Unknown error in ObjectParameters.setObjectParameter-1");
                    }
                }
            }

            foreach (PropertyInfo f1 in newObjectProperties)
            {
                if (f1.Name.ToLower() == name.ToLower())
                {
                    if (isItOnlyGetter(x, name)) return ObjectParameterOperationResult.sayOk();
                    try
                    {
                        convRez = Fn.convertedObject(f1.GetType().ToString(), value);
                        if (convRez.success)
                        {
                            f1.SetValue(x, convRez.returningValue);
                            return ObjectParameterOperationResult.sayOk();
                        }
                        else
                        {
                            return ObjectParameterOperationResult.sayNo(convRez.msg);
                        }

                    }
                    catch
                    {
                        return ObjectParameterOperationResult.sayNo("Unknown error in ObjectParameters.setObjectParameter-2");
                    }
                }
            }

            return ObjectParameterOperationResult.sayNo("Error in ObjectParameters.setObjectParameter: parameter not found");
        }
        public static ObjectParameter getObjectParameterByName(object x, string name)
        {
            ObjectParameter op = new ObjectParameter();
            op.name = name;

            FieldInfo[] newObjectFields = x.GetType().GetFields();
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            foreach (FieldInfo f0 in newObjectFields)
            {

                if (f0.Name.ToLower() == name.ToLower())
                {
                    op.value = f0.GetValue(x);
                    return op;
                }
            }

            foreach (PropertyInfo f1 in newObjectProperties)
            {
                if (f1.Name.ToLower() == name.ToLower())
                {
                    op.value = f1.GetValue(x);
                    return op;
                }
            }
            return null;
        }
        public static List<ObjectParameter> getObjectParameters(object x)
        {
            List<ObjectParameter> rez = new List<ObjectParameter>();

            FieldInfo[] newObjectFields = x.GetType().GetFields();
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            // 10/12/2020 - таким образом, экземпляр класса T, созданный в дженерике в рантайме, это экземпляр наследника, а не самого Т
            // их можно достать через fieldInfo и SetValue

            foreach (FieldInfo f0 in newObjectFields)  //это поля реального объекта, который только что был создан и будет добавлен в коллекцию
            {
                ObjectParameter op = new ObjectParameter();
                op.name = f0.Name;
                op.value = f0.GetValue(x);
                rez.Add(op);

            }

            foreach (PropertyInfo f1 in newObjectProperties)  //это поля реального объекта, который только что был создан и будет добавлен в коллекцию
            {
                ObjectParameter op = new ObjectParameter();
                op.name = f1.Name;
                op.value = f1.GetValue(x);
                rez.Add(op);
            }

            return rez;
        }

        public class ObjectParameter
        {
            public string name;
            public object value;
        }

        public class ObjectParameterOperationResult
        {
            //результат, возвращаемый после операций в объектном слое
            public bool success;
            public string msg;

            public static ObjectParameterOperationResult getInstance(bool _success, string _msg)
            {
                ObjectParameterOperationResult c = new ObjectParameterOperationResult();
                c.success = _success;
                c.msg = _msg;
                return c;
            }

            public static ObjectParameterOperationResult sayOk(string _msg = "") { return getInstance(true, _msg); }
            public static ObjectParameterOperationResult sayNo(string _msg = "") { return getInstance(false, _msg); }
        }

        public static bool isItOnlyGetter(object x, string fieldName)
        {
            PropertyInfo[] newObjectProperties = x.GetType().GetProperties();

            foreach (PropertyInfo f1 in newObjectProperties)
            {
                if (f1.Name.ToLower() == fieldName.ToLower())
                {
                    //вот он нашел это поле
                    return (f1.CanRead && (!f1.CanWrite));
                }
            }
            return false;

        }

        public static string getObjectDump(object x)
        {
            List<ObjectParameter> _params = getObjectParameters(x);

            StringBuilder sb = new StringBuilder();

            _params.ForEach(y => {

                sb.Append($"{y.name}={y.value};");

            });

            return sb.ToString();
        }

        public static void FillObject(object source, object target)
        {
            List<ObjectParameter> params0 = getObjectParameters(source);
            params0.ForEach(x=>setObjectParameter(target,x.name,x.value));
        }

        /*
        public void CallMetod(object x, string methodName)
        {
            MethodInfo objectMethod = x.GetType().GetMethods().Where(x=>x.Name== methodName).FirstOrDefault();
            if (objectMethod == null) return;
            objectMethod.;

        }

        */
    }
}
