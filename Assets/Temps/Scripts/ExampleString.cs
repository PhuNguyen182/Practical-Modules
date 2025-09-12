using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Text;
using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Unity;

namespace PracticalModules.Temps.Scripts;
public class YourClass
{
    public void Foo()
    {
        Debug.Log("Hello, World!");
    }
}

public class ExampleString : MonoBehaviour
{
    private void TestString()
    {
        string formattedString = ZString.Format("Hello {0} {1}", 1, 2);
        string concatenatedString = ZString.Concat("Hello ", "World", "!");

        using (var sb = ZString.CreateUtf8StringBuilder())
        {
            sb.Append("sdfsdf");
            sb.AppendJoin(",", new List<string> { "1", "2", "3" });
            string result = sb.ToString();
        }
    }

    private void TestLogger()
    {
        var loggerFactory = LoggerFactory.Create(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddZLoggerUnityDebug(); // log to UnityDebug
        });

        var x = loggerFactory.CreateLogger("");
        var logger = loggerFactory.CreateLogger<YourClass>();
        //logger.ZLogDebug("Hello, World!");

        var name = "foo";
        logger.ZLogInformation($"Hello, {name}!");
    }
}
