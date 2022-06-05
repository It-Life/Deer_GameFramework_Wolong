using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

public static class NewtonsoftExtensions
{
    public static bool IsValidJson(string json)
    {
        json = json.Trim();
        if ((json.StartsWith("{") && json.EndsWith("}")) || //For object
            (json.StartsWith("[") && json.EndsWith("]"))) //For array
        {
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException jex)
            {
                //Exception in parsing json
                Debug.Log(jex.Message);
                return false;
            }
            catch (Exception ex) //some other exception
            {
                Debug.Log(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static void Rename(this JToken token, string newName)
    {
        var parent = token.Parent;
        if (parent == null)
            throw new InvalidOperationException("The parent is missing.");
        var newToken = new JProperty(newName, token);
        parent.Replace(newToken);
    }
}
