// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-07-10 12-32-13  
//修改作者 : 杜鑫 
//修改时间 : 2021-07-10 12-32-13  
//版 本 : 0.1 
// ===============================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
public static class StringUtils
{
    public static int ToInt(this string value)
    {
        int result = 0;
        int.TryParse(value, out result);
        return result;
    }

    public static long ToLong(this string value)
    {
        long result = 0;
        long.TryParse(value, out result);
        return result;
    }

    public static float ToFloat(this string value)
    {
        float result = 0;
        float.TryParse(value, out result);
        return result;
    }

    public static double ToDouble(this string value)
    {
        double result = 0;
        double.TryParse(value, out result);
        return result;
    }

    public static short ToShort(this string value)
    {
        short result = 0;
        short.TryParse(value, out result);
        return result;
    }
    public static List<int> ToInts(this string value,string strSplit,int defValue = 0)
    {
        List<int> resultList = new List<int>();
        if (strSplit != null)
        {
            var results = value.Split(new string[] { strSplit },StringSplitOptions.RemoveEmptyEntries);
            int result = 0;
            foreach (var t in results)
            {
                result = defValue;
                int.TryParse(t, out result);
                resultList.Add(result);
            }
        }
        return resultList;
    }

    public static List<long> ToLongs(this string value,string strSplit,long defValue = 0)
    {
        List<long> resultList = new List<long>();
        if (strSplit != null)
        {
            var results = value.Split(new string[] { strSplit },StringSplitOptions.RemoveEmptyEntries);
            long result = 0;
            foreach (var t in results)
            {
                result = defValue;
                long.TryParse(t, out result);
                resultList.Add(result);
            }
        }
        return resultList;
    }

    public static List<float> ToFloats(this string value,string strSplit,float defValue = 0)
    {
        List<float> resultList = new List<float>();
        if (strSplit != null)
        {
            var results = value.Split(new string[] { strSplit },StringSplitOptions.RemoveEmptyEntries);
            float result = 0;
            foreach (var t in results)
            {
                result = defValue;
                float.TryParse(t, out result);
                resultList.Add(result);
            }
        }
        return resultList;
    }

    public static List<double> ToDoubles(this string value,string strSplit,double defValue = 0)
    {
        List<double> resultList = new List<double>();
        if (strSplit != null)
        {
            var results = value.Split(new string[] { strSplit },StringSplitOptions.RemoveEmptyEntries);
            double result = 0;
            foreach (var t in results)
            {
                result = defValue;
                double.TryParse(t, out result);
                resultList.Add(result);
            }
        }
        return resultList;
    }

    public static List<short> ToShorts(this string value,string strSplit,short defValue = 0)
    {
        List<short> resultList = new List<short>();
        if (strSplit != null)
        {
            var results = value.Split(new string[] { strSplit },StringSplitOptions.RemoveEmptyEntries);
            short result = 0;
            foreach (var t in results)
            {
                result = defValue;
                short.TryParse(t, out result);
                resultList.Add(result);
            }
        }
        return resultList;
    }
    public static List<string> ToStrings(this string value,string strSplit)
    {
        List<string> resultList = new List<string>();
        if (strSplit != null)
        {
            var results = value.Split(new string[] { strSplit },StringSplitOptions.RemoveEmptyEntries);
            resultList = results.ToList();
        }
        return resultList;
    }
    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }
    /// <summary>  
    /// 过滤字符  
    /// </summary>  
    public static string Replace(this string value, string oldchar, string newchar)
    {
        if (string.IsNullOrEmpty(value))
            return "";
        string tempChar = value;
        tempChar = tempChar.Replace(oldchar, newchar);

        return tempChar;
    }

    /**/
    /// <summary>  
    /// 过滤非法字符  
    /// </summary>  
    /// <param name="str"></param>  
    /// <returns></returns>  
    public static string ReplaceBadChar(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";
        string strBadChar, tempChar;
        string[] arrBadChar;
        strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\",";
        arrBadChar = strBadChar.SplitString(",");
        tempChar = value;
        for (int i = 0; i < arrBadChar.Length; i++)
        {
            if (arrBadChar[i].Length > 0)
                tempChar = tempChar.Replace(arrBadChar[i], "");
        }
        return tempChar;
    }


    /**/
    /// <summary>  
    /// 检查是否含有非法字符  
    /// </summary>  
    /// <param name="str">要检查的字符串</param>  
    /// <returns></returns>  
    public static bool ChkBadChar(this string value)
    {
        bool result = false;
        if (string.IsNullOrEmpty(value))
            return result;
        string strBadChar, tempChar;
        string[] arrBadChar;
        strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\"";
        arrBadChar = SplitString(strBadChar, ",");
        tempChar = value;
        for (int i = 0; i < arrBadChar.Length; i++)
        {
            if (tempChar.IndexOf(arrBadChar[i]) >= 0)
                result = true;
        }
        return result;
    }


    /**/
    /// <summary>  
    /// 分割字符串  
    /// </summary>  
    public static string[] SplitString(this string value, string strSplit)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        int i = value.IndexOf(strSplit);
        if (value.IndexOf(strSplit) < 0)
        {
            string[] tmp = { value };
            return tmp;
        }
        //return Regex.Split(strContent, @strSplit.Replace(".", @"\."), RegexOptions.IgnoreCase);  

        return Regex.Split(value, @strSplit.Replace(".", @"\."));
    }
    /// <summary>
    /// 分割字符 返回 移除空字符串
    /// </summary>
    /// <param name="strContent"></param>
    /// <param name="strSplit"></param>
    /// <returns></returns>
    public static string[] SplitRemoveEmpty(this string value, string strSplit)
    {
        string[] result = value.Split(new string[] { strSplit }, StringSplitOptions.RemoveEmptyEntries);
        return result;
    }
    /**/
    /// <summary>  
    /// string型转换为int型  
    /// </summary>  
    /// <param name="strValue">要转换的字符串</param>  
    /// <returns>转换后的int类型结果.如果要转换的字符串是非数字,则返回-1.</returns>  
    public static int StrToInt(string strValue)
    {
        int defValue = -1;
        if ((strValue == null) || (strValue.ToString() == string.Empty) || (strValue.ToString().Length > 10))
        {
            return defValue;
        }

        string val = strValue.ToString();
        string firstletter = val[0].ToString();

        if (val.Length == 10 && IsNumber(firstletter) && int.Parse(firstletter) > 1)
        {
            return defValue;
        }
        else if (val.Length == 10 && !IsNumber(firstletter))
        {
            return defValue;
        }


        int intValue = defValue;
        if (strValue != null)
        {
            bool IsInt = new Regex(@"^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString());
            if (IsInt)
            {
                intValue = Convert.ToInt32(strValue);
            }
        }

        return intValue;
    }

    /**/
    /// <summary>  
    /// string型转换为int型  
    /// </summary>  
    /// <param name="strValue">要转换的字符串</param>  
    /// <param name="defValue">缺省值</param>  
    /// <returns>转换后的int类型结果</returns>  
    public static int StrToInt(object strValue, int defValue)
    {
        if ((strValue == null) || (strValue.ToString() == string.Empty) || (strValue.ToString().Length > 10))
        {
            return defValue;
        }

        string val = strValue.ToString();
        string firstletter = val[0].ToString();

        if (val.Length == 10 && IsNumber(firstletter) && int.Parse(firstletter) > 1)
        {
            return defValue;
        }
        else if (val.Length == 10 && !IsNumber(firstletter))
        {
            return defValue;
        }


        int intValue = defValue;
        if (strValue != null)
        {
            bool IsInt = new Regex(@"^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString());
            if (IsInt)
            {
                intValue = Convert.ToInt32(strValue);
            }
        }

        return intValue;
    }



    /**/
    /// <summary>  
    /// string型转换为时间型  
    /// </summary>  
    /// <param name="strValue">要转换的字符串</param>  
    /// <param name="defValue">缺省值</param>  
    /// <returns>转换后的时间类型结果</returns>  
    public static DateTime StrToDateTime(object strValue, DateTime defValue)
    {
        if ((strValue == null) || (strValue.ToString().Length > 20))
        {
            return defValue;
        }

        DateTime intValue;

        if (!DateTime.TryParse(strValue.ToString(), out intValue))
        {
            intValue = defValue;
        }
        return intValue;
    }
    /// <summary>  
    /// 判断给定的字符串(strNumber)是否是数值型  
    /// </summary>  
    /// <param name="value">要确认的字符串</param>  
    /// <returns>是则返加true 不是则返回 false</returns>  
    public static bool IsNumber(this string value)
    {
        return new Regex(@"^([0-9])[0-9]*(\.\w*)?$").IsMatch(value);
    }
    /**/
    /// <summary>  
    /// 检测是否符合email格式  
    /// </summary>  
    /// <param name="value">要判断的email字符串</param>  
    /// <returns>判断结果</returns>  
    public static bool IsValidEmail(this string value)
    {
        return Regex.IsMatch(value, @"^([\w-\.]+)@((
                    [0−9]1,3\.[0−9]1,3\.[0−9]1,3\.)|(([\w−]+\.)+))([a−zA−Z]2,4|[0−9]1,3)(
                    ?)$");
    }
    /// <summary>  
    /// 检测是否符合url格式,前面必需含有http://  
    /// </summary>  
    /// <param name="value"></param>  
    /// <returns></returns>  
    public static bool IsURL(this string value)
    {
        return Regex.IsMatch(value, @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$");
    }
    /// <summary>  
    /// 检测是否符合电话格式  
    /// </summary>  
    /// <param name="value"></param>  
    /// <returns></returns>  
    public static bool IsPhoneNumber(this string value)
    {
        return Regex.IsMatch(value, @"^(\d3|\d{3}-)?\d{7,8}$");
    }
    /// <summary>  
    /// 检测是否符合身份证号码格式  
    /// </summary>  
    /// <param name="value"></param>  
    /// <returns></returns>  
    public static bool IsIdentityNumber(this string value)
    {
        return Regex.IsMatch(value, @"^\d{17}[\d|X]|\d{15}$");
    }
    /// <summary>
    /// 转换为小写
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToLowerCase(this string value)
    {
        return value.Substring(0, 1).ToLower() + (value.Length > 1 ? value.Substring(1) : "");
    }
    public static string ToUpperFirst(this string value)
    {
        return Regex.Replace(value, @"^\w", t => t.Value.ToUpper()); ;
    }
    public static string ToToLowerFirst(this string value)
    {
        return Regex.Replace(value, @"^\w", t => t.Value.ToLower()); ;
    }
}
