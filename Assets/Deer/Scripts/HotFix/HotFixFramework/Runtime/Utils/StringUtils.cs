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

    public static bool IsNullOrEmpty(string value)
    {
        return string.IsNullOrEmpty(value);
    }
    /// <summary>  
    /// 过滤字符  
    /// </summary>  
    public static string Replace(string strOriginal, string oldchar, string newchar)
    {
        if (string.IsNullOrEmpty(strOriginal))
            return "";
        string tempChar = strOriginal;
        tempChar = tempChar.Replace(oldchar, newchar);

        return tempChar;
    }

    /**/
    /// <summary>  
    /// 过滤非法字符  
    /// </summary>  
    /// <param name="str"></param>  
    /// <returns></returns>  
    public static string ReplaceBadChar(string str)
    {
        if (string.IsNullOrEmpty(str))
            return "";
        string strBadChar, tempChar;
        string[] arrBadChar;
        strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\",";
        arrBadChar = SplitString(strBadChar, ",");
        tempChar = str;
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
    public static bool ChkBadChar(string str)
    {
        bool result = false;
        if (string.IsNullOrEmpty(str))
            return result;
        string strBadChar, tempChar;
        string[] arrBadChar;
        strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\"";
        arrBadChar = SplitString(strBadChar, ",");
        tempChar = str;
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
    public static string[] SplitString(string strContent, string strSplit)
    {
        if (string.IsNullOrEmpty(strContent))
        {
            return null;
        }
        int i = strContent.IndexOf(strSplit);
        if (strContent.IndexOf(strSplit) < 0)
        {
            string[] tmp = { strContent };
            return tmp;
        }
        //return Regex.Split(strContent, @strSplit.Replace(".", @"\."), RegexOptions.IgnoreCase);  

        return Regex.Split(strContent, @strSplit.Replace(".", @"\."));
    }
    /// <summary>
    /// 分割字符 返回 移除空字符串
    /// </summary>
    /// <param name="strContent"></param>
    /// <param name="strSplit"></param>
    /// <returns></returns>
    public static string[] SplitRemoveEmpty(string strContent, string strSplit)
    {
        string[] result = strContent.Split(new string[] { strSplit }, StringSplitOptions.RemoveEmptyEntries);
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
    /// <param name="strNumber">要确认的字符串</param>  
    /// <returns>是则返加true 不是则返回 false</returns>  
    public static bool IsNumber(string strNumber)
    {
        return new Regex(@"^([0-9])[0-9]*(\.\w*)?$").IsMatch(strNumber);
    }
    /**/
    /// <summary>  
    /// 检测是否符合email格式  
    /// </summary>  
    /// <param name="strEmail">要判断的email字符串</param>  
    /// <returns>判断结果</returns>  
    public static bool IsValidEmail(string strEmail)
    {
        return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((
                    [0−9]1,3\.[0−9]1,3\.[0−9]1,3\.)|(([\w−]+\.)+))([a−zA−Z]2,4|[0−9]1,3)(
                    ?)$");
    }
    /// <summary>  
    /// 检测是否符合url格式,前面必需含有http://  
    /// </summary>  
    /// <param name="url"></param>  
    /// <returns></returns>  
    public static bool IsURL(string url)
    {
        return Regex.IsMatch(url, @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$");
    }
    /// <summary>  
    /// 检测是否符合电话格式  
    /// </summary>  
    /// <param name="phoneNumber"></param>  
    /// <returns></returns>  
    public static bool IsPhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^(\d3|\d{3}-)?\d{7,8}$");
    }
    /// <summary>  
    /// 检测是否符合身份证号码格式  
    /// </summary>  
    /// <param name="num"></param>  
    /// <returns></returns>  
    public static bool IsIdentityNumber(string num)
    {
        return Regex.IsMatch(num, @"^\d{17}[\d|X]|\d{15}$");
    }
    /// <summary>
    /// 转换为小写
    /// </summary>
    /// <param name="strContent"></param>
    /// <returns></returns>
    public static string ToLowerCase(string strContent)
    {
        return strContent.Substring(0, 1).ToLower() + (strContent.Length > 1 ? strContent.Substring(1) : "");
    }
    public static string ToUpperFirst(this string str)
    {
        return Regex.Replace(str, @"^\w", t => t.Value.ToUpper()); ;
    }
    public static string ToToLowerFirst(this string str)
    {
        return Regex.Replace(str, @"^\w", t => t.Value.ToLower()); ;
    }
}
